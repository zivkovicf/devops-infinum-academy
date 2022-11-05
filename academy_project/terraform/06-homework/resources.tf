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

  })

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
