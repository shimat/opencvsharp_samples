using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Removes a text watermark from an image via inpainting.
/// </summary>
/// <remarks>
/// There's no single "remove any watermark" algorithm: the classic approach is inpainting,
/// which needs a mask of exactly where the watermark pixels are. Real-world watermarks
/// (semi-transparent stamps, repeating logos) require detecting that mask first (thresholding,
/// template matching, or a trained model), which is well beyond a single sample. To keep this
/// self-contained and runnable headlessly, a synthetic text watermark is stamped onto a stock
/// image and its mask is known exactly by construction, isolating the actual point of interest:
/// what to do with the mask once you have it.
/// </remarks>
[SampleCategory(SampleCategory.Photo)]
class WatermarkRemovalSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var original = Cv2.ImRead(ImagePath.Fruits, ImreadModes.Color);

        using var watermarked = original.Clone();
        using var mask = new Mat(original.Size(), MatType.CV_8UC1, Scalar.Black);

        const string text = "SAMPLE WATERMARK";
        var textOrigin = new Point(20, original.Rows - 20);
        const double fontScale = 1.0;
        const int thickness = 2;

        // Semi-transparent stamp: blend text directly into a copy of the image...
        using var watermarkLayer = original.Clone();
        Cv2.PutText(watermarkLayer, text, textOrigin, HersheyFonts.HersheySimplex, fontScale, Scalar.White, thickness, LineTypes.AntiAlias);
        Cv2.AddWeighted(original, 0.5, watermarkLayer, 0.5, 0, watermarked);

        // ...and draw the same text at full opacity into a separate mask, so the exact set of
        // watermark pixels is known without having to detect anything.
        Cv2.PutText(mask, text, textOrigin, HersheyFonts.HersheySimplex, fontScale, Scalar.White, thickness, LineTypes.AntiAlias);
        Cv2.Dilate(mask, mask, default, iterations: 1);

        using var restoredTelea = new Mat();
        Cv2.Inpaint(watermarked, mask, restoredTelea, 3, InpaintTypes.Telea);

        using var restoredNs = new Mat();
        Cv2.Inpaint(watermarked, mask, restoredNs, 3, InpaintTypes.NS);

        display.Show(
            ("original", original),
            ("watermarked", watermarked),
            ("watermark mask", mask),
            ("restored (Telea)", restoredTelea),
            ("restored (Navier-Stokes)", restoredNs));
    }
}
