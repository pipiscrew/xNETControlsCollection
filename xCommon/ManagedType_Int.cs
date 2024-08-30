using System;

namespace xCollection
{
    internal class ManagedType_Int : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(int);
        }

        public object copy(object o)
        {
            int num = (int)o;
            return num;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            int i = (int)start;
            int i2 = (int)end;
            return Utility.interpolate(i, i2, dPercentage);
        }
    }
}