using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenCvSharp;
using SampleBase;

namespace SamplesCS
{
    /// <summary>
    /// Watershed algorithm sample
    /// </summary>
    /// <remarks>http://opencv.jp/sample/segmentation_and_connection.html#watershed</remarks>
    public class WatershedSample : ISample
    {
        public void Run()
        {
            using var srcImg = Cv2.ImRead(FilePath.Image.Lenna, ImreadModes.AnyDepth | ImreadModes.AnyColor);            
            using var markers = new Mat(srcImg.Size(), MatType.CV_32SC1, Scalar.All(0));

            using (var window = new Window("image", WindowMode.AutoSize, srcImg))
            {
                using var dspImg = srcImg.Clone();

                // Mouse event  
                int seedNum = 0;
                window.SetMouseCallback((MouseEventTypes ev, int x, int y, MouseEventFlags flags, IntPtr userdata) =>
                {
                    if (ev == MouseEventTypes.LButtonDown)
                    {
                        seedNum++;
                        var pt = new Point(x, y);
                        markers.Circle(pt, 10, Scalar.All(seedNum), Cv2.FILLED, LineTypes.Link8);
                        dspImg.Circle(pt, 10, Scalar.White, 3, LineTypes.Link8);
                        window.Image = dspImg;
                    }
                });
                Window.WaitKey();
            }

            Cv2.Watershed(srcImg, markers);

            // draws watershed
            using var dstImg = srcImg.Clone();
            for (int y = 0; y < markers.Height; y++)
            {
                for (int x = 0; x < markers.Width; x++)
                {
                    int idx = markers.Get<int>(y, x);
                    if (idx == -1)
                    {
                        dstImg.Rectangle(new Rect(x, y, 2, 2), Scalar.Red, -1);
                    }
                }
            }

            using (new Window("watershed transform", WindowMode.AutoSize, dstImg))
            {
                Window.WaitKey();
            }
        }
    }
}
