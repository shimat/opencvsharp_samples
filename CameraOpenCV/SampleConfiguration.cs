//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

using OpenCvSharp;

namespace SDKTemplate
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "OpenCV C# Sample";

        public List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Camera Example Operations", ClassType=typeof(Scenario1_ExampleOperations)},
            new Scenario() { Title="Image Example Operations", ClassType=typeof(Scenario2_ImageOperations)},
        };

        public List<Algorithm> algorithms = new List<Algorithm>
        {
            new Algorithm()
            {
                AlgorithmName ="Blur",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    //Cv2.Blur()
                    new AlgorithmProperty(0,typeof(Size),"Ksize","A Size object representing the size of the kernel.",480,1,5),
                    new AlgorithmProperty(1,typeof(Point),"Anchor","A variable of the type integer representing the anchor point.",5,1,3),
                    new AlgorithmProperty(2,typeof(BorderTypes),"BorderType","A variable of the type integer representing the type of the border to be used to the output.",7,0,0),
                }
            },
            new Algorithm()
            {
                AlgorithmName ="HoughLines",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    //Cv2.HoughLinesP()
                    //Cv2.Line()
                    new AlgorithmProperty(0,typeof(double),"Rho","A variable of the type double representing the resolution of the parameter r in pixels.",100,1,1),
                    new AlgorithmProperty(1,typeof(double),"Theta","A variable of the type double representing the resolution of the parameter Φ in radians.",157,1,1),
                    new AlgorithmProperty(2,typeof(int),"Threshold","A variable of the type integer representing the minimum number of intersections to “detect” a line.",100,1,10),
                    new AlgorithmProperty(3,typeof(double),"MinLineLength","The minimum line length. Line segments shorter than that will be rejected. [By default this is 0]",50,0,0),
                    new AlgorithmProperty(4,typeof(double),"MaxLineGap","The maximum allowed gap between points on the same line to link them. [By default this is 0]",50,0,0),
                    new AlgorithmProperty(5,typeof(Scalar),"Color","Line color.",255,0,0),
                    new AlgorithmProperty(6,typeof(int),"Thickness","Line thickness. [By default this is 1]",10,0,1),
                    new AlgorithmProperty(7,typeof(LineTypes),"LineTypes","Type of the line. [By default this is LineType.Link8]",2,0,1),
                }
            },
            new Algorithm()
            {
                //Cv2.FindContours() 
                //Cv2.DrawContours()
                AlgorithmName ="Contours",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(RetrievalModes),"RetrievalModes","Contour retrieval mode.",4,0,0),
                    new AlgorithmProperty(1,typeof(ContourApproximationModes),"ContourApproximationModes","Contour approximation method",3,0,0),
                    new AlgorithmProperty(2,typeof(Point),"Offset","Optional offset by which every contour point is shifted. This is useful if the contours are extracted from the image ROI and then they should be analyzed in the whole image context. ",10,0,0),
                    new AlgorithmProperty(3,typeof(Scalar),"Color","Line color.",255,0,0),
                    new AlgorithmProperty(4,typeof(int),"Thickness","Line thickness. [By default this is 1]",10,0,1),
                    new AlgorithmProperty(5,typeof(LineTypes),"LineTypes","Type of the line. [By default this is LineType.Link8]",2,0,1),
                    new AlgorithmProperty(6,typeof(double),"Threshold1","The first threshold for the hysteresis procedure.",255,0,50),
                    new AlgorithmProperty(7,typeof(double),"Threshold2","The second threshold for the hysteresis procedure.",255,0,200),
                    new AlgorithmProperty(8,typeof(int),"Length","The second threshold for the hysteresis procedure.",100,0,10),
                }
            },
            new Algorithm()
            {
                // MP! Todo: Sort out better range handling.
                AlgorithmName ="Canny",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"Threshold1","The first threshold for the hysteresis procedure.",255,0,50),
                    new AlgorithmProperty(1,typeof(double),"Threshold2","The second threshold for the hysteresis procedure.",255,0,200),
                    new AlgorithmProperty(2,typeof(int),"ApertureSize","Aperture size for the Sobel operator [By default this is ApertureSize.Size3]",7,1,3),
                }
            },
            new Algorithm()
            {
                // MP! Todo: Resolve usage of params.
                AlgorithmName ="Histogram",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"threshold1")
                }
            },
            new Algorithm()
            {
                AlgorithmName ="MotionDetector",
                AlgorithmProperties = new List<AlgorithmProperty>()
                {
                    new AlgorithmProperty(0,typeof(double),"Threshold1", "Learning rate.", 1, 0, 0.5)
                }
            },
        };
    }

    public enum AlgorithmPropertyType
    {
        currentValue = 0,
        parameterName,
        description,
        settingVisibility,
        maxValue,
        minValue
    }

    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
