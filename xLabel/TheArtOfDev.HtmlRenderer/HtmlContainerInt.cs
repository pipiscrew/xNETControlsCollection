using System;
using System.Collections.Generic;
using System.Diagnostics;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core
{
	public sealed class HtmlContainerInt : IDisposable
	{
		private readonly RAdapter _adapter;

		private readonly CssParser _cssParser;

		private CssBox _root;

		private List<HoverBoxBlock> _hoverBoxes;

		private SelectionHandler _selectionHandler;

		private ImageDownloader _imageDownloader;

		private RColor _selectionForeColor;

		private RColor _selectionBackColor;

		private CssData _cssData;

		private bool _isSelectionEnabled = true;

		private bool _isContextMenuEnabled = true;

		private bool _avoidGeometryAntialias;

		private bool _avoidAsyncImagesLoading;

		private bool _avoidImagesLateLoading;

		private bool _loadComplete;

		private RPoint _location;

		private RSize _maxSize;

		private RPoint _scrollOffset;

		private RSize _actualSize;

		private int _marginTop;

		private int _marginBottom;

		private int _marginLeft;

		private int _marginRight;

		internal RAdapter Adapter
		{
			get
			{
				return _adapter;
			}
		}

		internal CssParser CssParser
		{
			get
			{
				return _cssParser;
			}
		}

		public CssData CssData
		{
			get
			{
				return _cssData;
			}
		}

		public bool AvoidGeometryAntialias
		{
			get
			{
				return _avoidGeometryAntialias;
			}
			set
			{
				_avoidGeometryAntialias = value;
			}
		}

		public bool AvoidAsyncImagesLoading
		{
			get
			{
				return _avoidAsyncImagesLoading;
			}
			set
			{
				_avoidAsyncImagesLoading = value;
			}
		}

		public bool AvoidImagesLateLoading
		{
			get
			{
				return _avoidImagesLateLoading;
			}
			set
			{
				_avoidImagesLateLoading = value;
			}
		}

		public bool IsSelectionEnabled
		{
			get
			{
				return _isSelectionEnabled;
			}
			set
			{
				_isSelectionEnabled = value;
			}
		}

		public bool IsContextMenuEnabled
		{
			get
			{
				return _isContextMenuEnabled;
			}
			set
			{
				_isContextMenuEnabled = value;
			}
		}

		public RPoint ScrollOffset
		{
			get
			{
				return _scrollOffset;
			}
			set
			{
				_scrollOffset = value;
			}
		}

		public RPoint Location
		{
			get
			{
				return _location;
			}
			set
			{
				_location = value;
			}
		}

		public RSize MaxSize
		{
			get
			{
				return _maxSize;
			}
			set
			{
				_maxSize = value;
			}
		}

		public RSize ActualSize
		{
			get
			{
				return _actualSize;
			}
			set
			{
				_actualSize = value;
			}
		}

		public RSize PageSize { get; set; }

		public int MarginTop
		{
			get
			{
				return _marginTop;
			}
			set
			{
				if (value > -1)
					_marginTop = value;
			}
		}

		public int MarginBottom
		{
			get
			{
				return _marginBottom;
			}
			set
			{
				if (value > -1)
					_marginBottom = value;
			}
		}

		public int MarginLeft
		{
			get
			{
				return _marginLeft;
			}
			set
			{
				if (value > -1)
					_marginLeft = value;
			}
		}

		public int MarginRight
		{
			get
			{
				return _marginRight;
			}
			set
			{
				if (value > -1)
					_marginRight = value;
			}
		}

		public string SelectedText
		{
			get
			{
				return _selectionHandler.GetSelectedText();
			}
		}

		public string SelectedHtml
		{
			get
			{
				return _selectionHandler.GetSelectedHtml();
			}
		}

		internal CssBox Root
		{
			get
			{
				return _root;
			}
		}

		internal RColor SelectionForeColor
		{
			get
			{
				return _selectionForeColor;
			}
			set
			{
				_selectionForeColor = value;
			}
		}

		internal RColor SelectionBackColor
		{
			get
			{
				return _selectionBackColor;
			}
			set
			{
				_selectionBackColor = value;
			}
		}

		public event EventHandler LoadComplete;

		public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;

		public event EventHandler<HtmlRefreshEventArgs> Refresh;

		public event EventHandler<HtmlScrollEventArgs> ScrollChange;

		public event EventHandler<HtmlRenderErrorEventArgs> RenderError;

		public event EventHandler<HtmlStylesheetLoadEventArgs> StylesheetLoad;

		public event EventHandler<HtmlImageLoadEventArgs> ImageLoad;

		public HtmlContainerInt(RAdapter adapter)
		{
			ArgChecker.AssertArgNotNull(adapter, "global");
			_adapter = adapter;
			_cssParser = new CssParser(adapter);
		}

		public void SetMargins(int value)
		{
			if (value > -1)
				_marginBottom = (_marginLeft = (_marginTop = (_marginRight = value)));
		}

		public void SetHtml(string htmlSource, CssData baseCssData = null)
		{
			Clear();
			if (!string.IsNullOrEmpty(htmlSource))
			{
				_loadComplete = false;
				_cssData = baseCssData ?? _adapter.DefaultCssData;
				DomParser domParser = new DomParser(_cssParser);
				_root = domParser.GenerateCssTree(htmlSource, this, ref _cssData);
				if (_root != null)
				{
					_selectionHandler = new SelectionHandler(_root);
					_imageDownloader = new ImageDownloader();
				}
			}
		}

		public void Clear()
		{
			if (_root != null)
			{
				_root.Dispose();
				_root = null;
				if (_selectionHandler != null)
					_selectionHandler.Dispose();
				_selectionHandler = null;
				if (_imageDownloader != null)
					_imageDownloader.Dispose();
				_imageDownloader = null;
				_hoverBoxes = null;
			}
		}

		public void ClearSelection()
		{
			if (_selectionHandler != null)
			{
				_selectionHandler.ClearSelection();
				RequestRefresh(false);
			}
		}

		public string GetHtml(HtmlGenerationStyle styleGen = HtmlGenerationStyle.Inline)
		{
			return DomUtils.GenerateHtml(_root, styleGen);
		}

		public string GetAttributeAt(RPoint location, string attribute)
		{
			ArgChecker.AssertArgNotNullOrEmpty(attribute, "attribute");
			CssBox cssBox = DomUtils.GetCssBox(_root, OffsetByScroll(location));
			return (cssBox != null) ? DomUtils.GetAttribute(cssBox, attribute) : null;
		}

		public List<LinkElementData<RRect>> GetLinks()
		{
			List<CssBox> list = new List<CssBox>();
			DomUtils.GetAllLinkBoxes(_root, list);
			List<LinkElementData<RRect>> list2 = new List<LinkElementData<RRect>>();
			foreach (CssBox item in list)
			{
				list2.Add(new LinkElementData<RRect>(item.GetAttribute("id"), item.GetAttribute("href"), CommonUtils.GetFirstValueOrDefault(item.Rectangles, item.Bounds)));
			}
			return list2;
		}

		public string GetLinkAt(RPoint location)
		{
			CssBox linkBox = DomUtils.GetLinkBox(_root, OffsetByScroll(location));
			return (linkBox != null) ? linkBox.HrefLink : null;
		}

		public RRect? GetElementRectangle(string elementId)
		{
			ArgChecker.AssertArgNotNullOrEmpty(elementId, "elementId");
			CssBox boxById = DomUtils.GetBoxById(_root, elementId.ToLower());
			return (boxById != null) ? new RRect?(CommonUtils.GetFirstValueOrDefault(boxById.Rectangles, boxById.Bounds)) : null;
		}

		public void PerformLayout(RGraphics g)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			_actualSize = RSize.Empty;
			if (_root == null)
				return;
			_root.Size = new RSize((_maxSize.Width > 0.0) ? _maxSize.Width : 99999.0, 0.0);
			_root.Location = _location;
			_root.PerformLayout(g);
			if (_maxSize.Width <= 0.1)
			{
				_root.Size = new RSize((int)Math.Ceiling(_actualSize.Width), 0.0);
				_actualSize = RSize.Empty;
				_root.PerformLayout(g);
			}
			if (!_loadComplete)
			{
				_loadComplete = true;
				EventHandler loadComplete = this.LoadComplete;
				if (loadComplete != null)
					loadComplete(this, EventArgs.Empty);
			}
		}

		public void PerformPaint(RGraphics g)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			if (MaxSize.Height > 0.0)
				g.PushClip(new RRect(_location.X, _location.Y, Math.Min(_maxSize.Width, PageSize.Width), Math.Min(_maxSize.Height, PageSize.Height)));
			else
				g.PushClip(new RRect(MarginLeft, MarginTop, PageSize.Width, PageSize.Height));
			if (_root != null)
				_root.Paint(g);
			g.PopClip();
		}

		public void HandleMouseDown(RControl parent, RPoint location)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			try
			{
				if (_selectionHandler != null)
					_selectionHandler.HandleMouseDown(parent, OffsetByScroll(location), IsMouseInContainer(location));
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse down handle", exception);
			}
		}

		public void HandleMouseUp(RControl parent, RPoint location, RMouseEvent e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			try
			{
				if (_selectionHandler != null && IsMouseInContainer(location) && !_selectionHandler.HandleMouseUp(parent, e.LeftButton) && e.LeftButton)
				{
					RPoint location2 = OffsetByScroll(location);
					CssBox linkBox = DomUtils.GetLinkBox(_root, location2);
					if (linkBox != null)
						HandleLinkClicked(parent, location, linkBox);
				}
			}
			catch (HtmlLinkClickedException)
			{
				throw;
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse up handle", exception);
			}
		}

		public void HandleMouseDoubleClick(RControl parent, RPoint location)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			try
			{
				if (_selectionHandler != null && IsMouseInContainer(location))
					_selectionHandler.SelectWord(parent, OffsetByScroll(location));
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse double click handle", exception);
			}
		}

		public void HandleMouseMove(RControl parent, RPoint location)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			try
			{
				RPoint loc = OffsetByScroll(location);
				if (_selectionHandler != null && IsMouseInContainer(location))
					_selectionHandler.HandleMouseMove(parent, loc);
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse move handle", exception);
			}
		}

		public void HandleMouseLeave(RControl parent)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			try
			{
				if (_selectionHandler != null)
					_selectionHandler.HandleMouseLeave(parent);
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed mouse leave handle", exception);
			}
		}

		public void HandleKeyDown(RControl parent, RKeyEvent e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			try
			{
				if (e.Control && _selectionHandler != null)
				{
					if (e.AKeyCode)
						_selectionHandler.SelectAll(parent);
					if (e.CKeyCode)
						_selectionHandler.CopySelectedHtml();
				}
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.KeyboardMouse, "Failed key down handle", exception);
			}
		}

		internal void RaiseHtmlStylesheetLoadEvent(HtmlStylesheetLoadEventArgs args)
		{
			try
			{
				EventHandler<HtmlStylesheetLoadEventArgs> stylesheetLoad = this.StylesheetLoad;
				if (stylesheetLoad != null)
					stylesheetLoad(this, args);
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.CssParsing, "Failed stylesheet load event", exception);
			}
		}

		internal void RaiseHtmlImageLoadEvent(HtmlImageLoadEventArgs args)
		{
			try
			{
				EventHandler<HtmlImageLoadEventArgs> imageLoad = this.ImageLoad;
				if (imageLoad != null)
					imageLoad(this, args);
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.Image, "Failed image load event", exception);
			}
		}

		public void RequestRefresh(bool layout)
		{
			try
			{
				EventHandler<HtmlRefreshEventArgs> refresh = this.Refresh;
				if (refresh != null)
					refresh(this, new HtmlRefreshEventArgs(layout));
			}
			catch (Exception exception)
			{
				ReportError(HtmlRenderErrorType.General, "Failed refresh request", exception);
			}
		}

		internal void ReportError(HtmlRenderErrorType type, string message, Exception exception = null)
		{
			try
			{
				EventHandler<HtmlRenderErrorEventArgs> renderError = this.RenderError;
				if (renderError != null)
					renderError(this, new HtmlRenderErrorEventArgs(type, message, exception));
			}
			catch
			{
			}
		}

		internal void HandleLinkClicked(RControl parent, RPoint location, CssBox link)
		{
			EventHandler<HtmlLinkClickedEventArgs> linkClicked = this.LinkClicked;
			if (linkClicked != null)
			{
				HtmlLinkClickedEventArgs htmlLinkClickedEventArgs = new HtmlLinkClickedEventArgs(link.HrefLink, link.HtmlTag.Attributes);
				try
				{
					linkClicked(this, htmlLinkClickedEventArgs);
				}
				catch (Exception innerException)
				{
					throw new HtmlLinkClickedException("Error in link clicked intercept", innerException);
				}
				if (htmlLinkClickedEventArgs.Handled)
					return;
			}
			if (string.IsNullOrEmpty(link.HrefLink))
				return;
			if (link.HrefLink.StartsWith("#") && link.HrefLink.Length > 1)
			{
				EventHandler<HtmlScrollEventArgs> scrollChange = this.ScrollChange;
				if (scrollChange != null)
				{
					RRect? elementRectangle = GetElementRectangle(link.HrefLink.Substring(1));
					if (elementRectangle.HasValue)
					{
						scrollChange(this, new HtmlScrollEventArgs(elementRectangle.Value.Location));
						HandleMouseMove(parent, location);
					}
				}
			}
			else
			{
				ProcessStartInfo processStartInfo = new ProcessStartInfo(link.HrefLink);
				processStartInfo.UseShellExecute = true;
				Process.Start(processStartInfo);
			}
		}

		internal void AddHoverBox(CssBox box, CssBlock block)
		{
			ArgChecker.AssertArgNotNull(box, "box");
			ArgChecker.AssertArgNotNull(block, "block");
			if (_hoverBoxes == null)
				_hoverBoxes = new List<HoverBoxBlock>();
			_hoverBoxes.Add(new HoverBoxBlock(box, block));
		}

		internal ImageDownloader GetImageDownloader()
		{
			return _imageDownloader;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		private RPoint OffsetByScroll(RPoint location)
		{
			return new RPoint(location.X - ScrollOffset.X, location.Y - ScrollOffset.Y);
		}

		private bool IsMouseInContainer(RPoint location)
		{
			return location.X >= _location.X && location.X <= _location.X + _actualSize.Width && location.Y >= _location.Y + ScrollOffset.Y && location.Y <= _location.Y + ScrollOffset.Y + _actualSize.Height;
		}

		private void Dispose(bool all)
		{
			try
			{
				if (all)
				{
					this.LinkClicked = null;
					this.Refresh = null;
					this.RenderError = null;
					this.StylesheetLoad = null;
					this.ImageLoad = null;
				}
				_cssData = null;
				if (_root != null)
					_root.Dispose();
				_root = null;
				if (_selectionHandler != null)
					_selectionHandler.Dispose();
				_selectionHandler = null;
			}
			catch
			{
			}
		}
	}
}
