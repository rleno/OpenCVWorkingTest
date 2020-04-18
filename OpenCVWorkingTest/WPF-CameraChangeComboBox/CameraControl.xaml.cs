using System;

using System.Windows;
using System.Windows.Controls;

using Emgu.CV;
using Emgu.CV.Structure;

using DirectShowLib;
using Emgu.CV.CvEnum;

namespace Camera_EMGU_WPFSample
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : UserControl
    {

        private VideoCapture capture = null;

        private DsDevice[] webCams = null;

        public int CameraInitIndex { get; set; }

        private AbstractDetector Detector;

        private Mat Frame = new Mat();

        public CameraControl()
        {
            InitializeComponent();

            //Detector = new ChessboardDetector();
            Detector = new ArucoDetector();

            SetMatrix();
        }

        private void SetMatrix()
        {
            //var cMat = new double[,]
            //{
            //    {1.0017554539927537e+03,    .0,                         5.3714642465820737e+02},
            //    {.0,                        1.0017554539927537e+03,     3.0522634021232267e+02},
            //    {.0,                        .0,                         1.0}
            //};

            //for (int r = 0; r < 3; r++)
            //    for (int c = 0; c < 3; c++)
            //        Detector.cameraMatrix.SetValue(r, c, cMat[r, c]);

            //for (int i = 0; i < 8; i++)
            //    Detector.distCoeffs.SetValue(i, 0, 0);

            //Detector.distCoeffs.SetValue(0, 0, -9.9898495516849192e-02);
            //Detector.distCoeffs.SetValue(1, 0, 2.2626694964843341e-01);
            //Detector.distCoeffs.SetValue(2, 0, -1.1544241181928370e-02);
            //Detector.distCoeffs.SetValue(3, 0, -2.6699808935566056e-03);
            //Detector.distCoeffs.SetValue(4, 0, -1.9438362201403153e-01);
            var cMat = new double[,]
    {
                {1398.38,    .0,                         1082.4},
                {.0,                        1398.38,     627.885},
                {.0,                        .0,                         1.0}
    };


            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    Detector.cameraMatrix.SetValue(r, c, cMat[r, c]);

            for (int i = 0; i < 8; i++)
                Detector.distCoeffs.SetValue(i, 0, 0);

            Detector.distCoeffs.SetValue(0, 0, -0.175286);
            Detector.distCoeffs.SetValue(1, 0, 0.0287032);

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            webCams = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

            for (int i = 0; i < webCams.Length; i++)

                Combo.Items.Add(webCams[i].Name);

            //Combo.SelectedIndex
        }

        private void SetupCapture(int Camera_Identifier)
        {

            try
            {
                if (webCams.Length == 0)
                {
                    throw new Exception("camera error");
                }
                else if (Combo.SelectedItem == null)
                {
                    throw new Exception("change camera");
                }
                else if (capture != null)
                {
                    capture.Start();
                }
                else
                {
                    capture = new VideoCapture(Camera_Identifier);


                    capture.SetCaptureProperty(CapProp.FrameWidth, 640);
                    capture.SetCaptureProperty(CapProp.FrameHeight, 480);

                    capture.SetCaptureProperty(CapProp.FrameCount, 30);


                    capture.ImageGrabbed += Capture_ImageGrabbed;
                    capture.Start();

                    Emgu.CV.UI.ImageBox a;
                 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public void stopCapture()
        {
            try
            {
                if (capture != null)
                {
                    capture.Pause();

                    capture.Dispose();

                    capture = null;

                    CapturedImageBox.Image.Dispose();

                    CapturedImageBox.Image = null;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            stopCapture();
            SetupCapture(Combo.SelectedIndex);

        }

        private void Capture_ImageGrabbed(object sender, EventArgs e)
        {
            capture.Retrieve(Frame);

            Detector.detect(Frame, new System.Drawing.Size(9,6));

            CapturedImageBox.Invoke(new Action(() => CapturedImageBox.Image = Frame.ToImage<Bgr, Byte>()));

            Dispatcher.Invoke(ShowData);
        }

        private void ShowData()
        {
            TextBlockFx.Text = Detector.Fx.ToString("#000.00");
            TextBlockFy.Text = Detector.Fy.ToString("#000.00");
            TextBlockFz.Text = Detector.Fz.ToString("#000.00");
            TextBlockDx.Text = Detector.Dx.ToString("#000.00");
            TextBlockDy.Text = Detector.Dy.ToString("#000.00");
            TextBlockDz.Text = Detector.Dz.ToString("#000.00");
        }
    }
}



      

