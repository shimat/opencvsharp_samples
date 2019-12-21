using System.Linq;
using OpenCvSharp;
using SampleBase;

namespace SamplesCS
{
    /// <summary>
    /// https://stackoverflow.com/questions/51606215/how-to-draw-bounding-box-on-best-matches/51607041#51607041
    /// </summary>
    class DrawBestMatchRectangle : ISample
    {
        public void Run()
        {
            using var img1 = new Mat(FilePath.Image.Match1, ImreadModes.Color);
            using var img2 = new Mat(FilePath.Image.Match2, ImreadModes.Color);

            using var orb = ORB.Create(1000);
            using var descriptors1 = new Mat();
            using var descriptors2 = new Mat();
            orb.DetectAndCompute(img1, null, out var keyPoints1, descriptors1);
            orb.DetectAndCompute(img2, null, out var keyPoints2, descriptors2);

            using var bf = new BFMatcher(NormTypes.Hamming, crossCheck: true);
            var matches = bf.Match(descriptors1, descriptors2);

            var goodMatches = matches
                .OrderBy(x => x.Distance)
                .Take(10)
                .ToArray();

            var srcPts = goodMatches.Select(m => keyPoints1[m.QueryIdx].Pt).Select(p => new Point2d(p.X, p.Y));
            var dstPts = goodMatches.Select(m => keyPoints2[m.TrainIdx].Pt).Select(p => new Point2d(p.X, p.Y));

            using var homography = Cv2.FindHomography(srcPts, dstPts, HomographyMethods.Ransac, 5, null);

            int h = img1.Height, w = img1.Width;
            var img2Bounds = new[]
            {
                new Point2d(0, 0), 
                new Point2d(0, h-1),
                new Point2d(w-1, h-1), 
                new Point2d(w-1, 0),
            };
            var img2BoundsTransformed = Cv2.PerspectiveTransform(img2Bounds, homography);

            using var view = img2.Clone();
            var drawingPoints = img2BoundsTransformed.Select(p => (Point) p).ToArray();
            Cv2.Polylines(view, new []{drawingPoints}, true, Scalar.Red, 3);

            using (new Window("view", view))
            {
                Cv2.WaitKey();
            }
        }
    }
}