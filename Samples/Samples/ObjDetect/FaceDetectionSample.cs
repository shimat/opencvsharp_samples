using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Human face detection
/// http://docs.opencv.org/doc/tutorials/objdetect/cascade_classifier/cascade_classifier.html
/// </summary>
[SampleCategory(SampleCategory.ObjDetect)]
class FaceDetectionSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        // Load the cascades
        using var haarCascade = new CascadeClassifier(TextPath.HaarCascade);
        using var lbpCascade = new CascadeClassifier(TextPath.LbpCascade);

        // Detect faces
        Mat haarResult = DetectFace(haarCascade);
        Mat lbpResult = DetectFace(lbpCascade);

        display.Show(("Faces by Haar", haarResult), ("Faces by LBP", lbpResult));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cascade"></param>
    /// <returns></returns>
    private Mat DetectFace(CascadeClassifier cascade)
    {
        Mat result;

        using (var src = new Mat(ImagePath.Yalta, ImreadModes.Color))
        using (var gray = new Mat())
        {
            result = src.Clone();
            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

            // Detect faces
            Rect[] faces = cascade.DetectMultiScale(
                gray, 1.08, 2, HaarDetectionTypes.ScaleImage, new Size(30, 30));

            // Render all detected faces
            foreach (Rect face in faces)
            {
                var center = new Point
                {
                    X = (int)(face.X + face.Width * 0.5),
                    Y = (int)(face.Y + face.Height * 0.5)
                };
                var axes = new Size
                {
                    Width = (int)(face.Width * 0.5),
                    Height = (int)(face.Height * 0.5)
                };
                Cv2.Ellipse(result, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
            }
        }
        return result;
    }
}
