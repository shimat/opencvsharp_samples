using OpenCvSharp;
using SampleBase;

namespace SamplesLegacy
{
    /// <summary>
    /// cv::FAST
    /// </summary>
    class FASTSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            using Mat imgSrc = new Mat(ImagePath.Lenna, ImreadModes.Color);
            using Mat imgGray = new Mat();
            using Mat imgDst = imgSrc.Clone();
            Cv2.CvtColor(imgSrc, imgGray, ColorConversionCodes.BGR2GRAY, 0);

            KeyPoint[] keypoints = Cv2.FAST(imgGray, 50, true);

            foreach (KeyPoint kp in keypoints)
            {
                imgDst.Circle((Point)kp.Pt, 3, Scalar.Red, -1, LineTypes.AntiAlias, 0);
            }

            Cv2.ImShow("FAST", imgDst);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}