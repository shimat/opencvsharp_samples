using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the pose model available here: https://github.com/CMU-Perceptual-Computing-Lab/openpose/tree/master/models
    /// Add the files to the bin folder
    /// </summary>
    class Pose : ISample
    {
        public void Run()
        {
            const string Model = "pose_iter_160000.caffemodel";
            const string ModelTxt = "pose_deploy_linevec_faster_4_stages.prototxt";
            const string sampleImage = "single.jpeg";
            const string outputLoc = "Output-Skeleton.jpg";
            const int nPoints = 15;
            const double thresh = 0.1;

	    int[][] POSE_PAIRS =
	    {
		new int[] { 0, 1}, new int[] {1, 2}, new int[] {2, 3},
		new int[] { 3, 4}, new int[] {1, 5}, new int[] {5, 6},
		new int[] { 6, 7}, new int[] {1, 14}, new int[] {14,8}, new int[] {8, 9},
		new int[] { 9, 10}, new int[] {14, 11}, new int[] {11,12}, new int[] {12, 13},
	    };
            
            var image = sampleImage;
            var frame = Cv2.ImRead(image);
            Mat frameCopy = frame.Clone();
            int frameWidth = frame.Cols;
            int frameHeight = frame.Rows;

            int inWidth = 368;
            int inHeight = 368;

            var net = CvDnn.ReadNetFromCaffe(ModelTxt, Model);
            net.SetPreferableBackend(Net.Backend.OPENCV);
            net.SetPreferableTarget(Net.Target.CPU);

            Mat inpBlob = CvDnn.BlobFromImage(frame, 1.0 / 255, new OpenCvSharp.Size(inWidth, inHeight), new Scalar(0, 0, 0), false, false);

            net.SetInput(inpBlob);

            Mat output = net.Forward();
            int H = output.Size(2);
            int W = output.Size(3);

            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();

            for (int n = 0; n < nPoints; n++)
            {
                // Probability map of corresponding body's part.
                Mat probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0, n));

                Point2f p = new Point2f(-1,-1);
                OpenCvSharp.Point maxLoc;
                double minVal, maxVal;
                OpenCvSharp.Point minLoc;

                Cv2.MinMaxLoc(probMap, out minVal, out maxVal, out minLoc, out maxLoc);

                var x = (frameWidth * maxLoc.X) / W;
                var y = (frameHeight * maxLoc.Y) / H;

                if (maxVal > thresh)
                {
                    p = maxLoc;
                    p.X *= (float)frameWidth / W;
                    p.Y *= (float)frameHeight / H;

                    Cv2.Circle(frameCopy, (int)p.X, (int)p.Y, 8, new Scalar(0, 255, 255), -1);
                    Cv2.PutText(frameCopy, Cv2.Format(n), new OpenCvSharp.Point((int)p.X, (int)p.Y), HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 2);
                }

                points.Add((OpenCvSharp.Point)p);
            }
            int nPairs = 14; //(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

            for (int n = 0; n < nPairs; n++)
            {
                // lookup 2 connected body/hand parts
                Point2f partA = points[POSE_PAIRS[n][0]];
                Point partB = points[POSE_PAIRS[n][1]];

                if (partA.X <= 0 || partA.Y <= 0 || partB.X <= 0 || partB.Y <= 0)
                    continue;

                Cv2.Line(frame, (OpenCvSharp.Point)partA, (OpenCvSharp.Point)partB, new Scalar(0, 255, 255), 8);
                Cv2.Circle(frame, (int)partA.X, (int)partA.Y, 8, new Scalar(0, 0, 255), -1);
                Cv2.Circle(frame, (int)partB.X, (int)partB.Y, 8, new Scalar(0, 0, 255), -1);
            }
			
            var finalOutput = outputLoc;
            Cv2.ImWrite(finalOutput, frame);
        
        }

    }
}
