using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using OpenCvSharp;
using AvaPoint = Avalonia.Point;
using AvaRect = Avalonia.Rect;

namespace AvaloniaViewer;

public partial class MainWindow : Avalonia.Controls.Window
{
    private enum DragMode
    {
        None,
        Pan,
        Divider,
    }

    private const double DividerHitTolerance = 8; // screen pixels

    private readonly ScaleTransform zoomTransform = new();
    private readonly TranslateTransform panTransform = new();

    private Mat sourceColor = new();
    private Mat sourceGray = new();
    private readonly Mat edges = new();

    private WriteableBitmap? beforeBitmap;
    private WriteableBitmap? afterBitmap;

    private int imageWidth;
    private int imageHeight;
    private double dividerX;

    private DragMode dragMode = DragMode.None;
    private AvaPoint dragStartPointer;
    private AvaPoint dragStartPan;

    public MainWindow()
    {
        InitializeComponent();

        ImageCanvas.RenderTransform = new TransformGroup { Children = { zoomTransform, panTransform } };

        LoadImage("Mandrill.bmp");

        // ViewerHost isn't laid out yet during the constructor, so the initial fit
        // (triggered from LoadImage) is a no-op; retry once real bounds are available.
        Opened += (_, _) => FitToWindow();

        Closing += (_, _) =>
        {
            sourceColor.Dispose();
            sourceGray.Dispose();
            edges.Dispose();
            beforeBitmap?.Dispose();
            afterBitmap?.Dispose();
        };
    }

    private void LoadImage(string path)
    {
        using var loaded = Cv2.ImRead(path, ImreadModes.Color);
        if (loaded.Empty())
        {
            StatusText.Text = $"Failed to load '{path}'. The file may be missing, corrupt, or an unsupported format.";
            return;
        }

        sourceColor.Dispose();
        sourceGray.Dispose();
        sourceColor = loaded.Clone();
        sourceGray = new Mat();
        Cv2.CvtColor(sourceColor, sourceGray, ColorConversionCodes.BGR2GRAY);

        imageWidth = sourceColor.Width;
        imageHeight = sourceColor.Height;

        beforeBitmap?.Dispose();
        beforeBitmap = sourceColor.ToAvaloniaBitmap();
        BeforeImage.Source = beforeBitmap;

        zoomTransform.ScaleX = 1;
        zoomTransform.ScaleY = 1;
        panTransform.X = 0;
        panTransform.Y = 0;
        dividerX = imageWidth / 2.0;

        UpdateEdges();
        FitToWindow();
    }

    private void FitToWindow()
    {
        if (imageWidth == 0 || imageHeight == 0)
        {
            return;
        }

        var viewport = ViewerHost.Bounds;
        if (viewport.Width <= 0 || viewport.Height <= 0)
        {
            return; // Not laid out yet; the Opened handler retries this after the first layout pass.
        }

        var scale = Math.Min(viewport.Width / imageWidth, viewport.Height / imageHeight);
        SetZoomCentered(scale, viewport);
    }

    private void OnFitToWindowClick(object? sender, RoutedEventArgs e) => FitToWindow();

    private void OnActualSizeClick(object? sender, RoutedEventArgs e) => SetZoomCentered(1, ViewerHost.Bounds);

    private void SetZoomCentered(double scale, AvaRect viewport)
    {
        zoomTransform.ScaleX = scale;
        zoomTransform.ScaleY = scale;
        panTransform.X = (viewport.Width - (imageWidth * scale)) / 2;
        panTransform.Y = (viewport.Height - (imageHeight * scale)) / 2;
    }

    private void UpdateEdges()
    {
        // Guards against ValueChanged firing before the first LoadImage call completes.
        if (sourceGray.Empty())
        {
            return;
        }

        Threshold1ValueText.Text = $"{Threshold1Slider.Value:F0}";
        Threshold2ValueText.Text = $"{Threshold2Slider.Value:F0}";

        var stopwatch = Stopwatch.StartNew();
        Cv2.Canny(sourceGray, edges, Threshold1Slider.Value, Threshold2Slider.Value);
        stopwatch.Stop();

        afterBitmap?.Dispose();
        afterBitmap = edges.ToAvaloniaBitmap();
        AfterImage.Source = afterBitmap;
        UpdateDividerVisual();

        StatusText.Text = $"{imageWidth} x {imageHeight}px    Canny: {stopwatch.Elapsed.TotalMilliseconds:F1} ms";
    }

