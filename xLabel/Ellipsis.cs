using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace xCollection
{
	[Browsable(false)]
	[DebuggerStepThrough]
	public static class Ellipsis
	{
		[Flags]
		public enum EllipsisFormat
		{
			None = 0,
			End = 1,
			Start = 2,
			Middle = 3,
			Path = 4,
			Word = 8
		}

		public static readonly string EllipsisChars = "...";

		private static Regex prevWord = new Regex("\\W*\\w*$");

		private static Regex nextWord = new Regex("\\w*\\W*");

		public static string Compact(string text, Control control, EllipsisFormat options)
		{
			if (string.IsNullOrEmpty(text))
				return text;
			if ((EllipsisFormat.Middle & options) == 0)
				return text;
			if (control == null)
				throw new ArgumentNullException("ctrl");
			using (Graphics dc = control.CreateGraphics())
			{
				if (TextRenderer.MeasureText(dc, text, control.Font).Width <= control.Width)
					return text;
				string text2 = "";
				string text3 = text;
				string text4 = "";
				bool flag;
				if (flag = (EllipsisFormat.Path & options) != 0)
				{
					text2 = Path.GetPathRoot(text);
					text3 = Path.GetDirectoryName(text).Substring(text2.Length);
					text4 = Path.GetFileName(text);
				}
				int num = 0;
				int num2 = text3.Length;
				string text5 = "";
				while (num2 > 1)
				{
					num2 -= num2 / 2;
					int num3 = num + num2;
					int num4 = text3.Length;
					if (num3 > num4)
						continue;
					if ((EllipsisFormat.Middle & options) == EllipsisFormat.Middle)
					{
						num4 -= num3 / 2;
						num3 -= num3 / 2;
					}
					else if ((EllipsisFormat.Start & options) != 0)
					{
						num4 -= num3;
						num3 = 0;
					}
					if ((EllipsisFormat.Word & options) != 0)
					{
						if ((EllipsisFormat.End & options) != 0)
							num3 -= prevWord.Match(text3, 0, num3).Length;
						if ((EllipsisFormat.Start & options) != 0)
							num4 += nextWord.Match(text3, num4).Length;
					}
					string text6 = text3.Substring(0, num3) + EllipsisChars + text3.Substring(num4);
					if (flag)
						text6 = Path.Combine(Path.Combine(text2, text6), text4);
					if (TextRenderer.MeasureText(dc, text6, control.Font).Width <= control.Width)
					{
						num += num2;
						text5 = text6;
					}
				}
				if (num == 0)
				{
					if (!flag)
						return EllipsisChars;
					if (text2.Length == 0 && text3.Length == 0)
						return text4;
					text5 = Path.Combine(Path.Combine(text2, EllipsisChars), text4);
					if (TextRenderer.MeasureText(dc, text5, control.Font).Width > control.Width)
						text5 = Path.Combine(EllipsisChars, text4);
				}
				return text5;
			}
		}
	}
}
