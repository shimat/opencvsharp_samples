using OpenCvSharp;
using OpenCvSharp.DnnSuperres;
using SampleBase;

namespace SamplesCS
{
    class DnnSuperresSample : ISample
    {
        // https://github.com/Saafke/FSRCNN_Tensorflow/tree/master/models
        private const string ModelFileName = "Data/Model/FSRCNN_x4.pb";

        public void Run()
        {
            using var dnn = new DnnSuperResImpl("fsrcnn", 4);
            dnn.ReadModel(ModelFileName);

            using var src = new Mat(FilePath.Image.Mandrill, ImreadModes.Color);
            using var dst = new Mat();
            dnn.Upsample(src, dst);

            Window.ShowImages(
                new[]{src, dst}, 
                new[]{"src", "dst0"});
        }
    }
}
