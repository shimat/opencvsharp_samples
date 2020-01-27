using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace VideoCaptureForm
{
    public partial class VideoCaptureForm : Form
    {
        private readonly VideoCapture capture;
        private readonly CascadeClassifier cascadeClassifier;

        public VideoCaptureForm()
        {
            InitializeComponent();

            capture = new VideoCapture();
            cascadeClassifier = new CascadeClassifier("haarcascade_frontalface_default.xml");
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
            capture.Dispose();
            cascadeClassifier.Dispose();
            backgroundWorker1.CancelAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgWorker = (BackgroundWorker) sender;

            while (!bgWorker.CancellationPending)
            {
                using (var frameMat = capture.RetrieveMat())
                {
                    var rects = cascadeClassifier.DetectMultiScale(frameMat, 1.1, 5, HaarDetectionType.ScaleImage, new OpenCvSharp.Size(30, 30));
                    if (rects.Length > 0)
                    {
                        Cv2.Rectangle(frameMat, rects[0], Scalar.Red);
                    }

                    var frameBitmap = BitmapConverter.ToBitmap(frameMat);
                    bgWorker.ReportProgress(0, frameBitmap);
                }
                Thread.Sleep(100);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var frameBitmap = (Bitmap)e.UserState;
            pictureBox1.Image?.Dispose();
            pictureBox1.Image = frameBitmap;
        }
    }
}
