using System;
using System.Text;
using System.Windows.Forms;

namespace TheArtOfDev.HtmlRenderer.WinForms.Utilities
{
	internal static class ClipboardHelper
	{
		private const string Header = "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4";

		public const string StartFragment = "<!--StartFragment-->";

		public const string EndFragment = "<!--EndFragment-->";

		private static readonly char[] _byteCount = new char[1];

		public static DataObject CreateDataObject(string html, string plainText)
		{
			html = html ?? string.Empty;
			string text = GetHtmlDataString(html);
			if (Environment.Version.Major < 4 && html.Length != Encoding.UTF8.GetByteCount(html))
				text = Encoding.Default.GetString(Encoding.UTF8.GetBytes(text));
			DataObject dataObject = new DataObject();
			dataObject.SetData(DataFormats.Html, text);
			dataObject.SetData(DataFormats.Text, plainText);
			dataObject.SetData(DataFormats.UnicodeText, plainText);
			return dataObject;
		}

		public static void CopyToClipboard(string html, string plainText)
		{
			DataObject data = CreateDataObject(html, plainText);
			Clipboard.SetDataObject(data, true);
		}

		public static void CopyToClipboard(string plainText)
		{
			DataObject dataObject = new DataObject();
			dataObject.SetData(DataFormats.Text, plainText);
			dataObject.SetData(DataFormats.UnicodeText, plainText);
			Clipboard.SetDataObject(dataObject, true);
		}

		private static string GetHtmlDataString(string html)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4");
			stringBuilder.AppendLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">");
			int num = html.IndexOf("<!--StartFragment-->", StringComparison.OrdinalIgnoreCase);
			int num2 = html.LastIndexOf("<!--EndFragment-->", StringComparison.OrdinalIgnoreCase);
			int num3 = html.IndexOf("<html", StringComparison.OrdinalIgnoreCase);
			int num4 = ((num3 > -1) ? (html.IndexOf('>', num3) + 1) : (-1));
			int num5 = html.LastIndexOf("</html", StringComparison.OrdinalIgnoreCase);
			int num9;
			int num12;
			if (num < 0 && num2 < 0)
			{
				int num6 = html.IndexOf("<body", StringComparison.OrdinalIgnoreCase);
				int num7 = ((num6 > -1) ? (html.IndexOf('>', num6) + 1) : (-1));
				if (num4 >= 0 || num7 >= 0)
				{
					int num8 = html.LastIndexOf("</body", StringComparison.OrdinalIgnoreCase);
					if (num4 < 0)
						stringBuilder.Append("<html>");
					else
						stringBuilder.Append(html, 0, num4);
					if (num7 > -1)
						stringBuilder.Append(html, (num4 > -1) ? num4 : 0, num7 - ((num4 > -1) ? num4 : 0));
					stringBuilder.Append("<!--StartFragment-->");
					num9 = GetByteCount(stringBuilder);
					int num10 = ((num7 > -1) ? num7 : ((num4 > -1) ? num4 : 0));
					int num11 = ((num8 > -1) ? num8 : ((num5 > -1) ? num5 : html.Length));
					stringBuilder.Append(html, num10, num11 - num10);
					num12 = GetByteCount(stringBuilder);
					stringBuilder.Append("<!--EndFragment-->");
					if (num11 < html.Length)
						stringBuilder.Append(html, num11, html.Length - num11);
					if (num5 < 0)
						stringBuilder.Append("</html>");
				}
				else
				{
					stringBuilder.Append("<html><body>");
					stringBuilder.Append("<!--StartFragment-->");
					num9 = GetByteCount(stringBuilder);
					stringBuilder.Append(html);
					num12 = GetByteCount(stringBuilder);
					stringBuilder.Append("<!--EndFragment-->");
					stringBuilder.Append("</body></html>");
				}
			}
			else
			{
				if (num4 < 0)
					stringBuilder.Append("<html>");
				int byteCount = GetByteCount(stringBuilder);
				stringBuilder.Append(html);
				num9 = byteCount + GetByteCount(stringBuilder, byteCount, byteCount + num) + "<!--StartFragment-->".Length;
				num12 = byteCount + GetByteCount(stringBuilder, byteCount, byteCount + num2);
				if (num5 < 0)
					stringBuilder.Append("</html>");
			}
			stringBuilder.Replace("<<<<<<<<1", "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4".Length.ToString("D9"), 0, "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4".Length);
			stringBuilder.Replace("<<<<<<<<2", GetByteCount(stringBuilder).ToString("D9"), 0, "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4".Length);
			stringBuilder.Replace("<<<<<<<<3", num9.ToString("D9"), 0, "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4".Length);
			stringBuilder.Replace("<<<<<<<<4", num12.ToString("D9"), 0, "Version:0.9\nStartHTML:<<<<<<<<1\nEndHTML:<<<<<<<<2\nStartFragment:<<<<<<<<3\nEndFragment:<<<<<<<<4\nStartSelection:<<<<<<<<3\nEndSelection:<<<<<<<<4".Length);
			return stringBuilder.ToString();
		}

		private static int GetByteCount(StringBuilder sb, int start = 0, int end = -1)
		{
			int num = 0;
			end = ((end > -1) ? end : sb.Length);
			for (int i = start; i < end; i++)
			{
				_byteCount[0] = sb[i];
				num += Encoding.UTF8.GetByteCount(_byteCount);
			}
			return num;
		}
	}
}
