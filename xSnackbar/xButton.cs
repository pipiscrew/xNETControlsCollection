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
using xCollection;

namespace Utilities.xSnackbar
{
	[DefaultEvent("Click")]
	[ToolboxBitmap(typeof(Button))]
	[DefaultProperty("Text")]
	[Description("Provides a highly customizable button with improved styling options and additional feature improvements.")]
	[Designer(typeof(xDesigner))]
	[DebuggerStepThrough]
	internal class xButton : UserControl, IButtonControl
	{
		public enum BorderStyles
		{
			Solid,
			Dash,
			Dot
		}

		public enum ButtonStates
		{
			[Description("Denotes the Button's idle state.")]
			Idle,
			[Description("Denotes the Button's mouse-hover state.")]
			Hover,
			[Description("Denotes the Button's mouse-press or click state.")]
			Pressed,
			[Description("Denotes the Button's disabled state.")]
			Disabled
		}

		public enum state
		{
			idle,
			hover,
			pressed,
			disabled
		}

		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class StateProperties : ExpandableObjectConverter
		{
			[Description("Sets the control's border radius.")]
			public int BorderRadius { get; set; }

			[Description("Sets the control's border thickness.")]
			public int BorderThickness { get; set; }

			[Description("Sets the control's foreground color.")]
			public Color ForeColor { get; set; }

			[Description("Sets the control's background/fill color.")]
			public Color FillColor { get; set; }

			[Description("Sets the control's border color.")]
			public Color BorderColor { get; set; }

			[Description("Sets the control's right icon-image.")]
			public Image IconRightImage { get; set; }

			[Description("Sets the control's left icon-image.")]
			public Image IconLeftImage { get; set; }

			[Description("Sets the control's border style.")]
            private BorderStyles borderStyle = BorderStyles.Solid;

            public BorderStyles BorderStyle
            {
                get { return borderStyle; }
                set { borderStyle = value; }
            }


			public override string ToString()
			{
				return string.Format("BorderColor: {0}, FillColor: {1}, ", BorderColor, FillColor) + string.Format("ForeColor: {0}, Border Radius: {1}, ", ForeColor, BorderRadius) + string.Format("Border Thickness: {0}", BorderThickness);
			}
		}

