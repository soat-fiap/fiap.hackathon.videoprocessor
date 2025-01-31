using System.Diagnostics.CodeAnalysis;
using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using Microsoft.Extensions.DependencyInjection;
using VideoProcessor.Application.UseCases;

namespace VideoProcessor.Application;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddUseCases(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IUseCase<VideoReceived>, ExtractSnapshotsUseCase>();
    }
}