using OpenCvSharp;
using SampleBase;

namespace SamplesLegacy
{
    /// <summary>
    /// sample of photo module methods
    /// </summary>
    class PhotoMethods : ConsoleTestBase
    {
        public override void RunTest()
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

            using (new Window("src", src))
            using (new Window("edgePreservingFilter - NormconvFilter", normconv))
            using (new Window("edgePreservingFilter - RecursFilter", recursFiltered))
            using (new Window("detailEnhance", detailEnhance))
            using (new Window("pencilSketch grayscale", pencil1))
            using (new Window("pencilSketch color", pencil2))
            using (new Window("stylized", stylized))
            {
                Cv2.WaitKey();
            }
        }
    }
}