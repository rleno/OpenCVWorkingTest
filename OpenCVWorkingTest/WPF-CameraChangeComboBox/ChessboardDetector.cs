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
            var objectList = new List<MCvPoint3D32f>();

            var _squareSize = 20.0f;

            for (int i = 0; i < size.Height; i++)
                for (int j = 0; j < size.Width; j++)
                    objectList.Add(new MCvPoint3D32f(j * _squareSize, i * _squareSize, 0.0F));

            MCvPoint3D32f[][] _cornersObjectList = new MCvPoint3D32f[1][];
            _cornersObjectList[0] = objectList.ToArray();
            PointF[][] _cornersPointsList = new PointF[1][];
            _cornersPointsList[0] = Corners.ToArray();

            CvInvoke.SolvePnP(_cornersObjectList[0], _cornersPointsList[0], cameraMatrix, distCoeffs, rvecs, tvecs);

            FillData();
        }
    }
}
