using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using OpenCvSharp;

// Resolved against the output directory, not the process's current directory, since
// `dotnet run` starts with the project folder as the working directory.
var defaultImagePath = Path.Combine(AppContext.BaseDirectory, "Data", "Image", "Mandrill.bmp");
var defaultVideoPath = Path.Combine(AppContext.BaseDirectory, "Data", "Movie", "bach.mp4");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/api/cartoonify", async (HttpRequest request) =>
{
    var image = await GetOptionalFileAsync(request, "image");
    using var src = await LoadSourceImageAsync(image, defaultImagePath);
    if (src.Empty())
    {
        return Results.BadRequest("Could not decode the uploaded file as an image.");
    }

    using var gray = new Mat();
    Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
    Cv2.MedianBlur(gray, gray, 9);

    // Edge mask: dark lines (0) on a mostly white (255) background. A larger block size
    // and C cut down on speckling from fine texture (fur, skin) that isn't a real outline.
    using var edges = new Mat();
    Cv2.AdaptiveThreshold(gray, edges, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 15, 20);

    // Flatten flat color regions while keeping edges; two passes make the flattening
    // more pronounced than a single call.
    using var smooth1 = new Mat();
    using var smooth2 = new Mat();
    Cv2.BilateralFilter(src, smooth1, 9, 200, 200);
    Cv2.BilateralFilter(smooth1, smooth2, 9, 200, 200);

    using var cartoon = new Mat();
    Cv2.BitwiseAnd(smooth2, smooth2, cartoon, edges);

    Cv2.ImEncode(".png", cartoon, out var png);
    return Results.File(png, "image/png");
});

app.MapPost("/api/frame-analysis", async (HttpRequest request) =>
{
    var video = await GetOptionalFileAsync(request, "video");
    return AnalyzeFramesAsync(video, defaultVideoPath);
});

app.Run();

static async Task<IFormFile?> GetOptionalFileAsync(HttpRequest request, string fieldName)
{
    if (!request.HasFormContentType)
    {
        return null;
    }

    var form = await request.ReadFormAsync();
    return form.Files[fieldName];
}

static async Task<Mat> LoadSourceImageAsync(IFormFile? image, string defaultImagePath)
{
    if (image is { Length: > 0 })
    {
        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        return Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);
    }

    return Cv2.ImRead(defaultImagePath, ImreadModes.Color);
}

static async IAsyncEnumerable<FrameAnalysis> AnalyzeFramesAsync(IFormFile? video, string defaultVideoPath)
{
    string? tempPath = null;
    try
    {
        var videoPath = defaultVideoPath;
        if (video is { Length: > 0 })
        {
            tempPath = Path.GetTempFileName();
            await using var fileStream = File.Create(tempPath);
            await video.CopyToAsync(fileStream);
            videoPath = tempPath;
        }

        using var capture = new VideoCapture(videoPath);
        if (!capture.IsOpened())
        {
            yield break;
        }

        using var frame = new Mat();
        using var gray = new Mat();
        using var edges = new Mat();

        var frameIndex = 0;
        while (true)
        {
            capture.Read(frame);
            if (frame.Empty())
            {
                break;
            }

            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.Canny(gray, edges, 80, 160);

            var brightness = Cv2.Mean(gray).Val0;
            var edgeRatio = (double)Cv2.CountNonZero(edges) / (edges.Rows * edges.Cols);

            yield return new FrameAnalysis(frameIndex, frame.Width, frame.Height, brightness, edgeRatio);
            frameIndex++;
        }
    }
    finally
    {
        if (tempPath is not null)
        {
            File.Delete(tempPath);
        }
    }
}

sealed record FrameAnalysis(int FrameIndex, int Width, int Height, double Brightness, double EdgeRatio);
