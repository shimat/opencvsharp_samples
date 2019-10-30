using System;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the face model available here:https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
    /// Add the files to the bin folder
    /// </summary>
    internal class FaceDetectionDNN : ISample
    {
        public void Run()
        {
            const string configFile = "deploy.prototxt";
            const string faceModel = "res10_300x300_ssd_iter_140000_fp16.caffemodel";
            const string finalOutput = "DetectedFaces.jpg";
            const string image = "faces.jpg";

            // Read sample image
            using var frame = Cv2.ImRead(image);
            int frameHeight = frame.Rows;
            int frameWidth = frame.Cols;
            using var faceNet = CvDnn.ReadNetFromCaffe(configFile, faceModel);
            using var blob = CvDnn.BlobFromImage(frame, 1.0, new Size(300, 300),
                new Scalar(104, 117, 123), false, false);
            faceNet.SetInput(blob, "data");

            using var detection = faceNet.Forward("detection_out");
            using var detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F,
                detection.Ptr(0));
            for (int i = 0; i < detectionMat.Rows; i++)
            {
                float confidence = detectionMat.At<float>(i, 2);

                if (confidence > 0.7)
                {
                    int x1 = (int) (detectionMat.At<float>(i, 3) * frameWidth);
                    int y1 = (int) (detectionMat.At<float>(i, 4) * frameHeight);
                    int x2 = (int) (detectionMat.At<float>(i, 5) * frameWidth);
                    int y2 = (int) (detectionMat.At<float>(i, 6) * frameHeight);

                    Cv2.Rectangle(frame, new Point(x1, y1), new Point(x2, y2), new Scalar(0, 255, 0), 2,
                        LineTypes.Link4);
                }
            }

            Cv2.ImWrite(finalOutput, frame);
        }
    }
}
