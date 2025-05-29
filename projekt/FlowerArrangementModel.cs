namespace Szeminarium1_24_02_17_2
{
    internal class FlowerArrangementModel
    {
        /// <summary>
        /// Gets or sets wheather the animation should run or it should be frozen.
        /// </summary>
        public bool AnimationEnabeld { get; set; } = false;

        public double Scale { get; private set; } = 1;
    }

}
