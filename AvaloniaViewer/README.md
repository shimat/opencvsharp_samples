# AvaloniaViewer

Avalonia UI desktop app: a live Canny edge-detection preview driven by threshold sliders.

## Running

```console
dotnet run --project AvaloniaViewer
```

## What it does

Loads a bundled sample image (`Mandrill.bmp`) on startup, or any PNG/JPG/BMP via **File > Open**. Two sliders control the Canny lower/upper thresholds; every change re-runs `Cv2.Canny` and reports the elapsed time and image size in the status bar.

The result is shown as a before/after split view: the original image and the edge-detection output are stacked, with a draggable divider that reveals more or less of the edge output. Mouse wheel zooms (centered on the cursor), dragging pans, and the View menu offers "Fit to Window" / "Actual Size". A "Pixel Inspector" panel shows the (x, y) position, original BGR value, and whether Canny flagged that pixel as an edge.

## Prerequisites

Windows and Linux x64 (this project references both `OpenCvSharp5.runtime.win` and `OpenCvSharp5.official.runtime.linux-x64.slim` — the app only calls `Cv2.ImRead`/`CvtColor`/`Canny`, so the reduced `slim` module set is enough and there's no GTK3/libdrm concern to work around). On Linux this is still an Avalonia desktop app, not a headless service: it needs an X11 or Wayland display server to show its window, regardless of which OpenCvSharp native package is referenced. No webcam or downloads needed.
