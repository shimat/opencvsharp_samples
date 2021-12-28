using OpenCvSharp;
using SampleBase;

namespace SamplesLegacy
{
    /// <summary>
    /// Retrieves keypoints using the BRISK algorithm.
    /// </summary>
    class BRISKSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            var gray = new Mat(ImagePath.Lenna, ImreadModes.Grayscale);
            var dst = new Mat(ImagePath.Lenna, ImreadModes.Color);

            using var brisk = BRISK.Create();
            KeyPoint[] keypoints = brisk.Detect(gray);

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

            using (new Window("BRISK features", dst))
            {
                Cv2.WaitKey();
            }
        }
    }
}