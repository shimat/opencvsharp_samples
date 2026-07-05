using System.Collections.Generic;
using System.IO;
using OpenCvSharp;

namespace SampleBase.Interfaces
{
    /// <summary>
    /// Per-run helper for showing a sample's output. Replaces direct use of
    /// <see cref="Window"/>/<see cref="Cv2.ImShow"/>/<see cref="Cv2.WaitKey(int)"/> so that a
    /// sample can run without a display (headless mode) by writing images to disk instead of
    /// opening windows. One instance is created per test run and passed into
    /// <see cref="ITestBase.RunTest"/>, so its state never leaks between samples or between
    /// repeated runs of the same sample.
    /// </summary>
    public sealed class DisplayHelper
    {
        private const string OutputRoot = "headless-output";

        private readonly string sampleName;
        private readonly Dictionary<string, Window> windowCache = new();
        private int headlessFrameCount;

        public bool IsHeadless { get; }

        public DisplayHelper(string sampleName, bool isHeadless)
        {
            this.sampleName = sampleName;
            IsHeadless = isHeadless;
        }

        /// <summary>
        /// Shows one window per (title, image) pair, or writes them to disk when headless.
        /// </summary>
        public void Show(params (string Title, Mat Image)[] frames)
        {
            if (IsHeadless)
            {
                foreach (var (title, image) in frames)
                {
                    WriteToDisk(title, image);
                }
                return;
            }

            var titles = new string[frames.Length];
            var images = new Mat[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                titles[i] = frames[i].Title;
                images[i] = frames[i].Image;
            }
            Window.ShowImages(images, titles);
        }

        /// <summary>
        /// Shows one or more simultaneous video frames (e.g. "src"/"dst" side by side) with a
        /// single wait per iteration; call once per loop iteration. Returns false when the caller
        /// should stop looping (either the user asked to quit, or the headless frame cap was
        /// reached). Windows are kept open and updated in place across calls, matching the
        /// original new Window(...) + Image-property-update pattern.
        /// </summary>
        public bool ShowFrame(params (string Title, Mat Frame)[] frames)
            => ShowFrame(waitMs: 30, maxHeadlessFrames: 5, frames);

        public bool ShowFrame(int waitMs, int maxHeadlessFrames, params (string Title, Mat Frame)[] frames)
        {
            if (IsHeadless)
            {
                if (headlessFrameCount >= maxHeadlessFrames)
                    return false;

                foreach (var (title, frame) in frames)
                {
                    WriteToDisk($"{title}_{headlessFrameCount:D3}", frame);
                }
                headlessFrameCount++;
                return true;
            }

            foreach (var (title, frame) in frames)
            {
                if (!windowCache.TryGetValue(title, out var window))
                {
                    window = new Window(title);
                    windowCache[title] = window;
                }
                window.Image = frame;
            }
            return Cv2.WaitKey(waitMs) < 0;
        }

        /// <summary>
        /// Closes any windows opened by <see cref="ShowFrame(int, int, (string, Mat)[])"/>. Called
        /// once per sample run by <c>ConsoleTestManager</c>; a no-op when headless.
        /// </summary>
        public void DestroyAll()
        {
            if (IsHeadless)
                return;

            foreach (var window in windowCache.Values)
                window.Dispose();
            windowCache.Clear();
            Cv2.DestroyAllWindows();
        }

        private void WriteToDisk(string title, Mat image)
        {
            string dir = Path.Combine(OutputRoot, sampleName);
            Directory.CreateDirectory(dir);
            string safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            Cv2.ImWrite(Path.Combine(dir, $"{safeTitle}.png"), image);
        }
    }
}
