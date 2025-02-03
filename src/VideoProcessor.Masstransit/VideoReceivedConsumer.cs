using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace VideoProcessor.Masstransit;

public class VideoReceivedConsumer(
    ILogger<VideoReceivedConsumer> logger,
    IUseCase<VideoReceived> useCase,
    IDispatcher dispatcher)
    : IConsumer<VideoReceived>
{
    public async Task Consume(ConsumeContext<VideoReceived> context)
    {
        using var scope = logger.BeginScope("Processing   {JobId} for {UserId}", context.Message.JobId,
            context.Message.UserId);
        await dispatcher.PublishAsync(new VideoProcessingStarted(context.Message.UserId, context.Message.JobId));
        await useCase.ExecuteAsync(context.Message);
    }
}