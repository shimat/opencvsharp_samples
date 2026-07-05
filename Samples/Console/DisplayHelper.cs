using System.Collections.Generic;
using System.IO;
using OpenCvSharp;

namespace SampleBase.Console
{
    /// <summary>
    /// Single entry point for showing sample output. Replaces direct use of
    /// <see cref="Window"/>/<see cref="Cv2.ImShow"/>/<see cref="Cv2.WaitKey(int)"/> so that samples
    /// can run without a display (pass --headless, or set OPENCV_SAMPLES_HEADLESS=1) by writing
    /// images to disk instead of opening windows.
    /// </summary>
    public static class DisplayHelper
    {
        /// <summary>
        /// Set once by <see cref="SamplesCore.Program"/> at startup, from configuration.
        /// </summary>
        public static bool IsHeadless { get; private set; }

        public static void Initialize(bool headless)
        {
            IsHeadless = headless;
        }

        private const string OutputRoot = "headless-output";

        private static readonly Dictionary<string, Window> windowCache = new();
        private static readonly Dictionary<string, int> headlessFrameCounts = new();

        /// <summary>
        /// Shows one window per (title, image) pair, or writes them to disk when headless.
        /// </summary>
        public static void Show(string sampleName, params (string Title, Mat Image)[] frames)
        {
            if (IsHeadless)
            {
                foreach (var (title, image) in frames)
                {
                    WriteToDisk(sampleName, title, image);
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
        public static bool ShowFrame(string sampleName, params (string Title, Mat Frame)[] frames)
            => ShowFrame(sampleName, waitMs: 30, maxHeadlessFrames: 5, frames);

        public static bool ShowFrame(string sampleName, int waitMs, int maxHeadlessFrames, params (string Title, Mat Frame)[] frames)
        {
            if (IsHeadless)
            {
                headlessFrameCounts.TryGetValue(sampleName, out int count);
                if (count >= maxHeadlessFrames)
                    return false;

                foreach (var (title, frame) in frames)
                {
                    WriteToDisk(sampleName, $"{title}_{count:D3}", frame);
                }
                headlessFrameCounts[sampleName] = count + 1;
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
        /// Closes any windows opened by <see cref="ShowFrame(string, (string, Mat)[])"/>. Called
        /// once per sample run by <see cref="ConsoleTestManager"/>; a no-op when headless.
        /// </summary>
        public static void DestroyAll()
        {
            if (IsHeadless)
                return;

            foreach (var window in windowCache.Values)
                window.Dispose();
            windowCache.Clear();
            Cv2.DestroyAllWindows();
        }

        private static void WriteToDisk(string sampleName, string title, Mat image)
        {
            string dir = Path.Combine(OutputRoot, sampleName);
            Directory.CreateDirectory(dir);
            string safeTitle = string.Join("_", title.Split(Path.GetInvalidFileNameChars()));
            Cv2.ImWrite(Path.Combine(dir, $"{safeTitle}.png"), image);
        }
    }
}
