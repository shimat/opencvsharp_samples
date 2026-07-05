using System.Linq;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// 
/// </summary>
class ConnectedComponentsSample : ConsoleTestBase
{
    public override void RunTest()
    {
        using var src = new Mat(ImagePath.Shapes, ImreadModes.Color);
        using var gray = new Mat();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        using var binary = new Mat();
        Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
        using var labelView = src.EmptyClone();
        using var rectView = new Mat();
        Cv2.CvtColor(binary, rectView, ColorConversionCodes.GRAY2BGR);

        var cc = Cv2.ConnectedComponentsEx(binary);
        if (cc.LabelCount <= 1)
            return;

        // draw labels
        cc.RenderBlobs(labelView);

        // draw bonding boxes except background
        foreach (var blob in cc.Blobs.Skip(1))
        {
            Cv2.Rectangle(rectView, blob.Rect, Scalar.Red);
        }

        // filter maximum blob
        var maxBlob = cc.GetLargestBlob();
        var filtered = new Mat();
        cc.FilterByBlob(src, filtered, maxBlob);

        DisplayHelper.Show(nameof(ConnectedComponentsSample),
            new[] { "src", "binary", "labels", "bonding boxes", "maximum blob" },
            new[] { src, binary, labelView, rectView, filtered });
    }
}
