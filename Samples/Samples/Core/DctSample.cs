using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// DCT, inverse DCT, and the energy-compaction property that makes DCT useful for compression
/// (the same property JPEG relies on): most of the image's information ends up concentrated in
/// a handful of low-frequency coefficients, so keeping only those and discarding the rest still
/// gives a recognizable reconstruction.
/// </summary>
[SampleCategory(SampleCategory.Core)]
class DctSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var img = Cv2.ImRead(ImagePath.Walkman, ImreadModes.Grayscale);

        // 2D DCT requires even width/height; crop off the last row/column if needed.
        using var cropped = img[new Rect(0, 0, img.Cols & -2, img.Rows & -2)];

        using var imgF32 = new Mat();
        cropped.ConvertTo(imgF32, MatType.CV_32F);

        using var dct = new Mat();
        Cv2.Dct(imgF32, dct);

        // Log-scale magnitude, just to make the coefficient spectrum visible: it's heavily
        // concentrated in the top-left (low-frequency) corner.
        using var spectrum = new Mat();
        Cv2.Abs(dct).ToMat().ConvertTo(spectrum, MatType.CV_32F);
        using Mat spectrum1 = spectrum + Scalar.All(1);
        Cv2.Log(spectrum1, spectrum1);
        Cv2.Normalize(spectrum1, spectrum1, 0, 255, NormTypes.MinMax);
        spectrum1.ConvertTo(spectrum1, MatType.CV_8U);

        // Keep only the low-frequency coefficients (an 1/8-sized top-left block) and zero out
        // the rest, then reconstruct: this is the lossy "compression" case.
        using var dctLowFreqOnly = Mat.Zeros(dct.Size(), MatType.CV_32F).ToMat();
        var keepRect = new Rect(0, 0, dct.Cols / 8, dct.Rows / 8);
        using (var src = dct[keepRect])
        using (var dst = dctLowFreqOnly[keepRect])
        {
            src.CopyTo(dst);
        }

        using var compressed = new Mat();
        Cv2.Idct(dctLowFreqOnly, compressed);
        Cv2.Normalize(compressed, compressed, 0, 255, NormTypes.MinMax);
        compressed.ConvertTo(compressed, MatType.CV_8U);

        // Reconstruct from every coefficient: should match the input almost exactly.
        using var reconstructed = new Mat();
        Cv2.Idct(dct, reconstructed);
        reconstructed.ConvertTo(reconstructed, MatType.CV_8U);

        display.Show(
            ("Input Image", cropped),
            ("DCT Spectrum (log magnitude)", spectrum1),
            ($"Compressed (top-left {keepRect.Width}x{keepRect.Height} coefficients only)", compressed),
            ("Reconstructed by full Inverse DCT", reconstructed));
    }
}
