output "ssm_password_name" {
  value = aws_ssm_parameter.rds_password.name
}

output "url" {
  value = aws_db_instance.postgres_db.address
}
