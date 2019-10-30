using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the pose model available here: https://github.com/CMU-Perceptual-Computing-Lab/openpose/tree/master/models
    /// Add the files to the bin folder
    /// </summary>
    internal class Pose : ISample
    {
        public void Run()
        {
            const string model = "pose_iter_160000.caffemodel";
            const string modelTxt = "pose_deploy_linevec_faster_4_stages.prototxt";
            const string sampleImage = "single.jpeg";
            const string outputLoc = "Output-Skeleton.jpg";
            const int nPoints = 15;
            const double thresh = 0.1;

            int[][] posePairs =
            {
                new[] {0, 1}, new[] {1, 2}, new[] {2, 3},
                new[] {3, 4}, new[] {1, 5}, new[] {5, 6},
                new[] {6, 7}, new[] {1, 14}, new[] {14, 8}, new[] {8, 9},
                new[] {9, 10}, new[] {14, 11}, new[] {11, 12}, new[] {12, 13},
            };
            
            using var frame = Cv2.ImRead(sampleImage);
            using var frameCopy = frame.Clone();
            int frameWidth = frame.Cols;
            int frameHeight = frame.Rows;

            const int inWidth = 368;
            const int inHeight = 368;

            using var net = CvDnn.ReadNetFromCaffe(modelTxt, model);
            net.SetPreferableBackend(Net.Backend.OPENCV);
            net.SetPreferableTarget(Net.Target.CPU);

            using var inpBlob = CvDnn.BlobFromImage(frame, 1.0 / 255, new Size(inWidth, inHeight), new Scalar(0, 0, 0), false, false);

            net.SetInput(inpBlob);

            using var output = net.Forward();
            int H = output.Size(2);
            int W = output.Size(3);

            var points = new List<Point>();

            for (int n = 0; n < nPoints; n++)
            {
                // Probability map of corresponding body's part.
                using var probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0, n));
                var p = new Point2f(-1,-1);

                Cv2.MinMaxLoc(probMap, out _, out var maxVal, out _, out var maxLoc);

                var x = (frameWidth * maxLoc.X) / W;
                var y = (frameHeight * maxLoc.Y) / H;

                if (maxVal > thresh)
                {
                    p = maxLoc;
                    p.X *= (float)frameWidth / W;
                    p.Y *= (float)frameHeight / H;

                    Cv2.Circle(frameCopy, (int)p.X, (int)p.Y, 8, new Scalar(0, 255, 255), -1);
                    Cv2.PutText(frameCopy, Cv2.Format(n), new Point((int)p.X, (int)p.Y), HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 2);
                }

                points.Add((Point)p);
            }
            int nPairs = 14; //(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

            for (int n = 0; n < nPairs; n++)
            {
                // lookup 2 connected body/hand parts
                Point partA = points[posePairs[n][0]];
                Point partB = points[posePairs[n][1]];

                if (partA.X <= 0 || partA.Y <= 0 || partB.X <= 0 || partB.Y <= 0)
                    continue;

                Cv2.Line(frame, partA, partB, new Scalar(0, 255, 255), 8);
                Cv2.Circle(frame, partA.X, partA.Y, 8, new Scalar(0, 0, 255), -1);
                Cv2.Circle(frame, partB.X, partB.Y, 8, new Scalar(0, 0, 255), -1);
            }
			
            var finalOutput = outputLoc;
            Cv2.ImWrite(finalOutput, frame);
        }
    }
}
