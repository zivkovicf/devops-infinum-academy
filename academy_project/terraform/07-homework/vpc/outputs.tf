output "vpc_id" {
  value = aws_vpc.main.id
}

output "subnets_public" {
  value = aws_subnet.public_subnet.*.id
}

output "subnets_private" {
  value = aws_subnet.private_subnet.*.id
}
