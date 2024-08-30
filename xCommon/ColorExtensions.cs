using System.Drawing;

namespace xCollection
{
    public static class ColorExtensions
    {
        public static Color LightenBy(this Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)((double)percent / 100.0));
        }

        public static Color DarkenBy(this Color color, int percent)
        {
            return ChangeColorBrightness(color, (float)((double)(-1 * percent) / 100.0));
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float num = (int)color.R;
            float num2 = (int)color.G;
            float num3 = (int)color.B;
            if (correctionFactor < 0f)
            {
                correctionFactor = 1f + correctionFactor;
                num *= correctionFactor;
                num2 *= correctionFactor;
                num3 *= correctionFactor;
            }
            else
            {
                num = (255f - num) * correctionFactor + num;
                num2 = (255f - num2) * correctionFactor + num2;
                num3 = (255f - num3) * correctionFactor + num3;
            }
            return Color.FromArgb(color.A, (int)num, (int)num2, (int)num3);
        }
    }
}