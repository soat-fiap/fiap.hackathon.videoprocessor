namespace VideoProcessor.Application.Services;

public interface IVideoProcessor
{
    Task<List<string>> ExtractSnapshotsAsync(string videoFilePath, int desiredSnapshotCoun, string outputFolder);
}