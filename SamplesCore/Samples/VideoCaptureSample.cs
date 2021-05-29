using System;
using OpenCvSharp;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// 
    /// </summary>
    class VideoCaptureSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            // Opens MP4 file (ffmpeg is probably needed)
            using var capture = new VideoCapture(MoviePath.Bach);
            if (!capture.IsOpened())
                return;

            int sleepTime = (int)Math.Round(1000 / capture.Fps);

            using var window = new Window("capture");
            // Frame image buffer
            var image = new Mat();

            // When the movie playback reaches end, Mat.data becomes NULL.
            while (true)
            {
                capture.Read(image); // same as cvQueryFrame
                if(image.Empty())
                    break;

                window.ShowImage(image);
                Cv2.WaitKey(sleepTime);
            }
        }
    }
}