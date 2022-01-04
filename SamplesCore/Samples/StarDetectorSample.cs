using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// Retrieves keypoints using the StarDetector algorithm.
    /// </summary>
    class StarDetectorSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            var dst = new Mat(ImagePath.Lenna, ImreadModes.Color);
            var gray = new Mat(ImagePath.Lenna, ImreadModes.Grayscale);

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

            using (new Window("StarDetector features", dst))
            {
                Cv2.WaitKey();
            }
        }
    }
}