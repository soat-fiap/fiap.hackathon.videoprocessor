using Amazon.S3;
using Microsoft.Extensions.Logging;
using VideoProcessor.Application.Services;

namespace VideoProcessor.FileManager;

public class S3Service(ILogger<S3Service> logger, IAmazonS3 s3Client) : IFileStorageService
{
    private const string Bucket = "hackathon-video-processor";

    public Task DownloadFileAsync(string fileKey, string destinationFilePath)
    {
        logger.LogInformation($"Downloading file {fileKey} from {Bucket} to {destinationFilePath}");
        return s3Client.DownloadToFilePathAsync(Bucket, fileKey, destinationFilePath, default);
    }

    public void DeleteFile(string sourcePath)
    {
        logger.LogInformation($"Deleting file {sourcePath}");
        File.Delete(sourcePath);
    }

    public async Task SaveFileAsync(string filePath, string fileKey)
    {
        logger.LogInformation($"Uploading file {filePath} to {fileKey} on {Bucket} bucket.");
        await s3Client.UploadObjectFromFilePathAsync(Bucket, fileKey, filePath, default, default);
    }
}