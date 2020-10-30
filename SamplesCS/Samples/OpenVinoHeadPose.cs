using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first you nedd to compile OPENCV with Intel OpenVino
    /// Download the face detection model available here: https://github.com/openvinotoolkit/open_model_zoo/tree/master/models/intel/face-detection-adas-0001
    /// Download the head pose model available here: https://github.com/openvinotoolkit/open_model_zoo/tree/master/models/intel/head-pose-estimation-adas-0001
    /// Add the files to the bin folder
    /// </summary>
    internal class OpenVinoFaceDetection : ISample
    {
        public void Run()
        {
	        const string modelFace = "face-detection-adas-0001.bin";
            const string modelFaceTxt = "face-detection-adas-0001.xml";
            const string modelHead = "head-pose-estimation-adas-0001.bin";
            const string modelHeadTxt = "head-pose-estimation-adas-0001.xml";
            const string sampleImage = "sample.jpg";
            const string outputLoc = "sample_output.jpg";

            var frame = Cv2.ImRead(sampleImage);
            int frameHeight = frame.Rows;
            int frameWidth = frame.Cols;
            var netFace = CvDnn.ReadNet(modelFace, modelFaceTxt);
            var netHead = CvDnn.ReadNet(modelHead, modelHeadTxt);

            netFace.SetPreferableBackend(Net.Backend.INFERENCE_ENGINE);
            netFace.SetPreferableTarget(Net.Target.CPU);
            netHead.SetPreferableBackend(Net.Backend.INFERENCE_ENGINE);
            netHead.SetPreferableTarget(Net.Target.CPU);

            var blob = CvDnn.BlobFromImage(frame, 1.0, new OpenCvSharp.Size(672, 384), new OpenCvSharp.Scalar(0, 0, 0), false, false);
            netFace.SetInput(blob);

            using (var detection = netFace.Forward())
            {
                Mat detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0));
                for (int i = 0; i < detectionMat.Rows; i++)
                {
                    float confidence = detectionMat.At<float>(i, 2);

                    if (confidence > 0.7)
                    {
                        int x1 = (int)(detectionMat.At<float>(i, 3) * frameWidth); //xmin
                        int y1 = (int)(detectionMat.At<float>(i, 4) * frameHeight); //ymin
                        int x2 = (int)(detectionMat.At<float>(i, 5) * frameWidth); //xmax
                        int y2 = (int)(detectionMat.At<float>(i, 6) * frameHeight); //ymax                            

                        OpenCvSharp.Rect roi = new OpenCvSharp.Rect(x1, y1, (x2 - x1), (y2 - y1));
                        roi = AdjustBoundingBox(roi);
                        Mat face = new Mat(frame, roi);

                        var blob2 = CvDnn.BlobFromImage(face, 1.0, new OpenCvSharp.Size(60, 60), new OpenCvSharp.Scalar(0, 0, 0), false, false);
                        netHead.SetInput(blob2);

                        IEnumerable<string> outNames = netHead.GetUnconnectedOutLayersNames();
                        IEnumerable<Mat> outputBlobs = new List<Mat>() { new Mat(), new Mat(), new Mat() };

                        netHead.Forward(outputBlobs, outNames);

                        Point3f headAngles = HeadAngles(outputBlobs);
                        string printAngles = "Yaw " + headAngles.Y.ToString() + " | Pitch " + headAngles.X.ToString() + " | Roll " + headAngles.Z.ToString();

                        Cv2.Rectangle(frame, roi, new Scalar(0, 255, 0), 2, LineTypes.Link4);
                        Cv2.PutText(frame, printAngles, new OpenCvSharp.Point(10,500), HersheyFonts.HersheyComplex, 1.0, new Scalar(0, 255, 0), 2);
                    }
                }
            }
								
            var finalOutput = outputLoc;
            Cv2.ImWrite(finalOutput, frame);
        }

        private OpenCvSharp.Rect AdjustBoundingBox(OpenCvSharp.Rect faceRect)
        {
            int w = faceRect.Width;
            int h = faceRect.Height;

            faceRect.X -= (int)(0.067 * w);
            faceRect.Y -= (int)(0.028 * h);

            faceRect.Width += (int)(0.15 * w);
            faceRect.Height += (int)(0.13 * h);

            if (faceRect.Width < faceRect.Height)
            {
                var dx = (faceRect.Height - faceRect.Width);
                faceRect.X -= dx / 2;
                faceRect.Width += dx;
            }
            else
            {
                var dy = (faceRect.Width - faceRect.Height);
                faceRect.Y -= dy / 2;
                faceRect.Height += dy;
            }
            return faceRect;
        }
		
        private Point3f HeadAngles(IEnumerable<Mat> outAngles)
        {
            Point3f headAngles = new Point3f();
            headAngles.X = outAngles.ElementAt(0).At<float>(0, 0);
            headAngles.X = outAngles.ElementAt(0).At<float>(0, 0);
            headAngles.Y = outAngles.ElementAt(2).At<float>(0, 0);
            headAngles.Z = outAngles.ElementAt(1).At<float>(0, 0);

            return headAngles;
        }
    }
}
