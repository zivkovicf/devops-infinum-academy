/////////////////
//  BEGIN DATA
////////////////

data "aws_iam_policy_document" "instance-assume-role-policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}

data "aws_iam_policy_document" "ecs-task-assume-role-policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}


/////////////////
//  END DATA
////////////////


////////////////////
//  BEGIN RESOURCES
////////////////////

resource "aws_ecr_repository" "academy_ecr" {
  name                 = "academy"
  image_tag_mutability = "MUTABLE"
}

resource "aws_iam_role" "EC2FullAccesToECR" {
  name                = "EC2FullAccesToECR"
  description         = "Allows EC2 instances to call AWS services on your behalf."
  assume_role_policy  = data.aws_iam_policy_document.instance-assume-role-policy.json
  managed_policy_arns = ["arn:aws:iam::aws:policy/AmazonEC2ContainerRegistryFullAccess", "arn:aws:iam::aws:policy/AmazonS3FullAccess"]
}

resource "aws_iam_role" "ecsTaskExecutionPolicy" {
  name               = "ecsTaskExecutionPolicy"
  description        = "Allows ECS tasks to call AWS services on your behalf."
  assume_role_policy = data.aws_iam_policy_document.ecs-task-assume-role-policy.json
  managed_policy_arns = [
    "arn:aws:iam::aws:policy/AmazonS3FullAccess",
    "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy",
    "arn:aws:iam::aws:policy/AmazonSSMReadOnlyAccess",
    aws_iam_policy.readSSMParameterStore.arn
  ]
}

resource "aws_iam_policy" "readSSMParameterStore" {
  name = "readSSMParameterStore"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Action   = ["ssm:GetParameters"]
        Effect   = "Allow"
        Resource = "*"
      },
    ]
  })
}



resource "aws_cloudwatch_metric_alarm" "billing_alarm" {
  alarm_name                = "Billing alarm"
  comparison_operator       = "GreaterThanThreshold"
  evaluation_periods        = "1"
  metric_name               = "EstimatedCharges"
  namespace                 = "AWS/Billing"
  period                    = "21600"
  statistic                 = "Maximum"
  threshold                 = "50"
  alarm_description         = "Billing exceeding 50 USD"
  insufficient_data_actions = []
  datapoints_to_alarm       = 1
  alarm_actions             = ["arn:aws:sns:us-east-1:481078911083:server-admin"]
  dimensions = {
    Currency = "USD"
  }
}

resource "aws_iam_user" "ecr" {
  name = "ecr"
}

resource "aws_iam_policy" "ecr_rw" {
  name        = "allowEcrPushPull"
  description = "Allows push and pulling inmages from academy repo"
  policy      = <<EOF
{
  "Version": "2012-10-17",
  "Statement": [
      {
          "Sid": "AllowPushPull",
          "Effect": "Allow",
          "Action": [
              "ecr:BatchGetImage",
              "ecr:BatchCheckLayerAvailability",
              "ecr:CompleteLayerUpload",
              "ecr:GetDownloadUrlForLayer",
              "ecr:InitiateLayerUpload",
              "ecr:PutImage",
              "ecr:UploadLayerPart"
          ],
          "Resource": "${aws_ecr_repository.academy_ecr.arn}"
      },
      {
          "Sid": "AllowAuthorizationToken",
          "Effect": "Allow",
          "Action": "ecr:GetAuthorizationToken",
          "Resource": "*"
      }
  ]
}
  EOF
}

resource "aws_iam_user_policy_attachment" "ecr_rw_attachment" {
  user       = aws_iam_user.ecr.name
  policy_arn = aws_iam_policy.ecr_rw.arn
}

resource "aws_iam_access_key" "ecr_acces_key" {
  user = aws_iam_user.ecr.name
}

resource "aws_ssm_parameter" "ecr_secret_key" {
  name        = "/terraform/iam/ecr/secret_key"
  description = "Secret key for acces key ID"
  type        = "SecureString"
  value       = aws_iam_access_key.ecr_acces_key.secret
}
