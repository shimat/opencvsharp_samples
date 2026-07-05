using System;
using System.Linq;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Tracks sparse feature points across video frames using Lucas-Kanade optical flow,
/// drawing each point's recent motion trail.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/d4/dee/tutorial_optical_flow.html</remarks>
class OpticalFlowSample : ConsoleTestBase
{
    private static readonly TermCriteria Criteria =
        new(CriteriaTypes.Eps | CriteriaTypes.Count, 10, 0.03);

    public override void RunTest(DisplayHelper display)
    {
        using var capture = new VideoCapture(MoviePath.Bach);
        if (!capture.IsOpened())
            return;

        using var prevGray = new Mat();
        using (var firstFrame = new Mat())
        {
            capture.Read(firstFrame);
            if (firstFrame.Empty())
                return;
            Cv2.CvtColor(firstFrame, prevGray, ColorConversionCodes.BGR2GRAY);
        }

        Point2f[] points = Cv2.GoodFeaturesToTrack(
            prevGray, 30, 0.1, 60, default, blockSize: 7, useHarrisDetector: false, k: 0.04);
        var random = new Random(0);
        var colors = points.Select(_ => new Scalar(random.Next(256), random.Next(256), random.Next(256))).ToArray();

        using Mat mask = Mat.Zeros(prevGray.Rows, prevGray.Cols, MatType.CV_8UC3);
        using var frame = new Mat();
        using var gray = new Mat();

        while (points.Length > 0)
        {
            capture.Read(frame);
            if (frame.Empty())
                break;
            Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

            var nextPoints = (Point2f[])points.Clone();
            Cv2.CalcOpticalFlowPyrLK(
                prevGray, gray, points, ref nextPoints,
                out byte[] status, out _,
                new Size(15, 15), 2, Criteria);

            var good = Enumerable.Range(0, points.Length).Where(i => status[i] != 0).ToArray();

            using var view = frame.Clone();
            foreach (int i in good)
            {
                Cv2.Line(mask, (Point)points[i], (Point)nextPoints[i], colors[i], 4);
                Cv2.Circle(view, (Point)nextPoints[i], 10, colors[i], -1);
            }
            Cv2.Add(view, mask, view);

            if (!display.ShowFrame(("Lucas-Kanade optical flow", view)))
                break;

            gray.CopyTo(prevGray);
            points = good.Select(i => nextPoints[i]).ToArray();
            colors = good.Select(i => colors[i]).ToArray();
        }
    }
}
