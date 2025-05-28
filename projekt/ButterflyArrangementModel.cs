namespace Szeminarium1_24_02_17_2
{
    internal class ButterflyArrangementModel
    {
        /// <summary>
        /// Gets or sets wheather the animation should run or it should be frozen.
        /// </summary>
        public bool AnimationEnabeld { get; set; } = true;

        /// <summary>
        /// The time of the simulation. It helps to calculate time dependent values.
        /// </summary>
        private double Time { get; set; } = 0;

        /// <summary>
        /// The value by which the center cube is scaled. It varies between 0.8 and 1.2 with respect to the original size.
        /// </summary>
        public double Scale { get; private set; } = 1;


        /// <summary>
        /// The angle with which the diamond cube is rotated around the diagonal from bottom right front to top left back.
        /// </summary>
        public double AngleRevolutionOnGlobalY { get; private set; } = 0;

        internal void AdvanceTime(double deltaTime)
        {
            // we do not advance the simulation when animation is stopped
            if (!AnimationEnabeld)
                return;

            // set a simulation time
            Time += deltaTime;

            // lets produce an oscillating scale in time
            Scale = 1 + 0.2 * System.Math.Sin(1.5 * Time);

            AngleRevolutionOnGlobalY = -Time;
        }
    }
}
