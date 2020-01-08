using OpenCvSharp;
using System;
using System.Collections.Generic;

namespace SamplesCS.Samples
{
    public class PerspectiveTransformSample : ISample
    {
        List<Point2f> point2Fs = new List<Point2f>();

        Point2f[] srcPoints = new Point2f[] {
            new Point2f(0, 0),
            new Point2f(0, 0),
            new Point2f(0, 0),
            new Point2f(0, 0),
        };

        Point2f[] dstPoints = new Point2f[] {
            new Point2f(0, 0),
            new Point2f(0, 480),
            new Point2f(640, 480),
            new Point2f(640, 0),
        };

        Mat OriginalImage;

        //PerspectiveTransform
        public void Run()
        {
            OriginalImage = new Mat("./Resource.png", ImreadModes.AnyColor); // Your Resource
            using var Window = new Window("result", OriginalImage);

            Cv2.SetMouseCallback(Window.Name, CallbackOpenCVAnnotate);
            Window.WaitKey();
        }

        private void CallbackOpenCVAnnotate(MouseEventTypes e, int x, int y, MouseEventFlags flags, IntPtr userdata)
        {
            if (e == MouseEventTypes.LButtonDown)
            {
                point2Fs.Add(new Point2f(x, y));
                if (point2Fs.Count == 4)
                {
                    srcPoints = point2Fs.ToArray();
                    using var matrix = Cv2.GetPerspectiveTransform(srcPoints, dstPoints);
                    using var dst = new Mat(new Size(640, 480), MatType.CV_8UC3);
                    Cv2.WarpPerspective(OriginalImage, dst, matrix, dst.Size());
                    using var dsts = new Window("dst", dst);
                    point2Fs.Clear();
                }
            }
        }
    }
}
