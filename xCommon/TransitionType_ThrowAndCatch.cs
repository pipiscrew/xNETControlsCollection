using System.Collections.Generic;

namespace xCollection
{
    public class TransitionType_ThrowAndCatch : TransitionType_UserDefined
    {
        public TransitionType_ThrowAndCatch(int iTransitionTime)
        {
            setup(new List<TransitionElement>
			{
				new TransitionElement(50.0, 100.0, InterpolationMethod.Deceleration),
				new TransitionElement(100.0, 0.0, InterpolationMethod.Accleration)
			}, iTransitionTime);
        }
    }
}