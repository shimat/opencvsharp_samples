using System;
using OpenCvSharp;
using OpenCvSharp.Flann;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// cv::flann
/// </summary>
[SampleCategory(SampleCategory.Features2D)]
class FlannSample : ConsoleTestBase
{
    public override void RunTest(DisplayHelper display)
    {
        Console.WriteLine("===== FlannTest =====");

        // creates data set
        using var features = new Mat(10000, 2, MatType.CV_32FC1);
        int rows = features.Rows;
        for (int i = 0; i < rows; i++)
        {
            features.Set<float>(i, 0, Random.Shared.Next(10000));
            features.Set<float>(i, 1, Random.Shared.Next(10000));
        }

        // query
        var queryPoint = new Point2f(7777, 7777);
        using var queries = new Mat(1, 2, MatType.CV_32FC1);
        queries.Set<float>(0, 0, queryPoint.X);
        queries.Set<float>(0, 1, queryPoint.Y);
        Console.WriteLine($"query:({queryPoint.X}, {queryPoint.Y})");
        Console.WriteLine("-----");

        // knnSearch
        using var nnIndex = new OpenCvSharp.Flann.Index(features, new KDTreeIndexParams(4));
        const int Knn = 1;
        nnIndex.KnnSearch(queries, out int[] indices, out float[] dists, Knn, new SearchParams(32));

        for (int i = 0; i < Knn; i++)
        {
            int index = indices[i];
            float dist = dists[i];
            var pt = new Point2f(features.Get<float>(index, 0), features.Get<float>(index, 1));
            Console.WriteLine($"No.{i}\tindex:{index} distance:{dist} data:({pt.X}, {pt.Y})");
        }
    }
}
