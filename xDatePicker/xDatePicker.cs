using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using xCollection.Properties;

namespace x.UI.WinForms
{
	[Category("x UI For Windows Forms")]
	[DebuggerStepThrough]
	[ToolboxBitmap(typeof(DateTimePicker))]
	[Designer(typeof(xDesigner))]
	[Description("Add customizable Date pickers in your applications.")]
	public class xDatePicker : DateTimePicker
	{
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

		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					xDatePicker xDatePicker = (xDatePicker)base.Control;
					return SelectionRules.Moveable | SelectionRules.Visible | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
				}
			}

			public override DesignerActionListCollection ActionLists
			{
				get
				{
					if (actionLists == null)
						actionLists = new DesignerActionListCollection
						{
							new xControlActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class xControlActionList : DesignerActionList
		{
			private xDatePicker xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public bool FillDatePicker
			{
				get
				{
					return xControl.FillDatePicker;
				}
				set
				{
					SetValue(xControl, "FillDatePicker", value);
				}
			}

			public int BorderRadius
			{
				get
				{
					return xControl.BorderRadius;
				}
				set
				{
					SetValue(xControl, "BorderRadius", value);
				}
			}

			public int LeftTextMargin
			{
				get
				{
					return xControl.LeftTextMargin;
				}
				set
				{
					SetValue(xControl, "LeftTextMargin", value);
				}
			}

			public DateTime Value
			{
				get
				{
					return xControl.Value;
				}
				set
				{
					SetValue(xControl, "Value", value);
				}
			}

			public Color Color
			{
				get
				{
					return xControl.Color;
				}
				set
				{
					SetValue(xControl, "Color", value);
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
				}
			}

			public Color IconColor
			{
				get
				{
					return xControl.IconColor;
				}
				set
				{
					SetValue(xControl, "IconColor", value);
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
				}
			}

			public Image Icon
			{
				get
				{
					return xControl.Icon;
				}
				set
				{
					SetValue(xControl, "Icon", value);
				}
			}

			public BorderThickness DateBorderThickness
			{
				get
				{
					return xControl.DateBorderThickness;
				}
				set
				{
					SetValue(xControl, "DateBorderThickness", value);
				}
			}

			public Indicator IconLocation
			{
				get
				{
					return xControl.IconLocation;
				}
				set
				{
					SetValue(xControl, "IconLocation", value);
				}
			}

			public xControlActionList(IComponent component)
				: base(component)
			{
				xControl = component as xDatePicker;
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
				return new DesignerActionItemCollection
				{
					new DesignerActionHeaderItem("Common Tasks"),
					new DesignerActionHeaderItem("Appearance"),
					new DesignerActionHeaderItem("Behavior"),
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)                 ", "Common Tasks"),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Font", "Font:", "Common Tasks", GetPropertyDescriptor(base.Component, "Font").Description),
					new DesignerActionPropertyItem("LeftTextMargin", "LeftTextMargin:", "Behavior", GetPropertyDescriptor(base.Component, "LeftTextMargin").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Appearance", GetPropertyDescriptor(base.Component, "BorderRadius").Description),
					new DesignerActionPropertyItem("DateBorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "DateBorderThickness").Description),
					new DesignerActionPropertyItem("FillDatePicker", "FillDatePicker", "Appearance", GetPropertyDescriptor(base.Component, "FillDatePicker").Description),
					new DesignerActionPropertyItem("Color", "Color:", "Appearance", GetPropertyDescriptor(base.Component, "Color").Description),
					new DesignerActionPropertyItem("ForeColor", "ForeColor:", "Appearance", GetPropertyDescriptor(base.Component, "ForeColor").Description),
					new DesignerActionPropertyItem("IconColor", "IconColor:", "Appearance", GetPropertyDescriptor(base.Component, "IconColor").Description),
					new DesignerActionPropertyItem("Icon", "Icon:", "Appearance", GetPropertyDescriptor(base.Component, "Icon").Description),
					new DesignerActionPropertyItem("IconLocation", "IconLocation:", "Appearance", GetPropertyDescriptor(base.Component, "IconLocation").Description),
					new DesignerActionMethodItem(this, "Refresh", "Refresh Control Surface", "")
				};
			}

			public void Refresh()
			{
				try
				{
					xControl.Refresh();
				}
				catch (Exception)
				{
				}
			}
		}

		private bool _heightLess;

		private bool _fillPicker = false;

		private bool _fillIndicator = false;

		private string _text;

		private int _height;

		private int _radius = 1;

		private int _leftTextMargin = 5;

		private const int McmFirst = 4096;

		private const int DtmFirst = 4096;

		private const int McsWeeknumbers = 4;

		private const int DtmGetmonthcal = 4104;

		private const int McmGetminreqrect = 4105;

		private Color _iconColor;

		private Color _color = Color.Purple;

		private Color _disabledColor = Color.Gray;

		private Color _pickerColor = Color.Purple;

		private TextAlign _textAlign = TextAlign.Left;

		private Indicator _indicatorLocation = Indicator.Right;

		private BorderThickness _borderthickness = BorderThickness.Thick;

		private Image _icon;

		private Control ParentControl = null;

		private IContainer components = null;

		[Description("Sets a value indicating whether the control is enabled.")]
		[Category("x Properties")]
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
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the week numbers will be displayed.")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public virtual bool DisplayWeekNumbers { get; set; }

		[Description("Sets a value indicating whether the date picker will be filled using the 'Color' property.")]
		[Browsable(true)]
		public virtual bool FillDatePicker
		{
			get
			{
				return _fillPicker;
			}
			set
			{
				_fillPicker = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the border radius.")]
		public virtual int BorderRadius
		{
			get
			{
				int num = Height / 2 - 3 + 4;
				if (DateBorderThickness == BorderThickness.Thick)
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

		[Description("Sets minimum width of the date picker.")]
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
				Invalidate();
			}
		}

		[Browsable(true)]
		[Description("Sets minimum height of the date picker.")]
		public new virtual int Height
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height = value;
				Refresh();
			}
		}

		[Description("Sets the left text margin.")]
		[Category("x Properties")]
		public virtual int LeftTextMargin
		{
			get
			{
				return _leftTextMargin;
			}
			set
			{
				_leftTextMargin = value;
				Refresh();
			}
		}

		[Description("Sets the date picker icon color.")]
		[Category("x Properties")]
		public virtual Color IconColor
		{
			get
			{
				return _iconColor;
			}
			set
			{
				_iconColor = value;
				_icon = ChangeColor(new Bitmap(Icon), _iconColor);
				Refresh();
			}
		}

		[Description("Sets the disabled background/border color.")]
		[Category("x Properties")]
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

		[Category("x Properties")]
		[Description("Sets the border/background color.")]
		public virtual Color Color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				Refresh();
			}
		}

		[Browsable(true)]
		[Description("Gets or sets the background color.")]
		[Category("x Properties")]
		public new virtual Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Browsable(true)]
		[Description("Sets the foreground color.")]
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

		[Description("Sets the indicator icon.")]
		[Browsable(true)]
		[Category("x Properties")]
		public virtual Image Icon
		{
			get
			{
				return _icon;
			}
			set
			{
				if (value != null)
					_icon = value;
				else
					IconColor = ParentControl.BackColor;
				Refresh();
			}
		}

		[Browsable(true)]
		[Category("x Properties")]
		[Description("Sets the control font.")]
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

		[Description("Sets the border thickness.")]
		[Browsable(true)]
		[Category("x Properties")]
		public virtual BorderThickness DateBorderThickness
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

		[Description("Sets the date/time text alignment.")]
		[Browsable(false)]
		[Category("x Properties")]
		public virtual TextAlign DateTextAlign
		{
			get
			{
				return _textAlign;
			}
			set
			{
				_textAlign = value;
				switch (value)
				{
				case TextAlign.Right:
					_indicatorLocation = Indicator.Left;
					break;
				case TextAlign.Left:
					_indicatorLocation = Indicator.Right;
					break;
				case TextAlign.Center:
					_indicatorLocation = Indicator.None;
					break;
				}
				Refresh();
			}
		}

		[Browsable(true)]
		[RefreshProperties(RefreshProperties.All)]
		[Category("x Properties")]
		[Description("Sets the indicator location.")]
		public virtual Indicator IconLocation
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
					base.DropDownAlign = LeftRightAlignment.Right;
					_textAlign = TextAlign.Left;
					break;
				case Indicator.Left:
					base.DropDownAlign = LeftRightAlignment.Left;
					_textAlign = TextAlign.Right;
					break;
				case Indicator.None:
					_textAlign = TextAlign.Center;
					break;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool RightToLeftLayout
		{
			get
			{
				return base.RightToLeftLayout;
			}
			set
			{
				base.RightToLeftLayout = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public new virtual bool ShowUpDown
		{
			get
			{
				return base.ShowUpDown;
			}
			set
			{
				base.ShowUpDown = value;
			}
		}

		[Browsable(false)]
		public virtual int DPHeight
		{
			get
			{
				return _height;
			}
			set
			{
				_height = value;
				Refresh();
			}
		}

		public xDatePicker()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Selectable, true);
			Initialize();
		}

		[DllImport("User32.dll")]
		private static extern int GetWindowLong(IntPtr handleToWindow, int offsetToValueToGet);

		[DllImport("User32.dll")]
		private static extern int SetWindowLong(IntPtr h, int index, int value);

		[DllImport("User32.dll")]
		private static extern IntPtr SendMessage(IntPtr h, int msg, int param, int data);

		[DllImport("User32.dll")]
		private static extern IntPtr GetParent(IntPtr h);

		[DllImport("User32.dll")]
		private static extern int SendMessage(IntPtr h, int msg, int param, ref Rectangle data);

		[DllImport("User32.dll")]
		private static extern int MoveWindow(IntPtr h, int x, int y, int width, int height, bool repaint);

		public static Bitmap ChangeColor(Bitmap image, Color color)
		{
			Bitmap bitmap = new Bitmap(image.Width, image.Height);
			for (int i = 0; i < image.Width; i++)
			{
				for (int j = 0; j < image.Height; j++)
				{
					Color pixel = image.GetPixel(i, j);
					if (pixel.A <= 150)
						bitmap.SetPixel(i, j, pixel);
					else
						bitmap.SetPixel(i, j, color);
				}
			}
			return bitmap;
		}

		private void Initialize()
		{
			Width = 220;
			Height = 32;
			Icon = Resources.calendar;
			Color = Color.Silver;
			ForeColor = Color.Black;
			IconColor = Color.Gray;
			BackColor = Color.Transparent;
			MinimumSize = new Size(0, 32);
			DateBorderThickness = BorderThickness.Thin;
			Font = new Font("Segoe UI", 9f, FontStyle.Regular);
		}

		private void Draw(Graphics graphics, Color lineColor, Color foreColor)
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.InterpolationMode = InterpolationMode.High;
			SolidBrush brush = new SolidBrush(lineColor);
			Pen pen;
			int margin;
			if (DateBorderThickness == BorderThickness.Thick)
			{
				pen = new Pen(brush, 2f);
				margin = 1;
			}
			else
			{
				pen = new Pen(brush, 1f);
				margin = 0;
			}
			Rectangle rectangle = new Rectangle(base.ClientRectangle.X, base.ClientRectangle.Y, base.ClientRectangle.Width - 1, base.ClientRectangle.Height - 1);
			using (GraphicsPath path = RoundedRectangle(rectangle, BorderRadius, margin))
			{
				if (_fillPicker)
					graphics.FillPath(brush, path);
				else
					graphics.DrawPath(pen, path);
			}
			if (IconLocation == Indicator.Left)
			{
				Rectangle rect = new Rectangle(new Point(base.ClientRectangle.X + Height / 2 - 4, base.ClientRectangle.Y + Height / 4), new Size(base.ClientRectangle.Height / 2, base.ClientRectangle.Height / 2));
				graphics.DrawImage(Icon, rect);
				DrawString(graphics, Text, foreColor);
			}
			else if (IconLocation == Indicator.Right)
			{
				Rectangle rect2 = new Rectangle(new Point(base.ClientRectangle.X + Width - Height - 1 + 5, base.ClientRectangle.Y + Height / 4), new Size(base.ClientRectangle.Height / 2, base.ClientRectangle.Height / 2));
				graphics.DrawImage(Icon, rect2);
				DrawString(graphics, Text, foreColor);
			}
			else if (IconLocation == Indicator.None)
			{
				Rectangle rect3 = new Rectangle(new Point(base.ClientRectangle.X + Height / 2 - 1, base.ClientRectangle.Y + Height / 4), new Size(base.ClientRectangle.Height / 2, base.ClientRectangle.Height / 2));
				graphics.DrawImage(Icon, rect3);
				DrawString(graphics, Text, foreColor);
			}
		}

		private void DrawString(Graphics graphics, string text, Color color)
		{
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
			if (DateTextAlign == TextAlign.Right)
				bounds = new Rectangle(_leftTextMargin, 0, Width - _leftTextMargin * 2 - 1, Height - 1);
			else if (DateTextAlign == TextAlign.Left)
			{
				bounds = new Rectangle(_leftTextMargin, 0, Width - 1, Height - 1);
			}
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			TextRenderer.DrawText(flags: (DateTextAlign == TextAlign.Left) ? (TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : ((DateTextAlign == TextAlign.Right) ? (TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : ((DateTextAlign != TextAlign.Center) ? (TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding) : (TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.WordEllipsis | TextFormatFlags.LeftAndRightPadding))), dc: graphics, text: text, font: Font, bounds: bounds, foreColor: color);
		}

		private GraphicsPath RoundedRectangle(Rectangle rectangle, int radius, int margin)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			if (radius > 1)
			{
				graphicsPath.AddArc(rectangle.X + margin, rectangle.Y + margin, radius * 2, radius * 2, 180f, 90f);
				graphicsPath.AddArc(rectangle.X + rectangle.Width - margin - radius * 2, rectangle.Y + margin, radius * 2, radius * 2, 270f, 90f);
				graphicsPath.AddArc(rectangle.X + rectangle.Width - margin - radius * 2, rectangle.Y + rectangle.Height - margin - radius * 2, radius * 2, radius * 2, 0f, 90f);
				graphicsPath.AddArc(rectangle.X + margin, rectangle.Y + rectangle.Height - margin - radius * 2, radius * 2, radius * 2, 90f, 90f);
				graphicsPath.CloseFigure();
			}
			else
				graphicsPath.AddRectangle(rectangle);
			return graphicsPath;
		}

		private void OnResize(object sender, EventArgs e)
		{
			if (Width < 217)
				_heightLess = true;
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				SendKeys.Send("%{DOWN}");
		}

		private void OnParentChanged(object sender, EventArgs e)
		{
			ParentControl = base.Parent;
		}

		protected override void OnPaint(PaintEventArgs paintEvent)
		{
			if (Enabled)
			{
				Draw(paintEvent.Graphics, Color, ForeColor);
				base.OnPaint(paintEvent);
			}
			else
			{
				Draw(paintEvent.Graphics, DisabledColor, DisabledColor);
				base.OnPaint(paintEvent);
			}
		}

		protected override void OnDropDown(EventArgs e)
		{
			if (!_heightLess)
			{
				IntPtr intPtr = SendMessage(base.Handle, 4104, 0, 0);
				int windowLong = GetWindowLong(intPtr, -16);
				windowLong = ((!DisplayWeekNumbers) ? (windowLong & -5) : (windowLong | 4));
				Rectangle data = default(Rectangle);
				SendMessage(intPtr, 4105, 0, ref data);
				data.Width = base.ClientRectangle.Width;
				data.Height += 6;
				SetWindowLong(intPtr, -16, windowLong);
				MoveWindow(intPtr, 0, 0, data.Right, data.Bottom, true);
				IntPtr h = GetParent(intPtr);
				MoveWindow(h, 0, 0, data.Right, data.Bottom, true);
				base.OnDropDown(e);
			}
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
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			base.Resize += new System.EventHandler(OnResize);
			base.ParentChanged += new System.EventHandler(OnParentChanged);
			base.ResumeLayout(false);
		}
	}
}
