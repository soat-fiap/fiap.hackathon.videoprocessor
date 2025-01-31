namespace VideoProcessor.Application.Services;

public interface IFileStorageService
{
    Task DownloadFileAsync(string fileKey, string destinationFilePath);

    Task SaveFileAsync(string path, string fileKey);

    void DeleteFile(string sourcePath);
}