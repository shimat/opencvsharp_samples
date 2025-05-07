﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenCvSharp;
using SampleBase;
using SampleBase.Console;

namespace SamplesLegacy
{
    /// <summary>
    /// https://github.com/shimat/opencvsharp/issues/176
    /// </summary>
    class KAZESample2 : ConsoleTestBase
    {
        public static Point2d Point2fToPoint2d(Point2f pf)
        {
            return new Point2d(((int)pf.X), ((int)pf.Y));
        }

        public override void RunTest()
        {
            using var img1 = new Mat(ImagePath.SurfBox);
            using var img2 = new Mat(ImagePath.SurfBoxinscene);
            using var descriptors1 = new Mat();
            using var descriptors2 = new Mat();
            using var matcher = new BFMatcher(NormTypes.L2SQR);
            using var kaze = KAZE.Create();
            kaze.DetectAndCompute(img1, null, out var keypoints1, descriptors1);
            kaze.DetectAndCompute(img2, null, out var keypoints2, descriptors2);

            DMatch[][] matches = matcher.KnnMatch(descriptors1, descriptors2, 2);
            using Mat mask = new Mat(matches.Length, 1, MatType.CV_8U);
            mask.SetTo(new Scalar(255));
            int nonZero = Cv2.CountNonZero(mask);
            VoteForUniqueness(matches, mask);
            nonZero = Cv2.CountNonZero(mask);
            nonZero = VoteForSizeAndOrientation(keypoints2, keypoints1, matches, mask, 1.5f, 20);

            List<Point2f> obj = new List<Point2f>();
            List<Point2f> scene = new List<Point2f>();
            List<DMatch> goodMatchesList = new List<DMatch>();
            //iterate through the mask only pulling out nonzero items because they're matches
            for (int i = 0; i < mask.Rows; i++)
            {
                MatIndexer<byte> maskIndexer = mask.GetGenericIndexer<byte>();
                if (maskIndexer[i] > 0)
                {
                    obj.Add(keypoints1[matches[i][0].QueryIdx].Pt);
                    scene.Add(keypoints2[matches[i][0].TrainIdx].Pt);
                    goodMatchesList.Add(matches[i][0]);
                }
            }

            List<Point2d> objPts = obj.ConvertAll(Point2fToPoint2d);
            List<Point2d> scenePts = scene.ConvertAll(Point2fToPoint2d);
            if (nonZero >= 4)
            {
                Mat homography = Cv2.FindHomography(objPts, scenePts, HomographyMethods.Ransac, 1.5, mask);
                nonZero = Cv2.CountNonZero(mask);

                if (homography != null)
                {
                    Point2f[] objCorners = { new Point2f(0, 0),
                                      new Point2f(img1.Cols, 0),
                                      new Point2f(img1.Cols, img1.Rows),
                                      new Point2f(0, img1.Rows) };

                    Point2d[] sceneCorners = MyPerspectiveTransform3(objCorners, homography);

                    //This is a good concat horizontal
                    using Mat img3 = new Mat(Math.Max(img1.Height, img2.Height), img2.Width + img1.Width, MatType.CV_8UC3);
                    using Mat left = new Mat(img3, new Rect(0, 0, img1.Width, img1.Height));
                    using Mat right = new Mat(img3, new Rect(img1.Width, 0, img2.Width, img2.Height));
                    img1.CopyTo(left);
                    img2.CopyTo(right);

                    mask.GetArray(out byte[] maskBytes);
                    Cv2.DrawMatches(img1, keypoints1, img2, keypoints2, goodMatchesList, img3, Scalar.All(-1), Scalar.All(-1), maskBytes, DrawMatchesFlags.NotDrawSinglePoints);

                    List<List<Point>> listOfListOfPoint2D = new List<List<Point>>();
                    List<Point> listOfPoint2D = new List<Point>
                            {
                                new Point(sceneCorners[0].X + img1.Cols, sceneCorners[0].Y),
                                new Point(sceneCorners[1].X + img1.Cols, sceneCorners[1].Y),
                                new Point(sceneCorners[2].X + img1.Cols, sceneCorners[2].Y),
                                new Point(sceneCorners[3].X + img1.Cols, sceneCorners[3].Y)
                            };
                    listOfListOfPoint2D.Add(listOfPoint2D);
                    img3.Polylines(listOfListOfPoint2D, true, Scalar.LimeGreen, 2);

                    //This works too
                    //Cv2.Line(img3, scene_corners[0] + new Point2d(img1.Cols, 0), scene_corners[1] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
                    //Cv2.Line(img3, scene_corners[1] + new Point2d(img1.Cols, 0), scene_corners[2] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
                    //Cv2.Line(img3, scene_corners[2] + new Point2d(img1.Cols, 0), scene_corners[3] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);
                    //Cv2.Line(img3, scene_corners[3] + new Point2d(img1.Cols, 0), scene_corners[0] + new Point2d(img1.Cols, 0), Scalar.LimeGreen);

                    img3.SaveImage("Kaze_Output.png");
                    Window.ShowImages(img3);
                }
            }
        }

        // to avoid opencvsharp's bug
        static Point2d[] MyPerspectiveTransform1(Point2f[] yourData, Mat transformationMatrix)
        {
            using Mat src = Mat.FromPixelData(yourData.Length, 1, MatType.CV_32FC2, yourData);
            using Mat dst = new Mat();
            Cv2.PerspectiveTransform(src, dst, transformationMatrix);
            dst.GetArray(out Point2f[] dstArray);
            Point2d[] result = Array.ConvertAll(dstArray, Point2fToPoint2d);
            return result;
        }

