using System;
using SamplesCS;

namespace SamplesCore
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ISample sample =
                //new CaffeSample();
                new ClaheSample();
                //new ConnectedComponentsSample();
                //new HOGSample();
                //new HoughLinesSample();
                //new MatOperations();
                //new MatToWriteableBitmap();
                //new NormalArrayOperations();
                //new PhotoMethods();
                //new MorphologySample();
                //new PixelAccess();
                //new SolveEquation();
                //new Subdiv2DSample();
                //new SVMSample();
                //new VideoWriterSample();
                //new VideoCaptureSample();

            sample.Run();
        }
    }
}
