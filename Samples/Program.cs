using Microsoft.Extensions.Configuration;
using SampleBase.Console;
using SampleBase.Interfaces;
using System;
using System.Linq;

namespace SamplesCore;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine($"Runtime Version = {Environment.Version}");

        // Env var: OPENCV_SAMPLES_HEADLESS=1 (via Configuration rather than a raw
        // Environment.GetEnvironmentVariable lookup). CLI: --headless.
        // AddCommandLine doesn't support a bare value-less switch meaning "true", so that part is
        // just a plain args check.
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "OPENCV_SAMPLES_")
            .Build();
        bool headless = args.Contains("--headless") || configuration["Headless"] is "1" or "true";

        var testManager = new ConsoleTestManager(headless);

        testManager.AddTests(TestDiscovery.DiscoverTests().ToArray());

        testManager.ShowTestEntrance();
    }
}
