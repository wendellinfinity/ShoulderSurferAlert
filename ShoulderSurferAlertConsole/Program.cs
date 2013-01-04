using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.GPU;
using System.Drawing;

namespace ShoulderSurferAlertConsole {
     class Program : IDisposable {
          private const string FACEFILENAME = @"haarcascade_frontalface_default.xml";
          private System.Threading.Timer _runner;
          private Capture _capture; // this is our main capture device
          private System.Diagnostics.Stopwatch _stopwatch;

          private System.Windows.Forms.NotifyIcon notifyIcon;
          private System.Windows.Forms.ContextMenu contextMenu;
          private System.Windows.Forms.MenuItem menuItem;
          private System.ComponentModel.IContainer components;

          static void Main(string[] args) {
               using (Program execution = new Program()) {
                    while (Console.ReadKey().Key == ConsoleKey.Enter) {
                         break;
                    }
               }
          }

          public Program() {
               bool isProcessing = false;
               this._stopwatch = new System.Diagnostics.Stopwatch();
               this._capture = new Capture();

               this.components = new System.ComponentModel.Container();
               this.contextMenu = new System.Windows.Forms.ContextMenu();
               this.menuItem = new System.Windows.Forms.MenuItem();
               this.contextMenu.MenuItems.AddRange(
                           new System.Windows.Forms.MenuItem[] { this.menuItem });
               this.menuItem.Index = 0;
               this.menuItem.Text = "E&xit";
               this.menuItem.Click += new System.EventHandler(
                    delegate(object sender, EventArgs e) {
                         Application.Exit();
                    });
               this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
               notifyIcon.Icon = new Icon("info.ico");
               notifyIcon.ContextMenu = this.contextMenu;
               notifyIcon.Text = "Shoulder Surfer Alert";
               notifyIcon.Visible = true;
               
               this._runner = new System.Threading.Timer(new System.Threading.TimerCallback(
                    delegate(object state) {
                         if (!(bool)state) {
                              state = true;
                              this.ProcessFaces();
                              state = false;
                         }
                    }), isProcessing, 0, 500);
          }

          public void ProcessFaces() {
               this._stopwatch.Reset();
               this._stopwatch.Start();
               // get the current frame from our webcam
               Image<Bgr, Byte> frame = _capture.QueryFrame();
               Image<Gray, Byte> gray;
               //Read the HaarCascade objects
               using (HaarCascade face = new HaarCascade(FACEFILENAME)) {
                    //Convert it to Grayscale
                    gray = frame.Convert<Gray, Byte>();
                    //normalizes brightness and increases contrast of the image
                    gray._EqualizeHist();
                    //Detect the faces  from the gray scale image and store the locations as rectangle
                    //The first dimensional is the channel
                    //The second dimension is the index of the rectangle in the specific channel
                    MCvAvgComp[] facesDetected = face.Detect(
                         gray,
                         1.1,
                         10,
                         Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING,
                         new System.Drawing.Size(20, 20));
                    if (facesDetected != null && facesDetected.Length > 0) {
                         this.notifyIcon.ShowBalloonTip(5000, "Alert!", "Shoulder surfer detected!",ToolTipIcon.Warning);
                         Console.WriteLine("Shoulder surfer detected!");
                    }

               }
               this._stopwatch.Stop();
               Console.WriteLine("Elapsed {0}s", this._stopwatch.Elapsed.TotalSeconds);
          }


          #region IDisposable Members

          public void Dispose() {
               if (this._capture != null) {
                    this._capture.Dispose();
               }
          }

          #endregion
     }
}
