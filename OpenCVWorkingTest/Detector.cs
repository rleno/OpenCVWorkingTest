using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace OpenCVWorkingTest
{
    public class Detector
    {
        public Size PatterSize { get; set; }
        public VectorOfPointF Corners { get; }

        public event EventHandler ObjectDetected;

        private Thread _detectThread;
        private EventWaitHandle _waitFrame;
        private EventWaitHandle _waitDetect;

        private Mat _frame;
        public Mat Frame { get; set; }


        public Detector(Size patternSize)
        {
            PatterSize = patternSize;
            Corners = new VectorOfPointF();

            _waitFrame = new EventWaitHandle(false, EventResetMode.ManualReset);
            _waitDetect = new EventWaitHandle(false, EventResetMode.AutoReset);

            _detectThread = new Thread(DetectThread)
            {
                Name = "DetectThread",
                Priority = ThreadPriority.Highest
            };
            
            _detectThread.Start();
        }

        public async Task Detect(Mat frame)
        {
            _frame = frame;

            _waitFrame.Set();

            await Task.Run(()=> _waitDetect.WaitOne());
        }

        private void DetectThread()
        {
            while (_waitFrame.WaitOne())
            {
                Frame = Detect();
                
                _waitDetect.Set();
                
                ObjectDetected?.Invoke(this, null);
                
                _waitFrame.Reset();
            }
        }

        private Mat Detect()
        {
            var grayImage = new Mat();

            CvInvoke.CvtColor(_frame, grayImage, ColorConversion.Bgr2Gray);

            var result = CvInvoke.FindChessboardCorners(grayImage, PatterSize, Corners);

            if (result)
                CvInvoke.CornerSubPix(grayImage, Corners, PatterSize, new Size(-1, -1), new MCvTermCriteria(30, 0.1));

            CvInvoke.DrawChessboardCorners(_frame, PatterSize, Corners, result);

            return _frame;
        }
    }
}