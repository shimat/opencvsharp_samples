using OpenCvSharp;
using SampleBase.Console;
using SampleBase.Interfaces;
using System;

namespace SamplesCore;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Console.WriteLine("Runtime Version = {0}", Environment.Version);

        ITestManager testManager = new ConsoleTestManager();
            
        testManager.AddTests(
            new ArucoSample(),
            new BgSubtractorMOG(),
            new BinarizerSample(),
            new BRISKSample(),
            new CameraCalibrationSample(),
            new CameraCaptureSample(),
            new ClaheSample(),
            new ConnectedComponentsSample(),
            new DFT(),
            new DnnSuperresSample(),
            new DrawBestMatchRectangle(),
            new FaceDetection(),
            new FASTSample(),
            new FlannSample(),
            new FREAKSample(),
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
            new PerspectiveTransformSample(),
            new PhotoMethods(),
            new PixelAccess(),
            new SeamlessClone(),
            new SiftSurfSample(),
            new SimpleBlobDetectorSample(),
            new SolveEquation(),
            new SquareDetectionSample(),
            new StarDetectorSample(),
            new StereoMatchingSample(),
            new Stitching(),
            new Subdiv2DSample(),
            new SVMSample(),
            new VideoWriterSample(),
            new VideoCaptureSample(),
            new WatershedSample());

        testManager.ShowTestEntrance();



        var mat = new Mat();
        mat.ToString();
    }
}
