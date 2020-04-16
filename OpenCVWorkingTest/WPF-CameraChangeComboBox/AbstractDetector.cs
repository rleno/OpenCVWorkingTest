using Emgu.CV;
using Emgu.CV.Util;
using System.Drawing;

namespace Camera_EMGU_WPFSample
{
    abstract class AbstractDetector
    {
        protected VectorOfPointF Corners;

        public double Fx { get; internal set; }
        public double Fy { get; internal set; }
        public double Fz { get; internal set; }
        public double Dx { get; internal set; }
        public double Dy { get; internal set; }
        public double Dz { get; internal set; }

        abstract public void detect(Mat frame, Size size);
    }
}
