using System;
using System.Collections.Generic;
using System.IO;
using OpenCvSharp;

namespace SampleBase.Console
{
    /// <summary>
    /// Single entry point for showing sample output. Replaces direct use of
    /// <see cref="Window"/>/<see cref="Cv2.ImShow"/>/<see cref="Cv2.WaitKey(int)"/> so that samples
    /// can run without a display (set OPENCV_SAMPLES_HEADLESS=1, or run under CI where the
    /// CI environment variable is set) by writing images to disk instead of opening windows.
    /// </summary>
    public static class DisplayHelper
    {
        public static bool IsHeadless { get; } =
            Environment.GetEnvironmentVariable("OPENCV_SAMPLES_HEADLESS") is "1" or "true"
            || Environment.GetEnvironmentVariable("CI") is not null;

        private const string OutputRoot = "headless-output";

        private static readonly Dictionary<string, Window> windowCache = new();
        private static readonly Dictionary<string, int> headlessFrameCounts = new();

        /// <summary>
        /// Shows a single image, or writes it to disk when headless.
        /// </summary>
        public static void Show(string sampleName, string title, Mat image)
        {
            Show(sampleName, new[] { title }, new[] { image });
        }

        /// <summary>
        /// Shows one window per image/title pair, or writes them to disk when headless.
        /// </summary>
        public static void Show(string sampleName, string[] titles, Mat[] images)
        {
            if (IsHeadless)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    WriteToDisk(sampleName, titles[i], images[i]);
                }
                return;
            }

            Window.ShowImages(images, titles);
        }

        /// <summary>
        /// Shows a single video frame; call once per loop iteration. Returns false when the
        /// caller should stop looping (either the user asked to quit, or the headless frame cap
        /// was reached).
        /// </summary>
        public static bool ShowFrame(string sampleName, string title, Mat frame, int waitMs = 30, int maxHeadlessFrames = 5)
        {
            return ShowFrame(sampleName, new[] { title }, new[] { frame }, waitMs, maxHeadlessFrames);
        }

        /// <summary>
        /// Shows multiple simultaneous video frames (e.g. "src"/"dst" side by side) with a single
        /// wait per iteration. Windows are kept open and updated in place across calls, matching
        /// the original new Window(...) + Image-property-update pattern.
        /// </summary>
        public static bool ShowFrame(string sampleName, string[] titles, Mat[] frames, int waitMs = 30, int maxHeadlessFrames = 5)
        {
            if (IsHeadless)
            {
                headlessFrameCounts.TryGetValue(sampleName, out int count);
                if (count >= maxHeadlessFrames)
                    return false;

                for (int i = 0; i < frames.Length; i++)
                {
                    WriteToDisk(sampleName, $"{titles[i]}_{count:D3}", frames[i]);
                }
                headlessFrameCounts[sampleName] = count + 1;
                return true;
            }

            for (int i = 0; i < frames.Length; i++)
            {
                if (!windowCache.TryGetValue(titles[i], out var window))
                {
                    window = new Window(titles[i]);
                    windowCache[titles[i]] = window;
                }
                window.Image = frames[i];
            }
            return Cv2.WaitKey(waitMs) < 0;
        }

        /// <summary>
        /// Closes any windows opened by <see cref="ShowFrame(string,string,Mat,int,int)"/>. Called
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
