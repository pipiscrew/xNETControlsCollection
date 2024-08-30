using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace xCollection
{
	[Description("Provides enhanced visual support for on and off states when supporting feature selections.")]
	[DefaultEvent("CheckedChanged")]
	[DefaultProperty("Checked")]
	[DebuggerStepThrough]
	[ToolboxBitmap(typeof(CheckBox))]
	[DefaultBindingProperty("Checked")]
	[Designer(typeof(xDesigner))]
	[Category("x UI For Windows Forms")]
	public class xCheckBox : UserControl, INotifyPropertyChanged
	{
		public enum CheckBoxStyles
		{
			[Description("Renders x's standard CheckBox.")]
			x,
			[Description("Renders a standard flat designed CheckBox.")]
			Flat,
			[Description("Renders a smooth round designed CheckBox.")]
			Round
		}

		public enum CheckStates
		{
			[Description("The checked state.")]
			Checked,
			[Description("The unchecked state.")]
			Unchecked,
			[Description("The indeterminate state.")]
			Indeterminate
		}

		public enum BindingControlPositions
		{
			[Description("Positions the control to the left of the CheckBox.")]
			Left,
			[Description("Positions the control to the right of the CheckBox.")]
			Right
		}

		public class CheckedChangedEventArgs : EventArgs
		{
			private CheckStates _checkstate;

			private bool _checked;

			public bool Checked
			{
				get
				{
					return _checked;
				}
			}

			public CheckStates CheckState
			{
				get
				{
					return _checkstate;
				}
			}

			public CheckedChangedEventArgs(CheckStates state, bool isChecked)
			{
				_checkstate = state;
				_checked = isChecked;
			}
		}

		public class StatePropertiesChangedEventArgs : EventArgs
		{
			private CheckBoxState _checkstate;

			[Description("Provides access to the available state properties in the CheckBox.")]
			public CheckBoxState CurrentState
			{
				get
				{
					return _checkstate;
				}
			}

			[Description("Gets the checkbox inner fill color.")]
			public Color CheckBoxColor
			{
				get
				{
					return _checkstate.CheckBoxColor;
				}
			}

			[Description("Gets the checkbox's border radius.")]
			public int BorderRadius
			{
				get
				{
					return _checkstate.BorderRadius;
				}
			}

			[Description("Gets the checkbox border thickness.")]
			public int BorderThickness
			{
				get
				{
					return _checkstate.BorderThickness;
				}
			}

			[Description("Gets the checkbox border color.")]
			public Color BorderColor
			{
				get
				{
					return _checkstate.BorderColor;
				}
			}

			[Description("Gets the checkbox border color.")]
			public Color CheckmarkColor
			{
				get
				{
					return _checkstate.CheckmarkColor;
				}
			}

			[Description("Gets the checkmark thickness.")]
			public int CheckmarkThickness
			{
				get
				{
					return _checkstate.CheckmarkThickness;
				}
			}

			[Description("Gets a value indicating whether the property UseBorderThicknessForCheckmark has been allowed.")]
			public bool UseBorderThicknessForCheckmark
			{
				get
				{
					return _checkstate.UseBorderThicknessForCheckmark;
				}
			}

			public StatePropertiesChangedEventArgs(CheckBoxState currentState)
			{
				_checkstate = currentState;
			}
		}

		public class StylePropertyChangedEventArgs : EventArgs
		{
			private CheckBoxStyles _style;

			public CheckBoxStyles Style
			{
				get
				{
					return _style;
				}
			}

			public StylePropertyChangedEventArgs(CheckBoxStyles currentStyle)
			{
				_style = currentStyle;
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
					xCheckBox xCheckBox = (xCheckBox)base.Control;
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
							new xCheckBoxActionList(base.Component)
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
		public class xCheckBoxActionList : DesignerActionList
		{
			private xCheckBox xControl;

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
					designerActionUISvc.Refresh(base.Component);
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

			public CheckBoxStyles Style
			{
				get
				{
					return xControl.Style;
				}
				set
				{
					SetValue(xControl, "Style", value);
				}
			}

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

			public CheckStates CheckState
			{
				get
				{
					return xControl.CheckState;
				}
				set
				{
					SetValue(xControl, "CheckState", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public bool AllowCheckmarkAnimation
			{
				get
				{
					return xControl.AllowCheckmarkAnimation;
				}
				set
				{
					SetValue(xControl, "AllowCheckmarkAnimation", value);
				}
			}

			public bool AllowCheckBoxAnimation
			{
				get
				{
					return xControl.AllowCheckBoxAnimation;
				}
				set
				{
					SetValue(xControl, "AllowCheckBoxAnimation", value);
				}
			}

			public bool AllowBindingControlAnimation
			{
				get
				{
					return xControl.AllowBindingControlAnimation;
				}
				set
				{
					SetValue(xControl, "AllowBindingControlAnimation", value);
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

			public bool AllowOnHoverStates
			{
				get
				{
					return xControl.AllowOnHoverStates;
				}
				set
				{
					SetValue(xControl, "AllowOnHoverStates", value);
				}
			}

			public xCheckBoxActionList(IComponent component)
				: base(component)
			{
				xControl = component as xCheckBox;
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
					new DesignerActionHeaderItem("Behaviour"),
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)      ", "Common Tasks"),
					new DesignerActionPropertyItem("BindingControl", "BindingControl:", "Common Tasks", GetPropertyDescriptor(base.Component, "BindingControl").Description),
					new DesignerActionPropertyItem("CheckState", "CheckState:", "Appearance", GetPropertyDescriptor(base.Component, "CheckState").Description),
					new DesignerActionPropertyItem("Checked", "Checked", "Common Tasks", GetPropertyDescriptor(base.Component, "Checked").Description),
					new DesignerActionPropertyItem("Style", "Style:", "Appearance", GetPropertyDescriptor(base.Component, "Style").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Appearance", GetPropertyDescriptor(base.Component, "BorderRadius").Description),
					new DesignerActionMethodItem(this, "Refresh", "Refresh Appearance", "Appearance"),
					new DesignerActionPropertyItem("AllowCheckmarkAnimation", "AllowCheckmarkAnimation", "Behaviour", GetPropertyDescriptor(base.Component, "AllowCheckmarkAnimation").Description),
					new DesignerActionPropertyItem("AllowCheckBoxAnimation", "AllowCheckBoxAnimation", "Behaviour", GetPropertyDescriptor(base.Component, "AllowCheckBoxAnimation").Description),
					new DesignerActionPropertyItem("AllowBindingControlAnimation", "AllowBindingControlAnimation", "Behaviour", GetPropertyDescriptor(base.Component, "AllowBindingControlAnimation").Description),
					new DesignerActionPropertyItem("AllowBindingControlLocation", "AllowBindingControlLocation", "Behaviour", GetPropertyDescriptor(base.Component, "AllowBindingControlLocation").Description),
					new DesignerActionPropertyItem("AllowOnHoverStates", "AllowOnHoverStates", "Behaviour", GetPropertyDescriptor(base.Component, "AllowOnHoverStates").Description)
				};
			}

			public void Refresh()
			{
				xControl.Refresh();
			}
		}

		private const int MINIMUM_SIZE_FOR_THICKNESS = 22;

		private const int MINIMUM_THICKNESS_FOR_SIZE = 2;

		private int _checkboxTop = 0;

		private int _borderRadius = 12;

		private Color _defaultBindingControlForeColor;

		private Bitmap _checkboxBitmap;

		private Rectangle _outerRectangle;

		private Point _bindingControlPoints;

		private Image _customCheckmarkImage;

		private CheckStates _checkState;

		private CheckBoxStyles _checkboxStyles;

		private BindingControlPositions _bindingControlPosition;

		private CheckBoxState _onCheck = new CheckBoxState("OnCheck");

		private CheckBoxState _onUncheck = new CheckBoxState("OnUncheck");

		private CheckBoxState _onHoverChecked = new CheckBoxState("OnHoverChecked");

		private CheckBoxState _onHoverUnchecked = new CheckBoxState("OnHoverUnchecked");

		private CheckBoxState _onDisable = new CheckBoxState("OnDisable");

		private CheckBoxState _currentState = new CheckBoxState("CurrentState");

		private string _tooltipText;

		private string _lastBoundControl;

		private bool _checked;

		private bool _threeState;

		private bool _allowOnHoverStates;

		private bool _allowCheckBoxAnimation;

		private bool _allowCheckmarkAnimation;

		private bool _allowBindingControlLocation;

		private bool _allowBindingControlAnimations;

		private bool _tempAllowBindingControlLocation;

		private bool _allowBindingControlColorChanges;

		private ToolTip _checkboxTip = new ToolTip();

		private Control _bindingControl = new Control();

		private PictureBox _checkmarkImage = new PictureBox();

		private IContainer components = null;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		[Category("x Properties")]
		[Description("Represents the unchecked state of the control.")]
		public virtual CheckBoxState OnUncheck
		{
			get
			{
				if (base.Width <= 22)
				{
					if (_onUncheck.BorderThickness > 2)
						_onUncheck.BorderThickness = 2;
					if (_onUncheck.CheckmarkThickness > 2)
						_onUncheck.CheckmarkThickness = 2;
				}
				if (!Checked)
					_currentState = _onUncheck;
				else
					_currentState = _onCheck;
				Refresh();
				return _onUncheck;
			}
		}

		[Description("Represents the hovered state (mouse-focused) of the control when the CheckBox is checked.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		[Category("x Properties")]
		public virtual CheckBoxState OnHoverChecked
		{
			get
			{
				if (base.Width <= 22)
				{
					if (_onHoverChecked.BorderThickness > 2)
						_onHoverChecked.BorderThickness = 2;
					if (_onHoverChecked.CheckmarkThickness > 2)
						_onHoverChecked.CheckmarkThickness = 2;
				}
				Refresh();
				return _onHoverChecked;
			}
		}

		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		[Description("Represents the hovered state (mouse-focused) of the control when the CheckBox is unchecked.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("x Properties")]
		public virtual CheckBoxState OnHoverUnchecked
		{
			get
			{
				if (base.Width <= 22)
				{
					if (_onHoverUnchecked.BorderThickness > 2)
						_onHoverUnchecked.BorderThickness = 2;
					if (_onHoverUnchecked.CheckmarkThickness > 2)
						_onHoverUnchecked.CheckmarkThickness = 2;
				}
				Refresh();
				return _onHoverUnchecked;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Represents the checked state of the control.")]
		[Category("x Properties")]
		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		public virtual CheckBoxState OnCheck
		{
			get
			{
				if (base.Width <= 22)
				{
					if (_onCheck.BorderThickness > 2)
						_onCheck.BorderThickness = 2;
					if (_onCheck.CheckmarkThickness > 2)
						_onCheck.CheckmarkThickness = 2;
				}
				Refresh();
				return _onCheck;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Represents the disabled or inactive state of the control.")]
		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		[Category("x Properties")]
		public virtual CheckBoxState OnDisable
		{
			get
			{
				if (base.Width <= 22)
				{
					if (_onDisable.BorderThickness > 2)
						_onDisable.BorderThickness = 2;
					if (_onDisable.CheckmarkThickness > 2)
						_onDisable.CheckmarkThickness = 2;
				}
				Refresh();
				return _onDisable;
			}
		}

		[Category("x Properties")]
		[Description("Sets the control's overral border radius.")]
		public virtual int BorderRadius
		{
			get
			{
				return _borderRadius;
			}
			set
			{
				_borderRadius = value;
				OnCheck.BorderRadius = value;
				OnUncheck.BorderRadius = value;
				OnHoverChecked.BorderRadius = value;
				OnHoverUnchecked.BorderRadius = value;
				OnDisable.BorderRadius = value;
				Refresh();
			}
		}

		[Description("Causes the Checkbox to automatically change state when clicked.")]
		[Category("Behavior")]
        private bool autoCheck = true;

        public virtual bool AutoCheck
        {
            get { return autoCheck; }
            set { autoCheck = value; }
        }


		[Description("Sets the CheckBox check-state to true or false.")]
		[Bindable(true)]
		[Category("Appearance")]
		public virtual bool Checked
		{
			get
			{
				return _checked;
			}
			set
			{
				if (base.Enabled)
				{
					_checked = value;
					if (value)
					{
						_currentState = OnCheck;
						CheckState = CheckStates.Checked;
					}
					else if (!value)
					{
						CheckState = CheckStates.Unchecked;
					}
					else
					{
						CheckState = CheckStates.Indeterminate;
					}
					EventHandler<CheckedChangedEventArgs> checkedChanged = this.CheckedChanged;
					if (checkedChanged != null)
						checkedChanged(this, new CheckedChangedEventArgs(CheckState, _checked));
					EventHandler<StatePropertiesChangedEventArgs> statePropertiesChanged = this.StatePropertiesChanged;
					if (statePropertiesChanged != null)
						statePropertiesChanged(this, new StatePropertiesChangedEventArgs(_currentState));
					OnPropertyChange("Checked");
					Refresh();
				}
			}
		}

		[Category("Appearance")]
		[RefreshProperties(RefreshProperties.All)]
		[Description("Sets the checkbox check-state using the CheckState enumeration to Checked or Unchecked.")]
		public virtual CheckStates CheckState
		{
			get
			{
				return _checkState;
			}
			set
			{
				if (base.Enabled)
					_checkState = value;
				switch (value)
				{
				case CheckStates.Checked:
					_checked = true;
					_currentState = OnCheck;
					break;
				case CheckStates.Unchecked:
					_checked = false;
					_currentState = OnUncheck;
					break;
				case CheckStates.Indeterminate:
					_checked = false;
					_currentState = OnUncheck;
					break;
				}
				EventHandler<CheckedChangedEventArgs> checkStateChanged = this.CheckStateChanged;
				if (checkStateChanged != null)
					checkStateChanged(this, new CheckedChangedEventArgs(_checkState, Checked));
				EventHandler<StatePropertiesChangedEventArgs> statePropertiesChanged = this.StatePropertiesChanged;
				if (statePropertiesChanged != null)
					statePropertiesChanged(this, new StatePropertiesChangedEventArgs(_currentState));
				if (base.Enabled)
					Refresh();
			}
		}

		[Description("Indicates whether the checkbox will allow three states, that is, the \"Checked\", \"Unchecked\" and \"Indeterminate\" states rather than \"Checked\" and \"Unchecked\" only.")]
		[Category("Behavior")]
		public virtual bool ThreeState
		{
			get
			{
				return _threeState;
			}
			set
			{
				_threeState = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Indicates whether the Checkmark will allow check-state-change animations/transitions at runtime.")]
		public virtual bool AllowCheckmarkAnimation
		{
			get
			{
				return _allowCheckmarkAnimation;
			}
			set
			{
				_allowCheckmarkAnimation = value;
			}
		}

		[Description("Indicates whether the CheckBox will allow standard animations at runtime.")]
		[Category("x Properties")]
		public virtual bool AllowCheckBoxAnimation
		{
			get
			{
				return _allowCheckBoxAnimation;
			}
			set
			{
				_allowCheckBoxAnimation = value;
			}
		}

		[Category("x Properties")]
		[Description("Indicates whether the CheckBox animation will be applied to the bound control.")]
		public virtual bool AllowBindingControlAnimation
		{
			get
			{
				return _allowBindingControlAnimations;
			}
			set
			{
				_allowBindingControlAnimations = value;
			}
		}

		[Description("Gets or sets a value indicating whether the CheckBox will colorize the bound control. This is especially applicable where the bound control is a Label.")]
		[Category("x Properties")]
		public virtual bool AllowBindingControlColorChanges
		{
			get
			{
				return _allowBindingControlColorChanges;
			}
			set
			{
				_allowBindingControlColorChanges = value;
			}
		}

		[Category("x Properties")]
		[Description("Indicates whether the CheckBox will allow the \"OnHoverChecked\" and \"OnHoverUnchecked\" states to be used at runtime.")]
		public virtual bool AllowOnHoverStates
		{
			get
			{
				return _allowOnHoverStates;
			}
			set
			{
				_allowOnHoverStates = value;
			}
		}

		[Category("x Properties")]
		[Description("Indicates whether the CheckBox will allow the bound control's location to be positioned based on it's own location.")]
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
					HandleControlLocationBinding(true, _bindingControl);
			}
		}

		[Description("Gets or sets the control to bind directly with the CheckBox; this in most cases is a Label. This setting also binds the most appropriate events of the CheckBox to the control selected.")]
		[Category("x Properties")]
		public virtual Control BindingControl
		{
			get
			{
				return _bindingControl;
			}
			set
			{
				if (value == null)
					return;
				if ((value != null) & (_bindingControl != value) & (_bindingControl != null))
				{
					_bindingControl = value;
					_bindingControl.Cursor = Cursors.Default;
					_bindingControl.AccessibleRole = AccessibleRole.Default;
					if (this.BindingControlChanged != null)
						this.BindingControlChanged(this, new BindingControlChangedEventArgs(value));
				}
				try
				{
					_bindingControl.AccessibleRole = AccessibleRole.CheckButton;
					if (value == null)
						_bindingControlPoints = default(Point);
					if (_lastBoundControl != value.Name)
						_bindingControlPoints = default(Point);
					HandleControlLocationBinding(true, value);
					value.Click += BindingControl_Click;
					value.MouseHover += BindingControl_MouseHover;
					value.MouseLeave += BindingControl_MouseLeave;
				}
				catch (Exception)
				{
				}
			}
		}

		[Browsable(true)]
		[Description("Gets or sets the position of the bound control in relation to the CheckBox.")]
		[Category("x Properties")]
		public virtual BindingControlPositions BindingControlPosition
		{
			get
			{
				return _bindingControlPosition;
			}
			set
			{
				_bindingControlPosition = value;
				if (this.BindingControlPositionChanged != null)
					this.BindingControlPositionChanged(this, new PositionChangedEventArgs(value));
				base.Location = new Point(base.Location.X - 1, base.Location.Y);
				HandleControlLocationBinding(true, _bindingControl);
				base.Location = new Point(base.Location.X + 1, base.Location.Y);
			}
		}

		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[Category("x Properties")]
		[Description("Allows you to easily set some ToolTip information to be displayed to the user on mouse-hovering over the control.")]
		public virtual string ToolTipText
		{
			get
			{
				return _tooltipText;
			}
			set
			{
				_tooltipText = value;
				if (value != string.Empty)
				{
					_checkboxTip.UseFading = true;
					_checkboxTip.UseAnimation = true;
					_checkboxTip.SetToolTip(this, value);
				}
			}
		}

		[Description("[Deprecated] Gets or sets the standard CheckBox style to be applied.")]
		[Category("x Properties")]
		public virtual CheckBoxStyles Style
		{
			get
			{
				return _checkboxStyles;
			}
			set
			{
				_checkboxStyles = value;
				if (this.StylePropertyChanged != null)
					this.StylePropertyChanged(this, new StylePropertyChangedEventArgs(value));
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Image CustomCheckmarkImage
		{
			get
			{
				return _customCheckmarkImage;
			}
			set
			{
				try
				{
					if (value != null)
						_customCheckmarkImage = value;
				}
				catch (Exception)
				{
				}
			}
		}

		[Browsable(false)]
		public virtual Rectangle CheckmarkRectangle
		{
			get
			{
				return _checkmarkImage.DisplayRectangle;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
				base.Font = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		private static bool IsDesignMode
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

		[Category("x Events")]
		[Description("Occurs whenever the Checked property has been changed.")]
		public event EventHandler<CheckedChangedEventArgs> CheckedChanged = null;

		[Description("Occurs whenever the CheckState property has been changed.")]
		[Category("x Events")]
		public event EventHandler<CheckedChangedEventArgs> CheckStateChanged = null;

		[Description("Occurs whenever the active CheckBox state's properties have been changed.")]
		[Category("x Events")]
		public event EventHandler<StatePropertiesChangedEventArgs> StatePropertiesChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the CheckBox Style property has been changed.")]
		public event EventHandler<StylePropertyChangedEventArgs> StylePropertyChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the bound control has been changed.")]
		public event EventHandler<BindingControlChangedEventArgs> BindingControlChanged = null;

		[Description("Occurs whenever the bound control's Position property has been changed.")]
		[Category("x Events")]
		public event EventHandler<PositionChangedEventArgs> BindingControlPositionChanged = null;

		public event PropertyChangedEventHandler PropertyChanged;

		public xCheckBox()
		{
			InitializeComponent();
			SuspendLayout();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.Controls.Add(_checkmarkImage);
			base.Size = new Size(21, 21);
			Cursor = Cursors.Default;
			BackColor = Color.Transparent;
			BindingControl = null;
			BindingControlPosition = BindingControlPositions.Right;
			AllowCheckmarkAnimation = true;
			AllowBindingControlAnimation = true;
			AllowOnHoverStates = true;
			AllowBindingControlLocation = true;
			AllowBindingControlAnimation = true;
			OnUncheck.BorderColor = Color.DarkGray;
			OnUncheck.BorderRadius = 12;
			OnUncheck.BorderThickness = 1;
			OnUncheck.CheckBoxColor = Color.Transparent;
			OnUncheck.CheckmarkColor = Color.Transparent;
			OnUncheck.CheckmarkThickness = 2;
			OnUncheck.UseBorderThicknessForCheckmark = false;
			OnHoverChecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.BorderRadius = 12;
			OnHoverChecked.BorderThickness = 2;
			OnHoverChecked.CheckBoxColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.CheckmarkColor = Color.White;
			OnHoverChecked.CheckmarkThickness = 2;
			OnHoverChecked.UseBorderThicknessForCheckmark = false;
			OnHoverUnchecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverUnchecked.BorderRadius = 12;
			OnHoverUnchecked.BorderThickness = 1;
			OnHoverUnchecked.CheckBoxColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkThickness = 2;
			OnHoverUnchecked.UseBorderThicknessForCheckmark = false;
			OnCheck.BorderColor = Color.DodgerBlue;
			OnCheck.BorderRadius = 12;
			OnCheck.BorderThickness = 2;
			OnCheck.CheckBoxColor = Color.DodgerBlue;
			OnCheck.CheckmarkColor = Color.White;
			OnCheck.CheckmarkThickness = 2;
			OnCheck.UseBorderThicknessForCheckmark = false;
			OnDisable.BorderColor = Color.LightGray;
			OnDisable.BorderRadius = 12;
			OnDisable.BorderThickness = 2;
			OnDisable.CheckBoxColor = Color.Transparent;
			OnDisable.CheckmarkColor = Color.LightGray;
			OnDisable.CheckmarkThickness = 2;
			OnDisable.UseBorderThicknessForCheckmark = false;
			Style = CheckBoxStyles.x;
			Checked = true;
			_currentState = _onCheck;
			Refresh();
			ResumeLayout(false);
		}

		public xCheckBox(bool isChecked)
		{
			InitializeComponent();
			SuspendLayout();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.Controls.Add(_checkmarkImage);
			base.Size = new Size(21, 21);
			base.Location = new Point(2, 2);
			Cursor = Cursors.Default;
			BackColor = Color.Transparent;
			BindingControl = null;
			BindingControlPosition = BindingControlPositions.Right;
			AllowCheckmarkAnimation = true;
			AllowBindingControlAnimation = true;
			AllowOnHoverStates = true;
			AllowBindingControlLocation = true;
			AllowBindingControlAnimation = true;
			OnUncheck.BorderColor = Color.DarkGray;
			OnUncheck.BorderRadius = 1;
			OnUncheck.BorderThickness = 1;
			OnUncheck.CheckBoxColor = Color.Transparent;
			OnUncheck.CheckmarkColor = Color.Transparent;
			OnUncheck.CheckmarkThickness = 2;
			OnUncheck.UseBorderThicknessForCheckmark = false;
			OnHoverChecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.BorderRadius = 1;
			OnHoverChecked.BorderThickness = 2;
			OnHoverChecked.CheckBoxColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.CheckmarkColor = Color.White;
			OnHoverChecked.CheckmarkThickness = 2;
			OnHoverChecked.UseBorderThicknessForCheckmark = false;
			OnHoverUnchecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverUnchecked.BorderRadius = 1;
			OnHoverUnchecked.BorderThickness = 1;
			OnHoverUnchecked.CheckBoxColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkThickness = 2;
			OnHoverUnchecked.UseBorderThicknessForCheckmark = false;
			OnCheck.BorderColor = Color.DodgerBlue;
			OnCheck.BorderRadius = 1;
			OnCheck.BorderThickness = 2;
			OnCheck.CheckBoxColor = Color.DodgerBlue;
			OnCheck.CheckmarkColor = Color.White;
			OnCheck.CheckmarkThickness = 2;
			OnCheck.UseBorderThicknessForCheckmark = false;
			OnDisable.BorderColor = Color.LightGray;
			OnDisable.BorderRadius = 1;
			OnDisable.BorderThickness = 2;
			OnDisable.CheckBoxColor = Color.Transparent;
			OnDisable.CheckmarkColor = Color.LightGray;
			OnDisable.CheckmarkThickness = 2;
			OnDisable.UseBorderThicknessForCheckmark = false;
			Style = CheckBoxStyles.x;
			Checked = isChecked;
			_currentState = _onCheck;
			Refresh();
			ResumeLayout(false);
		}

		public xCheckBox(bool isChecked, Control bindingControl)
		{
			InitializeComponent();
			SuspendLayout();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.Controls.Add(_checkmarkImage);
			base.Size = new Size(21, 21);
			base.Location = new Point(2, 2);
			Cursor = Cursors.Default;
			Style = CheckBoxStyles.x;
			BackColor = Color.Transparent;
			BindingControl = bindingControl;
			BindingControlPosition = BindingControlPositions.Right;
			AllowCheckmarkAnimation = true;
			AllowBindingControlAnimation = true;
			AllowOnHoverStates = true;
			AllowBindingControlLocation = true;
			AllowBindingControlAnimation = true;
			OnUncheck.BorderColor = Color.DarkGray;
			OnUncheck.BorderRadius = 1;
			OnUncheck.BorderThickness = 1;
			OnUncheck.CheckBoxColor = Color.Transparent;
			OnUncheck.CheckmarkColor = Color.Transparent;
			OnUncheck.CheckmarkThickness = 2;
			OnUncheck.UseBorderThicknessForCheckmark = false;
			OnHoverChecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.BorderRadius = 1;
			OnHoverChecked.BorderThickness = 2;
			OnHoverChecked.CheckBoxColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.CheckmarkColor = Color.White;
			OnHoverChecked.CheckmarkThickness = 2;
			OnHoverChecked.UseBorderThicknessForCheckmark = false;
			OnHoverUnchecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverUnchecked.BorderRadius = 1;
			OnHoverUnchecked.BorderThickness = 1;
			OnHoverUnchecked.CheckBoxColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkThickness = 2;
			OnHoverUnchecked.UseBorderThicknessForCheckmark = false;
			OnCheck.BorderColor = Color.DodgerBlue;
			OnCheck.BorderRadius = 1;
			OnCheck.BorderThickness = 2;
			OnCheck.CheckBoxColor = Color.DodgerBlue;
			OnCheck.CheckmarkColor = Color.White;
			OnCheck.CheckmarkThickness = 2;
			OnCheck.UseBorderThicknessForCheckmark = false;
			OnDisable.BorderColor = Color.LightGray;
			OnDisable.BorderRadius = 1;
			OnDisable.BorderThickness = 2;
			OnDisable.CheckBoxColor = Color.Transparent;
			OnDisable.CheckmarkColor = Color.LightGray;
			OnDisable.CheckmarkThickness = 2;
			OnDisable.UseBorderThicknessForCheckmark = false;
			Style = CheckBoxStyles.x;
			Checked = isChecked;
			_currentState = _onCheck;
			Refresh();
			ResumeLayout(false);
		}

		public xCheckBox(bool isChecked, Control bindingControl, Point location)
		{
			InitializeComponent();
			SuspendLayout();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.Controls.Add(_checkmarkImage);
			base.Size = new Size(21, 21);
			Cursor = Cursors.Default;
			BackColor = Color.Transparent;
			Style = CheckBoxStyles.x;
			base.Location = new Point(location.X, location.Y);
			BindingControl = bindingControl;
			BindingControlPosition = BindingControlPositions.Right;
			AllowCheckmarkAnimation = true;
			AllowBindingControlAnimation = true;
			AllowOnHoverStates = true;
			AllowBindingControlLocation = true;
			AllowBindingControlAnimation = true;
			OnUncheck.BorderColor = Color.DarkGray;
			OnUncheck.BorderRadius = 1;
			OnUncheck.BorderThickness = 1;
			OnUncheck.CheckBoxColor = Color.Transparent;
			OnUncheck.CheckmarkColor = Color.Transparent;
			OnUncheck.CheckmarkThickness = 2;
			OnUncheck.UseBorderThicknessForCheckmark = false;
			OnHoverChecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.BorderRadius = 1;
			OnHoverChecked.BorderThickness = 2;
			OnHoverChecked.CheckBoxColor = Color.FromArgb(105, 181, 255);
			OnHoverChecked.CheckmarkColor = Color.White;
			OnHoverChecked.CheckmarkThickness = 2;
			OnHoverChecked.UseBorderThicknessForCheckmark = false;
			OnHoverUnchecked.BorderColor = Color.FromArgb(105, 181, 255);
			OnHoverUnchecked.BorderRadius = 1;
			OnHoverUnchecked.BorderThickness = 1;
			OnHoverUnchecked.CheckBoxColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkColor = Color.Transparent;
			OnHoverUnchecked.CheckmarkThickness = 2;
			OnHoverUnchecked.UseBorderThicknessForCheckmark = false;
			OnCheck.BorderColor = Color.DodgerBlue;
			OnCheck.BorderRadius = 1;
			OnCheck.BorderThickness = 2;
			OnCheck.CheckBoxColor = Color.DodgerBlue;
			OnCheck.CheckmarkColor = Color.White;
			OnCheck.CheckmarkThickness = 2;
			OnCheck.UseBorderThicknessForCheckmark = false;
			OnDisable.BorderColor = Color.LightGray;
			OnDisable.BorderRadius = 1;
			OnDisable.BorderThickness = 2;
			OnDisable.CheckBoxColor = Color.Transparent;
			OnDisable.CheckmarkColor = Color.LightGray;
			OnDisable.CheckmarkThickness = 2;
			OnDisable.UseBorderThicknessForCheckmark = false;
			Style = CheckBoxStyles.x;
			Checked = isChecked;
			_currentState = _onCheck;
			Refresh();
			ResumeLayout(false);
		}

		public virtual Control NewBindingControl(Control control)
		{
			base.ParentForm.Controls.Add(control);
			return control;
		}

		public virtual Control NewBindingLabel(string text, Font font = null, Color foreColor = default(Color))
		{
			font = new Font("Segoe UI", 8.25f, FontStyle.Regular);
			return NewBindingControl(new Label
			{
				Text = text,
				Font = font,
				ForeColor = foreColor,
				AutoSize = true
			});
		}

		public override void Refresh()
		{
			if (!base.Enabled)
			{
				RenderCanvas(_onDisable);
				if (BindingControl != null)
				{
					BindingControl.Enabled = false;
					BindingControl.Cursor = Cursors.Default;
				}
			}
			else
			{
				RenderCanvas(_currentState);
				if (BindingControl != null)
				{
					BindingControl.Enabled = true;
					BindingControl.Cursor = Cursor;
				}
			}
		}

		private void DrawCanvas(Graphics graphics, Rectangle Bounds, int cornerRadius, Pen drawingPen, Color fillColor, CheckBoxState checkBoxState)
		{
			graphics.Clear(Color.Transparent);
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			Rectangle rectangle = new Rectangle(Bounds.Width, Bounds.Height / 2, base.Width, base.Height);
			GraphicsPath graphicsPath = new GraphicsPath();
			SolidBrush solidBrush = new SolidBrush(fillColor);
			drawingPen.StartCap = LineCap.Round;
			drawingPen.EndCap = LineCap.Round;
			if (Style == CheckBoxStyles.Round)
				graphicsPath.AddEllipse(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height);
			else if (checkBoxState.BorderRadius > 1)
			{
				graphicsPath.AddArc(Bounds.X, Bounds.Y, cornerRadius, cornerRadius, 180f, 90f);
				graphicsPath.AddArc(Bounds.X + Bounds.Width - cornerRadius, Bounds.Y, cornerRadius, cornerRadius, 270f, 90f);
				graphicsPath.AddArc(Bounds.X + Bounds.Width - cornerRadius, Bounds.Y + Bounds.Height - cornerRadius, cornerRadius, cornerRadius, 0f, 90f);
				graphicsPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - cornerRadius, cornerRadius, cornerRadius, 90f, 90f);
				graphicsPath.Flatten();
			}
			else
			{
				graphicsPath.AddRectangle(new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height));
			}
			graphicsPath.CloseAllFigures();
			graphics.FillPath(solidBrush, graphicsPath);
			graphics.DrawPath(drawingPen, graphicsPath);
			solidBrush.Dispose();
			graphicsPath.Dispose();
			drawingPen.Dispose();
		}

		private Bitmap DrawCheckmark(ref Graphics graphics, ref Pen checkmarkPen, ref CheckBoxState checkBoxState, Point StartPoint, Point LeftPoint, Point RightPoint, int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height);
			using (graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				using (checkmarkPen)
					graphics.DrawPolygon(checkmarkPen, new Point[4] { StartPoint, LeftPoint, StartPoint, RightPoint });
			}
			_checkmarkImage.Image = bitmap;
			_checkmarkImage.Enabled = false;
			return bitmap;
		}

		private void RenderCanvas(CheckBoxState checkBoxState)
		{
			try
			{
				Pen pen = new Pen(checkBoxState.BorderColor, checkBoxState.BorderThickness);
				_checkboxBitmap = new Bitmap(base.Size.Width, base.Size.Height);
				Graphics graphics = Graphics.FromImage(_checkboxBitmap);
				Rectangle bounds = (_outerRectangle = new Rectangle(checkBoxState.BorderThickness, checkBoxState.BorderThickness, base.Width - checkBoxState.BorderThickness * 2, base.Height - checkBoxState.BorderThickness * 2));
				_checkmarkImage.Width = base.Width;
				_checkmarkImage.Height = base.Height;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.Clear(BackColor);
				DrawCanvas(graphics, bounds, checkBoxState.BorderRadius, pen, checkBoxState.CheckBoxColor, checkBoxState);
				BackgroundImage = _checkboxBitmap;
				BackgroundImageLayout = ImageLayout.Zoom;
				Brush brush = new SolidBrush(checkBoxState.BorderColor);
				Pen checkmarkPen = new Pen(checkBoxState.CheckmarkColor, checkBoxState.CheckmarkThickness);
				Pen pen2 = new Pen(checkBoxState.CheckBoxColor, checkBoxState.CheckmarkThickness);
				Point point = new Point(bounds.Width, checkBoxState.BorderThickness + checkBoxState.BorderRadius);
				if (CheckState == CheckStates.Indeterminate)
				{
					HideCheckmark(true);
					GraphicsPath graphicsPath = new GraphicsPath();
					Pen pen3 = checkmarkPen;
					checkmarkPen.StartCap = LineCap.Round;
					pen3.EndCap = LineCap.Round;
					int num = checkBoxState.BorderRadius + checkBoxState.BorderThickness;
					if (Style == CheckBoxStyles.Round)
						graphics.FillEllipse(brush, bounds.Location.X + 4, bounds.Location.Y + 4, bounds.Width - 8, bounds.Height - 8);
					else if (checkBoxState.BorderRadius > 1)
					{
						graphicsPath.AddRectangle(new Rectangle(bounds.Location.X + checkBoxState.BorderRadius, bounds.Location.Y + checkBoxState.BorderRadius, bounds.Width - checkBoxState.BorderRadius * 2, bounds.Height - checkBoxState.BorderRadius * 2));
					}
					else
					{
						graphicsPath.AddRectangle(new Rectangle(bounds.Location.X + num, bounds.Location.Y + num, bounds.Width - num * 2, bounds.Height - num * 2));
					}
					graphicsPath.CloseAllFigures();
					graphics.FillPath(brush, graphicsPath);
					pen2.Dispose();
					brush.Dispose();
					graphicsPath.Dispose();
				}
				else if (AutoCheck)
				{
					if (Checked)
					{
						ShowCheckmark(true);
						int num2 = checkBoxState.BorderThickness;
						if (checkBoxState.BorderThickness > 2)
							num2 = 2;
						if (checkBoxState.UseBorderThicknessForCheckmark)
						{
							Point startPoint = new Point(bounds.Left + bounds.Width / 2, (int)((double)(bounds.Bottom - bounds.Bottom / 2) + (double)bounds.Bottom * 0.25) - 1);
							Point leftPoint = new Point((int)((double)bounds.Width - (double)bounds.Width * 0.5 - (double)bounds.Width * 0.25) + num2 - 1, bounds.Top + bounds.Height / 2 + num2 - 1);
							Point rightPoint = new Point((int)((double)bounds.Right - (double)bounds.Width * 0.2) - num2 + 1, (int)((double)bounds.Top + (double)bounds.Height * 0.2) + num2 - 1);
							if (Style == CheckBoxStyles.Round)
							{
								startPoint = new Point(bounds.Left + bounds.Width / 2, (int)((double)(bounds.Bottom - bounds.Bottom / 2) + (double)bounds.Bottom * 0.25) - 1);
								leftPoint = new Point((int)((double)bounds.Width - (double)bounds.Width * 0.5 - (double)bounds.Width * 0.25) + num2, bounds.Top + bounds.Height / 2 + num2 - 2);
								rightPoint = new Point((int)((double)bounds.Right - (double)bounds.Width * 0.2) - num2 + 2, (int)((double)bounds.Top + (double)bounds.Height * 0.2) + num2 + 1);
							}
							DrawCheckmark(ref graphics, ref checkmarkPen, ref checkBoxState, startPoint, leftPoint, rightPoint, base.Width, base.Height);
						}
						else
						{
							Point startPoint2 = new Point(bounds.Left + bounds.Width / 2, (int)((double)(bounds.Bottom - bounds.Bottom / 2) + (double)bounds.Bottom * 0.25) - 1);
							Point leftPoint2 = new Point((int)((double)bounds.Width - (double)bounds.Width * 0.5 - (double)bounds.Width * 0.25) + num2 - 1, bounds.Top + bounds.Height / 2 + num2 - 2);
							Point rightPoint2 = new Point((int)((double)bounds.Right - (double)bounds.Width * 0.2) - num2 + 2, (int)((double)bounds.Top + (double)bounds.Height * 0.2) + num2 - 1);
							if (Style == CheckBoxStyles.Round)
							{
								leftPoint2 = new Point((int)((double)bounds.Width - (double)bounds.Width * 0.5 - (double)bounds.Width * 0.25) + num2, bounds.Top + bounds.Height / 2 + num2 - 2);
								rightPoint2 = new Point((int)((double)bounds.Right - (double)bounds.Width * 0.2) - num2 + 2, (int)((double)bounds.Top + (double)bounds.Height * 0.2) + num2 + 1);
							}
							DrawCheckmark(ref graphics, ref checkmarkPen, ref checkBoxState, startPoint2, leftPoint2, rightPoint2, base.Width, base.Height);
						}
					}
					else
						HideCheckmark(true);
				}
				pen.Dispose();
				checkmarkPen.Dispose();
				graphics.Dispose();
			}
			catch (Exception)
			{
			}
		}

		public void OnPropertyChange(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		private void PerformCheckStateTransitions()
		{
			if (AllowCheckBoxAnimation)
			{
				if (!AllowBindingControlAnimation)
				{
					_tempAllowBindingControlLocation = AllowBindingControlLocation;
					AllowBindingControlLocation = false;
				}
				Transition transition = new Transition(new TransitionType_Flash(2, 200));
				transition.TransitionCompletedEvent += AnimateCheckBox_TransitionCompletedEvent;
				transition.add(this, "Top", base.Top - 3);
				transition.run();
			}
			if (AllowCheckmarkAnimation)
			{
				if (Checked)
					ShowCheckmark(false);
				else
					HideCheckmark(false);
			}
			else if (Checked)
			{
				_checkmarkImage.Show();
			}
			else
			{
				_checkmarkImage.Hide();
			}
		}

		private void ShowCheckmark(bool isRendering)
		{
			if (!isRendering)
			{
				_checkmarkImage.Width = 0;
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(300));
				transition.add(_checkmarkImage, "Width", base.Width);
				transition.run();
			}
			else
				_checkmarkImage.Width = base.Width;
		}

		private void HideCheckmark(bool isRendering)
		{
			if (!isRendering)
			{
				_checkmarkImage.Width = base.Width;
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(300));
				transition.add(_checkmarkImage, "Width", 0);
				transition.run();
			}
			else
				_checkmarkImage.Width = 0;
		}

		private void AnimateCheckBox_TransitionCompletedEvent(object sender, Transition.Args e)
		{
			if (AllowBindingControlAnimation && _tempAllowBindingControlLocation)
				AllowBindingControlLocation = true;
		}

		private void AnimateChecks_TransitionCompletedEvent(object sender, Transition.Args e)
		{
		}

		private void HandleControlLocationBinding(bool isInPropertyMode, Control boundControl)
		{
			if (!AllowBindingControlLocation)
				return;
			if (isInPropertyMode)
			{
				if (boundControl != null)
				{
					boundControl.Cursor = Cursor;
					if (!(boundControl.GetType() != typeof(Form)))
						return;
					if (_bindingControlPoints == default(Point))
					{
						if (boundControl.Location.X >= base.Location.X - 40 || (boundControl.Location.X <= base.Location.X - 1 && boundControl.Location.Y >= base.Location.Y - 40) || boundControl.Location.Y <= base.Location.Y - 1)
						{
							if (BindingControlPosition == BindingControlPositions.Right)
								boundControl.Location = new Point(base.Location.X + base.Width + base.Margin.Right, base.Location.Y + base.Height / 4 - 1);
							else if (BindingControlPosition == BindingControlPositions.Left)
							{
								boundControl.Location = new Point(base.Location.X - boundControl.Width - base.Margin.Left, base.Location.Y + base.Height / 4 - 1);
							}
							_bindingControlPoints = boundControl.Location;
							_lastBoundControl = _bindingControl.Name;
						}
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

		private bool IsInDesignMode()
		{
			if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
				return true;
			return false;
		}

		protected override void OnValidating(CancelEventArgs e)
		{
			base.OnValidating(e);
			EventHandler<CheckedChangedEventArgs> checkedChanged = this.CheckedChanged;
			if (checkedChanged != null)
				checkedChanged(this, new CheckedChangedEventArgs(_checkState, _checked));
		}

		private void xCheckBox_Resize(object sender, EventArgs e)
		{
			Refresh();
			_currentState.CheckBoxSize = base.Width;
			base.Height = base.Width;
			HandleControlLocationBinding(false, _bindingControl);
		}

		private void xCheckBox_LocationChanged(object sender, EventArgs e)
		{
			HandleControlLocationBinding(false, _bindingControl);
		}

		private void xCheckBox_Click(object sender, EventArgs e)
		{
			if (AutoCheck)
			{
				Checked = !Checked;
				if (Checked)
					_currentState = OnCheck;
				else
					_currentState = OnUncheck;
			}
			if (BindingControl != null && AllowBindingControlColorChanges)
			{
				if (!(BindingControl is LinkLabel))
					BindingControl.ForeColor = _defaultBindingControlForeColor;
				else
					((LinkLabel)BindingControl).LinkColor = _defaultBindingControlForeColor;
			}
			if (AutoCheck)
				PerformCheckStateTransitions();
		}

		private void xCheckBox_MouseHover(object sender, EventArgs e)
		{
			if (!AllowOnHoverStates)
				return;
			if (Checked)
				_currentState = OnHoverChecked;
			else
				_currentState = OnHoverUnchecked;
			if (BindingControl != null)
			{
				if (BindingControl is LinkLabel)
				{
					if (AllowBindingControlColorChanges)
					{
						_defaultBindingControlForeColor = ((LinkLabel)BindingControl).LinkColor;
						((LinkLabel)BindingControl).LinkColor = OnHoverChecked.BorderColor;
					}
				}
				else if (AllowBindingControlColorChanges)
				{
					_defaultBindingControlForeColor = BindingControl.ForeColor;
					BindingControl.ForeColor = OnHoverChecked.BorderColor;
				}
			}
			Refresh();
			if (this.StatePropertiesChanged != null)
				this.StatePropertiesChanged(this, new StatePropertiesChangedEventArgs(_currentState));
		}

		private void xCheckBox_MouseLeave(object sender, EventArgs e)
		{
			if (!AllowOnHoverStates)
				return;
			if (Checked)
				_currentState = OnCheck;
			else
				_currentState = OnUncheck;
			if (BindingControl != null && AllowBindingControlColorChanges)
			{
				if (!(BindingControl is LinkLabel))
					BindingControl.ForeColor = _defaultBindingControlForeColor;
				else
					((LinkLabel)BindingControl).LinkColor = _defaultBindingControlForeColor;
			}
			Refresh();
			if (this.StatePropertiesChanged != null)
				this.StatePropertiesChanged(this, new StatePropertiesChangedEventArgs(_currentState));
		}

		private void xCheckBox_MarginChanged(object sender, EventArgs e)
		{
			HandleControlLocationBinding(false, _bindingControl);
		}

		private void xCheckBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Space)
			{
				Checked = !Checked;
				PerformCheckStateTransitions();
			}
		}

		private void BindingControl_Click(object sender, EventArgs e)
		{
			xCheckBox_Click(sender, e);
		}

		private void BindingControl_MouseHover(object sender, EventArgs e)
		{
			xCheckBox_MouseHover(sender, e);
		}

		private void BindingControl_MouseLeave(object sender, EventArgs e)
		{
			xCheckBox_MouseLeave(sender, e);
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
			this.MinimumSize = new System.Drawing.Size(17, 17);
			base.Name = "xCheckBox";
			base.Size = new System.Drawing.Size(22, 22);
			base.LocationChanged += new System.EventHandler(xCheckBox_LocationChanged);
			base.MarginChanged += new System.EventHandler(xCheckBox_MarginChanged);
			base.Click += new System.EventHandler(xCheckBox_Click);
			base.KeyDown += new System.Windows.Forms.KeyEventHandler(xCheckBox_KeyDown);
			base.MouseLeave += new System.EventHandler(xCheckBox_MouseLeave);
			base.MouseHover += new System.EventHandler(xCheckBox_MouseHover);
			base.Resize += new System.EventHandler(xCheckBox_Resize);
			base.ResumeLayout(false);
		}
	}
}
