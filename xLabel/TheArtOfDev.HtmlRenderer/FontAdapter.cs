using System;
using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class FontAdapter : RFont
	{
		private readonly Font _font;

		private IntPtr _hFont;

		private float _underlineOffset = -1f;

		private float _height = -1f;

		private double _whitespaceWidth = -1.0;

		public Font Font
		{
			get
			{
				return _font;
			}
		}

		public IntPtr HFont
		{
			get
			{
				if (_hFont == IntPtr.Zero)
					_hFont = _font.ToHfont();
				return _hFont;
			}
		}

		public override double Size
		{
			get
			{
				return _font.Size;
			}
		}

		public override double UnderlineOffset
		{
			get
			{
				return _underlineOffset;
			}
		}

		public override double Height
		{
			get
			{
				return _height;
			}
		}

		public override double LeftPadding
		{
			get
			{
				return _height / 6f;
			}
		}

		public FontAdapter(Font font)
		{
			_font = font;
		}

		public override double GetWhitespaceWidth(RGraphics graphics)
		{
			if (_whitespaceWidth < 0.0)
				_whitespaceWidth = graphics.MeasureString(" ", this).Width;
			return _whitespaceWidth;
		}

		internal void SetMetrics(int height, int underlineOffset)
		{
			_height = height;
			_underlineOffset = underlineOffset;
		}
	}
}
