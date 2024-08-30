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
	[Description("Add stylish radio buttons with intuitive customization options and design features.")]
	[ToolboxBitmap(typeof(RadioButton))]
	[Designer(typeof(xDesigner))]
	[DefaultProperty("Checked")]
	[DebuggerStepThrough]
	[Category("x UI For Windows Forms")]
	[DefaultEvent("CheckedChanged2")]
	public class xRadioButton : Control
	{
		public enum BindingControlPositions
		{
			Left,
			Right
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

		public class BindingControlChangedEventArgs : EventArgs
		{
			private Control _boundControl;

			public Control Control
			{
				get
				{
					return _boundControl;
				}
			}

			public BindingControlChangedEventArgs(Control currentlyBoundControl)
			{
				_boundControl = currentlyBoundControl;
			}
		}

		public class PositionChangedEventArgs : EventArgs
		{
			private BindingControlPositions _controlPosition;

			public BindingControlPositions BindingControlPosition
			{
				get
				{
					return _controlPosition;
				}
			}

			public PositionChangedEventArgs(BindingControlPositions currentControlPosition)
			{
				_controlPosition = currentControlPosition;
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
					xRadioButton RadioButton = (xRadioButton)base.Control;
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
							new xRadioButtonActionList(base.Component)
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
		public class xRadioButtonActionList : DesignerActionList
		{
			private xRadioButton xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public Control BindingControl
			{
				get
				{
					return xControl.BindingControl;
				}
				set
				{
					SetValue(xControl, "BindingControl", value);
				}
			}

			public bool AllowBindingControlLocation
			{
				get
				{
					return xControl.AllowBindingControlLocation;
				}
				set
				{
					SetValue(xControl, "AllowBindingControlLocation", value);
				}
			}

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

			public int BorderThickness
			{
				get
				{
					return xControl.BorderThickness;
				}
				set
				{
					SetValue(xControl, "BorderThickness", value);
				}
			}

			public Color OutlineColor
			{
				get
				{
					return xControl.OutlineColor;
				}
				set
				{
					SetValue(xControl, "OutlineColor", value);
				}
			}

			public Color OutlineColorUnchecked
			{
				get
				{
					return xControl.OutlineColorUnchecked;
				}
				set
				{
					SetValue(xControl, "OutlineColorUnchecked", value);
				}
			}

			public Color RadioColor
			{
				get
				{
					return xControl.RadioColor;
				}
				set
				{
					SetValue(xControl, "RadioColor", value);
				}
			}

			public BindingControlPositions BindingControlPosition
			{
				get
				{
					return xControl.BindingControlPosition;
				}
				set
				{
					SetValue(xControl, "BindingControlPosition", value);
				}
			}

			public xRadioButtonActionList(IComponent component)
				: base(component)
			{
				xControl = component as xRadioButton;
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
					new DesignerActionPropertyItem("BindingControl", "BindingControl:", "Common Tasks", GetPropertyDescriptor(base.Component, "BindingControl").Description),
					new DesignerActionPropertyItem("AllowBindingControlLocation", "AllowBindingControlLocation", "Common Tasks", GetPropertyDescriptor(base.Component, "AllowBindingControlLocation").Description),
					new DesignerActionPropertyItem("Checked", "Checked", "Common Tasks", GetPropertyDescriptor(base.Component, "Checked").Description),
					new DesignerActionPropertyItem("BorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "BorderThickness").Description),
					new DesignerActionPropertyItem("OutlineColor", "OutlineColor:", "Appearance", GetPropertyDescriptor(base.Component, "OutlineColor").Description),
					new DesignerActionPropertyItem("OutlineColorUnchecked", "OutlineColorUnchecked:", "Appearance", GetPropertyDescriptor(base.Component, "OutlineColorUnchecked").Description),
					new DesignerActionPropertyItem("RadioColor", "RadioColor:", "Appearance", GetPropertyDescriptor(base.Component, "RadioColor").Description)
				};
			}
		}

		private string _text;

		private string _lastBoundControl;

		private bool _clicked = true;

		private bool _checked = false;

		private bool _allowBindingControlLocation;

		private int _borderThickness = 1;

		private Point _bindingControlPoints;

		private Color _radioColor = Color.DodgerBlue;

		private Color _outlineColorChecked = Color.DodgerBlue;

		private Color _outlineColorUnchecked = Color.DarkGray;

		private Color _radioColorTabFocused = Color.FromArgb(40, 96, 144);

		private Color _outlineColorTabFocused = Color.FromArgb(40, 96, 144);

		private Control _parentControl = null;

		private Control _bindingControl = new Control();

		private BindingControlPositions _controlPosition = BindingControlPositions.Right;

		private IContainer components = null;

		[Description("Sets a value indicating whether the radio is checked.")]
		[Category("x Properties")]
		public virtual bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				_checked = value;
				if (value)
					ResetOthers();
				if (this.CheckedChanged != null)
					this.CheckedChanged(this, EventArgs.Empty);
				if (this.CheckedChanged2 != null)
					this.CheckedChanged2(this, new CheckedChangedEventArgs(_checked));
				Refresh();
			}
		}

		[Description("Indicates whether the control will allow the bound control's location to be positioned based on it's own location.")]
		[Category("x Properties")]
		public virtual bool AllowBindingControlLocation
		{
			get
			{
				return _allowBindingControlLocation;
			}
			set
			{
				_allowBindingControlLocation = value;
				if (BindingControl != null)
					PositionBoundControl(true, _bindingControl);
			}
		}

		[Description("Sets the radio color.")]
		[Category("x Properties")]
		public virtual Color RadioColor
		{
			get
			{
				return _radioColor;
			}
			set
			{
				_radioColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the radio color on focus.")]
		public virtual Color RadioColorTabFocused
		{
			get
			{
				return _radioColorTabFocused;
			}
			set
			{
				_radioColorTabFocused = value;
				Refresh();
			}
		}

		[Description("Sets the radio's outline color when unchecked.")]
		[Category("x Properties")]
		public virtual Color OutlineColorUnchecked
		{
			get
			{
				return _outlineColorUnchecked;
			}
			set
			{
				_outlineColorUnchecked = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the radio's color when focused.")]
		public virtual Color OutlineColorTabFocused
		{
			get
			{
				return _outlineColorTabFocused;
			}
			set
			{
				_outlineColorTabFocused = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the outline color.")]
		public virtual Color OutlineColor
		{
			get
			{
				return _outlineColorChecked;
			}
			set
			{
				_outlineColorChecked = value;
				Refresh();
			}
		}

		[Description("Sets the outline's border thickness.")]
		[Category("x Properties")]
		public virtual int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				_borderThickness = value;
				Refresh();
			}
		}

		[Description("Sets the control to bind directly with; this in most cases is a Label. This setting also binds with the most appropriate events for the control selected.")]
		[Category("x Properties")]
		public virtual Control BindingControl
		{
			get
			{
				return _bindingControl;
			}
			set
			{
				if ((value != null) & (_bindingControl != value) & (_bindingControl != null))
				{
					_bindingControl.Cursor = Cursors.Default;
					_bindingControl.AccessibleRole = AccessibleRole.Default;
					if (this.BindingControlChanged != null)
						this.BindingControlChanged(this, new BindingControlChangedEventArgs(value));
				}
				_bindingControl = value;
				try
				{
					_bindingControl.AccessibleRole = AccessibleRole.CheckButton;
					if (value == null)
						_bindingControlPoints = default(Point);
					if (_lastBoundControl != value.Name)
						_bindingControlPoints = default(Point);
					PositionBoundControl(true, value);
					value.Click += BindingControl_Click;
					value.MouseHover += BindingControl_MouseHover;
					value.MouseLeave += BindingControl_MouseLeave;
				}
				catch (Exception)
				{
				}
			}
		}

		[Description("Sets the position of the bound control in relation to the radio button.")]
		[Browsable(true)]
		[Category("x Properties")]
		public virtual BindingControlPositions BindingControlPosition
		{
			get
			{
				return _controlPosition;
			}
			set
			{
				_controlPosition = value;
				if (this.BindingControlPositionChanged != null)
					this.BindingControlPositionChanged(this, new PositionChangedEventArgs(value));
				base.Location = new Point(base.Location.X - 1, base.Location.Y);
				PositionBoundControl(true, _bindingControl);
				base.Location = new Point(base.Location.X + 1, base.Location.Y);
			}
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		[Browsable(false)]
		public event EventHandler CheckedChanged = null;

		[DisplayName("CheckedChanged")]
		[Description("Occurs when the 'Checked' property is changed.")]
		[Category("x Events")]
		public event EventHandler<CheckedChangedEventArgs> CheckedChanged2 = null;

		[Description("Occurs whenever the bound control has been changed.")]
		[Category("x Events")]
		public event EventHandler<BindingControlChangedEventArgs> BindingControlChanged = null;

		[Description("Occurs whenever the bound control's position has been changed.")]
		[Category("x Events")]
		public event EventHandler<PositionChangedEventArgs> BindingControlPositionChanged = null;

		public xRadioButton()
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
			base.BackColor = Color.Transparent;
			base.Size = new Size(21, 21);
		}

		private void DrawRadio(Graphics graphics)
		{
			base.Height = base.Width;
			Pen pen = new Pen(_outlineColorChecked, _borderThickness);
			SolidBrush solidBrush = new SolidBrush(_radioColor);
			pen.DashCap = DashCap.Round;
			if (!_checked)
				try
				{
					if (Focused)
					{
						pen.Color = _outlineColorTabFocused;
						solidBrush.Color = Color.Transparent;
					}
					else
					{
						pen.Color = _outlineColorUnchecked;
						solidBrush.Color = Color.Transparent;
					}
				}
				catch (Exception)
				{
				}
			else if (Focused && !_clicked)
			{
				pen.Color = _outlineColorTabFocused;
				solidBrush.Color = _radioColorTabFocused;
			}
			else
			{
				pen.Color = _outlineColorChecked;
				solidBrush.Color = _radioColor;
			}
			Rectangle rectangle = new Rectangle(0, 0, base.Width - 3, base.Height - 3);
			graphics.DrawEllipse(pen, _borderThickness, _borderThickness, rectangle.Width - _borderThickness, rectangle.Height - _borderThickness);
			graphics.FillEllipse(solidBrush, _borderThickness + 4, _borderThickness + 4, rectangle.Width - (_borderThickness + 8), rectangle.Height - (_borderThickness + 8));
			pen.Dispose();
			solidBrush.Dispose();
		}

		private void CheckRadio(object sender)
		{
			try
			{
				foreach (object control in base.Parent.Controls)
				{
					if (control.GetType() == typeof(xRadioButton))
					{
						xRadioButton RadioButton = control as xRadioButton;
						if (sender == RadioButton)
						{
							RadioButton.Checked = true;
							Focus();
						}
						else
							RadioButton.Checked = false;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void ResetOthers()
		{
			try
			{
				if (!base.DesignMode)
					return;
				foreach (Control control in _parentControl.Controls)
				{
					if (control.GetType() == typeof(xRadioButton) && control.GetHashCode() != GetHashCode())
						((xRadioButton)control).Checked = false;
				}
			}
			catch (Exception)
			{
			}
		}

		private void PositionBoundControl(bool isInPropertyMode, Control boundControl)
		{
			if (!AllowBindingControlLocation)
				return;
			if (isInPropertyMode)
			{
				if (boundControl != null)
				{
					boundControl.Cursor = Cursor;
					base.CursorChanged += delegate
					{
						boundControl.Cursor = Cursor;
					};
					if (!(boundControl.GetType() != typeof(Form)))
						return;
					if (_bindingControlPoints == default(Point))
					{
						if (boundControl.Location.X < base.Location.X - 40 && (boundControl.Location.X > base.Location.X - 1 || boundControl.Location.Y < base.Location.Y - 40) && boundControl.Location.Y > base.Location.Y - 1)
							return;
						if (BindingControlPosition != BindingControlPositions.Right)
						{
							if (BindingControlPosition == BindingControlPositions.Left)
								boundControl.Location = new Point(base.Location.X - boundControl.Width - base.Margin.Left, base.Location.Y + base.Height / 4 - 1);
						}
						else
							boundControl.Location = new Point(base.Location.X + base.Width + base.Margin.Right, base.Location.Y + base.Height / 4 - 1);
						_bindingControlPoints = boundControl.Location;
						_lastBoundControl = _bindingControl.Name;
					}
					else
					{
						_bindingControlPoints = default(Point);
						_lastBoundControl = "";
					}
				}
				else
				{
					_bindingControlPoints = default(Point);
					_lastBoundControl = "";
				}
			}
			else if (_bindingControl != null)
			{
				if (BindingControlPosition == BindingControlPositions.Right)
					_bindingControlPoints = new Point(base.Location.X + base.Width + base.Margin.Right, base.Location.Y + base.Height / 4 - 1);
				else if (BindingControlPosition == BindingControlPositions.Left)
				{
					_bindingControlPoints = new Point(base.Location.X - boundControl.Width - base.Margin.Left, base.Location.Y + base.Height / 4 - 1);
				}
				_bindingControl.Location = _bindingControlPoints;
			}
			else
			{
				_bindingControlPoints = default(Point);
			}
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			Graphics graphics = pe.Graphics;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			DrawRadio(graphics);
			base.OnPaint(pe);
		}

		private void xRadioButton_Click(object sender, EventArgs e)
		{
			_clicked = true;
			CheckRadio(sender);
		}

		private void xRadioButton_ParentChanged(object sender, EventArgs e)
		{
			_parentControl = base.Parent;
			CheckRadio(sender);
		}

		private void BindingControl_Click(object sender, EventArgs e)
		{
			xRadioButton_Click(this, e);
		}

		private void BindingControl_MouseHover(object sender, EventArgs e)
		{
			OnMouseHover(e);
		}

		private void BindingControl_MouseLeave(object sender, EventArgs e)
		{
			OnMouseLeave(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Return)
			{
				_clicked = true;
				CheckRadio(this);
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Refresh();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			_clicked = false;
			Refresh();
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			PositionBoundControl(false, _bindingControl);
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
			base.Size = new System.Drawing.Size(25, 25);
			base.Click += new System.EventHandler(xRadioButton_Click);
			base.ParentChanged += new System.EventHandler(xRadioButton_ParentChanged);
			base.ResumeLayout(false);
		}
	}
}
