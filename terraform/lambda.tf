resource "aws_lambda_function" "this" {
  function_name    = "otel-aws-lambda-sample"
  filename         = "..\\source\\OTelAwsLambdaSample.Lambda\\bin\\Release\\net8.0\\OTelAwsLambdaSample.Lambda.zip"
  source_code_hash = filebase64sha256("..\\source\\OTelAwsLambdaSample.Lambda\\bin\\Release\\net8.0\\OTelAwsLambdaSample.Lambda.zip")
  #source_code_hash = base64sha256(timestamp())
  role             = aws_iam_role.this.arn
  handler          = "OTelAwsLambdaSample.Lambda::OTelAwsLambdaSample.Lambda.DownloadFunction::FunctionHandler"
  runtime          = "dotnet8"
  architectures    = ["arm64"]
  memory_size      = 256
  timeout          = 60
  layers           = ["arn:aws:lambda:${data.aws_region.current.name}:901920570463:layer:aws-otel-collector-arm64-ver-0-98-0:5"]

  tracing_config {
    mode = "Active"
  }

  environment {
    variables = {
      S3_BUCKET_NAME = aws_s3_bucket.this.id
    }
  }
}

resource "aws_cloudwatch_log_group" "this" {
  name              = "/aws/lambda/${aws_lambda_function.this.id}"
  retention_in_days = 7
}


data "aws_iam_policy_document" "lambda-assume-role" {
  statement {
    effect = "Allow"
    principals {
      type        = "Service"
      identifiers = ["lambda.amazonaws.com"]
    }
    actions = ["sts:AssumeRole"]
  }
}


resource "aws_iam_role" "this" {
  name               = "otel-aws-lambda-sample"
  assume_role_policy = data.aws_iam_policy_document.lambda-assume-role.json
}

resource "aws_iam_role_policy_attachment" "cloudwatch" {
  role       = aws_iam_role.this.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
}

resource "aws_iam_role_policy_attachment" "xray" {
  role       = aws_iam_role.this.name
  policy_arn = "arn:aws:iam::aws:policy/AWSXrayWriteOnlyAccess"
}

resource "aws_iam_role_policy_attachment" "s3" {
  role       = aws_iam_role.this.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonS3FullAccess"
}


