resource "aws_db_instance" "postgres_db" {
  identifier             = var.identifier
  instance_class         = var.instance_class
  allocated_storage      = 5
  engine                 = "postgres"
  engine_version         = "14"
  username               = var.database_username
  db_name                = var.db_name
  password               = random_password.rds_password.result
  publicly_accessible    = false
  skip_final_snapshot    = true
  vpc_security_group_ids = [aws_security_group.db_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.db_subnet_group.name
}

resource "aws_security_group" "db_sg" {
  name        = "allow_traffic_to_rds"
  description = "Allow inbound traffic"
  vpc_id      = var.vpc_id
}

resource "aws_security_group_rule" "db_sg_rule" {
  type                     = "ingress"
  security_group_id        = aws_security_group.db_sg.id
  from_port                = 5432
  to_port                  = 5432
  protocol                 = "tcp"
  source_security_group_id = var.source_security_group_id
}

resource "aws_db_subnet_group" "db_subnet_group" {
  name        = "db_subnet_group"
  description = "DB subnet group"
  subnet_ids  = var.subnet_ids
}

resource "random_password" "rds_password" {
  length  = 16
  special = false
}

resource "aws_ssm_parameter" "rds_password" {
  name        = "/terraform/db/${aws_db_instance.postgres_db.identifier}/password"
  description = "Password for database access"
  type        = "SecureString"
  value       = random_password.rds_password.result
}


resource "aws_ssm_parameter" "rds_connection_string" {
  name        = "/terraform/db/${aws_db_instance.postgres_db.identifier}/connection_string"
  description = "Connection string for application"
  type        = "SecureString"
  value       = "Host=${aws_db_instance.postgres_db.address};Username=${var.database_username};Database=${var.db_name};Password=${random_password.rds_password.result}"
}

