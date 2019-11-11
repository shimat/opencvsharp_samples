using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Media;
using Windows.Storage;
using Windows.Foundation;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Controls.Primitives;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2_ImageOperations : Page
    {
        enum OperationType : int
        {
            Blur = 0,
            HoughLines,
            Contours,
            Canny,
            MotionDetector
        }

        private const int ImageRows = 480;
        private const int ImageCols = 640;

        public AlgorithmProperty StoredProperty { get; set; }
        public AlgorithmProperty LastStoredProperty { get; set; }
        private Algorithm storeditem;

        private MainPage rootPage;

        private FrameRenderer previewRenderer;
        private FrameRenderer outputRenderer;

        private DispatcherTimer fpsTimer;
        private int frameCount = 0;

        private OperationType currentOperation;
        private OcvOp operation;

        // Rendering
        private readonly SemaphoreSlim mLock = new SemaphoreSlim(1);
        private VideoFrame cacheFrame;

        public Scenario2_ImageOperations()
        {
            this.InitializeComponent();
            previewRenderer = new FrameRenderer(PreviewImage);
            outputRenderer = new FrameRenderer(OutputImage);

            fpsTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            fpsTimer.Tick += UpdateFps;
        }

        private void UpdateFps(object sender, object e)
        {
            var fc = Interlocked.Exchange(ref frameCount, 0);
            this.FPSMonitor.Text = "FPS: " + fc;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;

            // setting up the combobox, and default operation
            OperationComboBox.ItemsSource = Enum.GetValues(typeof(OperationType));
            OperationComboBox.SelectedIndex = (int)OperationType.Blur;

            operation = new OcvOp();
            FileOpen.IsEnabled = true;
            FileSaving.IsEnabled = true;
            fpsTimer.Start();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs args)
        {
            fpsTimer.Stop();
        }

        /// <summary>
        /// Handles a frame arrived event and renders the frame to the screen.
        /// </summary>
        private async Task ProcessWithOpenCV(VideoFrame frame)
        {
            if (frame != null)
            {
                SoftwareBitmap originalBitmap = null;
                var inputBitmap = frame.SoftwareBitmap;
                if (inputBitmap != null)
                {
                    try
                    {
                        // The XAML Image control can only display images in BRGA8 format with premultiplied or no alpha
                        // The frame reader as configured in this sample gives BGRA8 with straight alpha, so need to convert it
                        originalBitmap = SoftwareBitmap.Convert(inputBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

                        SoftwareBitmap outputBitmap = new SoftwareBitmap(BitmapPixelFormat.Bgra8, originalBitmap.PixelWidth, originalBitmap.PixelHeight, BitmapAlphaMode.Premultiplied);

                        // Operate on the image in the manner chosen by the user.
                        if (currentOperation == OperationType.Blur)
                        {
                            operation.Blur(originalBitmap, outputBitmap, storeditem);
                        }
                        else if (currentOperation == OperationType.HoughLines)
                        {
                            operation.HoughLines(originalBitmap, outputBitmap, storeditem);
                        }
                        else if (currentOperation == OperationType.Contours)
                        {
                            operation.Contours(originalBitmap, outputBitmap, storeditem);
                        }
                        else if (currentOperation == OperationType.Canny)
                        {
                            operation.Canny(originalBitmap, outputBitmap, storeditem);
                        }
                        else if (currentOperation == OperationType.MotionDetector)
                        {
                        }

                        // Display both the original bitmap and the processed bitmap.
                        previewRenderer.RenderFrame(originalBitmap);
                        outputRenderer.RenderFrame(outputBitmap);
                    }
                    catch (Exception ex)
                    {
                        await (new MessageDialog(ex.Message)).ShowAsync();
                    }
                }

                Interlocked.Increment(ref frameCount);
            }
        }

        private async void OnOpComboBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            // Process ComboBox selection when first loaded or when selection has changed from the current.
            if (comboBox != null && comboBox.IsLoaded == true && (int)currentOperation == comboBox.SelectedIndex)
                return;

            currentOperation = (OperationType)((sender as ComboBox).SelectedItem);

            if (OperationType.Blur == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Blur";
            }
            else if (OperationType.Contours == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Contours";
            }
            else if (OperationType.Canny == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Canny";
            }
            else if (OperationType.HoughLines == currentOperation)
            {
                this.CurrentOperationTextBlock.Text = "Current: Line detection";
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
            storeditem = rootPage.algorithms[OperationComboBox.SelectedIndex];

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                collection.ItemsSource = Algorithm.GetObjects(storeditem);
            });

            if (cacheFrame != null)
                await ProcessWithOpenCV(cacheFrame);
        }

        private async void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var slider = sender as Slider;

            if (storeditem != null && StoredProperty != null)
            {
                if (StoredProperty.isInitialize)
                {
                    if (slider.Tag?.ToString() != StoredProperty.ParameterName) return;
                    if (slider.Value >= StoredProperty.MinValue && slider.Value <= StoredProperty.MaxValue)
                    {
                        StoredProperty.CurrentValue = slider.Value;
                        UpdateStoredAlgorithm(currentOperation, StoredProperty);
                        if (cacheFrame != null) await ProcessWithOpenCV(cacheFrame);
                    }
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        // initialize slider
                        collection.ItemsSource = Algorithm.GetObjects(storeditem);
                        StoredProperty.isInitialize = true;
                    });
                }
            }
        }

        private async void OnCollectionItemClick(object sender, ItemClickEventArgs e)
        {
            // Get the collection item corresponding to the clicked item.
            var container = collection.ContainerFromItem(e.ClickedItem) as ListViewItem;

            if (container != null)
            {
                // Stash the clicked item for use later. We'll need it when we connect back from the detailpage.
                StoredProperty = container.Content as AlgorithmProperty;
                storeditem.RevertEnable(StoredProperty.ParameterName);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    collection.ItemsSource = Algorithm.GetObjects(storeditem);
                });
            }
        }

        private void UpdateStoredAlgorithm(OperationType operationType, AlgorithmProperty algorithmProperty)
        {
            if (OperationType.Blur == operationType)
            {
                if (algorithmProperty.ParameterName == "Ksize")
                {
                    storeditem.UpdateCurrentValue(algorithmProperty);
                    storeditem.UpdateProperty("Anchor", AlgorithmPropertyType.maxValue, algorithmProperty.CurrentDoubleValue);
                }
                else
                {
                    storeditem.UpdateCurrentValue(algorithmProperty);
                }
            }
            else if (OperationType.Contours == currentOperation)
            {
                storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.Canny == currentOperation)
            {
                storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.HoughLines == currentOperation)
            {
                storeditem.UpdateCurrentValue(algorithmProperty);
            }
            else if (OperationType.MotionDetector == currentOperation)
            {
                storeditem.UpdateCurrentValue(algorithmProperty);
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
                if (SettingPanel.Visibility == Visibility.Collapsed)
                {
                    Setting.Glyph = "\uE751";
                    SettingPanel.Visibility = Visibility.Visible;
                }
                else
                {
                    Setting.Glyph = "\uE713";
                    SettingPanel.Visibility = Visibility.Collapsed;
                }
            });
        }

        // Saving output Image
        private async void OnSave(object sender, RoutedEventArgs e)
        {
        }

        // Open a Exist Image
        private async void OnOpen(object sender, RoutedEventArgs e)
        {
            FileOpen.IsEnabled = false;
            FileSaving.IsEnabled = false;

            await mLock.WaitAsync();

            try
            {
                cacheFrame = await LoadVideoFrameFromFilePickedAsync();
                if (cacheFrame != null)
                {
                    SoftwareBitmapSource source = new SoftwareBitmapSource();
                    await source.SetBitmapAsync(cacheFrame.SoftwareBitmap);
                    PreviewImage.Source = source;
                    await ProcessWithOpenCV(cacheFrame);
                }
            }
            catch (Exception ex)
            {
                await (new MessageDialog(ex.Message)).ShowAsync();
            }

            mLock.Release();

            FileOpen.IsEnabled = true;
            FileSaving.IsEnabled = true;
        }

        /// <summary>
        /// Launch file picker for user to select a picture file and return a VideoFrame
        /// </summary>
        /// <returns>VideoFrame instanciated from the selected image file</returns>
        public static IAsyncOperation<VideoFrame> LoadVideoFrameFromFilePickedAsync()
        {
            return AsyncInfo.Run(async (token) =>
            {
                // Trigger file picker to select an image file
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
                StorageFile selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();

                if (selectedStorageFile == null)
                {
                    return null;
                }

                // Decoding image file content into a SoftwareBitmap, and wrap into VideoFrame
                VideoFrame resultFrame = null;
                SoftwareBitmap softwareBitmap = null;
                using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Create the decoder from the stream 
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    // Convert to friendly format for UI display purpose
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Encapsulate the image in a VideoFrame instance
                resultFrame = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);

                return resultFrame;
            });
        }

        private async void OnComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (storeditem != null && StoredProperty != null)
            {
                if (StoredProperty.isInitialize)
                {
                    var combo = sender as ComboBox;
                    var selectIdx = combo.SelectedIndex;
                    if (combo.Tag?.ToString() != StoredProperty.ParameterName) return;
                    StoredProperty.CurrentValue = (double)selectIdx;
                    UpdateStoredAlgorithm(currentOperation, StoredProperty);
                    if (cacheFrame != null) await ProcessWithOpenCV(cacheFrame);
                }
                else
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        // initialize slider
                        collection.ItemsSource = Algorithm.GetObjects(storeditem);
                        StoredProperty.isInitialize = true;
                    });
                }
            }
        }
    }
}
