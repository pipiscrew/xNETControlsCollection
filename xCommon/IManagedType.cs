using System;

namespace xCollection
{
    internal interface IManagedType
    {
        Type getManagedType();

        object copy(object o);

        object getIntermediateValue(object start, object end, double dPercentage);
    }
}