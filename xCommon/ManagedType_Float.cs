using System;

namespace xCollection
{
    internal class ManagedType_Float : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(float);
        }

        public object copy(object o)
        {
            float num = (float)o;
            return num;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            float f = (float)start;
            float f2 = (float)end;
            return Utility.interpolate(f, f2, dPercentage);
        }
    }
}