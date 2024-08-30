using System.Collections.Generic;

namespace xCollection
{
    public class TransitionType_Bounce : TransitionType_UserDefined
    {
        public TransitionType_Bounce(int iTransitionTime)
        {
            setup(new List<TransitionElement>
			{
				new TransitionElement(50.0, 100.0, InterpolationMethod.Accleration),
				new TransitionElement(100.0, 0.0, InterpolationMethod.Deceleration)
			}, iTransitionTime);
        }
    }
}