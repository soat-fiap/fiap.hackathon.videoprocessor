using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using JetBrains.Annotations;
using Moq;
using VideoProcessor.Application.Services;
using VideoProcessor.Application.UseCases;

namespace VideoProcessor.Tests.UseCases;

[TestSubject(typeof(ExtractSnapshotsUseCase))]
public class ExtractSnapshotsUseCaseTest
{
    [Fact]
    public async Task ExecuteAsync_VideoProcessingCompletesSuccessfully()
    {
        // Arrange
        var fileStorageServiceMock = new Mock<IFileStorageService>();
        var videoProcessorMock = new Mock<IVideoProcessor>();
        var zipServiceMock = new Mock<IZipService>();
        var dispatcherMock = new Mock<IDispatcher>();
        var useCase = new ExtractSnapshotsUseCase(fileStorageServiceMock.Object, videoProcessorMock.Object,
            zipServiceMock.Object, dispatcherMock.Object);
        var request = new VideoReceived("bucket", Guid.NewGuid(), Guid.NewGuid(), 10);

        fileStorageServiceMock.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        videoProcessorMock
            .Setup(x => x.ExtractSnapshotsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "snapshot1.png", "snapshot2.png" });
        zipServiceMock.Setup(x => x.CreateZipAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        fileStorageServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        dispatcherMock.Setup(x => x.PublishAsync(It.IsAny<object>(), default))
            .Returns(Task.CompletedTask);

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<object>(), default),
            Times.AtLeast(2));
    }

    [Fact]
    public async Task ExecuteAsync_VideoProcessingFails()
    {
        // Arrange
        var fileStorageServiceMock = new Mock<IFileStorageService>();
        var videoProcessorMock = new Mock<IVideoProcessor>();
        var zipServiceMock = new Mock<IZipService>();
        var dispatcherMock = new Mock<IDispatcher>();
        var useCase = new ExtractSnapshotsUseCase(fileStorageServiceMock.Object, videoProcessorMock.Object,
            zipServiceMock.Object, dispatcherMock.Object);
        var request = new VideoReceived("bucket", Guid.NewGuid(), Guid.NewGuid(), 10);

        fileStorageServiceMock.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("Download failed"));

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        dispatcherMock.Verify(x => x.PublishAsync(It.IsAny<object>(), default),
            Times.AtLeast(2));
    }

    [Fact]
    public async Task ExecuteAsync_CleansUpFilesAfterProcessing()
    {
        // Arrange
        var fileStorageServiceMock = new Mock<IFileStorageService>();
        var videoProcessorMock = new Mock<IVideoProcessor>();
        var zipServiceMock = new Mock<IZipService>();
        var dispatcherMock = new Mock<IDispatcher>();
        var useCase = new ExtractSnapshotsUseCase(fileStorageServiceMock.Object, videoProcessorMock.Object,
            zipServiceMock.Object, dispatcherMock.Object);
        var request = new VideoReceived("bucket", Guid.NewGuid(), Guid.NewGuid(), 10);

        fileStorageServiceMock.Setup(x => x.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        videoProcessorMock
            .Setup(x => x.ExtractSnapshotsAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync(new List<string> { "snapshot1.png", "snapshot2.png" });
        zipServiceMock.Setup(x => x.CreateZipAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        fileStorageServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        dispatcherMock.Setup(x => x.PublishAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await useCase.ExecuteAsync(request);

        // Assert
        fileStorageServiceMock.Verify(x => x.DeleteFile(It.IsAny<string>()), Times.AtLeastOnce);
    }
}