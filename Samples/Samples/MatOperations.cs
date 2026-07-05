using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// 
/// </summary>
class MatOperations : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        SubMat(display);
        RowColRangeOperation(display);
        RowColOperation(display);
    }

    /// <summary>
    /// Submatrix operations
    /// </summary>
    private static void SubMat(DisplayHelper display)
    {
        using var src = Cv2.ImRead(ImagePath.Fruits);

        // Assign small image to mat
        using var small = new Mat();
        Cv2.Resize(src, small, new Size(100, 100));
        src[10, 110, 10, 110] = small;
        src[370, 470, 400, 500] = small.T();
        // ↑ This is same as the following:
        //small.T().CopyTo(src[370, 470, 400, 500]);

        // Get partial mat (similar to cvSetImageROI)
        Mat part = src[200, 400, 200, 360];
        // Invert partial pixel values
        Cv2.BitwiseNot(part, part);

        // Fill the region (50..100, 100..150) with color (128, 0, 0)
        part = src.SubMat(50, 100, 400, 450);
        part.SetTo(128);

        display.Show(("SubMat", src));

        part.Dispose();
    }

    /// <summary>
    /// Submatrix operations
    /// </summary>
    private static void RowColRangeOperation(DisplayHelper display)
    {
        using var src = Cv2.ImRead(ImagePath.Fruits);

        Cv2.GaussianBlur(
            src.RowRange(100, 200),
            src.RowRange(200, 300),
            new Size(7, 7), 20);

        Cv2.GaussianBlur(
            src.ColRange(200, 300),
            src.ColRange(100, 200),
            new Size(7, 7), 20);

        display.Show(("RowColRangeOperation", src));
    }

    /// <summary>
    /// Submatrix expression operations
    /// </summary>
    private static void RowColOperation(DisplayHelper display)
    {
        using var src = Cv2.ImRead(ImagePath.Fruits);

        var rand = new Random();
        var srcRows = src.AsRows<Vec3b>();
        for (int i = 0; i < 200; i++)
        {
            int c1 = rand.Next(100, 400);
            int c2 = rand.Next(100, 400);
            var row1 = srcRows[c1];
            var row2 = srcRows[c2];
            var temp = row1.ToArray();
            row2.CopyTo(row1);
            temp.CopyTo(row2);
        }

        using (var colSrc = src.ColRange(450, 500))
        using (var colDst = src.ColRange(0, 50))
        {
            ((Mat)~colSrc).CopyTo(colDst);
        }

        using (var rowRange = src.RowRange(450, 460))
        {
            rowRange.SetTo(new Scalar(0, 0, 255));
        }

        display.Show(("RowColOperation", src));
    }
}
