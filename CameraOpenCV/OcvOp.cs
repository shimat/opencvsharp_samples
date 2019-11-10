using System;
using Windows.Graphics.Imaging;
using OpenCvSharp;

namespace SDKTemplate
{
    public sealed class OcvOp : IDisposable
    {
        private BackgroundSubtractorMOG2 mog2;

        public OcvOp()
        {
            mog2 = BackgroundSubtractorMOG2.Create();
        }

        public void Dispose()
        {
            mog2.Dispose();
        }

        public void Blur(SoftwareBitmap input, SoftwareBitmap output, Algorithm algorithm)
        {
            if (algorithm.AlgorithmName == "Blur")
            {
                using Mat mInput = SoftwareBitmap2Mat(input);
                using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);

                //App.CvHelper.T

                Cv2.Blur(mInput, mOutput,
                    (Size)algorithm.AlgorithmProperties[0].CurrentValue,
                    (Point)algorithm.AlgorithmProperties[1].CurrentValue,
                    (BorderTypes)algorithm.AlgorithmProperties[2].CurrentValue);
                //Cv2.ImShow("Blur", mOutput);
                Mat2SoftwareBitmap(mOutput, output);
            }
        }

        public void HoughLines(SoftwareBitmap input, SoftwareBitmap output, Algorithm algorithm)
        {
            if (algorithm.AlgorithmName == "HoughLines")
            {
                using Mat mInput = SoftwareBitmap2Mat(input);
                using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
                mInput.CopyTo(mOutput);
                using Mat gray = mInput.CvtColor(ColorConversionCodes.BGRA2GRAY);
                using Mat edges = gray.Canny(50, 200);
                //var res = Cv2.HoughLinesP(mInput,
                //    (double)algorithm.findParambyName("rho"),
                //    (double)algorithm.findParambyName("theta"),
                //    (int)algorithm.findParambyName("threshold"),
                //    (double)algorithm.findParambyName("minLineLength"),
                //    (double)algorithm.findParambyName("maxLineGap")
                //);
                var res = Cv2.HoughLinesP(edges,
                    (double)algorithm.AlgorithmProperties[0].CurrentValue,
                    (double)algorithm.AlgorithmProperties[1].CurrentValue / 100.0,
                    (int)algorithm.AlgorithmProperties[2].CurrentValue,
                    (double)algorithm.AlgorithmProperties[3].CurrentValue,
                    (double)algorithm.AlgorithmProperties[4].CurrentValue);

                for (int i = 0; i < res.Length; i++)
                {
                    //Cv2.Line(mOutput, res[i].P1, res[i].P2, 
                    //    (Scalar)algorithm.findParambyName("color"), 
                    //    (int)algorithm.findParambyName("thickness"), 
                    //    (LineTypes)algorithm.findParambyName("linetype"));

                    Cv2.Line(mOutput, res[i].P1, res[i].P2,
                        (Scalar)algorithm.AlgorithmProperties[5].CurrentValue,
                        (int)algorithm.AlgorithmProperties[6].CurrentValue,
                        (LineTypes)algorithm.AlgorithmProperties[7].CurrentValue);

                    //Cv2.Line(mOutput, res[i].P1, res[i].P2,
                    //    Scalar.Azure,
                    //    2,
                    //    LineTypes.Link4);
                }

                //Cv2.ImShow("HoughLines", mOutput);
                Mat2SoftwareBitmap(mOutput, output);
            }
        }

