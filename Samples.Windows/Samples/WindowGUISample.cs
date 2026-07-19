using OpenCvSharp;
using System;
using System.Diagnostics;

namespace Samples.Windows;

internal class WindowGUISample : ISample
{
    public void Run()
    {
        Windows_Example();
        TrackBar_Example();
        MouseCallBack_Example();
    }

    public static void Windows_Example()
    {
        using var srcImg = new Mat(FilePath.Image.SurfBoxinscene, ImreadModes.AnyColor);
        using var openCloseWindow = new Window("OpenCVWindow", srcImg);
        Debug.WriteLine(Cv2.WaitKey());
    }

    public void MouseCallBack_Example()
    {
        using var srcImg = new Mat(FilePath.Image.SurfBoxinscene, ImreadModes.AnyColor);
        using var foo = new Window("MouseEvent", srcImg);
        Cv2.SetMouseCallback(foo.Name, CallbackOpenCVAnnotate);
        Cv2.WaitKey();
    }

    private void CallbackOpenCVAnnotate(MouseEventTypes e, int x, int y, MouseEventFlags flags, IntPtr userdata)
    {
        var label = e switch
        {
            MouseEventTypes.LButtonDown => "Down",
            _ when flags.HasFlag(MouseEventFlags.LButton) => "flags",
            MouseEventTypes.LButtonUp => "Up",
            MouseEventTypes.MouseWheel => "Wheel",
            _ => null,
        };
        if (label is not null)
            Debug.WriteLine($"{x},{y} {label}");
    }

    public static void TrackBar_Example()
    {
        using var src = new Mat(FilePath.Image.SurfBoxinscene, ImreadModes.AnyColor);
        using var dst = new Mat();

        src.CopyTo(dst);

        var elementShape = MorphShapes.Rect;
        var maxIterations = 10;

        using var openCloseWindow = new Window("Open/Close", image: dst);
        var openCloseTrackbar = openCloseWindow.CreateTrackbar(
            trackbarName: "Iterations",
            initialPos: 10,
            max: maxIterations * 2 + 1,
            callback: pos =>
            {
                var n = pos - maxIterations;
                var an = n > 0 ? n : -n;
                using var element = Cv2.GetStructuringElement(
                        elementShape,
                        new Size(an * 2 + 1, an * 2 + 1),
                        new Point(an, an));

                if (n < 0)
                {
                    Cv2.MorphologyEx(src, dst, MorphTypes.Open, element);
                }
                else
                {
                    Cv2.MorphologyEx(src, dst, MorphTypes.Close, element);
                }

                Cv2.PutText(dst, (n < 0) ?
                    $"Open/Erosion [{elementShape}]"
                    : $"Close/Dilation [{elementShape}]",
                    new Point(10, 15), HersheyFonts.HersheyPlain, 1, Scalar.Black);
                openCloseWindow.Image = dst;
            });


        using var erodeDilateWindow = new Window("Erode/Dilate", image: dst);
        var erodeDilateTrackbar = erodeDilateWindow.CreateTrackbar(
            trackbarName: "Iterations",
            initialPos: 10,
            max: maxIterations * 2 + 1,
            callback: pos =>
            {
                var n = pos - maxIterations;
                var an = n > 0 ? n : -n;
                using var element = Cv2.GetStructuringElement(
                        elementShape,
                        new Size(an * 2 + 1, an * 2 + 1),
                        new Point(an, an));
                if (n < 0)
                {
                    Cv2.Erode(src, dst, element);
                }
                else
                {
                    Cv2.Dilate(src, dst, element);
                }

                Cv2.PutText(dst, (n < 0) ?
                    $"Erode[{elementShape}]" :
                    $"Dilate[{elementShape}]",
                    new Point(10, 15), HersheyFonts.HersheyPlain, 1, Scalar.Black);
                erodeDilateWindow.Image = dst;
            });


        while (true)
        {
            var key = Cv2.WaitKey();

            if ((char)key == 27) // ESC
                break;

            elementShape = (char)key switch
            {
                'e' => MorphShapes.Ellipse,
                'r' => MorphShapes.Rect,
                'c' => MorphShapes.Cross,
                _ => elementShape,
            };
        }
    }

}
