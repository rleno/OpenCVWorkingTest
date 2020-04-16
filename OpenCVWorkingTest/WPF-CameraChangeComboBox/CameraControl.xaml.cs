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

            Detector = new ChessboardDetector();


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



      

