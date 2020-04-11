using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.Cuda;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.XFeatures2D;

namespace OpenCVWorkingTest.Detectors
{
    class CirclesGridDetector : ChessboardDetector
    {
        private readonly Feature2D _blobDetector = new FastFeatureDetector();

        protected override Mat Detect()
        {
            var grayImage = new Mat();
            CvInvoke.CvtColor(Frame, grayImage, ColorConversion.Bgr2Gray);

            var sw = Stopwatch.StartNew();

            var result = CvInvoke.FindCirclesGrid(grayImage, PatterSize, Corners, CalibCgType.AsymmetricGrid, _blobDetector);

            sw.Stop();
            Debug.WriteLine("Detect: " + sw.ElapsedMilliseconds + "   -   " + result);

            CvInvoke.DrawChessboardCorners(Frame, PatterSize, Corners, result);

            return Frame;
        }

        public CirclesGridDetector(Size patternSize) : base(patternSize)
        {
        }
    }
}