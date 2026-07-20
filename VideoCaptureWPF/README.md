# VideoCaptureWPF

WPF app: live webcam capture with face detection. The WPF counterpart to [`VideoCaptureForm`](../VideoCaptureForm/README.md).

## Running

```console
dotnet run --project VideoCaptureWPF
```

## What it does

Opens the default webcam (device index 0) and continuously grabs frames on a background thread. Each frame is run through a Haar cascade (`CascadeClassifier`, bundled `haarcascade_frontalface_default.xml`) and a red rectangle is drawn around *every* detected face (unlike `VideoCaptureForm`, which only marks the first), then converted to a `WriteableBitmap` on the UI thread and displayed.

## Prerequisites

Windows only. Requires a working webcam. No downloads needed — the cascade file is bundled.
