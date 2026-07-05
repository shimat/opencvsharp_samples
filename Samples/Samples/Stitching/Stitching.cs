using System;
using System.Collections.Generic;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

[SampleCategory(SampleCategory.Stitching)]
class Stitching : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        Mat[] images = SelectStitchingImages(200, 200, 10, display);

        using var stitcher = Stitcher.Create(Stitcher.Mode.Scans);
        using var pano = new Mat();

        Console.Write("Stitching start...");
        // TODO: does not work??
        var status = stitcher.Stitch(images, pano);
        Console.WriteLine(" finish (status:{0})", status);

        display.Show(("pano", pano));

        foreach (var image in images)
        {
            image.Dispose();
        }
    }

    private static Mat[] SelectStitchingImages(int width, int height, int count, DisplayHelper display)
    {
        using var source = new Mat(ImagePath.Asahiyama, ImreadModes.Color);
        using var result = source.Clone();

        var rand = new Random();
        var mats = new List<Mat>();
        for (int i = 0; i < count; i++)
        {
            int x1 = rand.Next(source.Cols - width);
            int y1 = rand.Next(source.Rows - height);
            int x2 = x1 + width;
            int y2 = y1 + height;

            Cv2.Line(result, new Point(x1, y1), new Point(x1, y2), new Scalar(0, 0, 255));
            Cv2.Line(result, new Point(x1, y2), new Point(x2, y2), new Scalar(0, 0, 255));
            Cv2.Line(result, new Point(x2, y2), new Point(x2, y1), new Scalar(0, 0, 255));
            Cv2.Line(result, new Point(x2, y1), new Point(x1, y1), new Scalar(0, 0, 255));

            using var m = source[new Rect(x1, y1, width, height)];
            mats.Add(m.Clone());
        }

        display.Show(("stitching", result));

        return mats.ToArray();
    }
}
