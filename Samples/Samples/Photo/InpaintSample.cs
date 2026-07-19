using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Inpainting
/// </summary>
/// <remarks>http://opencv.jp/sample/special_transforms.html#inpaint</remarks>
[SampleCategory(SampleCategory.Photo)]
public class InpaintSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        // cvInpaint

        if (display.IsHeadless)
        {
            PrintWarning("Skipping: this sample requires interactively painting a mask with the mouse and cannot be meaningfully verified headlessly.");
            return;
        }

        Console.WriteLine(
            "Hot keys: \n" +
            "\tESC - quit the program\n" +
            "\tr - restore the original image\n" +
            "\ti or ENTER - run inpainting algorithm\n" +
            "\t\t(before running it, paint something on the image)"
        );

        using var img0 = Cv2.ImRead(ImagePath.Fruits, ImreadModes.AnyDepth | ImreadModes.AnyColor);
        using var img = img0.Clone();
        using var inpaintMask = new Mat(img0.Size(), MatType.CV_8U, Scalar.Black);
        using var inpainted = img0.EmptyClone();

        using var wImage = new Window("image", img);
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
                Cv2.Line(inpaintMask, prevPt, pt, Scalar.White, 5, LineTypes.AntiAlias, 0);
                Cv2.Line(img, prevPt, pt, Scalar.White, 5, LineTypes.AntiAlias, 0);
                prevPt = pt;
                wImage.ShowImage(img);
            }
        });

        Window? wInpaint1 = null;
        Window? wInpaint2 = null;
        try
        {
            while (true)
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
                        Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintTypes.Telea);
                        wInpaint1 ??= new Window("inpainted image (algorithm by Alexandru Telea)", WindowFlags.AutoSize);
                        wInpaint1.ShowImage(inpainted);
                        Cv2.Inpaint(img, inpaintMask, inpainted, 3, InpaintTypes.NS);
                        wInpaint2 ??= new Window("inpainted image (algorithm by Navier-Strokes)", WindowFlags.AutoSize);
                        wInpaint2.ShowImage(inpainted);
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
