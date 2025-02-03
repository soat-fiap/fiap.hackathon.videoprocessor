using Hackathon.Video.SharedKernel;
using Hackathon.Video.SharedKernel.Events;
using VideoProcessor.Application.Services;

namespace VideoProcessor.Application.UseCases;

public class ExtractSnapshotsUseCase : IUseCase<VideoReceived>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IVideoProcessor _videoProcessor;
    private readonly IZipService _zipService;
    private readonly IDispatcher _dispatcher;

    public ExtractSnapshotsUseCase(IFileStorageService fileStorageService,
        IVideoProcessor videoProcessor, IZipService zipService, IDispatcher dispatcher)
    {
        _fileStorageService = fileStorageService;
        _videoProcessor = videoProcessor;
        _zipService = zipService;
        _dispatcher = dispatcher;
    }

    public async Task ExecuteAsync(VideoReceived request)
    {
        await PublishVideoEvent(new VideoProcessingStarted(request.UserId, request.JobId));
        
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

            await CreateZipFileAsync(outputFolder, destinationZipFilePath);
            await SaveZipFileAsync(request, destinationZipFilePath, zipFileName);
            
            await PublishVideoEvent(new VideoProcessingCompleted(request.UserId, request.JobId));
        }
        catch (Exception e)
        {
            await PublishVideoEvent(new VideoProcessingFailed(request.UserId, request.JobId, e.Message));
        }
        finally
        {
            CleanUp(files);
        }
    }

    private async Task PublishVideoEvent(object @event)
    {
        await _dispatcher.PublishAsync(@event);
    }

    private Task SaveZipFileAsync(VideoReceived request, string destinationZipFilePath, string zipFileName)
    {
        return _fileStorageService.SaveFileAsync(destinationZipFilePath,
            $"{request.UserId}/{request.JobId}/{zipFileName}");
    }

    private Task CreateZipFileAsync(string outputFolder, string destinationZipFilePath)
    {
        return _zipService.CreateZipAsync(outputFolder, destinationZipFilePath);
    }

    private void CleanUp(IReadOnlyCollection<string> files)
    {
        foreach (var file in files)
        {
            _fileStorageService.DeleteFile(file);
        }
    }
}