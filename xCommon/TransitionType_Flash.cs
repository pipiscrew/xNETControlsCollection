using System.Collections.Generic;

namespace xCollection
{
    public class TransitionType_Flash : TransitionType_UserDefined
    {
        public TransitionType_Flash(int iNumberOfFlashes, int iFlashTime)
        {
            double num = 100.0 / (double)iNumberOfFlashes;
            IList<TransitionElement> list = new List<TransitionElement>();
            for (int i = 0; i < iNumberOfFlashes; i++)
            {
                double num2 = (double)i * num;
                double num3 = num2 + num;
                double endTime = (num2 + num3) / 2.0;
                list.Add(new TransitionElement(endTime, 100.0, InterpolationMethod.EaseInEaseOut));
                list.Add(new TransitionElement(num3, 0.0, InterpolationMethod.EaseInEaseOut));
            }
            setup(list, iFlashTime * iNumberOfFlashes);
        }
    }
}