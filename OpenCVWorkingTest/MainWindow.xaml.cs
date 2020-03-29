using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
//using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using Rectangle = System.Drawing.Rectangle;
//using UnityEngine;



namespace OpenCVWorkingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly Mat _cameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
        readonly Mat _distCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);
        private float _squareSize = 30.0f;

        #region Camera Capture Variables
        private VideoCapture _capture = null; //Camera
        //private bool _captureInProgress = false; //Variable to track camera state
        //int CameraDevice = 0; //Variable to track camera device selected
        //Video_Device[] WebCams; //List containing all the camera available
        #endregion

        private void SetupCapture(int Camera_Identifier)
        {
            //update the selected device
            //CameraDevice = Camera_Identifier;

            //Dispose of Capture if it was created before
            if (_capture != null) _capture.Dispose();
            try
            {
                //Set up capture device
                _capture = new VideoCapture(Camera_Identifier);

                _capture.SetCaptureProperty(CapProp.FrameWidth, 1280);
                _capture.SetCaptureProperty(CapProp.FrameHeight, 720);

                _capture.SetCaptureProperty(CapProp.FrameCount, 30);

                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs arg)
        {
            //***If you want to access the image data the use the following method call***/
            //Image<Bgr, Byte> frame = new Image<Bgr,byte>(_capture.RetrieveBgrFrame().ToBitmap());
            Mat image = new Mat();
            _capture.Read(image);

            Mat img_detected = Detect(image);

            Dispatcher.Invoke(()=>{
                ImageView.Source = GetImageSource(img_detected.ToBitmap(), image.Width, image.Height);
            });
            //because we are using an autosize picturebox we need to do a thread safe update
            //DisplayImage(frame.ToBitmap());
            //}
            //else if (RetrieveGrayFrame.Checked)
            //{
            //    Image<Gray, Byte> frame = _capture.RetrieveGrayFrame();
            //    //because we are using an autosize picturebox we need to do a thread safe update
            //    DisplayImage(frame.ToBitmap());
            //}
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private Mat GenarateChessbosard()
        {
            int w = 10,
                h = 10,
                s = 200;

            var image = new Mat(w*s, h*s, DepthType.Cv8U, 4);

            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    MCvScalar c;
                    if ((y % 2 + x) % 2 == 0)
                        c = new MCvScalar(0, 0, 0, 255);
                    else
                        c = new MCvScalar(255, 255, 255, 0);

                    CvInvoke.Rectangle(image, new Rectangle(x * s, y * s, s, s), c, -1, LineType.FourConnected);
                }
            
            image.Save(@"d:\tmp\chessboard1.bmp");

            return image;
        }

        private Mat Detect(Mat image)
        {
            var gray_image = new Mat();

            CvInvoke.CvtColor(image, gray_image, ColorConversion.Bgr2Gray);

            var patternSize = new System.Drawing.Size(6, 4);
            var corners = new VectorOfPointF();

            bool result = CvInvoke.FindChessboardCorners(gray_image, patternSize, corners);

            if (result)
            {
                CvInvoke.CornerSubPix(gray_image, corners, patternSize, new System.Drawing.Size(-1, -1), new MCvTermCriteria(30, 0.1));
                CvInvoke.DrawChessboardCorners(image, patternSize, corners, result);

                Calculate(corners, image);
            }

            return image;
        }

        private void Calculate(VectorOfPointF corners, Mat gray_image)
        {
            var patternSize = new System.Drawing.Size(6, 4);

            var objectList = new List<MCvPoint3D32f>();

            for (int i = 0; i < patternSize.Height; i++)
                for (int j = 0; j < patternSize.Width; j++)
                {
                    objectList.Add(new MCvPoint3D32f(j * _squareSize, i * _squareSize, 0.0F));
                }


            var cmat = new double[,]{
            {1.0, .0, .0},
            {.0,  1.0, .0},
            {.0,  .0, 1.0}
            };

            //var ortho = Matrix4x4.Ortho(-4, 4, -2, 2, 1, 100);

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    _cameraMatrix.SetValue(r, c, cmat[r, c]);

            Mat _rvecs, _tvecs;
            Mat[] _rvecs1, _tvecs1;

            for (int i = 0; i < 8; i++)
                _distCoeffs.SetValue(i, 0, 0);

            _rvecs = new Mat(3, 1, DepthType.Cv64F, 1);
            _tvecs = new Mat(3, 1, DepthType.Cv64F, 1);


            MCvPoint3D32f[][] _cornersObjectList = new MCvPoint3D32f[1][];
            _cornersObjectList[0] = objectList.ToArray();
            PointF[][] _cornersPointsList = new PointF[1][];
            _cornersPointsList[0] = corners.ToArray();

            CvInvoke.CalibrateCamera(_cornersObjectList, _cornersPointsList, gray_image.Size, _cameraMatrix, _distCoeffs
                    , CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out _rvecs1, out _tvecs1);

            for (int i = 0; i < 8; i++)
                _distCoeffs.SetValue(i, 0, 0);


            CvInvoke.SolvePnP(_cornersObjectList[0], _cornersPointsList[0], _cameraMatrix, _distCoeffs, _rvecs, _tvecs);

            var rmat = new Mat(3, 3, DepthType.Cv32F, 1);
            CvInvoke.Rodrigues(_rvecs, rmat);

            var Fx = Math.Atan2(rmat.GetValue(2, 1), rmat.GetValue(2, 2)) * 57.295779513;
            var Fy = Math.Atan2(-rmat.GetValue(2, 0), Math.Sqrt(rmat.GetValue(2, 1) * rmat.GetValue(2, 1) + rmat.GetValue(2, 2) * rmat.GetValue(2, 2))) * 57.295779513;
            var Fz = Math.Atan2(rmat.GetValue(1, 0), rmat.GetValue(0, 0)) * 57.295779513;

            Dispatcher.Invoke(() =>
            {
                FX.Text = Fx.ToString();
                FY.Text = Fy.ToString();
                FZ.Text = Fz.ToString();

                DX.Text = _tvecs.GetValue(0, 0).ToString();
                DY.Text = _tvecs.GetValue(0, 1).ToString(); 
                DZ.Text = _tvecs.GetValue(0, 2).ToString(); 
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //GenarateChessbosard();

            //var image = new Mat(@"calib.io_checker_200x150_8x11_15_cut.jpg");
            //var image = new Mat(@"d:\tmp\chessboard_3dsmax.bmp");
            
            //var gray_image = new Mat();

            //CvInvoke.CvtColor(image, gray_image, ColorConversion.Bgr2Gray);
            //            CvInvoke.Imwrite(@"c:\tmp\output.jpg", gray_image);
            
            //var patternSize = new System.Drawing.Size(9, 9);
            //var corners = new VectorOfPointF();

            //bool result = CvInvoke.FindChessboardCorners(gray_image, patternSize, corners);

            //CvInvoke.CornerSubPix(gray_image, corners, patternSize, new System.Drawing.Size(-1, -1), new MCvTermCriteria(30, 0.1));

            //CvInvoke.DrawChessboardCorners(image, patternSize, corners, result);




            //var objectList = new List<MCvPoint3D32f>();

            //for (int i = 0; i < patternSize.Height; i++)
            //    for (int j = 0; j < patternSize.Width; j++)
            //    {
            //        objectList.Add(new MCvPoint3D32f(j * _squareSize, i * _squareSize, 0.0F));
            //    }


            //var cmat = new double[,]{
            //{1.0, .0, .0},
            //{.0,  1.0, .0},
            //{.0,  .0, 1.0}
            //};

            ////var ortho = Matrix4x4.Ortho(-4, 4, -2, 2, 1, 100);

            //for (int r = 0; r < 3; r++)
            //    for (int c = 0; c < 3; c++)
            //        _cameraMatrix.SetValue(r, c, cmat[r, c]);

            //Mat _rvecs, _tvecs;
            //Mat[] _rvecs1, _tvecs1;

            //for (int i = 0; i < 8; i++)
            //    _distCoeffs.SetValue(i, 0, 0);

            //_rvecs = new Mat(3, 1, DepthType.Cv64F, 1);
            //_tvecs = new Mat(3, 1, DepthType.Cv64F, 1);


            //MCvPoint3D32f[][] _cornersObjectList = new MCvPoint3D32f[1][];
            //_cornersObjectList[0] = objectList.ToArray();
            //PointF[][] _cornersPointsList = new PointF[1][];
            //_cornersPointsList[0] = corners.ToArray();


            //CvInvoke.CalibrateCamera(_cornersObjectList, _cornersPointsList, gray_image.Size, _cameraMatrix, _distCoeffs
            //    , CalibType.RationalModel, new MCvTermCriteria(30, 0.1), out _rvecs1, out _tvecs1);

            //for (int i = 0; i < 8; i++)
            //    _distCoeffs.SetValue(i, 0, 0);


            //CvInvoke.SolvePnP(_cornersObjectList[0], _cornersPointsList[0], _cameraMatrix, _distCoeffs, _rvecs, _tvecs);

            //var rmat = new Mat(3, 3, DepthType.Cv32F, 1);
            //CvInvoke.Rodrigues(_rvecs, rmat);

            //var Fx = Math.Atan2(rmat.GetValue(2, 1), rmat.GetValue(2, 2)) * 57.295779513;
            //var Fy = Math.Atan2(-rmat.GetValue(2, 0), Math.Sqrt(rmat.GetValue(2, 1) * rmat.GetValue(2, 1) + rmat.GetValue(2, 2) * rmat.GetValue(2, 2))) * 57.295779513;
            //var Fz = Math.Atan2(rmat.GetValue(1, 0), rmat.GetValue(0, 0)) * 57.295779513;

            ////CvInvoke.CalibrateCamera(objectsPoints, imagePoints, gray_image.Size, _cameraMatrix, _distCoeffs, _rvecs, _tvecs
            ////    , CalibType.RationalModel, new MCvTermCriteria(30, 0.1));

            //var undist_image = new Mat();
            //CvInvoke.Undistort(image, undist_image, _cameraMatrix, _distCoeffs);
            //ImageView.Source = GetImageSource(undist_image.ToBitmap(), image.Width, image.Height);


            SetupCapture(0);
            _capture.Start();
            //image.Save(@"image.bmp");
            //undist_image.Save(@"undist_image.bmp");

        }

        private ImageSource GetImageSource(System.Drawing.Bitmap bitmap, int width, int height)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(width, height));
        }
    }

    public static class MatExtension
    {
        public static dynamic GetValue(this Mat mat, int row, int col)
        {
            var value = CreateElement(mat.Depth);
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static void SetValue(this Mat mat, int row, int col, dynamic value)
        {
            var target = CreateElement(mat.Depth, value);
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
        }
        private static dynamic CreateElement(DepthType depthType, dynamic value)
        {
            var element = CreateElement(depthType);
            element[0] = value;
            return element;
        }

        private static dynamic CreateElement(DepthType depthType)
        {
            if (depthType == DepthType.Cv8S)
            {
                return new sbyte[1];
            }
            if (depthType == DepthType.Cv8U)
            {
                return new byte[1];
            }
            if (depthType == DepthType.Cv16S)
            {
                return new short[1];
            }
            if (depthType == DepthType.Cv16U)
            {
                return new ushort[1];
            }
            if (depthType == DepthType.Cv32S)
            {
                return new int[1];
            }
            if (depthType == DepthType.Cv32F)
            {
                return new float[1];
            }
            if (depthType == DepthType.Cv64F)
            {
                return new double[1];
            }
            return new float[1];
        }
    }
}