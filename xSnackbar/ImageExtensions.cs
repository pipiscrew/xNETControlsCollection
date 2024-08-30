using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace Utilities.xSnackbar
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[DebuggerStepThrough]
	internal static class ImageExtensions
	{
		public enum FlipOrientation
		{
			Normal,
			Horizontal,
			Vertical
		}

		public static void Rotate(this Graphics graphics, double rotation, int width, int height)
		{
			if (!(Math.Abs(rotation) < 0.5))
			{
				float num = 0.5f * (float)width;
				float num2 = 0.5f * (float)height;
				graphics.TranslateTransform(num, num2);
				graphics.RotateTransform((float)rotation);
				graphics.TranslateTransform(0f - num, 0f - num2);
			}
		}

		public static void Flip(this Graphics graphics, FlipOrientation flip, int width, int height)
		{
			switch (flip)
			{
			case FlipOrientation.Vertical:
				graphics.ScaleTransform(1f, -1f);
				graphics.TranslateTransform(0f, -height);
				break;
			case FlipOrientation.Horizontal:
				graphics.ScaleTransform(-1f, 1f);
				graphics.TranslateTransform(-width, 0f);
				break;
			}
		}

		public static void Flip(this Image image, FlipOrientation flip)
		{
			RotateFlipType rotateFlipType = flip.ToRotateFlip();
			if (rotateFlipType != 0)
				image.RotateFlip(rotateFlipType);
		}

		private static RotateFlipType ToRotateFlip(this FlipOrientation flip)
		{
			switch (flip)
			{
			default:
				return RotateFlipType.RotateNoneFlipNone;
			case FlipOrientation.Vertical:
				return RotateFlipType.Rotate180FlipX;
			case FlipOrientation.Horizontal:
				return RotateFlipType.RotateNoneFlipX;
			}
		}
	}
}
