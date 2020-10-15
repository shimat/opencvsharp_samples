using OpenCvSharp;

namespace SamplesCore
{
    /// <summary>
    /// 
    /// </summary>
    class MorphologySample : ISample
    {
        public void Run()
        {
            using var gray = new Mat(FilePath.Image.Lenna, ImreadModes.Grayscale);
            using var binary = new Mat();
            using var dilate1 = new Mat();
            using var dilate2 = new Mat();
            byte[] kernelValues = {0, 1, 0, 1, 1, 1, 0, 1, 0}; // cross (+)
            using var kernel = new Mat(3, 3, MatType.CV_8UC1, kernelValues);

            // Binarize
            Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Otsu);

            // empty kernel
            Cv2.Dilate(binary, dilate1, null);
            // + kernel
            Cv2.Dilate(binary, dilate2, kernel);

            Cv2.ImShow("binary", binary);
            Cv2.ImShow("dilate (kernel = null)", dilate1);
            Cv2.ImShow("dilate (kernel = +)", dilate2);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }
    }
}