using System;
using System.Net;
using System.Text;
using System.Threading;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssBoxFrame : CssBox
	{
		private readonly CssRectImage _imageWord;

		private readonly bool _isVideo;

		private string _videoTitle;

		private string _videoImageUrl;

		private string _videoLinkUrl;

		private ImageLoadHandler _imageLoadHandler;

		private bool _imageLoadingComplete;

		public override bool IsClickable
		{
			get
			{
				return true;
			}
		}

		public override string HrefLink
		{
			get
			{
				return _videoLinkUrl ?? GetAttribute("src");
			}
		}

		public bool IsVideo
		{
			get
			{
				return _isVideo;
			}
		}

		public CssBoxFrame(CssBox parent, HtmlTag tag)
			: base(parent, tag)
		{
			_imageWord = new CssRectImage(this);
			base.Words.Add(_imageWord);
			Uri result;
			if (Uri.TryCreate(GetAttribute("src"), UriKind.Absolute, out result))
			{
				if (result.Host.IndexOf("youtube.com", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					_isVideo = true;
					LoadYoutubeDataAsync(result);
				}
				else if (result.Host.IndexOf("vimeo.com", StringComparison.InvariantCultureIgnoreCase) > -1)
				{
					_isVideo = true;
					LoadVimeoDataAsync(result);
				}
			}
			if (!_isVideo)
				SetErrorBorder();
		}

		public override void Dispose()
		{
			if (_imageLoadHandler != null)
				_imageLoadHandler.Dispose();
			base.Dispose();
		}

		private void LoadYoutubeDataAsync(Uri uri)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					Uri address = new Uri(string.Format("http://gdata.youtube.com/feeds/api/videos/{0}?v=2&alt=json", uri.Segments[2]));
					WebClient webClient = new WebClient();
					webClient.Encoding = Encoding.UTF8;
					webClient.DownloadStringCompleted += OnDownloadYoutubeApiCompleted;
					webClient.DownloadStringAsync(address);
				}
				catch (Exception exception)
				{
					HtmlContainerInt htmlContainer = base.HtmlContainer;
					Uri uri2 = uri;
					htmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to get youtube video data: " + (((object)uri2 != null) ? uri2.ToString() : null), exception);
					base.HtmlContainer.RequestRefresh(false);
				}
			});
		}

		private void OnDownloadYoutubeApiCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					if (e.Error == null)
					{
						int num = e.Result.IndexOf("\"media$title\"", StringComparison.Ordinal);
						if (num > -1)
						{
							num = e.Result.IndexOf("\"$t\"", num);
							if (num > -1)
							{
								num = e.Result.IndexOf('"', num + 4);
								if (num > -1)
								{
									int num2 = e.Result.IndexOf('"', num + 1);
									while (e.Result[num2 - 1] == '\\')
									{
										num2 = e.Result.IndexOf('"', num2 + 1);
									}
									if (num2 > -1)
										_videoTitle = e.Result.Substring(num + 1, num2 - num - 1).Replace("\\\"", "\"");
								}
							}
						}
						num = e.Result.IndexOf("\"media$thumbnail\"", StringComparison.Ordinal);
						if (num > -1)
						{
							int num3 = e.Result.IndexOf("sddefault", num);
							if (num3 > -1)
							{
								if (string.IsNullOrEmpty(base.Width))
									base.Width = "640px";
								if (string.IsNullOrEmpty(base.Height))
									base.Height = "480px";
							}
							else
							{
								num3 = e.Result.IndexOf("hqdefault", num);
								if (num3 > -1)
								{
									if (string.IsNullOrEmpty(base.Width))
										base.Width = "480px";
									if (string.IsNullOrEmpty(base.Height))
										base.Height = "360px";
								}
								else
								{
									num3 = e.Result.IndexOf("mqdefault", num);
									if (num3 > -1)
									{
										if (string.IsNullOrEmpty(base.Width))
											base.Width = "320px";
										if (string.IsNullOrEmpty(base.Height))
											base.Height = "180px";
									}
									else
									{
										num3 = e.Result.IndexOf("default", num);
										if (string.IsNullOrEmpty(base.Width))
											base.Width = "120px";
										if (string.IsNullOrEmpty(base.Height))
											base.Height = "90px";
									}
								}
							}
							num3 = e.Result.LastIndexOf("http:", num3, StringComparison.Ordinal);
							if (num3 > -1)
							{
								int num4 = e.Result.IndexOf('"', num3);
								if (num4 > -1)
									_videoImageUrl = e.Result.Substring(num3, num4 - num3).Replace("\\\"", "\"").Replace("\\", "");
							}
						}
						num = e.Result.IndexOf("\"link\"", StringComparison.Ordinal);
						if (num > -1)
						{
							num = e.Result.IndexOf("http:", num);
							if (num > -1)
							{
								int num5 = e.Result.IndexOf('"', num);
								if (num5 > -1)
									_videoLinkUrl = e.Result.Substring(num, num5 - num).Replace("\\\"", "\"").Replace("\\", "");
							}
						}
					}
					else
						HandleDataLoadFailure(e.Error, "YouTube");
				}
			}
			catch (Exception exception)
			{
				base.HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to parse YouTube video response", exception);
			}
			HandlePostApiCall(sender);
		}

		private void LoadVimeoDataAsync(Uri uri)
		{
			ThreadPool.QueueUserWorkItem(delegate
			{
				try
				{
					Uri address = new Uri(string.Format("http://vimeo.com/api/v2/video/{0}.json", uri.Segments[2]));
					WebClient webClient = new WebClient();
					webClient.Encoding = Encoding.UTF8;
					webClient.DownloadStringCompleted += OnDownloadVimeoApiCompleted;
					webClient.DownloadStringAsync(address);
				}
				catch (Exception exception)
				{
					_imageLoadingComplete = true;
					SetErrorBorder();
					HtmlContainerInt htmlContainer = base.HtmlContainer;
					Uri uri2 = uri;
					htmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to get vimeo video data: " + (((object)uri2 != null) ? uri2.ToString() : null), exception);
					base.HtmlContainer.RequestRefresh(false);
				}
			});
		}

		private void OnDownloadVimeoApiCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			try
			{
				if (!e.Cancelled)
				{
					if (e.Error == null)
					{
						int num = e.Result.IndexOf("\"title\"", StringComparison.Ordinal);
						if (num > -1)
						{
							num = e.Result.IndexOf('"', num + 7);
							if (num > -1)
							{
								int num2 = e.Result.IndexOf('"', num + 1);
								while (e.Result[num2 - 1] == '\\')
								{
									num2 = e.Result.IndexOf('"', num2 + 1);
								}
								if (num2 > -1)
									_videoTitle = e.Result.Substring(num + 1, num2 - num - 1).Replace("\\\"", "\"");
							}
						}
						num = e.Result.IndexOf("\"thumbnail_large\"", StringComparison.Ordinal);
						if (num > -1)
						{
							if (string.IsNullOrEmpty(base.Width))
								base.Width = "640";
							if (string.IsNullOrEmpty(base.Height))
								base.Height = "360";
						}
						else
						{
							num = e.Result.IndexOf("thumbnail_medium", num);
							if (num > -1)
							{
								if (string.IsNullOrEmpty(base.Width))
									base.Width = "200";
								if (string.IsNullOrEmpty(base.Height))
									base.Height = "150";
							}
							else
							{
								num = e.Result.IndexOf("thumbnail_small", num);
								if (string.IsNullOrEmpty(base.Width))
									base.Width = "100";
								if (string.IsNullOrEmpty(base.Height))
									base.Height = "75";
							}
						}
						if (num > -1)
						{
							num = e.Result.IndexOf("http:", num);
							if (num > -1)
							{
								int num3 = e.Result.IndexOf('"', num);
								if (num3 > -1)
									_videoImageUrl = e.Result.Substring(num, num3 - num).Replace("\\\"", "\"").Replace("\\", "");
							}
						}
						num = e.Result.IndexOf("\"url\"", StringComparison.Ordinal);
						if (num > -1)
						{
							num = e.Result.IndexOf("http:", num);
							if (num > -1)
							{
								int num4 = e.Result.IndexOf('"', num);
								if (num4 > -1)
									_videoLinkUrl = e.Result.Substring(num, num4 - num).Replace("\\\"", "\"").Replace("\\", "");
							}
						}
					}
					else
						HandleDataLoadFailure(e.Error, "Vimeo");
				}
			}
			catch (Exception exception)
			{
				base.HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to parse Vimeo video response", exception);
			}
			HandlePostApiCall(sender);
		}

		private void HandleDataLoadFailure(Exception ex, string source)
		{
			WebException ex2 = ex as WebException;
			HttpWebResponse httpWebResponse = ((ex2 != null) ? (ex2.Response as HttpWebResponse) : null);
			if (httpWebResponse != null && httpWebResponse.StatusCode == HttpStatusCode.NotFound)
				_videoTitle = "The video is not found, possibly removed by the user.";
			else
				base.HtmlContainer.ReportError(HtmlRenderErrorType.Iframe, "Failed to load " + source + " video data", ex);
		}

		private void HandlePostApiCall(object sender)
		{
			try
			{
				if (_videoImageUrl == null)
				{
					_imageLoadingComplete = true;
					SetErrorBorder();
				}
				WebClient webClient = (WebClient)sender;
				webClient.DownloadStringCompleted -= OnDownloadYoutubeApiCompleted;
				webClient.DownloadStringCompleted -= OnDownloadVimeoApiCompleted;
				webClient.Dispose();
				base.HtmlContainer.RequestRefresh(IsLayoutRequired());
			}
			catch
			{
			}
		}

		protected override void PaintImp(RGraphics g)
		{
			if (_videoImageUrl != null && _imageLoadHandler == null)
			{
				_imageLoadHandler = new ImageLoadHandler(base.HtmlContainer, OnLoadImageComplete);
				_imageLoadHandler.LoadImage(_videoImageUrl, (base.HtmlTag != null) ? base.HtmlTag.Attributes : null);
			}
			RRect firstValueOrDefault = CommonUtils.GetFirstValueOrDefault(base.Rectangles);
			RPoint rPoint = ((base.HtmlContainer == null || IsFixed) ? RPoint.Empty : base.HtmlContainer.ScrollOffset);
			firstValueOrDefault.Offset(rPoint);
			bool flag = RenderUtils.ClipGraphicsByOverflow(g, this);
			PaintBackground(g, firstValueOrDefault, true, true);
			BordersDrawHandler.DrawBoxBorders(g, this, firstValueOrDefault, true, true);
			CssRect cssRect = base.Words[0];
			RRect rectangle = cssRect.Rectangle;
			rectangle.Offset(rPoint);
			rectangle.Height -= base.ActualBorderTopWidth + base.ActualBorderBottomWidth + base.ActualPaddingTop + base.ActualPaddingBottom;
			rectangle.Y += base.ActualBorderTopWidth + base.ActualPaddingTop;
			rectangle.X = Math.Floor(rectangle.X);
			rectangle.Y = Math.Floor(rectangle.Y);
			RRect rect = rectangle;
			DrawImage(g, rPoint, rect);
			DrawTitle(g, rect);
			DrawPlay(g, rect);
			if (flag)
				g.PopClip();
		}

		private void DrawImage(RGraphics g, RPoint offset, RRect rect)
		{
			if (_imageWord.Image != null)
			{
				if (rect.Width > 0.0 && rect.Height > 0.0)
				{
					if (_imageWord.ImageRectangle == RRect.Empty)
						g.DrawImage(_imageWord.Image, rect);
					else
						g.DrawImage(_imageWord.Image, rect, _imageWord.ImageRectangle);
					if (_imageWord.Selected)
						g.DrawRectangle(GetSelectionBackBrush(g, true), _imageWord.Left + offset.X, _imageWord.Top + offset.Y, _imageWord.Width + 2.0, DomUtils.GetCssLineBoxByWord(_imageWord).LineHeight);
				}
			}
			else if (_isVideo && !_imageLoadingComplete)
			{
				RenderUtils.DrawImageLoadingIcon(g, base.HtmlContainer, rect);
				if (rect.Width > 19.0 && rect.Height > 19.0)
					g.DrawRectangle(g.GetPen(RColor.LightGray), rect.X, rect.Y, rect.Width, rect.Height);
			}
		}

		private void DrawTitle(RGraphics g, RRect rect)
		{
			if (_videoTitle != null && _imageWord.Width > 40.0 && _imageWord.Height > 40.0)
			{
				RFont font = base.HtmlContainer.Adapter.GetFont("Arial", 9.0, RFontStyle.Regular);
				g.DrawRectangle(g.GetSolidBrush(RColor.FromArgb(160, 0, 0, 0)), rect.Left, rect.Top, rect.Width, base.ActualFont.Height + 7.0);
				g.DrawString(point: new RRect(rect.Left + 3.0, rect.Top + 3.0, rect.Width - 6.0, rect.Height - 6.0).Location, str: _videoTitle, font: font, color: RColor.WhiteSmoke, size: RSize.Empty, rtl: false);
			}
		}

		private void DrawPlay(RGraphics g, RRect rect)
		{
			if (_isVideo && _imageWord.Width > 70.0 && _imageWord.Height > 50.0)
			{
				object prevMode = g.SetAntiAliasSmoothingMode();
				RSize rSize = new RSize(60.0, 40.0);
				double num = rect.Left + (rect.Width - rSize.Width) / 2.0;
				double num2 = rect.Top + (rect.Height - rSize.Height) / 2.0;
				g.DrawRectangle(g.GetSolidBrush(RColor.FromArgb(160, 0, 0, 0)), num, num2, rSize.Width, rSize.Height);
				RPoint[] points = new RPoint[3]
				{
					new RPoint(num + rSize.Width / 3.0 + 1.0, num2 + 3.0 * rSize.Height / 4.0),
					new RPoint(num + rSize.Width / 3.0 + 1.0, num2 + rSize.Height / 4.0),
					new RPoint(num + 2.0 * rSize.Width / 3.0 + 1.0, num2 + rSize.Height / 2.0)
				};
				g.DrawPolygon(g.GetSolidBrush(RColor.White), points);
				g.ReturnPreviousSmoothingMode(prevMode);
			}
		}

		internal override void MeasureWordsSize(RGraphics g)
		{
			if (!_wordsSizeMeasured)
			{
				MeasureWordSpacing(g);
				_wordsSizeMeasured = true;
			}
			CssLayoutEngine.MeasureImageSize(_imageWord);
		}

		private void SetErrorBorder()
		{
			SetAllBorders("solid", "2px", "#A0A0A0");
			string text3 = (base.BorderRightColor = (base.BorderBottomColor = "#E3E3E3"));
		}

		private void OnLoadImageComplete(RImage image, RRect rectangle, bool async)
		{
			_imageWord.Image = image;
			_imageWord.ImageRectangle = rectangle;
			_imageLoadingComplete = true;
			_wordsSizeMeasured = false;
			if (_imageLoadingComplete && image == null)
				SetErrorBorder();
			if (async)
				base.HtmlContainer.RequestRefresh(IsLayoutRequired());
		}

		private bool IsLayoutRequired()
		{
			CssLength cssLength = new CssLength(base.Width);
			CssLength cssLength2 = new CssLength(base.Height);
			return cssLength.Number <= 0.0 || cssLength.Unit != CssUnit.Pixels || cssLength2.Number <= 0.0 || cssLength2.Unit != CssUnit.Pixels;
		}
	}
}
