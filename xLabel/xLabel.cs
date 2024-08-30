using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Text;
using System.Globalization;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.WinForms;
using TheArtOfDev.HtmlRenderer.WinForms.Adapters;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace xCollection
{
	[Category("x UI For Windows Forms")]
	[DebuggerStepThrough]
	[ToolboxBitmap(typeof(Label))]
	[DefaultProperty("Text")]
	[DefaultEvent("Click")]
	[Description("Provides standard and rich HTML text-rendering capabilities with native support for CSS 2.0 styling and inline CSS style-tags.")]
	[Designer(typeof(xDesigner))]
	public class xLabel : Control
	{
		public enum TextAlignments
		{
			Left,
			Right,
			Center
		}

		public enum TextFormattingOptions
		{
			Default,
			UpperCase,
			LowerCase,
			TitleCase,
			SentenceCase
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					xLabel xLabel = (xLabel)base.Control;
					if (xLabel.AutoSize)
						return SelectionRules.Moveable | SelectionRules.Visible;
					if (xLabel.AutoSizeHeightOnly)
						return SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
					return SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
				}
			}

			public override DesignerActionListCollection ActionLists
			{
				get
				{
					if (actionLists == null)
					{
						actionLists = new DesignerActionListCollection();
						actionLists.Add(new xLabelActionList(base.Component));
					}
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		public class xLabelActionList : DesignerActionList
		{
			private xLabel xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public string Name
			{
				get
				{
					return xControl.Name;
				}
				set
				{
					SetValue(xControl, "Name", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public Font Font
			{
				get
				{
					return xControl.Font;
				}
				set
				{
					SetValue(xControl, "Font", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public bool AutoSize
			{
				get
				{
					return xControl.AutoSize;
				}
				set
				{
					SetValue(xControl, "AutoSize", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public bool AutoSizeHeightOnly
			{
				get
				{
					return xControl.AutoSizeHeightOnly;
				}
				set
				{
					SetValue(xControl, "AutoSizeHeightOnly", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public bool AutoEllipsis
			{
				get
				{
					return xControl.AutoEllipsis;
				}
				set
				{
					SetValue(xControl, "AutoEllipsis", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public ContentAlignment TextAlign
			{
				get
				{
					return xControl.TextAlignment;
				}
				set
				{
					SetValue(xControl, "TextAlignment", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public DockStyle Dock
			{
				get
				{
					return xControl.Dock;
				}
				set
				{
					SetValue(xControl, "Dock", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public Color BackColor
			{
				get
				{
					return xControl.BackColor;
				}
				set
				{
					SetValue(xControl, "BackColor", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public Color ForeColor
			{
				get
				{
					return xControl.ForeColor;
				}
				set
				{
					SetValue(xControl, "ForeColor", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			[Localizable(true)]
			[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
			public string Text
			{
				get
				{
					return xControl.Text;
				}
				set
				{
					SetValue(xControl, "Text", value);
					designerActionUISvc.Refresh(xControl);
				}
			}

			public xLabelActionList(IComponent component)
				: base(component)
			{
				xControl = component as xLabel;
				designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				AutoShow = false;
			}

			internal static PropertyDescriptor GetPropertyDescriptor(IComponent component, string propertyName)
			{
				return TypeDescriptor.GetProperties(component)[propertyName];
			}

			internal static IDesignerHost GetDesignerHost(IComponent component)
			{
				return (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			}

			internal static IComponentChangeService GetChangeService(IComponent component)
			{
				return (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			}

			internal static void SetValue(IComponent component, string propertyName, object value)
			{
				PropertyDescriptor propertyDescriptor = GetPropertyDescriptor(component, propertyName);
				IComponentChangeService changeService = GetChangeService(component);
				IDesignerHost designerHost = GetDesignerHost(component);
				DesignerTransaction designerTransaction = designerHost.CreateTransaction();
				try
				{
					changeService.OnComponentChanging(component, propertyDescriptor);
					propertyDescriptor.SetValue(component, value);
					changeService.OnComponentChanged(component, propertyDescriptor, null, null);
					designerTransaction.Commit();
					designerTransaction = null;
				}
				finally
				{
					if (designerTransaction != null)
						designerTransaction.Cancel();
				}
			}

			public override DesignerActionItemCollection GetSortedActionItems()
			{
				DesignerActionItemCollection designerActionItemCollection = new DesignerActionItemCollection();
				designerActionItemCollection.Add(new DesignerActionHeaderItem("Common Tasks"));
				designerActionItemCollection.Add(new DesignerActionHeaderItem("Design"));
				designerActionItemCollection.Add(new DesignerActionHeaderItem("Appearance"));
				designerActionItemCollection.Add(new DesignerActionHeaderItem("Layout"));
				designerActionItemCollection.Add(new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)     ", "Common Tasks"));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("Text", "Text:", "Common Tasks", "Gets or sets the text value of the label."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("Font", "Font:", "Common Tasks", "Gets or sets the label's font. This can also be overriden using the inline CSS \"font-family\" property."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("Name", "Name:", "Design", "Gets or sets the name of the label."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("ForeColor", "ForeColor:", "Appearance", "Gets or sets the foreground or text color of the label."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("BackColor", "BackColor:", "Appearance", "Gets or sets the background color of the label."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("Dock", "Dock:", "Layout", "Defines which borders of the control are bound to the container."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("TextAlign", "TextAlign", "Layout", "Determines the position of the text within the label."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("AutoSize", "AutoSize", "Layout", "Automatically sets the size of the label by content size."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("AutoSizeHeightOnly", "AutoSizeHeightOnly", "Layout", "Automatically sets the height of the label by content height (the width is not affected)."));
				designerActionItemCollection.Add(new DesignerActionPropertyItem("AutoEllipsis", "AutoEllipsis", "Layout", "Enables the automatic handling of text that extends beyond the width of the x Label control."));
				return designerActionItemCollection;
			}
		}

		private bool _autoEllipsis;

		private bool _allowParentOverrides;

		private bool _convertNewlinesToBreakTags;

		protected string _text = "";

		protected string _baseRawCssData;

		private string _longText = "";

		private string _padding = "1.2px";

		private string _verticalTextAlignment = "top";

		private string _fontStyle = "normal";

		private string _fontWeight = "normal";

		private string _horizontalTextAlignment = "left";

		private string _textDecoration = "none";

		protected BorderStyle _borderStyle;

		private ContentAlignment _textAlign;

		private TextAlignments _textAlignments;

		private ContentAlignment _textAlignment;

		private TextFormattingOptions _textFormat = TextFormattingOptions.Default;

		private Ellipsis.EllipsisFormat _ellipsisFormat;

		private Ellipsis.EllipsisFormat _lastEllipsisFormat;

		private Size _lastSize;

		private Color _backColor;

		private Color _foreColor;

		private Font _font;

		private Cursor _cursor = Cursors.Default;

		private ToolTip _toolTip = new ToolTip();

		protected CssData _baseCssData;

		protected HtmlContainer _htmlContainer;

		protected TextRenderingHint _textRenderingHint = TextRenderingHint.SystemDefault;

		protected bool _autoSizeHeight;

		protected bool _useSystemCursors;

		[Category("Behaviour")]
		[Description("Sets a value indicating whether the label will inherit its parent font and fore color properties.")]
		public virtual bool AllowParentOverrides
		{
			get
			{
				return _allowParentOverrides;
			}
			set
			{
				_allowParentOverrides = value;
				OnParentForeColorChanged(EventArgs.Empty);
				OnParentFontChanged(EventArgs.Empty);
			}
		}

		[Description("Sets the label text.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public override string Text
		{
			get
			{
				return _longText;
			}
			set
			{
				_text = value;
				_longText = value;
				base.Text = value;
				Render();
			}
		}

		[DefaultValue(typeof(Color), "Transparent")]
		[Description("Sets the background color of the label.")]
		[Browsable(true)]
		[Category("Appearance")]
		public override Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
				PerformLayout();
				Invalidate();
			}
		}

		[Category("Behaviour")]
		[Description("The text-rendering-hint to be used for rendering the text provided.")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue(TextRenderingHint.SystemDefault)]
		public TextRenderingHint TextRenderingHint
		{
			get
			{
				return _textRenderingHint;
			}
			set
			{
				_textRenderingHint = value;
			}
		}

		[DefaultValue(typeof(BorderStyle), "None")]
		[Category("Appearance")]
		public virtual BorderStyle BorderStyle
		{
			get
			{
				return _borderStyle;
			}
			set
			{
				if (BorderStyle != value)
				{
					_borderStyle = value;
					OnBorderStyleChanged(EventArgs.Empty);
				}
			}
		}

		[Description("Gets or sets the label's font. This can also be overriden using the inline CSS \"font-family\" property.")]
		[Category("Appearance")]
		[DefaultValue(typeof(Font))]
		[Browsable(true)]
		public new Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				_font = value;
				base.Font = value;
				Render();
			}
		}

		[Category("Appearance")]
		[Description("Gets or sets the foreground color of the label.")]
		[Browsable(true)]
		public new Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				_foreColor = value;
				Render();
			}
		}

		[Category("Appearance")]
		[Description("Gets or sets a value indicating whether control's elements are aligned to support locales using right-to-left fonts. You can also use the following inline CSS style: style=\"text-align: right; direction: Rtl;\"")]
		[DefaultValue("No")]
		[Browsable(true)]
		public override RightToLeft RightToLeft
		{
			get
			{
				return base.RightToLeft;
			}
			set
			{
				base.RightToLeft = value;
				Render(true);
			}
		}

		[Browsable(true)]
		[DisplayName("TextAlign")]
		[Description("Determines the position of the text within the label. Referred to as 'TextAlignment' when using it in code.")]
		public ContentAlignment TextAlignment
		{
			get
			{
				return _textAlignment;
			}
			set
			{
				_textAlignment = value;
				switch (value)
				{
				case ContentAlignment.MiddleCenter:
					_verticalTextAlignment = "middle";
					_horizontalTextAlignment = "center";
					break;
				case ContentAlignment.BottomLeft:
					_verticalTextAlignment = "bottom";
					_horizontalTextAlignment = "left";
					break;
				case ContentAlignment.BottomCenter:
					_verticalTextAlignment = "bottom";
					_horizontalTextAlignment = "center";
					break;
				case ContentAlignment.BottomRight:
					_verticalTextAlignment = "bottom";
					_horizontalTextAlignment = "right";
					break;
				case ContentAlignment.MiddleRight:
					_verticalTextAlignment = "middle";
					_horizontalTextAlignment = "right";
					break;
				case ContentAlignment.MiddleLeft:
					_verticalTextAlignment = "middle";
					_horizontalTextAlignment = "left";
					break;
				case ContentAlignment.TopRight:
					_verticalTextAlignment = "top";
					_horizontalTextAlignment = "right";
					break;
				case ContentAlignment.TopCenter:
					_verticalTextAlignment = "top";
					_horizontalTextAlignment = "center";
					break;
				case ContentAlignment.TopLeft:
					_verticalTextAlignment = "top";
					_horizontalTextAlignment = "left";
					break;
				}
				Render();
			}
		}

		[Browsable(false)]
		[Description("Provides a number of text-formatting options.")]
		[DefaultValue(true)]
		public TextFormattingOptions TextFormat
		{
			get
			{
				return _textFormat;
			}
			set
			{
				_textFormat = value;
				Render();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[Obsolete("This property is no longer supported. Please use 'TextAlignment' instead.")]
		[Description("Determines the position of the text within the label.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TextAlignments TextAlign
		{
			get
			{
				return _textAlignments;
			}
			set
			{
				_textAlignments = value;
				switch (value)
				{
				case TextAlignments.Left:
					_horizontalTextAlignment = "left";
					break;
				case TextAlignments.Center:
					_horizontalTextAlignment = "center";
					break;
				case TextAlignments.Right:
					_horizontalTextAlignment = "right";
					break;
				}
				Render();
			}
		}

		[Description("Sets the cursor that is displayed when the mouse pointer is over the control.")]
		[Category("Appearance")]
		[Browsable(true)]
		public new Cursor Cursor
		{
			get
			{
				return _cursor;
			}
			set
			{
				_cursor = value;
				base.Cursor = value;
			}
		}

		[Category("Behaviour")]
		[Browsable(true)]
		[Description("Sets the ellipsis format to be applied whenever the range of text exceeds the control's width.")]
		[DefaultValue(Ellipsis.EllipsisFormat.End)]
		public Ellipsis.EllipsisFormat EllipsisFormat
		{
			get
			{
				return _ellipsisFormat;
			}
			set
			{
				_ellipsisFormat = value;
				_lastEllipsisFormat = value;
				EventHandler ellipsisFormatChanged = this.EllipsisFormatChanged;
				if (ellipsisFormatChanged != null)
					ellipsisFormatChanged(this, EventArgs.Empty);
				Render();
			}
		}

		[Description("Enables the automatic handling of text that extends beyond the width of the x Label control.")]
		[Category("Behaviour")]
		public virtual bool AutoEllipsis
		{
			get
			{
				return _autoEllipsis;
			}
			set
			{
				_autoEllipsis = value;
				Render();
			}
		}

		[Description("Allow the label to use GDI+ text rendering to measure or draw text. By default, custom text rendering is enabled. If enabled, you can control the text's rendering using the 'TextRenderingHint' property.")]
		[Category("Behaviour")]
		[DefaultValue(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public bool UseGdiPlusTextRendering
		{
			get
			{
				return _htmlContainer.UseGdiPlusTextRendering;
			}
			set
			{
				_htmlContainer.UseGdiPlusTextRendering = value;
			}
		}

		[Category("Behaviour")]
		[Description("Sets a value indicating whether to allow conversion of newline characters to HTML break tags.")]
		[Browsable(false)]
		[DefaultValue(true)]
		public virtual bool ConvertNewlinesToBreakTags
		{
			get
			{
				return _convertNewlinesToBreakTags;
			}
			set
			{
				_convertNewlinesToBreakTags = value;
			}
		}

		[Category("Behaviour")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue(false)]
		[Description("If to use cursors defined by the operating system or .NET cursors.")]
		public bool UseSystemCursors
		{
			get
			{
				return _useSystemCursors;
			}
			set
			{
				_useSystemCursors = value;
			}
		}

		[Description("Gets or sets a value indicating whether to use the wait cursor for the current control and all child controls.")]
		[Category("Appearance")]
		[DefaultValue(false)]
		public new bool UseWaitCursor
		{
			get
			{
				return base.UseWaitCursor;
			}
			set
			{
				base.UseWaitCursor = value;
			}
		}

		[Category("Behaviour")]
		[Description("If anti-aliasing should be avoided for geometry-like backgrounds and borders.")]
		[DefaultValue(false)]
		public virtual bool AvoidGeometryAntialias
		{
			get
			{
				return _htmlContainer.AvoidGeometryAntialias;
			}
			set
			{
				_htmlContainer.AvoidGeometryAntialias = value;
			}
		}

		[Description("Is content-selection enabled for the rendered html text.")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DefaultValue(false)]
		[Category("Behaviour")]
		public virtual bool IsSelectionEnabled
		{
			get
			{
				return _htmlContainer.IsSelectionEnabled;
			}
			set
			{
				_htmlContainer.IsSelectionEnabled = value;
			}
		}

		[Browsable(true)]
		[DefaultValue(true)]
		[Description("Use the built-in context menu enabled and will be shown on mouse right-click.")]
		[Category("Behaviour")]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual bool IsContextMenuEnabled
		{
			get
			{
				return _htmlContainer.IsContextMenuEnabled;
			}
			set
			{
				_htmlContainer.IsContextMenuEnabled = value;
			}
		}

		[Browsable(true)]
		[Description("Automatically sets the height of the label by content height (width is not effected)")]
		[Category("Layout")]
		[DefaultValue(false)]
		public virtual bool AutoSizeHeightOnly
		{
			get
			{
				return _autoSizeHeight;
			}
			set
			{
				_autoSizeHeight = value;
				if (value)
				{
					AutoSize = false;
					PerformLayout();
					Invalidate();
				}
			}
		}

		[Description("Automatically sets the size of the label by content size.")]
		[DefaultValue(true)]
		[Browsable(true)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
				if (value)
				{
					_autoSizeHeight = false;
					Render();
				}
				else
					_lastSize = base.Size;
			}
		}

		[Browsable(true)]
		[DefaultValue(false)]
		[Category("Appearance")]
		[Description("Gets or sets a value indicating whether the control can accept data that the user drags onto it.")]
		public override bool AllowDrop
		{
			get
			{
				return base.AllowDrop;
			}
			set
			{
				base.AllowDrop = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Description("Set base stylesheet to be used by html rendered in the control.")]
		[Category("Appearance")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual string BaseStylesheet
		{
			get
			{
				return _baseRawCssData;
			}
			set
			{
				try
				{
					_baseRawCssData = value;
					if (_htmlContainer != null)
					{
						_baseCssData = HtmlRender.ParseStyleSheet(value);
						_htmlContainer.SetHtml(_longText, _baseCssData);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[Description("If AutoSize or AutoSizeHeightOnly is set this will restrict the max size of the control (0 is not restricted)")]
		public override Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				base.MaximumSize = value;
				if (_htmlContainer != null)
				{
					_htmlContainer.MaxSize = value;
					PerformLayout();
					Invalidate();
				}
			}
		}

		[Description("If AutoSize or AutoSizeHeightOnly is set, this will restrict the minimum size of the control (\"0\" is not restricted)")]
		public override Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				base.MinimumSize = value;
			}
		}

		[Browsable(false)]
		public Cursor CursorType
		{
			get
			{
				return _cursor;
			}
			set
			{
				_cursor = value;
				base.Cursor = value;
			}
		}

		[Browsable(false)]
		public virtual string SelectedText
		{
			get
			{
				return _htmlContainer.SelectedText;
			}
		}

		[Browsable(false)]
		public virtual string SelectedHtml
		{
			get
			{
				return _htmlContainer.SelectedHtml;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				switch (_borderStyle)
				{
				case BorderStyle.Fixed3D:
					createParams.ExStyle |= 512;
					break;
				case BorderStyle.FixedSingle:
					createParams.Style |= 8388608;
					break;
				}
				return createParams;
			}
		}

		private bool InDesignMode
		{
			get
			{
				bool result;
				if (!(result = LicenseManager.UsageMode == LicenseUsageMode.Designtime || Debugger.IsAttached))
				{
					using (Process process = Process.GetCurrentProcess())
					{
						return process.ProcessName.ToLowerInvariant().Contains("devenv");
					}
				}
				return result;
			}
		}

		[Category("Property Changed")]
		public event EventHandler EllipsisFormatChanged;

		[Category("Property Changed")]
		public event EventHandler BorderStyleChanged;

		[Category("x Events")]
		public event EventHandler LoadComplete;

		[Category("x Events")]
		public event EventHandler<HtmlLinkClickedEventArgs> LinkClicked;

		[Category("x Events")]
		public event EventHandler<HtmlRenderErrorEventArgs> RenderError;

		[Category("x Events")]
		public event EventHandler<HtmlImageLoadEventArgs> ImageLoad;

		public xLabel()
		{
			SuspendLayout();
			SetStyle(ControlStyles.Opaque, false);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			try
			{
				AutoSize = true;
				DoubleBuffered = true;
				BackColor = Color.Transparent;
				ConvertNewlinesToBreakTags = true;
				Font = new Font("Segoe UI", 9f, FontStyle.Regular);
				_lastSize = base.Size;
				_textAlignment = ContentAlignment.TopLeft;
				_ellipsisFormat = Ellipsis.EllipsisFormat.End;
				_lastEllipsisFormat = Ellipsis.EllipsisFormat.End;
				_htmlContainer = new HtmlContainer
				{
					AvoidImagesLateLoading = true,
					MaxSize = MaximumSize
				};
				_htmlContainer.LoadComplete += OnLoadComplete;
				_htmlContainer.LinkClicked += OnLinkClicked;
				_htmlContainer.RenderError += OnRenderError;
				_htmlContainer.Refresh += OnRefresh;
				_htmlContainer.ImageLoad += OnImageLoad;
				ResumeLayout(false);
				IsSelectionEnabled = false;
			}
			catch (Exception)
			{
			}
		}

		public virtual string GetHtml()
		{
			return (_htmlContainer != null) ? _htmlContainer.GetHtml() : null;
		}

		public virtual RectangleF? GetElementRectangle(string elementId)
		{
			return (_htmlContainer != null) ? _htmlContainer.GetElementRectangle(elementId) : null;
		}

		public void ClearSelection()
		{
			if (_htmlContainer != null)
				_htmlContainer.ClearSelection();
		}

		public string ToSentenceCase(string text)
		{
			string text2 = text.Substring(0, 1).ToUpper();
			text = text.Substring(1).ToLower();
			string text3 = "";
			for (int i = 0; i < text.Length; i++)
			{
				if (i > 1)
					text3 = text.Substring(i - 1, 1);
				text2 = ((!text3.Equals(" ") && !text3.Equals("\t") && !text3.Equals("\n") && !text3.Equals(".")) ? (text2 + text.Substring(i, 1)) : (text2 + text.Substring(i, 1).ToUpper()));
			}
			return text2;
		}

		private void Render(bool rightToLeft = false)
		{
			if (base.IsDisposed)
				return;
			try
			{
				if (Font.Bold)
					_fontWeight = "bold";
				else
					_fontWeight = "normal";
				if (Font.Italic)
					_fontStyle = "italic";
				else
					_fontStyle = "normal";
				if (Font.Underline && Font.Strikeout)
					_textDecoration = "line-through underline";
				else if (Font.Underline)
				{
					_textDecoration = "underline";
				}
				else if (Font.Strikeout)
				{
					_textDecoration = "line-through";
				}
				else
				{
					_textDecoration = "none";
				}
				if (InDesignMode)
				{
					if (Text == string.Empty)
						_padding = "0px";
				}
				else
					_padding = "0px";
				string text = string.Format("rgb({0}, {1}, {2})", _foreColor.R, _foreColor.G, _foreColor.B);
				string text2 = "";
				int num = 0;
				num = ((AutoEllipsis || AutoSize) ? (TextRenderer.MeasureText(Text, Font).Height - 1) : ((int)CreateGraphics().MeasureString(Text, Font, base.Width).Height));
				if (_verticalTextAlignment == "top")
					text2 = "";
				else if (_verticalTextAlignment == "middle")
				{
					text2 = ((AutoEllipsis || AutoSize) ? string.Format("margin: auto; padding-top: {0}px;", base.Height / 2 - num / 2) : string.Format("margin: auto; padding-top: {0}px;", base.Height / 2 - num / 2));
				}
				else if (_verticalTextAlignment == "bottom")
				{
					text2 = ((AutoEllipsis || AutoSize) ? string.Format("margin: auto; padding-top: {0}px;", base.Height - num - 1) : string.Format("margin: auto; padding-top: {0}px;", base.Height - num - 1));
				}
				if (RightToLeft == RightToLeft.No)
					_baseRawCssData = "#x-label { padding: " + _padding + "; " + text2 + string.Format("font-family: '{0}'; font-size: {1}pt; width: 100%; height: 100%; font-style: {2}; ", Font.FontFamily.Name, Font.SizeInPoints, _fontStyle) + "font-weight: " + _fontWeight + "; text-decoration: " + _textDecoration + "; color: " + text + "; text-align: " + _horizontalTextAlignment + "; }";
				else
					_baseRawCssData = "#x-label { padding: " + _padding + "; " + string.Format("font-family: '{0}'; font-size: {1}pt; width: 100%; height: 100%; font-style: {2}; ", Font.FontFamily.Name, Font.SizeInPoints, _fontStyle) + "font-weight: " + _fontWeight + "; text-decoration: " + _textDecoration + "; color: " + text + "; text-align: " + _horizontalTextAlignment + "; direction: rtl; }";
				BaseStylesheet = _baseRawCssData;
				if (ConvertNewlinesToBreakTags)
					_text = _text.Replace(Environment.NewLine, "<br>");
				if (AutoEllipsis && !AutoSize && !AutoSizeHeightOnly)
				{
					_text = Ellipsis.Compact(_longText, this, _ellipsisFormat);
					_toolTip.SetToolTip(this, _longText);
				}
				if (_textFormat == TextFormattingOptions.UpperCase)
					_text = _text.ToUpper();
				else if (_textFormat == TextFormattingOptions.LowerCase)
				{
					_text = _text.ToLower();
				}
				else if (_textFormat == TextFormattingOptions.TitleCase)
				{
					_text = new CultureInfo(CultureInfo.CurrentCulture.Name, false).TextInfo.ToTitleCase(_text);
				}
				else if (_textFormat == TextFormattingOptions.SentenceCase)
				{
					_text = ToSentenceCase(_text);
				}
				if (_htmlContainer != null)
				{
					_baseCssData = HtmlRender.ParseStyleSheet(_baseRawCssData);
					_htmlContainer.SetHtml("<div id = \"x-label\">" + _text.Replace(Environment.NewLine, "<br/>") + "</div>", _baseCssData);
				}
				PerformLayout();
				Invalidate();
			}
			catch (Exception)
			{
			}
		}

		[DebuggerStepThrough]
		protected override void WndProc(ref Message m)
		{
			if (_useSystemCursors && m.Msg == 32 && Cursor == Cursors.Hand)
				try
				{
					Win32Utils.SetCursor(Win32Utils.LoadCursor(0, 32649));
					m.Result = IntPtr.Zero;
					return;
				}
				catch (Exception exception)
				{
					OnRenderError(this, new HtmlRenderErrorEventArgs(HtmlRenderErrorType.General, "Failed to set OS hand cursor", exception));
				}
			base.WndProc(ref m);
		}

		protected override void Dispose(bool disposing)
		{
			if (_htmlContainer != null)
			{
				_htmlContainer.LoadComplete -= OnLoadComplete;
				_htmlContainer.LinkClicked -= OnLinkClicked;
				_htmlContainer.RenderError -= OnRenderError;
				_htmlContainer.Refresh -= OnRefresh;
				_htmlContainer.ImageLoad -= OnImageLoad;
				_htmlContainer.Dispose();
				_htmlContainer = null;
			}
			base.Dispose(disposing);
		}

		protected virtual void OnBorderStyleChanged(EventArgs e)
		{
			UpdateStyles();
			EventHandler borderStyleChanged = this.BorderStyleChanged;
			if (borderStyleChanged != null)
				borderStyleChanged(this, e);
		}

		protected virtual void OnLoadComplete(EventArgs e)
		{
			EventHandler loadComplete = this.LoadComplete;
			if (loadComplete != null)
				loadComplete(this, e);
		}

		protected virtual void OnLinkClicked(HtmlLinkClickedEventArgs e)
		{
			EventHandler<HtmlLinkClickedEventArgs> linkClicked = this.LinkClicked;
			if (linkClicked != null)
				linkClicked(this, e);
		}

		protected virtual void OnRenderError(HtmlRenderErrorEventArgs e)
		{
			EventHandler<HtmlRenderErrorEventArgs> renderError = this.RenderError;
			if (renderError != null)
				renderError(this, e);
		}

		protected virtual void OnImageLoad(HtmlImageLoadEventArgs e)
		{
			EventHandler<HtmlImageLoadEventArgs> imageLoad = this.ImageLoad;
			if (imageLoad != null)
				imageLoad(this, e);
		}

		protected virtual void OnRefresh(HtmlRefreshEventArgs e)
		{
			if (e.Layout)
				PerformLayout();
			Invalidate();
		}

		private void OnLoadComplete(object sender, EventArgs e)
		{
			OnLoadComplete(e);
		}

		private void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e)
		{
			OnLinkClicked(e);
		}

		private void OnRenderError(object sender, HtmlRenderErrorEventArgs e)
		{
			if (base.InvokeRequired)
				Invoke((MethodInvoker)delegate
				{
					OnRenderError(e);
				});
			else
				OnRenderError(e);
		}

		private void OnImageLoad(object sender, HtmlImageLoadEventArgs e)
		{
			OnImageLoad(e);
		}

		private void OnRefresh(object sender, HtmlRefreshEventArgs e)
		{
			if (base.InvokeRequired)
				Invoke((MethodInvoker)delegate
				{
					OnRefresh(e);
				});
			else
				OnRefresh(e);
		}

		protected override void OnParentForeColorChanged(EventArgs e)
		{
			base.OnParentForeColorChanged(e);
			try
			{
				if (_allowParentOverrides)
					ForeColor = base.Parent.ForeColor;
			}
			catch (Exception)
			{
			}
		}

		protected override void OnParentFontChanged(EventArgs e)
		{
			base.OnParentFontChanged(e);
			try
			{
				if (_allowParentOverrides)
					Font = base.Parent.Font;
			}
			catch (Exception)
			{
			}
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			OnParentFontChanged(e);
			OnParentForeColorChanged(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (!AutoSize)
				Render();
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			if (_htmlContainer != null)
			{
				Graphics graphics = Utils.CreateGraphics(this);
				if (graphics != null)
				{
					using (graphics)
						using (GraphicsAdapter g = new GraphicsAdapter(graphics, _htmlContainer.UseGdiPlusTextRendering))
						{
							RSize rSize = HtmlRendererUtils.Layout(g, _htmlContainer.HtmlContainerInt, new RSize(base.ClientSize.Width - base.Padding.Horizontal, base.ClientSize.Height - base.Padding.Vertical), new RSize(MinimumSize.Width - base.Padding.Horizontal, MinimumSize.Height - base.Padding.Vertical), new RSize(MaximumSize.Width - base.Padding.Horizontal, MaximumSize.Height - base.Padding.Vertical), AutoSize, AutoSizeHeightOnly);
							base.ClientSize = Utils.ConvertRound(new RSize(rSize.Width + (double)base.Padding.Horizontal, rSize.Height + (double)base.Padding.Vertical));
						}
				}
			}
			base.OnLayout(levent);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_htmlContainer != null)
			{
				e.Graphics.TextRenderingHint = _textRenderingHint;
				_htmlContainer.Location = new PointF(base.Padding.Left, base.Padding.Top);
				_htmlContainer.PerformPaint(e.Graphics);
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (_htmlContainer != null)
				_htmlContainer.HandleMouseMove(this, e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_htmlContainer != null)
				_htmlContainer.HandleMouseDown(this, e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (_htmlContainer != null)
				_htmlContainer.HandleMouseLeave(this);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (_htmlContainer != null)
				_htmlContainer.HandleMouseUp(this, e);
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
			if (_htmlContainer != null)
				_htmlContainer.HandleMouseDoubleClick(this, e);
		}
	}
}
