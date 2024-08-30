using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace xCollection
{
	[DefaultProperty("Value")]
	[DefaultEvent("ProgressChanged")]
	[Description("Add stylish progress bars to depict the flow of operations in your apps.")]
	[Category("x UI For Windows Forms")]
	[Designer(typeof(xDesigner))]
	[ToolboxBitmap(typeof(ProgressBar))]
	[DebuggerStepThrough]
	public class xProgressBar : UserControl
	{
		public class ProgressChangedEventArgs : EventArgs
		{
			private int _value;

			private int _maximum;

			public int ProgressValue
			{
				get
				{
					return _value;
				}
			}

			public double ProgressPercentage
			{
				get
				{
					return (double)_value / (double)_maximum * 100.0;
				}
			}

			public ProgressChangedEventArgs(int value, int maximum)
			{
				_value = value;
				_maximum = maximum;
			}
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
					if (((xProgressBar)base.Control).AutoSize)
						return SelectionRules.Moveable | SelectionRules.Visible;
					return SelectionRules.AllSizeable | SelectionRules.Moveable | SelectionRules.Visible;
				}
			}

			public override DesignerActionListCollection ActionLists
			{
				get
				{
					if (actionLists == null)
						actionLists = new DesignerActionListCollection
						{
							new xControlDesignerActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerStepThrough]
		public class xControlDesignerActionList : DesignerActionList
		{
			private xProgressBar Control;

			private DesignerActionUIService designerActionUISvc = null;

			public int Value
			{
				get
				{
					return Control.Value;
				}
				set
				{
					SetValue(Control, "Value", value);
				}
			}

			public int Maximum
			{
				get
				{
					return Control.Maximum;
				}
				set
				{
					SetValue(Control, "Maximum", value);
				}
			}

			public int Minimum
			{
				get
				{
					return Control.Minimum;
				}
				set
				{
					SetValue(Control, "Minimum", value);
				}
			}

			public int BorderRadius
			{
				get
				{
					return Control.BorderRadius;
				}
				set
				{
					SetValue(Control, "BorderRadius", value);
				}
			}

			public Color BackColor
			{
				get
				{
					return Control.BackColor;
				}
				set
				{
					SetValue(Control, "BackColor", value);
				}
			}

			public Color BorderColor
			{
				get
				{
					return Control.BorderColor;
				}
				set
				{
					SetValue(Control, "BorderColor", value);
				}
			}

			public Color ProgressColorLeft
			{
				get
				{
					return Control.ProgressColorLeft;
				}
				set
				{
					SetValue(Control, "ProgressColorLeft", value);
				}
			}

			public Color ProgressColorRight
			{
				get
				{
					return Control.ProgressColorRight;
				}
				set
				{
					SetValue(Control, "ProgressColorRight", value);
				}
			}

			public Orientation Orientation
			{
				get
				{
					return Control.Orientation;
				}
				set
				{
					SetValue(Control, "Orientation", value);
				}
			}

			public xControlDesignerActionList(IComponent component)
				: base(component)
			{
				Control = component as xProgressBar;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)      ", "Common Tasks"),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("Minimum", "Minimum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Minimum").Description),
					new DesignerActionPropertyItem("Orientation", "Orientation:", "Common Tasks", GetPropertyDescriptor(base.Component, "Orientation").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Appearance", GetPropertyDescriptor(base.Component, "BorderRadius").Description),
					new DesignerActionPropertyItem("BackColor", "BackColor:", "Appearance", GetPropertyDescriptor(base.Component, "BackColor").Description),
					new DesignerActionPropertyItem("BorderColor", "BorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "BorderColor").Description),
					new DesignerActionPropertyItem("ProgressColorLeft", "ProgressColorLeft:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColorLeft").Description),
					new DesignerActionPropertyItem("ProgressColorRight", "ProgressColorRight:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColorRight").Description)
				};
			}
		}

		private double CONST = 1.6;

		private int _value = 50;

		private int _minimum = 0;

		private int _maximum = 100;

		private int _borderRadius = 9;

		private int _animationStep = 10;

		private int _borderThickness = 1;

		private int _currentProgressWidth = 0;

		private Color _progressColorLeft = Color.DodgerBlue;

		private Color _progressColorRight = Color.DodgerBlue;

		private Color _borderColor = Color.FromArgb(223, 223, 223);

		private Color _progressBackColor = Color.FromArgb(223, 223, 223);

		private Orientation _orientation = Orientation.Horizontal;

		private IContainer components = null;

		private PictureBox pnlProgress;

		private PictureBox pbBuffer;

		[Category("x Properties")]
		[Browsable(false)]
		[Description("Sets a value indicating whether animations are enabled when the progress value is changing. You can now use the property 'ValueByTransition' to animate progress-value movements at runtime.")]
		public bool AllowAnimations
		{
			get
			{
				if (Animation <= 0)
					return false;
				return true;
			}
			set
			{
				if (value)
					Animation = 1;
				else
					Animation = 0;
			}
		}

		[Description("Sets the progress animation speed. Use the property 'ValueByTransition' via codeto animate the progress value's movement.")]
		[Category("x Properties")]
		private int animationSpeed = 220;

        public int AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }


		[Description("Sets the progress value. Alternatively, you can use the property 'ValueByTransition' via code to animate the progress value's movement.")]
		[Category("x Properties")]
		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				if (value >= MinimumValue && value <= MaximumValue)
				{
					_value = value;
					Refresh();
					if (this.ProgressChanged != null)
						this.ProgressChanged(this, new ProgressChangedEventArgs(_value, _maximum));
					if (this.onValueChange != null)
						this.onValueChange(this, new EventArgs());
					return;
				}
				throw new ArgumentOutOfRangeException("The progress value is outside the expected range, that is, between the Minimum and Maximum values.");
			}
		}

		[Category("x Properties")]
		[Description("Sets the maximum progress value.")]
		public int Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				if (value > Minimum)
				{
					_maximum = value;
					Refresh();
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets the minimum progress value.")]
		public int Minimum
		{
			get
			{
				return _minimum;
			}
			set
			{
				if (value < Maximum)
				{
					_minimum = value;
					Refresh();
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets the border radius.")]
		public int BorderRadius
		{
			get
			{
				return _borderRadius;
			}
			set
			{
				if (value > 0)
				{
					_borderRadius = value;
					Refresh();
				}
			}
		}

		[Description("Sets the border thickness.")]
		[Category("x Properties")]
		public int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				if (value > 0)
				{
					_borderThickness = value;
					Refresh();
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets the border color.")]
		public Color BorderColor
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
		[Description("Sets the progress background color.")]
		public new Color BackColor
		{
			get
			{
				return _progressBackColor;
			}
			set
			{
				_progressBackColor = value;
				Refresh();
			}
		}

		[Description("Sets the progress color to the left.")]
		[Category("x Properties")]
		public Color ProgressColorLeft
		{
			get
			{
				return _progressColorLeft;
			}
			set
			{
				_progressColorLeft = value;
				Refresh();
			}
		}

		[Description("Sets an emerging progress color to the right.")]
		[Category("x Properties")]
		public Color ProgressColorRight
		{
			get
			{
				return _progressColorRight;
			}
			set
			{
				_progressColorRight = value;
				Refresh();
			}
		}

		[Description("Sets the progress bar orientation.")]
		[Category("x Properties")]
		public virtual Orientation Orientation
		{
			get
			{
				return _orientation;
			}
			set
			{
				_orientation = value;
				int num = base.Width;
				int num2 = base.Height;
				switch (value)
				{
				case Orientation.Horizontal:
					base.Height = num;
					base.Width = num2;
					break;
				case Orientation.Vertical:
					base.Width = num2;
					base.Height = num;
					break;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		public int ValueByTransition
		{
			get
			{
				return _value;
			}
			set
			{
				if (!base.DesignMode)
					TransitionValue(value);
			}
		}

		[Browsable(false)]
		public int MaximumValue
		{
			get
			{
				return _maximum;
			}
			set
			{
				if (value > MinimumValue)
				{
					_maximum = value;
					Refresh();
				}
			}
		}

		[Browsable(false)]
		public int MinimumValue
		{
			get
			{
				return _minimum;
			}
			set
			{
				if (value < MaximumValue)
				{
					_minimum = value;
					Refresh();
				}
			}
		}

		[Browsable(false)]
        private int animation = 0;

        public int Animation
        {
            get { return animation; }
            set { animation = value; }
        }


		[Browsable(false)]
		public int AnimationStep
		{
			get
			{
				return _animationStep;
			}
			set
			{
				if (value > 0)
					_animationStep = value;
			}
		}

		[Browsable(false)]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage = value;
			}
		}

		[Browsable(false)]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout = value;
			}
		}

		[Browsable(false)]
		public Color ProgressBackColor
		{
			get
			{
				return _progressBackColor;
			}
			set
			{
				_progressBackColor = value;
				Refresh();
			}
		}

		[Category("x Events")]
		[Description("Occurs when the progress value changes.")]
		public event EventHandler<ProgressChangedEventArgs> ProgressChanged = null;

		[Browsable(false)]
		public event EventHandler onValueChange = null;

		public xProgressBar()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.BackColor = Color.Transparent;
			if (Orientation == Orientation.Horizontal)
				base.Size = new Size(285, 13);
			if (Orientation == Orientation.Vertical)
				base.Size = new Size(13, 285);
		}

		public new void Refresh()
		{
			int borderRadius = _borderRadius;
			if (_orientation == Orientation.Horizontal)
			{
				if (base.Height < borderRadius)
					base.Height = borderRadius;
			}
			else if (_orientation == Orientation.Vertical && base.Width < borderRadius)
			{
				base.Width = borderRadius;
			}
			RenderCanvas();
			RenderProgressByValue(Value);
		}

		public void TransitionValue(int value)
		{
			Transition.run(this, "Value", value, new TransitionType_EaseInEaseOut(AnimationSpeed));
		}

		public void TransitionValue(int value, int transitionSpeed = 200)
		{
			Transition.run(this, "Value", value, new TransitionType_EaseInEaseOut(transitionSpeed));
		}

		public static Image RoundCorners(Image image, int borderRadius, Color backColor)
		{
			Bitmap bitmap = new Bitmap(image.Width, image.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.Clear(backColor);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				Brush brush = new TextureBrush(image);
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddArc(0, 0, borderRadius, borderRadius, 180f, 90f);
				graphicsPath.AddArc(bitmap.Width - borderRadius, 0, borderRadius, borderRadius, 270f, 90f);
				graphicsPath.AddArc(bitmap.Width - borderRadius, bitmap.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
				graphicsPath.AddArc(0, bitmap.Height - borderRadius, borderRadius, borderRadius, 90f, 90f);
				graphics.FillPath(brush, graphicsPath);
				graphics.Dispose();
				graphicsPath.Dispose();
				brush.Dispose();
				return bitmap;
			}
		}

		public static Image RoundCorners(Size size, Image image, int borderRadius, Color backColor)
		{
			try
			{
				Bitmap bitmap = new Bitmap(size.Width, size.Height);
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(backColor);
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					Brush brush = new TextureBrush(image);
					GraphicsPath graphicsPath = new GraphicsPath();
					graphicsPath.AddArc(0, 0, borderRadius, borderRadius, 180f, 90f);
					graphicsPath.AddArc(bitmap.Width - borderRadius, 0, borderRadius, borderRadius, 270f, 90f);
					graphicsPath.AddArc(bitmap.Width - borderRadius, bitmap.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
					graphicsPath.AddArc(0, bitmap.Height - borderRadius, borderRadius, borderRadius, 90f, 90f);
					graphics.FillPath(brush, graphicsPath);
					graphics.Dispose();
					graphicsPath.Dispose();
					brush.Dispose();
					return bitmap;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		private void DrawRoundedRectangle(Graphics graphics, Rectangle bounds, int borderRadius, Pen pen, Color backColor, Size size, bool isProgress)
		{
			graphics.Clear(Color.Transparent);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			GraphicsPath graphicsPath = new GraphicsPath();
			pen.StartCap = LineCap.Round;
			pen.EndCap = LineCap.Round;
			if (_orientation == Orientation.Horizontal)
			{
				if (borderRadius > 1)
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, borderRadius - 2, borderRadius - 2, 180f, 90f);
					if (isProgress)
					{
						if (size.Width + _borderThickness - 2 < _borderRadius)
						{
							graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 0f, 0f);
							graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 0f);
						}
						else
						{
							graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
							graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
						}
					}
					else
					{
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
					}
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 90f, 90f);
				}
				else
					graphicsPath.AddRectangle(bounds);
			}
			else if (_orientation == Orientation.Vertical)
			{
				if (borderRadius > 1)
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, borderRadius - 2, borderRadius - 2, 180f, 90f);
					if (!isProgress)
					{
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 0f, 90f);
					}
					else if (size.Width + _borderThickness - 2 < _borderRadius)
					{
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y, borderRadius, borderRadius, 0f, 0f);
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 0f, 0f);
					}
					else
					{
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 0f, 90f);
					}
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 90f, 90f);
				}
				else
					graphicsPath.AddRectangle(bounds);
			}
			graphicsPath.CloseAllFigures();
			graphics.FillPath(new SolidBrush(backColor), graphicsPath);
			graphics.DrawPath(pen, graphicsPath);
			pen.Dispose();
			graphics.Dispose();
			graphicsPath.Dispose();
		}

		private void DrawRoundedGradientRectangle(Graphics graphics, Rectangle bounds, int borderRadius, Pen pen, Color backColor, Size size)
		{
			graphics.Clear(Color.Transparent);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			GraphicsPath graphicsPath = new GraphicsPath();
			pen.StartCap = LineCap.Round;
			pen.EndCap = LineCap.Round;
			if (_orientation == Orientation.Horizontal)
			{
				if (borderRadius > 1)
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, borderRadius, borderRadius, 180f, 90f);
					if (size.Width + _borderThickness - 2 < _borderRadius)
					{
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 0f, 0f);
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 0f);
					}
					else
					{
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
						graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
					}
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 90f, 90f);
				}
				else
					graphicsPath.AddRectangle(bounds);
			}
			else if (_orientation == Orientation.Vertical)
			{
				if (borderRadius > 1)
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, borderRadius, borderRadius, 180f, 90f);
					if (size.Width + _borderThickness - 2 < _borderRadius)
					{
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y, borderRadius, borderRadius, 0f, 0f);
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 0f, 0f);
					}
					else
					{
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
						graphicsPath.AddArc(bounds.X + bounds.Height - borderRadius, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 0f, 90f);
					}
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Width - borderRadius, borderRadius, borderRadius, 90f, 90f);
				}
				else
					graphicsPath.AddRectangle(bounds);
			}
			graphicsPath.CloseAllFigures();
			LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, _progressColorLeft, _progressColorRight, 1f, false);
			Pen pen2 = new Pen(linearGradientBrush);
			pen2.StartCap = LineCap.Round;
			pen2.EndCap = LineCap.Round;
			pen2.LineJoin = LineJoin.MiterClipped;
			graphics.FillPath(linearGradientBrush, graphicsPath);
			graphics.DrawPath(pen2, graphicsPath);
			pen.Dispose();
			linearGradientBrush.Dispose();
			graphics.Dispose();
			pen2.Dispose();
			graphicsPath.Dispose();
		}

		private Image DrawRoundedGradientImage(Image image, Rectangle bounds, int borderRadius, Pen pen, Color backColor)
		{
			Bitmap bitmap = new Bitmap(image.Width, image.Height);
			using (Graphics graphics = Graphics.FromImage(bitmap))
			{
				graphics.Clear(Color.Transparent);
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				GraphicsPath graphicsPath = new GraphicsPath();
				pen.StartCap = LineCap.Round;
				pen.EndCap = LineCap.Round;
				if (_borderRadius > 1)
				{
					graphicsPath.AddArc(bounds.X, bounds.Y, borderRadius, borderRadius, 180f, 90f);
					graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y, borderRadius, borderRadius, 270f, 90f);
					graphicsPath.AddArc(bounds.X + bounds.Width - borderRadius, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 0f, 90f);
					graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Height - borderRadius, borderRadius, borderRadius, 90f, 90f);
				}
				else
					graphicsPath.AddRectangle(bounds);
				graphicsPath.CloseAllFigures();
				LinearGradientBrush linearGradientBrush = new LinearGradientBrush(bounds, _progressColorLeft, _progressColorRight, 1f, false);
				Pen pen2 = new Pen(linearGradientBrush);
				pen2.StartCap = LineCap.Round;
				pen2.EndCap = LineCap.Round;
				pen2.LineJoin = LineJoin.MiterClipped;
				graphics.FillPath(linearGradientBrush, graphicsPath);
				graphics.DrawPath(pen2, graphicsPath);
				pen.Dispose();
				linearGradientBrush.Dispose();
				graphics.Dispose();
				pen2.Dispose();
				graphicsPath.Dispose();
				return bitmap;
			}
		}

		private void RenderCanvas()
		{
			try
			{
				if (_orientation == Orientation.Horizontal)
				{
					Bitmap bitmap = new Bitmap(base.Size.Width, base.Size.Height);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.Clear(Color.Transparent);
					Rectangle bounds = new Rectangle(BorderThickness, BorderThickness, base.Width - BorderThickness * 2 - 3, base.Height - BorderThickness * 2);
					DrawRoundedRectangle(graphics, bounds, BorderRadius, new Pen(BorderColor, BorderThickness), ProgressBackColor, bitmap.Size, false);
					BackgroundImage = bitmap;
					graphics.Dispose();
				}
				else if (_orientation == Orientation.Vertical)
				{
					Bitmap bitmap2 = new Bitmap(base.Size.Width, base.Size.Height);
					Graphics graphics2 = Graphics.FromImage(bitmap2);
					graphics2.SmoothingMode = SmoothingMode.HighQuality;
					graphics2.Clear(Color.Transparent);
					Rectangle bounds2 = new Rectangle(BorderThickness, BorderThickness, base.Height - BorderThickness * 2 - 3, base.Width - BorderThickness * 2);
					DrawRoundedRectangle(graphics2, bounds2, BorderRadius, new Pen(BorderColor, BorderThickness), ProgressBackColor, bitmap2.Size, false);
					BackgroundImage = bitmap2;
					graphics2.Dispose();
				}
			}
			catch (Exception)
			{
			}
		}

		private void RenderProgressByValue(int value)
		{
			int borderRadius = _borderRadius;
			try
			{
				if (_orientation == Orientation.Horizontal)
				{
					Bitmap bitmap = new Bitmap((value == 0) ? 1 : ValueToPixel(value), base.Size.Height);
					Graphics graphics = Graphics.FromImage(bitmap);
					graphics.SmoothingMode = SmoothingMode.HighQuality;
					graphics.Clear(Color.Transparent);
					pnlProgress.Dock = DockStyle.None;
					pnlProgress.Top = _borderThickness + 1;
					pnlProgress.Height = base.Height - _borderThickness * 2 - 2;
					pnlProgress.Left = _borderThickness + 1;
					pnlProgress.Width = PixelToValue(value) - _borderThickness * 2 - 2;
					if (ProgressColorLeft != ProgressColorRight)
					{
						Rectangle bounds = new Rectangle(BorderThickness, BorderThickness, pnlProgress.Width - _borderThickness * 2 - ((_borderThickness == 1) ? 1 : (-_borderThickness)), pnlProgress.Height - _borderThickness * 2);
						DrawRoundedGradientRectangle(graphics, bounds, borderRadius - 2, new Pen(ProgressColorLeft, 1f), ProgressColorLeft, bitmap.Size);
						pnlProgress.Image = bitmap;
						pnlProgress.BackColor = Color.Transparent;
					}
					else
					{
						Rectangle bounds2 = new Rectangle(BorderThickness, BorderThickness, pnlProgress.Width - _borderThickness * 2 - ((_borderThickness == 1) ? 1 : (-_borderThickness)), pnlProgress.Height - _borderThickness * 2);
						DrawRoundedRectangle(graphics, bounds2, borderRadius - 2, new Pen(ProgressColorLeft, 1f), ProgressColorLeft, bitmap.Size, true);
						pnlProgress.Image = bitmap;
						pnlProgress.BackColor = Color.Transparent;
					}
					graphics.Dispose();
				}
				else if (_orientation == Orientation.Vertical)
				{
					Bitmap bitmap2 = new Bitmap(base.Size.Height, (value == 0) ? 1 : ValueToPixel(value));
					Graphics graphics2 = Graphics.FromImage(bitmap2);
					graphics2.SmoothingMode = SmoothingMode.HighQuality;
					graphics2.Clear(Color.Transparent);
					pnlProgress.Dock = DockStyle.None;
					pnlProgress.Left = _borderThickness + 1;
					pnlProgress.Width = base.Width - _borderThickness * 2 - 2;
					pnlProgress.Top = _borderThickness + 1;
					pnlProgress.Height = PixelToValue(value) - _borderThickness * 2 - 2;
					pnlProgress.Location = new Point(_borderThickness + 1, base.Height - pnlProgress.Height - (_borderThickness * 2 + 2));
					if (ProgressColorLeft != ProgressColorRight)
					{
						Rectangle bounds3 = new Rectangle(BorderThickness, BorderThickness, pnlProgress.Height - _borderThickness * 2 - ((_borderThickness == 1) ? 1 : (-_borderThickness)), pnlProgress.Width - _borderThickness * 2);
						DrawRoundedGradientRectangle(graphics2, bounds3, borderRadius - 2, new Pen(ProgressColorLeft, 1f), ProgressColorLeft, bitmap2.Size);
						pnlProgress.Image = bitmap2;
						pnlProgress.BackColor = Color.Transparent;
					}
					else
					{
						Rectangle bounds4 = new Rectangle(BorderThickness, BorderThickness, pnlProgress.Height - _borderThickness * 2 - ((_borderThickness == 1) ? 1 : (-_borderThickness)), pnlProgress.Width - _borderThickness * 2);
						DrawRoundedRectangle(graphics2, bounds4, borderRadius - 2, new Pen(ProgressColorLeft, 1f), ProgressColorLeft, bitmap2.Size, true);
						pnlProgress.Image = bitmap2;
						pnlProgress.BackColor = Color.Transparent;
					}
					graphics2.Dispose();
				}
			}
			catch (Exception)
			{
			}
		}

		private int ValueToPixel(int value)
		{
			double cONST = CONST;
			cONST = 1.0;
			if (_orientation == Orientation.Horizontal)
				return (int)Math.Round((double)value * ((double)base.Width - (double)BorderThickness * cONST * 2.0) / (double)(MaximumValue - MinimumValue), 0);
			return (int)Math.Round((double)value * ((double)base.Height - (double)BorderThickness * cONST * 2.0) / (double)(MaximumValue - MinimumValue), 0);
		}

		private int PixelToValue(int pixels)
		{
			double cONST = CONST;
			cONST = 1.0;
			if (_orientation == Orientation.Horizontal)
				return (int)Math.Round((double)pixels * ((double)base.Width - (double)BorderThickness * cONST * 2.0) / (double)(MaximumValue - MinimumValue), 0);
			return (int)Math.Round((double)pixels * ((double)base.Height - (double)BorderThickness * cONST * 2.0) / (double)(MaximumValue - MinimumValue), 0);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			Refresh();
			_currentProgressWidth = ValueToPixel(Value);
		}

		private void OnResize(object sender, EventArgs e)
		{
			Refresh();
			_currentProgressWidth = ValueToPixel(Value);
		}

		private void OnProgressPanelClick(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.pnlProgress = new System.Windows.Forms.PictureBox();
			this.pbBuffer = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)this.pnlProgress).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pbBuffer).BeginInit();
			base.SuspendLayout();
			this.pnlProgress.BackColor = System.Drawing.Color.Transparent;
			this.pnlProgress.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlProgress.Enabled = false;
			this.pnlProgress.Location = new System.Drawing.Point(0, 0);
			this.pnlProgress.Name = "pnlProgress";
			this.pnlProgress.Size = new System.Drawing.Size(284, 10);
			this.pnlProgress.TabIndex = 0;
			this.pnlProgress.TabStop = false;
			this.pnlProgress.Click += new System.EventHandler(OnProgressPanelClick);
			this.pbBuffer.Location = new System.Drawing.Point(0, 0);
			this.pbBuffer.Name = "pbBuffer";
			this.pbBuffer.Size = new System.Drawing.Size(0, 10);
			this.pbBuffer.TabIndex = 1;
			this.pbBuffer.TabStop = false;
			this.pbBuffer.Visible = false;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			base.Controls.Add(this.pbBuffer);
			base.Controls.Add(this.pnlProgress);
			this.DoubleBuffered = true;
			base.Name = "xProgressBar";
			base.Size = new System.Drawing.Size(284, 10);
			base.Load += new System.EventHandler(OnLoad);
			base.Resize += new System.EventHandler(OnResize);
			((System.ComponentModel.ISupportInitialize)this.pnlProgress).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pbBuffer).EndInit();
			base.ResumeLayout(false);
		}
	}
}
