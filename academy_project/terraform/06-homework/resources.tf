/////////////////
//  BEGIN DATA
////////////////
data "aws_ami" "ubuntu" {
  most_recent = true

  filter {
    name   = "name"
    values = ["ubuntu/images/hvm-ssd/ubuntu-focal-20.04-amd64-server-*"]
  }

  filter {
    name   = "virtualization-type"
    values = ["hvm"]
  }

  owners = ["099720109477"] # Canonical
}

data "aws_vpc" "main" {
  id = var.vpc_id
}

data "aws_iam_role" "EC2FullAccesToECR" {
  name = "EC2FullAccesToECR"
}

data "aws_ecr_repository" "academy" {
  name = "academy"
}


/////////////////
//  END DATA
////////////////

////////////////////
//  BEGIN RESOURCES
///////////////////


resource "aws_instance" "web" {
  ami                    = data.aws_ami.ubuntu.id
  instance_type          = "t3.micro"
  key_name               = aws_key_pair.developer-key.key_name
  vpc_security_group_ids = [aws_security_group.allow_ssh.id, aws_security_group.allow_traffic_to_ec2.id]
  user_data = templatefile("init.sh", {
    database_url             = aws_db_instance.postgres_db.address
    database_username        = var.database_username
    database_password        = random_password.rds_password.result
    database_master_username = aws_db_instance.postgres_db.username
    database_master_password = random_password.rds_master_password.result
    ecr_repository_uri       = data.aws_ecr_repository.academy.repository_url
    connection_string        = aws_ssm_parameter.connection_string.value
    s3_name                  = aws_ssm_parameter.s3_bucket_name.value
    s3_url                   = aws_ssm_parameter.s3_bucket_url.value
    s3_region                = aws_s3_bucket.bucket.region
  })
  iam_instance_profile = aws_iam_instance_profile.ec2_profile.name

  depends_on = [
    aws_key_pair.developer-key,
    aws_db_instance.postgres_db
  ]
}


resource "aws_key_pair" "developer-key" {
  key_name   = "developer-key"
  public_key = var.ssh_key
}



resource "aws_security_group" "allow_ssh" {
  name        = "allow_ssh"
  description = "Allow SSH inbound traffic"
  vpc_id      = data.aws_vpc.main.id

  ingress {
    description = "SSH from VPC"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_security_group" "allow_traffic_to_ec2" {
  name        = "allow_traffic_to_ec2"
  description = "Allow EC2 inbound traffic"
  vpc_id      = data.aws_vpc.main.id
}

resource "aws_security_group_rule" "allow_traffic_to_ec2" {
  type              = "ingress"
  security_group_id = aws_security_group.allow_traffic_to_ec2.id
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
}

resource "aws_security_group" "allow_ec2_to_rds" {
  name        = "allow_ec2_to_rds"
  description = "Allow EC2 inbound traffic"
  vpc_id      = data.aws_vpc.main.id
}

resource "aws_security_group_rule" "allow_ec2_to_rds" {
  type                     = "ingress"
  security_group_id        = aws_security_group.allow_ec2_to_rds.id
  from_port                = 5432
  to_port                  = 5432
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.allow_ssh.id
}

resource "random_password" "rds_password" {
  length  = 16
  special = false
}
resource "random_password" "rds_master_password" {
  length  = 16
  special = false
}

resource "aws_iam_instance_profile" "ec2_profile" {
  name = "ec2_profile"
  role = data.aws_iam_role.EC2FullAccesToECR.name
}

resource "aws_db_instance" "postgres_db" {
  identifier             = "db"
  instance_class         = "db.t3.micro"
  allocated_storage      = 5
  engine                 = "postgres"
  engine_version         = "14"
  username               = "academy"
  password               = random_password.rds_master_password.result
  publicly_accessible    = false
  skip_final_snapshot    = true
  vpc_security_group_ids = [aws_security_group.allow_ec2_to_rds.id]
}

resource "aws_ssm_parameter" "rds_master_password" {
  name        = "/terraform/database/master_password"
  description = "Master password for database access"
  type        = "SecureString"
  value       = random_password.rds_master_password.result
}


resource "aws_ssm_parameter" "rds_application_password" {
  name        = "/terraform/database/application_password"
  description = "Password for database access for application user"
  type        = "SecureString"
  value       = random_password.rds_password.result
}

resource "aws_ssm_parameter" "connection_string" {
  name        = "/terraform/application/connection_string"
  description = "Application connection string to database"
  type        = "SecureString"
  value       = "Host=${aws_db_instance.postgres_db.address};Username=${var.database_username};Database=movies;Password=${random_password.rds_password.result}"
}

resource "aws_s3_bucket" "bucket" {
}
resource "aws_s3_bucket_public_access_block" "example" {
  bucket = aws_s3_bucket.bucket.id

  block_public_acls       = true
  block_public_policy     = false
  ignore_public_acls      = true
  restrict_public_buckets = false
}

resource "aws_s3_bucket_versioning" "versioning_example" {
  bucket = aws_s3_bucket.bucket.id
  versioning_configuration {
    status = "Enabled"
  }
}

resource "aws_ssm_parameter" "s3_bucket_name" {
  name        = "/terraform/s3/name"
  description = "Name(id) of the s3 bucket"
  type        = "String"
  value       = aws_s3_bucket.bucket.id
}
//hacky beacuse this issue is only for region us-east-1 , according to this GH issue https://github.com/hashicorp/terraform-provider-aws/issues/15102
resource "aws_ssm_parameter" "s3_bucket_url" {
  name        = "/terraform/s3/url"
  description = "Url of the s3 bucket"
  type        = "String"
  value       = format("https://%s", replace(aws_s3_bucket.bucket.bucket_regional_domain_name, "s3.amazonaws.com", "s3.${aws_s3_bucket.bucket.region}.amazonaws.com"))
}


//////////////////
//  END RESOURCES
/////////////////

////////////////////
//  BEGIN VARIABLES
///////////////////


variable "vpc_id" {
  description = "ID of default VPC"
  type        = string
}
variable "ssh_key" {
  description = "Public ssh key used for connecting to ec2"
  type        = string
}

variable "database_username" {
  description = "Name of application user to connect to db"
  type        = string
  default     = "application_user"
}


//////////////////
//  END VARIABLES
/////////////////

