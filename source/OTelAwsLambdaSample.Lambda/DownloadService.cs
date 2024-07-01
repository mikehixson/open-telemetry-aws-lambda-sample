using Amazon.S3;
using Microsoft.Extensions.Configuration;

namespace OTelAwsLambdaSample.Lambda
{
    internal class DownloadService(HttpClient http, IAmazonS3 s3, IConfiguration configuration)
    {
        private readonly HttpClient _http = http;
        private readonly IAmazonS3 _s3 = s3;
        private readonly string _bucketName = configuration["S3_BUCKET_NAME"]!;

        public async Task Download(DownloadRequest request)
        {
            var httpResponse = await _http.GetAsync(request.Url);

            httpResponse.EnsureSuccessStatusCode();

            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            {
                var properties = new Dictionary<string, object>();

                if (httpResponse.Content.Headers.ContentType != null)
                {
                    properties.Add("ContentType", httpResponse.Content.Headers.ContentType.ToString());
                };
                
                await _s3.UploadObjectFromStreamAsync(_bucketName, request.Id.ToString(), stream, properties);
            }
        }
    }
}
