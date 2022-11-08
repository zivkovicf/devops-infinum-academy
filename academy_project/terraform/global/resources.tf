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
data "aws_iam_policy_document" "instance-assume-role-policy" {
  statement {
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ec2.amazonaws.com"]
    }
  }
}


