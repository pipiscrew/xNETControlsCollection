using System;
using System.Reflection;

namespace xCollection
{
    public static class ReflectionHelper
    {
        public static object GetPropValue(this object obj, string propName)
        {
            string[] array = propName.Split('.');
            if (array.Length != 1)
            {
                string[] array2 = array;
                int num = 0;
                while (true)
                {
                    if (num < array2.Length)
                    {
                        string name = array2[num];
                        if (obj == null)
                            break;
                        Type type = obj.GetType();
                        PropertyInfo property = type.GetProperty(name);
                        if (!(property == null))
                        {
                            obj = property.GetValue(obj, null);
                            num++;
                            continue;
                        }
                        return null;
                    }
                    return obj;
                }
                return null;
            }
            return obj.GetType().GetProperty(propName).GetValue(obj, null);
        }
    }
}