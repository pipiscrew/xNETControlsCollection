namespace xCollection
{
    public class TransitionElement
    {
        public double EndTime { get; set; }

        public double EndValue { get; set; }

        public InterpolationMethod InterpolationMethod { get; set; }

        public TransitionElement(double endTime, double endValue, InterpolationMethod interpolationMethod)
        {
            EndTime = endTime;
            EndValue = endValue;
            InterpolationMethod = interpolationMethod;
        }
    }
}
