using OpenCvSharp;
using OpenCvSharp.DnnSuperres;
using SampleBase;

namespace SamplesLegacy
{
    class DnnSuperresSample : ConsoleTestBase
    {
        // https://github.com/Saafke/FSRCNN_Tensorflow/tree/master/models
        private const string ModelFileName = "Data/Model/FSRCNN_x4.pb";

        public override void RunTest()
        {
            using var dnn = new DnnSuperResImpl("fsrcnn", 4);
            dnn.ReadModel(ModelFileName);

            using var src = new Mat(ImagePath.Mandrill, ImreadModes.Color);
            using var dst = new Mat();
            dnn.Upsample(src, dst);

            Window.ShowImages(
                new[]{src, dst}, 
                new[]{"src", "dst0"});
        }
    }
}
