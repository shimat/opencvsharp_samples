using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the face model available here: http://posefs1.perception.cs.cmu.edu/OpenPose/models/hand/pose_iter_102000.caffemodel
    /// Add the files to the bin folder
    /// </summary>
    class HandPose : ISample
    {
        public void Run()
        {
			string model = "pose_iter_102000.caffemodel";
			string modelTxt = "pose_deploy.prototxt";
			const string sampleImage = "hand.jpg";
			const string outputLoc = "Output_Hand.jpg";
			const int nPoints = 22;
			const double thresh = 0.01;
			
			int[][] POSE_PAIRS =
			{
				new int[] { 0, 1}, new int[] {1, 2}, new int[] {2,3}, new int[] {3,4}, //thumb
				new int[] { 0, 5}, new int[] {5, 6}, new int[] {6,7}, new int[] {7,8}, //index
				new int[] { 0, 9}, new int[] {9, 10}, new int[] {10,11}, new int[] {11,12}, //middle
				new int[] { 0, 13}, new int[] {13, 14}, new int[] {14,15}, new int[] {15,16}, //ring
				new int[] { 0, 17}, new int[] {17, 18}, new int[] {18,19}, new int[] {19,20}, //small
			};
            
            var frame = Cv2.ImRead(sampleImage);
            Mat frameCopy = frame.Clone();
            int frameWidth = frame.Cols;
            int frameHeight = frame.Rows;

            float aspect_ratio = frameWidth / (float)frameHeight;
            int inHeight = 368;
            int inWidth = ((int)(aspect_ratio * inHeight) * 8) / 8;

            var net = CvDnn.ReadNetFromCaffe(modelTxt, model);
            Mat inpBlob = CvDnn.BlobFromImage(frame, 1.0 / 255, new OpenCvSharp.Size(inWidth, inHeight), new Scalar(0, 0, 0), false, false);
			
            net.SetInput(inpBlob);

            Mat output = net.Forward();
            int H = output.Size(2);
            int W = output.Size(3);

            List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();

            for (int n = 0; n < nPoints; n++)
            {
                // Probability map of corresponding body's part.
                Mat probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0,n));
                Cv2.Resize(probMap, probMap, new OpenCvSharp.Size(frameWidth, frameHeight));
                double minVal, maxVal;
                OpenCvSharp.Point minLoc, maxLoc;
                Cv2.MinMaxLoc(probMap, out minVal, out maxVal, out minLoc, out maxLoc);

                if (maxVal > thresh)
                {
                    Cv2.Circle(frameCopy, (int)maxLoc.X, (int)maxLoc.Y, 8, new Scalar(0, 255, 255), -1, LineTypes.Link8);
                    Cv2.PutText(frameCopy, Cv2.Format(n), new OpenCvSharp.Point((int)maxLoc.X, (int)maxLoc.Y), HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
                }

                points.Add(maxLoc);
            }

            int nPairs = 20;//(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

            for (int n = 0; n < nPairs; n++)
            {
                // lookup 2 connected body/hand parts
                Point2f partA = points[POSE_PAIRS[n][0]];
                Point2f partB = points[POSE_PAIRS[n][1]];

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