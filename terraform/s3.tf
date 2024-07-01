resource "aws_s3_bucket" "this" {
    bucket_prefix = "otel-aws-lambda-sample-"
    force_destroy = true
}