using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal static class BordersDrawHandler
	{
		private static readonly RPoint[] _borderPts = new RPoint[4];

		public static void DrawBoxBorders(RGraphics g, CssBox box, RRect rect, bool isFirst, bool isLast)
		{
			if (rect.Width > 0.0 && rect.Height > 0.0)
			{
				if (!string.IsNullOrEmpty(box.BorderTopStyle) && !(box.BorderTopStyle == "none") && !(box.BorderTopStyle == "hidden") && box.ActualBorderTopWidth > 0.0)
					DrawBorder(Border.Top, box, g, rect, isFirst, isLast);
				if (isFirst && !string.IsNullOrEmpty(box.BorderLeftStyle) && !(box.BorderLeftStyle == "none") && !(box.BorderLeftStyle == "hidden") && box.ActualBorderLeftWidth > 0.0)
					DrawBorder(Border.Left, box, g, rect, true, isLast);
				if (!string.IsNullOrEmpty(box.BorderBottomStyle) && !(box.BorderBottomStyle == "none") && !(box.BorderBottomStyle == "hidden") && box.ActualBorderBottomWidth > 0.0)
					DrawBorder(Border.Bottom, box, g, rect, isFirst, isLast);
				if (isLast && !string.IsNullOrEmpty(box.BorderRightStyle) && !(box.BorderRightStyle == "none") && !(box.BorderRightStyle == "hidden") && box.ActualBorderRightWidth > 0.0)
					DrawBorder(Border.Right, box, g, rect, isFirst, true);
			}
		}

		public static void DrawBorder(Border border, RGraphics g, CssBox box, RBrush brush, RRect rectangle)
		{
			SetInOutsetRectanglePoints(border, box, rectangle, true, true);
			g.DrawPolygon(brush, _borderPts);
		}

		private static void DrawBorder(Border border, CssBox box, RGraphics g, RRect rect, bool isLineStart, bool isLineEnd)
		{
			string style = GetStyle(border, box);
			RColor color = GetColor(border, box, style);
			RGraphicsPath roundedBorderPath = GetRoundedBorderPath(g, border, box, rect);
			if (roundedBorderPath != null)
			{
				object prevMode = null;
				if (box.HtmlContainer != null && !box.HtmlContainer.AvoidGeometryAntialias && box.IsRounded)
					prevMode = g.SetAntiAliasSmoothingMode();
				RPen pen = GetPen(g, style, color, GetWidth(border, box));
				using (roundedBorderPath)
					g.DrawPath(pen, roundedBorderPath);
				g.ReturnPreviousSmoothingMode(prevMode);
				return;
			}
			if (style == "inset" || style == "outset")
			{
				SetInOutsetRectanglePoints(border, box, rect, isLineStart, isLineEnd);
				g.DrawPolygon(g.GetSolidBrush(color), _borderPts);
				return;
			}
			RPen pen2 = GetPen(g, style, color, GetWidth(border, box));
			switch (border)
			{
			case Border.Top:
				g.DrawLine(pen2, Math.Ceiling(rect.Left), rect.Top + box.ActualBorderTopWidth / 2.0, rect.Right - 1.0, rect.Top + box.ActualBorderTopWidth / 2.0);
				break;
			case Border.Right:
				g.DrawLine(pen2, rect.Right - box.ActualBorderRightWidth / 2.0, Math.Ceiling(rect.Top), rect.Right - box.ActualBorderRightWidth / 2.0, Math.Floor(rect.Bottom));
				break;
			case Border.Bottom:
				g.DrawLine(pen2, Math.Ceiling(rect.Left), rect.Bottom - box.ActualBorderBottomWidth / 2.0, rect.Right - 1.0, rect.Bottom - box.ActualBorderBottomWidth / 2.0);
				break;
			case Border.Left:
				g.DrawLine(pen2, rect.Left + box.ActualBorderLeftWidth / 2.0, Math.Ceiling(rect.Top), rect.Left + box.ActualBorderLeftWidth / 2.0, Math.Floor(rect.Bottom));
				break;
			}
		}

		private static void SetInOutsetRectanglePoints(Border border, CssBox b, RRect r, bool isLineStart, bool isLineEnd)
		{
			switch (border)
			{
			case Border.Top:
				_borderPts[0] = new RPoint(r.Left, r.Top);
				_borderPts[1] = new RPoint(r.Right, r.Top);
				_borderPts[2] = new RPoint(r.Right, r.Top + b.ActualBorderTopWidth);
				_borderPts[3] = new RPoint(r.Left, r.Top + b.ActualBorderTopWidth);
				if (isLineEnd)
					_borderPts[2].X -= b.ActualBorderRightWidth;
				if (isLineStart)
					_borderPts[3].X += b.ActualBorderLeftWidth;
				break;
			case Border.Right:
				_borderPts[0] = new RPoint(r.Right - b.ActualBorderRightWidth, r.Top + b.ActualBorderTopWidth);
				_borderPts[1] = new RPoint(r.Right, r.Top);
				_borderPts[2] = new RPoint(r.Right, r.Bottom);
				_borderPts[3] = new RPoint(r.Right - b.ActualBorderRightWidth, r.Bottom - b.ActualBorderBottomWidth);
				break;
			case Border.Bottom:
				_borderPts[0] = new RPoint(r.Left, r.Bottom - b.ActualBorderBottomWidth);
				_borderPts[1] = new RPoint(r.Right, r.Bottom - b.ActualBorderBottomWidth);
				_borderPts[2] = new RPoint(r.Right, r.Bottom);
				_borderPts[3] = new RPoint(r.Left, r.Bottom);
				if (isLineStart)
					_borderPts[0].X += b.ActualBorderLeftWidth;
				if (isLineEnd)
					_borderPts[1].X -= b.ActualBorderRightWidth;
				break;
			case Border.Left:
				_borderPts[0] = new RPoint(r.Left, r.Top);
				_borderPts[1] = new RPoint(r.Left + b.ActualBorderLeftWidth, r.Top + b.ActualBorderTopWidth);
				_borderPts[2] = new RPoint(r.Left + b.ActualBorderLeftWidth, r.Bottom - b.ActualBorderBottomWidth);
				_borderPts[3] = new RPoint(r.Left, r.Bottom);
				break;
			}
		}

		private static RGraphicsPath GetRoundedBorderPath(RGraphics g, Border border, CssBox b, RRect r)
		{
			RGraphicsPath rGraphicsPath = null;
			switch (border)
			{
			case Border.Top:
				if (b.ActualCornerNw > 0.0 || b.ActualCornerNe > 0.0)
				{
					rGraphicsPath = g.GetGraphicsPath();
					rGraphicsPath.Start(r.Left + b.ActualBorderLeftWidth / 2.0, r.Top + b.ActualBorderTopWidth / 2.0 + b.ActualCornerNw);
					if (b.ActualCornerNw > 0.0)
						rGraphicsPath.ArcTo(r.Left + b.ActualBorderLeftWidth / 2.0 + b.ActualCornerNw, r.Top + b.ActualBorderTopWidth / 2.0, b.ActualCornerNw, RGraphicsPath.Corner.TopLeft);
					rGraphicsPath.LineTo(r.Right - b.ActualBorderRightWidth / 2.0 - b.ActualCornerNe, r.Top + b.ActualBorderTopWidth / 2.0);
					if (b.ActualCornerNe > 0.0)
						rGraphicsPath.ArcTo(r.Right - b.ActualBorderRightWidth / 2.0, r.Top + b.ActualBorderTopWidth / 2.0 + b.ActualCornerNe, b.ActualCornerNe, RGraphicsPath.Corner.TopRight);
				}
				break;
			case Border.Right:
				if (b.ActualCornerNe > 0.0 || b.ActualCornerSe > 0.0)
				{
					rGraphicsPath = g.GetGraphicsPath();
					bool flag3 = b.BorderTopStyle == "none" || b.BorderTopStyle == "hidden";
					bool flag4 = b.BorderBottomStyle == "none" || b.BorderBottomStyle == "hidden";
					rGraphicsPath.Start(r.Right - b.ActualBorderRightWidth / 2.0 - (flag3 ? b.ActualCornerNe : 0.0), r.Top + b.ActualBorderTopWidth / 2.0 + (flag3 ? 0.0 : b.ActualCornerNe));
					if (b.ActualCornerNe > 0.0 && flag3)
						rGraphicsPath.ArcTo(r.Right - b.ActualBorderLeftWidth / 2.0, r.Top + b.ActualBorderTopWidth / 2.0 + b.ActualCornerNe, b.ActualCornerNe, RGraphicsPath.Corner.TopRight);
					rGraphicsPath.LineTo(r.Right - b.ActualBorderRightWidth / 2.0, r.Bottom - b.ActualBorderBottomWidth / 2.0 - b.ActualCornerSe);
					if (b.ActualCornerSe > 0.0 && flag4)
						rGraphicsPath.ArcTo(r.Right - b.ActualBorderRightWidth / 2.0 - b.ActualCornerSe, r.Bottom - b.ActualBorderBottomWidth / 2.0, b.ActualCornerSe, RGraphicsPath.Corner.BottomRight);
				}
				break;
			case Border.Bottom:
				if (b.ActualCornerSw > 0.0 || b.ActualCornerSe > 0.0)
				{
					rGraphicsPath = g.GetGraphicsPath();
					rGraphicsPath.Start(r.Right - b.ActualBorderRightWidth / 2.0, r.Bottom - b.ActualBorderBottomWidth / 2.0 - b.ActualCornerSe);
					if (b.ActualCornerSe > 0.0)
						rGraphicsPath.ArcTo(r.Right - b.ActualBorderRightWidth / 2.0 - b.ActualCornerSe, r.Bottom - b.ActualBorderBottomWidth / 2.0, b.ActualCornerSe, RGraphicsPath.Corner.BottomRight);
					rGraphicsPath.LineTo(r.Left + b.ActualBorderLeftWidth / 2.0 + b.ActualCornerSw, r.Bottom - b.ActualBorderBottomWidth / 2.0);
					if (b.ActualCornerSw > 0.0)
						rGraphicsPath.ArcTo(r.Left + b.ActualBorderLeftWidth / 2.0, r.Bottom - b.ActualBorderBottomWidth / 2.0 - b.ActualCornerSw, b.ActualCornerSw, RGraphicsPath.Corner.BottomLeft);
				}
				break;
			case Border.Left:
				if (b.ActualCornerNw > 0.0 || b.ActualCornerSw > 0.0)
				{
					rGraphicsPath = g.GetGraphicsPath();
					bool flag = b.BorderTopStyle == "none" || b.BorderTopStyle == "hidden";
					bool flag2 = b.BorderBottomStyle == "none" || b.BorderBottomStyle == "hidden";
					rGraphicsPath.Start(r.Left + b.ActualBorderLeftWidth / 2.0 + (flag2 ? b.ActualCornerSw : 0.0), r.Bottom - b.ActualBorderBottomWidth / 2.0 - (flag2 ? 0.0 : b.ActualCornerSw));
					if (b.ActualCornerSw > 0.0 && flag2)
						rGraphicsPath.ArcTo(r.Left + b.ActualBorderLeftWidth / 2.0, r.Bottom - b.ActualBorderBottomWidth / 2.0 - b.ActualCornerSw, b.ActualCornerSw, RGraphicsPath.Corner.BottomLeft);
					rGraphicsPath.LineTo(r.Left + b.ActualBorderLeftWidth / 2.0, r.Top + b.ActualBorderTopWidth / 2.0 + b.ActualCornerNw);
					if (b.ActualCornerNw > 0.0 && flag)
						rGraphicsPath.ArcTo(r.Left + b.ActualBorderLeftWidth / 2.0 + b.ActualCornerNw, r.Top + b.ActualBorderTopWidth / 2.0, b.ActualCornerNw, RGraphicsPath.Corner.TopLeft);
				}
				break;
			}
			return rGraphicsPath;
		}

		private static RPen GetPen(RGraphics g, string style, RColor color, double width)
		{
			RPen pen = g.GetPen(color);
			pen.Width = width;
			switch (style)
			{
			case "dotted":
				pen.DashStyle = RDashStyle.Dot;
				break;
			case "dashed":
				pen.DashStyle = RDashStyle.Dash;
				break;
			case "solid":
				pen.DashStyle = RDashStyle.Solid;
				break;
			}
			return pen;
		}

		private static RColor GetColor(Border border, CssBoxProperties box, string style)
		{
			switch (border)
			{
			default:
				throw new ArgumentOutOfRangeException("border");
			case Border.Top:
				return (style == "inset") ? Darken(box.ActualBorderTopColor) : box.ActualBorderTopColor;
			case Border.Right:
				return (style == "outset") ? Darken(box.ActualBorderRightColor) : box.ActualBorderRightColor;
			case Border.Bottom:
				return (style == "outset") ? Darken(box.ActualBorderBottomColor) : box.ActualBorderBottomColor;
			case Border.Left:
				return (style == "inset") ? Darken(box.ActualBorderLeftColor) : box.ActualBorderLeftColor;
			}
		}

		private static double GetWidth(Border border, CssBoxProperties box)
		{
			switch (border)
			{
			default:
				throw new ArgumentOutOfRangeException("border");
			case Border.Top:
				return box.ActualBorderTopWidth;
			case Border.Right:
				return box.ActualBorderRightWidth;
			case Border.Bottom:
				return box.ActualBorderBottomWidth;
			case Border.Left:
				return box.ActualBorderLeftWidth;
			}
		}

		private static string GetStyle(Border border, CssBoxProperties box)
		{
			switch (border)
			{
			default:
				throw new ArgumentOutOfRangeException("border");
			case Border.Top:
				return box.BorderTopStyle;
			case Border.Right:
				return box.BorderRightStyle;
			case Border.Bottom:
				return box.BorderBottomStyle;
			case Border.Left:
				return box.BorderLeftStyle;
			}
		}

		private static RColor Darken(RColor c)
		{
			return RColor.FromArgb((int)c.R / 2, (int)c.G / 2, (int)c.B / 2);
		}
	}
}
