using Amazon.S3;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using VideoProcessor.FileManager;

namespace VideoProcessor.Tests.FileManager;

[TestSubject(typeof(S3Service))]
public class S3ServiceTest
{
    [Fact]
    public async Task DownloadFileAsync_LogsInformationAndDownloadsFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<S3Service>>();
        var s3ClientMock = new Mock<IAmazonS3>();
        var service = new S3Service(loggerMock.Object, s3ClientMock.Object);
        var fileKey = "test-file-key";
        var destinationFilePath = "test-destination-path";

        s3ClientMock.Setup(x =>
                x.DownloadToFilePathAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default, default))
            .Returns(Task.CompletedTask);

        // Act
        await service.DownloadFileAsync(fileKey, destinationFilePath);

        // Assert
        s3ClientMock.Verify(
            x => x.DownloadToFilePathAsync("hackathon-video-processor", fileKey, destinationFilePath, default, default),
            Times.Once);
    }

    [Fact]
    public void DeleteFile_LogsInformationAndDeletesFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<S3Service>>();
        var s3ClientMock = new Mock<IAmazonS3>();
        var service = new S3Service(loggerMock.Object, s3ClientMock.Object);
        var sourcePath = "test-source-path";

        // Act
        service.DeleteFile(sourcePath);

        // Assert
        Assert.False(File.Exists(sourcePath));
    }

    [Fact]
    public async Task SaveFileAsync_LogsInformationAndUploadsFile()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<S3Service>>();
        var s3ClientMock = new Mock<IAmazonS3>();
        var service = new S3Service(loggerMock.Object, s3ClientMock.Object);
        var filePath = "test-file-path";
        var fileKey = "test-file-key";

        s3ClientMock.Setup(x =>
                x.UploadObjectFromFilePathAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default,
                    default))
            .Returns(Task.CompletedTask);

        // Act
        await service.SaveFileAsync(filePath, fileKey);

        // Assert
        s3ClientMock.Verify(
            x => x.UploadObjectFromFilePathAsync("hackathon-video-processor", fileKey, filePath, default, default),
            Times.Once);
    }
}