data "aws_iam_role" "task_role" {
  name = "ecsTaskFullAccesToS3"
}

data "aws_iam_role" "task_execution_role" {
  name = "ecsTaskExecutionPolicy"
}

data "aws_ecr_repository" "academy_ecr" {
  name = "academy"
}


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

resource "aws_security_group" "lb_sg" {
  name        = "lb_sg"
  description = "Security group for load balancer"
  vpc_id      = module.vpc.vpc_id
}

resource "aws_security_group_rule" "allow_inb_traffic_lb" {
  type              = "ingress"
  security_group_id = aws_security_group.lb_sg.id
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
}

resource "aws_security_group_rule" "allow_egress_to_ecs" {
  type                     = "egress"
  security_group_id        = aws_security_group.lb_sg.id
  from_port                = 80
  to_port                  = 80
  protocol                 = "tcp"
  source_security_group_id = aws_security_group.allow_db_traffic.id
}

resource "aws_security_group_rule" "allow_ecs_from_lb" {
  type                     = "ingress"
  security_group_id        = aws_security_group.allow_db_traffic.id
  source_security_group_id = aws_security_group.lb_sg.id
  from_port                = 80
  to_port                  = 80
  protocol                 = "tcp"
}

resource "aws_security_group_rule" "allow_inbound_traffic" {
  type              = "ingress"
  security_group_id = aws_security_group.allow_db_traffic.id
  from_port         = 80
  to_port           = 80
  protocol          = "tcp"
  cidr_blocks       = ["0.0.0.0/0"]
}

resource "aws_ecs_task_definition" "task_definition" {
  family                   = "academy"
  task_role_arn            = data.aws_iam_role.task_role.arn
  execution_role_arn       = data.aws_iam_role.task_execution_role.arn
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"
  cpu                      = 512
  memory                   = 1024
  container_definitions    = jsonencode([
  {
    name              = "app",
    image             = "${data.aws_ecr_repository.academy_ecr.repository_url}:latest",
    cpu               = 512,
    memory            = 256,
    memoryReservation = 256,
    essential         = true,
    portMappings = [
      {
        containerPort = 80,
        hostPort      = 80,
        protocol      = "tcp"
      }
    ],
    healthCheck = {
      command     = ["CMD-SHELL","echo works"],
      interval    = 30,
      timeout     = 5,
      startPeriod = 20,
      retries     = 5
    },
    environment = [
      {name = "AWS__S3__Region", value = "us-east-1" },
      {name = "AWS__S3__BucketName", value = "${module.s3.s3_bucket_name}" },
      {name = "AWS__S3__ServiceURL", value = "${module.s3.s3_bucket_url}" }
    ],
    secrets = [
      {valueFrom = "${module.rds.ssm_connection_string}", name = "ConnectionStrings__MoviesDb"}
    ]
  }
])

  runtime_platform {
    operating_system_family = "LINUX"
  }

}

resource "aws_ecs_cluster" "academy_cluster" {
  name = "academy-cluster"
}

resource "aws_ecs_service" "ecs_service" {
  name                               = "app-lb-pub-ssm"
  cluster                            = aws_ecs_cluster.academy_cluster.id
  task_definition                    = aws_ecs_task_definition.task_definition.arn
  scheduling_strategy                = "REPLICA"
  launch_type                        = "FARGATE"
  desired_count                      = 2
  deployment_minimum_healthy_percent = 100
  deployment_maximum_percent         = 200
  enable_ecs_managed_tags            = true
  propagate_tags                     = "NONE"
  network_configuration {
    subnets          = module.vpc.subnets_public
    security_groups  = [aws_security_group.allow_db_traffic.id]
    assign_public_ip = true
  }
  load_balancer {
    container_name   = "app"
    container_port   = 80
    target_group_arn = aws_lb_target_group.lb_tg.arn
  }
  depends_on = [
    aws_lb_listener.alb_listener
  ]
}

resource "aws_lb_target_group" "lb_tg" {
  name        = "academytg-123"
  port        = 80
  protocol    = "HTTP"
  vpc_id      = module.vpc.vpc_id
  target_type = "ip"
  stickiness {
    enabled = true
    type    = "lb_cookie"
  }
}

resource "aws_lb" "lb" {
  name               = "application-lb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.lb_sg.id]
  subnets            = module.vpc.subnets_public

  enable_deletion_protection = true
}

resource "aws_lb_listener" "alb_listener" {
  load_balancer_arn = aws_lb.lb.arn
  port              = "80"
  protocol          = "HTTP"
  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.lb_tg.arn
  }
}
