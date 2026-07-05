using System;
using OpenCvSharp;
using SampleBase.Console;
using SampleBase.Interfaces;

namespace SamplesCore;

/// <summary>
/// Multidimensional Scaling (多次元尺度構成法)
/// for C++ cv::Mat testing
/// </summary>
class MDS : ConsoleTestBase
{
    /// <summary>
    /// Distance among 10 American cities
    /// </summary>
    /// <example>
    /// * The linear distance between Atlanta and Chicago is 587km.
    /// </example>
    static readonly double[,] CityDistance =
        {
            /*Atlanta*/         {0,      587,    1212,   701,    1936,   604,    748,    2139,   2182,   543},
            /*Chicago*/         {587,    0,      920,    940,    1745,   1188,   713,    1858,   1737,   597},
            /*Denver*/          {1212,   920,    0,      879,    831,    1726,   1631,   949,    1021,   1494},
            /*Houston*/         {701,    940,    879,    0,      1734,   968,    1420,   1645,   1891,   1220},
            /*Los Angeles*/     {1936,   1745,   831,    1734,   0,      2339,   2451,   347,    959,    2300},
            /*Miami*/           {604,    1188,   1726,   968,    2339,   0,      1092,   2594,   2734,   923},
            /*New York*/        {748,    713,    1631,   1420,   2451,   1092,   0,      2571,   2408,   205},
            /*San Francisco*/   {2139,   1858,   949,    1645,   347,    2594,   2571,   0,      678,    2442},
            /*Seattle*/         {2182,   1737,   1021,   1891,   959,    2734,   2408,   678,    0,      2329},
            /*Washington D.C.*/ {543,    597,    1494,   1220,   2300,   923,    205,    2442,   2329,   0}
        };

    /// <summary>
    /// City names
    /// </summary>
    static readonly string[] CityNames =
        [
            "Atlanta","Chicago","Denver","Houston","Los Angeles","Miami","New York","San Francisco","Seattle","Washington D.C."
        ];


    /// <summary>
    /// Classical Multidimensional Scaling
    /// </summary>
    public override void RunTest(DisplayHelper display)
    {
        // creates distance matrix
        int size = CityDistance.GetLength(0);
        var t = Mat.FromPixelData(size, size, MatType.CV_64FC1, CityDistance);
        // adds Torgerson's additive constant to t
        double torgarson = Torgerson(t);
        t += new Scalar(torgarson);
        // squares all elements of t
        t = t.Mul(t);

        // centering matrix G
        using var g = CenteringMatrix(size);
        // calculates inner product matrix B
        var gt = g.T();
        using var gt_mat = new Mat(gt);
        var tmp = g * t;
        var tmp2 = tmp * gt_mat;
        var b = tmp2 * -0.5;
        // calculates eigenvalues and eigenvectors of B
        using var values = new Mat();
        using var vectors = new Mat();
        Cv2.Eigen(b, values, vectors);
        int valuesRows = values.Rows;
        for (int r = 0; r < valuesRows; r++)
        {
            if (values.Get<double>(r) < 0)
                values.Set<double>(r, 0);
        }

        //Console.WriteLine(values.Dump());

        // multiplies sqrt(eigenvalue) by eigenvector
        using var result = vectors.RowRange(0, 2);
        {
            var resultRows = result.AsRows<double>();
            int rows = result.Rows;
            int cols = result.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    resultRows[r][c] *= Math.Sqrt(values.Get<double>(r));
                }
            }
        }

        // scaling
        Cv2.Normalize(result, result, 0, 800, NormTypes.MinMax);

        // opens a window
        using (Mat img = Mat.Zeros(600, 800, MatType.CV_8UC3))
        {
            var resultRows = result.AsRows<double>();
            for (int c = 0; c < size; c++)
            {
                double x = resultRows[0][c];
                double y = resultRows[1][c];
                x = x * 0.7 + img.Width * 0.1;
                y = y * 0.7 + img.Height * 0.1;
                Cv2.Circle(img, (int)x, (int)y, 5, Scalar.Red, -1);
                var textPos = new Point(x + 5, y + 10);
                Cv2.PutText(img, CityNames[c], textPos, HersheyFonts.HersheySimplex, 0.5, Scalar.White);
            }
            display.Show(("City Location Estimation", img));
        }
    }

    /// <summary>
    /// Returns Torgerson's additive constant
    /// </summary>
    /// <param name="mat"></param>
    /// <returns></returns>
    private static double Torgerson(Mat mat)
    {
        if (mat is null)
            throw new ArgumentNullException(nameof(mat));
        if (mat.Rows != mat.Cols)
            throw new ArgumentException("mat.Rows != mat.Cols", nameof(mat));

        int n = mat.Rows;
        // Additive constant in case of negative value
        Cv2.MinMaxLoc(-mat, out _, out double max);
        double c2 = max;
        // Additive constant from triangular inequality
        double c1 = 0;

        var matRows = mat.AsRows<double>();
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    double v = matRows[i][k] - matRows[i][j] - matRows[j][k];
                    if (v > c1)
                    {
                        c1 = v;
                    }
                }
            }
        }
        return Math.Max(Math.Max(c1, c2), 0);
    }

    /// <summary>
    /// Returns centering matrix
    /// </summary>
    /// <param name="n">Size of matrix</param>
    /// <returns></returns>
    private static Mat CenteringMatrix(int n)
    {
        var eye = Mat.Eye(n, n, MatType.CV_64FC1);
        return (eye - (Scalar)(1.0 / n));
    }
}
