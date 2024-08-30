using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Parse
{
	internal sealed class CssParser
	{
		private static readonly char[] _cssBlockSplitters = new char[2] { '}', ';' };

		private readonly RAdapter _adapter;

		private readonly CssValueParser _valueParser;

		private static readonly char[] _cssClassTrimChars = new char[8] { '\r', '\n', '\t', ' ', '-', '!', '<', '>' };

		public CssParser(RAdapter adapter)
		{
			ArgChecker.AssertArgNotNull(adapter, "global");
			_valueParser = new CssValueParser(adapter);
			_adapter = adapter;
		}

		public CssData ParseStyleSheet(string stylesheet, bool combineWithDefault)
		{
			CssData cssData = (combineWithDefault ? _adapter.DefaultCssData.Clone() : new CssData());
			if (!string.IsNullOrEmpty(stylesheet))
				ParseStyleSheet(cssData, stylesheet);
			return cssData;
		}

		public void ParseStyleSheet(CssData cssData, string stylesheet)
		{
			if (!string.IsNullOrEmpty(stylesheet))
			{
				stylesheet = RemoveStylesheetComments(stylesheet);
				ParseStyleBlocks(cssData, stylesheet);
				ParseMediaStyleBlocks(cssData, stylesheet);
			}
		}

		public CssBlock ParseCssBlock(string className, string blockSource)
		{
			return ParseCssBlockImp(className, blockSource);
		}

		public string ParseFontFamily(string value)
		{
			return ParseFontFamilyProperty(value);
		}

		public RColor ParseColor(string colorStr)
		{
			return _valueParser.GetActualColor(colorStr);
		}

		private static string RemoveStylesheetComments(string stylesheet)
		{
			StringBuilder stringBuilder = null;
			int num = 0;
			int num2 = 0;
			while (num2 > -1 && num2 < stylesheet.Length)
			{
				num2 = stylesheet.IndexOf("/*", num2);
				if (num2 > -1)
				{
					if (stringBuilder == null)
						stringBuilder = new StringBuilder(stylesheet.Length);
					stringBuilder.Append(stylesheet.Substring(num, num2 - num));
					int num3 = stylesheet.IndexOf("*/", num2 + 2);
					if (num3 < 0)
						num3 = stylesheet.Length;
					num = (num2 = num3 + 2);
				}
				else if (stringBuilder != null)
				{
					stringBuilder.Append(stylesheet.Substring(num));
				}
			}
			return (stringBuilder != null) ? stringBuilder.ToString() : stylesheet;
		}

		private void ParseStyleBlocks(CssData cssData, string stylesheet)
		{
			int i = 0;
			int j = 0;
			while (i < stylesheet.Length && j > -1)
			{
				j = i;
				while (j + 1 < stylesheet.Length)
				{
					j++;
					if (stylesheet[j] == '}')
						i = j + 1;
					if (stylesheet[j] == '{')
						break;
				}
				int num = j + 1;
				if (j <= -1)
					continue;
				for (j++; j < stylesheet.Length; j++)
				{
					if (stylesheet[j] == '{')
						i = num + 1;
					if (stylesheet[j] == '}')
						break;
				}
				if (j < stylesheet.Length)
				{
					for (; char.IsWhiteSpace(stylesheet[i]); i++)
					{
					}
					string block = stylesheet.Substring(i, j - i + 1);
					FeedStyleBlock(cssData, block);
				}
				i = j + 1;
			}
		}

		private void ParseMediaStyleBlocks(CssData cssData, string stylesheet)
		{
			int startIdx = 0;
			string cssAtRules;
			while ((cssAtRules = RegexParserUtils.GetCssAtRules(stylesheet, ref startIdx)) != null)
			{
				if (!cssAtRules.StartsWith("@media", StringComparison.InvariantCultureIgnoreCase))
					continue;
				MatchCollection matchCollection = RegexParserUtils.Match("@media[^\\{\\}]*\\{", cssAtRules);
				if (matchCollection.Count != 1)
					continue;
				string value = matchCollection[0].Value;
				if (!value.StartsWith("@media", StringComparison.InvariantCultureIgnoreCase) || !value.EndsWith("{"))
					continue;
				string[] array = value.Substring(6, value.Length - 7).Split(' ');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (string.IsNullOrEmpty(text.Trim()))
						continue;
					MatchCollection matchCollection2 = RegexParserUtils.Match("[^\\{\\}]*\\{[^\\{\\}]*\\}", cssAtRules);
					foreach (Match item in matchCollection2)
					{
						FeedStyleBlock(cssData, item.Value, text.Trim());
					}
				}
			}
		}

		private void FeedStyleBlock(CssData cssData, string block, string media = "all")
		{
			int num = block.IndexOf("{", StringComparison.Ordinal);
			int num2 = ((num > -1) ? block.IndexOf("}", num) : (-1));
			if (num <= -1 || num2 <= -1)
				return;
			string blockSource = block.Substring(num + 1, num2 - num - 1);
			string[] array = block.Substring(0, num).Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text.Trim(_cssClassTrimChars);
				if (!string.IsNullOrEmpty(text2))
				{
					CssBlock cssBlock = ParseCssBlockImp(text2, blockSource);
					if (cssBlock != null)
						cssData.AddCssBlock(media, cssBlock);
				}
			}
		}

		private CssBlock ParseCssBlockImp(string className, string blockSource)
		{
			className = className.ToLower();
			string text = null;
			int num = className.IndexOf(":", StringComparison.Ordinal);
			if (num > -1 && !className.StartsWith("::"))
			{
				text = ((num < className.Length - 1) ? className.Substring(num + 1).Trim() : null);
				className = className.Substring(0, num).Trim();
			}
			if (!string.IsNullOrEmpty(className) && (text == null || text == "link" || text == "hover"))
			{
				string firstClass;
				List<CssBlockSelectorItem> selectors = ParseCssBlockSelector(className, out firstClass);
				Dictionary<string, string> properties = ParseCssBlockProperties(blockSource);
				return new CssBlock(firstClass, properties, selectors, text == "hover");
			}
			return null;
		}

		private static List<CssBlockSelectorItem> ParseCssBlockSelector(string className, out string firstClass)
		{
			List<CssBlockSelectorItem> list = null;
			firstClass = null;
			int num = className.Length - 1;
			while (num > -1)
			{
				bool flag = false;
				while (char.IsWhiteSpace(className[num]) || className[num] == '>')
				{
					flag = flag || className[num] == '>';
					num--;
				}
				int num2 = num;
				while (num2 > -1 && !char.IsWhiteSpace(className[num2]) && className[num2] != '>')
				{
					num2--;
				}
				if (num2 > -1)
				{
					if (list == null)
						list = new List<CssBlockSelectorItem>();
					string text = className.Substring(num2 + 1, num - num2);
					if (firstClass == null)
						firstClass = text;
					else
					{
						while (char.IsWhiteSpace(className[num2]) || className[num2] == '>')
						{
							num2--;
						}
						list.Add(new CssBlockSelectorItem(text, flag));
					}
				}
				else if (firstClass != null)
				{
					list.Add(new CssBlockSelectorItem(className.Substring(0, num + 1), flag));
				}
				num = num2;
			}
			firstClass = firstClass ?? className;
			return list;
		}

		private Dictionary<string, string> ParseCssBlockProperties(string blockSource)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = 0;
			while (num < blockSource.Length)
			{
				int num2 = blockSource.IndexOfAny(_cssBlockSplitters, num);
				if (num >= 0 && num2 - num >= 10 && blockSource.Length - num >= 10 && blockSource.IndexOf("data:image", num, num2 - num) >= 0)
					num2 = blockSource.IndexOfAny(_cssBlockSplitters, num2 + 1);
				if (num2 < 0)
					num2 = blockSource.Length - 1;
				int num3 = blockSource.IndexOf(':', num, num2 - num);
				if (num3 > -1)
				{
					num += ((blockSource[num] == ' ') ? 1 : 0);
					int num4 = num2 - ((blockSource[num2] == ' ' || blockSource[num2] == ';') ? 1 : 0);
					string propName = blockSource.Substring(num, num3 - num).Trim().ToLower();
					num3 += ((blockSource[num3 + 1] != ' ') ? 1 : 2);
					if (num4 >= num3)
					{
						string text = blockSource.Substring(num3, num4 - num3 + 1).Trim();
						if (!text.StartsWith("url", StringComparison.InvariantCultureIgnoreCase))
							text = text.ToLower();
						AddProperty(propName, text, dictionary);
					}
				}
				num = num2 + 1;
			}
			return dictionary;
		}

		private void AddProperty(string propName, string propValue, Dictionary<string, string> properties)
		{
			propValue = propValue.Replace("!important", string.Empty).Trim();
			if (!(propName == "width") && !(propName == "height") && !(propName == "lineheight"))
			{
				switch (propName)
				{
				default:
					if (!(propName == "borderrightcolor"))
					{
						switch (propName)
						{
						case "border-bottom":
							ParseBorderProperty(propValue, "-bottom", properties);
							break;
						case "margin":
							ParseMarginProperty(propValue, properties);
							break;
						case "border-style":
							ParseBorderStyleProperty(propValue, properties);
							break;
						case "border-width":
							ParseBorderWidthProperty(propValue, properties);
							break;
						case "border-color":
							ParseBorderColorProperty(propValue, properties);
							break;
						case "padding":
							ParsePaddingProperty(propValue, properties);
							break;
						case "background-image":
							properties["background-image"] = ParseImageProperty(propValue);
							break;
						case "content":
							properties["content"] = ParseImageProperty(propValue);
							break;
						case "font-family":
							properties["font-family"] = ParseFontFamilyProperty(propValue);
							break;
						default:
							properties[propName] = propValue;
							break;
						case "border-right":
							ParseBorderProperty(propValue, "-right", properties);
							break;
						case "border-top":
							ParseBorderProperty(propValue, "-top", properties);
							break;
						case "border-left":
							ParseBorderProperty(propValue, "-left", properties);
							break;
						case "border":
							ParseBorderProperty(propValue, null, properties);
							break;
						case "font":
							ParseFontProperty(propValue, properties);
							break;
						}
						return;
					}
					break;
				case "color":
				case "backgroundcolor":
				case "bordertopcolor":
				case "borderbottomcolor":
				case "borderleftcolor":
					break;
				}
				ParseColorProperty(propName, propValue, properties);
			}
			else
				ParseLengthProperty(propName, propValue, properties);
		}

		private static void ParseLengthProperty(string propName, string propValue, Dictionary<string, string> properties)
		{
			if (CssValueParser.IsValidLength(propValue) || propValue.Equals("auto", StringComparison.OrdinalIgnoreCase))
				properties[propName] = propValue;
		}

		private void ParseColorProperty(string propName, string propValue, Dictionary<string, string> properties)
		{
			if (_valueParser.IsColorValid(propValue))
				properties[propName] = propValue;
		}

		private void ParseFontProperty(string propValue, Dictionary<string, string> properties)
		{
			int position;
			string text = RegexParserUtils.Search("(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)(\\/(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%))?(\\s|$)", propValue, out position);
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Trim();
				string source = propValue.Substring(0, position);
				string value = RegexParserUtils.Search("(normal|italic|oblique)", source);
				string value2 = RegexParserUtils.Search("(normal|small-caps)", source);
				string value3 = RegexParserUtils.Search("(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)", source);
				string text2 = propValue.Substring(position + text.Length);
				string text3 = text2.Trim();
				string value4 = text;
				string value5 = string.Empty;
				if (text.Contains("/") && text.Length > text.IndexOf("/", StringComparison.Ordinal) + 1)
				{
					int num = text.IndexOf("/", StringComparison.Ordinal);
					value4 = text.Substring(0, num);
					value5 = text.Substring(num + 1);
				}
				if (!string.IsNullOrEmpty(text3))
					properties["font-family"] = ParseFontFamilyProperty(text3);
				if (!string.IsNullOrEmpty(value))
					properties["font-style"] = value;
				if (!string.IsNullOrEmpty(value2))
					properties["font-variant"] = value2;
				if (!string.IsNullOrEmpty(value3))
					properties["font-weight"] = value3;
				if (!string.IsNullOrEmpty(value4))
					properties["font-size"] = value4;
				if (!string.IsNullOrEmpty(value5))
					properties["line-height"] = value5;
			}
		}

		private static string ParseImageProperty(string propValue)
		{
			int num = propValue.IndexOf("url(", StringComparison.InvariantCultureIgnoreCase);
			if (num > -1)
			{
				num += 4;
				int num2 = propValue.IndexOf(')', num);
				if (num2 > -1)
				{
					for (num2--; num < num2 && (char.IsWhiteSpace(propValue[num]) || propValue[num] == '\'' || propValue[num] == '"'); num++)
					{
					}
					while (num < num2 && (char.IsWhiteSpace(propValue[num2]) || propValue[num2] == '\'' || propValue[num2] == '"'))
					{
						num2--;
					}
					if (num <= num2)
						return propValue.Substring(num, num2 - num + 1);
				}
			}
			return propValue;
		}

		private string ParseFontFamilyProperty(string propValue)
		{
			int i = 0;
			string text;
			while (true)
			{
				if (i > -1 && i < propValue.Length)
				{
					for (; char.IsWhiteSpace(propValue[i]) || propValue[i] == ',' || propValue[i] == '\'' || propValue[i] == '"'; i++)
					{
					}
					int num = propValue.IndexOf(',', i);
					if (num < 0)
						num = propValue.Length;
					int num2 = num - 1;
					while (char.IsWhiteSpace(propValue[num2]) || propValue[num2] == '\'' || propValue[num2] == '"')
					{
						num2--;
					}
					text = propValue.Substring(i, num2 - i + 1);
					if (_adapter.IsFontExists(text))
						break;
					i = num;
					continue;
				}
				return "inherit";
			}
			return text;
		}

		private void ParseBorderProperty(string propValue, string direction, Dictionary<string, string> properties)
		{
			string width;
			string style;
			string color;
			ParseBorder(propValue, out width, out style, out color);
			if (direction != null)
			{
				if (width != null)
					properties["border" + direction + "-width"] = width;
				if (style != null)
					properties["border" + direction + "-style"] = style;
				if (color != null)
					properties["border" + direction + "-color"] = color;
			}
			else
			{
				if (width != null)
					ParseBorderWidthProperty(width, properties);
				if (style != null)
					ParseBorderStyleProperty(style, properties);
				if (color != null)
					ParseBorderColorProperty(color, properties);
			}
		}

		private static void ParseMarginProperty(string propValue, Dictionary<string, string> properties)
		{
			string left;
			string top;
			string right;
			string bottom;
			SplitMultiDirectionValues(propValue, out left, out top, out right, out bottom);
			if (left != null)
				properties["margin-left"] = left;
			if (top != null)
				properties["margin-top"] = top;
			if (right != null)
				properties["margin-right"] = right;
			if (bottom != null)
				properties["margin-bottom"] = bottom;
		}

		private static void ParseBorderStyleProperty(string propValue, Dictionary<string, string> properties)
		{
			string left;
			string top;
			string right;
			string bottom;
			SplitMultiDirectionValues(propValue, out left, out top, out right, out bottom);
			if (left != null)
				properties["border-left-style"] = left;
			if (top != null)
				properties["border-top-style"] = top;
			if (right != null)
				properties["border-right-style"] = right;
			if (bottom != null)
				properties["border-bottom-style"] = bottom;
		}

		private static void ParseBorderWidthProperty(string propValue, Dictionary<string, string> properties)
		{
			string left;
			string top;
			string right;
			string bottom;
			SplitMultiDirectionValues(propValue, out left, out top, out right, out bottom);
			if (left != null)
				properties["border-left-width"] = left;
			if (top != null)
				properties["border-top-width"] = top;
			if (right != null)
				properties["border-right-width"] = right;
			if (bottom != null)
				properties["border-bottom-width"] = bottom;
		}

		private static void ParseBorderColorProperty(string propValue, Dictionary<string, string> properties)
		{
			string left;
			string top;
			string right;
			string bottom;
			SplitMultiDirectionValues(propValue, out left, out top, out right, out bottom);
			if (left != null)
				properties["border-left-color"] = left;
			if (top != null)
				properties["border-top-color"] = top;
			if (right != null)
				properties["border-right-color"] = right;
			if (bottom != null)
				properties["border-bottom-color"] = bottom;
		}

		private static void ParsePaddingProperty(string propValue, Dictionary<string, string> properties)
		{
			string left;
			string top;
			string right;
			string bottom;
			SplitMultiDirectionValues(propValue, out left, out top, out right, out bottom);
			if (left != null)
				properties["padding-left"] = left;
			if (top != null)
				properties["padding-top"] = top;
			if (right != null)
				properties["padding-right"] = right;
			if (bottom != null)
				properties["padding-bottom"] = bottom;
		}

		private static void SplitMultiDirectionValues(string propValue, out string left, out string top, out string right, out string bottom)
		{
			top = null;
			left = null;
			right = null;
			bottom = null;
			string[] array = SplitValues(propValue);
			switch (array.Length)
			{
			case 1:
				top = (left = (right = (bottom = array[0])));
				break;
			case 2:
				top = (bottom = array[0]);
				left = (right = array[1]);
				break;
			case 3:
				top = array[0];
				left = (right = array[1]);
				bottom = array[2];
				break;
			case 4:
				top = array[0];
				right = array[1];
				bottom = array[2];
				left = array[3];
				break;
			}
		}

		private static string[] SplitValues(string value, char separator = ' ')
		{
			if (!string.IsNullOrEmpty(value))
			{
				string[] array = value.Split(separator);
				List<string> list = new List<string>();
				string[] array2 = array;
				foreach (string text in array2)
				{
					string text2 = text.Trim();
					if (!string.IsNullOrEmpty(text2))
						list.Add(text2);
				}
				return list.ToArray();
			}
			return new string[0];
		}

		public void ParseBorder(string value, out string width, out string style, out string color)
		{
			width = (style = (color = null));
			if (string.IsNullOrEmpty(value))
				return;
			int idx = 0;
			int length;
			while ((idx = CommonUtils.GetNextSubString(value, idx, out length)) > -1)
			{
				if (width == null)
					width = ParseBorderWidth(value, idx, length);
				if (style == null)
					style = ParseBorderStyle(value, idx, length);
				if (color == null)
					color = ParseBorderColor(value, idx, length);
				idx = idx + length + 1;
			}
		}

		private static string ParseBorderWidth(string str, int idx, int length)
		{
			if ((length > 2 && char.IsDigit(str[idx])) || (length > 3 && str[idx] == '.'))
			{
				string text = null;
				if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "px"))
					text = "px";
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "pt"))
				{
					text = "pt";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "em"))
				{
					text = "em";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "ex"))
				{
					text = "ex";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "in"))
				{
					text = "in";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "cm"))
				{
					text = "cm";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "mm"))
				{
					text = "mm";
				}
				else if (CommonUtils.SubStringEquals(str, idx + length - 2, 2, "pc"))
				{
					text = "pc";
				}
				if (text != null && CssValueParser.IsFloat(str, idx, length - 2))
					return str.Substring(idx, length);
			}
			else
			{
				if (CommonUtils.SubStringEquals(str, idx, length, "thin"))
					return "thin";
				if (CommonUtils.SubStringEquals(str, idx, length, "medium"))
					return "medium";
				if (CommonUtils.SubStringEquals(str, idx, length, "thick"))
					return "thick";
			}
			return null;
		}

		private static string ParseBorderStyle(string str, int idx, int length)
		{
			if (!CommonUtils.SubStringEquals(str, idx, length, "none"))
			{
				if (!CommonUtils.SubStringEquals(str, idx, length, "solid"))
				{
					if (CommonUtils.SubStringEquals(str, idx, length, "hidden"))
						return "hidden";
					if (!CommonUtils.SubStringEquals(str, idx, length, "dotted"))
					{
						if (!CommonUtils.SubStringEquals(str, idx, length, "dashed"))
						{
							if (CommonUtils.SubStringEquals(str, idx, length, "double"))
								return "double";
							if (CommonUtils.SubStringEquals(str, idx, length, "groove"))
								return "groove";
							if (CommonUtils.SubStringEquals(str, idx, length, "ridge"))
								return "ridge";
							if (CommonUtils.SubStringEquals(str, idx, length, "inset"))
								return "inset";
							if (CommonUtils.SubStringEquals(str, idx, length, "outset"))
								return "outset";
							return null;
						}
						return "dashed";
					}
					return "dotted";
				}
				return "solid";
			}
			return "none";
		}

		private string ParseBorderColor(string str, int idx, int length)
		{
			RColor color;
			return _valueParser.TryGetColor(str, idx, length, out color) ? str.Substring(idx, length) : null;
		}
	}
}
