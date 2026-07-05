using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// 
/// </summary>
class MorphologySample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var gray = new Mat(ImagePath.Cake, ImreadModes.Grayscale);
        using var binary = new Mat();
        using var dilate1 = new Mat();
        using var dilate2 = new Mat();
        byte[] kernelValues = { 0, 1, 0, 1, 1, 1, 0, 1, 0 }; // cross (+)
        using var kernel = Mat.FromPixelData(3, 3, MatType.CV_8UC1, kernelValues);

        // Binarize
        Cv2.Threshold(gray, binary, 0, 255, ThresholdTypes.Otsu);

        // empty kernel
        Cv2.Dilate(binary, dilate1, default);
        // + kernel
        Cv2.Dilate(binary, dilate2, kernel);

        display.Show(("binary", binary), ("dilate (kernel = null)", dilate1), ("dilate (kernel = +)", dilate2));
    }
}
