using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Drawing;

using Emgu.CV.Aruco;

namespace Camera_EMGU_WPFSample
{    class ArucoDetector : AbstractDetector
    {
        public Dictionary ARUCO_MIP_25h7 { get; private set; }

        public override void detect(Mat frame, Size size)
        {
            Mat m = new Mat();


            VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF();
            VectorOfVectorOfPointF rejectedimgpoints = new VectorOfVectorOfPointF();
            VectorOfInt ids = new Emgu.CV.Util.VectorOfInt(new int[] { 1 });
            Matrix<double> dsC = new Matrix<double>(1, 4);
            Matrix<double> calib = new Matrix<double>(3, 3);

            DetectorParameters parameters = DetectorParameters.GetDefault(); //default detector parameters
            float markersize = 4.4f;                                          //size of the aruco markers
            MCvScalar color = new MCvScalar(255, 0, 0);     //color to draw the markers in

            //detect markers
            

            ARUCO_MIP_25h7 = new Dictionary(Dictionary.PredefinedDictionaryName.Dict6X6_250);

            Mat markerImage = new Mat();

            //ArucoInvoke.KC

            //ArucoInvoke.DrawMarker(ARUCO_MIP_25h7, 1, 250, frame);

            m = frame;
            ArucoInvoke.DetectMarkers(m, ARUCO_MIP_25h7, corners, ids, parameters, rejectedimgpoints);

            
            //check if anything was detected - if they were, do stuff
            if (ids.Size != 0)
            {
                //draw the markers in the image
                ArucoInvoke.DrawDetectedMarkers(m, corners, ids, color);

                //make the distortion matrix
                dsC[0, 0] = -0.175286;
                dsC[0, 1] = 0.0287032;
                dsC[0, 2] = 0;
                dsC[0, 3] = 0;

                //make the camera matrix
                calib[0, 0] = 1398.38;
                calib[0, 1] = 0;
                calib[0, 2] = 1082.4;
                calib[1, 0] = 0;
                calib[1, 1] = 1398.38;
                calib[1, 2] = 627.885;
                calib[2, 0] = 0;
                calib[2, 1] = 0;
                calib[2, 2] = 1;

                //estimate the pose using the marker info and the camera properties
                ArucoInvoke.EstimatePoseSingleMarkers(corners, markersize, cameraMatrix, distCoeffs, rvecs, tvecs);

                //draw the axis on the image
                ArucoInvoke.DrawAxis(m, calib, dsC, rvecs, tvecs, markersize);

                FillData();
            }
        }
    }

}
