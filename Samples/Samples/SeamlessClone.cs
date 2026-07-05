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
            Mat src = new Mat(ImagePath.Girl, ImreadModes.Color);
            Mat dst = new Mat(ImagePath.Fruits, ImreadModes.Color);
            Mat src0 = new Mat();
            Cv2.Resize(src, src0, dst.Size(), 0, 0, InterpolationFlags.Lanczos4);
            Mat mask = Mat.Zeros(src0.Size(), MatType.CV_8UC3);

            Cv2.Circle(mask, 200, 200, 100, Scalar.White, -1);

            Mat blend1 = new Mat();
            Mat blend2 = new Mat();
            Mat blend3 = new Mat();
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
