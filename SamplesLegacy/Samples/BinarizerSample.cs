using System;
using System.Diagnostics;
using OpenCvSharp;
using OpenCvSharp.XImgProc;
using SampleBase;

namespace SamplesLegacy
{
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
            CvXImgProc.NiblackThreshold(src, niblack, 255, ThresholdTypes.Binary, kernelSize, -0.2, LocalBinarizationMethods.Niblack);
            sw.Stop();
            Console.WriteLine($"Niblack {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            CvXImgProc.NiblackThreshold(src, sauvola, 255, ThresholdTypes.Binary, kernelSize, 0.1, LocalBinarizationMethods.Sauvola);
            sw.Stop();
            Console.WriteLine($"Sauvola {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            CvXImgProc.NiblackThreshold(src, nick, 255, ThresholdTypes.Binary, kernelSize, -0.14, LocalBinarizationMethods.Nick);
            sw.Stop();
            Console.WriteLine($"Nick {sw.ElapsedMilliseconds} ms");

            using (new Window("src", src, WindowFlags.AutoSize))
            using (new Window("Niblack", niblack, WindowFlags.AutoSize))
            using (new Window("Sauvola", sauvola, WindowFlags.AutoSize))
            using (new Window("Nick", nick, WindowFlags.AutoSize))
            {
                Cv2.WaitKey();
            }
        }
    }
}
