using System;
using System.Drawing;
using System.Windows.Forms;

namespace Utilities.xProgressBar
{
	internal class xColorTransition
	{
		private int _ProgessValue;

		private Color _color1;

		private Color _color2;

		private Color _value;

		public int ProgessValue
		{
			get
			{
				return _ProgessValue;
			}
			set
			{
				_ProgessValue = value;
				Refresh();
			}
		}

		public Color Value
		{
			get
			{
				return _value;
			}
		}

		public Color Color1
		{
			get
			{
				return _color1;
			}
			set
			{
				_color1 = value;
				Refresh();
			}
		}

		public Color Color2
		{
			get
			{
				return _color2;
			}
			set
			{
				_color2 = value;
				Refresh();
			}
		}

		public void Refresh()
		{
			Color colorScale = GetColorScale(ProgessValue, Color1, Color2);
			if (colorScale != Value)
				_value = colorScale;
		}

		public static Bitmap ApplyGradient(Control Target, Color startColor, Color endColor, int GradientStep = 1)
		{
			Bitmap bitmap = new Bitmap(Target.Width, Target.Height);
			for (int i = 0; i < bitmap.Width; i += GradientStep)
			{
				int passentage = (int)Math.Round((double)i / (double)bitmap.Width * 100.0, 0);
				Color colorScale = GetColorScale(passentage, startColor, endColor);
				for (int j = 0; j < bitmap.Height; j++)
				{
					bitmap.SetPixel(i, j, colorScale);
				}
			}
			return bitmap;
		}

		public static Image ApplyGradient(Size Target, Color startColor, Color endColor, int GradientStep = 1)
		{
			Bitmap bitmap = new Bitmap(Target.Width, Target.Height);
			for (int i = 0; i < bitmap.Width; i += GradientStep)
			{
				int passentage = (int)Math.Round((double)i / (double)bitmap.Width * 100.0, 0);
				Color colorScale = GetColorScale(passentage, startColor, endColor);
				for (int j = 0; j < bitmap.Height; j++)
				{
					bitmap.SetPixel(i, j, colorScale);
				}
			}
			return bitmap;
		}

		public static Color GetColorScale(int Passentage, Color startColor, Color endColor)
		{
			try
			{
				double value = (double)(int)startColor.R + (double)((endColor.R - startColor.R) * Passentage) * 0.01;
				int red = int.Parse(Math.Round(value, 0).ToString());
				double value2 = (double)(int)startColor.G + (double)((endColor.G - startColor.G) * Passentage) * 0.01;
				int green = int.Parse(Math.Round(value2, 0).ToString());
				double value3 = (double)(int)startColor.B + (double)((endColor.B - startColor.B) * Passentage) * 0.01;
				int blue = int.Parse(Math.Round(value3, 0).ToString());
				return Color.FromArgb(255, red, green, blue);
			}
			catch (Exception)
			{
				return startColor;
			}
		}
	}
}
