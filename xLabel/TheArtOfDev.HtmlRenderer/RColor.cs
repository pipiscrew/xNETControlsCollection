using System;
using System.Text;

namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public struct RColor
	{
		public static readonly RColor Empty = default(RColor);

		private readonly long _value;

		public static RColor Transparent
		{
			get
			{
				return new RColor(0L);
			}
		}

		public static RColor Black
		{
			get
			{
				return FromArgb(0, 0, 0);
			}
		}

		public static RColor White
		{
			get
			{
				return FromArgb(255, 255, 255);
			}
		}

		public static RColor WhiteSmoke
		{
			get
			{
				return FromArgb(245, 245, 245);
			}
		}

		public static RColor LightGray
		{
			get
			{
				return FromArgb(211, 211, 211);
			}
		}

		public byte R
		{
			get
			{
				return (byte)((ulong)(_value >> 16) & 0xFFuL);
			}
		}

		public byte G
		{
			get
			{
				return (byte)((ulong)(_value >> 8) & 0xFFuL);
			}
		}

		public byte B
		{
			get
			{
				return (byte)((ulong)_value & 0xFFuL);
			}
		}

		public byte A
		{
			get
			{
				return (byte)((ulong)(_value >> 24) & 0xFFuL);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return _value == 0L;
			}
		}

		private RColor(long value)
		{
			_value = value;
		}

		public static bool operator ==(RColor left, RColor right)
		{
			return left._value == right._value;
		}

		public static bool operator !=(RColor left, RColor right)
		{
			return !(left == right);
		}

		public static RColor FromArgb(int alpha, int red, int green, int blue)
		{
			CheckByte(alpha);
			CheckByte(red);
			CheckByte(green);
			CheckByte(blue);
			return new RColor((long)(uint)((red << 16) | (green << 8) | blue | (alpha << 24)) & 0xFFFFFFFFL);
		}

		public static RColor FromArgb(int red, int green, int blue)
		{
			return FromArgb(255, red, green, blue);
		}

		public override bool Equals(object obj)
		{
			if (obj is RColor)
			{
				RColor rColor = (RColor)obj;
				return _value == rColor._value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(32);
			stringBuilder.Append(GetType().Name);
			stringBuilder.Append(" [");
			if ((ulong)_value > 0uL)
			{
				stringBuilder.Append("A=");
				stringBuilder.Append(A);
				stringBuilder.Append(", R=");
				stringBuilder.Append(R);
				stringBuilder.Append(", G=");
				stringBuilder.Append(G);
				stringBuilder.Append(", B=");
				stringBuilder.Append(B);
			}
			else
				stringBuilder.Append("Empty");
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		private static void CheckByte(int value)
		{
			if (value < 0 || value > 255)
				throw new ArgumentException("InvalidEx2BoundArgument");
		}
	}
}
