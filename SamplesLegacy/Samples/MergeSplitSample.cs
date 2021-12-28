using OpenCvSharp;
using SampleBase;

namespace SamplesLegacy
{
    /// <summary>
    /// 
    /// </summary>
    class MergeSplitSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            // Split/Merge Test
            {
                using var src = new Mat(ImagePath.Lenna, ImreadModes.Color);

                // Split each plane
                Cv2.Split(src, out var planes);

                Cv2.ImShow("planes 0", planes[0]);
                Cv2.ImShow("planes 1", planes[1]);
                Cv2.ImShow("planes 2", planes[2]);
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();

                // Invert G plane
                Cv2.BitwiseNot(planes[1], planes[1]);

                // Merge
                using var merged = new Mat();
                Cv2.Merge(planes, merged);

                Cv2.ImShow("src", src);
                Cv2.ImShow("merged", merged);
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();
            }

            // MixChannels Test
            {
                using var rgba = new Mat(300, 300, MatType.CV_8UC4, new Scalar(50, 100, 150, 200));
                using var bgr = new Mat(rgba.Rows, rgba.Cols, MatType.CV_8UC3);
                using var alpha = new Mat(rgba.Rows, rgba.Cols, MatType.CV_8UC1);

                Mat[] input = { rgba };
                Mat[] output = { bgr, alpha };
                // rgba[0] -> bgr[2], rgba[1] -> bgr[1],
                // rgba[2] -> bgr[0], rgba[3] -> alpha[0]
                int[] fromTo = { 0, 2, 1, 1, 2, 0, 3, 3 };
                Cv2.MixChannels(input, output, fromTo);

                Cv2.ImShow("rgba", rgba);
                Cv2.ImShow("bgr", bgr);
                Cv2.ImShow("alpha", alpha);
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();
            }
        }
    }
}