using System;

namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public struct RPoint
	{
		public static readonly RPoint Empty;

		private double _x;

		private double _y;

		public bool IsEmpty
		{
			get
			{
				if (Math.Abs(_x - 0.0) < 0.001)
					return Math.Abs(_y - 0.0) < 0.001;
				return false;
			}
		}

		public double X
		{
			get
			{
				return _x;
			}
			set
			{
				_x = value;
			}
		}

		public double Y
		{
			get
			{
				return _y;
			}
			set
			{
				_y = value;
			}
		}

		static RPoint()
		{
			Empty = default(RPoint);
		}

		public RPoint(double x, double y)
		{
			_x = x;
			_y = y;
		}

		public static RPoint operator +(RPoint pt, RSize sz)
		{
			return Add(pt, sz);
		}

		public static RPoint operator -(RPoint pt, RSize sz)
		{
			return Subtract(pt, sz);
		}

		public static bool operator ==(RPoint left, RPoint right)
		{
			if (left.X == right.X)
				return left.Y == right.Y;
			return false;
		}

		public static bool operator !=(RPoint left, RPoint right)
		{
			return !(left == right);
		}

		public static RPoint Add(RPoint pt, RSize sz)
		{
			return new RPoint(pt.X + sz.Width, pt.Y + sz.Height);
		}

		public static RPoint Subtract(RPoint pt, RSize sz)
		{
			return new RPoint(pt.X - sz.Width, pt.Y - sz.Height);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RPoint))
				return false;
			RPoint rPoint = (RPoint)obj;
			if (rPoint.X == X && rPoint.Y == Y)
				return rPoint.GetType().Equals(GetType());
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("{{X={0}, Y={1}}}", new object[2] { _x, _y });
		}
	}
}
