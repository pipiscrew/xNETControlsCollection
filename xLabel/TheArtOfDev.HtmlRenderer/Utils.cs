using System;
using System.Drawing;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Utilities
{
	internal static class Utils
	{
		public static RPoint Convert(PointF p)
		{
			return new RPoint(p.X, p.Y);
		}

		public static PointF[] Convert(RPoint[] points)
		{
			PointF[] array = new PointF[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				array[i] = Convert(points[i]);
			}
			return array;
		}

		public static PointF Convert(RPoint p)
		{
			return new PointF((float)p.X, (float)p.Y);
		}

		public static Point ConvertRound(RPoint p)
		{
			return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
		}

		public static RSize Convert(SizeF s)
		{
			return new RSize(s.Width, s.Height);
		}

		public static SizeF Convert(RSize s)
		{
			return new SizeF((float)s.Width, (float)s.Height);
		}

		public static Size ConvertRound(RSize s)
		{
			return new Size((int)Math.Round(s.Width), (int)Math.Round(s.Height));
		}

		public static RRect Convert(RectangleF r)
		{
			return new RRect(r.X, r.Y, r.Width, r.Height);
		}

		public static RectangleF Convert(RRect r)
		{
			return new RectangleF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
		}

		public static Rectangle ConvertRound(RRect r)
		{
			return new Rectangle((int)Math.Round(r.X), (int)Math.Round(r.Y), (int)Math.Round(r.Width), (int)Math.Round(r.Height));
		}

		public static RColor Convert(Color c)
		{
			return RColor.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static Color Convert(RColor c)
		{
			return Color.FromArgb(c.A, c.R, c.G, c.B);
		}

		public static Graphics CreateGraphics(Control control)
		{
			return control.CreateGraphics();
		}
	}
}
