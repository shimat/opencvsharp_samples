using System;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.XImgProc;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

internal class BinarizerSample : ConsoleTestBase
{
    public override void RunTest()
    {
        using var src = Cv2.ImRead(ImagePath.Binarization, ImreadModes.Grayscale);
        using var niblack = new Mat();
        using var sauvola = new Mat();
        using var nick = new Mat();
        int kernelSize = 51;

        var sw = new Stopwatch();
        sw.Start();
        Cv2.XImgProc.NiblackThreshold(src, niblack, 255, ThresholdTypes.Binary, kernelSize, -0.2, LocalBinarizationMethods.Niblack);
        sw.Stop();
        Console.WriteLine($"Niblack {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        Cv2.XImgProc.NiblackThreshold(src, sauvola, 255, ThresholdTypes.Binary, kernelSize, 0.1, LocalBinarizationMethods.Sauvola);
        sw.Stop();
        Console.WriteLine($"Sauvola {sw.ElapsedMilliseconds} ms");

        sw.Restart();
        Cv2.XImgProc.NiblackThreshold(src, nick, 255, ThresholdTypes.Binary, kernelSize, -0.14, LocalBinarizationMethods.Nick);
        sw.Stop();
        Console.WriteLine($"Nick {sw.ElapsedMilliseconds} ms");

        DisplayHelper.Show(nameof(BinarizerSample), new[] { "src", "Niblack", "Sauvola", "Nick" }, new[] { src, niblack, sauvola, nick });
    }
}
