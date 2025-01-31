using System.Drawing;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using VideoProcessor.Application.Services;

namespace VideoProcessor.FileManager;

public class VideoProcessor(ILogger<VideoProcessor> logger) : IVideoProcessor
{
    public async Task<List<string>> ExtractSnapshotsAsync(string videoFilePath, int desiredSnapshotCount,
        string outputFolder)
    {
        var videoInfo = await FFProbe.AnalyseAsync(videoFilePath);
        var duration = videoInfo.Duration;
        var interval = duration.Duration() / desiredSnapshotCount;
        var files = new List<string>();

        Directory.CreateDirectory(outputFolder);
        logger.LogInformation("Start processing frames");
        for (var currentTime = TimeSpan.Zero; currentTime < duration; currentTime += interval)
        {
            logger.LogInformation($"Processing  frame: {currentTime}");
            var outputPath = Path.Combine(outputFolder, $"frame_at_{currentTime.TotalSeconds}.png");
            files.Add(outputPath);
            FFMpeg.Snapshot(videoFilePath, outputPath, new Size(1280, 720), currentTime);
        }

        logger.LogInformation("All frames processed");

        return files;
    }
}