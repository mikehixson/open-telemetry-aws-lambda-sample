# open-telemetry-aws-lambda-sample

A Sample that shows how to setup a .Net Lambda Function with OpenTelemetry.

## Sample Request

Invoke the lambda with the following request.

```json
{
    "Id": "93bc038a-211a-4677-b0ca-89f9c8a5fe7b",
    "Url": "https://www.nasa.gov/wp-content/uploads/2024/06/ksc-20240625-ph-spx01-0009orig.jpg"
}
```
The content will be downloaded to S3 using the Id as a key.
