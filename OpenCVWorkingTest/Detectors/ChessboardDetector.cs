using System.Diagnostics;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace OpenCVWorkingTest.Detectors
{
    class ChessboardDetector : Detector
    {
        public Size PatterSize { get; set; }

        public ChessboardDetector(Size patternSize)
        {
            PatterSize = patternSize;
        }

        protected override Mat Detect()
        {
            var grayImage = new Mat();
            CvInvoke.CvtColor(Frame, grayImage, ColorConversion.Bgr2Gray);

//            var sw = Stopwatch.StartNew();

            var result = CvInvoke.FindChessboardCorners(grayImage, PatterSize, Corners);

//            sw.Stop();
//            Debug.WriteLine("Detect: " + sw.ElapsedMilliseconds + "   -   " + result);

            if (result)
                CvInvoke.CornerSubPix(grayImage, Corners, PatterSize, new Size(-1, -1), new MCvTermCriteria(30, 0.1));

            CvInvoke.DrawChessboardCorners(Frame, PatterSize, Corners, result);

            return Frame;
        }
    }
}
