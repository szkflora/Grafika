using Silk.NET.Maths;
using System;

namespace Szeminarium1_24_02_17_2
{
    public enum CameraMode
    {
        Orbital,       // original
        FirstPerson,   // snail view
        TopDownFollow  // above the snail
    }
    internal class CameraDescriptor
    {
        public CameraMode Mode = CameraMode.Orbital;

        private double DistanceToOrigin = 6;

        private double AngleToZYPlane = 0;

        private double AngleToZXPlane = 0;

        private const double DistanceScaleFactor = 1.1;

        private const double AngleChangeStepSize = System.Math.PI / 180 * 5;

        public Vector3D<float> SnailPosition;

        public Vector3D<float> SnailForward = new Vector3D<float>(0, 0, 7);
        public Vector3D<float> SnailBackward = new Vector3D<float>(0.3f, 0, 0);

        public Vector3D<float> Position
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.FirstPerson:
                        return SnailPosition + new Vector3D<float>(0, 0.07f, 0) + SnailForward * 0.15f;// - SnailBackward;// + new Vector3D<float>(0.2f, 0, 0);
                    case CameraMode.TopDownFollow:
                        return SnailPosition + new Vector3D<float>(0, 13f, 1f); 
                    case CameraMode.Orbital:
                    default:
                        return GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane);
                }
            }
        }

        public Vector3D<float> Target
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.FirstPerson:
                        return SnailPosition + new Vector3D<float>(0, 0.07f, 0) + SnailForward;
                    case CameraMode.TopDownFollow:
                        return SnailPosition;
                    case CameraMode.Orbital:
                    default:
                            return Vector3D<float>.Zero;
                }
            }
        }

        public Vector3D<float> UpVector
        {
            get
            {
                switch (Mode)
                {
                    case CameraMode.FirstPerson:
                    case CameraMode.TopDownFollow:
                        return Vector3D<float>.UnitY;
                    case CameraMode.Orbital:
                    default:
                        return Vector3D.Normalize(GetPointFromAngles(DistanceToOrigin, AngleToZYPlane, AngleToZXPlane + System.Math.PI / 2));
                }
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