		[DebuggerStepThrough]
		[Description("Includes the list of available border edges or dimensions that can be customized or excluded when styling controls.")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class BorderEdges
		{
			private bool _all = true;

			private bool _topLeft = true;

			private bool _topRight = true;

			private bool _bottomLeft = true;

			private bool _bottomRight = true;

			[RefreshProperties(RefreshProperties.All)]
			[Description("Sets a value indicating whether the top-left edge will be included.")]
			public bool TopLeft
			{
				get
				{
					return _topLeft;
				}
				set
				{
					_topLeft = value;
					if (!value)
						_all = false;
				}
			}

			[Description("Sets a value indicating whether the top-right edge will be included.")]
			[RefreshProperties(RefreshProperties.All)]
			public bool TopRight
			{
				get
				{
					return _topRight;
				}
				set
				{
					_topRight = value;
					if (!value)
						_all = false;
				}
			}

			[Description("Sets a value indicating whether the bottom-left edge will be included.")]
			[RefreshProperties(RefreshProperties.All)]
			public bool BottomLeft
			{
				get
				{
					return _bottomLeft;
				}
				set
				{
					_bottomLeft = value;
					if (!value)
						_all = false;
				}
			}

			[Description("Sets a value indicating whether the bottom-right edge will be included.")]
			[RefreshProperties(RefreshProperties.All)]
			public bool BottomRight
			{
				get
				{
					return _bottomRight;
				}
				set
				{
					_bottomRight = value;
					if (!value)
						_all = false;
				}
			}

			public BorderEdges()
			{
				_all = true;
			}

			public override string ToString()
			{
				return string.Format("Top-left: {0}; Top-right: {1}; ", TopLeft, TopRight) + string.Format("Bottom-left: {0}; Bottom-right: {1}", BottomLeft, BottomRight);
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
					if (((xButton)base.Control).AutoSize)
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
							new xButtonActionList(base.Component)
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
		public class xButtonActionList : DesignerActionList
		{
			private xButton xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public bool AutoSize
			{
				get
				{
					return xControl.AutoSize;
				}
				set
				{
					SetValue(xControl, "AutoSize", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public bool AllowToggling
			{
				get
				{
					return xControl.AllowToggling;
				}
				set
				{
					SetValue(xControl, "AllowToggling", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public bool IndicateFocus
			{
				get
				{
					return xControl.IndicateFocus;
				}
				set
				{
					SetValue(xControl, "IndicateFocus", value);
				}
			}

			public int TextMarginLeft
			{
				get
				{
					return xControl.TextMarginLeft;
				}
				set
				{
					SetValue(xControl, "TextMarginLeft", value);
				}
			}

			public int BorderRadius
			{
				get
				{
					return xControl.IdleBorderRadius;
				}
				set
				{
					SetValue(xControl, "IdleBorderRadius", value);
				}
			}

			public int BorderThickness
			{
				get
				{
					return xControl.IdleBorderThickness;
				}
				set
				{
					SetValue(xControl, "IdleBorderThickness", value);
				}
			}

			[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
			public string Text
			{
				get
				{
					return xControl.Text;
				}
				set
				{
					SetValue(xControl, "Text", value);
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

			public Color FillColor
			{
				get
				{
					return xControl.IdleFillColor;
				}
				set
				{
					SetValue(xControl, "IdleFillColor", value);
				}
			}

			public Color BorderColor
			{
				get
				{
					return xControl.IdleBorderColor;
				}
				set
				{
					SetValue(xControl, "IdleBorderColor", value);
				}
			}

			public BorderStyles BorderStyle
			{
				get
				{
					return xControl.BorderStyle;
				}
				set
				{
					SetValue(xControl, "BorderStyle", value);
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

			public Image LeftIcon
			{
				get
				{
					return xControl.IdleIconLeftImage;
				}
				set
				{
					SetValue(xControl, "IdleIconLeftImage", value);
				}
			}

			public Image RightIcon
			{
				get
				{
					return xControl.IdleIconRightImage;
				}
				set
				{
					SetValue(xControl, "IdleIconRightImage", value);
				}
			}

			public xButtonActionList(IComponent component)
				: base(component)
			{
				xControl = component as xButton;
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
					new DesignerActionPropertyItem("AutoSize", "AutoSize", "Behaviour", GetPropertyDescriptor(base.Component, "AutoSize").Description),
					new DesignerActionPropertyItem("AllowToggling", "AllowToggling", "Behaviour", GetPropertyDescriptor(base.Component, "AllowToggling").Description),
					new DesignerActionPropertyItem("IndicateFocus", "IndicateFocus", "Behaviour", GetPropertyDescriptor(base.Component, "IndicateFocus").Description),
					new DesignerActionPropertyItem("TextMarginLeft", "TextMarginLeft:", "Appearance", GetPropertyDescriptor(base.Component, "TextMarginLeft").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Appearance", GetPropertyDescriptor(base.Component, "IdleBorderRadius").Description),
					new DesignerActionPropertyItem("BorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "IdleBorderThickness").Description),
					new DesignerActionPropertyItem("ForeColor", "ForeColor:", "Appearance", GetPropertyDescriptor(base.Component, "ForeColor").Description),
					new DesignerActionPropertyItem("BorderColor", "BorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "IdleBorderColor").Description),
					new DesignerActionPropertyItem("FillColor", "FillColor:", "Appearance", GetPropertyDescriptor(base.Component, "IdleFillColor").Description),
					new DesignerActionPropertyItem("BorderStyle", "BorderStyle:", "Appearance", GetPropertyDescriptor(base.Component, "BorderStyle").Description),
					new DesignerActionPropertyItem("Font", "Font:", "Common Tasks", GetPropertyDescriptor(base.Component, "Font").Description),
					new DesignerActionPropertyItem("LeftIcon", "Left Icon:", "Appearance", GetPropertyDescriptor(base.Component, "IdleIconLeftImage").Description),
					new DesignerActionPropertyItem("RightIcon", "Right Icon:", "Appearance", GetPropertyDescriptor(base.Component, "IdleIconRightImage").Description),
					new DesignerActionPropertyItem("Text", "Text:", "Common Tasks", GetPropertyDescriptor(base.Component, "Text").Description)
				};
			}
		}

		private bool _autoSize;

		private bool _pressed = false;

		private bool _toggled = false;

		private bool _autoGenerateColors;

		private bool _allowToggling = false;

		private bool _useDefaultRadiusAndThickness;

		private int _animationSpeed;

		private int _iconPadding = 10;

		private int _borderRadius = 1;

		private int _textMarginLeft = 0;

		private int _borderThickness = 0;

		private int _iconMarginLeft = 11;

		private int _colorContrastOnHover;

		private int _colorContrastOnClick;

		private string _text = "x Button";

		private Pen _pen;

		private Color _backColor = Color.FromArgb(51, 122, 183);

		private Color _idleBorderColor = Color.FromArgb(51, 122, 183);

		private Color _disabledForecolor = Color.FromArgb(168, 160, 168);

		private Color _disabledFillColor = Color.FromArgb(204, 204, 204);

		private Color _disabledBorderColor = Color.FromArgb(191, 191, 191);

		private Image _leftIcon;

		private Image _rightIcon;

		private ButtonStates _focusState;

		private BorderStyles _borderStyle;

		private state _currentState = state.idle;

		private BorderEdges _customizableEdges = new BorderEdges();

		private StateProperties _onIdleState = new StateProperties();

		private StateProperties _onHoverState = new StateProperties();

		private StateProperties _tmpIdleState = new StateProperties();

		private StateProperties _onPressedState = new StateProperties();

		private StateProperties _onDisabledState = new StateProperties();

		private IContainer components = null;

		private PictureBox pbLeftIcon;

		private PictureBox pbRightIcon;

		private Timer focusMonitor;

		private Label Label;

		[Category("x Properties")]
		[Description("Sets a value indicating whether the button will be automatically toggled to receive or release focus when clicked.")]
		public virtual bool AllowToggling
		{
			get
			{
				return _allowToggling;
			}
			set
			{
				_allowToggling = value;
				if (value)
					IndicateFocus = true;
				Refresh();
			}
		}

		[Description("When set to true, the button's 'IdleFillColor' and 'IdleBorderColor' will be used to generate colors for the various states supported.")]
		[Category("x Properties")]
		public virtual bool AutoGenerateColors
		{
			get
			{
				return _autoGenerateColors;
			}
			set
			{
				_autoGenerateColors = value;
				if (value && base.DesignMode)
					GenerateColors(true);
			}
		}

		[Category("x Properties")]
		[Description("Specifies whether the Button will automatically size it's with to fit its contents. Use 'TextMarginLeft' property to set the Text's padding (edges' distance).")]
		public new bool AutoSize
		{
			get
			{
				return _autoSize;
			}
			set
			{
				_autoSize = value;
				Refresh();
			}
		}

		[Description("Sets a value indicating whether the Button will provide a visual cue when focused.")]
		[Category("x Properties")]
		public virtual bool IndicateFocus { get; set; }

		[Description("Sets the Button's animation speed (in milliseconds) when moving from one state to another.")]
		[Category("x Properties")]
		public virtual int AnimationSpeed
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
		[Description("Sets the Button's padding for both the left and the right icon.")]
		public virtual int IconPadding
		{
			get
			{
				return _iconPadding;
			}
			set
			{
				_iconPadding = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's left icon margin.")]
		public virtual int IconMarginLeft
		{
			get
			{
				return _iconMarginLeft;
			}
			set
			{
				_iconMarginLeft = value;
				Refresh();
			}
		}

		[Description("Sets the Button's left text margin.")]
		[Category("x Properties")]
		public virtual int TextMarginLeft
		{
			get
			{
				return _textMarginLeft;
			}
			set
			{
				_textMarginLeft = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's border radius when idle.")]
		public virtual int IdleBorderRadius
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
					_onIdleState.BorderRadius = value;
					if (_useDefaultRadiusAndThickness)
					{
						onHoverState.BorderRadius = value;
						OnPressedState.BorderRadius = value;
						_onDisabledState.BorderRadius = value;
					}
					Refresh();
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's border thickness.")]
		public virtual int IdleBorderThickness
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
					_onIdleState.BorderThickness = value;
					if (_useDefaultRadiusAndThickness)
					{
						onHoverState.BorderThickness = value;
						OnPressedState.BorderThickness = value;
						_onDisabledState.BorderThickness = value;
					}
					Refresh();
				}
			}
		}

		[Description("Sets how dark or light the button's color will be whenever a mouse-hover event has occurred.")]
		[Category("x Properties")]
		public virtual int ColorContrastOnHover
		{
			get
			{
				return _colorContrastOnHover;
			}
			set
			{
				_colorContrastOnHover = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets how dark or light the button's color will be whenever a mouse-click event has occurred.")]
		public virtual int ColorContrastOnClick
		{
			get
			{
				return _colorContrastOnClick;
			}
			set
			{
				_colorContrastOnClick = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's right icon cursor.")]
		public virtual Cursor IconRightCursor
		{
			get
			{
				return pbRightIcon.Cursor;
			}
			set
			{
				pbRightIcon.Cursor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's left icon cursor.")]
		public virtual Cursor IconLeftCursor
		{
			get
			{
				return pbLeftIcon.Cursor;
			}
			set
			{
				pbLeftIcon.Cursor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's left icon when idle.")]
		public virtual Image IdleIconLeftImage
		{
			get
			{
				return _leftIcon;
			}
			set
			{
				_leftIcon = value;
				_onIdleState.IconLeftImage = value;
				pbLeftIcon.Image = _leftIcon;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's right icon when idle.")]
		public virtual Image IdleIconRightImage
		{
			get
			{
				return _rightIcon;
			}
			set
			{
				_rightIcon = value;
				_onIdleState.IconRightImage = value;
				pbRightIcon.Image = _rightIcon;
				Refresh();
			}
		}

		[Description("Sets the Button's default font.")]
		[Category("x Properties")]
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

		[Category("x Properties")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[Browsable(true)]
		[Description("Sets the text associated with this control.")]
		public override string Text
		{
			get
			{
				return _text;
			}
			set
			{
				base.Text = value;
				ButtonText = value;
			}
		}

		[Description("Sets the Button's border color when disabled.")]
		[Category("x Properties")]
		public virtual Color DisabledBorderColor
		{
			get
			{
				return _disabledBorderColor;
			}
			set
			{
				_disabledBorderColor = value;
				_onDisabledState.BorderColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's background color when disabled.")]
		public virtual Color DisabledFillColor
		{
			get
			{
				return _disabledFillColor;
			}
			set
			{
				_disabledFillColor = value;
				_onDisabledState.FillColor = value;
				Refresh();
			}
		}

		[Description("Sets the Button's foreground color when disabled.")]
		[Category("x Properties")]
		public virtual Color DisabledForecolor
		{
			get
			{
				return _disabledForecolor;
			}
			set
			{
				_disabledForecolor = value;
				_onDisabledState.ForeColor = value;
				Refresh();
			}
		}

		[Description("Sets the Button's border color when idle/inactive.")]
		[Category("x Properties")]
		public virtual Color IdleBorderColor
		{
			get
			{
				return _idleBorderColor;
			}
			set
			{
				_idleBorderColor = value;
				_onIdleState.BorderColor = value;
				if (AutoGenerateColors && base.DesignMode)
					GenerateColors(true);
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's background/fill color when idle.")]
		public virtual Color IdleFillColor
		{
			get
			{
				return BackColor1;
			}
			set
			{
				BackColor1 = value;
				_onIdleState.FillColor = value;
				if (AutoGenerateColors && base.DesignMode)
					GenerateColors(true);
				Refresh();
			}
		}

		[Description("Sets the Button's foreground color.")]
		[Category("x Properties")]
		[RefreshProperties(RefreshProperties.All)]
		public override Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
				_onIdleState.ForeColor = value;
			}
		}

		[Description("Sets the Button's idle state design.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("x Properties")]
		public virtual StateProperties OnIdleState
		{
			get
			{
				ForeColor = _onIdleState.ForeColor;
				BorderStyle = _onIdleState.BorderStyle;
				IdleFillColor = _onIdleState.FillColor;
				IdleBorderColor = _onIdleState.BorderColor;
				IdleBorderRadius = _onIdleState.BorderRadius;
				IdleIconLeftImage = _onIdleState.IconLeftImage;
				IdleIconRightImage = _onIdleState.IconRightImage;
				IdleBorderThickness = _onIdleState.BorderThickness;
				if (_useDefaultRadiusAndThickness)
				{
					onHoverState.BorderRadius = _onIdleState.BorderRadius;
					OnPressedState.BorderRadius = _onIdleState.BorderRadius;
					_onDisabledState.BorderRadius = _onIdleState.BorderRadius;
					onHoverState.BorderThickness = _onIdleState.BorderThickness;
					OnPressedState.BorderThickness = _onIdleState.BorderThickness;
					_onDisabledState.BorderThickness = _onIdleState.BorderThickness;
				}
				return _onIdleState;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Sets the Button's hover state design.")]
		[DisplayName("OnHoverState")]
		[Category("x Properties")]
		public virtual StateProperties onHoverState
		{
			get
			{
				return _onHoverState;
			}
			set
			{
				_onHoverState = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Sets the Button's pressed state design.")]
		[Category("x Properties")]
		public virtual StateProperties OnPressedState
		{
			get
			{
				return _onPressedState;
			}
			set
			{
				_onPressedState = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the Button's border style.")]
		public new virtual BorderStyles BorderStyle
		{
			get
			{
				return _borderStyle;
			}
			set
			{
				_borderStyle = value;
				_onIdleState.BorderStyle = value;
				Refresh();
			}
		}

		[Description("Sets the state to use when the Button contains focus while the cursor is away.")]
		[Category("x Properties")]
		public virtual ButtonStates FocusState
		{
			get
			{
				return _focusState;
			}
			set
			{
				_focusState = value;
				if (!base.DesignMode)
					Refresh();
			}
		}

		[Description("Sets the 'DialogResult' returned by the Button.")]
		[Category("x Properties")]
		public DialogResult DialogResult { get; set; }

		[Description("Sets the list of border edges that will be customized whenever the border radius is applied.")]
		[Category("x Properties")]
		public virtual BorderEdges CustomizableEdges
		{
			get
			{
				Refresh();
				return _customizableEdges;
			}
			set
			{
				_customizableEdges = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public PictureBox LeftIcon
		{
			get
			{
				return pbLeftIcon;
			}
		}

		[Browsable(false)]
		public PictureBox RightIcon
		{
			get
			{
				return pbRightIcon;
			}
		}

		[Browsable(false)]
		public bool UseDefaultRadiusAndThickness
		{
			get
			{
				return _useDefaultRadiusAndThickness;
			}
			set
			{
				_useDefaultRadiusAndThickness = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public bool IsDefault { get; private set; }

		[Browsable(false)]
		public bool Toggled
		{
			get
			{
				return _toggled;
			}
		}

		[Browsable(false)]
		public virtual StateProperties OnDisabledState
		{
			get
			{
				_disabledForecolor = _onDisabledState.ForeColor;
				_disabledFillColor = _onDisabledState.FillColor;
				_disabledBorderColor = _onDisabledState.BorderColor;
				return _onDisabledState;
			}
		}

		[Browsable(false)]
		public string ButtonText
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				Label.Text = _text;
				Refresh();
			}
		}

		[Browsable(false)]
		public int ButtonTextMarginLeft
		{
			get
			{
				return _textMarginLeft;
			}
			set
			{
				_textMarginLeft = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public int PreferredHeight
		{
			get
			{
				return Label.PreferredHeight * 2;
			}
		}

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

		[Browsable(false)]
		public Color BackColor1
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
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
		public virtual ContentAlignment TextAlign
		{
			get
			{
				return Label.TextAlign;
			}
			set
			{
				Label.TextAlign = value;
				Refresh();
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 33554432;
				return createParams;
			}
		}

		[Description("Occurs when the Button's left icon is clicked.")]
		[Category("x Events")]
		public event EventHandler IconLeftClick = null;

		[Description("Occurs when the Button's right icon is clicked.")]
		[Category("x Events")]
		public event EventHandler IconRightClick = null;

		public xButton()
		{
			InitializeComponent();
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			Cursor = Cursors.Default;
			DoubleBuffered = true;
			AnimationSpeed = 200;
			IdleBorderRadius = 1;
			IndicateFocus = false;
			IdleBorderThickness = 1;
			ForeColor = Color.White;
			ColorContrastOnHover = 45;
			ColorContrastOnClick = 45;
			AutoGenerateColors = false;
			BorderStyle = BorderStyles.Solid;
			IdleFillColor = Color.DodgerBlue;
			FocusState = ButtonStates.Pressed;
			IdleBorderColor = Color.DodgerBlue;
			UseDefaultRadiusAndThickness = true;
			OnDisabledState.BorderRadius = 1;
			OnDisabledState.BorderThickness = 1;
			OnDisabledState.ForeColor = Color.FromArgb(168, 160, 168);
			OnDisabledState.FillColor = Color.FromArgb(204, 204, 204);
			OnDisabledState.BorderColor = Color.FromArgb(191, 191, 191);
			onHoverState.BorderRadius = 1;
			onHoverState.BorderThickness = 1;
			onHoverState.ForeColor = Color.White;
			onHoverState.FillColor = Color.FromArgb(105, 181, 255);
			onHoverState.BorderColor = Color.FromArgb(105, 181, 255);
			OnPressedState.BorderRadius = 1;
			OnPressedState.BorderThickness = 1;
			OnPressedState.ForeColor = Color.White;
			OnPressedState.FillColor = Color.FromArgb(40, 96, 144);
			OnPressedState.BorderColor = Color.FromArgb(40, 96, 144);
			base.Size = new Size(150, 39);
			Font = new Font("Segoe UI", 9f, FontStyle.Regular);
			Refresh();
		}

		public state GetCurrentState()
		{
			return _currentState;
		}

		public new void Refresh()
		{
			int num = _borderThickness * 2;
			int num3 = (pbLeftIcon.Top = (pbLeftIcon.Left = _iconPadding));
			pbLeftIcon.Left = _iconMarginLeft;
			PictureBox pictureBox = pbLeftIcon;
			PictureBox pictureBox2 = pbLeftIcon;
			PictureBox pictureBox3 = pbRightIcon;
			int num5 = (pbRightIcon.Width = base.Height - _iconPadding * 2);
			int num7 = (pictureBox3.Height = num5);
			num3 = (pictureBox.Height = (pictureBox2.Width = num7));
			Label.Top = base.Height / 2 - Label.Height / 2;
			pbRightIcon.Top = _iconPadding;
			pbRightIcon.Left = base.Width - pbRightIcon.Width - _iconPadding;
			Label.ForeColor = ForeColor;
			Color idleFillColor = IdleFillColor;
			int idleBorderRadius = IdleBorderRadius;
			int idleBorderThickness = IdleBorderThickness;
			if (base.Enabled)
			{
				if (_pressed)
				{
					idleBorderRadius = OnPressedState.BorderRadius;
					idleBorderThickness = OnPressedState.BorderThickness;
					if (OnPressedState.IconRightImage != null)
						pbRightIcon.Image = OnPressedState.IconRightImage;
					if (OnPressedState.IconLeftImage != null)
						pbLeftIcon.Image = OnPressedState.IconLeftImage;
				}
				else if (GetCurrentState() == state.idle)
				{
					idleBorderRadius = IdleBorderRadius;
					idleBorderThickness = IdleBorderThickness;
					if (IdleIconLeftImage != null)
						pbLeftIcon.Image = IdleIconLeftImage;
					if (IdleIconRightImage != null)
						pbRightIcon.Image = IdleIconRightImage;
				}
				else
				{
					idleBorderRadius = onHoverState.BorderRadius;
					idleBorderThickness = onHoverState.BorderThickness;
					if (onHoverState.IconRightImage != null)
						pbRightIcon.Image = onHoverState.IconRightImage;
					if (onHoverState.IconLeftImage != null)
						pbLeftIcon.Image = onHoverState.IconLeftImage;
				}
			}
			else
			{
				idleBorderRadius = IdleBorderRadius;
				idleBorderThickness = IdleBorderThickness;
				if (OnDisabledState.IconLeftImage != null)
					pbLeftIcon.Image = OnDisabledState.IconLeftImage;
				if (OnDisabledState.IconRightImage != null)
					pbRightIcon.Image = OnDisabledState.IconRightImage;
			}
			int num10 = (base.Width - Label.Width) / 2;
			if (pbLeftIcon.Image != null)
			{
				Label.Left = num10 + ButtonTextMarginLeft;
				pbLeftIcon.Visible = true;
			}
			else
			{
				Label.Left = num10 + ButtonTextMarginLeft;
				pbLeftIcon.Visible = false;
			}
			if (pbRightIcon.Image == null)
			{
				Label.Width = num10 + base.Width - Label.Left;
				pbRightIcon.Visible = false;
			}
			else
			{
				Label.Width = pbRightIcon.Left - Label.Left - _iconPadding;
				pbRightIcon.Visible = true;
			}
			if (AutoSize)
			{
				if (!pbLeftIcon.Visible && !pbRightIcon.Visible)
				{
					Label.Left = _textMarginLeft;
					base.Width = Label.Width + Label.Left * 2;
					base.Height = Label.Height + Label.Top * 2;
				}
				else if (pbLeftIcon.Visible && !pbRightIcon.Visible)
				{
					Label.Left = pbLeftIcon.Right + _iconPadding + _textMarginLeft;
					base.Width = Label.Right + pbLeftIcon.Left;
					base.Height = Label.Height + Label.Top * 2;
				}
				else if (pbLeftIcon.Visible && pbRightIcon.Visible)
				{
					Label.Left = pbLeftIcon.Right + _iconPadding + _textMarginLeft - num;
					base.Width = Label.Left + Label.Width + (Label.Left - pbLeftIcon.Right) + pbRightIcon.Width + _iconPadding;
					base.Height = Label.Height + Label.Top * 2;
				}
				else if (!pbLeftIcon.Visible && pbRightIcon.Visible)
				{
					Label.Left = _textMarginLeft;
					base.Width = Label.Right + pbLeftIcon.Left + pbLeftIcon.Width;
					base.Height = Label.Height + Label.Top * 2;
				}
			}
			Bitmap bitmap = new Bitmap(base.Width, base.Height);
			_pen = new Pen(IdleBorderColor, IdleBorderThickness);
			if (_borderStyle == BorderStyles.Dash)
				_pen.DashStyle = DashStyle.Dash;
			else if (_borderStyle == BorderStyles.Dot)
			{
				_pen.DashStyle = DashStyle.Dot;
			}
			else if (_borderStyle == BorderStyles.Solid)
			{
				_pen.DashStyle = DashStyle.Solid;
			}
			else
			{
				_pen.DashStyle = DashStyle.Solid;
			}
			DrawRoundedRectangle(Bounds: new Rectangle(idleBorderThickness, idleBorderThickness, base.Width - idleBorderThickness * 2 - 1, base.Height - idleBorderThickness * 2 - 1), gfx: Graphics.FromImage(bitmap), CornerRadius: idleBorderRadius, DrawPen: _pen, FillColor: idleFillColor);
			BackgroundImage = bitmap;
		}

		public void ApplyState(StateProperties state)
		{
			Animate(state);
		}

		public void PerformClick()
		{
			if (base.CanSelect)
				OnClick(EventArgs.Empty);
		}

		public void Reset()
		{
			_tmpIdleState.FillColor = Color.Empty;
			_tmpIdleState.ForeColor = Color.Empty;
			_tmpIdleState.BorderColor = Color.Empty;
			_tmpIdleState.BorderStyle = BorderStyles.Solid;
		}

		public void Reset(StateProperties state)
		{
			if (state != null)
			{
				_tmpIdleState.FillColor = Color.Empty;
				_tmpIdleState.ForeColor = Color.Empty;
				_tmpIdleState.BorderColor = Color.Empty;
				_tmpIdleState.BorderStyle = BorderStyles.Solid;
				ApplyState(state);
			}
			else
			{
				_tmpIdleState.FillColor = Color.Empty;
				_tmpIdleState.ForeColor = Color.Empty;
				_tmpIdleState.BorderColor = Color.Empty;
				_tmpIdleState.BorderStyle = BorderStyles.Solid;
			}
		}

		public void ResetColors()
		{
			Reset();
		}

		public void NotifyDefault(bool value)
		{
			SaveCurrentState();
			if (IsDefault != value)
				IsDefault = value;
			if (value)
				ApplyFocusState();
			else
				Animate(OnIdleState);
		}

		public void GenerateColors(bool invalidate = false)
		{
			onHoverState.BorderColor = _onIdleState.BorderColor.LightenBy(_colorContrastOnHover);
			OnPressedState.BorderColor = _onIdleState.BorderColor.DarkenBy(_colorContrastOnClick);
			if (_onIdleState.FillColor.IsEmpty || _onIdleState.FillColor == Color.Transparent)
			{
				onHoverState.FillColor = _onIdleState.BorderColor.LightenBy(_colorContrastOnHover);
				OnPressedState.FillColor = _onIdleState.BorderColor.DarkenBy(_colorContrastOnClick);
			}
			else
			{
				onHoverState.FillColor = _onIdleState.FillColor.LightenBy(_colorContrastOnHover);
				OnPressedState.FillColor = _onIdleState.FillColor.DarkenBy(_colorContrastOnClick);
			}
			Reset();
			SaveCurrentState();
			if (invalidate)
				Refresh();
		}

		private void DrawRoundedRectangle(Graphics gfx, Rectangle Bounds, int CornerRadius, Pen DrawPen, Color FillColor)
		{
			gfx.Clear(Color.Transparent);
			gfx.SmoothingMode = SmoothingMode.AntiAlias;
			GraphicsPath graphicsPath = new GraphicsPath();
			DrawPen.StartCap = LineCap.Round;
			DrawPen.EndCap = LineCap.Round;
			if (_borderRadius > 1)
			{
				if (_customizableEdges.TopLeft && _customizableEdges.TopRight && _customizableEdges.BottomLeft && _customizableEdges.BottomRight)
				{
					graphicsPath.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180f, 90f);
					graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270f, 90f);
					graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0f, 90f);
					graphicsPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90f, 90f);
				}
				else
				{
					if (_customizableEdges.TopLeft)
						graphicsPath.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180f, 90f);
					else
						graphicsPath.AddLine(Bounds.X, Bounds.Y, Bounds.X, Bounds.Y);
					if (_customizableEdges.TopRight)
						graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270f, 90f);
					else
						graphicsPath.AddLine(Bounds.X + Bounds.Width, Bounds.Y, Bounds.X + Bounds.Width, Bounds.Y);
					if (_customizableEdges.BottomRight)
						graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0f, 90f);
					else
						graphicsPath.AddLine(Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height, Bounds.X + Bounds.Width, Bounds.Y + Bounds.Height);
					if (_customizableEdges.BottomLeft)
						graphicsPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90f, 90f);
					else
						graphicsPath.AddLine(Bounds.X, Bounds.Y + Bounds.Height, Bounds.X, Bounds.Y + Bounds.Height);
				}
			}
			else
			{
				DrawPen.StartCap = LineCap.Square;
				DrawPen.EndCap = LineCap.Square;
				graphicsPath.AddRectangle(new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height));
			}
			graphicsPath.CloseAllFigures();
			gfx.FillPath(new SolidBrush(FillColor), graphicsPath);
			gfx.DrawPath(DrawPen, graphicsPath);
			DrawPen.Dispose();
			gfx.Dispose();
		}

		private bool IsMouseWithinControl()
		{
			if (base.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
				return true;
			return false;
		}

		private void SaveCurrentState()
		{
			try
			{
				if (_tmpIdleState.BorderColor.IsEmpty)
					_tmpIdleState.BorderColor = IdleBorderColor;
				if (_tmpIdleState.FillColor.IsEmpty)
					_tmpIdleState.FillColor = IdleFillColor;
				if (_tmpIdleState.ForeColor.IsEmpty)
				{
					_tmpIdleState.ForeColor = ForeColor;
					_tmpIdleState.BorderStyle = BorderStyle;
					_tmpIdleState.IconLeftImage = IdleIconLeftImage;
					_tmpIdleState.IconRightImage = IdleIconRightImage;
				}
			}
			catch (Exception)
			{
			}
		}

		private void Animate(StateProperties state)
		{
			try
			{
				BorderStyle = state.BorderStyle;
				if (state.IconLeftImage != null)
					IdleIconLeftImage = state.IconLeftImage;
				if (state.IconRightImage != null)
					IdleIconRightImage = state.IconRightImage;
				if (AnimationSpeed == 1)
				{
					ForeColor = state.ForeColor;
					IdleFillColor = state.FillColor;
					IdleBorderColor = state.BorderColor;
				}
				else
				{
					Transition.run(this, "ForeColor", state.ForeColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
					Transition.run(this, "IdleFillColor", state.FillColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
					Transition.run(this, "IdleBorderColor", state.BorderColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
				}
			}
			catch (Exception)
			{
			}
		}

		private void ApplyFocusState()
		{
			if (FocusState == ButtonStates.Disabled)
				Animate(OnDisabledState);
			else if (FocusState == ButtonStates.Hover)
			{
				Animate(onHoverState);
			}
			else if (FocusState == ButtonStates.Idle)
			{
				Animate(OnIdleState);
			}
			else if (FocusState == ButtonStates.Pressed)
			{
				Animate(OnPressedState);
			}
		}

		private void ReturnDialogResult()
		{
			if (FindForm() != null)
				FindForm().DialogResult = DialogResult;
		}

		private void OnResizeControl(object sender, EventArgs e)
		{
			Refresh();
		}

		private void OnLoad(object sender, EventArgs e)
		{
		}

		private void OnBackColorChanged(object sender, EventArgs e)
		{
		}

		private void OnClickLabel(object sender, EventArgs e)
		{
			if (base.Enabled)
			{
				ReturnDialogResult();
				base.OnClick(e);
			}
		}

		private void OnMouseDownLabel(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		private void OnMouseUpLabel(object sender, MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		private void OnClickLeftIcon(object sender, EventArgs e)
		{
			if (base.Enabled)
			{
				ReturnDialogResult();
				if (this.IconLeftClick != null)
					this.IconLeftClick(this, e);
				else
					base.OnClick(e);
			}
		}

		private void OnClickRightIcon(object sender, EventArgs e)
		{
			if (base.Enabled)
			{
				ReturnDialogResult();
				if (this.IconRightClick != null)
					this.IconRightClick(this, e);
				else
					base.OnClick(e);
			}
		}

		private void OnDoubleClickLeftIcon(object sender, EventArgs e)
		{
			if (base.Enabled)
				base.OnDoubleClick(e);
		}

		private void OnDoubleClickRightIcon(object sender, EventArgs e)
		{
			if (base.Enabled)
				base.OnDoubleClick(e);
		}

		private void OnDoubleClickLabel(object sender, EventArgs e)
		{
			OnDoubleClick(e);
		}

		private void OnMouseClickLabel(object sender, MouseEventArgs e)
		{
			OnMouseClick(e);
		}

		private void OnMouseDoubleClickLabel(object sender, MouseEventArgs e)
		{
			OnMouseDoubleClick(e);
		}

		private void OnChangeFont(object sender, EventArgs e)
		{
			Label.Font = Font;
		}

		private void OnChangeForeColor(object sender, EventArgs e)
		{
			Label.ForeColor = ForeColor;
		}

		private void OnMouseEnterControls(object sender, EventArgs e)
		{
			OnMouseHover(e);
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			_pressed = true;
			_toggled = !_toggled;
			_currentState = state.pressed;
			Focus();
			if (!AllowToggling)
				Animate(OnPressedState);
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			_pressed = false;
			_currentState = state.idle;
			if (AllowToggling)
			{
				Focus();
				Animate(OnPressedState);
			}
			else
				Animate(_tmpIdleState);
		}

		protected override void OnClick(EventArgs e)
		{
			ReturnDialogResult();
			base.OnClick(e);
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			SaveCurrentState();
			base.OnEnabledChanged(e);
			if (!base.Enabled)
			{
				_currentState = state.disabled;
				Animate(OnDisabledState);
			}
			else
			{
				_currentState = state.idle;
				Animate(_tmpIdleState);
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			OnMouseEnter(e);
			try
			{
				if (!AllowToggling)
					return;
				foreach (object control in base.Parent.Controls)
				{
					if (control is xButton)
					{
						xButton xButton = (xButton)control;
                        if (xButton.GetHashCode() != GetHashCode() && xButton.AllowToggling)
						{
							xButton.SaveCurrentState();
                            xButton.ApplyState(xButton._tmpIdleState);
						}
					}
				}
			}
			catch (Exception)
			{
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			OnMouseLeave(e);
		}

		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			SaveCurrentState();
			if (IsMouseWithinControl())
			{
				if (!AllowToggling)
					Animate(onHoverState);
			}
			else if (FocusState != 0)
			{
				if (FocusState == ButtonStates.Hover)
					Animate(onHoverState);
				else if (FocusState == ButtonStates.Pressed)
				{
					Animate(OnPressedState);
				}
			}
			else
			{
				Animate(OnIdleState);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (!IsMouseWithinControl() && !Focused)
			{
				if (!AllowToggling)
				{
					_currentState = state.idle;
					Animate(_tmpIdleState);
				}
			}
			else
			{
				if (IsMouseWithinControl() || !Focused)
					return;
				if (IndicateFocus)
				{
					if (FocusState == ButtonStates.Idle)
					{
						_currentState = state.idle;
						Animate(_tmpIdleState);
					}
					else if (FocusState == ButtonStates.Hover)
					{
						_currentState = state.hover;
						Animate(onHoverState);
					}
					else if (FocusState == ButtonStates.Pressed)
					{
						_currentState = state.pressed;
						Animate(OnPressedState);
					}
				}
				else
				{
					_currentState = state.idle;
					Animate(_tmpIdleState);
				}
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (e.KeyCode == Keys.Return || e.KeyCode == Keys.Space)
				PerformClick();
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
			this.pbLeftIcon = new System.Windows.Forms.PictureBox();
			this.pbRightIcon = new System.Windows.Forms.PictureBox();
			this.focusMonitor = new System.Windows.Forms.Timer(this.components);
			this.Label = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)this.pbLeftIcon).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pbRightIcon).BeginInit();
			base.SuspendLayout();
			this.pbLeftIcon.BackColor = System.Drawing.Color.Transparent;
			this.pbLeftIcon.Location = new System.Drawing.Point(11, 10);
			this.pbLeftIcon.Name = "pbLeftIcon";
			this.pbLeftIcon.Size = new System.Drawing.Size(27, 25);
			this.pbLeftIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbLeftIcon.TabIndex = 1;
			this.pbLeftIcon.TabStop = false;
			this.pbLeftIcon.Visible = false;
			this.pbLeftIcon.Click += new System.EventHandler(OnClickLeftIcon);
			this.pbLeftIcon.DoubleClick += new System.EventHandler(OnDoubleClickLeftIcon);
			this.pbLeftIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			this.pbLeftIcon.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.pbLeftIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
			this.pbRightIcon.BackColor = System.Drawing.Color.Transparent;
			this.pbRightIcon.Location = new System.Drawing.Point(173, 10);
			this.pbRightIcon.Name = "pbRightIcon";
			this.pbRightIcon.Size = new System.Drawing.Size(27, 25);
			this.pbRightIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbRightIcon.TabIndex = 2;
			this.pbRightIcon.TabStop = false;
			this.pbRightIcon.Visible = false;
			this.pbRightIcon.Click += new System.EventHandler(OnClickRightIcon);
			this.pbRightIcon.DoubleClick += new System.EventHandler(OnDoubleClickRightIcon);
			this.pbRightIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			this.pbRightIcon.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.pbRightIcon.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
			this.Label.AutoEllipsis = true;
			this.Label.AutoSize = true;
			this.Label.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
			this.Label.Location = new System.Drawing.Point(59, 14);
			this.Label.Name = "Label";
			this.Label.Size = new System.Drawing.Size(93, 17);
			this.Label.TabIndex = 3;
			this.Label.Text = "x Button";
			this.Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.Label.Click += new System.EventHandler(OnClickLabel);
			this.Label.DoubleClick += new System.EventHandler(OnDoubleClickLabel);
			this.Label.MouseClick += new System.Windows.Forms.MouseEventHandler(OnMouseClickLabel);
			this.Label.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(OnMouseDoubleClickLabel);
			this.Label.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDownLabel);
			this.Label.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.Label.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUpLabel);
			this.BackColor = System.Drawing.Color.Transparent;
			base.Controls.Add(this.pbRightIcon);
			base.Controls.Add(this.pbLeftIcon);
			base.Controls.Add(this.Label);
			this.ForeColor = System.Drawing.Color.White;
			base.Name = "xButton";
			base.Size = new System.Drawing.Size(210, 45);
			base.BackColorChanged += new System.EventHandler(OnBackColorChanged);
			base.FontChanged += new System.EventHandler(OnChangeFont);
			base.ForeColorChanged += new System.EventHandler(OnChangeForeColor);
			base.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
			base.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);
			base.Resize += new System.EventHandler(OnResizeControl);
			((System.ComponentModel.ISupportInitialize)this.pbLeftIcon).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pbRightIcon).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
