using Sample.Test;
using Sample.Test.Interfaces;
using System;
using System.Linq;

namespace SamplesCore
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ITestManager testManager = new ConsoleTestManager();

            testManager.AddTest(new CaffeSample());
            testManager.AddTest(new ClaheSample());
            testManager.AddTest(new ConnectedComponentsSample());
            testManager.AddTest(new CameraCaptureSample());
            testManager.AddTest(new DnnSuperresSample());
            testManager.AddTest(new HOGSample());
            testManager.AddTest(new HoughLinesSample());
            testManager.AddTest(new MatOperations());
            testManager.AddTest(new NormalArrayOperations());
            testManager.AddTest(new PhotoMethods());
            testManager.AddTest(new MorphologySample());
            testManager.AddTest(new PixelAccess());
            testManager.AddTest(new SolveEquation());
            testManager.AddTest(new Subdiv2DSample());
            testManager.AddTest(new SVMSample());
            testManager.AddTest(new VideoWriterSample());
            testManager.AddTest(new VideoCaptureSample());

            var printer = testManager.GetAllTests().First().GetMessagePrinter(); 
            printer.PrintInfo("Superclass of all test classes has been replaced with ConsoleTestBase (ISample has been removed)");
            printer.PrintInfo("To make it easier to execute test cases, it is expected to maintain test cases in this way.");
            testManager.ShowTestEntrance();
        }
    }
}
