using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace VideoProcessor.Masstransit;

public class VideoReceivedConsumer(
    ILogger<VideoReceivedConsumer> logger,
    IUseCase<VideoReceived> useCase)
    : IConsumer<VideoReceived>
{
    public Task Consume(ConsumeContext<VideoReceived> context)
    {
        return useCase.ExecuteAsync(context.Message);
    }
}