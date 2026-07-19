using System;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Retrieves keypoints using the KAZE and AKAZE algorithm.
/// </summary>
[SampleCategory(SampleCategory.Features2D)]
internal class KAZESample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var gray = new Mat(ImagePath.Cat, ImreadModes.Grayscale);
        using var kaze = KAZE.Create();
        using var akaze = AKAZE.Create();

        using var kazeDescriptors = new Mat();
        using var akazeDescriptors = new Mat();
        KeyPoint[] kazeKeyPoints = [], akazeKeyPoints = [];
        var kazeTime = MeasureTime(() =>
            kaze.DetectAndCompute(gray, default, out kazeKeyPoints, kazeDescriptors));
        var akazeTime = MeasureTime(() =>
            akaze.DetectAndCompute(gray, default, out akazeKeyPoints, akazeDescriptors));

        using var dstKaze = new Mat();
        using var dstAkaze = new Mat();
        Cv2.DrawKeypoints(gray, kazeKeyPoints, dstKaze);
        Cv2.DrawKeypoints(gray, akazeKeyPoints, dstAkaze);

        display.Show(
            ($"KAZE [{kazeTime.TotalMilliseconds:F2}ms]", dstKaze),
            ($"AKAZE [{akazeTime.TotalMilliseconds:F2}ms]", dstAkaze));
    }

    private static TimeSpan MeasureTime(Action action)
    {
        var watch = Stopwatch.StartNew();
        action();
        watch.Stop();
        return watch.Elapsed;
    }
}
