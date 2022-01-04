using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// To run this example first you nedd to compile OPENCV with Intel OpenVino
    /// Download the face detection model available here: https://github.com/openvinotoolkit/open_model_zoo/tree/master/models/intel/face-detection-adas-0001
    /// Add the files to the bin folder
    /// </summary>
    internal class OpenVinoFaceDetection : ConsoleTestBase
    {
        const string modelFace = "face-detection-adas-0001.bin"; 
        const string modelFaceTxt = "face-detection-adas-0001.xml";
        const string sampleImage = "sample.jpg";
        const string outputLoc = "sample_output.jpg";

        public override void RunTest()
        {
            using var frame = Cv2.ImRead(sampleImage);
            int frameHeight = frame.Rows;
            int frameWidth = frame.Cols;

            using var netFace = CvDnn.ReadNet(modelFace, modelFaceTxt);			
            netFace.SetPreferableBackend(Backend.INFERENCE_ENGINE);
            netFace.SetPreferableTarget(Target.CPU);
			
            using var blob = CvDnn.BlobFromImage(frame, 1.0, new Size(672, 384), new Scalar(0, 0, 0), false, false);
			netFace.SetInput(blob);
			
            using (var detection = netFace.Forward())
            {
                using var detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0));

                for (int i = 0; i < detectionMat.Rows; i++)
                {
                    float confidence = detectionMat.At<float>(i, 2);

                    if (confidence > 0.7)
                    {
                        int x1 = (int)(detectionMat.At<float>(i, 3) * frameWidth); //xmin
                        int y1 = (int)(detectionMat.At<float>(i, 4) * frameHeight); //ymin
                        int x2 = (int)(detectionMat.At<float>(i, 5) * frameWidth); //xmax
                        int y2 = (int)(detectionMat.At<float>(i, 6) * frameHeight); //ymax                            

                        var roi = new Rect(x1, y1, (x2 - x1), (y2 - y1));							
                        roi = AdjustBoundingBox(roi);							
                        Cv2.Rectangle(frame, roi, new Scalar(0, 255, 0), 2, LineTypes.Link4);
                    }
                }
            }
								
            var finalOutput = outputLoc;
            Cv2.ImWrite(finalOutput, frame);
        }

        private Rect AdjustBoundingBox(Rect faceRect)
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
    }
}
