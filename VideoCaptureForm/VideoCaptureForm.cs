using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.GdipExtensions;

namespace VideoCaptureForm;

public partial class VideoCaptureForm : Form
{
    private readonly VideoCapture capture;
    private readonly CascadeClassifier cascadeClassifier;

    public VideoCaptureForm()
    {
        InitializeComponent();

        capture = new VideoCapture();
        cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");

        backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
    }

    private void VideoCaptureForm_Load(object sender, EventArgs e)
    {
        capture.Open(0, VideoCaptureAPIs.ANY);
        if (!capture.IsOpened())
        {
            Close();
            return;
        }

        ClientSize = new System.Drawing.Size(capture.FrameWidth, capture.FrameHeight);

        backgroundWorker1.RunWorkerAsync();
    }

    private void VideoCaptureForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        // The worker may still be using capture/cascadeClassifier at this point; actually
        // dispose them once it has observed cancellation and fully exited (see
        // backgroundWorker1_RunWorkerCompleted), not immediately here.
        backgroundWorker1.CancelAsync();
    }

    private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        capture.Dispose();
        cascadeClassifier.Dispose();
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker bgWorker)
        {
            return;
        }

        while (!bgWorker.CancellationPending)
        {
            using var frameMat = capture.RetrieveMat();
            if (frameMat.Empty())
                break;

            var rects = cascadeClassifier.DetectMultiScale(frameMat, 1.1, 5, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));
            if (rects.Length > 0)
            {
                Cv2.Rectangle(frameMat, rects[0], Scalar.Red);
            }

            var frameBitmap = BitmapConverter.ToBitmap(frameMat);
            bgWorker.ReportProgress(0, frameBitmap);
            Thread.Sleep(100);
        }
    }

    private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (e.UserState is Bitmap frameBitmap)
        {
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = frameBitmap;
        }
    }
}
