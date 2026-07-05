using OpenCvSharp;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
///
/// </summary>
[SampleCategory(SampleCategory.Video)]
class CameraCaptureSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        if (display.IsHeadless)
        {
            PrintWarning("Skipping: this sample needs a live camera and cannot be meaningfully verified headlessly.");
            return;
        }

        using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
        if (!capture.IsOpened())
            return;

        capture.FrameWidth = 1920;
        capture.FrameHeight = 1280;
        capture.AutoFocus = true;

        const int sleepTime = 10;

        var image = new Mat();

        while (true)
        {
            capture.Read(image);
            if (image.Empty())
                break;

            if (!display.ShowFrame(waitMs: sleepTime, maxHeadlessFrames: 5, ("capture", image)))
                break;
        }
    }
}
