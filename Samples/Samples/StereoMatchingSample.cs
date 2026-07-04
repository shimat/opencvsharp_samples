using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// Computes a disparity map from a stereo pair using StereoBM and StereoSGBM.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/dd/d53/tutorial_py_depthmap.html</remarks>
class StereoMatchingSample : ConsoleTestBase
{
    public override void RunTest()
    {
        using var left = new Mat(ImagePath.TsukubaLeft, ImreadModes.Grayscale);
        using var right = new Mat(ImagePath.TsukubaRight, ImreadModes.Grayscale);

        const int numDisparities = 64;

        using var bm = StereoBM.Create(numDisparities, 15);
        using var disparityBm = new Mat();
        bm.Compute(left, right, disparityBm);

        using var sgbm = StereoSGBM.Create(
            minDisparity: 0, numDisparities: numDisparities, blockSize: 5,
            p1: 8 * 5 * 5, p2: 32 * 5 * 5, disp12MaxDiff: 1,
            preFilterCap: 63, uniquenessRatio: 10, speckleWindowSize: 100, speckleRange: 32);
        using var disparitySgbm = new Mat();
        sgbm.Compute(left, right, disparitySgbm);

        using var disparityBmView = new Mat();
        using var disparitySgbmView = new Mat();
        Cv2.Normalize(disparityBm, disparityBmView, 0, 255, NormTypes.MinMax, MatType.CV_8U);
        Cv2.Normalize(disparitySgbm, disparitySgbmView, 0, 255, NormTypes.MinMax, MatType.CV_8U);

        Window.ShowImages(
            new[] { left, right, disparityBmView, disparitySgbmView },
            new[] { "left", "right", "disparity (BM)", "disparity (SGBM)" });
    }
}
