using OpenCvSharp;
using OpenCvSharp.Tracking;
using SampleBase;
using SampleBase.Console;

namespace SamplesCore;

/// <summary>
/// Tracks a moving object across video frames using the CSRT tracker.
/// </summary>
/// <remarks>https://docs.opencv.org/4.x/d2/d0a/tutorial_introduction_to_tracker.html</remarks>
class ObjectTrackingSample : ConsoleTestBase
{
    // Bounding box around the puppy in the first frame
    private static readonly Rect InitialBox = new(300, 150, 180, 200);

    public override void RunTest()
    {
        using var capture = new VideoCapture(MoviePath.Hara);
        if (!capture.IsOpened())
            return;

        using var frame = new Mat();
        capture.Read(frame);
        if (frame.Empty())
            return;

        using var tracker = TrackerCSRT.Create();
        Rect box = InitialBox;
        tracker.Init(frame, box);

        while (true)
        {
            bool found = tracker.Update(frame, ref box);
            using var view = frame.Clone();
            if (found)
            {
                Cv2.Rectangle(view, box, Scalar.Red, 2);
            }
            if (!DisplayHelper.ShowFrame(nameof(ObjectTrackingSample), "CSRT object tracking", view, 30))
                break;

            capture.Read(frame);
            if (frame.Empty())
                break;
        }
    }
}
