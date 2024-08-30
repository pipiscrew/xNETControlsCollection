using System.Globalization;
using System.Text.RegularExpressions;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal abstract class CssBoxProperties
	{
		private string _backgroundColor = "transparent";

		private string _backgroundGradient = "none";

		private string _backgroundGradientAngle = "90";

		private string _backgroundImage = "none";

		private string _backgroundPosition = "0% 0%";

		private string _backgroundRepeat = "repeat";

		private string _borderTopWidth = "medium";

		private string _borderRightWidth = "medium";

		private string _borderBottomWidth = "medium";

		private string _borderLeftWidth = "medium";

		private string _borderTopColor = "black";

		private string _borderRightColor = "black";

		private string _borderBottomColor = "black";

		private string _borderLeftColor = "black";

		private string _borderTopStyle = "none";

		private string _borderRightStyle = "none";

		private string _borderBottomStyle = "none";

		private string _borderLeftStyle = "none";

		private string _borderSpacing = "0";

		private string _borderCollapse = "separate";

		private string _bottom;

		private string _color = "black";

		private string _content = "normal";

		private string _cornerNwRadius = "0";

		private string _cornerNeRadius = "0";

		private string _cornerSeRadius = "0";

		private string _cornerSwRadius = "0";

		private string _cornerRadius = "0";

		private string _emptyCells = "show";

		private string _direction = "ltr";

		private string _display = "inline";

		private string _fontFamily;

		private string _fontSize = "medium";

		private string _fontStyle = "normal";

		private string _fontVariant = "normal";

		private string _fontWeight = "normal";

		private string _float = "none";

		private string _height = "auto";

		private string _marginBottom = "0";

		private string _marginLeft = "0";

		private string _marginRight = "0";

		private string _marginTop = "0";

		private string _left = "auto";

		private string _lineHeight = "normal";

		private string _listStyleType = "disc";

		private string _listStyleImage = string.Empty;

		private string _listStylePosition = "outside";

		private string _listStyle = string.Empty;

		private string _overflow = "visible";

		private string _paddingLeft = "0";

		private string _paddingBottom = "0";

		private string _paddingRight = "0";

		private string _paddingTop = "0";

		private string _pageBreakInside = "auto";

		private string _right;

		private string _textAlign = string.Empty;

		private string _textDecoration = string.Empty;

		private string _textIndent = "0";

		private string _top = "auto";

		private string _position = "static";

		private string _verticalAlign = "baseline";

		private string _width = "auto";

		private string _maxWidth = "none";

		private string _wordSpacing = "normal";

		private string _wordBreak = "normal";

		private string _whiteSpace = "normal";

		private string _visibility = "visible";

		private RPoint _location;

		private RSize _size;

		private double _actualCornerNw = double.NaN;

		private double _actualCornerNe = double.NaN;

		private double _actualCornerSw = double.NaN;

		private double _actualCornerSe = double.NaN;

		private RColor _actualColor = RColor.Empty;

		private double _actualBackgroundGradientAngle = double.NaN;

		private double _actualHeight = double.NaN;

		private double _actualWidth = double.NaN;

		private double _actualPaddingTop = double.NaN;

		private double _actualPaddingBottom = double.NaN;

		private double _actualPaddingRight = double.NaN;

		private double _actualPaddingLeft = double.NaN;

		private double _actualMarginTop = double.NaN;

		private double _collapsedMarginTop = double.NaN;

		private double _actualMarginBottom = double.NaN;

		private double _actualMarginRight = double.NaN;

		private double _actualMarginLeft = double.NaN;

		private double _actualBorderTopWidth = double.NaN;

		private double _actualBorderLeftWidth = double.NaN;

		private double _actualBorderBottomWidth = double.NaN;

		private double _actualBorderRightWidth = double.NaN;

		private double _actualLineHeight = double.NaN;

		private double _actualWordSpacing = double.NaN;

		private double _actualTextIndent = double.NaN;

		private double _actualBorderSpacingHorizontal = double.NaN;

		private double _actualBorderSpacingVertical = double.NaN;

		private RColor _actualBackgroundGradient = RColor.Empty;

		private RColor _actualBorderTopColor = RColor.Empty;

		private RColor _actualBorderLeftColor = RColor.Empty;

		private RColor _actualBorderBottomColor = RColor.Empty;

		private RColor _actualBorderRightColor = RColor.Empty;

		private RColor _actualBackgroundColor = RColor.Empty;

		private RFont _actualFont;

		public string BorderBottomWidth
		{
			get
			{
				return _borderBottomWidth;
			}
			set
			{
				_borderBottomWidth = value;
				_actualBorderBottomWidth = double.NaN;
			}
		}

		public string BorderLeftWidth
		{
			get
			{
				return _borderLeftWidth;
			}
			set
			{
				_borderLeftWidth = value;
				_actualBorderLeftWidth = double.NaN;
			}
		}

		public string BorderRightWidth
		{
			get
			{
				return _borderRightWidth;
			}
			set
			{
				_borderRightWidth = value;
				_actualBorderRightWidth = double.NaN;
			}
		}

		public string BorderTopWidth
		{
			get
			{
				return _borderTopWidth;
			}
			set
			{
				_borderTopWidth = value;
				_actualBorderTopWidth = double.NaN;
			}
		}

		public string BorderBottomStyle
		{
			get
			{
				return _borderBottomStyle;
			}
			set
			{
				_borderBottomStyle = value;
			}
		}

		public string BorderLeftStyle
		{
			get
			{
				return _borderLeftStyle;
			}
			set
			{
				_borderLeftStyle = value;
			}
		}

		public string BorderRightStyle
		{
			get
			{
				return _borderRightStyle;
			}
			set
			{
				_borderRightStyle = value;
			}
		}

		public string BorderTopStyle
		{
			get
			{
				return _borderTopStyle;
			}
			set
			{
				_borderTopStyle = value;
			}
		}

		public string BorderBottomColor
		{
			get
			{
				return _borderBottomColor;
			}
			set
			{
				_borderBottomColor = value;
				_actualBorderBottomColor = RColor.Empty;
			}
		}

		public string BorderLeftColor
		{
			get
			{
				return _borderLeftColor;
			}
			set
			{
				_borderLeftColor = value;
				_actualBorderLeftColor = RColor.Empty;
			}
		}

		public string BorderRightColor
		{
			get
			{
				return _borderRightColor;
			}
			set
			{
				_borderRightColor = value;
				_actualBorderRightColor = RColor.Empty;
			}
		}

		public string BorderTopColor
		{
			get
			{
				return _borderTopColor;
			}
			set
			{
				_borderTopColor = value;
				_actualBorderTopColor = RColor.Empty;
			}
		}

		public string BorderSpacing
		{
			get
			{
				return _borderSpacing;
			}
			set
			{
				_borderSpacing = value;
			}
		}

		public string BorderCollapse
		{
			get
			{
				return _borderCollapse;
			}
			set
			{
				_borderCollapse = value;
			}
		}

		public string CornerRadius
		{
			get
			{
				return _cornerRadius;
			}
			set
			{
				MatchCollection matchCollection = RegexParserUtils.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", value);
				switch (matchCollection.Count)
				{
				case 1:
					CornerNeRadius = matchCollection[0].Value;
					CornerNwRadius = matchCollection[0].Value;
					CornerSeRadius = matchCollection[0].Value;
					CornerSwRadius = matchCollection[0].Value;
					break;
				case 2:
					CornerNeRadius = matchCollection[0].Value;
					CornerNwRadius = matchCollection[0].Value;
					CornerSeRadius = matchCollection[1].Value;
					CornerSwRadius = matchCollection[1].Value;
					break;
				case 3:
					CornerNeRadius = matchCollection[0].Value;
					CornerNwRadius = matchCollection[1].Value;
					CornerSeRadius = matchCollection[2].Value;
					break;
				case 4:
					CornerNeRadius = matchCollection[0].Value;
					CornerNwRadius = matchCollection[1].Value;
					CornerSeRadius = matchCollection[2].Value;
					CornerSwRadius = matchCollection[3].Value;
					break;
				}
				_cornerRadius = value;
			}
		}

		public string CornerNwRadius
		{
			get
			{
				return _cornerNwRadius;
			}
			set
			{
				_cornerNwRadius = value;
			}
		}

		public string CornerNeRadius
		{
			get
			{
				return _cornerNeRadius;
			}
			set
			{
				_cornerNeRadius = value;
			}
		}

		public string CornerSeRadius
		{
			get
			{
				return _cornerSeRadius;
			}
			set
			{
				_cornerSeRadius = value;
			}
		}

		public string CornerSwRadius
		{
			get
			{
				return _cornerSwRadius;
			}
			set
			{
				_cornerSwRadius = value;
			}
		}

		public string MarginBottom
		{
			get
			{
				return _marginBottom;
			}
			set
			{
				_marginBottom = value;
			}
		}

		public string MarginLeft
		{
			get
			{
				return _marginLeft;
			}
			set
			{
				_marginLeft = value;
			}
		}

		public string MarginRight
		{
			get
			{
				return _marginRight;
			}
			set
			{
				_marginRight = value;
			}
		}

		public string MarginTop
		{
			get
			{
				return _marginTop;
			}
			set
			{
				_marginTop = value;
			}
		}

		public string PaddingBottom
		{
			get
			{
				return _paddingBottom;
			}
			set
			{
				_paddingBottom = value;
				_actualPaddingBottom = double.NaN;
			}
		}

		public string PaddingLeft
		{
			get
			{
				return _paddingLeft;
			}
			set
			{
				_paddingLeft = value;
				_actualPaddingLeft = double.NaN;
			}
		}

		public string PaddingRight
		{
			get
			{
				return _paddingRight;
			}
			set
			{
				_paddingRight = value;
				_actualPaddingRight = double.NaN;
			}
		}

		public string PaddingTop
		{
			get
			{
				return _paddingTop;
			}
			set
			{
				_paddingTop = value;
				_actualPaddingTop = double.NaN;
			}
		}

		public string PageBreakInside
		{
			get
			{
				return _pageBreakInside;
			}
			set
			{
				_pageBreakInside = value;
			}
		}

		public string Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
				if (Position == "fixed")
					_location = GetActualLocation(Left, Top);
			}
		}

		public string Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top = value;
				if (Position == "fixed")
					_location = GetActualLocation(Left, Top);
			}
		}

		public string Width
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

		public string MaxWidth
		{
			get
			{
				return _maxWidth;
			}
			set
			{
				_maxWidth = value;
			}
		}

		public string Height
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

		public string BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
			set
			{
				_backgroundColor = value;
			}
		}

		public string BackgroundImage
		{
			get
			{
				return _backgroundImage;
			}
			set
			{
				_backgroundImage = value;
			}
		}

		public string BackgroundPosition
		{
			get
			{
				return _backgroundPosition;
			}
			set
			{
				_backgroundPosition = value;
			}
		}

		public string BackgroundRepeat
		{
			get
			{
				return _backgroundRepeat;
			}
			set
			{
				_backgroundRepeat = value;
			}
		}

		public string BackgroundGradient
		{
			get
			{
				return _backgroundGradient;
			}
			set
			{
				_backgroundGradient = value;
			}
		}

		public string BackgroundGradientAngle
		{
			get
			{
				return _backgroundGradientAngle;
			}
			set
			{
				_backgroundGradientAngle = value;
			}
		}

		public string Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				_actualColor = RColor.Empty;
			}
		}

		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
			}
		}

		public string Display
		{
			get
			{
				return _display;
			}
			set
			{
				_display = value;
			}
		}

		public string Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
			}
		}

		public string EmptyCells
		{
			get
			{
				return _emptyCells;
			}
			set
			{
				_emptyCells = value;
			}
		}

		public string Float
		{
			get
			{
				return _float;
			}
			set
			{
				_float = value;
			}
		}

		public string Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public string LineHeight
		{
			get
			{
				return _lineHeight;
			}
			set
			{
				_lineHeight = string.Format(NumberFormatInfo.InvariantInfo, "{0}px", new object[1] { CssValueParser.ParseLength(value, Size.Height, this, "em") });
			}
		}

		public string VerticalAlign
		{
			get
			{
				return _verticalAlign;
			}
			set
			{
				_verticalAlign = value;
			}
		}

		public string TextIndent
		{
			get
			{
				return _textIndent;
			}
			set
			{
				_textIndent = NoEms(value);
			}
		}

		public string TextAlign
		{
			get
			{
				return _textAlign;
			}
			set
			{
				_textAlign = value;
			}
		}

		public string TextDecoration
		{
			get
			{
				return _textDecoration;
			}
			set
			{
				_textDecoration = value;
			}
		}

		public string WhiteSpace
		{
			get
			{
				return _whiteSpace;
			}
			set
			{
				_whiteSpace = value;
			}
		}

		public string Visibility
		{
			get
			{
				return _visibility;
			}
			set
			{
				_visibility = value;
			}
		}

		public string WordSpacing
		{
			get
			{
				return _wordSpacing;
			}
			set
			{
				_wordSpacing = NoEms(value);
			}
		}

		public string WordBreak
		{
			get
			{
				return _wordBreak;
			}
			set
			{
				_wordBreak = value;
			}
		}

		public string FontFamily
		{
			get
			{
				return _fontFamily;
			}
			set
			{
				_fontFamily = value;
			}
		}

		public string FontSize
		{
			get
			{
				return _fontSize;
			}
			set
			{
				string text = RegexParserUtils.Search("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", value);
				if (text != null)
				{
					CssLength cssLength = new CssLength(text);
					string text2 = (_fontSize = (cssLength.HasError ? "medium" : ((cssLength.Unit == CssUnit.Ems && GetParent() != null) ? cssLength.ConvertEmToPoints(GetParent().ActualFont.Size).ToString() : cssLength.ToString())));
				}
				else
					_fontSize = value;
			}
		}

		public string FontStyle
		{
			get
			{
				return _fontStyle;
			}
			set
			{
				_fontStyle = value;
			}
		}

		public string FontVariant
		{
			get
			{
				return _fontVariant;
			}
			set
			{
				_fontVariant = value;
			}
		}

		public string FontWeight
		{
			get
			{
				return _fontWeight;
			}
			set
			{
				_fontWeight = value;
			}
		}

		public string ListStyle
		{
			get
			{
				return _listStyle;
			}
			set
			{
				_listStyle = value;
			}
		}

		public string Overflow
		{
			get
			{
				return _overflow;
			}
			set
			{
				_overflow = value;
			}
		}

		public string ListStylePosition
		{
			get
			{
				return _listStylePosition;
			}
			set
			{
				_listStylePosition = value;
			}
		}

		public string ListStyleImage
		{
			get
			{
				return _listStyleImage;
			}
			set
			{
				_listStyleImage = value;
			}
		}

		public string ListStyleType
		{
			get
			{
				return _listStyleType;
			}
			set
			{
				_listStyleType = value;
			}
		}

		public RPoint Location
		{
			get
			{
				if (_location.IsEmpty && Position == "fixed")
				{
					string left = Left;
					string top = Top;
					_location = GetActualLocation(Left, Top);
				}
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public RSize Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		public RRect Bounds
		{
			get
			{
				return new RRect(Location, Size);
			}
		}

		public double AvailableWidth
		{
			get
			{
				return Size.Width - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth;
			}
		}

		public double ActualRight
		{
			get
			{
				return Location.X + Size.Width;
			}
			set
			{
				Size = new RSize(value - Location.X, Size.Height);
			}
		}

		public double ActualBottom
		{
			get
			{
				return Location.Y + Size.Height;
			}
			set
			{
				Size = new RSize(Size.Width, value - Location.Y);
			}
		}

		public double ClientLeft
		{
			get
			{
				return Location.X + ActualBorderLeftWidth + ActualPaddingLeft;
			}
		}

		public double ClientTop
		{
			get
			{
				return Location.Y + ActualBorderTopWidth + ActualPaddingTop;
			}
		}

		public double ClientRight
		{
			get
			{
				return ActualRight - ActualPaddingRight - ActualBorderRightWidth;
			}
		}

		public double ClientBottom
		{
			get
			{
				return ActualBottom - ActualPaddingBottom - ActualBorderBottomWidth;
			}
		}

		public RRect ClientRectangle
		{
			get
			{
				return RRect.FromLTRB(ClientLeft, ClientTop, ClientRight, ClientBottom);
			}
		}

		public double ActualHeight
		{
			get
			{
				if (double.IsNaN(_actualHeight))
					_actualHeight = CssValueParser.ParseLength(Height, Size.Height, this);
				return _actualHeight;
			}
		}

		public double ActualWidth
		{
			get
			{
				if (double.IsNaN(_actualWidth))
					_actualWidth = CssValueParser.ParseLength(Width, Size.Width, this);
				return _actualWidth;
			}
		}

		public double ActualPaddingTop
		{
			get
			{
				if (double.IsNaN(_actualPaddingTop))
					_actualPaddingTop = CssValueParser.ParseLength(PaddingTop, Size.Width, this);
				return _actualPaddingTop;
			}
		}

		public double ActualPaddingLeft
		{
			get
			{
				if (double.IsNaN(_actualPaddingLeft))
					_actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, Size.Width, this);
				return _actualPaddingLeft;
			}
		}

		public double ActualPaddingBottom
		{
			get
			{
				if (double.IsNaN(_actualPaddingBottom))
					_actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, Size.Width, this);
				return _actualPaddingBottom;
			}
		}

		public double ActualPaddingRight
		{
			get
			{
				if (double.IsNaN(_actualPaddingRight))
					_actualPaddingRight = CssValueParser.ParseLength(PaddingRight, Size.Width, this);
				return _actualPaddingRight;
			}
		}

		public double ActualMarginTop
		{
			get
			{
				if (double.IsNaN(_actualMarginTop))
				{
					if (MarginTop == "auto")
						MarginTop = "0";
					double num = CssValueParser.ParseLength(MarginTop, Size.Width, this);
					if (MarginLeft.EndsWith("%"))
						return num;
					_actualMarginTop = num;
				}
				return _actualMarginTop;
			}
		}

		public double CollapsedMarginTop
		{
			get
			{
				return double.IsNaN(_collapsedMarginTop) ? 0.0 : _collapsedMarginTop;
			}
			set
			{
				_collapsedMarginTop = value;
			}
		}

		public double ActualMarginLeft
		{
			get
			{
				if (double.IsNaN(_actualMarginLeft))
				{
					if (MarginLeft == "auto")
						MarginLeft = "0";
					double num = CssValueParser.ParseLength(MarginLeft, Size.Width, this);
					if (MarginLeft.EndsWith("%"))
						return num;
					_actualMarginLeft = num;
				}
				return _actualMarginLeft;
			}
		}

		public double ActualMarginBottom
		{
			get
			{
				if (double.IsNaN(_actualMarginBottom))
				{
					if (MarginBottom == "auto")
						MarginBottom = "0";
					double num = CssValueParser.ParseLength(MarginBottom, Size.Width, this);
					if (MarginLeft.EndsWith("%"))
						return num;
					_actualMarginBottom = num;
				}
				return _actualMarginBottom;
			}
		}

		public double ActualMarginRight
		{
			get
			{
				if (double.IsNaN(_actualMarginRight))
				{
					if (MarginRight == "auto")
						MarginRight = "0";
					double num = CssValueParser.ParseLength(MarginRight, Size.Width, this);
					if (MarginLeft.EndsWith("%"))
						return num;
					_actualMarginRight = num;
				}
				return _actualMarginRight;
			}
		}

		public double ActualBorderTopWidth
		{
			get
			{
				if (double.IsNaN(_actualBorderTopWidth))
				{
					_actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
					if (string.IsNullOrEmpty(BorderTopStyle) || BorderTopStyle == "none")
						_actualBorderTopWidth = 0.0;
				}
				return _actualBorderTopWidth;
			}
		}

		public double ActualBorderLeftWidth
		{
			get
			{
				if (double.IsNaN(_actualBorderLeftWidth))
				{
					_actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
					if (string.IsNullOrEmpty(BorderLeftStyle) || BorderLeftStyle == "none")
						_actualBorderLeftWidth = 0.0;
				}
				return _actualBorderLeftWidth;
			}
		}

		public double ActualBorderBottomWidth
		{
			get
			{
				if (double.IsNaN(_actualBorderBottomWidth))
				{
					_actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);
					if (string.IsNullOrEmpty(BorderBottomStyle) || BorderBottomStyle == "none")
						_actualBorderBottomWidth = 0.0;
				}
				return _actualBorderBottomWidth;
			}
		}

		public double ActualBorderRightWidth
		{
			get
			{
				if (double.IsNaN(_actualBorderRightWidth))
				{
					_actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
					if (string.IsNullOrEmpty(BorderRightStyle) || BorderRightStyle == "none")
						_actualBorderRightWidth = 0.0;
				}
				return _actualBorderRightWidth;
			}
		}

		public RColor ActualBorderTopColor
		{
			get
			{
				if (_actualBorderTopColor.IsEmpty)
					_actualBorderTopColor = GetActualColor(BorderTopColor);
				return _actualBorderTopColor;
			}
		}

		public RColor ActualBorderLeftColor
		{
			get
			{
				if (_actualBorderLeftColor.IsEmpty)
					_actualBorderLeftColor = GetActualColor(BorderLeftColor);
				return _actualBorderLeftColor;
			}
		}

		public RColor ActualBorderBottomColor
		{
			get
			{
				if (_actualBorderBottomColor.IsEmpty)
					_actualBorderBottomColor = GetActualColor(BorderBottomColor);
				return _actualBorderBottomColor;
			}
		}

		public RColor ActualBorderRightColor
		{
			get
			{
				if (_actualBorderRightColor.IsEmpty)
					_actualBorderRightColor = GetActualColor(BorderRightColor);
				return _actualBorderRightColor;
			}
		}

		public double ActualCornerNw
		{
			get
			{
				if (double.IsNaN(_actualCornerNw))
					_actualCornerNw = CssValueParser.ParseLength(CornerNwRadius, 0.0, this);
				return _actualCornerNw;
			}
		}

		public double ActualCornerNe
		{
			get
			{
				if (double.IsNaN(_actualCornerNe))
					_actualCornerNe = CssValueParser.ParseLength(CornerNeRadius, 0.0, this);
				return _actualCornerNe;
			}
		}

		public double ActualCornerSe
		{
			get
			{
				if (double.IsNaN(_actualCornerSe))
					_actualCornerSe = CssValueParser.ParseLength(CornerSeRadius, 0.0, this);
				return _actualCornerSe;
			}
		}

		public double ActualCornerSw
		{
			get
			{
				if (double.IsNaN(_actualCornerSw))
					_actualCornerSw = CssValueParser.ParseLength(CornerSwRadius, 0.0, this);
				return _actualCornerSw;
			}
		}

		public bool IsRounded
		{
			get
			{
				return ActualCornerNe > 0.0 || ActualCornerNw > 0.0 || ActualCornerSe > 0.0 || ActualCornerSw > 0.0;
			}
		}

		public double ActualWordSpacing
		{
			get
			{
				return _actualWordSpacing;
			}
		}

		public RColor ActualColor
		{
			get
			{
				if (_actualColor.IsEmpty)
					_actualColor = GetActualColor(Color);
				return _actualColor;
			}
		}

		public RColor ActualBackgroundColor
		{
			get
			{
				if (_actualBackgroundColor.IsEmpty)
					_actualBackgroundColor = GetActualColor(BackgroundColor);
				return _actualBackgroundColor;
			}
		}

		public RColor ActualBackgroundGradient
		{
			get
			{
				if (_actualBackgroundGradient.IsEmpty)
					_actualBackgroundGradient = GetActualColor(BackgroundGradient);
				return _actualBackgroundGradient;
			}
		}

		public double ActualBackgroundGradientAngle
		{
			get
			{
				if (double.IsNaN(_actualBackgroundGradientAngle))
					_actualBackgroundGradientAngle = CssValueParser.ParseNumber(BackgroundGradientAngle, 360.0);
				return _actualBackgroundGradientAngle;
			}
		}

		public RFont ActualParentFont
		{
			get
			{
				return (GetParent() == null) ? ActualFont : GetParent().ActualFont;
			}
		}

		public RFont ActualFont
		{
			get
			{
				if (_actualFont == null)
				{
					if (string.IsNullOrEmpty(FontFamily))
						FontFamily = CssConstants.DefaultFont;
					if (string.IsNullOrEmpty(FontSize))
						FontSize = CssConstants.FontSize.ToString(CultureInfo.InvariantCulture) + "pt";
					RFontStyle rFontStyle = RFontStyle.Regular;
					if (FontStyle == "italic" || FontStyle == "oblique")
						rFontStyle |= RFontStyle.Italic;
					if (FontWeight != "normal" && FontWeight != "lighter" && !string.IsNullOrEmpty(FontWeight) && FontWeight != "inherit")
						rFontStyle |= RFontStyle.Bold;
					double num = CssConstants.FontSize;
					if (GetParent() != null)
						num = GetParent().ActualFont.Size;
					double num2;
					switch (FontSize)
					{
					case "medium":
						num2 = CssConstants.FontSize;
						break;
					case "x-small":
						num2 = CssConstants.FontSize - 3.0;
						break;
					case "small":
						num2 = CssConstants.FontSize - 2.0;
						break;
					case "xx-large":
						num2 = CssConstants.FontSize + 4.0;
						break;
					case "large":
						num2 = CssConstants.FontSize + 2.0;
						break;
					case "xx-small":
						num2 = CssConstants.FontSize - 4.0;
						break;
					case "smaller":
						num2 = num - 2.0;
						break;
					case "x-large":
						num2 = CssConstants.FontSize + 3.0;
						break;
					case "larger":
						num2 = num + 2.0;
						break;
					default:
						num2 = CssValueParser.ParseLength(FontSize, num, num, null, true, true);
						break;
					}
					if (num2 <= 1.0)
						num2 = CssConstants.FontSize;
					_actualFont = GetCachedFont(FontFamily, num2, rFontStyle);
				}
				return _actualFont;
			}
		}

		public double ActualLineHeight
		{
			get
			{
				if (double.IsNaN(_actualLineHeight))
					_actualLineHeight = 0.89999997615814209 * CssValueParser.ParseLength(LineHeight, Size.Height, this);
				return _actualLineHeight;
			}
		}

		public double ActualTextIndent
		{
			get
			{
				if (double.IsNaN(_actualTextIndent))
					_actualTextIndent = CssValueParser.ParseLength(TextIndent, Size.Width, this);
				return _actualTextIndent;
			}
		}

		public double ActualBorderSpacingHorizontal
		{
			get
			{
				if (double.IsNaN(_actualBorderSpacingHorizontal))
				{
					MatchCollection matchCollection = RegexParserUtils.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", BorderSpacing);
					if (matchCollection.Count == 0)
						_actualBorderSpacingHorizontal = 0.0;
					else if (matchCollection.Count > 0)
					{
						_actualBorderSpacingHorizontal = CssValueParser.ParseLength(matchCollection[0].Value, 1.0, this);
					}
				}
				return _actualBorderSpacingHorizontal;
			}
		}

		public double ActualBorderSpacingVertical
		{
			get
			{
				if (double.IsNaN(_actualBorderSpacingVertical))
				{
					MatchCollection matchCollection = RegexParserUtils.Match("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", BorderSpacing);
					if (matchCollection.Count != 0)
					{
						if (matchCollection.Count != 1)
							_actualBorderSpacingVertical = CssValueParser.ParseLength(matchCollection[1].Value, 1.0, this);
						else
							_actualBorderSpacingVertical = CssValueParser.ParseLength(matchCollection[0].Value, 1.0, this);
					}
					else
						_actualBorderSpacingVertical = 0.0;
				}
				return _actualBorderSpacingVertical;
			}
		}

		protected abstract RPoint GetActualLocation(string X, string Y);

		protected abstract RColor GetActualColor(string colorStr);

		protected abstract RFont GetCachedFont(string fontFamily, double fsize, RFontStyle st);

		protected abstract CssBoxProperties GetParent();

		public double GetEmHeight()
		{
			return ActualFont.Height;
		}

		protected string NoEms(string length)
		{
			CssLength cssLength = new CssLength(length);
			if (cssLength.Unit == CssUnit.Ems)
				length = cssLength.ConvertEmToPixels(GetEmHeight()).ToString();
			return length;
		}

		protected void SetAllBorders(string style = null, string width = null, string color = null)
		{
			if (style != null)
			{
				string text2 = (BorderBottomStyle = style);
				string text4 = (BorderRightStyle = text2);
				string text7 = (BorderLeftStyle = (BorderTopStyle = text4));
			}
			if (width != null)
			{
				string text2 = (BorderBottomWidth = width);
				string text4 = (BorderRightWidth = text2);
				string text7 = (BorderLeftWidth = (BorderTopWidth = text4));
			}
			if (color != null)
			{
				string text2 = (BorderBottomColor = color);
				string text4 = (BorderRightColor = text2);
				string text7 = (BorderLeftColor = (BorderTopColor = text4));
			}
		}

		protected void MeasureWordSpacing(RGraphics g)
		{
			if (double.IsNaN(ActualWordSpacing))
			{
				_actualWordSpacing = CssUtils.WhiteSpace(g, this);
				if (WordSpacing != "normal")
				{
					string length = RegexParserUtils.Search("([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)", WordSpacing);
					_actualWordSpacing += CssValueParser.ParseLength(length, 1.0, this);
				}
			}
		}

		protected void InheritStyle(CssBox p, bool everything)
		{
			if (p != null)
			{
				_borderSpacing = p._borderSpacing;
				_borderCollapse = p._borderCollapse;
				_color = p._color;
				_emptyCells = p._emptyCells;
				_whiteSpace = p._whiteSpace;
				_visibility = p._visibility;
				_textIndent = p._textIndent;
				_textAlign = p._textAlign;
				_verticalAlign = p._verticalAlign;
				_fontFamily = p._fontFamily;
				_fontSize = p._fontSize;
				_fontStyle = p._fontStyle;
				_fontVariant = p._fontVariant;
				_fontWeight = p._fontWeight;
				_listStyleImage = p._listStyleImage;
				_listStylePosition = p._listStylePosition;
				_listStyleType = p._listStyleType;
				_listStyle = p._listStyle;
				_lineHeight = p._lineHeight;
				_wordBreak = p.WordBreak;
				_direction = p._direction;
				if (everything)
				{
					_backgroundColor = p._backgroundColor;
					_backgroundGradient = p._backgroundGradient;
					_backgroundGradientAngle = p._backgroundGradientAngle;
					_backgroundImage = p._backgroundImage;
					_backgroundPosition = p._backgroundPosition;
					_backgroundRepeat = p._backgroundRepeat;
					_borderTopWidth = p._borderTopWidth;
					_borderRightWidth = p._borderRightWidth;
					_borderBottomWidth = p._borderBottomWidth;
					_borderLeftWidth = p._borderLeftWidth;
					_borderTopColor = p._borderTopColor;
					_borderRightColor = p._borderRightColor;
					_borderBottomColor = p._borderBottomColor;
					_borderLeftColor = p._borderLeftColor;
					_borderTopStyle = p._borderTopStyle;
					_borderRightStyle = p._borderRightStyle;
					_borderBottomStyle = p._borderBottomStyle;
					_borderLeftStyle = p._borderLeftStyle;
					_bottom = p._bottom;
					_cornerNwRadius = p._cornerNwRadius;
					_cornerNeRadius = p._cornerNeRadius;
					_cornerSeRadius = p._cornerSeRadius;
					_cornerSwRadius = p._cornerSwRadius;
					_cornerRadius = p._cornerRadius;
					_display = p._display;
					_float = p._float;
					_height = p._height;
					_marginBottom = p._marginBottom;
					_marginLeft = p._marginLeft;
					_marginRight = p._marginRight;
					_marginTop = p._marginTop;
					_left = p._left;
					_lineHeight = p._lineHeight;
					_overflow = p._overflow;
					_paddingLeft = p._paddingLeft;
					_paddingBottom = p._paddingBottom;
					_paddingRight = p._paddingRight;
					_paddingTop = p._paddingTop;
					_right = p._right;
					_textDecoration = p._textDecoration;
					_top = p._top;
					_position = p._position;
					_width = p._width;
					_maxWidth = p._maxWidth;
					_wordSpacing = p._wordSpacing;
				}
			}
		}
	}
}
