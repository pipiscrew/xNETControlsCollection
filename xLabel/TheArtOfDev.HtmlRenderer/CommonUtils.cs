using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal static class CommonUtils
	{
		private static readonly string[,] _romanDigitsTable = new string[4, 10]
		{
			{ "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" },
			{ "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" },
			{ "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" },
			{ "", "M", "MM", "MMM", "M(V)", "(V)", "(V)M", "(V)MM", "(V)MMM", "M(X)" }
		};

		private static readonly string[,] _hebrewDigitsTable = new string[3, 9]
		{
			{ "א", "ב", "ג", "ד", "ה", "ו", "ז", "ח", "ט" },
			{ "י", "כ", "ל", "מ", "נ", "ס", "ע", "פ", "צ" },
			{ "ק", "ר", "ש", "ת", "תק", "תר", "תש", "תת", "תתק" }
		};

		private static readonly string[,] _georgianDigitsTable = new string[3, 9]
		{
			{ "ა", "ბ", "გ", "დ", "ე", "ვ", "ზ", "ჱ", "თ" },
			{ "ი", "პ", "ლ", "მ", "ნ", "ჲ", "ო", "პ", "ჟ" },
			{ "რ", "ს", "ტ", "ჳ", "ფ", "ქ", "ღ", "ყ", "შ" }
		};

		private static readonly string[,] _armenianDigitsTable = new string[3, 9]
		{
			{ "Ա", "Բ", "Գ", "Դ", "Ե", "Զ", "Է", "Ը", "Թ" },
			{ "Ժ", "Ի", "Լ", "Խ", "Ծ", "Կ", "Հ", "Ձ", "Ղ" },
			{ "Ճ", "Մ", "Յ", "Ն", "Շ", "Ո", "Չ", "Պ", "Ջ" }
		};

		private static readonly string[] _hiraganaDigitsTable = new string[48]
		{
			"あ", "ぃ", "ぅ", "ぇ", "ぉ", "か", "き", "く", "け", "こ",
			"さ", "し", "す", "せ", "そ", "た", "ち", "つ", "て", "と",
			"な", "に", "ぬ", "ね", "の", "は", "ひ", "ふ", "へ", "ほ",
			"ま", "み", "む", "め", "も", "ゃ", "ゅ", "ょ", "ら", "り",
			"る", "れ", "ろ", "ゎ", "ゐ", "ゑ", "を", "ん"
		};

		private static readonly string[] _satakanaDigitsTable = new string[48]
		{
			"ア", "イ", "ウ", "エ", "オ", "カ", "キ", "ク", "ケ", "コ",
			"サ", "シ", "ス", "セ", "ソ", "タ", "チ", "ツ", "テ", "ト",
			"ナ", "ニ", "ヌ", "ネ", "ノ", "ハ", "ヒ", "フ", "ヘ", "ホ",
			"マ", "ミ", "ム", "メ", "モ", "ヤ", "ユ", "ヨ", "ラ", "リ",
			"ル", "レ", "ロ", "ワ", "ヰ", "ヱ", "ヲ", "ン"
		};

		public static string _tempPath;

		public static bool IsAsianCharecter(char ch)
		{
			return ch >= '一' && ch <= '鶴';
		}

		public static bool IsDigit(char ch, bool hex = false)
		{
			return (ch >= '0' && ch <= '9') || (hex && ((ch >= 'a' && ch <= 'f') || (ch >= 'A' && ch <= 'F')));
		}

		public static int ToDigit(char ch, bool hex = false)
		{
			if (ch >= '0' && ch <= '9')
				return ch - 48;
			if (hex)
			{
				if (ch >= 'a' && ch <= 'f')
					return ch - 97 + 10;
				if (ch >= 'A' && ch <= 'F')
					return ch - 65 + 10;
			}
			return 0;
		}

		public static RSize Max(RSize size, RSize other)
		{
			return new RSize(Math.Max(size.Width, other.Width), Math.Max(size.Height, other.Height));
		}

		public static Uri TryGetUri(string path)
		{
			try
			{
				if (Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
					return new Uri(path);
			}
			catch
			{
			}
			return null;
		}

		public static TValue GetFirstValueOrDefault<TKey, TValue>(IDictionary<TKey, TValue> dic, TValue defaultValue = default(TValue))
		{
			if (dic != null)
			{
				using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = dic.GetEnumerator())
				{
					if (enumerator.MoveNext())
						return enumerator.Current.Value;
				}
			}
			return defaultValue;
		}

		public static FileInfo TryGetFileInfo(string path)
		{
			try
			{
				return new FileInfo(path);
			}
			catch
			{
			}
			return null;
		}

		public static string GetResponseContentType(WebClient client)
		{
			foreach (string responseHeader in client.ResponseHeaders)
			{
				if (responseHeader.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
					return client.ResponseHeaders[responseHeader];
			}
			return null;
		}

		public static FileInfo GetLocalfileName(Uri imageUri)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string absoluteUri = imageUri.AbsoluteUri;
			int num = absoluteUri.LastIndexOf('/');
			if (num != -1)
			{
				string text = absoluteUri.Substring(0, num);
				stringBuilder.Append(text.GetHashCode().ToString());
				stringBuilder.Append('_');
				string text2 = absoluteUri.Substring(num + 1);
				int num2 = text2.IndexOf('?');
				if (num2 == -1)
				{
					string value = ".png";
					int num3 = text2.IndexOf('.');
					if (num3 > -1)
					{
						value = text2.Substring(num3);
						text2 = text2.Substring(0, num3);
					}
					stringBuilder.Append(text2);
					stringBuilder.Append(value);
				}
				else
				{
					int num4 = text2.IndexOf('.');
					if (num4 == -1 || num4 > num2)
					{
						stringBuilder.Append(text2);
						stringBuilder.Append(".png");
					}
					else if (num2 > num4)
					{
						stringBuilder.Append(text2, 0, num4);
						stringBuilder.Append(text2, num2, text2.Length - num2);
						stringBuilder.Append(text2, num4, num2 - num4);
					}
				}
				string text3 = GetValidFileName(stringBuilder.ToString());
				if (text3.Length > 25)
					text3 = text3.Substring(0, 24) + text3.Substring(24).GetHashCode() + Path.GetExtension(text3);
				if (_tempPath == null)
				{
					_tempPath = Path.Combine(Path.GetTempPath(), "HtmlRenderer");
					if (!Directory.Exists(_tempPath))
						Directory.CreateDirectory(_tempPath);
				}
				return new FileInfo(Path.Combine(_tempPath, text3));
			}
			return null;
		}

		public static int GetNextSubString(string str, int idx, out int length)
		{
			while (idx < str.Length && char.IsWhiteSpace(str[idx]))
			{
				idx++;
			}
			if (idx < str.Length)
			{
				int i;
				for (i = idx + 1; i < str.Length && !char.IsWhiteSpace(str[i]); i++)
				{
				}
				length = i - idx;
				return idx;
			}
			length = 0;
			return -1;
		}

		public static bool SubStringEquals(string str, int idx, int length, string str2)
		{
			if (length == str2.Length && idx + length <= str.Length)
			{
				for (int i = 0; i < length; i++)
				{
					if (char.ToLowerInvariant(str[idx + i]) != char.ToLowerInvariant(str2[i]))
						return false;
				}
				return true;
			}
			return false;
		}

		private static string GetValidFileName(string source)
		{
			string text = source;
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			char[] array = invalidFileNameChars;
			foreach (char oldChar in array)
			{
				text = text.Replace(oldChar, '_');
			}
			return text;
		}

		public static string ConvertToAlphaNumber(int number, string style = "upper-alpha")
		{
			if (number != 0)
			{
				if (!style.Equals("lower-greek", StringComparison.InvariantCultureIgnoreCase))
				{
					if (style.Equals("lower-roman", StringComparison.InvariantCultureIgnoreCase))
						return ConvertToRomanNumbers(number, true);
					if (!style.Equals("upper-roman", StringComparison.InvariantCultureIgnoreCase))
					{
						if (!style.Equals("armenian", StringComparison.InvariantCultureIgnoreCase))
						{
							if (!style.Equals("georgian", StringComparison.InvariantCultureIgnoreCase))
							{
								if (!style.Equals("hebrew", StringComparison.InvariantCultureIgnoreCase))
								{
									if (style.Equals("hiragana", StringComparison.InvariantCultureIgnoreCase) || style.Equals("hiragana-iroha", StringComparison.InvariantCultureIgnoreCase))
										return ConvertToSpecificNumbers2(number, _hiraganaDigitsTable);
									if (style.Equals("katakana", StringComparison.InvariantCultureIgnoreCase) || style.Equals("katakana-iroha", StringComparison.InvariantCultureIgnoreCase))
										return ConvertToSpecificNumbers2(number, _satakanaDigitsTable);
									bool lowercase = style.Equals("lower-alpha", StringComparison.InvariantCultureIgnoreCase) || style.Equals("lower-latin", StringComparison.InvariantCultureIgnoreCase);
									return ConvertToEnglishNumber(number, lowercase);
								}
								return ConvertToSpecificNumbers(number, _hebrewDigitsTable);
							}
							return ConvertToSpecificNumbers(number, _georgianDigitsTable);
						}
						return ConvertToSpecificNumbers(number, _armenianDigitsTable);
					}
					return ConvertToRomanNumbers(number, false);
				}
				return ConvertToGreekNumber(number);
			}
			return string.Empty;
		}

		private static string ConvertToEnglishNumber(int number, bool lowercase)
		{
			string text = string.Empty;
			int num = (lowercase ? 97 : 65);
			while (number > 0)
			{
				int num2 = number % 26 - 1;
				if (num2 >= 0)
				{
					text = (char)(num + num2) + text;
					number /= 26;
				}
				else
				{
					text = (char)(num + 25) + text;
					number = (number - 1) / 26;
				}
			}
			return text;
		}

		private static string ConvertToGreekNumber(int number)
		{
			string text = string.Empty;
			while (number > 0)
			{
				int num = number % 24 - 1;
				if (num > 16)
					num++;
				if (num >= 0)
				{
					text = (char)(945 + num) + text;
					number /= 24;
				}
				else
				{
					text = "ω" + text;
					number = (number - 1) / 25;
				}
			}
			return text;
		}

		private static string ConvertToRomanNumbers(int number, bool lowercase)
		{
			string text = string.Empty;
			int num = 1000;
			int num2 = 3;
			while (num > 0)
			{
				int num3 = number / num;
				text += string.Format(_romanDigitsTable[num2, num3]);
				number -= num3 * num;
				num /= 10;
				num2--;
			}
			return lowercase ? text.ToLower() : text;
		}

		private static string ConvertToSpecificNumbers(int number, string[,] alphabet)
		{
			int num = 0;
			string text = string.Empty;
			while (number > 0 && num < alphabet.GetLength(0))
			{
				int num2 = number % 10;
				if (num2 > 0)
					text = alphabet[num, number % 10 - 1].ToString(CultureInfo.InvariantCulture) + text;
				number /= 10;
				num++;
			}
			return text;
		}

		private static string ConvertToSpecificNumbers2(int number, string[] alphabet)
		{
			for (int num = 20; num > 0; num--)
			{
				if (number > 49 * num - num + 1)
					number++;
			}
			string text = string.Empty;
			while (number > 0)
			{
				text = alphabet[Math.Max(0, number % 49 - 1)].ToString(CultureInfo.InvariantCulture) + text;
				number /= 49;
			}
			return text;
		}
	}
}
