using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// sample of photo module methods
/// </summary>
[SampleCategory(SampleCategory.Photo)]
class PhotoMethods : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        using var src = new Mat(ImagePath.Fruits, ImreadModes.Color);

        using var normconv = new Mat();
        using var recursFiltered = new Mat();
        Cv2.EdgePreservingFilter(src, normconv, EdgePreservingMethods.NormconvFilter);
        Cv2.EdgePreservingFilter(src, recursFiltered, EdgePreservingMethods.RecursFilter);

        using var detailEnhance = new Mat();
        Cv2.DetailEnhance(src, detailEnhance);

        using var pencil1 = new Mat();
        using var pencil2 = new Mat();
        Cv2.PencilSketch(src, pencil1, pencil2);

        using var stylized = new Mat();
        Cv2.Stylization(src, stylized);

        display.Show(
            ("src", src),
            ("edgePreservingFilter - NormconvFilter", normconv),
            ("edgePreservingFilter - RecursFilter", recursFiltered),
            ("detailEnhance", detailEnhance),
            ("pencilSketch grayscale", pencil1),
            ("pencilSketch color", pencil2),
            ("stylized", stylized));
    }
}
