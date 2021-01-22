using System;
using OpenCvSharp;

namespace SamplesCore
{
    /// <summary>
    /// 
    /// </summary>
    class CameraCaptureSample : ISample
    {
        public void Run()
        {
            using var capture = new VideoCapture(0, VideoCaptureAPIs.DSHOW);
            if (!capture.IsOpened())
                return;

            capture.FrameWidth = 1920;
            capture.FrameHeight = 1280;
            capture.AutoFocus = true;

            const int sleepTime = 10;

            using var window = new Window("capture");
            var image = new Mat();
            
            while (true)
            {
                capture.Read(image); 
                if (image.Empty())
                    break;

                window.ShowImage(image);
                int c = Cv2.WaitKey(sleepTime);
                if (c >= 0)
                {
                    break;
                }
            }
        }
    }
}