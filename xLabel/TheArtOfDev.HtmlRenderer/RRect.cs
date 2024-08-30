using System;

namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public struct RRect
	{
		public static readonly RRect Empty = default(RRect);

		private double _height;

		private double _width;

		private double _x;

		private double _y;

		public RPoint Location
		{
			get
			{
				return new RPoint(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}

		public RSize Size
		{
			get
			{
				return new RSize(Width, Height);
			}
			set
			{
				Width = value.Width;
				Height = value.Height;
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

		public double Width
		{
			get
			{
				return _width;
			}
			set
			{
				_width = value;
			}
		}

		public double Height
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
			}
		}

		public double Left
		{
			get
			{
				return X;
			}
		}

		public double Top
		{
			get
			{
				return Y;
			}
		}

		public double Right
		{
			get
			{
				return X + Width;
			}
		}

		public double Bottom
		{
			get
			{
				return Y + Height;
			}
		}

		public bool IsEmpty
		{
			get
			{
				if (Width > 0.0)
					return Height <= 0.0;
				return true;
			}
		}

		public RRect(double x, double y, double width, double height)
		{
			_x = x;
			_y = y;
			_width = width;
			_height = height;
		}

		public RRect(RPoint location, RSize size)
		{
			_x = location.X;
			_y = location.Y;
			_width = size.Width;
			_height = size.Height;
		}

		public static bool operator ==(RRect left, RRect right)
		{
			if (Math.Abs(left.X - right.X) < 0.001 && Math.Abs(left.Y - right.Y) < 0.001 && Math.Abs(left.Width - right.Width) < 0.001)
				return Math.Abs(left.Height - right.Height) < 0.001;
			return false;
		}

		public static bool operator !=(RRect left, RRect right)
		{
			return !(left == right);
		}

		public static RRect FromLTRB(double left, double top, double right, double bottom)
		{
			return new RRect(left, top, right - left, bottom - top);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RRect))
				return false;
			RRect rRect = (RRect)obj;
			if (Math.Abs(rRect.X - X) < 0.001 && Math.Abs(rRect.Y - Y) < 0.001 && Math.Abs(rRect.Width - Width) < 0.001)
				return Math.Abs(rRect.Height - Height) < 0.001;
			return false;
		}

		public bool Contains(double x, double y)
		{
			if (X <= x && x < X + Width && Y <= y)
				return y < Y + Height;
			return false;
		}

		public bool Contains(RPoint pt)
		{
			return Contains(pt.X, pt.Y);
		}

		public bool Contains(RRect rect)
		{
			if (X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y)
				return rect.Y + rect.Height <= Y + Height;
			return false;
		}

		public void Inflate(double x, double y)
		{
			X -= x;
			Y -= y;
			Width += 2.0 * x;
			Height += 2.0 * y;
		}

		public void Inflate(RSize size)
		{
			Inflate(size.Width, size.Height);
		}

		public static RRect Inflate(RRect rect, double x, double y)
		{
			RRect result = rect;
			result.Inflate(x, y);
			return result;
		}

		public void Intersect(RRect rect)
		{
			RRect rRect = Intersect(rect, this);
			X = rRect.X;
			Y = rRect.Y;
			Width = rRect.Width;
			Height = rRect.Height;
		}

		public static RRect Intersect(RRect a, RRect b)
		{
			double num = Math.Max(a.X, b.X);
			double num2 = Math.Min(a.X + a.Width, b.X + b.Width);
			double num3 = Math.Max(a.Y, b.Y);
			double num4 = Math.Min(a.Y + a.Height, b.Y + b.Height);
			if (num2 >= num && num4 >= num3)
				return new RRect(num, num3, num2 - num, num4 - num3);
			return Empty;
		}

		public bool IntersectsWith(RRect rect)
		{
			if (rect.X < X + Width && X < rect.X + rect.Width && rect.Y < Y + Height)
				return Y < rect.Y + rect.Height;
			return false;
		}

		public static RRect Union(RRect a, RRect b)
		{
			double num = Math.Min(a.X, b.X);
			double num2 = Math.Max(a.X + a.Width, b.X + b.Width);
			double num3 = Math.Min(a.Y, b.Y);
			double num4 = Math.Max(a.Y + a.Height, b.Y + b.Height);
			return new RRect(num, num3, num2 - num, num4 - num3);
		}

		public void Offset(RPoint pos)
		{
			Offset(pos.X, pos.Y);
		}

		public void Offset(double x, double y)
		{
			X += x;
			Y += y;
		}

		public override int GetHashCode()
		{
			return (int)((uint)X ^ (((uint)Y << 13) | ((uint)Y >> 19)) ^ (((uint)Width << 26) | ((uint)Width >> 6)) ^ (((uint)Height << 7) | ((uint)Height >> 25)));
		}

		public override string ToString()
		{
			return "{X=" + X + ",Y=" + Y + ",Width=" + Width + ",Height=" + Height + "}";
		}
	}
}
