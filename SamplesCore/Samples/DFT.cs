using System;
using OpenCvSharp;
using SampleBase;

namespace SamplesCore
{
    /// <summary>
    /// DFT, inverse DFT
    /// http://stackoverflow.com/questions/19761526/how-to-do-inverse-dft-in-opencv
    /// </summary>
    class DFT : ConsoleTestBase
    {
        public override void RunTest()
        {
            using var img = Cv2.ImRead(ImagePath.Lenna, ImreadModes.Grayscale);

            // expand input image to optimal size
            using var padded = new Mat(); 
            int m = Cv2.GetOptimalDFTSize(img.Rows);
            int n = Cv2.GetOptimalDFTSize(img.Cols); // on the border add zero values
            Cv2.CopyMakeBorder(img, padded, 0, m - img.Rows, 0, n - img.Cols, BorderTypes.Constant, Scalar.All(0));
            
            // Add to the expanded another plane with zeros
            using var paddedF32 = new Mat();
            padded.ConvertTo(paddedF32, MatType.CV_32F);
            Mat[] planes = { paddedF32, Mat.Zeros(padded.Size(), MatType.CV_32F) };
            using var complex = new Mat();
            Cv2.Merge(planes, complex);         

            // this way the result may fit in the source matrix
            using var dft = new Mat();
            Cv2.Dft(complex, dft);            

            // compute the magnitude and switch to logarithmic scale
            // => log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
            Cv2.Split(dft, out var dftPlanes);  // planes[0] = Re(DFT(I), planes[1] = Im(DFT(I))

            // planes[0] = magnitude
            using var magnitude = new Mat();
            Cv2.Magnitude(dftPlanes[0], dftPlanes[1], magnitude);

            using Mat magnitude1 = magnitude + Scalar.All(1);  // switch to logarithmic scale
            Cv2.Log(magnitude1, magnitude1);

            // crop the spectrum, if it has an odd number of rows or columns
            using var spectrum = magnitude1[
                new Rect(0, 0, magnitude1.Cols & -2, magnitude1.Rows & -2)];

            // rearrange the quadrants of Fourier image  so that the origin is at the image center
            int cx = spectrum.Cols / 2;
            int cy = spectrum.Rows / 2;

            using var q0 = new Mat(spectrum, new Rect(0, 0, cx, cy));   // Top-Left - Create a ROI per quadrant
            using var q1 = new Mat(spectrum, new Rect(cx, 0, cx, cy));  // Top-Right
            using var q2 = new Mat(spectrum, new Rect(0, cy, cx, cy));  // Bottom-Left
            using var q3 = new Mat(spectrum, new Rect(cx, cy, cx, cy)); // Bottom-Right

            // swap quadrants (Top-Left with Bottom-Right)
            using var tmp = new Mat();                           
            q0.CopyTo(tmp);
            q3.CopyTo(q0);
            tmp.CopyTo(q3);

            // swap quadrant (Top-Right with Bottom-Left)
            q1.CopyTo(tmp);                    
            q2.CopyTo(q1);
            tmp.CopyTo(q2);

            // Transform the matrix with float values into a
            Cv2.Normalize(spectrum, spectrum, 0, 255, NormTypes.MinMax); 
            spectrum.ConvertTo(spectrum, MatType.CV_8U);
                                     
            // Show the result
            Cv2.ImShow("Input Image"       , img);
            Cv2.ImShow("Spectrum Magnitude", spectrum);

            // calculating the idft
            using var inverseTransform = new Mat();
            Cv2.Dft(dft, inverseTransform, DftFlags.Inverse | DftFlags.RealOutput);
            Cv2.Normalize(inverseTransform, inverseTransform, 0, 255, NormTypes.MinMax);
            inverseTransform.ConvertTo(inverseTransform, MatType.CV_8U);

            Cv2.ImShow("Reconstructed by Inverse DFT", inverseTransform);
            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
        }
    }
}