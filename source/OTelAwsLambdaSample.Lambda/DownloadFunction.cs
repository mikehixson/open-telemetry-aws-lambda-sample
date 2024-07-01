using Amazon.Lambda.Core;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AWSLambda;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using Amazon.S3;


[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace OTelAwsLambdaSample.Lambda;

public class DownloadFunction
{
    private readonly ServiceProvider _serviceProvider;

    public DownloadFunction()
    {
        var services = new ServiceCollection();

        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService("OTelAwsLambdaSample.Lambda");
            })
            .WithTracing(tracing =>
            {                            
                tracing.AddAWSLambdaConfigurations();                
                tracing.AddAWSInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddOtlpExporter();
                //tracing.AddConsoleExporter(u => u.Targets = OpenTelemetry.Exporter.ConsoleExporterOutputTargets.Debug);
            });

        services.AddHttpClient();
        services.AddAWSService<IAmazonS3>();
        services.AddSingleton<DownloadService>();

        _serviceProvider = services.BuildServiceProvider();
    }


    public async Task FunctionHandler(DownloadRequest request, ILambdaContext context)
    {        
        var tracerProvider = _serviceProvider.GetRequiredService<TracerProvider>();
        
        await AWSLambdaWrapper.TraceAsync(tracerProvider, InternalFunctionHandler, request, context);        
    }

    private async Task InternalFunctionHandler(DownloadRequest request, ILambdaContext context)
    {
        var service = _serviceProvider.GetRequiredService<DownloadService>();

        await service.Download(request);
    }

    // Without OpenTelemetry
    //public async Task FunctionHandler(Request request, ILambdaContext context)
    //{
    //    var service = _serviceProvider.GetRequiredService<DownloadService>();

    //    await service.Download(request);
    //}
}