using System;
using System.Collections.Generic;

namespace xCollection
{
    public class TransitionType_UserDefined : ITransitionType
    {
        private IList<TransitionElement> m_Elements = null;

        private double m_dTransitionTime = 0.0;

        private int m_iCurrentElement = 0;

        public TransitionType_UserDefined()
        {
        }

        public TransitionType_UserDefined(IList<TransitionElement> elements, int iTransitionTime)
        {
            setup(elements, iTransitionTime);
        }

        public void setup(IList<TransitionElement> elements, int iTransitionTime)
        {
            m_Elements = elements;
            m_dTransitionTime = iTransitionTime;
            if (elements.Count == 0)
                throw new Exception("The list of elements passed to the constructor of TransitionType_UserDefined had zero elements. It must have at least one element.");
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            double num = (double)iTime / m_dTransitionTime;
            double dStartTime;
            double dEndTime;
            double dStartValue;
            double dEndValue;
            InterpolationMethod eInterpolationMethod;
            getElementInfo(num, out dStartTime, out dEndTime, out dStartValue, out dEndValue, out eInterpolationMethod);
            double num2 = dEndTime - dStartTime;
            double num3 = num - dStartTime;
            double num4 = num3 / num2;
            double dPercentage2;
            switch (eInterpolationMethod)
            {
                default:
                    throw new Exception("Interpolation method not handled: " + eInterpolationMethod);
                case InterpolationMethod.Linear:
                    dPercentage2 = num4;
                    break;
                case InterpolationMethod.Accleration:
                    dPercentage2 = Utility.convertLinearToAcceleration(num4);
                    break;
                case InterpolationMethod.Deceleration:
                    dPercentage2 = Utility.convertLinearToDeceleration(num4);
                    break;
                case InterpolationMethod.EaseInEaseOut:
                    dPercentage2 = Utility.convertLinearToEaseInEaseOut(num4);
                    break;
            }
            dPercentage = Utility.interpolate(dStartValue, dEndValue, dPercentage2);
            if ((double)iTime >= m_dTransitionTime)
            {
                bCompleted = true;
                dPercentage = dEndValue;
            }
            else
                bCompleted = false;
        }

        private void getElementInfo(double dTimeFraction, out double dStartTime, out double dEndTime, out double dStartValue, out double dEndValue, out InterpolationMethod eInterpolationMethod)
        {
            int count = m_Elements.Count;
            while (m_iCurrentElement < count)
            {
                TransitionElement transitionElement = m_Elements[m_iCurrentElement];
                double num = transitionElement.EndTime / 100.0;
                if (dTimeFraction < num)
                    break;
                m_iCurrentElement++;
            }
            if (m_iCurrentElement == count)
                m_iCurrentElement = count - 1;
            dStartTime = 0.0;
            dStartValue = 0.0;
            if (m_iCurrentElement > 0)
            {
                TransitionElement transitionElement2 = m_Elements[m_iCurrentElement - 1];
                dStartTime = transitionElement2.EndTime / 100.0;
                dStartValue = transitionElement2.EndValue / 100.0;
            }
            TransitionElement transitionElement3 = m_Elements[m_iCurrentElement];
            dEndTime = transitionElement3.EndTime / 100.0;
            dEndValue = transitionElement3.EndValue / 100.0;
            eInterpolationMethod = transitionElement3.InterpolationMethod;
        }
    }
}