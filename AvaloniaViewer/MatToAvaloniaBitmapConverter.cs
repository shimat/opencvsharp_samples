using System;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using OpenCvSharp;

namespace AvaloniaViewer;

/// <summary>
/// Converts <see cref="Mat"/> to an Avalonia-displayable bitmap, analogous to what
/// OpenCvSharp.WpfExtensions.BitmapSourceConverter does for WPF's BitmapSource.
/// </summary>
public static class MatToAvaloniaBitmapConverter
{
    public static WriteableBitmap ToAvaloniaBitmap(this Mat mat)
    {
        using var bgra = new Mat();
        switch (mat.Channels())
        {
            case 1:
                Cv2.CvtColor(mat, bgra, ColorConversionCodes.GRAY2BGRA);
                break;
            case 3:
                Cv2.CvtColor(mat, bgra, ColorConversionCodes.BGR2BGRA);
                break;
            case 4:
                mat.CopyTo(bgra);
                break;
            default:
                throw new NotSupportedException($"Unsupported channel count: {mat.Channels()}");
        }

        var size = new PixelSize(bgra.Width, bgra.Height);
        // Alpha is always opaque here, so Premul vs. straight alpha makes no numeric difference.
        var bitmap = new WriteableBitmap(size, new Vector(96, 96), PixelFormats.Bgra8888, AlphaFormat.Premul);

        using var buffer = bitmap.Lock();
        unsafe
        {
            var src = (byte*)bgra.Data;
            var dst = (byte*)buffer.Address;
            var rowBytes = size.Width * 4;
            for (var y = 0; y < size.Height; y++)
            {
                Buffer.MemoryCopy(
                    src + y * bgra.Step(),
                    dst + y * buffer.RowBytes,
                    buffer.RowBytes,
                    rowBytes);
            }
        }

        return bitmap;
    }
}
