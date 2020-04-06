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

            using (Mat img0 = Cv2.ImRead(FilePath.Image.Fruits, ImreadModes.AnyDepth | ImreadModes.AnyColor))
            {
                using (Mat img = img0.Clone())
                using (Mat inpaintMask = new Mat(img0.Size(), MatType.CV_8U, 1))
                using (Mat inpainted = img0.Clone())
                {
                    inpainted.SetTo(Scalar.Black);
                    inpaintMask.SetTo(Scalar.Black);

                    using (Window wImage = new Window("image", WindowMode.AutoSize, img))
                    {
                        Point prevPt = new Point(-1, -1);
                        wImage.SetMouseCallback((MouseEventTypes ev, int x, int y, MouseEventFlags flags, IntPtr userdata)=>
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

                        for (; ; )
                        {
                            switch ((char)Window.WaitKey(0))
                            {
                                case (char)27:    // exit
                                    Window.DestroyAllWindows();
                                    return;
                                case 'r':   // restore original image
                                    inpaintMask.SetTo(Scalar.Black);
                                    img0.CopyTo(img);
                                    wImage.ShowImage(img);
                                    break;
                                case 'i':   // do Inpaint
                                case '\r':
                                    Window wInpaint1 = new Window("inpainted image (algorithm by Alexandru Telea)", WindowMode.AutoSize);
                                    Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintMethod.Telea);
                                    wInpaint1.ShowImage(inpainted);
                                    Window wInpaint2 = new Window("inpainted image (algorithm by Navier-Strokes)", WindowMode.AutoSize);
                                    Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintMethod.NS);
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

                }
            }
        }
    }
}