using OpenCvSharp;
using SampleBase;

namespace SamplesLegacy
{
    class BgSubtractorMOG : ConsoleTestBase
    {
        public override void RunTest()
        {
            using var capture = new VideoCapture(MoviePath.Bach);
            using var mog = BackgroundSubtractorMOG.Create();
            using var windowSrc = new Window("src");
            using var windowDst = new Window("dst");

            using var frame = new Mat();
            using var fg = new Mat();
            while (true)
            {
                capture.Read(frame);
                if (frame.Empty())
                    break;
                mog.Apply(frame, fg, 0.01);

                windowSrc.Image = frame;
                windowDst.Image = fg;
                Cv2.WaitKey(50);
            }
        }
    }
}