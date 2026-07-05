using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// Extracts the foreground from an image using the GrabCut algorithm.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/d8/d83/tutorial_py_grabcut.html</remarks>
class GrabCutSample : ConsoleTestBase
{
    public override void RunTest()
    {
        using var src = new Mat(ImagePath.Fruits, ImreadModes.Color);

        // Rough rectangle around the foreground, inset from the image borders
        var rect = new Rect(
            (int)(src.Width * 0.05), (int)(src.Height * 0.05),
            (int)(src.Width * 0.9), (int)(src.Height * 0.9));

        using var mask = new Mat();
        using var bgdModel = new Mat();
        using var fgdModel = new Mat();
        Cv2.GrabCut(src, mask, rect, bgdModel, fgdModel, 5, GrabCutModes.InitWithRect);

        using var fgMask = new Mat();
        Cv2.Compare(mask & 1, new Mat(mask.Size(), mask.Type(), Scalar.All(0)), fgMask, CmpTypes.GT);

        using var foreground = new Mat();
        src.CopyTo(foreground, fgMask);

        using var rectView = src.Clone();
        Cv2.Rectangle(rectView, rect, Scalar.Red, 2);

        DisplayHelper.Show(nameof(GrabCutSample), new[] { "initial rect", "foreground mask", "extracted foreground" }, new[] { rectView, fgMask, foreground });
    }
}
