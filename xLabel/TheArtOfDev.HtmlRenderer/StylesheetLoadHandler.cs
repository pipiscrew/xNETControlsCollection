using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal static class StylesheetLoadHandler
	{
		public static void LoadStylesheet(HtmlContainerInt htmlContainer, string src, Dictionary<string, string> attributes, out string stylesheet, out CssData stylesheetData)
		{
			ArgChecker.AssertArgNotNull(htmlContainer, "htmlContainer");
			stylesheet = null;
			stylesheetData = null;
			try
			{
				HtmlStylesheetLoadEventArgs htmlStylesheetLoadEventArgs = new HtmlStylesheetLoadEventArgs(src, attributes);
				htmlContainer.RaiseHtmlStylesheetLoadEvent(htmlStylesheetLoadEventArgs);
				if (!string.IsNullOrEmpty(htmlStylesheetLoadEventArgs.SetStyleSheet))
					stylesheet = htmlStylesheetLoadEventArgs.SetStyleSheet;
				else if (htmlStylesheetLoadEventArgs.SetStyleSheetData != null)
				{
					stylesheetData = htmlStylesheetLoadEventArgs.SetStyleSheetData;
				}
				else if (htmlStylesheetLoadEventArgs.SetSrc != null)
				{
					stylesheet = LoadStylesheet(htmlContainer, htmlStylesheetLoadEventArgs.SetSrc);
				}
				else
				{
					stylesheet = LoadStylesheet(htmlContainer, src);
				}
			}
			catch (Exception exception)
			{
				htmlContainer.ReportError(HtmlRenderErrorType.CssParsing, "Exception in handling stylesheet source", exception);
			}
		}

		private static string LoadStylesheet(HtmlContainerInt htmlContainer, string src)
		{
			Uri uri = CommonUtils.TryGetUri(src);
			if (uri == null || uri.Scheme == "file")
				return LoadStylesheetFromFile(htmlContainer, (uri != null) ? uri.AbsolutePath : src);
			return LoadStylesheetFromUri(htmlContainer, uri);
		}

		private static string LoadStylesheetFromFile(HtmlContainerInt htmlContainer, string path)
		{
			FileInfo fileInfo = CommonUtils.TryGetFileInfo(path);
			if (fileInfo != null)
			{
				if (fileInfo.Exists)
				{
					using (StreamReader streamReader = new StreamReader(fileInfo.FullName))
						return streamReader.ReadToEnd();
				}
				htmlContainer.ReportError(HtmlRenderErrorType.CssParsing, "No stylesheet found by path: " + path);
			}
			else
				htmlContainer.ReportError(HtmlRenderErrorType.CssParsing, "Failed load image, invalid source: " + path);
			return string.Empty;
		}

		private static string LoadStylesheetFromUri(HtmlContainerInt htmlContainer, Uri uri)
		{
			using (WebClient webClient = new WebClient())
			{
				string text = webClient.DownloadString(uri);
				try
				{
					text = CorrectRelativeUrls(text, uri);
				}
				catch (Exception exception)
				{
					htmlContainer.ReportError(HtmlRenderErrorType.CssParsing, "Error in correcting relative URL in loaded stylesheet", exception);
				}
				return text;
			}
		}

		private static string CorrectRelativeUrls(string stylesheet, Uri baseUri)
		{
			int num = 0;
			while (num >= 0 && num < stylesheet.Length)
			{
				num = stylesheet.IndexOf("url(", num, StringComparison.OrdinalIgnoreCase);
				if (num < 0)
					continue;
				int num2 = stylesheet.IndexOf(')', num);
				if (num2 > num + 4)
				{
					int num3 = 4 + ((stylesheet[num + 4] == '\'') ? 1 : 0);
					int num4 = ((stylesheet[num2 - 1] == '\'') ? 1 : 0);
					string uriString = stylesheet.Substring(num + num3, num2 - num - num3 - num4);
					Uri result;
					if (Uri.TryCreate(uriString, UriKind.Relative, out result))
					{
						result = new Uri(baseUri, result);
						stylesheet = stylesheet.Remove(num + 4, num2 - num - 4);
						stylesheet = stylesheet.Insert(num + 4, result.AbsoluteUri);
						num += result.AbsoluteUri.Length + 4;
					}
					else
						num = num2 + 1;
				}
				else
					num += 4;
			}
			return stylesheet;
		}
	}
}
