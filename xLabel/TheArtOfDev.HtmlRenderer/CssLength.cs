using System;
using System.Globalization;
using TheArtOfDev.HtmlRenderer.Core.Parse;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssLength
	{
		private readonly double _number;

		private readonly bool _isRelative;

		private readonly CssUnit _unit;

		private readonly string _length;

		private readonly bool _isPercentage;

		private readonly bool _hasError;

		public double Number
		{
			get
			{
				return _number;
			}
		}

		public bool HasError
		{
			get
			{
				return _hasError;
			}
		}

		public bool IsPercentage
		{
			get
			{
				return _isPercentage;
			}
		}

		public bool IsRelative
		{
			get
			{
				return _isRelative;
			}
		}

		public CssUnit Unit
		{
			get
			{
				return _unit;
			}
		}

		public string Length
		{
			get
			{
				return _length;
			}
		}

		public CssLength(string length)
		{
			_length = length;
			_number = 0.0;
			_unit = CssUnit.None;
			_isPercentage = false;
			if (string.IsNullOrEmpty(length) || length == "0")
				return;
			if (length.EndsWith("%"))
			{
				_number = CssValueParser.ParseNumber(length, 1.0);
				_isPercentage = true;
				return;
			}
			if (length.Length < 3)
			{
				double.TryParse(length, out _number);
				_hasError = true;
				return;
			}
			string text = length.Substring(length.Length - 2, 2);
			string s = length.Substring(0, length.Length - 2);
			switch (text)
			{
			case "in":
				_unit = CssUnit.Inches;
				break;
			case "em":
				_unit = CssUnit.Ems;
				_isRelative = true;
				break;
			case "pc":
				_unit = CssUnit.Picas;
				break;
			case "ex":
				_unit = CssUnit.Ex;
				_isRelative = true;
				break;
			case "pt":
				_unit = CssUnit.Points;
				break;
			case "px":
				_unit = CssUnit.Pixels;
				_isRelative = true;
				break;
			case "cm":
				_unit = CssUnit.Centimeters;
				break;
			default:
				_hasError = true;
				return;
			case "mm":
				_unit = CssUnit.Milimeters;
				break;
			}
			if (!double.TryParse(s, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out _number))
				_hasError = true;
		}

		public CssLength ConvertEmToPoints(double emSize)
		{
			if (HasError)
				throw new InvalidOperationException("Invalid length");
			if (Unit != CssUnit.Ems)
				throw new InvalidOperationException("Length is not in ems");
			return new CssLength(string.Format("{0}pt", Convert.ToSingle(Number * emSize).ToString("0.0", NumberFormatInfo.InvariantInfo)));
		}

		public CssLength ConvertEmToPixels(double pixelFactor)
		{
			if (HasError)
				throw new InvalidOperationException("Invalid length");
			if (Unit != CssUnit.Ems)
				throw new InvalidOperationException("Length is not in ems");
			return new CssLength(string.Format("{0}px", Convert.ToSingle(Number * pixelFactor).ToString("0.0", NumberFormatInfo.InvariantInfo)));
		}

		public override string ToString()
		{
			if (!HasError)
			{
				if (IsPercentage)
					return string.Format(NumberFormatInfo.InvariantInfo, "{0}%", new object[1] { Number });
				string text = string.Empty;
				switch (Unit)
				{
				case CssUnit.Ems:
					text = "em";
					break;
				case CssUnit.Pixels:
					text = "px";
					break;
				case CssUnit.Ex:
					text = "ex";
					break;
				case CssUnit.Inches:
					text = "in";
					break;
				case CssUnit.Centimeters:
					text = "cm";
					break;
				case CssUnit.Milimeters:
					text = "mm";
					break;
				case CssUnit.Points:
					text = "pt";
					break;
				case CssUnit.Picas:
					text = "pc";
					break;
				}
				return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}", new object[2] { Number, text });
			}
			return string.Empty;
		}
	}
}
