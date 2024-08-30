using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace x.UI.WinForms
{
	[ToolboxBitmap(typeof(ComboBox))]
	[DefaultProperty("Items")]
	[Category("x UI For Windows Forms")]
	[Description("Bring style and elegance to your item selections with x Dropdown.")]
	[DefaultEvent("SelectedIndexChanged")]
	[DebuggerStepThrough]
	public class xDropdown : ComboBox
	{
		public enum Directions
		{
			Up,
			Down
		}

		public enum Indicator
		{
			Left,
			Right,
			None
		}

		public enum TextAlign
		{
			Left,
			Right,
			Center
		}

		public enum BorderThickness
		{
			Thick,
			Thin
		}

		private bool _filldropdown = true;

		private bool _fillindicator = false;

		private int _radius = 1;

		private int _itemTopMargin = 1;

		private int _textLeftMargin = 5;

		private const int CB_SETITEMHEIGHT = 339;

		private string _text;

		private Color _disabledIndicatorColor = Color.DarkGray;

		private Color _disabledColor = Color.FromArgb(240, 240, 240);

		private Color _disabledForeColor = Color.FromArgb(109, 109, 109);

		private Color _disabledBorderColor = Color.FromArgb(204, 204, 204);

		private Color _borderColor = Color.Silver;

		private Color _backColor = Color.White;

		private Color _indicatorColor = Color.Purple;

		private Color _itemForeColor = Color.Purple;

		private Color _itemsBackColor = Color.White;

		private Color _itemsBorderColor = Color.White;

		private Color _highlightColor = Color.LightSlateGray;

		private Color _highlightForeColor = Color.White;

		private Image _icon = null;

		private Graphics _graphics;

		private Control ParentControl = null;

		private BorderThickness _borderthickness = BorderThickness.Thick;

		private Directions _direction = Directions.Down;

		private Indicator _indicatorLocation = Indicator.Right;

		private TextAlign _textalign = TextAlign.Left;

		private IContainer components = null;

		[Category("x Properties")]
		[Description("Enables or Disables the control")]
		[Browsable(true)]
		public new virtual bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
			}
		}

		[Description("Sets a value inidicating whether the indicator will be filled.")]
		[Category("x Properties")]
		public virtual bool FillIndicator
		{
			get
			{
				return _fillindicator;
			}
			set
			{
				_fillindicator = value;
				Refresh();
			}
		}

		[Description("Sets the dropdown's height.")]
		[Category("x Properties")]
		[Browsable(true)]
		public new virtual int ItemHeight
		{
			get
			{
				if (base.ItemHeight < 15)
					base.ItemHeight = 15;
				return base.ItemHeight;
			}
			set
			{
				base.ItemHeight = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's border radius.")]
		public virtual int BorderRadius
		{
			get
			{
				int num = ItemHeight / 2 + 4;
				if (DropdownBorderThickness == BorderThickness.Thick)
				{
					if (_radius > num - 4)
						_radius = num - 4;
				}
				else if (_radius > num)
				{
					_radius = num;
				}
				return _radius;
			}
			set
			{
				_radius = value;
				Refresh();
			}
		}

		[Description("Sets the margin between the dropdown's text and its borders.")]
		[Category("x Properties")]
		public virtual int TextLeftMargin
		{
			get
			{
				return _textLeftMargin;
			}
			set
			{
				_textLeftMargin = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the margin between each dropdown's item and the items container.")]
		public virtual int ItemTopMargin
		{
			get
			{
				return _itemTopMargin;
			}
			set
			{
				_itemTopMargin = value;
				Refresh();
			}
		}

		[Browsable(true)]
		[Description("Sets the dropdown's item height.")]
		public new virtual int Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height = value;
				ItemHeight = value;
				Refresh();
			}
		}

		[Description("Sets dropdown's item width.")]
		[Browsable(true)]
		public new virtual int Width
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width = value;
				Refresh();
			}
		}

		[Description("Sets the dropdown's text.")]
		[Category("x Properties")]
		public override string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				Refresh();
			}
		}

		[Description("Sets the indicator color.")]
		[Category("x Properties")]
		public virtual Color IndicatorColor
		{
			get
			{
				return _indicatorColor;
			}
			set
			{
				_indicatorColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the default items' fore color.")]
		public virtual Color ItemForeColor
		{
			get
			{
				return _itemForeColor;
			}
			set
			{
				_itemForeColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's back color when disabled.")]
		public virtual Color DisabledBackColor
		{
			get
			{
				return _disabledColor;
			}
			set
			{
				_disabledColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's border color when disabled.")]
		public virtual Color DisabledBorderColor
		{
			get
			{
				return _disabledBorderColor;
			}
			set
			{
				_disabledBorderColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's indicator color when disabled.")]
		public virtual Color DisabledIndicatorColor
		{
			get
			{
				return _disabledIndicatorColor;
			}
			set
			{
				_disabledIndicatorColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's fore color when disabled.")]
		public virtual Color DisabledForeColor
		{
			get
			{
				return _disabledForeColor;
			}
			set
			{
				_disabledForeColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[DisplayName("ItemHighlightColor")]
		[Description("Sets the highlighted item's back color.")]
		public virtual Color ItemHighLightColor
		{
			get
			{
				return _highlightColor;
			}
			set
			{
				_highlightColor = value;
				Refresh();
			}
		}

		[Description("Sets the highlighted item's fore color.")]
		[DisplayName("ItemHighlightForeColor")]
		[Category("x Properties")]
		public virtual Color ItemHighLightForeColor
		{
			get
			{
				return _highlightForeColor;
			}
			set
			{
				_highlightForeColor = value;
				Refresh();
			}
		}

		[Description("Sets dropdown's border color.")]
		[Category("x Properties")]
		public virtual Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[DisplayName("BackColor")]
		[Description("Sets the dropdown's background color.")]
		public virtual Color BackgroundColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's items border color.")]
		public virtual Color ItemBorderColor
		{
			get
			{
				return _itemsBorderColor;
			}
			set
			{
				_itemsBorderColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the background color of the dropdown list.")]
		public virtual Color ItemBackColor
		{
			get
			{
				return _itemsBackColor;
			}
			set
			{
				_itemsBackColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's fore color.")]
		public new virtual Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the dropdown's font.")]
		public new virtual Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
				Refresh();
			}
		}

		[Description("Sets the dropdown's border thickness.")]
		[Category("x Properties")]
		public virtual BorderThickness DropdownBorderThickness
		{
			get
			{
				return _borderthickness;
			}
			set
			{
				_borderthickness = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the default text alignment.")]
		public virtual TextAlign TextAlignment
		{
			get
			{
				return _textalign;
			}
			set
			{
				_textalign = value;
				switch (value)
				{
				case TextAlign.Right:
					_indicatorLocation = Indicator.Left;
					break;
				case TextAlign.Left:
					_indicatorLocation = Indicator.Right;
					break;
				}
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the indicator's alignment position.")]
		[RefreshProperties(RefreshProperties.All)]
		public virtual Indicator IndicatorAlignment
		{
			get
			{
				return _indicatorLocation;
			}
			set
			{
				_indicatorLocation = value;
				switch (value)
				{
				case Indicator.Right:
					_textalign = TextAlign.Left;
					break;
				case Indicator.Left:
					_textalign = TextAlign.Right;
					break;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual bool FillDropDown
		{
			get
			{
				return _filldropdown;
			}
			set
			{
				_filldropdown = value;
				if (_filldropdown)
					IndicatorColor = Color.White;
				else
					IndicatorColor = Color;
				Refresh();
			}
		}

		[Browsable(false)]
		public new virtual int SelectedIndex
		{
			get
			{
				return base.SelectedIndex;
			}
			set
			{
				base.SelectedIndex = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Color Color
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Color DisabledColor
		{
			get
			{
				return _disabledColor;
			}
			set
			{
				_disabledColor = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Image Icon
		{
			get
			{
				return _icon;
			}
			set
			{
				_icon = value;
			}
		}

		[Browsable(false)]
		public virtual Directions Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual TextAlign DropDownTextAlign
		{
			get
			{
				return _textalign;
			}
			set
			{
				_textalign = value;
				switch (value)
				{
				case TextAlign.Right:
					_indicatorLocation = Indicator.Left;
					break;
				case TextAlign.Left:
					_indicatorLocation = Indicator.Right;
					break;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Indicator IndicatorLocation
		{
			get
			{
				return _indicatorLocation;
			}
			set
			{
				_indicatorLocation = value;
				switch (value)
				{
				case Indicator.Right:
					_textalign = TextAlign.Left;
					break;
				case Indicator.Left:
					_textalign = TextAlign.Right;
					break;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new virtual ComboBoxStyle DropDownStyle
		{
			get
			{
				return base.DropDownStyle;
			}
			set
			{
				base.DropDownStyle = value;
				Refresh();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new virtual FlatStyle FlatStyle
		{
			get
			{
				return base.FlatStyle;
			}
			set
			{
				base.FlatStyle = value;
				Refresh();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new virtual DrawMode DrawMode
		{
			get
			{
				return base.DrawMode;
			}
			set
			{
				base.DrawMode = value;
				Refresh();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		public xDropdown()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			FlatStyle = FlatStyle.Flat;
			DrawMode = DrawMode.Normal;
			DropDownStyle = ComboBoxStyle.DropDownList;
			Width = 260;
			Height = 32;
			ItemHeight = 26;
			DrawMode = DrawMode.OwnerDrawFixed;
			base.DrawItem += xDropdown_DrawItem;
			ForeColor = Color.Black;
			BorderColor = Color.Silver;
			ItemBackColor = Color.White;
			ItemForeColor = Color.Black;
			ItemBorderColor = Color.White;
			ItemTopMargin = 3;
			IndicatorColor = Color.Gray;
			ItemHighLightForeColor = Color.White;
			ItemHighLightColor = Color.DodgerBlue;
			DropdownBorderThickness = BorderThickness.Thin;
			Font = new Font("Segoe UI", 9f, FontStyle.Regular);
			BackColor = Color.Transparent;
		}

		private void DrawIndicator(Graphics graphics, Color lineColor, Color indicatorColor, Color foreColor)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.High;
			SolidBrush solidBrush = new SolidBrush(_borderColor);
			SolidBrush solidBrush2 = new SolidBrush(_backColor);
			if (!Enabled)
			{
				indicatorColor = _disabledIndicatorColor;
				solidBrush.Color = _disabledBorderColor;
				solidBrush2.Color = _disabledColor;
			}
			Pen pen;
			int margin;
			if (DropdownBorderThickness == BorderThickness.Thick)
			{
				pen = new Pen(solidBrush, 2f);
				margin = 1;
			}
			else
			{
				pen = new Pen(solidBrush, 1f);
				margin = 0;
			}
			Rectangle rectangle = new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width - 1, base.ClientRectangle.Height - 1);
			using (GraphicsPath path = DrawRoundedRectangle(rectangle, BorderRadius, margin))
			{
				graphics.FillPath(solidBrush2, path);
				graphics.DrawPath(pen, path);
			}
			if (_indicatorLocation == Indicator.Left)
			{
				Rectangle rect = new Rectangle(new Point(base.ClientRectangle.X + (_textLeftMargin * 2 - 3), base.ClientRectangle.Y + Height / 2 - 3), new Size(base.ClientRectangle.Height / 2, base.ClientRectangle.Height / 4));
				DrawTriangle(graphics, rect, Directions.Down, indicatorColor);
				DrawString(graphics, Text, foreColor);
			}
			else if (_indicatorLocation == Indicator.Right)
			{
				Rectangle rect2 = new Rectangle(new Point(base.ClientRectangle.X + Width - Height, base.ClientRectangle.Y + Height / 2 - 3), new Size(base.ClientRectangle.Height / 2, base.ClientRectangle.Height / 4));
				DrawTriangle(graphics, rect2, Directions.Down, indicatorColor);
				DrawString(graphics, Text, foreColor);
			}
			else if (_indicatorLocation == Indicator.None)
			{
				DrawString(graphics, Text, foreColor);
			}
			solidBrush.Dispose();
			solidBrush2.Dispose();
			pen.Dispose();
		}

		private void DrawString(Graphics graphics, string text, Color foreColor)
		{
			if (!Enabled)
				foreColor = _disabledForeColor;
			Rectangle bounds = new Rectangle(_textLeftMargin, 0, Width - _textLeftMargin * 2, Height - 1);
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			TextRenderer.DrawText(flags: (DropDownTextAlign == TextAlign.Left) ? (TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : ((DropDownTextAlign == TextAlign.Right) ? (TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : ((DropDownTextAlign != TextAlign.Center) ? (TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : (TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding))), dc: graphics, text: text, font: Font, bounds: bounds, foreColor: foreColor);
		}

		private void DrawImage(Graphics graphics, Rectangle rectangle, Image image)
		{
			graphics.DrawImage(image, rectangle, 50, 50, 150, 150, GraphicsUnit.Pixel);
		}

		private void SetComboBoxHeight(IntPtr comboBoxHandle, int comboBoxDesiredHeight)
		{
			SendMessage(comboBoxHandle, 339u, -1, comboBoxDesiredHeight);
		}

		private void DrawTriangle(Graphics g, Rectangle rect, Directions direction, Color backColor)
		{
			rect.X = rect.Left + 4;
			int num = rect.Width / 2;
			int num2 = rect.Height / 2;
			Point point = Point.Empty;
			Point point2 = Point.Empty;
			Point point3 = Point.Empty;
			switch (direction)
			{
			case Directions.Down:
				point = new Point(rect.Left + num, rect.Bottom - 1);
				point2 = new Point(rect.Left + 2, rect.Top);
				point3 = new Point(rect.Right - 2, rect.Top);
				break;
			case Directions.Up:
				point = new Point(rect.Left + num, rect.Top);
				point2 = new Point(rect.Left, rect.Bottom);
				point3 = new Point(rect.Right, rect.Bottom);
				break;
			}
			SolidBrush solidBrush = new SolidBrush(backColor);
			Pen pen = ((DropdownBorderThickness != 0) ? new Pen(solidBrush, 1f) : new Pen(solidBrush, 2f));
			if (FillIndicator)
				g.FillPolygon(solidBrush, new Point[3] { point, point2, point3 });
			else
				g.DrawPolygon(pen, new Point[4] { point, point2, point, point3 });
			solidBrush.Dispose();
			pen.Dispose();
		}

		private GraphicsPath DrawRoundedRectangle(Rectangle rectangle, int cornerRadius, int margin)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			if (cornerRadius > 1)
			{
				graphicsPath.AddArc(rectangle.X + margin, rectangle.Y + margin, cornerRadius * 2, cornerRadius * 2, 180f, 90f);
				graphicsPath.AddArc(rectangle.X + rectangle.Width - margin - cornerRadius * 2, rectangle.Y + margin, cornerRadius * 2, cornerRadius * 2, 270f, 90f);
				graphicsPath.AddArc(rectangle.X + rectangle.Width - margin - cornerRadius * 2, rectangle.Y + rectangle.Height - margin - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0f, 90f);
				graphicsPath.AddArc(rectangle.X + margin, rectangle.Y + rectangle.Height - margin - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90f, 90f);
				graphicsPath.CloseFigure();
			}
			else
				graphicsPath.AddRectangle(rectangle);
			return graphicsPath;
		}

		private static GraphicsPath DrawRoundRectangle(Rectangle rectangle, int topLeftRadius, int topRightRadius, int bottomRightRadius, int bottomLeftRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			int left = rectangle.Left;
			int top = rectangle.Top;
			int num = rectangle.Width;
			int num2 = rectangle.Height;
			if (topLeftRadius > 0)
				graphicsPath.AddArc(left, top, topLeftRadius * 2, topLeftRadius * 2, 180f, 90f);
			graphicsPath.AddLine(left + topLeftRadius, top, left + num - topRightRadius, top);
			if (topRightRadius > 0)
				graphicsPath.AddArc(left + num - topRightRadius * 2, top, topRightRadius * 2, topRightRadius * 2, 270f, 90f);
			graphicsPath.AddLine(left + num, top + topRightRadius, left + num, top + num2 - bottomRightRadius);
			if (bottomRightRadius > 0)
				graphicsPath.AddArc(left + num - bottomRightRadius * 2, top + num2 - bottomRightRadius * 2, bottomRightRadius * 2, bottomRightRadius * 2, 0f, 90f);
			graphicsPath.AddLine(left + num - bottomRightRadius, top + num2, left + bottomLeftRadius, top + num2);
			if (bottomLeftRadius > 0)
				graphicsPath.AddArc(left, top + num2 - bottomLeftRadius * 2, bottomLeftRadius * 2, bottomLeftRadius * 2, 90f, 90f);
			graphicsPath.AddLine(left, top + num2 - bottomLeftRadius, left, top + topLeftRadius);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		[DllImport("user32.dll")]
		private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

		protected override void OnPaint(PaintEventArgs pe)
		{
			if (Enabled)
				DrawIndicator(pe.Graphics, Color, IndicatorColor, ForeColor);
			else
				DrawIndicator(pe.Graphics, _disabledBorderColor, _disabledBorderColor, _disabledForeColor);
			base.OnPaint(pe);
		}

		private void xDropdown_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0 || (e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit)
				return;
			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus || (e.State & DrawItemState.Selected) == DrawItemState.Selected || (e.State & DrawItemState.HotLight) == DrawItemState.HotLight)
				e.DrawBackground();
			else
			{
				using (Brush brush = new SolidBrush(ItemBackColor))
					e.Graphics.FillRectangle(brush, e.Bounds);
			}
			e.Graphics.DrawString(GetItemText(base.Items[e.Index]), Font, new SolidBrush(ItemForeColor), new RectangleF(e.Bounds.X + _textLeftMargin, e.Bounds.Y + _itemTopMargin, e.Bounds.Width, e.Bounds.Height));
			if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
				e.DrawFocusRectangle();
			using (Pen pen = new Pen(ItemBorderColor, 0f))
			{
				Point pt;
				Point pt2;
				if (e.Index == 0)
				{
					pt = new Point(e.Bounds.Left, e.Bounds.Top);
					pt2 = new Point(e.Bounds.Left + e.Bounds.Width - 1, e.Bounds.Top);
					e.Graphics.DrawLine(pen, pt, pt2);
				}
				pt = new Point(e.Bounds.Left, e.Bounds.Top);
				pt2 = new Point(e.Bounds.Left, e.Bounds.Top + e.Bounds.Height - 1);
				e.Graphics.DrawLine(pen, pt, pt2);
				pt = new Point(e.Bounds.Left + e.Bounds.Width - 1, e.Bounds.Top);
				pt2 = new Point(e.Bounds.Left + e.Bounds.Width - 1, e.Bounds.Top + e.Bounds.Height - 1);
				e.Graphics.DrawLine(pen, pt, pt2);
				if (e.Index == base.Items.Count - 1)
				{
					pt = new Point(e.Bounds.Left, e.Bounds.Top + e.Bounds.Height - 1);
					pt2 = new Point(e.Bounds.Left + e.Bounds.Width - 1, e.Bounds.Top + e.Bounds.Height - 1);
					e.Graphics.DrawLine(pen, pt, pt2);
				}
			}
			if (e.Index >= 0)
			{
				Color itemForeColor = ItemForeColor;
				if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
				{
					e.Graphics.FillRectangle(new SolidBrush(ItemHighLightColor), e.Bounds);
					itemForeColor = ItemHighLightForeColor;
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(ItemBackColor), e.Bounds);
					itemForeColor = ItemForeColor;
				}
				e.Graphics.DrawString(GetItemText(base.Items[e.Index]), e.Font, new SolidBrush(itemForeColor), new Point(e.Bounds.X + _textLeftMargin, e.Bounds.Y + _itemTopMargin));
				e.DrawFocusRectangle();
				e.Graphics.Dispose();
			}
		}

		private void xDropdown_ParentChanged(object sender, EventArgs e)
		{
			ParentControl = base.Parent;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.ParentChanged += new System.EventHandler(xDropdown_ParentChanged);
			base.ResumeLayout(false);
		}
	}
}
