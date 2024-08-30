using System;
using System.Collections.Generic;
using System.IO;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Adapters
{
	public abstract class RAdapter
	{
		private readonly Dictionary<RColor, RBrush> _brushesCache = new Dictionary<RColor, RBrush>();

		private readonly Dictionary<RColor, RPen> _penCache = new Dictionary<RColor, RPen>();

		private readonly FontsHandler _fontsHandler;

		private CssData _defaultCssData;

		private RImage _loadImage;

		private RImage _errorImage;

		public CssData DefaultCssData
		{
			get
			{
				return _defaultCssData ?? (_defaultCssData = CssData.Parse(this, "\n        html, address,\n        blockquote,\n        body, dd, div,\n        dl, dt, fieldset, form,\n        frame, frameset,\n        h1, h2, h3, h4,\n        h5, h6, noframes,\n        ol, p, ul, center,\n        dir, menu, pre   { display: block }\n        li              { display: list-item }\n        head            { display: none }\n        table           { display: table }\n        tr              { display: table-row }\n        thead           { display: table-header-group }\n        tbody           { display: table-row-group }\n        tfoot           { display: table-footer-group }\n        col             { display: table-column }\n        colgroup        { display: table-column-group }\n        td, th          { display: table-cell }\n        caption         { display: table-caption }\n        th              { font-weight: bolder; text-align: center }\n        caption         { text-align: center }\n        body            { margin: 8px }\n        h1              { font-size: 2em; margin: .67em 0 }\n        h2              { font-size: 1.5em; margin: .75em 0 }\n        h3              { font-size: 1.17em; margin: .83em 0 }\n        h4, p,\n        blockquote, ul,\n        fieldset, form,\n        ol, dl, dir,\n        menu            { margin: 1.12em 0 }\n        h5              { font-size: .83em; margin: 1.5em 0 }\n        h6              { font-size: .75em; margin: 1.67em 0 }\n        h1, h2, h3, h4,\n        h5, h6, b,\n        strong          { font-weight: bolder; }\n        blockquote      { margin-left: 40px; margin-right: 40px }\n        i, cite, em,\n        var, address    { font-style: italic }\n        pre, tt, code,\n        kbd, samp       { font-family: monospace }\n        pre             { white-space: pre }\n        button, textarea,\n        input, select   { display: inline-block }\n        big             { font-size: 1.17em }\n        small, sub, sup { font-size: .83em }\n        sub             { vertical-align: sub }\n        sup             { vertical-align: super }\n        table           { border-spacing: 2px; }\n        thead, tbody,\n        tfoot, tr       { vertical-align: middle }\n        td, th          { vertical-align: inherit }\n        s, strike, del  { text-decoration: line-through }\n        hr              { border: 1px inset; }\n        ol, ul, dir,\n        menu, dd        { margin-left: 40px }\n        ol              { list-style-type: decimal }\n        ol ul, ul ol,\n        ul ul, ol ol    { margin-top: 0; margin-bottom: 0 }\n        ol ul, ul ul   { list-style-type: circle }\n        ul ul ul, \n        ol ul ul, \n        ul ol ul        { list-style-type: square }\n        u, ins          { text-decoration: underline }\n        br:before       { content: \"\\A\" }\n        :before, :after { white-space: pre-line }\n        center          { text-align: center }\n        :link, :visited { text-decoration: underline }\n        :focus          { outline: thin dotted invert }\n\n        /* Begin bidirectionality settings (do not change) */\n        BDO[DIR=\"ltr\"]  { direction: ltr; unicode-bidi: bidi-override }\n        BDO[DIR=\"rtl\"]  { direction: rtl; unicode-bidi: bidi-override }\n\n        *[DIR=\"ltr\"]    { direction: ltr; unicode-bidi: embed }\n        *[DIR=\"rtl\"]    { direction: rtl; unicode-bidi: embed }\n\n        @media print {\n          h1            { page-break-before: always }\n          h1, h2, h3,\n          h4, h5, h6    { page-break-after: avoid }\n          ul, ol, dl    { page-break-before: avoid }\n        }\n\n        /* Not in the specification but necessary */\n        a               { color: #0055BB; text-decoration:underline }\n        table           { border-color:#dfdfdf; }\n        td, th          { border-color:#dfdfdf; overflow: hidden; }\n        style, title,\n        script, link,\n        meta, area,\n        base, param     { display:none }\n        hr              { border-top-color: #9A9A9A; border-left-color: #9A9A9A; border-bottom-color: #EEEEEE; border-right-color: #EEEEEE; }\n        pre             { font-size: 10pt; margin-top: 15px; }\n        \n        /*This is the background of the HtmlToolTip*/\n        .htmltooltip {\n            border:solid 1px #767676;\n            background-color:white;\n            background-gradient:#E4E5F0;\n            padding: 8px; \n            Font: 9pt Tahoma;\n        }", false));
			}
		}

		protected RAdapter()
		{
			_fontsHandler = new FontsHandler(this);
		}

		public RColor GetColor(string colorName)
		{
			ArgChecker.AssertArgNotNullOrEmpty(colorName, "colorName");
			return GetColorInt(colorName);
		}

		public RPen GetPen(RColor color)
		{
			RPen value;
			if (!_penCache.TryGetValue(color, out value))
				value = (_penCache[color] = CreatePen(color));
			return value;
		}

		public RBrush GetSolidBrush(RColor color)
		{
			RBrush value;
			if (!_brushesCache.TryGetValue(color, out value))
				value = (_brushesCache[color] = CreateSolidBrush(color));
			return value;
		}

		public RBrush GetLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
		{
			return CreateLinearGradientBrush(rect, color1, color2, angle);
		}

		public RImage ConvertImage(object image)
		{
			return ConvertImageInt(image);
		}

		public RImage ImageFromStream(Stream memoryStream)
		{
			return ImageFromStreamInt(memoryStream);
		}

		public bool IsFontExists(string font)
		{
			return _fontsHandler.IsFontExists(font);
		}

		public void AddFontFamily(RFontFamily fontFamily)
		{
			_fontsHandler.AddFontFamily(fontFamily);
		}

		public void AddFontFamilyMapping(string fromFamily, string toFamily)
		{
			_fontsHandler.AddFontFamilyMapping(fromFamily, toFamily);
		}

		public RFont GetFont(string family, double size, RFontStyle style)
		{
			return _fontsHandler.GetCachedFont(family, size, style);
		}

		public RImage GetLoadingImage()
		{
			if (_loadImage == null)
			{
				Stream manifestResourceStream = typeof(HtmlRendererUtils).Assembly.GetManifestResourceStream("TheArtOfDev.HtmlRenderer.Core.Utils.ImageLoad.png");
				if (manifestResourceStream != null)
					_loadImage = ImageFromStream(manifestResourceStream);
			}
			return _loadImage;
		}

		public RImage GetLoadingFailedImage()
		{
			if (_errorImage == null)
			{
				Stream manifestResourceStream = typeof(HtmlRendererUtils).Assembly.GetManifestResourceStream("TheArtOfDev.HtmlRenderer.Core.Utils.ImageError.png");
				if (manifestResourceStream != null)
					_errorImage = ImageFromStream(manifestResourceStream);
			}
			return _errorImage;
		}

		public object GetClipboardDataObject(string html, string plainText)
		{
			return GetClipboardDataObjectInt(html, plainText);
		}

		public void SetToClipboard(string text)
		{
			SetToClipboardInt(text);
		}

		public void SetToClipboard(string html, string plainText)
		{
			SetToClipboardInt(html, plainText);
		}

		public void SetToClipboard(RImage image)
		{
			SetToClipboardInt(image);
		}

		public RContextMenu GetContextMenu()
		{
			return CreateContextMenuInt();
		}

		public void SaveToFile(RImage image, string name, string extension, RControl control = null)
		{
			SaveToFileInt(image, name, extension, control);
		}

		internal RFont CreateFont(string family, double size, RFontStyle style)
		{
			return CreateFontInt(family, size, style);
		}

		internal RFont CreateFont(RFontFamily family, double size, RFontStyle style)
		{
			return CreateFontInt(family, size, style);
		}

		protected abstract RColor GetColorInt(string colorName);

		protected abstract RPen CreatePen(RColor color);

		protected abstract RBrush CreateSolidBrush(RColor color);

		protected abstract RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle);

		protected abstract RImage ConvertImageInt(object image);

		protected abstract RImage ImageFromStreamInt(Stream memoryStream);

		protected abstract RFont CreateFontInt(string family, double size, RFontStyle style);

		protected abstract RFont CreateFontInt(RFontFamily family, double size, RFontStyle style);

		protected virtual object GetClipboardDataObjectInt(string html, string plainText)
		{
			throw new NotImplementedException();
		}

		protected virtual void SetToClipboardInt(string text)
		{
			throw new NotImplementedException();
		}

		protected virtual void SetToClipboardInt(string html, string plainText)
		{
			throw new NotImplementedException();
		}

		protected virtual void SetToClipboardInt(RImage image)
		{
			throw new NotImplementedException();
		}

		protected virtual RContextMenu CreateContextMenuInt()
		{
			throw new NotImplementedException();
		}

		protected virtual void SaveToFileInt(RImage image, string name, string extension, RControl control = null)
		{
			throw new NotImplementedException();
		}
	}
}
