using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

class BgSubtractorMOG : ConsoleTestBase
{
    public override void RunTest()
    {
        using var capture = new VideoCapture(MoviePath.Bach);
        using var mog = BackgroundSubtractorMOG.Create();

        using var frame = new Mat();
        using var fg = new Mat();
        while (true)
        {
            capture.Read(frame);
            if (frame.Empty())
                break;
            mog.Apply(frame, fg, 0.01);

            if (!DisplayHelper.ShowFrame(nameof(BgSubtractorMOG), new[] { "src", "dst" }, new[] { frame, fg }, 50))
                break;
        }
    }
}
