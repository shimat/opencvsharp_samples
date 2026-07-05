using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Detects and decodes a QR code using QRCodeDetector.
/// </summary>
[SampleCategory(SampleCategory.ObjDetect)]
class QRCodeDetectionSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var src = new Mat(ImagePath.QrCode, ImreadModes.Color);

        using var detector = new QRCodeDetector();
        using var straightQrCode = new Mat();
        string text = detector.DetectAndDecode(src, out Point2f[] points, straightQrCode);

        using var dst = src.Clone();
        if (points.Length > 0)
        {
            var corners = Array.ConvertAll(points, p => (Point)p);
            Cv2.Polylines(dst, new[] { corners }, true, Scalar.Red, 3, LineTypes.AntiAlias);
        }

        Console.WriteLine(string.IsNullOrEmpty(text)
            ? "No QR code found."
            : $"Decoded text: {text}");

        display.Show(("detected", dst), ("rectified", straightQrCode));
    }
}
