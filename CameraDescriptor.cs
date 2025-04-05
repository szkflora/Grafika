using Silk.NET.Maths;
using System;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public double DistanceToOrigin { get; private set; } = 5;

        public double AngleToZYPlane { get; private set; } = 7;

        public double AngleToZXPlane { get; private set; } = 7;

        public float XStep { get; private set; } = 0;
        public float YStep { get; private set; } = 0;

        const double DistanceScaleFactor = 1.1;

        const double AngleChangeStepSize = Math.PI / 180 * 5;

        /// <summary>
        /// Gets the position of the camera.
        /// </summary>
        public Vector3D<float> Position
        {
            get
            {
                return GetPointFromAngles(DistanceToOrigin, AngleToZXPlane, AngleToZYPlane) + new Vector3D<float>(XStep, YStep, 0);
            }
        }

        /// <summary>
        /// Gets the up vector of the camera.
        /// </summary>
        public Vector3D<float> UpVector
        {
            get
            {
                return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZXPlane, AngleToZYPlane + Math.PI / 2));
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
                return new Vector3D<float>(XStep, YStep, 0);
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
        public void MoveRight()
        {
            XStep += 0.5f;
        }

        public void MoveLeft()
        {
            XStep -= 0.5f;
        }

        public void MoveUp()
        {
            YStep += 0.5f;
        }

        public void MoveDown()
        {
            YStep -= 0.5f;
        }
        private Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Sin(angleToMinZYPlane);
            var z = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Cos(angleToMinZYPlane);
            var y = distanceToOrigin * Math.Sin(angleToMinZXPlane);


            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}
