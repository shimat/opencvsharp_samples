using System;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using SampleBase;

namespace SamplesCS
{
    /// <summary>
    /// To run this example first download the face model available here:https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
    /// Add the files to the bin folder
    /// </summary>
    class FaceDetectionDNN : ISample
    {
        public void Run()
        {
            
			string faceModel = "res10_300x300_ssd_iter_140000_fp16.caffemodel";
			string configFile = "deploy.prototxt";
			var image = "faces.jpg";
            // Read sample image
            var frame = Cv2.ImRead(image);
            int frameHeight = frame.Rows;
            int frameWidth = frame.Cols;
            using (var faceNet = CvDnn.ReadNetFromCaffe(configFile, faceModel)) 
            {
				using (Mat blob = CvDnn.BlobFromImage(frame, 1.0, new OpenCvSharp.Size(300, 300), new Scalar(104, 117, 123), false, false)) 
				{
					faceNet.SetInput(blob, "data");

					using (var detection = faceNet.Forward("detection_out")) 
					{
						using (Mat detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0))) 
						{
							
						}                                
					}                          
				}
			}
        }

    }
}