using OpenCvSharp;
using SampleBase;

namespace SamplesCS
{
    internal class SimpleBlobDetectorSample : ISample
    {
        public void Run()
        {
            using var src = Cv2.ImRead(FilePath.Image.Shapes);
            using var detectedCircles = new Mat();
            using var detectedOvals = new Mat();

            // Invert the image. Shapes has a black background and SimpleBlobDetector doesn't seem to work well with that.
            Cv2.BitwiseNot(src, src);

            // Parameters tuned to detect only circles
            var circleParams = new SimpleBlobDetector.Params
            {
                MinThreshold = 10,
                MaxThreshold = 230,

                // The area is the number of pixels in the blob.
                FilterByArea = true,
                MinArea = 500,
                MaxArea = 50000,

                // Circularity is a ratio of the area to the perimeter. Polygons with more sides are more circular.
                FilterByCircularity = true,
                MinCircularity = 0.9f,

                // Convexity is the ratio of the area of the blob to the area of its convex hull.
                FilterByConvexity = true,
                MinConvexity = 0.95f,

                // A circle's inertia ratio is 1. A line's is 0. An oval is between 0 and 1.
                FilterByInertia = true,
                MinInertiaRatio = 0.95f
            };

            // Parameters tuned to find the ovals in the Shapes image.
            var ovalParams = new SimpleBlobDetector.Params
            {
                MinThreshold = 10,
                MaxThreshold = 230,
                FilterByArea = true,
                MinArea = 500,
                // The ovals are the smallest blobs in Shapes, so we limit the max area to eliminate the larger blobs.
                MaxArea = 10000,
                FilterByCircularity = true,
                MinCircularity = 0.58f,
                FilterByConvexity = true,
                MinConvexity = 0.96f,
                FilterByInertia = true,
                MinInertiaRatio = 0.1f
            };

            using var circleDetector = SimpleBlobDetector.Create(circleParams);
            var circleKeyPoints = circleDetector.Detect(src);
            Cv2.DrawKeypoints(src, circleKeyPoints, detectedCircles, Scalar.HotPink, DrawMatchesFlags.DrawRichKeypoints);

            using var ovalDetector = SimpleBlobDetector.Create(ovalParams);
            var ovalKeyPoints = ovalDetector.Detect(src);
            Cv2.DrawKeypoints(src, ovalKeyPoints, detectedOvals, Scalar.HotPink, DrawMatchesFlags.DrawRichKeypoints);

            using var _1 = new Window("Detected Circles", WindowMode.AutoSize, detectedCircles);
            using var _2 = new Window("Detected Ovals", WindowMode.AutoSize, detectedOvals);

            Cv2.WaitKey();

        }
    }
}
