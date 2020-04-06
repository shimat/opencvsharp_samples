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
            using var dstImg = srcImg.Clone();
            using var dspImg = srcImg.Clone();
            using var markers = new Mat(srcImg.Size(), MatType.CV_32S, 1);

            markers.SetTo(Scalar.Black);

            using (var window = new Window("image", WindowMode.AutoSize))
            {
                window.Image = srcImg;
                // Mouse event  
                int seedNum = 0;
                window.SetMouseCallback((MouseEventTypes ev, int x, int y, MouseEventFlags flags, IntPtr userdata) =>
                {
                    if (ev == MouseEventTypes.LButtonDown)
                    {
                        seedNum++;
                        Point pt = new Point(x, y);
                        markers.Circle(pt, 10, Scalar.All(seedNum), Cv2.FILLED, LineTypes.Link8, 0);
                        dspImg.Circle(pt, 10, Scalar.White, 3, LineTypes.Link8, 0);
                        window.Image = dspImg;
                    }
                });
                Window.WaitKey();
            }

            Cv2.Watershed(srcImg, markers);

            // draws watershed
            for (int i = 0; i < markers.Height; i++)
            {
                for (int j = 0; j < markers.Width; j++)
                {
                    //int idx = (int)(markers.Get2D(i, j).Val0);
                    int idx = (int)(markers.Get<Point>(i, j).X);
                    if (idx == -1)
                    {
                        // dstImg.Set2D(i, j, CvColor.Red);
                        dstImg.Set<Scalar>(i, j, Scalar.Red);
                    }
                }
            }
            using (Window wDst = new Window("watershed transform", WindowMode.AutoSize))
            {
                wDst.Image = dstImg;
                Window.WaitKey();
            }
        }
    }
}