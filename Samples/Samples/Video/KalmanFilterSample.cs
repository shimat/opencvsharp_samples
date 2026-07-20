using System;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Tracks a point rotating around a circle from noisy angle measurements using a Kalman filter.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/dd/d6a/classcv_1_1KalmanFilter.html</remarks>
[SampleCategory(SampleCategory.Video)]
class KalmanFilterSample : ConsoleTestBase
{
    private const int Width = 640;
    private const int Height = 640;
    private const int Radius = 220;
    private const float AngularVelocity = 0.05f;

    public override void RunTest(DisplayHelper display)
    {
        var center = new Point(Width / 2, Height / 2);

        // State: [angle, angular velocity]. Measurement: [angle].
        using var kalman = new KalmanFilter(2, 1, 0);
        using var transitionMatrix = new Mat(2, 2, MatType.CV_32F);
        transitionMatrix.Set(0, 0, 1f);
        transitionMatrix.Set(0, 1, 1f); // angle(k) = angle(k-1) + angularVelocity(k-1)
        transitionMatrix.Set(1, 0, 0f);
        transitionMatrix.Set(1, 1, 1f); // angularVelocity(k) = angularVelocity(k-1)
        kalman.TransitionMatrix = transitionMatrix;
        Cv2.SetIdentity(kalman.MeasurementMatrix);
        Cv2.SetIdentity(kalman.ProcessNoiseCov, Scalar.All(1e-4));
        Cv2.SetIdentity(kalman.MeasurementNoiseCov, Scalar.All(0.3));
        Cv2.SetIdentity(kalman.ErrorCovPost, Scalar.All(1));
        kalman.StatePost.Set(0, 0, 0f);
        kalman.StatePost.Set(1, 0, AngularVelocity);

        using var measurement = new Mat(1, 1, MatType.CV_32F, Scalar.All(0));

        // Fixed seed so headless output is reproducible across runs.
        var rng = new Random(12345);
        float trueAngle = 0f;

        for (int i = 0; i < 300; i++)
        {
            using var prediction = kalman.Predict();

            // Advance ground truth and take a noisy angle measurement, as if from a sensor.
            trueAngle += AngularVelocity;
            float measuredAngle = trueAngle + ((float)rng.NextDouble() - 0.5f) * 0.3f;
            measurement.Set(0, 0, measuredAngle);

            using var estimated = kalman.Correct(measurement);
            float estimatedAngle = estimated.Get<float>(0, 0);

            using var frame = new Mat(Height, Width, MatType.CV_8UC3, Scalar.Black);
            Cv2.Circle(frame, center, Radius, Scalar.FromRgb(60, 60, 60), 1, LineTypes.AntiAlias);
            DrawSpoke(frame, center, trueAngle, Scalar.White);
            DrawSpoke(frame, center, measuredAngle, Scalar.Red);
            DrawSpoke(frame, center, estimatedAngle, Scalar.LimeGreen);
            Cv2.PutText(frame, "white=true angle, red=noisy measurement, green=Kalman estimate",
                new Point(10, 20), HersheyFonts.HersheySimplex, 0.5, Scalar.White);

            if (!display.ShowFrame(("Kalman filter: rotating point tracking", frame)))
                break;
        }
    }

    private static void DrawSpoke(Mat frame, Point center, float angle, Scalar color)
    {
        var tip = new Point(
            center.X + (int)(Radius * Math.Cos(angle)),
            center.Y + (int)(Radius * Math.Sin(angle)));
        Cv2.Line(frame, center, tip, color, 2, LineTypes.AntiAlias);
    }
}
