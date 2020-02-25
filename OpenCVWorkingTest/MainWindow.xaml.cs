using System;
using System.Collections.Generic;
using System.Linq;
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

namespace OpenCVWorkingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var image = new Mat(@"calib.io_checker_200x150_8x11_15.jpg");
            var gray_image = new Mat();

            CvInvoke.CvtColor(image, gray_image, ColorConversion.Bgr2Gray);
            //            CvInvoke.Imwrite(@"c:\tmp\output.jpg", gray_image);

            var patternSize = new System.Drawing.Size(9, 6);
            var corners = new VectorOfPointF();

            bool result = CvInvoke.FindChessboardCorners(gray_image, patternSize, corners);

            CvInvoke.DrawChessboardCorners(image, patternSize, corners, result);

            ImageView.Source = GetImageSource(image.ToBitmap(), gray_image.Width, gray_image.Height);
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
}