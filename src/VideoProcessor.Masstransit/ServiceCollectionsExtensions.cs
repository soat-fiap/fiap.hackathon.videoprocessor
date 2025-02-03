using System.Diagnostics.CodeAnalysis;
using Hackathon.Video.SharedKernel;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VideoProcessor.Masstransit;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddVideoProcessingBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<VideoReceivedConsumer>();
            bus.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    // h.Scope("dev", true);
                });
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(false));
            });

            bus.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("health");
            });
        });

        services.AddScoped<IDispatcher, Dispatcher>();
    }
}