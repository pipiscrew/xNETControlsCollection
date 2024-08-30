using System;
using System.Drawing.Drawing2D;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class GraphicsPathAdapter : RGraphicsPath
	{
		private readonly GraphicsPath _graphicsPath = new GraphicsPath();

		private RPoint _lastPoint;

		public GraphicsPath GraphicsPath
		{
			get
			{
				return _graphicsPath;
			}
		}

		public override void Start(double x, double y)
		{
			_lastPoint = new RPoint(x, y);
		}

		public override void LineTo(double x, double y)
		{
			_graphicsPath.AddLine((float)_lastPoint.X, (float)_lastPoint.Y, (float)x, (float)y);
			_lastPoint = new RPoint(x, y);
		}

		public override void ArcTo(double x, double y, double size, Corner corner)
		{
			float x2 = (float)(Math.Min(x, _lastPoint.X) - ((corner == Corner.TopRight || corner == Corner.BottomRight) ? size : 0.0));
			float y2 = (float)(Math.Min(y, _lastPoint.Y) - ((corner == Corner.BottomLeft || corner == Corner.BottomRight) ? size : 0.0));
			_graphicsPath.AddArc(x2, y2, (float)size * 2f, (float)size * 2f, GetStartAngle(corner), 90f);
			_lastPoint = new RPoint(x, y);
		}

		public override void Dispose()
		{
			_graphicsPath.Dispose();
		}

		private static int GetStartAngle(Corner corner)
		{
			switch (corner)
			{
			default:
				throw new ArgumentOutOfRangeException("corner");
			case Corner.TopLeft:
				return 180;
			case Corner.TopRight:
				return 270;
			case Corner.BottomLeft:
				return 90;
			case Corner.BottomRight:
				return 0;
			}
		}
	}
}
