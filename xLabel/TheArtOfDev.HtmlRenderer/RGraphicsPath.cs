using System;

namespace TheArtOfDev.HtmlRenderer.Adapters
{
	public abstract class RGraphicsPath : IDisposable
	{
		public enum Corner
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		public abstract void Start(double x, double y);

		public abstract void LineTo(double x, double y);

		public abstract void ArcTo(double x, double y, double size, Corner corner);

		public abstract void Dispose();
	}
}
