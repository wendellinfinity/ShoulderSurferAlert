using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.GPU;

namespace ShoulderSurferAlert {
     /// <summary>
     /// Interaction logic for MainWindow.xaml
     /// </summary>
     public partial class MainWindow : Window {

          DispatcherTimer _timer; // used to refresh our image
          Capture _capture; // this is our main capture device
          System.Diagnostics.Stopwatch _stopwatch;


          private const string FACEFILENAME = @"haarcascade_frontalface_default.xml";

          public MainWindow() {
               InitializeComponent();
               this._stopwatch = new System.Diagnostics.Stopwatch();
               // declare the timer
               this._timer = new DispatcherTimer();
               // tick the timer per 10 ms
               this._timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
               // instantiate our capture device
               this._capture = new Capture();

               // initialize our timer and assign what happens every tick
               this._timer.Tick += new EventHandler(delegate(object sender, EventArgs e) {
                    this.DetectShoulderSurfer(); // call the worker function
               });
               // dispose the capture device
               Application.Current.Exit += new ExitEventHandler(
                    delegate(object sender, ExitEventArgs e) {
                         if (this._capture != null) {
                              this._capture.Dispose();
                         }
                    });
               // start the fun!
               this._timer.Start();
          }


          // our function to do all the work
          protected void DetectShoulderSurfer() {
               this._stopwatch.Reset();
               this._stopwatch.Start();
               // get the current frame from our webcam
               Image<Bgr, Byte> frame = _capture.QueryFrame();
               // push our frame to the image container
               surferView.Source = Emgu.CV.WPF.BitmapSourceConvert.ToBitmapSource(frame);
               //Read the HaarCascade objects
               /*
               using (HaarCascade face = new HaarCascade(FACEFILENAME)) {
                    //Convert it to Grayscale
                    using (Image<Gray, Byte> gray = frame.Convert<Gray, Byte>()) {
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
                         foreach (MCvAvgComp f in facesDetected) {
                              //draw the face detected in the 0th (gray) channel with blue color
                              System.Diagnostics.Debug.WriteLine("Shoulder surfer detected!");
                              //surferView.Draw(f.rect, new Bgr(Color.Blue), 2);
                         }
                    }
               }
                */
               this._stopwatch.Stop();
               System.Diagnostics.Debug.WriteLine("Elapsed {0}s", this._stopwatch.Elapsed.TotalSeconds);
          }
     }
}
