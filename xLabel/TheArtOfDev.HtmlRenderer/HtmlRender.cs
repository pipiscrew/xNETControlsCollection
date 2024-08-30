using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WinForms.Adapters;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms
{
	public static class HtmlRender
	{
		public static void AddFontFamily(FontFamily fontFamily)
		{
			ArgChecker.AssertArgNotNull(fontFamily, "fontFamily");
			WinFormsAdapter.Instance.AddFontFamily(new FontFamilyAdapter(fontFamily));
		}

		public static void AddFontFamilyMapping(string fromFamily, string toFamily)
		{
			ArgChecker.AssertArgNotNullOrEmpty(fromFamily, "fromFamily");
			ArgChecker.AssertArgNotNullOrEmpty(toFamily, "toFamily");
			WinFormsAdapter.Instance.AddFontFamilyMapping(fromFamily, toFamily);
		}

		public static CssData ParseStyleSheet(string stylesheet, bool combineWithDefault = true)
		{
			return CssData.Parse(WinFormsAdapter.Instance, stylesheet, combineWithDefault);
		}

		public static SizeF Measure(Graphics g, string html, float maxWidth = 0f, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return Measure(g, html, maxWidth, cssData, false, stylesheetLoad, imageLoad);
		}

		public static SizeF MeasureGdiPlus(Graphics g, string html, float maxWidth = 0f, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return Measure(g, html, maxWidth, cssData, true, stylesheetLoad, imageLoad);
		}

		public static SizeF Render(Graphics g, string html, float left = 0f, float top = 0f, float maxWidth = 0f, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return RenderClip(g, html, new PointF(left, top), new SizeF(maxWidth, 0f), cssData, false, stylesheetLoad, imageLoad);
		}

		public static SizeF Render(Graphics g, string html, PointF location, SizeF maxSize, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return RenderClip(g, html, location, maxSize, cssData, false, stylesheetLoad, imageLoad);
		}

		public static SizeF RenderGdiPlus(Graphics g, string html, float left = 0f, float top = 0f, float maxWidth = 0f, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return RenderClip(g, html, new PointF(left, top), new SizeF(maxWidth, 0f), cssData, true, stylesheetLoad, imageLoad);
		}

		public static SizeF RenderGdiPlus(Graphics g, string html, PointF location, SizeF maxSize, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			return RenderClip(g, html, location, maxSize, cssData, true, stylesheetLoad, imageLoad);
		}

		public static Metafile RenderToMetafile(string html, float left = 0f, float top = 0f, float maxWidth = 0f, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			IntPtr dib;
			IntPtr intPtr = Win32Utils.CreateMemoryHdc(IntPtr.Zero, 1, 1, out dib);
			Metafile metafile;
			try
			{
				metafile = new Metafile(intPtr, EmfType.EmfPlusDual, "..");
				using (Graphics g = Graphics.FromImage(metafile))
					Render(g, html, left, top, maxWidth, cssData, stylesheetLoad, imageLoad);
			}
			finally
			{
				Win32Utils.ReleaseMemoryHdc(intPtr, dib);
			}
			return metafile;
		}

		public static void RenderToImage(Image image, string html, PointF location = default(PointF), CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(image, "image");
			SizeF maxSize = new SizeF((float)image.Size.Width - location.X, (float)image.Size.Height - location.Y);
			RenderToImage(image, html, location, maxSize, cssData, stylesheetLoad, imageLoad);
		}

		public static void RenderToImage(Image image, string html, PointF location, SizeF maxSize, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			ArgChecker.AssertArgNotNull(image, "image");
			if (string.IsNullOrEmpty(html))
				return;
			IntPtr dib;
			IntPtr intPtr = Win32Utils.CreateMemoryHdc(IntPtr.Zero, image.Width, image.Height, out dib);
			try
			{
				using (Graphics graphics = Graphics.FromHdc(intPtr))
				{
					graphics.DrawImageUnscaled(image, 0, 0);
					RenderHtml(graphics, html, location, maxSize, cssData, false, stylesheetLoad, imageLoad);
				}
				CopyBufferToImage(intPtr, image);
			}
			finally
			{
				Win32Utils.ReleaseMemoryHdc(intPtr, dib);
			}
		}

		public static Image RenderToImage(string html, Size size, Color backgroundColor = default(Color), CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			if (backgroundColor == Color.Transparent)
				throw new ArgumentOutOfRangeException("backgroundColor", "Transparent background in not supported");
			Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
			if (!string.IsNullOrEmpty(html))
			{
				IntPtr dib;
				IntPtr intPtr = Win32Utils.CreateMemoryHdc(IntPtr.Zero, bitmap.Width, bitmap.Height, out dib);
				try
				{
					using (Graphics graphics = Graphics.FromHdc(intPtr))
					{
						graphics.Clear((backgroundColor != Color.Empty) ? backgroundColor : Color.White);
						RenderHtml(graphics, html, PointF.Empty, size, cssData, true, stylesheetLoad, imageLoad);
					}
					CopyBufferToImage(intPtr, bitmap);
				}
				finally
				{
					Win32Utils.ReleaseMemoryHdc(intPtr, dib);
				}
			}
			return bitmap;
		}

		public static Image RenderToImage(string html, int maxWidth = 0, int maxHeight = 0, Color backgroundColor = default(Color), CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			return RenderToImage(html, Size.Empty, new Size(maxWidth, maxHeight), backgroundColor, cssData, stylesheetLoad, imageLoad);
		}

		public static Image RenderToImage(string html, Size minSize, Size maxSize, Color backgroundColor = default(Color), CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			if (backgroundColor == Color.Transparent)
				throw new ArgumentOutOfRangeException("backgroundColor", "Transparent background in not supported");
			if (string.IsNullOrEmpty(html))
				return new Bitmap(0, 0, PixelFormat.Format32bppArgb);
			using (HtmlContainer htmlContainer = new HtmlContainer())
			{
				htmlContainer.AvoidAsyncImagesLoading = true;
				htmlContainer.AvoidImagesLateLoading = true;
				if (stylesheetLoad != null)
					htmlContainer.StylesheetLoad += stylesheetLoad;
				if (imageLoad != null)
					htmlContainer.ImageLoad += imageLoad;
				htmlContainer.SetHtml(html, cssData);
				Size size = MeasureHtmlByRestrictions(htmlContainer, minSize, maxSize);
				htmlContainer.MaxSize = size;
				Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
				IntPtr dib;
				IntPtr intPtr = Win32Utils.CreateMemoryHdc(IntPtr.Zero, bitmap.Width, bitmap.Height, out dib);
				try
				{
					using (Graphics graphics = Graphics.FromHdc(intPtr))
					{
						graphics.Clear((backgroundColor != Color.Empty) ? backgroundColor : Color.White);
						htmlContainer.PerformPaint(graphics);
					}
					CopyBufferToImage(intPtr, bitmap);
				}
				finally
				{
					Win32Utils.ReleaseMemoryHdc(intPtr, dib);
				}
				return bitmap;
			}
		}

		public static Image RenderToImageGdiPlus(string html, Size size, TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.TextRenderingHint = textRenderingHint;
				RenderHtml(graphics, html, PointF.Empty, size, cssData, true, stylesheetLoad, imageLoad);
			}
			return bitmap;
		}

		public static Image RenderToImageGdiPlus(string html, int maxWidth = 0, int maxHeight = 0, TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			return RenderToImageGdiPlus(html, Size.Empty, new Size(maxWidth, maxHeight), textRenderingHint, cssData, stylesheetLoad, imageLoad);
		}

		public static Image RenderToImageGdiPlus(string html, Size minSize, Size maxSize, TextRenderingHint textRenderingHint = TextRenderingHint.AntiAlias, CssData cssData = null, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = null, EventHandler<HtmlImageLoadEventArgs> imageLoad = null)
		{
			if (string.IsNullOrEmpty(html))
				return new Bitmap(0, 0, PixelFormat.Format32bppArgb);
			using (HtmlContainer htmlContainer = new HtmlContainer())
			{
				htmlContainer.AvoidAsyncImagesLoading = true;
				htmlContainer.AvoidImagesLateLoading = true;
				htmlContainer.UseGdiPlusTextRendering = true;
				if (stylesheetLoad != null)
					htmlContainer.StylesheetLoad += stylesheetLoad;
				if (imageLoad != null)
					htmlContainer.ImageLoad += imageLoad;
				htmlContainer.SetHtml(html, cssData);
				Size size = MeasureHtmlByRestrictions(htmlContainer, minSize, maxSize);
				htmlContainer.MaxSize = size;
				Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.TextRenderingHint = textRenderingHint;
					htmlContainer.PerformPaint(graphics);
				}
				return bitmap;
			}
		}

		private static SizeF Measure(Graphics g, string html, float maxWidth, CssData cssData, bool useGdiPlusTextRendering, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad, EventHandler<HtmlImageLoadEventArgs> imageLoad)
		{
			SizeF result = SizeF.Empty;
			if (!string.IsNullOrEmpty(html))
			{
				using (HtmlContainer htmlContainer = new HtmlContainer())
				{
					htmlContainer.MaxSize = new SizeF(maxWidth, 0f);
					htmlContainer.AvoidAsyncImagesLoading = true;
					htmlContainer.AvoidImagesLateLoading = true;
					htmlContainer.UseGdiPlusTextRendering = useGdiPlusTextRendering;
					if (stylesheetLoad != null)
						htmlContainer.StylesheetLoad += stylesheetLoad;
					if (imageLoad != null)
						htmlContainer.ImageLoad += imageLoad;
					htmlContainer.SetHtml(html, cssData);
					htmlContainer.PerformLayout(g);
					result = htmlContainer.ActualSize;
				}
			}
			return result;
		}

		private static Size MeasureHtmlByRestrictions(HtmlContainer htmlContainer, Size minSize, Size maxSize)
		{
			using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
				using (GraphicsAdapter g2 = new GraphicsAdapter(g, htmlContainer.UseGdiPlusTextRendering))
				{
					RSize s = HtmlRendererUtils.MeasureHtmlByRestrictions(g2, htmlContainer.HtmlContainerInt, Utils.Convert(minSize), Utils.Convert(maxSize));
					if (maxSize.Width < 1 && s.Width > 4096.0)
						s.Width = 4096.0;
					return Utils.ConvertRound(s);
				}
		}

		private static SizeF RenderClip(Graphics g, string html, PointF location, SizeF maxSize, CssData cssData, bool useGdiPlusTextRendering, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad, EventHandler<HtmlImageLoadEventArgs> imageLoad)
		{
			Region region = null;
			if (maxSize.Height > 0f)
			{
				region = g.Clip;
				g.SetClip(new RectangleF(location, maxSize));
			}
			SizeF result = RenderHtml(g, html, location, maxSize, cssData, useGdiPlusTextRendering, stylesheetLoad, imageLoad);
			if (region != null)
				g.SetClip(region, CombineMode.Replace);
			return result;
		}

		private static SizeF RenderHtml(Graphics g, string html, PointF location, SizeF maxSize, CssData cssData, bool useGdiPlusTextRendering, EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad, EventHandler<HtmlImageLoadEventArgs> imageLoad)
		{
			SizeF result = SizeF.Empty;
			if (!string.IsNullOrEmpty(html))
			{
				using (HtmlContainer htmlContainer = new HtmlContainer())
				{
					htmlContainer.Location = location;
					htmlContainer.MaxSize = maxSize;
					htmlContainer.AvoidAsyncImagesLoading = true;
					htmlContainer.AvoidImagesLateLoading = true;
					htmlContainer.UseGdiPlusTextRendering = useGdiPlusTextRendering;
					if (stylesheetLoad != null)
						htmlContainer.StylesheetLoad += stylesheetLoad;
					if (imageLoad != null)
						htmlContainer.ImageLoad += imageLoad;
					htmlContainer.SetHtml(html, cssData);
					htmlContainer.PerformLayout(g);
					htmlContainer.PerformPaint(g);
					result = htmlContainer.ActualSize;
				}
			}
			return result;
		}

		private static void CopyBufferToImage(IntPtr memoryHdc, Image image)
		{
			using (Graphics graphics = Graphics.FromImage(image))
			{
				IntPtr hdc = graphics.GetHdc();
				Win32Utils.BitBlt(hdc, 0, 0, image.Width, image.Height, memoryHdc, 0, 0, 13369376u);
				graphics.ReleaseHdc(hdc);
			}
		}
	}
}
