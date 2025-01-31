using Amazon.S3;
using VideoProcessor.Application.Services;

namespace VideoProcessor.FileManager;

public class S3Service : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;

    public S3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public Task DownloadFileAsync(string folder, string fileKey, string destinationFilePath)
    {
        return _s3Client.DownloadToFilePathAsync(folder, fileKey, destinationFilePath, default);
    }

    public Task SaveFileAsync(string path, object file)
    {
        throw new NotImplementedException();
    }

    public void DeleteFile(string sourcePath)
    {
        Console.WriteLine("apagando arquivos" + sourcePath);
        File.Delete(sourcePath);
    }

    public async Task SaveFileAsync(string filePath, string fileKey)
    {
        Console.WriteLine("upload  zip " + fileKey + "/" + fileKey);
        await _s3Client.UploadObjectFromFilePathAsync("hackathon-video-processor",
            fileKey, filePath,
            default, default);
    }
}