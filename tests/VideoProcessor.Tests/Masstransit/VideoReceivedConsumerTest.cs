using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessor.Masstransit;

namespace VideoProcessor.Tests.Masstransit;

[TestSubject(typeof(VideoReceivedConsumer))]
public class VideoReceivedConsumerTest
{
    [Fact]
    public async Task Consume_PublishesVideoProcessingStarted()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VideoReceivedConsumer>>();
        var useCaseMock = new Mock<IUseCase<VideoReceived>>();
        var dispatcherMock = new Mock<IDispatcher>();
        var consumer = new VideoReceivedConsumer(loggerMock.Object, useCaseMock.Object, dispatcherMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoReceived>>();
        var message = new VideoReceived("bucket", Guid.NewGuid(), Guid.NewGuid(), 10);
        contextMock.SetupGet(x => x.Message).Returns(message);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        dispatcherMock.Verify(
            x => x.PublishAsync(It.Is<VideoProcessingStarted>(v => v.UserId != Guid.Empty && v.JobId != Guid.Empty),
                default),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ExecutesUseCase()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<VideoReceivedConsumer>>();
        var useCaseMock = new Mock<IUseCase<VideoReceived>>();
        var dispatcherMock = new Mock<IDispatcher>();
        var consumer = new VideoReceivedConsumer(loggerMock.Object, useCaseMock.Object, dispatcherMock.Object);
        var contextMock = new Mock<ConsumeContext<VideoReceived>>();
        var message = new VideoReceived("bucket", Guid.NewGuid(), Guid.NewGuid(), 10);
        contextMock.SetupGet(x => x.Message).Returns(message);

        // Act
        await consumer.Consume(contextMock.Object);

        // Assert
        useCaseMock.Verify(
            x => x.ExecuteAsync(It.Is<VideoReceived>(v => v.UserId != Guid.Empty && v.JobId != Guid.Empty)),
            Times.Once);
    }
}