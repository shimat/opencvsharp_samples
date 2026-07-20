# Samples.Windows

.NET 8 Windows-only app (WinForms + WPF both enabled) with two small interop samples. Unlike `Samples/`, this isn't a menu-driven suite — `Program.cs` hardcodes which sample runs.

## Running

```console
dotnet run --project Samples.Windows
```

By default this runs `MatToWriteableBitmap`. To try `WindowGUISample` instead, edit `Program.cs` and swap the instantiated `ISample`.

## Samples

- **`MatToWriteableBitmap.cs`** — Loads `fruits.jpg` (chosen because its width isn't a multiple of 4, to exercise stride/padding handling) into a `Mat`, converts it to a WPF `WriteableBitmap` via `OpenCvSharp.WpfExtensions`, and displays it in a bare WPF window.
- **`WindowGUISample.cs`** — Three demos using OpenCvSharp's own highgui-style `Window` (not WinForms/WPF controls):
  - `Windows_Example` — opens a `Window` showing an image.
  - `MouseCallBack_Example` — registers a native mouse callback and prints button/wheel events with coordinates.
  - `TrackBar_Example` — two windows with a trackbar that live-adjusts morphology (Open/Close and Erode/Dilate); press `e`/`r`/`c` to switch the structuring-element shape, Esc to exit.

## Data

`Data/Image/` (`fruits.jpg`, `box_in_scene.png`) is bundled; paths are centralized in `FilePath.cs`.

## Prerequisites

Windows only. No webcam or downloads needed.
