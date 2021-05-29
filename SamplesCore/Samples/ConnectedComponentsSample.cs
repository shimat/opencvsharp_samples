using System.Linq;
using OpenCvSharp;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// 
    /// </summary>
    class ConnectedComponentsSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            using var src = new Mat(ImagePath.Shapes, ImreadModes.Color);
            using var gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            using var binary = gray.Threshold(0, 255, ThresholdTypes.Otsu | ThresholdTypes.Binary);
            using var labelView = src.EmptyClone();
            using var rectView = binary.CvtColor(ColorConversionCodes.GRAY2BGR);

            var cc = Cv2.ConnectedComponentsEx(binary);
            if (cc.LabelCount <= 1)
                return;

            // draw labels
            cc.RenderBlobs(labelView);

            // draw bonding boxes except background
            foreach (var blob in cc.Blobs.Skip(1))
            {
                rectView.Rectangle(blob.Rect, Scalar.Red);
            }

            // filter maximum blob
            var maxBlob = cc.GetLargestBlob();
            var filtered = new Mat();
            cc.FilterByBlob(src, filtered, maxBlob);

            using (new Window("src", src))
            using (new Window("binary", binary))
            using (new Window("labels", labelView))
            using (new Window("bonding boxes", rectView))
            using (new Window("maximum blob", filtered))
            {
                Cv2.WaitKey();
            }
        }
    }
}