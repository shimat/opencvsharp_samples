using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

class ClaheSample : ConsoleTestBase
{
    public override void RunTest()
    {
        using var src = new Mat(ImagePath.TsukubaLeft, ImreadModes.Grayscale);
        using var dst1 = new Mat();
        using var dst2 = new Mat();
        using var dst3 = new Mat();

        using (var clahe = Cv2.CreateCLAHE())
        {
            clahe.ClipLimit = 20;
            clahe.Apply(src, dst1);
            clahe.ClipLimit = 40;
            clahe.Apply(src, dst2);
            clahe.TilesGridSize = new Size(4, 4);
            clahe.Apply(src, dst3);
        }

        DisplayHelper.Show(nameof(ClaheSample), new[] { "src", "dst clip20", "dst clip40", "dst tile4x4" }, new[] { src, dst1, dst2, dst3 });
    }
}
