using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using VideoProcessor.Application.Services;

namespace VideoProcessor.Application.UseCases;

public class ExtractSnapshotsUseCase : IUseCase<VideoReceived>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IVideoProcessor _videoProcessor;
    private readonly IZipService _zipService;

    public ExtractSnapshotsUseCase(IFileStorageService fileStorageService,
        IVideoProcessor videoProcessor, IZipService zipService)
    {
        _fileStorageService = fileStorageService;
        _videoProcessor = videoProcessor;
        _zipService = zipService;
    }

    public async Task ExecuteAsync(VideoReceived request)
    {
        const string videoFileName = "video.mkv";
        var localJobFolder = Path.Combine(Directory.GetCurrentDirectory(), request.JobId);
        var videoFilePath = Path.Combine(localJobFolder, videoFileName);
        const string zipFileName = "images.zip";
        var outputFolder = Path.Combine(localJobFolder, "snapshots");
        var files = new List<string>();
        var destinationZipFilePath = Path.Combine(Directory.GetCurrentDirectory(), request.JobId, zipFileName);
        files.Add(destinationZipFilePath);

        try
        {
            await _fileStorageService.DownloadFileAsync($"{request.UserId}/{request.JobId}/{videoFileName}",
                videoFilePath);
            files.Add(videoFilePath);
            files.AddRange(await _videoProcessor.ExtractSnapshotsAsync(videoFilePath, request.Frames, outputFolder));

            await _zipService.CreateZipAsync(outputFolder, destinationZipFilePath);
            await _fileStorageService.SaveFileAsync(destinationZipFilePath,
                $"{request.UserId}/{request.JobId}/{zipFileName}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            CleanUp(files);
        }
    }

    private void CleanUp(IReadOnlyCollection<string> files)
    {
        foreach (var file in files)
        {
            _fileStorageService.DeleteFile(file);
        }
    }
}