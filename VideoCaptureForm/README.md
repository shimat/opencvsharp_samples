# VideoCaptureForm

WinForms app: live webcam capture with face detection.

## Running

```console
dotnet run --project VideoCaptureForm
```

## What it does

Opens the default webcam (device index 0) and continuously grabs frames on a background thread. Each frame is run through a Haar cascade (`CascadeClassifier`, bundled `haarcascade_frontalface_default.xml`) and a red rectangle is drawn around the first detected face, then displayed in a `PictureBox`.

See also [`VideoCaptureWPF`](../VideoCaptureWPF/README.md), the WPF equivalent (which draws a rectangle around *every* detected face, not just the first).

## Prerequisites

Windows only. Requires a working webcam. No downloads needed — the cascade file is bundled.
