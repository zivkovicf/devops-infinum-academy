
module "vpc" {
  source             = "./vpc"
  cidr_block         = "10.0.0.0/16"
  enable_nat_gateway = false
}

module "rds" {
  source                   = "./rds"
  identifier               = "academyy"
  vpc_id                   = module.vpc.vpc_id
  database_username        = "academy"
  db_name                  = "movies"
  source_security_group_id = aws_security_group.allow_db_traffic.id
  subnet_ids               = module.vpc.subnets_private
  depends_on = [
    aws_security_group.allow_db_traffic,
    module.vpc
  ]
}

module "s3" {
  source      = "./s3"
  bucket_name = "my-academy-ecs-bucket"
}

resource "aws_security_group" "allow_db_traffic" {
  name        = "allow_trafic_to_db"
  description = "Allow traffic to db"
  vpc_id      = module.vpc.vpc_id
}
