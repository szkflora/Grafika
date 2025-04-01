using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Szeminarium
{
    internal class CubeArrangementModel
    {
        /// <summary>
        /// Gets or sets wheather the animation should run or it should be frozen.
        /// </summary>
        public bool AnimationEnabled { get; set; } = false;
        public bool Forward { get; set; } = true;

        /// <summary>
        /// The time of the simulation. It helps to calculate time dependent values.
        /// </summary>
        private double Time { get; set; } = 0;
        private double progress;
        private const double AnimationTotalTime = 2.0;

        /// <summary>
        /// The value by which the center cube is scaled. It varies between 0.8 and 1.2 with respect to the original size.
        /// </summary>
        public double CenterCubeScale { get; private set; } = 1;

        /// <summary>
        /// The angle with which the diamond cube is rotated around the diagonal from bottom right front to top left back.
        /// </summary>
        public double DiamondCubeLocalAngle { get; private set; } = 0;

        /// <summary>
        /// The angle with which the diamond cube is rotated around the global Y axes.
        /// </summary>
        //public double DiamondCubeGlobalYAngle { get; private set; } = 0;
        public float GlobalRotationX { get; set; } = 0;
        public float RotationsSoFar { get; set; } = 0;
        public float CurrentRatation { get; set; } = 0;

        internal void AdvanceTime(double deltaTime)
        {
            // we do not advance the simulation when animation is stopped
            if (!AnimationEnabled)
                return;

            // set a simulation time
            Time += deltaTime;
            progress = Time / AnimationTotalTime;
            CurrentRatation = Math.Min((float)(progress * Math.PI / 2), (float)(Math.PI / 2));

            if (Forward)
            {
                GlobalRotationX = RotationsSoFar + CurrentRatation;
            }
            else
            {
                GlobalRotationX = RotationsSoFar - CurrentRatation;
            }

            if (progress >= 1)
            {
                AnimationEnabled = false;
                RotationsSoFar = GlobalRotationX;
                Time = 0;
            }

            // lets produce an oscillating scale in time
            //CenterCubeScale = 1 + 0.2 * Math.Sin(1.5 * Time);

                // the rotation angle is time x angular velocity;
                //DiamondCubeLocalAngle = Time * 10;

                //DiamondCubeGlobalYAngle = -Time;
        }
    }
}
