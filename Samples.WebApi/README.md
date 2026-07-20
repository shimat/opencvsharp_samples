# Samples.WebApi

ASP.NET Core minimal API (.NET 10) exposing two image/video processing endpoints. Cross-platform (Windows + Linux), unlike most of the other sample projects.

## Running

```console
dotnet run --project Samples.WebApi
```

Then open `http://localhost:5000` (or whatever port is printed) — `wwwroot/index.html` is a small demo page with file inputs for both endpoints.

## Endpoints

- **`POST /api/cartoonify`** — Accepts an optional `image` multipart file (falls back to the bundled `Mandrill.bmp` if omitted). Produces a cartoon-style PNG: an edge mask from grayscale + median blur + adaptive threshold, combined with a bilateral-filtered (flattened-color) version of the image.
- **`POST /api/frame-analysis`** — Accepts an optional `video` multipart file (falls back to the bundled `bach.mp4` if omitted). Streams per-frame brightness and edge-pixel-ratio results back as they're computed, using ASP.NET Core's native `IAsyncEnumerable` JSON streaming — the client sees results incrementally instead of waiting for the whole video.

## Docker

The Dockerfile must be built from the **repo root**, not from inside this folder, because it pulls in `Samples/Data/` assets:

```console
docker build -f Samples.WebApi/Dockerfile -t samples-webapi .
docker run --rm -p 8080:8080 samples-webapi
```

This project references `OpenCvSharp5.official.runtime.linux-x64.headless` rather than the full `OpenCvSharp5.official.runtime.linux-x64` package: same module set (including `VideoCapture`), but built with `highgui` disabled, so GTK3/X11 are no longer needed. The base image still installs `libdrm2` — the headless build currently still links `libdrm.so.2` unconditionally, tracked upstream as [opencvsharp#2071](https://github.com/shimat/opencvsharp/issues/2071); that `apt-get` step can go away once the fix is released.

## Tests

Integration tests for these endpoints live in [`tests/`](../tests/README.md).

## Prerequisites

None — both endpoints work out of the box against bundled sample assets when no file is uploaded.
