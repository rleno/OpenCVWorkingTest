using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Util;

namespace OpenCVWorkingTest.Detectors
{
    public abstract class Detector
    {
        public Mat Frame { get; private set; }
        public VectorOfPointF Corners { get; }

        public event EventHandler ObjectDetected;

        private Thread _detectThread;
        private readonly EventWaitHandle _waitFrame;
        private readonly EventWaitHandle _waitDetect;

        private static int _detectThreadCount = 0;

        private readonly Stopwatch _stopwatch;


        protected abstract Mat Detect();

        protected Detector()
        {
            Corners = new VectorOfPointF();

            _waitFrame = new EventWaitHandle(false, EventResetMode.ManualReset);
            _waitDetect = new EventWaitHandle(false, EventResetMode.AutoReset);

            StartDetectThread();

            _stopwatch = new Stopwatch();
        }

        public async Task Detect(Mat frame)
        {
            if (_waitFrame.WaitOne(0)) 
                return;
            
            Frame = frame;

            _waitFrame.Set();

            await Task.Run(()=> _waitDetect.WaitOne());
        }

        private void StartDetectThread()
        {
            _detectThread = new Thread(DetectThread)
            {
                Name = "DetectThread (" + _detectThreadCount++ + ")",
                Priority = ThreadPriority.Highest
            };

            _detectThread.Start();
        }

        private void DetectThread()
        {
            while (_waitFrame.WaitOne())
            {
                Detect();
                
                ObjectDetected?.Invoke(this, null);

                _waitDetect.Set();
                _waitFrame.Reset();
            }
        }

        public void Terminate()
        {
            _stopwatch.Start();

            _detectThread.Priority = ThreadPriority.Lowest;
            _detectThread.Abort();
            _waitDetect.Set();

            //            _detectThread.Join();

            _stopwatch.Stop();
            Console.WriteLine(": " + _stopwatch.ElapsedMilliseconds);
            _stopwatch.Reset();

        }
    }
}