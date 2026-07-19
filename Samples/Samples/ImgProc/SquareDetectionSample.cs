using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Detects squares/rectangles in images using Canny, findContours and approxPolyDP.
/// A C# port of the official OpenCV squares.cpp sample.
/// </summary>
/// <remarks>https://github.com/opencv/opencv/blob/4.x/samples/cpp/squares.cpp</remarks>
[SampleCategory(SampleCategory.ImgProc)]
class SquareDetectionSample : ConsoleTestBase
{
    private const int Thresh = 50;
    private const int Levels = 11;

    private static readonly string[] ImagePaths =
    [
        ImagePath.Square1,
        ImagePath.Square2,
        ImagePath.Square3,
        ImagePath.Square4,
        ImagePath.Square5,
        ImagePath.Square6,
    ];

    public override void RunTest(DisplayHelper display)
    {
        foreach (var path in ImagePaths)
        {
            using var src = new Mat(path, ImreadModes.Color);
            var squares = FindSquares(src);

            using var dst = src.Clone();
            Cv2.Polylines(dst, squares, true, Scalar.Red, 3, LineTypes.AntiAlias);

            display.Show((path, dst));
        }
    }

    /// <summary>
    /// Returns the cosine of the angle at pt0, between the vectors pt0-&gt;pt1 and pt0-&gt;pt2.
    /// </summary>
    private static double Angle(Point pt1, Point pt2, Point pt0)
    {
        double dx1 = pt1.X - pt0.X, dy1 = pt1.Y - pt0.Y;
        double dx2 = pt2.X - pt0.X, dy2 = pt2.Y - pt0.Y;
        return (dx1 * dx2 + dy1 * dy2) / Math.Sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
    }

    private static List<Point[]> FindSquares(Mat image)
    {
        // Blur helps remove noise that would otherwise create spurious contours
        using var pyrDown = new Mat();
        using var blurred = new Mat();
        Cv2.PyrDown(image, pyrDown, new Size(image.Width / 2, image.Height / 2));
        Cv2.PyrUp(pyrDown, blurred, image.Size());

        var squares = new List<Point[]>();
        Cv2.Split(blurred, out var planes);

        foreach (var plane in planes)
        {
            using var planeDisposer = plane;
            for (int level = 0; level < Levels; level++)
            {
                using var gray = new Mat();
                if (level == 0)
                {
                    Cv2.Canny(plane, gray, 0, Thresh, apertureSize: 5);
                    Cv2.Dilate(gray, gray, default);
                }
                else
                {
                    Cv2.Threshold(plane, gray, (level + 1) * 255.0 / Levels, 255, ThresholdTypes.Binary);
                }

                Cv2.FindContours(gray, out var contours, out _, RetrievalModes.List, ContourApproximationModes.ApproxSimple);

                foreach (var contour in contours)
                {
                    var approx = Cv2.ApproxPolyDP(contour, Cv2.ArcLength(contour, true) * 0.02, true);
                    if (approx.Length != 4 ||
                        Math.Abs(Cv2.ContourArea(approx)) <= 1000 ||
                        !Cv2.IsContourConvex(approx))
                    {
                        continue;
                    }

                    double maxCosine = 0;
                    for (int j = 2; j < 5; j++)
                    {
                        double cosine = Math.Abs(Angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                        maxCosine = Math.Max(maxCosine, cosine);
                    }

                    if (maxCosine < 0.3)
                    {
                        squares.Add(approx);
                    }
                }
            }
        }

        return squares;
    }
}
