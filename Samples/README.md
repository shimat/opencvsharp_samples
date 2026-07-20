# Samples

.NET 8 console app with an interactive menu of small, focused OpenCvSharp samples, grouped by module.

## Running

```
dotnet run --project Samples
```

You'll get a numbered menu grouped by category (`Core`, `ImgProc`, `Features2D`, `Calib3D`, `Video`, `Photo`, `ObjDetect`, `Dnn`, `Ml`, `Stitching`). Enter a number to run that sample, `c` to clear the screen, `h` for help, `0` to exit.

### Headless mode

Samples that would normally open a `Window` or block on `Cv2.WaitKey` can instead run without a display by writing their output frames as PNGs under `headless-output/<SampleName>/`:

```
dotnet run --project Samples -- --headless
# or
OPENCV_SAMPLES_HEADLESS=1 dotnet run --project Samples
```

This is how the whole sample suite can be smoke-tested in CI. Samples that inherently need live hardware (e.g. `CameraCaptureSample`) skip themselves with a warning when run headless.

## Structure

- `Program.cs` — discovers every sample class via reflection (`ITestBase` + `[SampleCategory]`) and hands off to the console menu.
- `Console/` — the menu/runner (`ConsoleTestManager`, `ConsoleTestBase`).
- `Interfaces/DisplayHelper.cs` — the display abstraction each sample uses instead of calling highgui directly, so the same sample code works both windowed and headless.
- `Samples/` — the actual sample implementations, one subfolder per module (see below).
- `Data/` — bundled images, video clips, cascade/model files, and ML datasets used by the samples. No downloads needed.
- `Path.cs` — centralizes paths into `Data/` (`ImagePath`, `MoviePath`, `TextPath`).

## Sample catalog (`Samples/Samples/`)

- **Calib3D** — camera calibration + undistortion from chessboard images; stereo disparity (`StereoBM`/`StereoSGBM`).
- **Core** — DCT/DFT round trips, `Mat` submatrix/ROI operations, array-based pixel access and performance comparisons, multidimensional scaling, split/merge, solving linear equations.
- **Dnn** — super-resolution upscaling with a bundled FSRCNN model.
- **Features2D** — keypoint detectors and matchers: BRISK, FAST, FREAK, KAZE/AKAZE, MSER, SIFT/SURF, SimpleBlobDetector, StarDetector, FLANN matching, best-match bounding box.
- **ImgProc** — CLAHE, connected components, GrabCut, histograms, Hough lines, morphology, perspective warp, square/rectangle detection, Delaunay triangulation (`Subdiv2D`), watershed, adaptive binarization.
- **Ml** — SVM regression/classification.
- **ObjDetect** — ArUco marker detection, Haar/LBP face detection, HOG people detection, QR code detection.
- **Photo** — inpainting, watermark removal, seamless (Poisson) cloning.
- **Stitching** — image stitching from cropped overlapping patches. **Known issue**: currently not working reliably (see the `TODO` in `Stitching.cs`).
- **Video** — background subtraction, live webcam capture, Kalman filter tracking, CSRT object tracking, Lucas-Kanade optical flow, video read/write.

## Prerequisites

None beyond the .NET 8 SDK — all data assets are bundled. `Video/CameraCaptureSample.cs` additionally needs a webcam and is skipped automatically in headless mode.
