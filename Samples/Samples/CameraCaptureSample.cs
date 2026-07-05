using OpenCvSharp;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// 
/// </summary>
class CameraCaptureSample : ConsoleTestBase
{
    public override void RunTest()
    {
        if (DisplayHelper.IsHeadless)
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

            if (!DisplayHelper.ShowFrame(nameof(CameraCaptureSample), waitMs: sleepTime, maxHeadlessFrames: 5, ("capture", image)))
                break;
        }
    }
}
