// https://developer.hashicorp.com/terraform/language/providers

terraform {
  required_providers {
    aws = {
      source = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

// https://registry.terraform.io/providers/hashicorp/aws/latest/docs

provider "aws" {
  region = "eu-central-1"
  default_tags {
    tags = {
        Owner = "REPLACE_THIS_WITH_YOUR_NAME_AND_SURNAME"
        Project = "devops-academy"
    }
  }
}