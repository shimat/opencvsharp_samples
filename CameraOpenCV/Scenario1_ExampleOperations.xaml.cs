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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using Windows.Graphics.Imaging;
using Windows.Media.Capture.Frames;
using Windows.Media.Capture;
using System.Threading.Tasks;
using System.Diagnostics;
using Windows.Media.MediaProperties;
using System.Collections.ObjectModel;

using OpenCvSharp;

namespace SDKTemplate
{
    /// <summary>
    /// Scenario that illustrates using OpenCV along with camera frames. 
    /// </summary>
    public sealed partial class Scenario1_ExampleOperations : Page
    {
        private MainPage rootPage;

        private MediaCapture _mediaCapture = null;
        private MediaFrameReader _reader = null;
        private FrameRenderer _previewRenderer = null;
        private FrameRenderer _outputRenderer = null;

        private Algorithm _storeditem { get; set; }
        public AlgorithmProperty _storedProperty { get; set; }
        public AlgorithmProperty _lastStoredProperty { get; set; }

        private int _frameCount = 0;

        private const int IMAGE_ROWS = 480;
        private const int IMAGE_COLS = 640;

        private OcvOp _op = new OcvOp();

        private DispatcherTimer _FPSTimer = null;

        enum OperationType
        {
            Blur = 0,
            HoughLines,
            Contours,
            Canny,
            Histogram,
            MotionDetector
        }
        OperationType currentOperation;

        public Scenario1_ExampleOperations()
        {
            this.InitializeComponent();
            _previewRenderer = new FrameRenderer(PreviewImage);
            _outputRenderer = new FrameRenderer(OutputImage);

            _FPSTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _FPSTimer.Tick += UpdateFPS;
        }

        /// <summary>
        /// Initializes the MediaCapture object with the given source group.
        /// </summary>
        /// <param name="sourceGroup">SourceGroup with which to initialize.</param>
        private async Task InitializeMediaCaptureAsync(MediaFrameSourceGroup sourceGroup)
        {
            if (_mediaCapture != null)
            {
                return;
            }

            _mediaCapture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings()
            {
                SourceGroup = sourceGroup,
                SharingMode = MediaCaptureSharingMode.ExclusiveControl,
                StreamingCaptureMode = StreamingCaptureMode.Video,
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };
            await _mediaCapture.InitializeAsync(settings);
        }

        /// <summary>
        /// Unregisters FrameArrived event handlers, stops and disposes frame readers
        /// and disposes the MediaCapture object.
        /// </summary>
        private async Task CleanupMediaCaptureAsync()
        {
            if (_mediaCapture != null)
            {
                await _reader.StopAsync();
                _reader.FrameArrived -= ColorFrameReader_FrameArrivedAsync;
                _reader.Dispose();
                _mediaCapture = null;
            }
        }

        private void UpdateFPS(object sender, object e)
        {
            var frameCount = Interlocked.Exchange(ref _frameCount, 0);
            this.FPSMonitor.Text = "FPS: " + frameCount;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            App.dispatcher = this.Dispatcher;
            Cv2.InitContainer((object)App.container);
            //_helper.SetContainer(App.container);
            rootPage = MainPage.Current;

            // setting up the combobox, and default operation
            OperationComboBox.ItemsSource = Enum.GetValues(typeof(OperationType));
            OperationComboBox.SelectedIndex = 0;
            currentOperation = OperationType.Blur;

            // Find the sources 
            var allGroups = await MediaFrameSourceGroup.FindAllAsync();
            var sourceGroups = allGroups.Select(g => new
            {
                Group = g,
                SourceInfo = g.SourceInfos.FirstOrDefault(i => i.SourceKind == MediaFrameSourceKind.Color)
            }).Where(g => g.SourceInfo != null).ToList();

            if (sourceGroups.Count == 0)
            {
                // No camera sources found
                return;
            }
            var selectedSource = sourceGroups.FirstOrDefault();

            // Initialize MediaCapture
            try
            {
                await InitializeMediaCaptureAsync(selectedSource.Group);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("MediaCapture initialization error: " + exception.Message);
                await CleanupMediaCaptureAsync();
                return;
            }

            // Create the frame reader
            MediaFrameSource frameSource = _mediaCapture.FrameSources[selectedSource.SourceInfo.Id];
            BitmapSize size = new BitmapSize() // Choose a lower resolution to make the image processing more performant
            {
                Height = IMAGE_ROWS,
                Width = IMAGE_COLS
            };
            _reader = await _mediaCapture.CreateFrameReaderAsync(frameSource, MediaEncodingSubtypes.Bgra8, size);
            _reader.FrameArrived += ColorFrameReader_FrameArrivedAsync;
            await _reader.StartAsync();

            _FPSTimer.Start();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs args)
        {
            _FPSTimer.Stop();
            await CleanupMediaCaptureAsync();
        }

