using System;
using OpenCvSharp;
using SampleBase;

namespace SamplesCS
{
    /// <summary>
    /// 
    /// </summary>
    class MatOperations : ISample
    {
        public void Run()
        {
            SubMat();
            RowColRangeOperation();
            RowColOperation();
        }

        /// <summary>
        /// Submatrix operations
        /// </summary>
        private void SubMat()
        {
            var src = Cv2.ImRead(FilePath.Image.Lenna);

            // Assign small image to mat
            using var small = new Mat();
            Cv2.Resize(src, small, new Size(100, 100));
            src[10, 110, 10, 110] = small;
            src[370, 470, 400, 500] = small.T();
            // ↑ This is same as the following:
            //small.T().CopyTo(src[370, 470, 400, 500]);

            // Get partial mat (similar to cvSetImageROI)
            Mat part = src[200, 400, 200, 360];
            // Invert partial pixel values
            Cv2.BitwiseNot(part, part);

            // Fill the region (50..100, 100..150) with color (128, 0, 0)
            part = src.SubMat(50, 100, 400, 450);
            part.SetTo(128);

            using (new Window("SubMat", src))
            {
                Cv2.WaitKey();
            }

            part.Dispose();
        }

        /// <summary>
        /// Submatrix operations
        /// </summary>
        private void RowColRangeOperation()
        {
            using var src = Cv2.ImRead(FilePath.Image.Lenna);

            Cv2.GaussianBlur(
                src.RowRange(100, 200),
                src.RowRange(200, 300),
                new Size(7, 7), 20);

            Cv2.GaussianBlur(
                src.ColRange(200, 300),
                src.ColRange(100, 200),
                new Size(7, 7), 20);

            using (new Window("RowColRangeOperation", src))
            {
                Cv2.WaitKey();
            }
        }

        /// <summary>
        /// Submatrix expression operations
        /// </summary>
        private void RowColOperation()
        {
            using var src = Cv2.ImRead(FilePath.Image.Lenna);

            var rand = new Random();
            for (int i = 0; i < 200; i++)
            {
                int c1 = rand.Next(100, 400);
                int c2 = rand.Next(100, 400);
                using Mat temp = src.Row(c1).Clone();
                src.Row(c2).CopyTo(src.Row(c1));
                temp.CopyTo(src.Row(c2));
            }

            ((Mat)~src.ColRange(450, 500)).CopyTo(src.ColRange(0, 50));

            src.RowRange(450, 460).SetTo(new Scalar(0, 0, 255));

            using (new Window("RowColOperation", src))
            {
                Cv2.WaitKey();
            }
        }
    }
}