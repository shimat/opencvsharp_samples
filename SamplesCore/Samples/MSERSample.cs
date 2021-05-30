using OpenCvSharp;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// Maximally Stable Extremal Regions
    /// </summary>
    class MSERSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            using Mat src = new Mat(ImagePath.Distortion, ImreadModes.Color);
            using Mat gray = new Mat();
            using Mat dst = src.Clone();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            CppStyleMSER(gray, dst);  // C++ style

            using (new Window("MSER src", src))
            using (new Window("MSER gray", gray))
            using (new Window("MSER dst", dst))
            {
                Cv2.WaitKey();
            }
        }
        
        /// <summary>
        /// Extracts MSER by C++-style code (cv::MSER)
        /// </summary>
        /// <param name="gray"></param>
        /// <param name="dst"></param>
        private void CppStyleMSER(Mat gray, Mat dst)
        {
            MSER mser = MSER.Create();
            mser.DetectRegions(gray, out Point[][] contours, out _);
            foreach (Point[] pts in contours)
            {
                Scalar color = Scalar.RandomColor();
                foreach (Point p in pts)
                {
                    dst.Circle(p, 1, color);
                }
            }
        }
    }
}