using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Parse
{
	internal static class HtmlParser
	{
		public static CssBox ParseDocument(string source)
		{
			CssBox cssBox = CssBox.CreateBlock();
			CssBox curBox = cssBox;
			int num = 0;
			int num2 = 0;
			while (num2 >= 0)
			{
				int num3 = source.IndexOf('<', num2);
				if (num3 >= 0 && num3 < source.Length)
				{
					AddTextBox(source, num2, num3, ref curBox);
					if (source[num3 + 1] == '!')
					{
						if (source[num3 + 2] == '-')
						{
							num2 = source.IndexOf("-->", num3 + 2);
							num = ((num2 > 0) ? (num2 + 3) : (num3 + 2));
						}
						else
						{
							num2 = source.IndexOf(">", num3 + 2);
							num = ((num2 > 0) ? (num2 + 1) : (num3 + 2));
						}
					}
					else
					{
						num = ParseHtmlTag(source, num3, ref curBox) + 1;
						if (curBox.HtmlTag != null && curBox.HtmlTag.Name.Equals("style", StringComparison.OrdinalIgnoreCase))
						{
							int startIdx = num;
							num = source.IndexOf("</style>", num, StringComparison.OrdinalIgnoreCase);
							if (num > -1)
								AddTextBox(source, startIdx, num, ref curBox);
						}
					}
				}
				num2 = ((num3 <= -1 || num <= 0) ? (-1) : num);
			}
			if (num > -1 && num < source.Length)
			{
				SubString subString = new SubString(source, num, source.Length - num);
				if (!subString.IsEmptyOrWhitespace())
				{
					CssBox cssBox2 = CssBox.CreateBox(cssBox);
					cssBox2.Text = subString;
				}
			}
			return cssBox;
		}

		private static void AddTextBox(string source, int startIdx, int tagIdx, ref CssBox curBox)
		{
			SubString subString = ((tagIdx > startIdx) ? new SubString(source, startIdx, tagIdx - startIdx) : null);
			if (subString != null)
			{
				CssBox cssBox = CssBox.CreateBox(curBox);
				cssBox.Text = subString;
			}
		}

		private static int ParseHtmlTag(string source, int tagIdx, ref CssBox curBox)
		{
			int num = source.IndexOf('>', tagIdx + 1);
			if (num > 0)
			{
				int length = num - tagIdx + 1 - ((source[num - 1] == '/') ? 1 : 0);
				string name;
				Dictionary<string, string> attributes;
				if (ParseHtmlTag(source, tagIdx, length, out name, out attributes))
				{
					if (!HtmlUtils.IsSingleTag(name) && curBox.ParentBox != null)
						curBox = DomUtils.FindParent(curBox.ParentBox, name, curBox);
				}
				else if (!string.IsNullOrEmpty(name))
				{
					bool flag = HtmlUtils.IsSingleTag(name) || source[num - 1] == '/';
					HtmlTag tag = new HtmlTag(name, flag, attributes);
					if (!flag)
						curBox = CssBox.CreateBox(tag, curBox);
					else
						CssBox.CreateBox(tag, curBox);
				}
				else
				{
					num = tagIdx + 1;
				}
			}
			return num;
		}

		private static bool ParseHtmlTag(string source, int idx, int length, out string name, out Dictionary<string, string> attributes)
		{
			idx++;
			length -= ((source[idx + length - 3] == '/') ? 3 : 2);
			bool flag = false;
			if (source[idx] == '/')
			{
				idx++;
				length--;
				flag = true;
			}
			int i;
			for (i = idx; i < idx + length && !char.IsWhiteSpace(source, i); i++)
			{
			}
			name = source.Substring(idx, i - idx).ToLower();
			attributes = null;
			if (!flag && idx + length > i)
				ExtractAttributes(source, i, length - (i - idx), out attributes);
			return flag;
		}

		private static void ExtractAttributes(string source, int idx, int length, out Dictionary<string, string> attributes)
		{
			attributes = null;
			int i = idx;
			while (i < idx + length)
			{
				for (; i < idx + length && char.IsWhiteSpace(source, i); i++)
				{
				}
				int j;
				for (j = i + 1; j < idx + length && !char.IsWhiteSpace(source, j) && source[j] != '='; j++)
				{
				}
				if (i >= idx + length)
					continue;
				string text = source.Substring(i, j - i);
				string value = "";
				for (i = j + 1; i < idx + length && (char.IsWhiteSpace(source, i) || source[i] == '='); i++)
				{
				}
				bool flag = false;
				if (i < idx + length)
				{
					char c = source[i];
					if (c == '"' || c == '\'')
					{
						flag = true;
						i++;
					}
					for (j = i + ((!flag) ? 1 : 0); j < idx + length && (flag ? (source[j] != c) : (!char.IsWhiteSpace(source, j))); j++)
					{
					}
					value = source.Substring(i, j - i);
					value = HtmlUtils.DecodeHtml(value);
				}
				if (text.Length != 0)
				{
					if (attributes == null)
						attributes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
					attributes[text.ToLower()] = value;
				}
				i = j + ((!flag) ? 1 : 2);
			}
		}
	}
}
