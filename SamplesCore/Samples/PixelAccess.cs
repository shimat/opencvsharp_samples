using System;
using System.Diagnostics;
using OpenCvSharp;

namespace SamplesCore
{
    /// <summary>
    /// Swaps B for R 
    /// </summary>
    class PixelAccess : ISample
    {
        public void Run()
        {
            Console.WriteLine("Get/Set: {0}ms", MeasureTime(GetSet));
            Console.WriteLine("GenericIndexer: {0}ms", MeasureTime(GenericIndexer));
            Console.WriteLine("TypeSpecificMat: {0}ms", MeasureTime(TypeSpecificMat));
            Console.Read();
        }

        /// <summary>
        /// Slow
        /// </summary>
        private void GetSet()
        {
            using (var mat = new Mat(FilePath.Image.Lenna, ImreadModes.Color))
            {
                for (int y = 0; y < mat.Height; y++)
                {
                    for (int x = 0; x < mat.Width; x++)
                    {
                        Vec3b color = mat.Get<Vec3b>(y, x);
                        Vec3b newColor = new Vec3b(color.Item2, color.Item1, color.Item0);
                        mat.Set<Vec3b>(y, x, newColor);
                    }
                }
                //Cv2.ImShow("Slow", mat);
                //Cv2.WaitKey(0);
                //Cv2.DestroyAllWindows();
            }
        }

        /// <summary>
        /// Reasonably fast
        /// </summary>
        private void GenericIndexer()
        {
            using (var mat = new Mat(FilePath.Image.Lenna, ImreadModes.Color))
            {
                var indexer = mat.GetGenericIndexer<Vec3b>();
                for (int y = 0; y < mat.Height; y++)
                {
                    for (int x = 0; x < mat.Width; x++)
                    {
                        Vec3b color = indexer[y, x];
                        Vec3b newColor = new Vec3b(color.Item2, color.Item1, color.Item0);
                        indexer[y, x] = newColor;
                    }
                }
                //Cv2.ImShow("GenericIndexer", mat);
                //Cv2.WaitKey(0);
                //Cv2.DestroyAllWindows();
            }
        }

        /// <summary>
        /// Faster
        /// </summary>
        private void TypeSpecificMat()
        {
            using (var mat = new Mat(FilePath.Image.Lenna, ImreadModes.Color))
            {
                var mat3 = new Mat<Vec3b>(mat);
                var indexer = mat3.GetIndexer();
                for (int y = 0; y < mat.Height; y++)
                {
                    for (int x = 0; x < mat.Width; x++)
                    {
                        Vec3b color = indexer[y, x];
                        Vec3b newColor = new Vec3b(color.Item2, color.Item1, color.Item0);
                        indexer[y, x] = newColor;
                    }
                }
                //Cv2.ImShow("TypeSpecificMat", mat);
                //Cv2.WaitKey(0);
                //Cv2.DestroyAllWindows();
            }
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = b;
            b = a;
            a = temp;
        }

        private static long MeasureTime(Action action)
        {
            var watch = Stopwatch.StartNew();
            action();
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }
    }
}