using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Parse;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal static class CssUtils
	{
		private static readonly RColor _defaultSelectionBackcolor = RColor.FromArgb(169, 51, 153, 255);

		public static RColor DefaultSelectionBackcolor
		{
			get
			{
				return _defaultSelectionBackcolor;
			}
		}

		public static double WhiteSpace(RGraphics g, CssBoxProperties box)
		{
			double num = box.ActualFont.GetWhitespaceWidth(g);
			if (!string.IsNullOrEmpty(box.WordSpacing) && !(box.WordSpacing == "normal"))
				num += CssValueParser.ParseLength(box.WordSpacing, 0.0, box, true);
			return num;
		}

		public static string GetPropertyValue(CssBox cssBox, string propName)
		{
			switch (propName)
			{
			case "top":
				return cssBox.Top;
			case "float":
				return cssBox.Float;
			case "border-left-color":
				return cssBox.BorderLeftColor;
			case "padding-left":
				return cssBox.PaddingLeft;
			case "max-width":
				return cssBox.MaxWidth;
			case "background-image":
				return cssBox.BackgroundImage;
			case "font-weight":
				return cssBox.FontWeight;
			case "list-style-type":
				return cssBox.ListStyleType;
			case "white-space":
				return cssBox.WhiteSpace;
			case "background-gradient-angle":
				return cssBox.BackgroundGradientAngle;
			case "width":
				return cssBox.Width;
			case "list-style":
				return cssBox.ListStyle;
			case "overflow":
				return cssBox.Overflow;
			case "border-bottom-color":
				return cssBox.BorderBottomColor;
			case "border-spacing":
				return cssBox.BorderSpacing;
			case "border-left-width":
				return cssBox.BorderLeftWidth;
			case "padding-right":
				return cssBox.PaddingRight;
			case "background-color":
				return cssBox.BackgroundColor;
			case "word-break":
				return cssBox.WordBreak;
			case "border-right-style":
				return cssBox.BorderRightStyle;
			case "height":
				return cssBox.Height;
			case "border-top-color":
				return cssBox.BorderTopColor;
			case "border-left-style":
				return cssBox.BorderLeftStyle;
			case "corner-sw-radius":
				return cssBox.CornerSwRadius;
			case "direction":
				return cssBox.Direction;
			case "background-gradient":
				return cssBox.BackgroundGradient;
			case "border-top-width":
				return cssBox.BorderTopWidth;
			case "line-height":
				return cssBox.LineHeight;
			case "padding-bottom":
				return cssBox.PaddingBottom;
			case "border-top-style":
				return cssBox.BorderTopStyle;
			case "font-size":
				return cssBox.FontSize;
			case "border-collapse":
				return cssBox.BorderCollapse;
			case "list-style-position":
				return cssBox.ListStylePosition;
			case "font-variant":
				return cssBox.FontVariant;
			case "text-align":
				return cssBox.TextAlign;
			case "left":
				return cssBox.Left;
			case "font-style":
				return cssBox.FontStyle;
			case "font-family":
				return cssBox.FontFamily;
			case "list-style-image":
				return cssBox.ListStyleImage;
			case "word-spacing":
				return cssBox.WordSpacing;
			case "vertical-align":
				return cssBox.VerticalAlign;
			case "margin-top":
				return cssBox.MarginTop;
			case "border-bottom-width":
				return cssBox.BorderBottomWidth;
			case "text-indent":
				return cssBox.TextIndent;
			case "corner-radius":
				return cssBox.CornerRadius;
			case "border-right-width":
				return cssBox.BorderRightWidth;
			case "color":
				return cssBox.Color;
			case "margin-right":
				return cssBox.MarginRight;
			case "display":
				return cssBox.Display;
			case "border-bottom-style":
				return cssBox.BorderBottomStyle;
			case "margin-left":
				return cssBox.MarginLeft;
			case "corner-se-radius":
				return cssBox.CornerSeRadius;
			case "padding-top":
				return cssBox.PaddingTop;
			case "empty-cells":
				return cssBox.EmptyCells;
			case "corner-ne-radius":
				return cssBox.CornerNeRadius;
			case "background-repeat":
				return cssBox.BackgroundRepeat;
			case "corner-nw-radius":
				return cssBox.CornerNwRadius;
			case "text-decoration":
				return cssBox.TextDecoration;
			case "border-right-color":
				return cssBox.BorderRightColor;
			case "background-position":
				return cssBox.BackgroundPosition;
			case "visibility":
				return cssBox.Visibility;
			case "margin-bottom":
				return cssBox.MarginBottom;
			case "page-break-inside":
				return cssBox.PageBreakInside;
			case "position":
				return cssBox.Position;
			case "content":
				return cssBox.Content;
			default:
				return null;
			}
		}

		public static void SetPropertyValue(CssBox cssBox, string propName, string value)
		{
			switch (propName)
			{
			case "border-right-width":
				cssBox.BorderRightWidth = value;
				break;
			case "color":
				cssBox.Color = value;
				break;
			case "margin-right":
				cssBox.MarginRight = value;
				break;
			case "display":
				cssBox.Display = value;
				break;
			case "margin-top":
				cssBox.MarginTop = value;
				break;
			case "border-bottom-width":
				cssBox.BorderBottomWidth = value;
				break;
			case "text-indent":
				cssBox.TextIndent = value;
				break;
			case "corner-radius":
				cssBox.CornerRadius = value;
				break;
			case "font-variant":
				cssBox.FontVariant = value;
				break;
			case "text-align":
				cssBox.TextAlign = value;
				break;
			case "left":
				cssBox.Left = value;
				break;
			case "font-style":
				cssBox.FontStyle = value;
				break;
			case "font-family":
				cssBox.FontFamily = value;
				break;
			case "list-style-image":
				cssBox.ListStyleImage = value;
				break;
			case "word-spacing":
				cssBox.WordSpacing = value;
				break;
			case "vertical-align":
				cssBox.VerticalAlign = value;
				break;
			case "border-bottom-style":
				cssBox.BorderBottomStyle = value;
				break;
			case "margin-left":
				cssBox.MarginLeft = value;
				break;
			case "corner-se-radius":
				cssBox.CornerSeRadius = value;
				break;
			case "padding-top":
				cssBox.PaddingTop = value;
				break;
			case "empty-cells":
				cssBox.EmptyCells = value;
				break;
			case "corner-ne-radius":
				cssBox.CornerNeRadius = value;
				break;
			case "background-repeat":
				cssBox.BackgroundRepeat = value;
				break;
			case "corner-nw-radius":
				cssBox.CornerNwRadius = value;
				break;
			case "text-decoration":
				cssBox.TextDecoration = value;
				break;
			case "border-right-color":
				cssBox.BorderRightColor = value;
				break;
			case "background-position":
				cssBox.BackgroundPosition = value;
				break;
			case "visibility":
				cssBox.Visibility = value;
				break;
			case "margin-bottom":
				cssBox.MarginBottom = value;
				break;
			case "page-break-inside":
				cssBox.PageBreakInside = value;
				break;
			case "position":
				cssBox.Position = value;
				break;
			case "content":
				cssBox.Content = value;
				break;
			case "padding-bottom":
				cssBox.PaddingBottom = value;
				break;
			case "border-top-style":
				cssBox.BorderTopStyle = value;
				break;
			case "font-size":
				cssBox.FontSize = value;
				break;
			case "border-collapse":
				cssBox.BorderCollapse = value;
				break;
			case "list-style-position":
				cssBox.ListStylePosition = value;
				break;
			case "direction":
				cssBox.Direction = value;
				break;
			case "background-gradient":
				cssBox.BackgroundGradient = value;
				break;
			case "border-top-width":
				cssBox.BorderTopWidth = value;
				break;
			case "line-height":
				cssBox.LineHeight = value;
				break;
			case "padding-right":
				cssBox.PaddingRight = value;
				break;
			case "background-color":
				cssBox.BackgroundColor = value;
				break;
			case "word-break":
				cssBox.WordBreak = value;
				break;
			case "border-right-style":
				cssBox.BorderRightStyle = value;
				break;
			case "height":
				cssBox.Height = value;
				break;
			case "border-top-color":
				cssBox.BorderTopColor = value;
				break;
			case "border-left-style":
				cssBox.BorderLeftStyle = value;
				break;
			case "corner-sw-radius":
				cssBox.CornerSwRadius = value;
				break;
			case "white-space":
				cssBox.WhiteSpace = value;
				break;
			case "background-gradient-angle":
				cssBox.BackgroundGradientAngle = value;
				break;
			case "width":
				cssBox.Width = value;
				break;
			case "list-style":
				cssBox.ListStyle = value;
				break;
			case "overflow":
				cssBox.Overflow = value;
				break;
			case "border-bottom-color":
				cssBox.BorderBottomColor = value;
				break;
			case "border-spacing":
				cssBox.BorderSpacing = value;
				break;
			case "border-left-width":
				cssBox.BorderLeftWidth = value;
				break;
			case "top":
				cssBox.Top = value;
				break;
			case "float":
				cssBox.Float = value;
				break;
			case "border-left-color":
				cssBox.BorderLeftColor = value;
				break;
			case "padding-left":
				cssBox.PaddingLeft = value;
				break;
			case "max-width":
				cssBox.MaxWidth = value;
				break;
			case "background-image":
				cssBox.BackgroundImage = value;
				break;
			case "font-weight":
				cssBox.FontWeight = value;
				break;
			case "list-style-type":
				cssBox.ListStyleType = value;
				break;
			}
		}
	}
}
