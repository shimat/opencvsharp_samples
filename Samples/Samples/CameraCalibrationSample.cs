using System;
using System.Collections.Generic;
using System.Linq;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Calibrates a camera from chessboard images and undistorts one of them.
/// A C# port of the official OpenCV camera calibration tutorial.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/d4/d94/tutorial_camera_calibration.html</remarks>
class CameraCalibrationSample : ConsoleTestBase
{
    private static readonly Size PatternSize = new(9, 6);

    private static readonly string[] ImagePaths = Enumerable.Range(1, 13)
        .Select(i => string.Format(ImagePath.CalibrationLeft, i))
        .ToArray();

    public override void RunTest(DisplayHelper display)
    {
        var objectPoints = new List<Point3f[]>();
        var imagePoints = new List<Point2f[]>();
        Size imageSize = default;
        var annotated = new List<Mat>();

        var objectCorners = new Point3f[PatternSize.Width * PatternSize.Height];
        for (int y = 0; y < PatternSize.Height; y++)
        {
            for (int x = 0; x < PatternSize.Width; x++)
            {
                objectCorners[y * PatternSize.Width + x] = new Point3f(x, y, 0);
            }
        }

        foreach (var path in ImagePaths)
        {
            using var gray = new Mat(path, ImreadModes.Grayscale);
            if (gray.Empty())
            {
                continue;
            }
            imageSize = gray.Size();

            bool found = Cv2.FindChessboardCorners(gray, PatternSize, out var corners,
                ChessboardFlags.AdaptiveThresh | ChessboardFlags.NormalizeImage);

            var color = new Mat(path, ImreadModes.Color);
            if (found)
            {
                Cv2.CornerSubPix(gray, corners, new Size(11, 11), new Size(-1, -1),
                    new TermCriteria(CriteriaTypes.Eps | CriteriaTypes.MaxIter, 30, 0.1));

                objectPoints.Add(objectCorners);
                imagePoints.Add(corners);
            }
            Cv2.DrawChessboardCorners(color, PatternSize, corners, found);
            annotated.Add(color);
        }

        Console.WriteLine($"Detected the chessboard in {imagePoints.Count}/{ImagePaths.Length} images");

        var objectPointsMats = objectPoints.Select(Mat.FromArray).ToArray();
        var imagePointsMats = imagePoints.Select(Mat.FromArray).ToArray();

        using var cameraMatrix = new Mat();
        using var distCoeffs = new Mat();
        double rms = Cv2.CalibrateCamera(
            objectPointsMats, imagePointsMats, imageSize, cameraMatrix, distCoeffs,
            out _, out _);

        foreach (var m in objectPointsMats) m.Dispose();
        foreach (var m in imagePointsMats) m.Dispose();

        Console.WriteLine($"Re-projection error (RMS): {rms:F4}");
        Console.WriteLine($"Camera matrix:\n{cameraMatrix.Dump()}");
        Console.WriteLine($"Distortion coefficients:\n{distCoeffs.Dump()}");

        using var sample = new Mat(ImagePaths[0], ImreadModes.Color);
        using var undistorted = new Mat();
        Cv2.Undistort(sample, undistorted, cameraMatrix, distCoeffs);

        foreach (var img in annotated)
        {
            using (img)
            {
                display.Show(("detected corners", img));
            }
        }

        display.Show(("original", sample), ("undistorted", undistorted));
    }
}
