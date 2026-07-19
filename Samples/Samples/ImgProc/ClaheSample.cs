using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

[SampleCategory(SampleCategory.ImgProc)]
class ClaheSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var src = new Mat(ImagePath.TsukubaLeft, ImreadModes.Grayscale);
        using var dst1 = new Mat();
        using var dst2 = new Mat();
        using var dst3 = new Mat();

        using var clahe = Cv2.CreateCLAHE();
        clahe.ClipLimit = 20;
        clahe.Apply(src, dst1);
        clahe.ClipLimit = 40;
        clahe.Apply(src, dst2);
        clahe.TilesGridSize = new Size(4, 4);
        clahe.Apply(src, dst3);

        display.Show(("src", src), ("dst clip20", dst1), ("dst clip40", dst2), ("dst tile4x4", dst3));
    }
}
