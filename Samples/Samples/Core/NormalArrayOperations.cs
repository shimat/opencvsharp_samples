using System;
using System.Collections.Generic;
using OpenCvSharp;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
///
/// </summary>
[SampleCategory(SampleCategory.Core)]
class NormalArrayOperations : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        Threshold1();
        Threshold2();
        Threshold3();
    }

    /// <summary>
    /// Run thresholding to byte array 
    /// </summary>
    private void Threshold1()
    {
        const int T = 3;
        const int Max = 5;

        byte[] input = { 1, 2, 3, 4, 5, };
        using var output = new Mat();

        Cv2.Threshold(InputArray.Create(input), output,
            T, Max, ThresholdTypes.Binary);
        output.GetArray(out byte[] outputArr);

        Console.WriteLine("Threshold: {0}", T);
        Console.WriteLine("input:  {0}", string.Join(",", input));
        Console.WriteLine("output: {0}", string.Join(",", outputArr));
    }

    /// <summary>
    /// Run thresholding to short array 
    /// </summary>
    private void Threshold2()
    {
        const int T = 150;
        const int Max = 250;

        short[] input = { 50, 100, 150, 200, 250, };
        using var output = new Mat();

        Cv2.Threshold(InputArray.Create(input), output,
            T, Max, ThresholdTypes.Binary);
        output.GetArray(out short[] outputArr);

        Console.WriteLine("Threshold: {0}", T);
        Console.WriteLine("input:  {0}", string.Join(",", input));
        Console.WriteLine("output: {0}", string.Join(",", outputArr));
    }

    /// <summary>
    /// Run thresholding to struct array 
    /// </summary>
    private void Threshold3()
    {
        const double T = 2000;
        const double Max = 5000;

        // threshold does not support Point (int)    
        Point2f[] input = {
                                  new Point2f(1000, 1500),
                                  new Point2f(2000, 2001),
                                  new Point2f(500, 5000),
                              };
        using var output = new Mat();

        Cv2.Threshold(InputArray.Create(input), output,
            T, Max, ThresholdTypes.Binary);
        output.GetArray(out Point2f[] outputArr);

        Console.WriteLine("Threshold: {0}", T);
        Console.WriteLine("input:  {0}", string.Join(",", input));
        Console.WriteLine("output: {0}", string.Join(",", outputArr));
    }
}
