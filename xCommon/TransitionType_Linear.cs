using System;

namespace xCollection
{
    public class TransitionType_Linear : ITransitionType
    {
        private double m_dTransitionTime = 0.0;

        public TransitionType_Linear(int iTransitionTime)
        {
            if (iTransitionTime <= 0)
                throw new Exception("Transition time must be greater than zero.");
            m_dTransitionTime = iTransitionTime;
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            dPercentage = (double)iTime / m_dTransitionTime;
            if (dPercentage >= 1.0)
            {
                dPercentage = 1.0;
                bCompleted = true;
            }
            else
                bCompleted = false;
        }
    }
}