        // fixed FromArray behavior
        static Point2d[] MyPerspectiveTransform2(Point2f[] yourData, Mat transformationMatrix)
        {
            using var s = Mat<Point2f>.FromArray(yourData);
            using var d = new Mat<Point2f>();
            Cv2.PerspectiveTransform(s, d, transformationMatrix);
            Point2f[] f = d.ToArray();
            return f.Select(Point2fToPoint2d).ToArray();
        }

        // new API
        static Point2d[] MyPerspectiveTransform3(Point2f[] yourData, Mat transformationMatrix)
        {
            Point2f[] ret = Cv2.PerspectiveTransform(yourData, transformationMatrix);
            return ret.Select(Point2fToPoint2d).ToArray();
        }

        static int VoteForSizeAndOrientation(KeyPoint[] modelKeyPoints, KeyPoint[] observedKeyPoints, DMatch[][] matches, Mat mask, float scaleIncrement, int rotationBins)
        {
            int idx = 0;
            int nonZeroCount = 0;
            byte[] maskMat = new byte[mask.Rows];
            var maskHandle = GCHandle.Alloc(maskMat, GCHandleType.Pinned);
            using (var m = Mat.FromPixelData(mask.Rows, 1, MatType.CV_8U, maskHandle.AddrOfPinnedObject()))
            {
                mask.CopyTo(m);
                List<float> logScale = new List<float>();
                List<float> rotations = new List<float>();
                double s, maxS, minS, r;
                maxS = -1.0e-10f; minS = 1.0e10f;

                //if you get an exception here, it's because you're passing in the model and observed keypoints backwards.  Just switch the order.
                for (int i = 0; i < maskMat.Length; i++)
                {
                    if (maskMat[i] > 0)
                    {
                        KeyPoint observedKeyPoint = observedKeyPoints[i];
                        KeyPoint modelKeyPoint = modelKeyPoints[matches[i][0].TrainIdx];
                        s = Math.Log10(observedKeyPoint.Size / modelKeyPoint.Size);
                        logScale.Add((float)s);
                        maxS = s > maxS ? s : maxS;
                        minS = s < minS ? s : minS;

                        r = observedKeyPoint.Angle - modelKeyPoint.Angle;
                        r = r < 0.0f ? r + 360.0f : r;
                        rotations.Add((float)r);
                    }
                }

                int scaleBinSize = (int)Math.Ceiling((maxS - minS) / Math.Log10(scaleIncrement));
                if (scaleBinSize < 2)
                    scaleBinSize = 2;
                float[] scaleRanges = { (float)minS, (float)(minS + scaleBinSize + Math.Log10(scaleIncrement)) };

                using var scalesMat = Mat.FromArray(logScale.ToArray());
                using var rotationsMat = Mat.FromArray(rotations.ToArray());
                using var flagsMat = new Mat<float>(logScale.Count, 1);
                using Mat hist = new Mat();
                flagsMat.SetTo(new Scalar(0.0f));
                float[] flagsMatFloat1 = flagsMat.ToArray();

                int[] histSize = { scaleBinSize, rotationBins };
                float[] rotationRanges = { 0.0f, 360.0f };
                int[] channels = { 0, 1 };
                Rangef[] ranges = { new Rangef(scaleRanges[0], scaleRanges[1]), new Rangef(rotations.Min(), rotations.Max()) };

                Mat[] arrs = { scalesMat, rotationsMat };
                Cv2.CalcHist(arrs, channels, null, hist, 2, histSize, ranges);
                Cv2.MinMaxLoc(hist, out double minVal, out double maxVal);

                Cv2.Threshold(hist, hist, maxVal * 0.5, 0, ThresholdTypes.Tozero);
                Cv2.CalcBackProject(arrs, channels, hist, flagsMat, ranges);

                MatIndexer<float> flagsMatIndexer = flagsMat.GetIndexer();

                for (int i = 0; i < maskMat.Length; i++)
                {
                    if (maskMat[i] > 0)
                    {
                        if (flagsMatIndexer[idx++] != 0.0f)
                        {
                            nonZeroCount++;
                        }
                        else
                            maskMat[i] = 0;
                    }
                }
                m.CopyTo(mask);
            }
            maskHandle.Free();

            return nonZeroCount;
        }

        private static void VoteForUniqueness(DMatch[][] matches, Mat mask, float uniqnessThreshold = 0.80f)
        {
            byte[] maskData = new byte[matches.Length];
            GCHandle maskHandle = GCHandle.Alloc(maskData, GCHandleType.Pinned);
            using (Mat m = Mat.FromPixelData(matches.Length, 1, MatType.CV_8U, maskHandle.AddrOfPinnedObject()))
            {
                mask.CopyTo(m);
                for (int i = 0; i < matches.Length; i++)
                {
                    //This is also known as NNDR Nearest Neighbor Distance Ratio
                    if ((matches[i][0].Distance / matches[i][1].Distance) <= uniqnessThreshold)
                        maskData[i] = 255;
                    else
                        maskData[i] = 0;
                }
                m.CopyTo(mask);
            }
            maskHandle.Free();
        }
    }
}
