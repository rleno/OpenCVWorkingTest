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

using System.Timers;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

using Rectangle = System.Drawing.Rectangle;
using System.Diagnostics;
using Emgu.CV.UI;
//using UnityEngine;



namespace OpenCVWorkingTest
{
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

        #region import

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

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
                _capture.FlipHorizontal = true;

                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }

        private Mat Frame => _frames[_swap ? 0 : 1];
        private Mat SwappedFrame => _frames[!_swap ? 0 : 1];

        private bool _swap;
        Mat [] _frames = new Mat[2] {new Mat(), new Mat()};
        Timer _timer;
        int _fps;
        Stopwatch stopwatch = new Stopwatch();
        Task _detectorTask = Task.CompletedTask;
        Mat _image = new Mat();



        Detector _detector = new Detector(new System.Drawing.Size(6, 4));

        private void DisplayFrame(Mat frame)
        {
            Dispatcher.InvokeAsync(() => { ImageView.Image = frame; });
        }


        private void ProcessFrame(object sender, EventArgs arg)
        {
            _fps++;

            _capture.Retrieve(Frame);

            if (_detectorTask.IsCompleted)
            {
                _detectorTask = _detector.Detect(Frame);

                _swap = !_swap;

            }

            DisplayFrame(Frame);

            //            if (_task == null || _task.IsCompleted)
            //                _task = Task.Run(() =>
            //                {
            //                    //_image = Detect(frame);
            //                });
            //else
            //    Dispatcher.InvokeAsync(() => { ImageView.Image = frame; });

            //Dispatcher.InvokeAsync(()=>{
            //    stopwatch.Start();
            //    ImageView.Source = GetImageSource(frame.ToBitmap(), frame.Width, frame.Height);
            //    stopwatch.Stop();
            //    Console.WriteLine(": " + stopwatch.ElapsedMilliseconds);
            //    stopwatch.Reset();
            //});


            //Task.Run(() =>
            //{
            //    Mat img_detected = Detect(frame);

            //    if (img_detected != null)
            //        Dispatcher.Invoke(() =>
            //        {
            //            ImageView.Source = GetImageSource(img_detected.ToImage.ToBitmap(), frame.Width, frame.Height);
            //        });
            //});
        }

        public MainWindow()
        {
            bool 

            InitializeComponent();
#if DEBUG
            ImageView.FunctionalMode = ImageBox.FunctionalModeOption.Everything;
#else
            ImageView.FunctionalMode = ImageBox.FunctionalModeOption.PanAndZoom;
#endif
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

            _timer = new Timer(1000);
            _timer.Elapsed += _timer_Elapsed;
            _timer.Start();
            //image.Save(@"image.bmp");
            //undist_image.Save(@"undist_image.bmp");

        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine(_fps);
            _fps = 0;
        }

        private ImageSource GetImageSource(Bitmap bitmap, int width, int height)
        {
            BitmapSource source = null;
            var hBitmap = bitmap.GetHbitmap();

            try
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromWidthAndHeight(width, height));
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return source;
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

//public override DocumentPage GetPage(int pageNumber)
//{
//    // Получить запрошенную страницу
//    DocumentPage page = flowDocumentPaginator.GetPage(pageNumber);

//    // Поместить страницу в объект Visual. После этого можно 
//    // будет применять трансформации и добавлять другие элементы
//    ContainerVisual newVisual = new ContainerVisual();
//    newVisual.Children.Add(page.Visual);

//    // Создать заголовок
//    DrawingVisual header = new DrawingVisual();
//    using (DrawingContext dc = header.RenderOpen())
//    {
//        Typeface typeface = new Typeface("Times New Roman");
//        FormattedText text = new FormattedText("Страница " +
//            (pageNumber + 1).ToString(), System.Globalization.CultureInfo.CurrentCulture,
//            FlowDirection.LeftToRight, typeface, 14, Brushes.Black);

//        // Оставить четверть дюйма пространства между краем страницы и текстом
//        dc.DrawText(text, new Point(96 * 0.25, 96 * 0.25));
//    }

//    // Добавить заголовок к объекту Visual
//    newVisual.Children.Add(header);

//    // Поместить объект Visual в новую страницу
//    DocumentPage newPage = new DocumentPage(newVisual);
//    return newPage;
//}