        /// <summary>
        /// Handles a frame arrived event and renders the frame to the screen.
        /// </summary>
        private void ColorFrameReader_FrameArrivedAsync(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            var frame = sender.TryAcquireLatestFrame();
            if (frame != null)
            {
                SoftwareBitmap originalBitmap = null;
                var inputBitmap = frame.VideoMediaFrame?.SoftwareBitmap;
                if (inputBitmap != null)
                {
                    // The XAML Image control can only display images in BRGA8 format with premultiplied or no alpha
                    // The frame reader as configured in this sample gives BGRA8 with straight alpha, so need to convert it
                    originalBitmap = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                    SoftwareBitmap outputBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, originalBitmap.PixelWidth, originalBitmap.PixelHeight, BitmapAlphaMode.Premultiplied);

                    // Operate on the image in the manner chosen by the user.
                    if (currentOperation == OperationType.Blur)
                    {
                        _op.Blur(originalBitmap, outputBitmap, _storeditem);
                    }
                    else if (currentOperation == OperationType.HoughLines)
                    {
                        _op.HoughLines(originalBitmap, outputBitmap, _storeditem);
                    }
                    else if (currentOperation == OperationType.Contours)
                    {
                        _op.Contours(originalBitmap, outputBitmap, _storeditem);
                    }
                    else if (currentOperation == OperationType.Canny)
                    {
                        _op.Canny(originalBitmap, outputBitmap, _storeditem);
                    }
                    else if (currentOperation == OperationType.MotionDetector)
                    {
                        _op.MotionDetector(originalBitmap, outputBitmap, _storeditem);
                    }
                    else if (currentOperation == OperationType.Histogram)
                    {
                        // TODO
                        //App.CvHelper.Histogram(originalBitmap, outputBitmap);
                    }

                    // Display both the original bitmap and the processed bitmap.
                    _previewRenderer.RenderFrame(originalBitmap);
                    _outputRenderer.RenderFrame(outputBitmap);
                }

                Interlocked.Increment(ref _frameCount);
            }
        }

        private async void OperationComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //UpdateAlgorithm(_storeditem);
            currentOperation = (OperationType)((sender as ComboBox).SelectedItem);

            if (OperationType.Contours != currentOperation)
                App.container.Visibility = Visibility.Collapsed;
            else
                App.container.Visibility = Visibility.Visible;

            if (OperationType.Blur == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Blur";
            }
            else if (OperationType.Contours == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Contours";
            }
            else if (OperationType.Histogram == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Histogram of RGB channels";
            }
            else if (OperationType.HoughLines == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Line detection";
            }
            else if (OperationType.Canny == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Canny";
            }
            else if (OperationType.MotionDetector == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Motion detection";
            }
            else
            {
                this.CurrentOperationTextBlock.Text = string.Empty;
            }

            rootPage.algorithms[OperationComboBox.SelectedIndex].ResetEnable();
            _storeditem = rootPage.algorithms[OperationComboBox.SelectedIndex];
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                collection.ItemsSource = Algorithm.GetObjects(_storeditem);
            });
        }



        private async void Slider1_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (_storeditem != null && _storedProperty != null)
                {
                    var res = sender as Slider;
                    if (res.Tag.ToString() != _storedProperty.ParameterName) return;
                    if (_storedProperty.isInitialize)
                    {
                        _storedProperty.CurrentValue = res.Value;
                        UpdateStoredAlgorithm(currentOperation, _storedProperty);
                    }
                    else
                    {
                        // initialize slider
                        collection.ItemsSource = Algorithm.GetObjects(_storeditem);
                        _storedProperty.isInitialize = true;
                    }
                }
            });
        }

        private void Slider1_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {

        }

        private void Slider2_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void Slider3_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }


        private async void Collection_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the collection item corresponding to the clicked item.
            var container = collection.ContainerFromItem(e.ClickedItem) as ListViewItem;
            if (container != null)
            {
                // Stash the clicked item for use later. We'll need it when we connect back from the detailpage.
                _storedProperty = container.Content as AlgorithmProperty;
                _storeditem.RevertEnable(_storedProperty.ParameterName);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    collection.ItemsSource = Algorithm.GetObjects(_storeditem);
                });
            }
        }

        private void UpdateAlgorithm(Algorithm algorithm)
        {
            if (algorithm != null)
            {
                for (int i = 0; i < rootPage.algorithms.Count; i++)
                {
                    if (rootPage.algorithms[i].AlgorithmName == algorithm.AlgorithmName)
                    {
                        rootPage.algorithms[i].SetObjects(algorithm);
                    }
                }
            }
        }

        private void UpdateStoredAlgorithm(OperationType operationType, AlgorithmProperty algorithmProperty)
        {
            if (OperationType.Blur == operationType)
            {
                if (algorithmProperty.ParameterName == "Ksize")
                {
                    _storeditem.UpdateCurrentValue(algorithmProperty);
                    _storeditem.UpdateProperty("Anchor", AlgorithmPropertyType.maxValue, algorithmProperty.CurrentDoubleValue);
                }
                else
                {
                    _storeditem.UpdateCurrentValue(algorithmProperty);
                }
            }
            else if (OperationType.Contours == currentOperation)
            {
                _storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.Histogram == currentOperation)
            {
                _storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.HoughLines == currentOperation)
            {
                _storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.Canny == currentOperation)
            {
                _storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.MotionDetector == currentOperation)
            {
                _storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else
            {
            }
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

        private async void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (collection.Visibility == Visibility.Collapsed)
                {
                    Setting.Glyph = "\uE751";
                    collection.Visibility = Visibility.Visible;
                }
                else
                {
                    Setting.Glyph = "\uE713";
                    collection.Visibility = Visibility.Collapsed;
                    //Slider1.Visibility = Visibility.Collapsed;
                }
            });
        }

        private async void Collection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //if(collection.SelectedIndex!=-1)
                //{
                //    //int marginVal = collection.SelectedIndex * ((int)collection.ActualHeight / _storeditem.algorithmProperties.Count);
                //    int marginVal = collection.SelectedIndex * 200;
                //    SliderGrid.Margin = new Thickness(0, 120 + marginVal, 0, 0);
                //}
                //Slider1.Visibility = Visibility.Visible;
                //curValue.Visibility = Visibility.Visible;
            });
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
