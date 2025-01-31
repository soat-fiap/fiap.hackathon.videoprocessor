using System.Diagnostics.CodeAnalysis;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessor.Application.Services;

namespace VideoProcessor.FileManager;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddS3(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IFileStorageService, S3Service>()
            .AddScoped<IAmazonS3>(_ => new AmazonS3Client())
            .AddScoped<IZipService, ZipService>()
            .AddScoped<IVideoProcessor, VideoProcessor>();
    }
}