using System;
using System.Globalization;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Parse
{
	internal sealed class CssValueParser
	{
		private readonly RAdapter _adapter;

		public CssValueParser(RAdapter adapter)
		{
			ArgChecker.AssertArgNotNull(adapter, "global");
			_adapter = adapter;
		}

		public static bool IsFloat(string str, int idx, int length)
		{
			if (length >= 1)
			{
				bool flag = false;
				int num = 0;
				while (true)
				{
					if (num < length)
					{
						if (str[idx + num] == '.')
						{
							if (flag)
								return false;
							flag = true;
						}
						else if (!char.IsDigit(str[idx + num]))
						{
							break;
						}
						num++;
						continue;
					}
					return true;
				}
				return false;
			}
			return false;
		}

		public static bool IsInt(string str, int idx, int length)
		{
			if (length < 1)
				return false;
			for (int i = 0; i < length; i++)
			{
				if (!char.IsDigit(str[idx + i]))
					return false;
			}
			return true;
		}

		public static bool IsValidLength(string value)
		{
			if (value.Length > 1)
			{
				string s = string.Empty;
				if (value.EndsWith("%"))
					s = value.Substring(0, value.Length - 1);
				else if (value.Length > 2)
				{
					s = value.Substring(0, value.Length - 2);
				}
				double result;
				return double.TryParse(s, out result);
			}
			return false;
		}

		public static double ParseNumber(string number, double hundredPercent)
		{
			if (!string.IsNullOrEmpty(number))
			{
				string s = number;
				bool flag;
				if (flag = number.EndsWith("%"))
					s = number.Substring(0, number.Length - 1);
				double result;
				if (double.TryParse(s, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out result))
				{
					if (flag)
						result = result / 100.0 * hundredPercent;
					return result;
				}
				return 0.0;
			}
			return 0.0;
		}

		public static double ParseLength(string length, double hundredPercent, CssBoxProperties box, bool fontAdjust = false)
		{
			return ParseLength(length, hundredPercent, box.GetEmHeight(), null, fontAdjust, false);
		}

		public static double ParseLength(string length, double hundredPercent, CssBoxProperties box, string defaultUnit)
		{
			return ParseLength(length, hundredPercent, box.GetEmHeight(), defaultUnit, false, false);
		}

		public static double ParseLength(string length, double hundredPercent, double emFactor, string defaultUnit, bool fontAdjust, bool returnPoints)
		{
			if (!string.IsNullOrEmpty(length) && !(length == "0"))
			{
				if (!length.EndsWith("%"))
				{
					bool hasUnit;
					string unit = GetUnit(length, defaultUnit, out hasUnit);
					string number = (hasUnit ? length.Substring(0, length.Length - 2) : length);
					double num;
					switch (unit)
					{
					case "in":
						num = 96.0;
						break;
					case "em":
						num = emFactor;
						break;
					case "pc":
						num = 16.0;
						break;
					case "ex":
						num = emFactor / 2.0;
						break;
					case "pt":
						num = 1.3333333730697632;
						if (returnPoints)
							return ParseNumber(number, hundredPercent);
						break;
					case "px":
						num = (fontAdjust ? 0.75f : 1f);
						break;
					case "cm":
						num = 37.7952766418457;
						break;
					default:
						num = 0.0;
						break;
					case "mm":
						num = 3.7795276641845703;
						break;
					}
					return num * ParseNumber(number, hundredPercent);
				}
				return ParseNumber(length, hundredPercent);
			}
			return 0.0;
		}

		private static string GetUnit(string length, string defaultUnit, out bool hasUnit)
		{
			string text = ((length.Length >= 3) ? length.Substring(length.Length - 2, 2) : string.Empty);
			switch (text)
			{
			default:
				hasUnit = false;
				text = defaultUnit ?? string.Empty;
				break;
			case "in":
			case "em":
			case "pc":
			case "ex":
			case "cm":
			case "mm":
			case "pt":
			case "px":
				hasUnit = true;
				break;
			}
			return text;
		}

		public bool IsColorValid(string colorValue)
		{
			RColor color;
			return TryGetColor(colorValue, 0, colorValue.Length, out color);
		}

		public RColor GetActualColor(string colorValue)
		{
			RColor color;
			TryGetColor(colorValue, 0, colorValue.Length, out color);
			return color;
		}

		public bool TryGetColor(string str, int idx, int length, out RColor color)
		{
			try
			{
				if (!string.IsNullOrEmpty(str))
				{
					if (length > 1 && str[idx] == '#')
						return GetColorByHex(str, idx, length, out color);
					if (length > 10 && CommonUtils.SubStringEquals(str, idx, 4, "rgb(") && str[length - 1] == ')')
						return GetColorByRgb(str, idx, length, out color);
					if (length > 13 && CommonUtils.SubStringEquals(str, idx, 5, "rgba(") && str[length - 1] == ')')
						return GetColorByRgba(str, idx, length, out color);
					return GetColorByName(str, idx, length, out color);
				}
			}
			catch
			{
			}
			color = RColor.Black;
			return false;
		}

		public static double GetActualBorderWidth(string borderValue, CssBoxProperties b)
		{
			if (string.IsNullOrEmpty(borderValue))
				return GetActualBorderWidth("medium", b);
			switch (borderValue)
			{
			default:
				return Math.Abs(ParseLength(borderValue, 1.0, b));
			case "thick":
				return 4.0;
			case "medium":
				return 2.0;
			case "thin":
				return 1.0;
			}
		}

		private static bool GetColorByHex(string str, int idx, int length, out RColor color)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			switch (length)
			{
			case 7:
				num = ParseHexInt(str, idx + 1, 2);
				num2 = ParseHexInt(str, idx + 3, 2);
				num3 = ParseHexInt(str, idx + 5, 2);
				break;
			case 4:
				num = ParseHexInt(str, idx + 1, 1);
				num = num * 16 + num;
				num2 = ParseHexInt(str, idx + 2, 1);
				num2 = num2 * 16 + num2;
				num3 = ParseHexInt(str, idx + 3, 1);
				num3 = num3 * 16 + num3;
				break;
			}
			if (num > -1 && num2 > -1 && num3 > -1)
			{
				color = RColor.FromArgb(num, num2, num3);
				return true;
			}
			color = RColor.Empty;
			return false;
		}

		private static bool GetColorByRgb(string str, int idx, int length, out RColor color)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			if (length > 10)
			{
				int startIdx = idx + 4;
				num = ParseIntAtIndex(str, ref startIdx);
				if (startIdx < idx + length)
					num2 = ParseIntAtIndex(str, ref startIdx);
				if (startIdx < idx + length)
					num3 = ParseIntAtIndex(str, ref startIdx);
			}
			if (num > -1 && num2 > -1 && num3 > -1)
			{
				color = RColor.FromArgb(num, num2, num3);
				return true;
			}
			color = RColor.Empty;
			return false;
		}

		private static bool GetColorByRgba(string str, int idx, int length, out RColor color)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			if (length > 13)
			{
				int startIdx = idx + 5;
				num = ParseIntAtIndex(str, ref startIdx);
				if (startIdx < idx + length)
					num2 = ParseIntAtIndex(str, ref startIdx);
				if (startIdx < idx + length)
					num3 = ParseIntAtIndex(str, ref startIdx);
				if (startIdx < idx + length)
					num4 = ParseIntAtIndex(str, ref startIdx);
			}
			if (num > -1 && num2 > -1 && num3 > -1 && num4 > -1)
			{
				color = RColor.FromArgb(num4, num, num2, num3);
				return true;
			}
			color = RColor.Empty;
			return false;
		}

		private bool GetColorByName(string str, int idx, int length, out RColor color)
		{
			color = _adapter.GetColor(str.Substring(idx, length));
			return color.A > 0;
		}

		private static int ParseIntAtIndex(string str, ref int startIdx)
		{
			int i = 0;
			while (char.IsWhiteSpace(str, startIdx))
			{
				startIdx++;
			}
			for (; char.IsDigit(str, startIdx + i); i++)
			{
			}
			int result = ParseInt(str, startIdx, i);
			startIdx = startIdx + i + 1;
			return result;
		}

		private static int ParseInt(string str, int idx, int length)
		{
			if (length >= 1)
			{
				int num = 0;
				for (int i = 0; i < length; i++)
				{
					int num2 = str[idx + i];
					if (num2 >= 48 && num2 <= 57)
					{
						num = num * 10 + num2 - 48;
						continue;
					}
					return -1;
				}
				return num;
			}
			return -1;
		}

		private static int ParseHexInt(string str, int idx, int length)
		{
			if (length >= 1)
			{
				int num = 0;
				for (int i = 0; i < length; i++)
				{
					int num2 = str[idx + i];
					if ((num2 >= 48 && num2 <= 57) || (num2 >= 65 && num2 <= 70) || (num2 >= 97 && num2 <= 102))
					{
						num = num * 16 + ((num2 <= 57) ? (num2 - 48) : (10 + num2 - ((num2 <= 70) ? 65 : 97)));
						continue;
					}
					return -1;
				}
				return num;
			}
			return -1;
		}
	}
}
