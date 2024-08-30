using System;

namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public struct RSize
	{
		public static readonly RSize Empty = default(RSize);

		private double _height;

		private double _width;

		public bool IsEmpty
		{
			get
			{
				if (Math.Abs(_width) < 0.0001)
					return Math.Abs(_height) < 0.0001;
				return false;
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

		public RSize(RSize size)
		{
			_width = size._width;
			_height = size._height;
		}

		public RSize(RPoint pt)
		{
			_width = pt.X;
			_height = pt.Y;
		}

		public RSize(double width, double height)
		{
			_width = width;
			_height = height;
		}

		public static explicit operator RPoint(RSize size)
		{
			return new RPoint(size.Width, size.Height);
		}

		public static RSize operator +(RSize sz1, RSize sz2)
		{
			return Add(sz1, sz2);
		}

		public static RSize operator -(RSize sz1, RSize sz2)
		{
			return Subtract(sz1, sz2);
		}

		public static bool operator ==(RSize sz1, RSize sz2)
		{
			if (Math.Abs(sz1.Width - sz2.Width) < 0.001)
				return Math.Abs(sz1.Height - sz2.Height) < 0.001;
			return false;
		}

		public static bool operator !=(RSize sz1, RSize sz2)
		{
			return !(sz1 == sz2);
		}

		public static RSize Add(RSize sz1, RSize sz2)
		{
			return new RSize(sz1.Width + sz2.Width, sz1.Height + sz2.Height);
		}

		public static RSize Subtract(RSize sz1, RSize sz2)
		{
			return new RSize(sz1.Width - sz2.Width, sz1.Height - sz2.Height);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RSize))
				return false;
			RSize rSize = (RSize)obj;
			if (Math.Abs(rSize.Width - Width) < 0.001 && Math.Abs(rSize.Height - Height) < 0.001)
				return rSize.GetType() == GetType();
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public RPoint ToPointF()
		{
			return (RPoint)this;
		}

		public override string ToString()
		{
			return "{Width=" + _width + ", Height=" + _height + "}";
		}
	}
}
