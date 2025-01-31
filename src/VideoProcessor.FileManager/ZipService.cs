using System.IO.Compression;
using Microsoft.Extensions.Logging;
using VideoProcessor.Application.Services;

namespace VideoProcessor.FileManager;

public class ZipService(ILogger<ZipService> logger) : IZipService
{
    public Task CreateZipAsync(string sourceFolder, string destinationZipFilePath)
    {
        logger.LogInformation($"Creating Zip file: {destinationZipFilePath}");
        ZipFile.CreateFromDirectory(sourceFolder, destinationZipFilePath);
        logger.LogInformation($"{destinationZipFilePath} created.");
        return Task.CompletedTask;
    }
}
