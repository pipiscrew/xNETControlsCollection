using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Utilities.xSlider;

namespace xCollection
{
	[Category("x UI For Windows Forms")]
	[Designer(typeof(xDesigner))]
	[ToolboxBitmap(typeof(TrackBar))]
	[DefaultProperty("Value")]
	[DefaultEvent("Scroll")]
	[DebuggerStepThrough]
	[ToolboxItem(true)]
	[Description("Provides enhanced horizontal ranged value selections and extra customization options.")]
	public class xHSlider : xHScrollBar
	{
		public new enum ThumbStyles
		{
			Fill,
			Outline
		}

		public enum SliderStyles
		{
			Thick,
			Thin
		}

		public enum ThumbSizes
		{
			Small,
			Medium,
			Large
		}

		public enum SliderStates
		{
			[Description("Denotes the Slider's idle state.")]
			Idle,
			[Description("Denotes the Slider's mouse-hover state.")]
			Hover,
			[Description("Denotes the Slider's mouse-press or click state.")]
			Pressed
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public new class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					xHSlider HSlider = (xHSlider)base.Control;
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
							new xHSliderActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		public class xHSliderActionList : DesignerActionList
		{
			private xHSlider xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public int Value
			{
				get
				{
					return xControl.Value;
				}
				set
				{
					SetValue(xControl, "Value", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public int Maximum
			{
				get
				{
					return xControl.Maximum;
				}
				set
				{
					SetValue(xControl, "Maximum", value);
				}
			}

			public int Minimum
			{
				get
				{
					return xControl.Minimum;
				}
				set
				{
					SetValue(xControl, "Minimum", value);
				}
			}

			public int LargeChange
			{
				get
				{
					return xControl.LargeChange;
				}
				set
				{
					SetValue(xControl, "LargeChange", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public int SmallChange
			{
				get
				{
					return xControl.SmallChange;
				}
				set
				{
					SetValue(xControl, "SmallChange", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color SliderColor
			{
				get
				{
					return xControl.SliderColor;
				}
				set
				{
					SetValue(xControl, "SliderColor", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color ElapsedColor
			{
				get
				{
					return xControl.ElapsedColor;
				}
				set
				{
					SetValue(xControl, "ElapsedColor", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color ThumbColor
			{
				get
				{
					return xControl.ThumbColor;
				}
				set
				{
					SetValue(xControl, "ThumbColor", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public ThumbSizes ThumbSize
			{
				get
				{
					return xControl.ThumbSize;
				}
				set
				{
					SetValue(xControl, "ThumbSize", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public xHSliderActionList(IComponent component)
				: base(component)
			{
				xControl = component as xHSlider;
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
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("Minimum", "Minimum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Minimum").Description),
					new DesignerActionPropertyItem("LargeChange", "LargeChange:", "Behaviour", GetPropertyDescriptor(base.Component, "LargeChange").Description),
					new DesignerActionPropertyItem("SmallChange", "SmallChange:", "Behaviour", GetPropertyDescriptor(base.Component, "SmallChange").Description),
					new DesignerActionPropertyItem("SliderColor", "SliderColor:", "Appearance", GetPropertyDescriptor(base.Component, "SliderColor").Description),
					new DesignerActionPropertyItem("ElapsedColor", "ElapsedColor:", "Appearance", GetPropertyDescriptor(base.Component, "ElapsedColor").Description),
					new DesignerActionPropertyItem("ThumbColor", "ThumbColor:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbColor").Description),
					new DesignerActionPropertyItem("ThumbSize", "ThumbSize:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbSize").Description)
				};
			}
		}

		[Description("An abstract class used to define various states within x Sliders.")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class SliderState
		{
			[EditorBrowsable(EditorBrowsableState.Always)]
			[Browsable(false)]
			public virtual string Name { get; internal set; }

			[Category("x Properties")]
			[Description("Sets the background color of the Slider.")]
			public Color SliderColor { get; set; }

			[Category("x Properties")]
			[Description("Sets the slider's elapsed region color.")]
			public Color ElapsedColor { get; set; }

			[Description("Sets the background color of the Thumb.")]
			[Category("x Properties")]
			public Color ThumbColor { get; set; }

			[Description("Sets the thumb's inner fill color. This is only necessary when the property ThumbStyle' is set to 'Outline'.")]
			[Category("x Properties")]
			public Color ThumbFillColor { get; set; }

			public SliderState(string name = "")
			{
				Name = name;
			}

			public override string ToString()
			{
				return SliderColor.ToString() + "; " + ElapsedColor.ToString() + "; " + ThumbColor.ToString();
			}
		}

		private bool _allowMouseEffects;

		private int _animationSpeed;

		private Color _elapsedColor;

		private ThumbStyles _thumbStyle;

		private SliderStates _focusState;

		private SliderStyles _sliderStyle;

		private SliderState _tempState = new SliderState();

		private SliderState _onIdle = new SliderState("OnIdle");

		private SliderState _onHover = new SliderState("OnHover");

		private SliderState _onPress = new SliderState("OnPress");

		private SliderState _onDisable = new SliderState("OnDisable");

		private IContainer components = null;

		[Category("x Properties")]
		[Description("Sets a value indicating whether the control will provide a visual cue when focused.")]
		private bool IndicateFocus { get; set; }

		[Description("Sets the Slider's animation speed (in milliseconds) when moving from one state to another.")]
		[Category("x Properties")]
		private int AnimationSpeed
		{
			get
			{
				return _animationSpeed;
			}
			set
			{
				if (value == 0)
					value = 1;
				_animationSpeed = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the slider's BackColor.")]
		public virtual Color SliderColor
		{
			get
			{
				return ScrollBarColor;
			}
			set
			{
				ScrollBarColor = value;
				ScrollBarBorderColor = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the slider's elapsed region color.")]
		public virtual Color ElapsedColor
		{
			get
			{
				return _elapsedColor;
			}
			set
			{
				_elapsedColor = value;
				Refresh();
			}
		}

		[Description("Sets the thumb's inner fill color. This is only necessary when the property ThumbStyle' is set to 'Outline'.")]
		[Category("x Properties")]
		public override Color ThumbFillColor
		{
			get
			{
				return base.ThumbFillColor;
			}
			set
			{
				base.ThumbFillColor = value;
			}
		}

		[Description("Sets the slider's thumb style.")]
		[Category("x Properties")]
		public new ThumbStyles ThumbStyle
		{
			get
			{
				return _thumbStyle;
			}
			set
			{
				_thumbStyle = value;
				switch (value)
				{
				case ThumbStyles.Fill:
					base.ThumbDrawMode = DrawModes.Fill;
					break;
				case ThumbStyles.Outline:
					base.ThumbDrawMode = DrawModes.Outline;
					break;
				}
				Refresh();
			}
		}

		[Description("Sets the slider's overral style.")]
		[Category("x Properties")]
		public SliderStyles SliderStyle
		{
			get
			{
				return _sliderStyle;
			}
			set
			{
				_sliderStyle = value;
				switch (value)
				{
				case SliderStyles.Thick:
					base.DrawThickBorder = true;
					break;
				case SliderStyles.Thin:
					base.DrawThickBorder = false;
					break;
				}
			}
		}

		[Description("Sets the state to use when the Slider contains focus while the cursor is away.")]
		[Category("x Properties")]
		private SliderStates FocusState
		{
			get
			{
				return _focusState;
			}
			set
			{
				_focusState = value;
				if (!base.DesignMode)
					Invalidate();
			}
		}

		[Editor(typeof(HSliderStatesColorEditor), typeof(UITypeEditor))]
		[Browsable(true)]
		[Category("x Properties")]
		[Description("Represents the idle state of the control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		private SliderState OnIdleState
		{
			get
			{
				return _onIdle;
			}
		}

		[Browsable(true)]
		[Category("x Properties")]
		[DisplayName("OnDisabledState")]
		[Editor(typeof(HSliderStatesColorEditor), typeof(UITypeEditor))]
		[Description("Represents the disabled or inactive state of the control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		private SliderState OnDisabledState
		{
			get
			{
				return _onDisable;
			}
		}

		[Browsable(true)]
		[Category("x Properties")]
		[Editor(typeof(HSliderStatesColorEditor), typeof(UITypeEditor))]
		[Description("Represents the mouse hover state of the control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		private SliderState OnHoverState
		{
			get
			{
				return _onHover;
			}
		}

		[Browsable(true)]
		[Category("x Properties")]
		[Editor(typeof(HSliderStatesColorEditor), typeof(UITypeEditor))]
		[Description("Represents the mouse pressed or click state of the control.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		private SliderState OnPressedState
		{
			get
			{
				return _onPress;
			}
		}

		[Browsable(false)]
		public virtual SliderState OnDisableSlider
		{
			get
			{
				OnDisable.ThumbColor = _onDisable.ThumbColor;
				OnDisable.ScrollBarColor = OnDisable.ScrollBarColor;
				OnDisable.ScrollBarBorderColor = OnDisable.ScrollBarBorderColor;
				return _onDisable;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int BorderRadius
		{
			get
			{
				return base.BorderRadius;
			}
			set
			{
				base.BorderRadius = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int BorderThickness
		{
			get
			{
				return base.BorderThickness;
			}
			set
			{
				base.BorderThickness = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int ThumbMargin
		{
			get
			{
				return base.ThumbMargin;
			}
			set
			{
				base.ThumbMargin = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override int ShrinkSizeLimit
		{
			get
			{
				return base.ShrinkSizeLimit;
			}
			set
			{
				base.ShrinkSizeLimit = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override int ThumbLength
		{
			get
			{
				return base.ThumbLength;
			}
			set
			{
				base.ThumbLength = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseHoverEffects
		{
			get
			{
				return base.AllowMouseHoverEffects;
			}
			set
			{
				base.AllowMouseHoverEffects = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseDownEffects
		{
			get
			{
				return base.AllowMouseDownEffects;
			}
			set
			{
				base.AllowMouseDownEffects = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowShrinkingOnFocusLost
		{
			get
			{
				return base.AllowShrinkingOnFocusLost;
			}
			set
			{
				base.AllowShrinkingOnFocusLost = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override Color ScrollBarBorderColor
		{
			get
			{
				return base.ScrollBarBorderColor;
			}
			set
			{
				base.ScrollBarBorderColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Color ScrollBarColor
		{
			get
			{
				return base.ScrollBarColor;
			}
			set
			{
				base.ScrollBarColor = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override SliderThumbStyles SliderThumbStyle
		{
			get
			{
				return base.SliderThumbStyle;
			}
			set
			{
				base.SliderThumbStyle = value;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override ScrollBarState OnDisable
		{
			get
			{
				return base.OnDisable;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override Control BindingContainer
		{
			get
			{
				return base.BindingContainer;
			}
			set
			{
				base.BindingContainer = value;
			}
		}

		public xHSlider()
		{
			InitializeComponent();
			SetDefaults();
		}

		public void ResetAnimations()
		{
			try
			{
				_tempState.Name = string.Empty;
			}
			catch (Exception)
			{
			}
		}

		private void SetDefaults()
		{
			MinimumSize = new Size(0, 31);
			OnDisableSlider.ThumbColor = Color.Silver;
			OnDisableSlider.ElapsedColor = Color.Silver;
			OnDisableSlider.SliderColor = Color.Gainsboro;
			OnDisabledState.SliderColor = Color.Gainsboro;
			OnDisabledState.ElapsedColor = Color.Silver;
			OnDisabledState.ThumbColor = Color.Silver;
			OnDisabledState.ThumbFillColor = Color.Silver;
			OnIdleState.SliderColor = Color.FromArgb(210, 232, 255);
			OnIdleState.ElapsedColor = Color.DodgerBlue;
			OnIdleState.ThumbColor = Color.DodgerBlue;
			OnIdleState.ThumbFillColor = BackColor;
			OnHoverState.SliderColor = Color.FromArgb(210, 232, 255);
			OnHoverState.ElapsedColor = Color.FromArgb(105, 181, 255);
			OnHoverState.ThumbColor = Color.FromArgb(105, 181, 255);
			OnHoverState.ThumbFillColor = BackColor;
			OnPressedState.SliderColor = Color.FromArgb(210, 232, 255);
			OnPressedState.ElapsedColor = Color.DodgerBlue;
			OnPressedState.ThumbColor = Color.DodgerBlue;
			OnPressedState.ThumbFillColor = Color.DodgerBlue;
			Value = 50;
			AnimationSpeed = 220;
			Cursor = Cursors.Hand;
			ThumbBorderThickness = 2;
			ThumbFillColor = BackColor;
			ThumbColor = Color.DodgerBlue;
			BackColor = Color.Transparent;
			AllowMouseDownEffects = false;
			AllowMouseHoverEffects = false;
			ElapsedColor = Color.DodgerBlue;
			SliderStyle = SliderStyles.Thin;
			ThumbStyle = ThumbStyles.Outline;
			FocusState = SliderStates.Pressed;
			ThumbDrawMode = DrawModes.Outline;
			VisualStyle = ScrollBarVisualStyles.Slider;
			SliderColor = Color.FromArgb(210, 232, 255);
			SliderThumbStyle = SliderThumbStyles.Circular;
			base.Size = new Size(200, 31);
		}

		private void Animate(SliderState sliderState)
		{
			try
			{
				if (sliderState.Name == "OnPress")
				{
					SliderColor = sliderState.SliderColor;
					ElapsedColor = sliderState.ElapsedColor;
					ThumbColor = sliderState.ThumbColor;
					ThumbFillColor = sliderState.ThumbFillColor;
				}
				else
				{
					Transition.run(this, "SliderColor", sliderState.SliderColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
					Transition.run(this, "ElapsedColor", sliderState.ElapsedColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
					Transition.run(this, "ThumbColor", sliderState.ThumbColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
					Transition.run(this, "ThumbFillColor", sliderState.ThumbFillColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
				}
			}
			catch (Exception)
			{
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			SolidBrush solidBrush = new SolidBrush(ElapsedColor);
			e.Graphics.FillRectangle(solidBrush, new Rectangle(base.ScrollBarRectangle.X, base.ScrollBarRectangle.Y, _thumb.Left, base.ScrollBarRectangle.Height + 1));
			solidBrush.Dispose();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (_allowMouseEffects)
			{
				if (_tempState.Name == string.Empty)
				{
					_tempState.Name = "TempState";
					_tempState.SliderColor = SliderColor;
					_tempState.ElapsedColor = ElapsedColor;
					_tempState.ThumbColor = ThumbColor;
					_tempState.ThumbFillColor = ThumbFillColor;
				}
				Animate(OnHoverState);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (!_allowMouseEffects)
				return;
			if (!Focused)
				Animate(OnIdleState);
			if (!Focused || !IndicateFocus)
				return;
			if (FocusState != 0)
			{
				if (FocusState == SliderStates.Hover)
					Animate(OnHoverState);
				else if (FocusState == SliderStates.Pressed)
				{
					Animate(OnPressedState);
				}
			}
			else
				Animate(OnIdleState);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (_allowMouseEffects)
				Animate(_tempState);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_allowMouseEffects)
				Animate(OnPressedState);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
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
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		}
	}
}
