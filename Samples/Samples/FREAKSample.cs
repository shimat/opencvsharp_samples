using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Retrieves keypoints using the FREAK algorithm.
/// </summary>
[SampleCategory(SampleCategory.Features2D)]
class FREAKSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var gray = new Mat(ImagePath.Penguin2, ImreadModes.Grayscale);
        using var dst = new Mat(ImagePath.Penguin2, ImreadModes.Color);

        // ORB
        using var orb = ORB.Create(1000);
        KeyPoint[] keypoints = orb.Detect(gray);

        // FREAK
        using var freak = FREAK.Create();
        Mat freakDescriptors = new Mat();
        freak.Compute(gray, ref keypoints, freakDescriptors);

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

        display.Show(("FREAK", dst));
    }
}
