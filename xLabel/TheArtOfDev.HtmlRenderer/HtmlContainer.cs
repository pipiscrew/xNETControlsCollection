using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WinForms.Adapters;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms
{
	public sealed class HtmlContainer : IDisposable
	{
		private readonly HtmlContainerInt _htmlContainerInt;

		private bool _useGdiPlusTextRendering;

		internal HtmlContainerInt HtmlContainerInt
		{
			get
			{
				return _htmlContainerInt;
			}
		}

		public bool UseGdiPlusTextRendering
		{
			get
			{
				return _useGdiPlusTextRendering;
			}
			set
			{
				if (_useGdiPlusTextRendering != value)
				{
					_useGdiPlusTextRendering = value;
					_htmlContainerInt.RequestRefresh(true);
				}
			}
		}

		public CssData CssData
		{
			get
			{
				return _htmlContainerInt.CssData;
			}
		}

		public bool AvoidGeometryAntialias
		{
			get
			{
				return _htmlContainerInt.AvoidGeometryAntialias;
			}
			set
			{
				_htmlContainerInt.AvoidGeometryAntialias = value;
			}
		}

		public bool AvoidAsyncImagesLoading
		{
			get
			{
				return _htmlContainerInt.AvoidAsyncImagesLoading;
			}
			set
			{
				_htmlContainerInt.AvoidAsyncImagesLoading = value;
			}
		}

		public bool AvoidImagesLateLoading
		{
			get
			{
				return _htmlContainerInt.AvoidImagesLateLoading;
			}
			set
			{
				_htmlContainerInt.AvoidImagesLateLoading = value;
			}
		}

		public bool IsSelectionEnabled
		{
			get
			{
				return _htmlContainerInt.IsSelectionEnabled;
			}
			set
			{
				_htmlContainerInt.IsSelectionEnabled = value;
			}
		}

		public bool IsContextMenuEnabled
		{
			get
			{
				return _htmlContainerInt.IsContextMenuEnabled;
			}
			set
			{
				_htmlContainerInt.IsContextMenuEnabled = value;
			}
		}

		public Point ScrollOffset
		{
			get
			{
				return Utils.ConvertRound(_htmlContainerInt.ScrollOffset);
			}
			set
			{
				_htmlContainerInt.ScrollOffset = Utils.Convert(value);
			}
		}

		public PointF Location
		{
			get
			{
				return Utils.Convert(_htmlContainerInt.Location);
			}
			set
			{
				_htmlContainerInt.Location = Utils.Convert(value);
			}
		}

		public SizeF MaxSize
		{
			get
			{
				return Utils.Convert(_htmlContainerInt.MaxSize);
			}
			set
			{
				_htmlContainerInt.MaxSize = Utils.Convert(value);
			}
		}

		public SizeF ActualSize
		{
			get
			{
				return Utils.Convert(_htmlContainerInt.ActualSize);
			}
			internal set
			{
				_htmlContainerInt.ActualSize = Utils.Convert(value);
			}
		}

		public string SelectedText
		{
			get
			{
				return _htmlContainerInt.SelectedText;
			}
		}

		public string SelectedHtml
		{
			get
			{
				return _htmlContainerInt.SelectedHtml;
			}
		}

		public event EventHandler LoadComplete
		{
			add
			{
				HtmlContainerInt.LoadComplete += value;
			}
			remove
			{
				HtmlContainerInt.LoadComplete -= value;
			}
		}

		public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked
		{
			add
			{
				_htmlContainerInt.LinkClicked += value;
			}
			remove
			{
				_htmlContainerInt.LinkClicked -= value;
			}
		}

		public event EventHandler<HtmlRefreshEventArgs> Refresh
		{
			add
			{
				_htmlContainerInt.Refresh += value;
			}
			remove
			{
				_htmlContainerInt.Refresh -= value;
			}
		}

		public event EventHandler<HtmlScrollEventArgs> ScrollChange
		{
			add
			{
				_htmlContainerInt.ScrollChange += value;
			}
			remove
			{
				_htmlContainerInt.ScrollChange -= value;
			}
		}

		public event EventHandler<HtmlRenderErrorEventArgs> RenderError
		{
			add
			{
				_htmlContainerInt.RenderError += value;
			}
			remove
			{
				_htmlContainerInt.RenderError -= value;
			}
		}

		public event EventHandler<HtmlStylesheetLoadEventArgs> StylesheetLoad
		{
			add
			{
				_htmlContainerInt.StylesheetLoad += value;
			}
			remove
			{
				_htmlContainerInt.StylesheetLoad -= value;
			}
		}

		public event EventHandler<HtmlImageLoadEventArgs> ImageLoad
		{
			add
			{
				_htmlContainerInt.ImageLoad += value;
			}
			remove
			{
				_htmlContainerInt.ImageLoad -= value;
			}
		}

		public HtmlContainer()
		{
			_htmlContainerInt = new HtmlContainerInt(WinFormsAdapter.Instance);
			_htmlContainerInt.SetMargins(0);
			_htmlContainerInt.PageSize = new RSize(99999.0, 99999.0);
		}

		public void ClearSelection()
		{
			HtmlContainerInt.ClearSelection();
		}

		public void SetHtml(string htmlSource, CssData baseCssData = null)
		{
			_htmlContainerInt.SetHtml(htmlSource, baseCssData);
		}

		public string GetHtml(HtmlGenerationStyle styleGen = HtmlGenerationStyle.Inline)
		{
			return _htmlContainerInt.GetHtml(styleGen);
		}

		public string GetAttributeAt(Point location, string attribute)
		{
			return _htmlContainerInt.GetAttributeAt(Utils.Convert(location), attribute);
		}

		public List<LinkElementData<RectangleF>> GetLinks()
		{
			List<LinkElementData<RectangleF>> list = new List<LinkElementData<RectangleF>>();
			foreach (LinkElementData<RRect> link in HtmlContainerInt.GetLinks())
			{
				list.Add(new LinkElementData<RectangleF>(link.Id, link.Href, Utils.Convert(link.Rectangle)));
			}
			return list;
		}

		public string GetLinkAt(Point location)
		{
			return _htmlContainerInt.GetLinkAt(Utils.Convert(location));
		}

		public RectangleF? GetElementRectangle(string elementId)
		{
			RRect? elementRectangle = _htmlContainerInt.GetElementRectangle(elementId);
			return elementRectangle.HasValue ? new RectangleF?(Utils.Convert(elementRectangle.Value)) : null;
		}

		public void PerformLayout(Graphics g)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			using (GraphicsAdapter g2 = new GraphicsAdapter(g, _useGdiPlusTextRendering))
				_htmlContainerInt.PerformLayout(g2);
		}

		public void PerformPaint(Graphics g)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			using (GraphicsAdapter g2 = new GraphicsAdapter(g, _useGdiPlusTextRendering))
				_htmlContainerInt.PerformPaint(g2);
		}

		public void HandleMouseDown(Control parent, MouseEventArgs e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			_htmlContainerInt.HandleMouseDown(new ControlAdapter(parent, _useGdiPlusTextRendering), Utils.Convert(e.Location));
		}

		public void HandleMouseUp(Control parent, MouseEventArgs e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			_htmlContainerInt.HandleMouseUp(new ControlAdapter(parent, _useGdiPlusTextRendering), Utils.Convert(e.Location), CreateMouseEvent(e));
		}

		public void HandleMouseDoubleClick(Control parent, MouseEventArgs e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			_htmlContainerInt.HandleMouseDoubleClick(new ControlAdapter(parent, _useGdiPlusTextRendering), Utils.Convert(e.Location));
		}

		public void HandleMouseMove(Control parent, MouseEventArgs e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			_htmlContainerInt.HandleMouseMove(new ControlAdapter(parent, _useGdiPlusTextRendering), Utils.Convert(e.Location));
		}

		public void HandleMouseLeave(Control parent)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			_htmlContainerInt.HandleMouseLeave(new ControlAdapter(parent, _useGdiPlusTextRendering));
		}

		public void HandleKeyDown(Control parent, KeyEventArgs e)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			ArgChecker.AssertArgNotNull(e, "e");
			_htmlContainerInt.HandleKeyDown(new ControlAdapter(parent, _useGdiPlusTextRendering), CreateKeyEevent(e));
		}

		public void Dispose()
		{
			_htmlContainerInt.Dispose();
		}

		private static RMouseEvent CreateMouseEvent(MouseEventArgs e)
		{
			return new RMouseEvent((e.Button & MouseButtons.Left) != 0);
		}

		private static RKeyEvent CreateKeyEevent(KeyEventArgs e)
		{
			return new RKeyEvent(e.Control, e.KeyCode == Keys.A, e.KeyCode == Keys.C);
		}
	}
}
