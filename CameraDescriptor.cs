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

        public bool RotateInPlace { get; set; } = false;

        /// <summary>
        /// Y axes rotation
        /// </summary>
        public double Yaw { get; private set; } = Math.PI;

        /// <summary>
        /// X axes rotation
        /// </summary>
        public double Pitch { get; private set; } = -Math.PI / 4;

        private const double RotationStep = Math.PI / 180 * 5;

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
                if (!RotateInPlace)
                {
                    return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZXPlane, AngleToZYPlane + Math.PI / 2));
                }

                /// Approximation: Y-up world
                return new Vector3D<float>(0, 1, 0);
            }
        }

        /// <summary>
        /// Gets the target point of the camera view.
        /// </summary>
        public Vector3D<float> Target
        {
            get
            {
                if (!RotateInPlace)
                {
                    return new Vector3D<float>(XStep, YStep, 0);
                }

                /// rotate in place
                var x = (float)(Math.Cos(Pitch) * Math.Sin(Yaw));
                var y = (float)(Math.Sin(Pitch));
                var z = (float)(Math.Cos(Pitch) * Math.Cos(Yaw));

                var direction = new Vector3D<float>(x, y, z);
                return Position + direction;
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

        public void RotateLeft()
        {
            Yaw -= RotationStep;
        }

        public void RotateRight()
        {
            Yaw += RotationStep;
        }

        public void RotateUp()
        {
            Pitch += RotationStep;
            Pitch = Math.Max(-Math.PI / 2, Math.Min(Pitch, Math.PI / 2));
        }

        public void RotateDown()
        {
            Pitch -= RotationStep;
            Pitch = Math.Max(-Math.PI / 2, Math.Min(Pitch, Math.PI / 2));
        }
        private Vector3D<float> GetPointFromAngles(double distanceToOrigin, double angleToMinZYPlane, double angleToMinZXPlane)
        {
            var x = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Sin(angleToMinZYPlane);
            var y = distanceToOrigin * Math.Sin(angleToMinZXPlane);
            var z = distanceToOrigin * Math.Cos(angleToMinZXPlane) * Math.Cos(angleToMinZYPlane);

            return new Vector3D<float>((float)x, (float)y, (float)z);
        }
    }
}
