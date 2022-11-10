variable "vpc_id" {
  type = string
}

variable "source_security_group_id" {
  type = string
}

variable "instance_class" {
  type    = string
  default = "db.t3.micro"
}


variable "allocated_storage" {
  type    = number
  default = 5

  validation {
    condition     = var.allocated_storage < 20
    error_message = "Allocated storage too big."
  }
}

variable "database_username" {
  type = string
}

variable "db_name" {
  type = string
}

variable "identifier" {
  type = string
}

variable "subnet_ids" {
  type = list(string)
}
