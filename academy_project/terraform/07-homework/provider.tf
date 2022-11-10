// https://developer.hashicorp.com/terraform/language/providers

terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.4.3"
    }
  }
}

// https://registry.terraform.io/providers/hashicorp/aws/latest/docs

provider "aws" {
  region = "us-east-1"
  default_tags {
    tags = {
      Owner     = "filip_zivkovic"
      Project   = "devops-academy"
      Terraform = "true"
    }
  }
}