    private void UpdateDividerVisual()
    {
        dividerX = Math.Clamp(dividerX, 0, imageWidth);

        Canvas.SetLeft(DividerLine, dividerX);
        Canvas.SetTop(DividerLine, 0);
        DividerLine.Height = imageHeight;

        AfterImage.Clip = new RectangleGeometry(new AvaRect(dividerX, 0, imageWidth - dividerX, imageHeight));
    }

    private void OnThresholdChanged(object? sender, RangeBaseValueChangedEventArgs e) => UpdateEdges();

    private async void OnOpenClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return;
        }

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Image",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Image Files") { Patterns = ["*.png", "*.jpg", "*.jpeg", "*.bmp"] },
            ],
        });

        var file = files.FirstOrDefault();
        if (file is null)
        {
            return;
        }

        LoadImage(file.Path.LocalPath);
    }

    private AvaPoint ScreenToImage(AvaPoint screenPoint) => new(
        (screenPoint.X - panTransform.X) / zoomTransform.ScaleX,
        (screenPoint.Y - panTransform.Y) / zoomTransform.ScaleY);

    private void UpdatePixelInspector(AvaPoint imagePos)
    {
        var x = (int)Math.Floor(imagePos.X);
        var y = (int)Math.Floor(imagePos.Y);

        if (x < 0 || y < 0 || x >= imageWidth || y >= imageHeight)
        {
            PixelPosText.Text = "-";
            PixelColorText.Text = "-";
            PixelEdgeText.Text = "-";
            return;
        }

        var color = sourceColor.Get<Vec3b>(y, x);
        var edgeValue = edges.Get<byte>(y, x);

        PixelPosText.Text = $"({x}, {y})";
        PixelColorText.Text = $"B={color.Item0} G={color.Item1} R={color.Item2}";
        PixelEdgeText.Text = edgeValue.ToString();
    }

    private void OnViewerPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var screenPos = e.GetPosition(ViewerHost);
        var dividerScreenX = dividerX * zoomTransform.ScaleX + panTransform.X;

        dragMode = Math.Abs(screenPos.X - dividerScreenX) <= DividerHitTolerance ? DragMode.Divider : DragMode.Pan;
        dragStartPointer = screenPos;
        dragStartPan = new AvaPoint(panTransform.X, panTransform.Y);

        e.Pointer.Capture(ViewerHost);
        UpdateCursor(screenPos);
    }

    private void OnViewerPointerMoved(object? sender, PointerEventArgs e)
    {
        var screenPos = e.GetPosition(ViewerHost);
        var imagePos = ScreenToImage(screenPos);
        UpdatePixelInspector(imagePos);
        UpdateCursor(screenPos);

        switch (dragMode)
        {
            case DragMode.Pan:
                var delta = screenPos - dragStartPointer;
                panTransform.X = dragStartPan.X + delta.X;
                panTransform.Y = dragStartPan.Y + delta.Y;
                break;

            case DragMode.Divider:
                dividerX = imagePos.X;
                UpdateDividerVisual();
                break;
        }
    }

    private void OnViewerPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        dragMode = DragMode.None;
        e.Pointer.Capture(null);
        UpdateCursor(e.GetPosition(ViewerHost));
    }

    private void UpdateCursor(AvaPoint screenPos)
    {
        var dividerScreenX = dividerX * zoomTransform.ScaleX + panTransform.X;
        var nearDivider = Math.Abs(screenPos.X - dividerScreenX) <= DividerHitTolerance;

        ViewerHost.Cursor = dragMode == DragMode.Divider || (dragMode == DragMode.None && nearDivider)
            ? new Cursor(StandardCursorType.SizeWestEast)
            : Cursor.Default;
    }

    private void OnViewerPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var screenPos = e.GetPosition(ViewerHost);
        var imagePosBeforeZoom = ScreenToImage(screenPos);

        var factor = e.Delta.Y > 0 ? 1.1 : 1 / 1.1;
        var newScale = Math.Clamp(zoomTransform.ScaleX * factor, 0.02, 64);

        zoomTransform.ScaleX = newScale;
        zoomTransform.ScaleY = newScale;

        // Keep the point under the cursor fixed on screen while the scale changes.
        panTransform.X = screenPos.X - (imagePosBeforeZoom.X * newScale);
        panTransform.Y = screenPos.Y - (imagePosBeforeZoom.Y * newScale);

        e.Handled = true;
    }
}
