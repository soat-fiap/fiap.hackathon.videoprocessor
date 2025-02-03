namespace VideoProcessor.Application.Services;

public interface IVideoProcessor
{
    Task<List<string>> ExtractSnapshotsAsync(string videoFilePath, int desiredSnapshotCount, string outputFolder);
}