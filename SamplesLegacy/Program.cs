using SampleBase;
using SampleBase.Interfaces;
using System;

namespace SamplesLegacy
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("Runtime Version = {0}", Environment.Version);

            ITestManager testManager = new ConsoleTestManager();
            
            testManager.AddTests(
                new ArucoSample(),
                new BgSubtractorMOG(),
                new BinarizerSample(),
                new BRISKSample(),
                new CaffeSample(),
                new CameraCaptureSample(),
                new ClaheSample(),
                new ConnectedComponentsSample(),
                new DFT(),
                new DnnSuperresSample(),
                new DrawBestMatchRectangle(),
                new FaceDetection(),
                new FaceDetectionDNN(),
                new FASTSample(),
                new FlannSample(),
                new FREAKSample(),
                new HandPose(),
                new HistSample(),
                new HOGSample(),
                new HoughLinesSample(),
                new InpaintSample(),
                new KAZESample(),
                new KAZESample2(),
                new MatOperations(),
                new MDS(),
                new MergeSplitSample(),
                new MorphologySample(),
                new MSERSample(),
                new NormalArrayOperations(),
                new OpenVinoFaceDetection(),
                new PhotoMethods(),
                new PixelAccess(),
                new Pose(),
                new SeamlessClone(),
                new SiftSurfSample(),
                new SimpleBlobDetectorSample(),
                new SolveEquation(),
                new StarDetectorSample(),
                new Stitching(),
                new Subdiv2DSample(),
                new SuperResolutionSample(),
                new SVMSample(),
                new VideoWriterSample(),
                new VideoCaptureSample(),
                new WatershedSample());

            testManager.ShowTestEntrance();
        }
    }
}
