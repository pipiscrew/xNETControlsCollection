using System.Drawing;
using System.Drawing.Drawing2D;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class PenAdapter : RPen
	{
		private readonly Pen _pen;

		public Pen Pen
		{
			get
			{
				return _pen;
			}
		}

		public override double Width
		{
			get
			{
				return _pen.Width;
			}
			set
			{
				_pen.Width = (float)value;
			}
		}

		public override RDashStyle DashStyle
		{
			set
			{
				switch (value)
				{
				default:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
					break;
				case RDashStyle.Solid:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
					break;
				case RDashStyle.Dash:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
					if (Width < 2.0)
						_pen.DashPattern = new float[2] { 4f, 4f };
					break;
				case RDashStyle.Dot:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
					break;
				case RDashStyle.DashDot:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;
					break;
				case RDashStyle.DashDotDot:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot;
					break;
				case RDashStyle.Custom:
					_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Custom;
					break;
				}
			}
		}

		public PenAdapter(Pen pen)
		{
			_pen = pen;
		}
	}
}
