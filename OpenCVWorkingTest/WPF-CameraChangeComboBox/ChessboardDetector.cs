using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Camera_EMGU_WPFSample
{
    class ChessboardDetector : AbstractDetector
    {
        public override void detect(Mat frame, Size size)
        {
            var res = _detect(frame);

            if (res)
                Calculate(frame, size);
        }

        private bool _detect(Mat frame)
        {
            var gray_image = new Mat();

            CvInvoke.CvtColor(frame, gray_image, ColorConversion.Bgr2Gray);

            var patternSize = new System.Drawing.Size(6, 9);
            Corners = new VectorOfPointF();

            bool result = CvInvoke.FindChessboardCorners(gray_image, patternSize, Corners);

            if (result)
            {
                CvInvoke.CornerSubPix(gray_image, Corners, patternSize, new System.Drawing.Size(-1, -1), new MCvTermCriteria(30, 0.1));
                CvInvoke.DrawChessboardCorners(frame, patternSize, Corners, result);
            }

            return result;
        }

        private void Calculate(Mat frame, Size size)
        {
            Mat cameraMatrix = new Mat(3, 3, DepthType.Cv64F, 1);
            Mat distCoeffs = new Mat(8, 1, DepthType.Cv64F, 1);

            var objectList = new List<MCvPoint3D32f>();

            Mat rvecs = new Mat(3, 1, DepthType.Cv64F, 1),
                tvecs = new Mat(3, 1, DepthType.Cv64F, 1);

            var _squareSize = 20.0f;


            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                    objectList.Add(new MCvPoint3D32f(j * _squareSize, i * _squareSize, 0.0F));

            var cMat = new double[,]
            {
                {1.0017554539927537e+03,    .0,                         5.3714642465820737e+02},
                {.0,                        1.0017554539927537e+03,     3.0522634021232267e+02},
                {.0,                        .0,                         1.0}
            };

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    cameraMatrix.SetValue(r, c, cMat[r, c]);

            for (int i = 0; i < 8; i++)
                distCoeffs.SetValue(i, 0, 0);

            distCoeffs.SetValue(0, 0, -9.9898495516849192e-02);
            distCoeffs.SetValue(1, 0, 2.2626694964843341e-01);
            distCoeffs.SetValue(2, 0, -1.1544241181928370e-02);
            distCoeffs.SetValue(3, 0, -2.6699808935566056e-03);
            distCoeffs.SetValue(4, 0, -1.9438362201403153e-01);


            MCvPoint3D32f[][] _cornersObjectList = new MCvPoint3D32f[1][];
            _cornersObjectList[0] = objectList.ToArray();
            PointF[][] _cornersPointsList = new PointF[1][];
            _cornersPointsList[0] = Corners.ToArray();

            CvInvoke.SolvePnP(_cornersObjectList[0], _cornersPointsList[0], cameraMatrix, distCoeffs, rvecs, tvecs);

            var rmat = new Mat(3, 3, DepthType.Cv32F, 1);
            CvInvoke.Rodrigues(rvecs, rmat);

            Fx = Math.Atan2(rmat.GetValue(2, 1), rmat.GetValue(2, 2)) * 57.295779513;
            Fy = Math.Atan2(-rmat.GetValue(2, 0), Math.Sqrt(rmat.GetValue(2, 1) * rmat.GetValue(2, 1) + rmat.GetValue(2, 2) * rmat.GetValue(2, 2))) * 57.295779513;
            Fz = Math.Atan2(rmat.GetValue(1, 0), rmat.GetValue(0, 0)) * 57.295779513;

            Dx = tvecs.GetValue(0, 0);
            Dy = tvecs.GetValue(0, 1);
            Dx = tvecs.GetValue(0, 2);
        }
    }
}
