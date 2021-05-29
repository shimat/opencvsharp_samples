Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports OpenCvSharp
Imports SampleBase

''' <summary>
''' cv::FAST
''' </summary>
Friend Module FASTSample
    Public Sub Start()
        Using imgSrc As New Mat(ImagePath.Lenna, ImreadModes.Color),
            imgGray As New Mat(imgSrc.Size, MatType.CV_8UC1),
            imgDst As Mat = imgSrc.Clone()
            Cv2.CvtColor(imgSrc, imgGray, ColorConversionCodes.BGR2GRAY, 0)

            Dim keypoints() = Cv2.FAST(imgGray, 50, True)

            For Each kp As KeyPoint In keypoints
                imgDst.Circle(kp.Pt, 3, Scalar.Red, -1, LineTypes.AntiAlias, 0)
            Next kp

            Cv2.ImShow("FAST", imgDst)
            Cv2.WaitKey(0)
            Cv2.DestroyAllWindows()
        End Using
    End Sub
End Module
' End Namespace
