using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
///
/// </summary>
[SampleCategory(SampleCategory.Video)]
class VideoCaptureSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
            // Opens MP4 file (ffmpeg is probably needed)
            using var capture = new VideoCapture(MoviePath.Bach);
            if (!capture.IsOpened())
                return;

            int sleepTime = (int)Math.Round(1000 / capture.Fps);

            // Frame image buffer
            var image = new Mat();

            // When the movie playback reaches end, Mat.data becomes NULL.
            while (true)
            {
                capture.Read(image); // same as cvQueryFrame
                if(image.Empty())
                    break;

                if (!display.ShowFrame(waitMs: sleepTime, maxHeadlessFrames: 5, ("capture", image)))
                    break;
            }
        }
}
