# tests

Integration tests for [`Samples.WebApi`](../Samples.WebApi/README.md), using xUnit v3 and `WebApplicationFactory` to run the real app in-memory. This is currently the only test project in the repo — there are no tests for the `Samples` console library.

## Running

```
dotnet test tests/Samples.WebApi.Tests/Samples.WebApi.Tests.csproj
```

## What's covered

- `POST /api/cartoonify` with no upload returns a 200 PNG response (verifies the bundled-default-image fallback).
- `POST /api/frame-analysis` with no upload returns at least one streamed frame result (verifies the bundled-default-video fallback).

No special setup needed — runs on both Windows and Linux CI, using only bundled sample assets.
