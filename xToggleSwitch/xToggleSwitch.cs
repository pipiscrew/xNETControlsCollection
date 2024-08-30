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
	[Description("Add customizable switches that toggle between the visual states of settings and features.")]
	[Category("x UI For Windows Forms")]
	[DebuggerStepThrough]
	[DefaultEvent("CheckedChanged")]
	[ToolboxBitmap(typeof(CheckBox))]
	[Designer(typeof(xDesigner))]
	[DefaultProperty("Checked")]
	public class xToggleSwitch : UserControl
	{
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class ToggleState : ExpandableObjectConverter
		{
			private int _BorderThicknessInner = 1;

			private int _BorderRadiusInner = 1;

			private int _BorderThickness = 1;

			private int _BorderRadius = 1;

			private Color _BorderColor = Color.FromArgb(236, 0, 140);

			private Color _BorderColorInner;

			private Color _BackColorInner;

			private Color _BackColor;

			public int BorderThicknessInner
			{
				get
				{
					return _BorderThicknessInner;
				}
				set
				{
					_BorderThicknessInner = value;
				}
			}

			public int BorderRadiusInner
			{
				get
				{
					return _BorderRadiusInner;
				}
				set
				{
					_BorderRadiusInner = value;
				}
			}

			public int BorderThickness
			{
				get
				{
					return _BorderThickness;
				}
				set
				{
					_BorderThickness = value;
				}
			}

			public int BorderRadius
			{
				get
				{
					return _BorderRadius;
				}
				set
				{
					_BorderRadius = value;
				}
			}

			public Color BorderColor
			{
				get
				{
					return _BorderColor;
				}
				set
				{
					_BorderColor = value;
				}
			}

			public Color BorderColorInner
			{
				get
				{
					return _BorderColorInner;
				}
				set
				{
					_BorderColorInner = value;
				}
			}

			public Color BackColor
			{
				get
				{
					return _BackColor;
				}
				set
				{
					_BackColor = value;
				}
			}

			public Color BackColorInner
			{
				get
				{
					return _BackColorInner;
				}
				set
				{
					_BackColorInner = value;
				}
			}

			public override string ToString()
			{
				return string.Format("BackColor: {0}, BorderColor: {1}, InnerBackColor: {2},", BackColor, BorderColor, BackColorInner) + string.Format("InnerBorderColor: {0}, BorderRadius: {1}, InnerBorderRadius: ", BorderColorInner, BorderRadius) + string.Format("{0}, BorderThickness: {1}, InnerBorderRadius: {2}", BorderRadiusInner, BorderThickness, BorderThicknessInner);
			}
		}

		public class CheckedChangedEventArgs : EventArgs
		{
			private bool _checked;

			public bool Checked
			{
				get
				{
					return _checked;
				}
			}

			public CheckedChangedEventArgs(bool isChecked)
			{
				_checked = isChecked;
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
					xToggleSwitch xToggleSwitch = (xToggleSwitch)base.Control;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerStepThrough]
		public class xControlActionList : DesignerActionList
		{
			private xToggleSwitch xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public bool Checked
			{
				get
				{
					return xControl.Checked;
				}
				set
				{
					SetValue(xControl, "Checked", value);
				}
			}

			public int ThumbMargin
			{
				get
				{
					return xControl.ThumbMargin;
				}
				set
				{
					SetValue(xControl, "ThumbMargin", value);
				}
			}

			public xControlActionList(IComponent component)
				: base(component)
			{
				xControl = component as xToggleSwitch;
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
					new DesignerActionPropertyItem("Checked", "Checked", "Common Tasks", GetPropertyDescriptor(base.Component, "Checked").Description),
					new DesignerActionPropertyItem("ThumbMargin", "ThumbMargin:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbMargin").Description),
					new DesignerActionMethodItem(this, "Redraw", "Refresh Control", "Appearance", "Redraws the control based on the properties applied.")
				};
			}

			public void Redraw()
			{
				xControl.Refresh();
			}
		}

		private bool _value = true;

		private int _animation = 5;

		private int _Padding = 3;

		private ToggleState _currentState = null;

		private ToggleState _toggleStateOn = new ToggleState();

		private ToggleState _toggleStateOff = new ToggleState();

		private ToggleState _toggleStateDisabled = new ToggleState();

		private IContainer components = null;

		private PictureBox circle;

		private Timer tmrAnimate;

		[Description("Sets a value indicating whether the Toggle Switch has been checked.")]
		[Category("x Properties")]
		public bool Checked
		{
			get
			{
				return Value;
			}
			set
			{
				Value = value;
			}
		}

		[Description("Sets the animation speed.")]
		[Category("x Properties")]
		public int Animation
		{
			get
			{
				return _animation;
			}
			set
			{
				if (value > 1)
					_animation = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the thumb's (inner circle) padding.")]
		public int ThumbMargin
		{
			get
			{
				return Padding;
			}
			set
			{
				Padding = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the visual properties of the 'ON' toggle state.")]
		public ToggleState ToggleStateOn
		{
			get
			{
				Refresh();
				return _toggleStateOn;
			}
			set
			{
				_toggleStateOn = value;
			}
		}

		[Description("Sets the visual properties of the 'OFF' toggle state.")]
		[Category("x Properties")]
		public ToggleState ToggleStateOff
		{
			get
			{
				Refresh();
				return _toggleStateOff;
			}
			set
			{
				_toggleStateOff = value;
			}
		}

		[Description("Sets the visual properties of the disabled toggle state.")]
		[Category("x Properties")]
		public ToggleState ToggleStateDisabled
		{
			get
			{
				Refresh();
				return _toggleStateDisabled;
			}
			set
			{
				_toggleStateDisabled = value;
			}
		}

		[Browsable(false)]
		public bool Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				EventHandler onValuechange = this.OnValuechange;
				if (onValuechange != null)
					onValuechange(this, new EventArgs());
				EventHandler<CheckedChangedEventArgs> checkedChanged = this.CheckedChanged;
				if (checkedChanged != null)
					checkedChanged(this, new CheckedChangedEventArgs(value));
				Refresh();
				tmrAnimate.Interval = 1;
				tmrAnimate.Start();
			}
		}

		[Browsable(false)]
		public int InnerCirclePadding
		{
			get
			{
				return Padding;
			}
			set
			{
				Padding = value;
			}
		}

		[Browsable(false)]
		public new int Padding
		{
			get
			{
				return _Padding;
			}
			set
			{
				_Padding = value;
				circle.Top = _Padding;
				circle.Height = base.Height - _Padding * 2;
				circle.Width = circle.Height;
				if (Value)
					circle.Left = base.Width - circle.Width - _Padding;
				else
					circle.Left = _Padding;
				Refresh();
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

		[Description("Occurs when the switch value has been changed.")]
		[Category("x Events")]
		public event EventHandler<CheckedChangedEventArgs> CheckedChanged = null;

		[Browsable(false)]
		public event EventHandler OnValuechange = null;

		public xToggleSwitch()
		{
			InitializeComponent();
			circle.Top = _Padding;
			circle.Height = base.Height - _Padding * 2;
			circle.Width = circle.Height;
			_toggleStateDisabled.BackColor = Color.DarkGray;
			_toggleStateDisabled.BackColorInner = Color.White;
			_toggleStateDisabled.BorderColor = Color.DarkGray;
			_toggleStateDisabled.BorderColorInner = Color.White;
			_toggleStateDisabled.BorderRadius = 17;
			_toggleStateDisabled.BorderRadiusInner = 11;
			_toggleStateDisabled.BorderThickness = 1;
			_toggleStateDisabled.BorderThicknessInner = 1;
			if (_value)
				circle.Left = base.Width - circle.Width - _Padding;
			else
				circle.Left = _Padding;
			base.Size = new Size(32, 18);
		}

		public void ResizeControl()
		{
			circle.Top = _Padding;
			circle.Height = base.Height - _Padding * 2;
			circle.Width = circle.Height;
			if (Value)
				circle.Left = base.Width - circle.Width - _Padding;
			else
				circle.Left = _Padding;
		}

		public new void Refresh()
		{
			if (base.Enabled)
			{
				if (_value)
				{
					_currentState = _toggleStateOn;
					if (_currentState.BackColor == Color.Empty)
					{
						_currentState.BackColor = Color.DodgerBlue;
						_currentState.BackColorInner = Color.White;
						_currentState.BorderColor = Color.DodgerBlue;
						_currentState.BorderColorInner = Color.White;
						_currentState.BorderRadius = 17;
						_currentState.BorderRadiusInner = 11;
						_currentState.BorderThickness = 1;
						_currentState.BorderThicknessInner = 1;
					}
				}
				else
				{
					_currentState = _toggleStateOff;
					if (_currentState.BackColor == Color.Empty)
					{
						_currentState.BackColor = Color.FromArgb(191, 191, 191);
						_currentState.BackColorInner = Color.White;
						_currentState.BorderColor = Color.FromArgb(191, 191, 191);
						_currentState.BorderColorInner = Color.White;
						_currentState.BorderRadius = 17;
						_currentState.BorderRadiusInner = 11;
						_currentState.BorderThickness = 1;
						_currentState.BorderThicknessInner = 1;
					}
				}
			}
			else
			{
				_currentState = _toggleStateDisabled;
				if (_currentState.BackColor == Color.Empty)
				{
					_currentState.BackColor = Color.DarkGray;
					_currentState.BackColorInner = Color.White;
					_currentState.BorderColor = Color.DarkGray;
					_currentState.BorderColorInner = Color.White;
					_currentState.BorderRadius = 17;
					_currentState.BorderRadiusInner = 11;
					_currentState.BorderThickness = 1;
					_currentState.BorderThicknessInner = 1;
				}
			}
			DrawBackground(_currentState);
			DrawCircle(_currentState);
		}

		private void DrawCircle(ToggleState State)
		{
			Bitmap image = new Bitmap(circle.Width, circle.Height);
			Rectangle bounds = new Rectangle(0, 0, circle.Width - 1, circle.Height - 1);
			Pen pen = new Pen(State.BorderColorInner, State.BorderThicknessInner);
			DrawRoundedRectangle(Graphics.FromImage(image), bounds, State.BorderRadiusInner, pen, State.BackColorInner);
			pen.Dispose();
			circle.Image = image;
		}

		private void DrawBackground(ToggleState State)
		{
			Bitmap bitmap = new Bitmap(base.Width, base.Height);
			Rectangle bounds = new Rectangle(0, 0, base.Width - 1, base.Height - 1);
			Pen pen = new Pen(State.BorderColor, State.BorderThickness);
			DrawRoundedRectangle(Graphics.FromImage(bitmap), bounds, State.BorderRadius, pen, State.BackColor);
			pen.Dispose();
			BackgroundImage = bitmap;
		}

		private void DrawRoundedRectangle(Graphics graphics, Rectangle bounds, int radius, Pen pen, Color backColor)
		{
			graphics.Clear(Color.Transparent);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			GraphicsPath graphicsPath = new GraphicsPath();
			pen.StartCap = LineCap.Round;
			pen.EndCap = LineCap.Round;
			graphicsPath.AddArc(bounds.X, bounds.Y, radius, radius, 180f, 90f);
			graphicsPath.AddArc(bounds.X + bounds.Width - radius, bounds.Y, radius, radius, 270f, 90f);
			graphicsPath.AddArc(bounds.X + bounds.Width - radius, bounds.Y + bounds.Height - radius, radius, radius, 0f, 90f);
			graphicsPath.AddArc(bounds.X, bounds.Y + bounds.Height - radius, radius, radius, 90f, 90f);
			graphicsPath.CloseAllFigures();
			graphics.FillPath(new SolidBrush(backColor), graphicsPath);
			graphics.DrawPath(pen, graphicsPath);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			Refresh();
		}

		private void OnResize(object sender, EventArgs e)
		{
			ResizeControl();
			Refresh();
		}

		private void OnClick(object sender, EventArgs e)
		{
			Value = !Value;
		}

		private void OnTickAnimation(object sender, EventArgs e)
		{
			if (Value)
			{
				if (circle.Left >= base.Width - circle.Width - _Padding)
				{
					tmrAnimate.Stop();
					circle.Left = base.Width - circle.Width - _Padding;
				}
				else
				{
					circle.Left += Animation;
					tmrAnimate.Interval *= 2;
				}
			}
			else if (circle.Left <= _Padding)
			{
				tmrAnimate.Stop();
				circle.Left = _Padding;
			}
			else
			{
				circle.Left -= Animation;
				tmrAnimate.Interval *= 2;
			}
		}

		protected override void OnParentFontChanged(EventArgs e)
		{
			base.OnParentFontChanged(e);
			ResizeControl();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			ResizeControl();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.circle = new System.Windows.Forms.PictureBox();
			this.tmrAnimate = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)this.circle).BeginInit();
			base.SuspendLayout();
			this.circle.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.circle.BackColor = System.Drawing.Color.Transparent;
			this.circle.Enabled = false;
			this.circle.Location = new System.Drawing.Point(0, 1);
			this.circle.Name = "circle";
			this.circle.Size = new System.Drawing.Size(15, 16);
			this.circle.TabIndex = 0;
			this.circle.TabStop = false;
			this.tmrAnimate.Interval = 1;
			this.tmrAnimate.Tick += new System.EventHandler(OnTickAnimation);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			base.Controls.Add(this.circle);
			this.Cursor = System.Windows.Forms.Cursors.Hand;
			base.Name = "xToggleSwitch";
			base.Size = new System.Drawing.Size(35, 18);
			base.Load += new System.EventHandler(OnLoad);
			base.Click += new System.EventHandler(OnClick);
			base.Resize += new System.EventHandler(OnResize);
			((System.ComponentModel.ISupportInitialize)this.circle).EndInit();
			base.ResumeLayout(false);
		}
	}
}
