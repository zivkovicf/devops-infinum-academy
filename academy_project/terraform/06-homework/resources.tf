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
  vpc_security_group_ids = [aws_security_group.allow_ssh.id]
  depends_on = [
    aws_key_pair.developer-key
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


variable "vpc_id" {
  description = "ID of default VPC"
  type        = string
}
variable "ssh_key" {
  description = "Public ssh key used for connecting to ec2"
  type        = string
}
