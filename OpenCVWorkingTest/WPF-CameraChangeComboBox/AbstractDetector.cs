using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using System;
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

        public Mat cameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
        public Mat distCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);

        public Mat rvecs = new Mat(3, 1, DepthType.Cv64F, 1);
        public Mat tvecs = new Mat(3, 1, DepthType.Cv64F, 1);

        abstract public void detect(Mat frame, Size size);

        protected void FillData()
        {
            var rmat = new Mat(3, 3, DepthType.Cv32F, 1);
            CvInvoke.Rodrigues(rvecs, rmat);

            Fx = Math.Atan2(rmat.GetValue(2, 1), rmat.GetValue(2, 2)) * 57.295779513;
            Fy = Math.Atan2(-rmat.GetValue(2, 0), Math.Sqrt(rmat.GetValue(2, 1) * rmat.GetValue(2, 1) + rmat.GetValue(2, 2) * rmat.GetValue(2, 2))) * 57.295779513;
            Fz = Math.Atan2(rmat.GetValue(1, 0), rmat.GetValue(0, 0)) * 57.295779513;

            Dx = tvecs.GetValue(0, 0);
            Dy = tvecs.GetValue(0, 1);
            Dz = tvecs.GetValue(0, 2);
        }
    }
}
