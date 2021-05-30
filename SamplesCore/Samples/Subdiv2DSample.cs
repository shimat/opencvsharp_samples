using System;
using System.Linq;
using OpenCvSharp;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// cv::Subdiv2D test
    /// </summary>
    class Subdiv2DSample : ConsoleTestBase
    {
        public override void RunTest()
        {
            const int Size = 600;

            // Creates random point list
            var rand = new Random();
            var points = Enumerable.Range(0, 100).Select(_ =>
                new Point2f(rand.Next(0, Size), rand.Next(0, Size))).ToArray();

            using var imgExpr = Mat.Zeros(Size, Size, MatType.CV_8UC3);
            using var img = imgExpr.ToMat();
            foreach (var p in points)
            {
                img.Circle((Point)p, 4, Scalar.Red, -1);
            }

            // Initializes Subdiv2D
            using var subdiv = new Subdiv2D();
            subdiv.InitDelaunay(new Rect(0, 0, Size, Size));
            subdiv.Insert(points);

            // Draws voronoi diagram
            subdiv.GetVoronoiFacetList(null, out var facetList, out var facetCenters);

            using var vonoroi = img.Clone();
            foreach (var list in facetList)
            {
                var before = list.Last();
                foreach (var p in list)
                {
                    vonoroi.Line((Point)before, (Point)p, new Scalar(64, 255, 128), 1);
                    before = p;
                }
            }

            // Draws delaunay diagram
            Vec4f[] edgeList = subdiv.GetEdgeList();
            using var delaunay = img.Clone();
            foreach (var edge in edgeList)
            {
                var p1 = new Point(edge.Item0, edge.Item1);
                var p2 = new Point(edge.Item2, edge.Item3);
                delaunay.Line(p1, p2, new Scalar(64, 255, 128), 1);
            }

            Cv2.ImShow("voronoi", vonoroi);
            Cv2.ImShow("delaunay", delaunay);
            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
        }
    }
}