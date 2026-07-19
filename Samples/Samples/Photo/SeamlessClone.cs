using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// cv::seamlessClone
/// </summary>
[SampleCategory(SampleCategory.Photo)]
class SeamlessClone : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var src = new Mat(ImagePath.Girl, ImreadModes.Color);
        using var dst = new Mat(ImagePath.Fruits, ImreadModes.Color);
        using var src0 = new Mat();
        Cv2.Resize(src, src0, dst.Size(), 0, 0, InterpolationFlags.Lanczos4);
        using var mask = Mat.Zeros(src0.Size(), MatType.CV_8UC3).ToMat();

        Cv2.Circle(mask, 200, 200, 100, Scalar.White, -1);

        using var blend1 = new Mat();
        using var blend2 = new Mat();
        using var blend3 = new Mat();
        Cv2.SeamlessClone(
            src0, dst, mask, new Point(260, 270), blend1,
            SeamlessCloneFlags.NormalClone);
        Cv2.SeamlessClone(
            src0, dst, mask, new Point(260, 270), blend2,
            SeamlessCloneFlags.MonochromeTransfer);
        Cv2.SeamlessClone(
            src0, dst, mask, new Point(260, 270), blend3,
            SeamlessCloneFlags.MixedClone);

        display.Show(
            ("src", src0), ("dst", dst), ("mask", mask),
            ("blend NormalClone", blend1), ("blend MonochromeTransfer", blend2), ("blend MixedClone", blend3));
    }
}
