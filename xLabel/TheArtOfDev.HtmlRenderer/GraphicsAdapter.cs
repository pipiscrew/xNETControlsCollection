using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class GraphicsAdapter : RGraphics
	{
		private static readonly int[] _charFit;

		private static readonly int[] _charFitWidth;

		private static readonly CharacterRange[] _characterRanges;

		private static readonly StringFormat _stringFormat;

		private static readonly StringFormat _stringFormat2;

		private readonly Graphics _g;

		private readonly bool _useGdiPlusTextRendering;

		private IntPtr _hdc;

		private readonly bool _releaseGraphics;

		private bool _setRtl;

		static GraphicsAdapter()
		{
			_charFit = new int[1];
			_charFitWidth = new int[1000];
			_characterRanges = new CharacterRange[1];
			_stringFormat = new StringFormat(StringFormat.GenericTypographic);
			_stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoClip;
			_stringFormat2 = new StringFormat(StringFormat.GenericTypographic);
		}

		public GraphicsAdapter(Graphics g, bool useGdiPlusTextRendering, bool releaseGraphics = false)
			: base(WinFormsAdapter.Instance, Utils.Convert(g.ClipBounds))
		{
			ArgChecker.AssertArgNotNull(g, "g");
			_g = g;
			_releaseGraphics = releaseGraphics;
			_useGdiPlusTextRendering = useGdiPlusTextRendering;
		}

		public override void PopClip()
		{
			ReleaseHdc();
			_clipStack.Pop();
			_g.SetClip(Utils.Convert(_clipStack.Peek()), CombineMode.Replace);
		}

		public override void PushClip(RRect rect)
		{
			ReleaseHdc();
			_clipStack.Push(rect);
			_g.SetClip(Utils.Convert(rect), CombineMode.Replace);
		}

		public override void PushClipExclude(RRect rect)
		{
			ReleaseHdc();
			_clipStack.Push(_clipStack.Peek());
			_g.SetClip(Utils.Convert(rect), CombineMode.Exclude);
		}

		public override object SetAntiAliasSmoothingMode()
		{
			ReleaseHdc();
			SmoothingMode smoothingMode = _g.SmoothingMode;
			_g.SmoothingMode = SmoothingMode.AntiAlias;
			return smoothingMode;
		}

		public override void ReturnPreviousSmoothingMode(object prevMode)
		{
			if (prevMode != null)
			{
				ReleaseHdc();
				_g.SmoothingMode = (SmoothingMode)prevMode;
			}
		}

		public override RSize MeasureString(string str, RFont font)
		{
			if (_useGdiPlusTextRendering)
			{
				ReleaseHdc();
				FontAdapter fontAdapter = (FontAdapter)font;
				Font font2 = fontAdapter.Font;
				_characterRanges[0] = new CharacterRange(0, str.Length);
				_stringFormat.SetMeasurableCharacterRanges(_characterRanges);
				SizeF size = _g.MeasureCharacterRanges(str, font2, RectangleF.Empty, _stringFormat)[0].GetBounds(_g).Size;
				if (font.Height < 0.0)
				{
					int height = font2.Height;
					float num = font2.Size * (float)font2.FontFamily.GetCellDescent(font2.Style) / (float)font2.FontFamily.GetEmHeight(font2.Style);
					fontAdapter.SetMetrics(height, (int)Math.Round((float)height - num + 0.5f));
				}
				return Utils.Convert(size);
			}
			SetFont(font);
			Size size2 = default(Size);
			Win32Utils.GetTextExtentPoint32(_hdc, str, str.Length, ref size2);
			if (font.Height < 0.0)
			{
				TextMetric lptm;
				Win32Utils.GetTextMetrics(_hdc, out lptm);
				((FontAdapter)font).SetMetrics(size2.Height, lptm.tmHeight - lptm.tmDescent + lptm.tmUnderlined + 1);
			}
			return Utils.Convert(size2);
		}

		public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
		{
			charFit = 0;
			charFitWidth = 0.0;
			if (_useGdiPlusTextRendering)
			{
				ReleaseHdc();
				RSize rSize = MeasureString(str, font);
				for (int i = 1; i <= str.Length; i++)
				{
					charFit = i - 1;
					RSize rSize2 = MeasureString(str.Substring(0, i), font);
					if (rSize2.Height <= rSize.Height && rSize2.Width < maxWidth)
					{
						charFitWidth = rSize2.Width;
						continue;
					}
					break;
				}
			}
			else
			{
				SetFont(font);
				Size size = default(Size);
				Win32Utils.GetTextExtentExPoint(_hdc, str, str.Length, (int)Math.Round(maxWidth), _charFit, _charFitWidth, ref size);
				charFit = _charFit[0];
				charFitWidth = ((charFit > 0) ? _charFitWidth[charFit - 1] : 0);
			}
		}

		public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
		{
			if (_useGdiPlusTextRendering)
			{
				ReleaseHdc();
				SetRtlAlignGdiPlus(rtl);
				Brush brush = ((BrushAdapter)_adapter.GetSolidBrush(color)).Brush;
				_g.DrawString(str, ((FontAdapter)font).Font, brush, (int)(Math.Round(point.X) + (rtl ? size.Width : 0.0)), (int)Math.Round(point.Y), _stringFormat2);
				return;
			}
			Point point2 = Utils.ConvertRound(point);
			Color color2 = Utils.Convert(color);
			if (color.A == byte.MaxValue)
			{
				SetFont(font);
				SetTextColor(color2);
				SetRtlAlignGdi(rtl);
				Win32Utils.TextOut(_hdc, point2.X, point2.Y, str, str.Length);
			}
			else
			{
				InitHdc();
				SetRtlAlignGdi(rtl);
				DrawTransparentText(_hdc, str, font, point2, Utils.ConvertRound(size), color2);
			}
		}

		public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
		{
			TextureBrush textureBrush = new TextureBrush(((ImageAdapter)image).Image, Utils.Convert(dstRect));
			textureBrush.TranslateTransform((float)translateTransformLocation.X, (float)translateTransformLocation.Y);
			return new BrushAdapter(textureBrush, true);
		}

		public override RGraphicsPath GetGraphicsPath()
		{
			return new GraphicsPathAdapter();
		}

		public override void Dispose()
		{
			ReleaseHdc();
			if (_releaseGraphics)
				_g.Dispose();
			if (_useGdiPlusTextRendering && _setRtl)
				_stringFormat2.FormatFlags ^= StringFormatFlags.DirectionRightToLeft;
		}

		public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
		{
			ReleaseHdc();
			_g.DrawLine(((PenAdapter)pen).Pen, (float)x1, (float)y1, (float)x2, (float)y2);
		}

		public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
		{
			ReleaseHdc();
			_g.DrawRectangle(((PenAdapter)pen).Pen, (float)x, (float)y, (float)width, (float)height);
		}

		public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
		{
			ReleaseHdc();
			_g.FillRectangle(((BrushAdapter)brush).Brush, (float)x, (float)y, (float)width, (float)height);
		}

		public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
		{
			ReleaseHdc();
			_g.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect), Utils.Convert(srcRect), GraphicsUnit.Pixel);
		}

		public override void DrawImage(RImage image, RRect destRect)
		{
			ReleaseHdc();
			_g.DrawImage(((ImageAdapter)image).Image, Utils.Convert(destRect));
		}

		public override void DrawPath(RPen pen, RGraphicsPath path)
		{
			_g.DrawPath(((PenAdapter)pen).Pen, ((GraphicsPathAdapter)path).GraphicsPath);
		}

		public override void DrawPath(RBrush brush, RGraphicsPath path)
		{
			ReleaseHdc();
			_g.FillPath(((BrushAdapter)brush).Brush, ((GraphicsPathAdapter)path).GraphicsPath);
		}

		public override void DrawPolygon(RBrush brush, RPoint[] points)
		{
			if (points != null && points.Length != 0)
			{
				ReleaseHdc();
				_g.FillPolygon(((BrushAdapter)brush).Brush, Utils.Convert(points));
			}
		}

		private void ReleaseHdc()
		{
			if (_hdc != IntPtr.Zero)
			{
				Win32Utils.SelectClipRgn(_hdc, IntPtr.Zero);
				_g.ReleaseHdc(_hdc);
				_hdc = IntPtr.Zero;
			}
		}

		private void InitHdc()
		{
			if (_hdc == IntPtr.Zero)
			{
				IntPtr hrgn = _g.Clip.GetHrgn(_g);
				_hdc = _g.GetHdc();
				_setRtl = false;
				Win32Utils.SetBkMode(_hdc, 1);
				Win32Utils.SelectClipRgn(_hdc, hrgn);
				Win32Utils.DeleteObject(hrgn);
			}
		}

		private void SetFont(RFont font)
		{
			InitHdc();
			Win32Utils.SelectObject(_hdc, ((FontAdapter)font).HFont);
		}

		private void SetTextColor(Color color)
		{
			InitHdc();
			int color2 = ((color.B & 0xFF) << 16) | ((color.G & 0xFF) << 8) | color.R;
			Win32Utils.SetTextColor(_hdc, color2);
		}

		private void SetRtlAlignGdi(bool rtl)
		{
			if (_setRtl)
			{
				if (!rtl)
					Win32Utils.SetTextAlign(_hdc, 0u);
			}
			else if (rtl)
			{
				Win32Utils.SetTextAlign(_hdc, 256u);
			}
			_setRtl = rtl;
		}

		private static void DrawTransparentText(IntPtr hdc, string str, RFont font, Point point, Size size, Color color)
		{
			IntPtr dib;
			IntPtr intPtr = Win32Utils.CreateMemoryHdc(hdc, size.Width, size.Height, out dib);
			try
			{
				Win32Utils.BitBlt(intPtr, 0, 0, size.Width, size.Height, hdc, point.X, point.Y, 13369376u);
				Win32Utils.SelectObject(intPtr, ((FontAdapter)font).HFont);
				Win32Utils.SetTextColor(intPtr, ((color.B & 0xFF) << 16) | ((color.G & 0xFF) << 8) | color.R);
				Win32Utils.TextOut(intPtr, 0, 0, str, str.Length);
				Win32Utils.AlphaBlend(hdc, point.X, point.Y, size.Width, size.Height, intPtr, 0, 0, size.Width, size.Height, new BlendFunction(color.A));
			}
			finally
			{
				Win32Utils.ReleaseMemoryHdc(intPtr, dib);
			}
		}

		private void SetRtlAlignGdiPlus(bool rtl)
		{
			if (_setRtl)
			{
				if (!rtl)
					_stringFormat2.FormatFlags ^= StringFormatFlags.DirectionRightToLeft;
			}
			else if (rtl)
			{
				_stringFormat2.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			_setRtl = rtl;
		}
	}
}