        public async void Contours(SoftwareBitmap input, SoftwareBitmap output, Algorithm algorithm)
        {
            if (algorithm.AlgorithmName == "Contours")
            {
                using Mat mInput = SoftwareBitmap2Mat(input);
                using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
                mInput.CopyTo(mOutput);
                using Mat gray = mInput.CvtColor(ColorConversionCodes.BGRA2GRAY);
                using Mat edges = gray.Canny((double)algorithm.AlgorithmProperties[6].CurrentValue, (double)algorithm.AlgorithmProperties[7].CurrentValue);

                Cv2.FindContours(
                    edges,
                    out OpenCvSharp.Point[][] contours,
                    out HierarchyIndex[] outputArray,
                    (RetrievalModes)algorithm.AlgorithmProperties[0].CurrentValue,
                    (ContourApproximationModes)algorithm.AlgorithmProperties[1].CurrentValue,
                    (Point)algorithm.AlgorithmProperties[2].CurrentValue);

                int maxLen = 0;
                int maxIdx = -1;
                for (int i = 0; i < contours.Length; i++)
                {
                    if (contours[i].Length > maxLen)
                    {
                        maxIdx = i;
                        maxLen = contours[i].Length;
                    }
                    if (contours[i].Length > (int)algorithm.AlgorithmProperties[8].CurrentValue)
                    {
                        Cv2.DrawContours(
                            mOutput,
                            contours,
                            i,
                            (Scalar)algorithm.AlgorithmProperties[3].CurrentValue,
                            (int)algorithm.AlgorithmProperties[4].CurrentValue,
                            (LineTypes)algorithm.AlgorithmProperties[5].CurrentValue,
                            outputArray,
                            0);

                    }
                }
                if (maxIdx != -1)
                {
                    var res = Cv2.ApproxPolyDP(contours[maxIdx], 1, true);
                    //Cv2.DrawContours(
                    //    mOutput,
                    //    contours,
                    //    maxIdx,
                    //    (Scalar)algorithm.algorithmProperties[3].CurrentValue,
                    //    (int)algorithm.algorithmProperties[4].CurrentValue,
                    //    (LineTypes)algorithm.algorithmProperties[5].CurrentValue,
                    //    outputArray,
                    //    0);
                    ////return Cv2.ContourArea(res);
                }

                Mat2SoftwareBitmap(mOutput, output);

                // Must run on UI thread.  The winrt container also needs to be set.
                if (App.container != null)
                {
                    await App.container.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Cv2.ImShow("Contours", mOutput);
                    });
                }
            }
        }

        public void Canny(SoftwareBitmap input, SoftwareBitmap output, Algorithm algorithm)
        {
            if (algorithm.AlgorithmName == "Canny")
            {
                using Mat mInput = SoftwareBitmap2Mat(input);
                using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
                using Mat intermediate = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);

                // MP! Todo: add param support
                Cv2.Canny(mInput, intermediate, 80, 90);
                Cv2.CvtColor(intermediate, mOutput, ColorConversionCodes.GRAY2BGRA);

                Mat2SoftwareBitmap(mOutput, output);
            }
        }

        public void MotionDetector(SoftwareBitmap input, SoftwareBitmap output, Algorithm algorithm)
        {
            if (algorithm.AlgorithmName == "MotionDetector")
            {
                using Mat mInput = SoftwareBitmap2Mat(input);
                using Mat mOutput = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
                using Mat fgMaskMOG2 = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);
                using Mat temp = new Mat(mInput.Rows, mInput.Cols, MatType.CV_8UC4);

                // MP! Todo: add param support
                mog2.Apply(mInput, fgMaskMOG2);
                Cv2.CvtColor(fgMaskMOG2, temp, ColorConversionCodes.GRAY2BGRA);

                using Mat element = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
                Cv2.Erode(temp, temp, element);
                temp.CopyTo(mOutput);
                Mat2SoftwareBitmap(mOutput, output);
            }
        }

        public static unsafe Mat SoftwareBitmap2Mat(SoftwareBitmap softwareBitmap)
        {
            using (BitmapBuffer buffer = softwareBitmap.LockBuffer(BitmapBufferAccessMode.Write))
            {
                using (var reference = buffer.CreateReference())
                {
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);

                    Mat outputMat = new Mat(softwareBitmap.PixelHeight, softwareBitmap.PixelWidth, MatType.CV_8UC4, (IntPtr)dataInBytes);
                    return outputMat;
                }
            }
        }

        public static unsafe void Mat2SoftwareBitmap(Mat input, SoftwareBitmap output)
        {
            //SoftwareBitmap softwareBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, input.Width, input.Height, BitmapAlphaMode.Premultiplied);
            using (BitmapBuffer buffer = output.LockBuffer(BitmapBufferAccessMode.ReadWrite))
            {
                using (var reference = buffer.CreateReference())
                {
                    ((IMemoryBufferByteAccess)reference).GetBuffer(out var dataInBytes, out var capacity);
                    BitmapPlaneDescription bufferLayout = buffer.GetPlaneDescription(0);
                    for (int i = 0; i < bufferLayout.Height; i++)
                    {
                        for (int j = 0; j < bufferLayout.Width; j++)
                        {
                            //byte value = input.DataPointer[i * bufferLayout.Width + j];
                            dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 0] =
                                input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 0];
                            dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 1] =
                                input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 1];
                            dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 2] =
                                input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 2];
                            dataInBytes[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 3] =
                                input.DataPointer[bufferLayout.StartIndex + bufferLayout.Stride * i + 4 * j + 3];
                        }
                    }
                }
            }
            //return softwareBitmap;
        }
    }
}