using System;

namespace xCollection
{
    internal class ManagedType_String : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(string);
        }

        public object copy(object o)
        {
            string text = (string)o;
            return new string(text.ToCharArray());
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            string text = (string)start;
            string text2 = (string)end;
            int length = text.Length;
            int length2 = text2.Length;
            int num = Utility.interpolate(length, length2, dPercentage);
            char[] array = new char[num];
            for (int i = 0; i < num; i++)
            {
                char value = 'a';
                if (i < length)
                    value = text[i];
                char c = 'a';
                if (i < length2)
                    c = text2[i];
                char c2;
                if (c == ' ')
                    c2 = ' ';
                else
                {
                    int i2 = Convert.ToInt32(value);
                    int i3 = Convert.ToInt32(c);
                    int value2 = Utility.interpolate(i2, i3, dPercentage);
                    c2 = Convert.ToChar(value2);
                }
                array[i] = c2;
            }
            return new string(array);
        }
    }
}