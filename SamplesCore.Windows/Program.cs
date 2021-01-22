using System;

namespace SamplesCore.Windows
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ISample sample =
                new MatToWriteableBitmap();

            sample.Run();
        }
    }
}
