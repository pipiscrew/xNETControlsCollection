using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TheArtOfDev.HtmlRenderer.Core.Parse
{
	internal static class RegexParserUtils
	{
		public const string CssMediaTypes = "@media[^\\{\\}]*\\{";

		public const string CssBlocks = "[^\\{\\}]*\\{[^\\{\\}]*\\}";

		public const string CssNumber = "{[0-9]+|[0-9]*\\.[0-9]+}";

		public const string CssPercentage = "([0-9]+|[0-9]*\\.[0-9]+)\\%";

		public const string CssLength = "([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)";

		public const string CssLineHeight = "(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%)";

		public const string CssFontFamily = "(\"[^\"]*\"|'[^']*'|\\S+\\s*)(\\s*\\,\\s*(\"[^\"]*\"|'[^']*'|\\S+))*";

		public const string CssFontStyle = "(normal|italic|oblique)";

		public const string CssFontVariant = "(normal|small-caps)";

		public const string CssFontWeight = "(normal|bold|bolder|lighter|100|200|300|400|500|600|700|800|900)";

		public const string CssFontSize = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)";

		public const string CssFontSizeAndLineHeight = "(([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%|xx-small|x-small|small|medium|large|x-large|xx-large|larger|smaller)(\\/(normal|{[0-9]+|[0-9]*\\.[0-9]+}|([0-9]+|[0-9]*\\.[0-9]+)(em|ex|px|in|cm|mm|pt|pc)|([0-9]+|[0-9]*\\.[0-9]+)\\%))?(\\s|$)";

		private static readonly Dictionary<string, Regex> _regexes = new Dictionary<string, Regex>();

		public static string GetCssAtRules(string stylesheet, ref int startIdx)
		{
			startIdx = stylesheet.IndexOf('@', startIdx);
			if (startIdx > -1)
			{
				int num = 1;
				int num2 = stylesheet.IndexOf('{', startIdx);
				if (num2 > -1)
				{
					while (num > 0 && num2 < stylesheet.Length)
					{
						num2++;
						if (stylesheet[num2] == '{')
							num++;
						else if (stylesheet[num2] == '}')
						{
							num--;
						}
					}
					if (num2 < stylesheet.Length)
					{
						string result = stylesheet.Substring(startIdx, num2 - startIdx + 1);
						startIdx = num2;
						return result;
					}
				}
			}
			return null;
		}

		public static MatchCollection Match(string regex, string source)
		{
			Regex regex2 = GetRegex(regex);
			return regex2.Matches(source);
		}

		public static string Search(string regex, string source)
		{
			int position;
			return Search(regex, source, out position);
		}

		public static string Search(string regex, string source, out int position)
		{
			MatchCollection matchCollection = Match(regex, source);
			if (matchCollection.Count > 0)
			{
				position = matchCollection[0].Index;
				return matchCollection[0].Value;
			}
			position = -1;
			return null;
		}

		private static Regex GetRegex(string regex)
		{
			Regex value;
			if (!_regexes.TryGetValue(regex, out value))
			{
				value = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Singleline);
				_regexes[regex] = value;
			}
			return value;
		}
	}
}
