using System;
using OpenCvSharp;
using Sample.Test;

namespace SamplesCore
{
    /// <summary>
    /// Hough Transform Sample / ハフ変換による直線検出
    /// </summary>
    /// <remarks>http://opencv.jp/sample/special_transforms.html#hough_line</remarks>
    class HoughLinesSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            SampleCpp();      
        }

        /// <summary>
        /// sample of new C++ style wrapper
        /// </summary>
        private void SampleCpp()
        {
            // (1) Load the image
            using (var imgGray = new Mat(FilePath.Image.Goryokaku, ImreadModes.Grayscale))
            using (var imgStd = new Mat(FilePath.Image.Goryokaku, ImreadModes.Color))
            using (var imgProb = imgStd.Clone())
            {
                // Preprocess
                Cv2.Canny(imgGray, imgGray, 50, 200, 3, false);

                // (3) Run Standard Hough Transform 
                LineSegmentPolar[] segStd = Cv2.HoughLines(imgGray, 1, Math.PI / 180, 50, 0, 0);
                int limit = Math.Min(segStd.Length, 10);
                for (int i = 0; i < limit; i++ )
                {
                    // Draws result lines
                    float rho = segStd[i].Rho;
                    float theta = segStd[i].Theta;
                    double a = Math.Cos(theta);
                    double b = Math.Sin(theta);
                    double x0 = a * rho;
                    double y0 = b * rho;
                    Point pt1 = new Point { X = (int)Math.Round(x0 + 1000 * (-b)), Y = (int)Math.Round(y0 + 1000 * (a)) };
                    Point pt2 = new Point { X = (int)Math.Round(x0 - 1000 * (-b)), Y = (int)Math.Round(y0 - 1000 * (a)) };
                    imgStd.Line(pt1, pt2, Scalar.Red, 3, LineTypes.AntiAlias, 0);
                }

                // (4) Run Probabilistic Hough Transform
                LineSegmentPoint[] segProb = Cv2.HoughLinesP(imgGray, 1, Math.PI / 180, 50, 50, 10);
                foreach (LineSegmentPoint s in segProb)
                {
                    imgProb.Line(s.P1, s.P2, Scalar.Red, 3, LineTypes.AntiAlias, 0);
                }

                // (5) Show results
                using (new Window("Hough_line_standard", imgStd, WindowFlags.AutoSize))
                using (new Window("Hough_line_probabilistic", imgProb, WindowFlags.AutoSize))
                {
                    Window.WaitKey(0);
                }
            }
        }

    }
}
