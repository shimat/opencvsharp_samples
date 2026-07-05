using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// 
/// </summary>
class VideoWriterSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        const string OutVideoFile = "out.avi";

        // Opens MP4 file (ffmpeg is probably needed)
        using var capture = new VideoCapture(MoviePath.Bach);

        // Read movie frames and write them to VideoWriter
        var dsize = new Size(640, 480);
        int fourcc = VideoWriter.FourCC('M', 'J', 'P', 'G');
        // isColor: false because the frames written below (grayscale -> Canny) are single-channel.
        using (var writer = new VideoWriter(OutVideoFile, fourcc, capture.Fps, dsize, isColor: false))
        {
            Console.WriteLine("Converting each movie frames...");
            using var frame = new Mat();
            while (true)
            {
                // Read image
                capture.Read(frame);
                if (frame.Empty())
                    break;

                // Console.CursorLeft throws when stdout is redirected (e.g. piped/CI), so only
                // do the in-place progress update when attached to a real console.
                if (!Console.IsOutputRedirected)
                {
                    Console.CursorLeft = 0;
                    Console.Write("{0} / {1}", capture.PosFrames, capture.FrameCount);
                }

                // grayscale -> canny -> resize
                using var gray = new Mat();
                using var canny = new Mat();
                using var dst = new Mat();
                Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);
                Cv2.Canny(gray, canny, 100, 180);
                Cv2.Resize(canny, dst, dsize, 0, 0, InterpolationFlags.Linear);
                // Write mat to VideoWriter
                writer.Write(dst);
            }
            Console.WriteLine();
        }

        // Watch result movie
        using (var capture2 = new VideoCapture(OutVideoFile))
        {
            int sleepTime = (int)(1000 / capture.Fps);

            using var frame = new Mat();
            while (true)
            {
                capture2.Read(frame);
                if (frame.Empty())
                    break;

                if (!display.ShowFrame(waitMs: sleepTime, maxHeadlessFrames: 5, ("result", frame)))
                    break;
            }
        }
    }

}
