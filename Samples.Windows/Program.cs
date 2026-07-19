using System;

namespace Samples.Windows;

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
