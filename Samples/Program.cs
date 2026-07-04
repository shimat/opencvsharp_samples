using SampleBase.Console;
using SampleBase.Interfaces;
using System;
using System.Linq;

namespace SamplesCore;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Console.WriteLine("Runtime Version = {0}", Environment.Version);

        ITestManager testManager = new ConsoleTestManager();

        testManager.AddTests(TestDiscovery.DiscoverTests().OrderBy(t => t.Name).ToArray());

        testManager.ShowTestEntrance();
    }
}
