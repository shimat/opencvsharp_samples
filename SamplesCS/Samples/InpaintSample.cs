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
    /// Inpainting
    /// </summary>
    /// <remarks>http://opencv.jp/sample/special_transforms.html#inpaint</remarks>
    public class InpaintSample : ISample
    {
        public void Run()
        {
            // cvInpaint

            Console.WriteLine(
                "Hot keys: \n" +
                "\tESC - quit the program\n" +
                "\tr - restore the original image\n" +
                "\ti or ENTER - run inpainting algorithm\n" +
                "\t\t(before running it, paint something on the image)\n" +
                "\ts - save the original image, mask image, original+mask image and inpainted image to desktop."
            );

            using var img0 = Cv2.ImRead(FilePath.Image.Fruits, ImreadModes.AnyDepth | ImreadModes.AnyColor);
            using var img = img0.Clone();
            using var inpaintMask = new Mat(img0.Size(), MatType.CV_8U, Scalar.Black);
            using var inpainted = img0.EmptyClone();

            using var wImage = new Window("image", WindowMode.AutoSize, img);
            var prevPt = new Point(-1, -1);
            wImage.SetMouseCallback((MouseEventTypes ev, int x, int y, MouseEventFlags flags, IntPtr userdata) =>
            {
                if (ev == MouseEventTypes.LButtonUp || (flags & MouseEventFlags.LButton) == 0)
                {
                    prevPt = new Point(-1, -1);
                }
                else if (ev == MouseEventTypes.LButtonDown)
                {
                    prevPt = new Point(x, y);
                }
                else if (ev == MouseEventTypes.MouseMove && (flags & MouseEventFlags.LButton) != 0)
                {
                    Point pt = new Point(x, y);
                    if (prevPt.X < 0)
                    {
                        prevPt = pt;
                    }
                    inpaintMask.Line(prevPt, pt, Scalar.White, 5, LineTypes.AntiAlias, 0);
                    img.Line(prevPt, pt, Scalar.White, 5, LineTypes.AntiAlias, 0);
                    prevPt = pt;
                    wImage.ShowImage(img);
                }
            });

            Window wInpaint1 = null;
            Window wInpaint2 = null;
            try
            {
                for (; ; )
                {
                    switch ((char)Window.WaitKey(0))
                    {
                        case (char)27:    // exit
                            return;
                        case 'r':   // restore original image
                            inpaintMask.SetTo(Scalar.Black);
                            img0.CopyTo(img);
                            wImage.ShowImage(img);
                            break;
                        case 'i':   // do Inpaint
                        case '\r':
                            Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintMethod.Telea);
                            wInpaint1 ??= new Window("inpainted image (algorithm by Alexandru Telea)", WindowMode.AutoSize);
                            wInpaint1.ShowImage(inpainted);
                            Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintMethod.NS);
                            wInpaint2 ??= new Window("inpainted image (algorithm by Navier-Strokes)", WindowMode.AutoSize);
                            wInpaint2.ShowImage(inpainted);
                            break;
                        case 's': // save images
                            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                            img0.SaveImage(Path.Combine(desktop, "original.png"));
                            inpaintMask.SaveImage(Path.Combine(desktop, "mask.png"));
                            img.SaveImage(Path.Combine(desktop, "original+mask.png"));
                            inpainted.SaveImage(Path.Combine(desktop, "inpainted.png"));
                            break;
                    }
                }
            }
            finally
            {
                wInpaint1?.Dispose();
                wInpaint2?.Dispose();
                Window.DestroyAllWindows();
            }
        }
    }
}