using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;

namespace SamplesCS
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ISample sample =
            //new ArucoSample();
            //new BgSubtractorMOG();
            //new BinarizerSample();
            //new BRISKSample();
            //new CaffeSample();
            //new ClaheSample();
            //new ConnectedComponentsSample();
            //new DFT();
            //new DrawBestMatchRectangle();
            //new FaceDetection();
            //new FaceDetectionDNN();
            //new FASTSample();
            //new FlannSample(); 
            //new FREAKSample();
            //new HandPose();
            //new HistSample();
            //new HOGSample();
            //new HoughLinesSample();
            //new InpaintSample();
            //new KAZESample2();
            //new KAZESample();
            //new MatOperations();
            //new MatToBitmap();
            //new MDS();
            //new MSERSample();
            //new NormalArrayOperations();
            //new PerspectiveTransformSample();
            //new PhotoMethods();
            //new MergeSplitSample();
            //new MorphologySample();
            //new PixelAccess();
            //new Pose();
            //new SeamlessClone();
            //new SiftSurfSample();
            //new SimpleBlobDetectorSample();
            //new SolveEquation();
            //new StarDetectorSample();
            //new Stitching();
            //new Subdiv2DSample();
            //new SuperResolutionSample();
            //new SVMSample();
            //new VideoWriterSample();
            //new VideoCaptureSample();
            new WatershedSample();
            //new WindowGUISample();
            sample.Run();
        }
    }
}
