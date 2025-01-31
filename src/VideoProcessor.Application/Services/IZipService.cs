namespace VideoProcessor.Application.Services;

public interface IZipService
{
    Task CreateZipAsync(string sourceFolder, string destinationZipFilePath);
}