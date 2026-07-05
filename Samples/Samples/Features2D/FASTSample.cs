using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// cv::FAST
/// </summary>
[SampleCategory(SampleCategory.Features2D)]
class FASTSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using Mat imgSrc = new Mat(ImagePath.Maltese, ImreadModes.Color);
        using Mat imgGray = new Mat();
        using Mat imgDst = imgSrc.Clone();
        Cv2.CvtColor(imgSrc, imgGray, ColorConversionCodes.BGR2GRAY, 0);

        KeyPoint[] keypoints = Cv2.FAST(imgGray, 50, true);

        foreach (KeyPoint kp in keypoints)
        {
            Cv2.Circle(imgDst, (Point)kp.Pt, 3, Scalar.Red, -1, LineTypes.AntiAlias, 0);
        }

        display.Show(("FAST", imgDst));
    }
}
