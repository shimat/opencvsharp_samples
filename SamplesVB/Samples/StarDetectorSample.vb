Imports System
Imports OpenCvSharp
Imports OpenCvSharp.XFeatures2D

' Namespace OpenCvSharpSamplesVB
Imports SampleBase

''' <summary>
''' Retrieves keypoints using the StarDetector algorithm.
''' </summary>
Friend Module StarDetectorSample
    Public Sub Start()
        Using src As New Mat(ImagePath.Lenna, ImreadModes.Grayscale),
              dst As New Mat()
            Cv2.CvtColor(src, dst, ColorConversionCodes.GRAY2BGR)

            CppStyleStarDetector(src, dst) ' C++-style

            Using w1 As New Window("img", src),
                  w2 As New Window("features", dst)
                Cv2.WaitKey()
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' Extracts keypoints by C++-style code (cv::StarDetector)
    ''' </summary>
    ''' <param name="src"></param>
    ''' <param name="dst"></param>
    Private Sub CppStyleStarDetector(src As Mat, dst As Mat)
        Dim detector As StarDetector = StarDetector.Create()
        Dim keypoints() As KeyPoint = detector.Detect(src, Nothing)

        If keypoints IsNot Nothing Then
            For Each kpt As KeyPoint In keypoints
                Dim r As Single = kpt.Size / 2
                Dim a = kpt.Pt

                Cv2.Circle(dst, kpt.Pt, Math.Truncate(r), New Scalar(0, 255, 0), 1, LineTypes.Link8, 0)
                Cv2.Line(dst, New Point(kpt.Pt.X + r, kpt.Pt.Y + r), New Point(kpt.Pt.X - r, kpt.Pt.Y - r), New Scalar(0, 255, 0), 1, LineTypes.Link8, 0)
                Cv2.Line(dst, New Point(kpt.Pt.X - r, kpt.Pt.Y + r), New Point(kpt.Pt.X + r, kpt.Pt.Y - r), New Scalar(0, 255, 0), 1, LineTypes.Link8, 0)
            Next kpt
        End If

    End Sub
End Module
' End Namespace
