using System;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// Retrieves keypoints using the KAZE and AKAZE algorithm.
/// </summary>
internal class KAZESample : ConsoleTestBase
{
    public override void RunTest()
    {
        var gray = new Mat(ImagePath.Cat, ImreadModes.Grayscale);
        var kaze = KAZE.Create();
        var akaze = AKAZE.Create();

        var kazeDescriptors = new Mat();
        var akazeDescriptors = new Mat();
        KeyPoint[] kazeKeyPoints = null, akazeKeyPoints = null;
        var kazeTime = MeasureTime(() =>
            kaze.DetectAndCompute(gray, default, out kazeKeyPoints, kazeDescriptors));
        var akazeTime = MeasureTime(() =>
            akaze.DetectAndCompute(gray, default, out akazeKeyPoints, akazeDescriptors));

        var dstKaze = new Mat();
        var dstAkaze = new Mat();
        Cv2.DrawKeypoints(gray, kazeKeyPoints, dstKaze);
        Cv2.DrawKeypoints(gray, akazeKeyPoints, dstAkaze);

        DisplayHelper.Show(nameof(KAZESample),
            (String.Format("KAZE [{0:F2}ms]", kazeTime.TotalMilliseconds), dstKaze),
            (String.Format("AKAZE [{0:F2}ms]", akazeTime.TotalMilliseconds), dstAkaze));
    }

    private TimeSpan MeasureTime(Action action)
    {
        var watch = Stopwatch.StartNew();
        action();
        watch.Stop();
        return watch.Elapsed;
    }
}
