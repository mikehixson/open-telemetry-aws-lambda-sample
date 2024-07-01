using Amazon.Lambda.TestUtilities;

namespace OTelAwsLambdaSample.Lambda.Test
{
    public class DownloadFunctionTests
    {
        [Fact]
        public async void Test1()
        {
            // Update with your bucket name
            Environment.SetEnvironmentVariable("S3_BUCKET_NAME", "otel-aws-lambda-sample-20240701000734403700000001");

            var function = new DownloadFunction();
            await function.FunctionHandler(new DownloadRequest(Guid.Empty, "https://www.nasa.gov/wp-content/uploads/2024/06/potw2425a.jpg"), new TestLambdaContext());
        }
    }
}