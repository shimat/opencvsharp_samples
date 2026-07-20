# AvaloniaViewer

Avalonia UI desktop app: a live Canny edge-detection preview driven by threshold sliders.

## Running

```
dotnet run --project AvaloniaViewer
```

## What it does

Loads a bundled sample image (`Mandrill.bmp`) on startup, or any PNG/JPG/BMP via **File > Open**. Two sliders control the Canny lower/upper thresholds; every change re-runs `Cv2.Canny` and reports the elapsed time and image size in the status bar.

The result is shown as a before/after split view: the original image and the edge-detection output are stacked, with a draggable divider that reveals more or less of the edge output. Mouse wheel zooms (centered on the cursor), dragging pans, and the View menu offers "Fit to Window" / "Actual Size". A "Pixel Inspector" panel shows the (x, y) position, original BGR value, and whether Canny flagged that pixel as an edge.

## Prerequisites

Windows only (as currently configured — only the `win` OpenCvSharp native runtime is referenced, even though Avalonia itself is cross-platform). No webcam or downloads needed.
