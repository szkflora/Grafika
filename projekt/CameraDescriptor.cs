using Silk.NET.Maths;
using System;

namespace Szeminarium1_24_02_17_2
{
    internal class CameraDescriptor
    {
        private double DistanceToOrigin = 4;

        private double AngleToZYPlane = 0;

        private double AngleToZXPlane = 0;

        private const double DistanceScaleFactor = 1.1;

        private const double AngleChangeStepSize = System.Math.PI / 180 * 5;

        private Vector3D<float> manualPosition;
        private bool useManualPosition = false;
        private Vector3D<float> manualTarget;
        private bool useManualTarget = false;

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        public Vector3D<float> Position
        {
            get
            {
                //if (useManualPosition)
                //    return manualPosition;
                //else
                    return GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane);
            }
            //set
            //{
            //    manualPosition = value;
            //    useManualPosition = true;
            //}
        }

        /// <summary>
        /// Gets the up vector of the camera.
        /// </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                    return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + System.Math.PI / 2));
            }
        }

        /// <summary>
        /// Gets the target point of the camera view.
        /// </summary>
        public Vector3D<float> Target
        {
            get
            {
                // For the moment the camera is always pointed at the origin.
                if (useManualTarget)
                    return manualTarget;
                else
                    return Vector3D<float>.Zero;
            }
            set
            {
                manualTarget = value;
                useManualTarget = true;
            }
        }

        public void IncreaseZXAngle()
        {
            AngleToZXPlane += AngleChangeStepSize;
        }

        public void DecreaseZXAngle()
        {
            AngleToZXPlane -= AngleChangeStepSize;
        }

        public void IncreaseZYAngle()
        {
            AngleToZYPlane += AngleChangeStepSize;

        }

        public void DecreaseZYAngle()
        {
            AngleToZYPlane -= AngleChangeStepSize;
        }

        public void IncreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin * DistanceScaleFactor;
        }

        public void DecreaseDistance()
        {
            DistanceToOrigin = DistanceToOrigin / DistanceScaleFactor;
        }

        private static Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * System.Math.Cos(angleToMinZXPlane) * System.Math.Sin(angleToMinZYPlane);
            var z = distanceToOrigin * System.Math.Cos(angleToMinZXPlane) * System.Math.Cos(angleToMinZYPlane);
            var y = distanceToOrigin * System.Math.Sin(angleToMinZXPlane);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}
