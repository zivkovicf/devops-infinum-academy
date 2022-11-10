output "s3_bucket_name" {
  value = aws_s3_bucket.bucket.id
}

output "s3_bucket_url" {
  value = format("https://%s", replace(aws_s3_bucket.bucket.bucket_regional_domain_name, "s3.amazonaws.com", "s3.${aws_s3_bucket.bucket.region}.amazonaws.com"))
}