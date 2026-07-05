using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

class BgSubtractorMOG : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
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

            if (!display.ShowFrame(waitMs: 50, maxHeadlessFrames: 5, ("src", frame), ("dst", fg)))
                break;
        }
    }
}
