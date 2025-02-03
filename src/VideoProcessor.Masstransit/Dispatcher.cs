using Hackathon.Video.SharedKernel;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace VideoProcessor.Masstransit;

public class Dispatcher(IBus bus, ILogger<Dispatcher> logger) : IDispatcher
{
    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            logger.LogInformation("Publishing event: {Event}", @event);
            await bus.Publish(@event, cancellationToken);
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Error when trying to publish event: {@Event}", @event);
        }
    }
}