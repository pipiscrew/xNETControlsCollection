using System;
using System.Drawing;

namespace xCollection
{
    internal class ManagedType_Color : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(Color);
        }

        public object copy(object o)
        {
            Color color = Color.FromArgb(((Color)o).ToArgb());
            return color;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            Color color = (Color)start;
            Color color2 = (Color)end;
            int r = color.R;
            int g = color.G;
            int b = color.B;
            int a = color.A;
            int r2 = color2.R;
            int g2 = color2.G;
            int b2 = color2.B;
            int a2 = color2.A;
            int red = Utility.interpolate(r, r2, dPercentage);
            int green = Utility.interpolate(g, g2, dPercentage);
            int blue = Utility.interpolate(b, b2, dPercentage);
            int alpha = Utility.interpolate(a, a2, dPercentage);
            return Color.FromArgb(alpha, red, green, blue);
        }
    }
}