using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Retrieves keypoints using the StarDetector algorithm.
/// </summary>
[SampleCategory(SampleCategory.Features2D)]
class StarDetectorSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        var dst = new Mat(ImagePath.Newspaper, ImreadModes.Color);
        var gray = new Mat(ImagePath.Newspaper, ImreadModes.Grayscale);

        StarDetector detector = StarDetector.Create(45);
        KeyPoint[] keypoints = detector.Detect(gray);

        if (keypoints != null)
        {
            var color = new Scalar(0, 255, 0);
            foreach (KeyPoint kpt in keypoints)
            {
                float r = kpt.Size / 2;
                Cv2.Circle(dst, (Point)kpt.Pt, (int)r, color);
                Cv2.Line(dst,
                    (Point)new Point2f(kpt.Pt.X + r, kpt.Pt.Y + r),
                    (Point)new Point2f(kpt.Pt.X - r, kpt.Pt.Y - r),
                    color);
                Cv2.Line(dst,
                    (Point)new Point2f(kpt.Pt.X - r, kpt.Pt.Y + r),
                    (Point)new Point2f(kpt.Pt.X + r, kpt.Pt.Y - r),
                    color);
            }
        }

        display.Show(("StarDetector features", dst));
    }
}
