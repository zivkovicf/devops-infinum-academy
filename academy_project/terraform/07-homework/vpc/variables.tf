variable "cidr_block" {
  type        = string
  description = "CIDR block IP address for VPC"
}

variable "enable_nat_gateway" {
  description = "Should be true if we want to provision NAT Gateway/s at all"
  type        = bool
  default     = true
}
