using System;

namespace xCollection
{
    internal class ManagedType_Double : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(double);
        }

        public object copy(object o)
        {
            double num = (double)o;
            return num;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            double d = (double)start;
            double d2 = (double)end;
            return Utility.interpolate(d, d2, dPercentage);
        }
    }
}