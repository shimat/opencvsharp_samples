using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the hand model available here: http://posefs1.perception.cs.cmu.edu/OpenPose/models/hand/pose_iter_102000.caffemodel
    /// Or also available here https://github.com/CMU-Perceptual-Computing-Lab/openpose/tree/master/models
    /// Add the files to the bin folder
    /// </summary>
    internal class HandPose : ISample
    {
        public void Run()
        {
            const string model = "pose_iter_102000.caffemodel";
            const string modelTxt = "pose_deploy.prototxt";
            const string sampleImage = "hand.jpg";
            const string outputLoc = "Output_Hand.jpg";
            const int nPoints = 22;
            const double thresh = 0.01;

            int[][] posePairs =
            {
                new[] {0, 1}, new[] {1, 2}, new[] {2, 3}, new[] {3, 4}, //thumb
                new[] {0, 5}, new[] {5, 6}, new[] {6, 7}, new[] {7, 8}, //index
                new[] {0, 9}, new[] {9, 10}, new[] {10, 11}, new[] {11, 12}, //middle
                new[] {0, 13}, new[] {13, 14}, new[] {14, 15}, new[] {15, 16}, //ring
                new[] {0, 17}, new[] {17, 18}, new[] {18, 19}, new[] {19, 20}, //small
            };

            using var frame = Cv2.ImRead(sampleImage);
            using var frameCopy = frame.Clone();
            int frameWidth = frame.Cols;
            int frameHeight = frame.Rows;

            float aspectRatio = frameWidth / (float) frameHeight;
            int inHeight = 368;
            int inWidth = ((int) (aspectRatio * inHeight) * 8) / 8;

            using var net = CvDnn.ReadNetFromCaffe(modelTxt, model);
            using var inpBlob = CvDnn.BlobFromImage(frame, 1.0 / 255, new Size(inWidth, inHeight),
                new Scalar(0, 0, 0), false, false);

            net.SetInput(inpBlob);

            using var output = net.Forward();
            int H = output.Size(2);
            int W = output.Size(3);

            var points = new List<Point>();

            for (int n = 0; n < nPoints; n++)
            {
                // Probability map of corresponding body's part.
                using var probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0, n));
                Cv2.Resize(probMap, probMap, new Size(frameWidth, frameHeight));
                Cv2.MinMaxLoc(probMap, out _, out var maxVal, out _, out var maxLoc);

                if (maxVal > thresh)
                {
                    Cv2.Circle(frameCopy, maxLoc.X, maxLoc.Y, 8, new Scalar(0, 255, 255), -1,
                        LineTypes.Link8);
                    Cv2.PutText(frameCopy, Cv2.Format(n), new OpenCvSharp.Point(maxLoc.X, maxLoc.Y),
                        HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
                }

                points.Add(maxLoc);
            }

            int nPairs = 20; //(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

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
