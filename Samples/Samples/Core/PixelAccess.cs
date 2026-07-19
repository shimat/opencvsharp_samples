using System;
using System.Diagnostics;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Swaps B for R
/// </summary>
[SampleCategory(SampleCategory.Core)]
class PixelAccess : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        Console.WriteLine($"Get/Set: {MeasureTime(GetSet)}ms");
        Console.WriteLine($"GenericIndexer: {MeasureTime(GenericIndexer)}ms");
        Console.WriteLine($"TypeSpecificMat: {MeasureTime(TypeSpecificMat)}ms");
    }

    /// <summary>
    /// Slow
    /// </summary>
    private void GetSet()
    {
        using var mat = new Mat(ImagePath.Hand, ImreadModes.Color);
        int height = mat.Height;
        int width = mat.Width;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var color = mat.Get<Vec3b>(y, x);
                var newColor = new Vec3b(color.Item2, color.Item1, color.Item0);
                mat.Set<Vec3b>(y, x, newColor);
            }
        }
        //Cv2.ImShow("Slow", mat);
        //Cv2.WaitKey(0);
        //Cv2.DestroyAllWindows();
    }

    /// <summary>
    /// Reasonably fast
    /// </summary>
    private void GenericIndexer()
    {
        using var mat = new Mat(ImagePath.Hand, ImreadModes.Color);
        var matRows = mat.AsRows<Vec3b>();
        int height = mat.Height;
        int width = mat.Width;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vec3b color = matRows[y][x];
                matRows[y][x] = new Vec3b(color.Item2, color.Item1, color.Item0);
            }
        }
        //Cv2.ImShow("GenericIndexer", mat);
        //Cv2.WaitKey(0);
        //Cv2.DestroyAllWindows();
    }

    /// <summary>
    /// Faster
    /// </summary>
    private void TypeSpecificMat()
    {
        using var mat = new Mat(ImagePath.Hand, ImreadModes.Color);
        var mat3 = new Mat<Vec3b>(mat);
        var indexer = mat3.GetIndexer();
        int height = mat.Height;
        int width = mat.Width;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var color = indexer[y, x];
                var newColor = new Vec3b(color.Item2, color.Item1, color.Item0);
                indexer[y, x] = newColor;
            }
        }
        //Cv2.ImShow("TypeSpecificMat", mat);
        //Cv2.WaitKey(0);
        //Cv2.DestroyAllWindows();
    }

    private static long MeasureTime(Action action)
    {
        var watch = Stopwatch.StartNew();
        action();
        watch.Stop();
        return watch.ElapsedMilliseconds;
    }
}
