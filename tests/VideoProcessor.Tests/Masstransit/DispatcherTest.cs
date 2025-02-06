using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessor.Masstransit;

namespace VideoProcessor.Tests.Masstransit;

[TestSubject(typeof(Dispatcher))]
public class DispatcherTest
{

    [Fact]
    public async Task PublishAsync_PublishesEventSuccessfully()
    {
        var busMock = new Mock<IBus>();
        var loggerMock = new Mock<ILogger<Dispatcher>>();
        var dispatcher = new Dispatcher(busMock.Object, loggerMock.Object);
        var @event = new VideoProcessingCompleted(Guid.NewGuid(), Guid.NewGuid()); 

        await dispatcher.PublishAsync(@event, default);

        busMock.Verify(b => b.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}