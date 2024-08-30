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
using Utilities.xTextBox;

namespace xCollection
{
	[Designer(typeof(xDesigner))]
	[DefaultProperty("Text")]
	[DefaultEvent("TextChanged")]
	[ToolboxBitmap(typeof(TextBox))]
	[DebuggerStepThrough]
	[Category("x UI For Windows Forms")]
	[Description("Provides enhanced TextBox with customized styling properties and features for greater flexibility.")]
	public class xTextBox : UserControl
	{
		public enum _Style
		{
			[Description("Uses the default x TextBox design.")]
			x,
			[Description("Provides a custom Material TextBox design.")]
			Material
		}

		public enum state
		{
			[Description("Represents the idle state.")]
			idle,
			[Description("Represents the hover state.")]
			hover,
			[Description("Represents the active state.")]
			active,
			[Description("Represents the disabled state.")]
			disabled
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class StateProperties : ExpandableObjectConverter
		{
			[Description("Sets the control's foreground color.")]
			public Color ForeColor { get; set; }

			[Description("Sets the control's placeholder foreground color.")]
			public Color PlaceholderForeColor { get; set; }

			[Description("Sets the control's background/fill color.")]
			public Color FillColor { get; set; }

			[Description("Sets the control's border color.")]
			public Color BorderColor { get; set; }

			public override string ToString()
			{
				return string.Format("BorderColor: {0}, FillColor: {1}, ", BorderColor, FillColor) + string.Format("ForeColor: {0}", ForeColor);
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

		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class xControlActionList : DesignerActionList
		{
			private xTextBox xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public bool Multiline
			{
				get
				{
					return xControl.Multiline;
				}
				set
				{
					SetValue(xControl, "Multiline", value);
					designerActionUISvc.Refresh(base.Component);
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

			public int TextMarginTop
			{
				get
				{
					return xControl.TextMarginTop;
				}
				set
				{
					SetValue(xControl, "TextMarginTop", value);
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

			[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
			public string PlaceholderText
			{
				get
				{
					return xControl.PlaceholderText;
				}
				set
				{
					SetValue(xControl, "PlaceholderText", value);
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

			public Color PlaceholderForeColor
			{
				get
				{
					return xControl.PlaceholderForeColor;
				}
				set
				{
					SetValue(xControl, "PlaceholderForeColor", value);
				}
			}

			public Color FillColor
			{
				get
				{
					return xControl.FillColor;
				}
				set
				{
					SetValue(xControl, "FillColor", value);
				}
			}

			public Color BorderColorActive
			{
				get
				{
					return xControl.BorderColorActive;
				}
				set
				{
					SetValue(xControl, "BorderColorActive", value);
				}
			}

			public Color BorderColorHover
			{
				get
				{
					return xControl.BorderColorHover;
				}
				set
				{
					SetValue(xControl, "BorderColorHover", value);
				}
			}

			public Color BorderColorIdle
			{
				get
				{
					return xControl.BorderColorIdle;
				}
				set
				{
					SetValue(xControl, "BorderColorIdle", value);
				}
			}

			public _Style Style
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

			public HorizontalAlignment TextAlign
			{
				get
				{
					return xControl.TextAlign;
				}
				set
				{
					SetValue(xControl, "TextAlign", value);
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

			public xControlActionList(IComponent component)
				: base(component)
			{
				xControl = component as xTextBox;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)                         ", "Common Tasks"),
					new DesignerActionPropertyItem("PlaceholderText", "PlaceholderText:", "Common Tasks", GetPropertyDescriptor(base.Component, "PlaceholderText").Description),
					new DesignerActionPropertyItem("Text", "Text:", "Common Tasks", GetPropertyDescriptor(base.Component, "Text").Description),
					new DesignerActionPropertyItem("Font", "Font:", "Common Tasks", GetPropertyDescriptor(base.Component, "Font").Description),
					new DesignerActionPropertyItem("TextAlign", "TextAlign:", "Common Tasks", GetPropertyDescriptor(base.Component, "TextAlign").Description),
					new DesignerActionPropertyItem("Multiline", "Multiline", "Common Tasks", GetPropertyDescriptor(base.Component, "Multiline").Description),
					new DesignerActionPropertyItem("ForeColor", "ForeColor:", "Appearance", GetPropertyDescriptor(base.Component, "ForeColor").Description),
					new DesignerActionPropertyItem("PlaceholderForeColor", "PlaceholderForeColor:", "Appearance", GetPropertyDescriptor(base.Component, "PlaceholderForeColor").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Appearance", GetPropertyDescriptor(base.Component, "BorderRadius").Description),
					new DesignerActionPropertyItem("BorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "BorderThickness").Description),
					new DesignerActionPropertyItem("BorderColorIdle", "BorderColorIdle:", "Appearance", GetPropertyDescriptor(base.Component, "BorderColorIdle").Description),
					new DesignerActionPropertyItem("BorderColorActive", "BorderColorActive:", "Appearance", GetPropertyDescriptor(base.Component, "BorderColorActive").Description),
					new DesignerActionPropertyItem("BorderColorHover", "BorderColorHover:", "Appearance", GetPropertyDescriptor(base.Component, "BorderColorHover").Description),
					new DesignerActionPropertyItem("FillColor", "FillColor:", "Appearance", GetPropertyDescriptor(base.Component, "FillColor").Description),
					new DesignerActionPropertyItem("Style", "Style:", "Appearance", GetPropertyDescriptor(base.Component, "Style").Description),
					new DesignerActionPropertyItem("TextMarginLeft", "TextMarginLeft:", "Appearance", GetPropertyDescriptor(base.Component, "TextMarginLeft").Description),
					new DesignerActionPropertyItem("TextMarginTop", "TextMarginTop:", "Appearance", GetPropertyDescriptor(base.Component, "TextMarginTop").Description),
					new DesignerActionMethodItem(this, "Refresh", "Refresh Control", "")
				};
			}

			public void Refresh()
			{
				try
				{
					xControl.PlaceholderText = PlaceholderText;
					xControl.Invalidate();
					xControl.Refresh();
				}
				catch (Exception)
				{
				}
			}
		}

		private const int WM_SETFOCUS = 7;

		private const int WM_KILLFOCUS = 8;

		private bool _enabled = true;

		private int _padding = 10;

		private int _textMarginBottom;

		private int _borderRadius = 1;

		private int _textMarginTop = 0;

		private int _textMarginLeft = 3;

		private int _borderThickness = 2;

		private Pen _pen;

		private Color _backColor = Color.White;

		private Color _idleBorderColor = Color.FromArgb(107, 107, 107);

		private Color _hoverBorderColor = Color.FromArgb(105, 181, 255);

		private Color _activeBorderColor = Color.FromArgb(102, 45, 145);

		private Color _disabledBorderColor = Color.FromArgb(161, 161, 161);

		private _Style _style = _Style.x;

		private state _currentState = state.idle;

		private StateProperties _onIdleState = new StateProperties();

		private StateProperties _tmpIdleState = new StateProperties();

		private StateProperties _onHoverState = new StateProperties();

		private StateProperties _onActiveState = new StateProperties();

		private StateProperties _onDisabledState = new StateProperties();

		private IContainer components = null;

		private PictureBox pbLeftIcon;

		private PictureBox pbRightIcon;

		private Timer focusMonitor;

		private PlaceholderTextBox edit;

		[Description("Indicates whether the control can respond to user interaction.")]
		[Category("x Properties")]
		public new virtual bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
				edit.SelectionEnabled = value;
				pbLeftIcon.Enabled = value;
				pbRightIcon.Enabled = value;
				Refresh();
				OnEnabledChanged(this, EventArgs.Empty);
				EventHandler enabledChanged = this.EnabledChanged;
				if (enabledChanged != null)
					enabledChanged(this, EventArgs.Empty);
			}
		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("Sets a value indicating whether text in the text box is read-only.")]
		public bool ReadOnly
		{
			get
			{
				return edit.ReadOnly;
			}
			set
			{
				edit.ReadOnly = value;
			}
		}

		[Description("Sets the Button's animation speed (in milliseconds) when moving from one state to another.")]
		[Category("x Properties")]
        private int animationSpeed = 200;

        public virtual int AnimationSpeed
        {
            get { return animationSpeed; }
            set { animationSpeed = value; }
        }


		[Description("Sets the TextBox's left margin.")]
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

		[Browsable(false)]
		[Category("x Properties")]
		[Description("Sets the TextBox's bottom margin.")]
		public virtual int TextMarginBottom
		{
			get
			{
				return _textMarginBottom;
			}
			set
			{
				_textMarginBottom = value;
			}
		}

		[Browsable(true)]
		[Category("x Properties")]
		[Description("Sets the TextBox's top margin.")]
		public virtual int TextMarginTop
		{
			get
			{
				return _textMarginTop;
			}
			set
			{
				_textMarginTop = value;
				Refresh();
			}
		}

		[Description("Sets the TextBox's padding for both the left and the right icon.")]
		[Category("x Properties")]
		public virtual int IconPadding
		{
			get
			{
				return _padding;
			}
			set
			{
				_padding = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets TextBox's border thickness.")]
		public virtual int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				if (value > 0 || value <= 5)
				{
					_borderThickness = value;
					Refresh();
				}
			}
		}

		[Description("Sets the TextBox's border radius.")]
		[Category("x Properties")]
		public virtual int BorderRadius
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

		[Category("x Properties")]
		[Description("Sets the TextBox's placeholder.")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public virtual string PlaceholderText
		{
			get
			{
				return edit.PlaceholderText;
			}
			set
			{
				edit.PlaceholderText = value;
				Refresh();
			}
		}

		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		[Category("x Properties")]
		[Description("Sets the text input.")]
		[Browsable(true)]
		[DisplayName("Text")]
		public virtual string DefaultText
		{
			get
			{
				return edit.Text;
			}
			set
			{
				edit.Text = value;
				Refresh();
				OnSizeChanged(this, EventArgs.Empty);
			}
		}

		[Description("Sets the TextBox's left icon.")]
		[Category("x Properties")]
		public virtual Image IconLeft
		{
			get
			{
				return pbLeftIcon.Image;
			}
			set
			{
				pbLeftIcon.Image = value;
				pbLeftIcon.Visible = pbLeftIcon.Image != null;
			}
		}

		[Description("Sets the TextBox's right icon.")]
		[Category("x Properties")]
		public virtual Image IconRight
		{
			get
			{
				return pbRightIcon.Image;
			}
			set
			{
				pbRightIcon.Image = value;
				pbRightIcon.Visible = pbRightIcon.Image != null;
			}
		}

		[Category("x Properties")]
		[Description("Sets the placeholder's ForeColor.")]
		public virtual Color PlaceholderForeColor
		{
			get
			{
				return edit.PlaceholderTextColor;
			}
			set
			{
				edit.PlaceholderTextColor = value;
				edit.Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the TextBox's foreground color.")]
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

		[Category("x Properties")]
		[Description("Sets the TextBox's border color when disabled.")]
		public virtual Color BorderColorDisabled
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

		[Description("Sets the TextBox's border color when idle.")]
		[Category("x Properties")]
		public virtual Color BorderColorIdle
		{
			get
			{
				return _idleBorderColor;
			}
			set
			{
				_idleBorderColor = value;
				_onIdleState.BorderColor = value;
				Refresh();
			}
		}

		[Description("Sets the TextBox's fill color or inner-background color.")]
		[Category("x Properties")]
		public virtual Color FillColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
				edit.BackColor = value;
				_onIdleState.FillColor = value;
				Refresh();
			}
		}

		[Description("Sets the TextBox's border color on mouse hover.")]
		[Category("x Properties")]
		public virtual Color BorderColorHover
		{
			get
			{
				return _hoverBorderColor;
			}
			set
			{
				_hoverBorderColor = value;
				OnHoverState.BorderColor = value;
				Refresh();
			}
		}

		[Description("Sets the TextBox's border color when active or focused.")]
		[Category("x Properties")]
		public virtual Color BorderColorActive
		{
			get
			{
				return _activeBorderColor;
			}
			set
			{
				_activeBorderColor = value;
				OnActiveState.BorderColor = value;
				Refresh();
			}
		}

		[Description("Sets the TextBox's right icon cursor.")]
		[Category("x Properties")]
		public virtual Cursor IconRightCursor
		{
			get
			{
				return pbRightIcon.Cursor;
			}
			set
			{
				pbRightIcon.Cursor = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets TextBox's left icon cursor.")]
		public virtual Cursor IconLeftCursor
		{
			get
			{
				return pbLeftIcon.Cursor;
			}
			set
			{
				pbLeftIcon.Cursor = value;
			}
		}

		[Description("Sets the TextBox's style.")]
		[Category("x Properties")]
		public virtual _Style Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
				Refresh();
			}
		}

		[Category("Appearance")]
		[Description("Represents the image used to paint the mouse pointer.")]
		public override Cursor Cursor
		{
			get
			{
				return base.Cursor;
			}
			set
			{
				base.Cursor = value;
				edit.Cursor = value;
				if (pbLeftIcon.Image == null)
					pbLeftIcon.Cursor = value;
				if (pbRightIcon.Image == null)
					pbRightIcon.Cursor = value;
			}
		}

		[DisplayName("Font")]
		[Browsable(true)]
		[Description("Sets the text input's font.")]
		[Category("x Properties")]
		public new Font DefaultFont
		{
			get
			{
				return edit.Font;
			}
			set
			{
				edit.Font = value;
			}
		}

		[Category("x Properties")]
		[Editor(typeof(TextBoxStatesColorEditor), typeof(UITypeEditor))]
		[Description("Sets the Textbox's idle state design.")]
		public virtual StateProperties OnIdleState
		{
			get
			{
				ForeColor = _onIdleState.ForeColor;
				FillColor = _onIdleState.FillColor;
				BorderColorIdle = _onIdleState.BorderColor;
				Refresh();
				return _onIdleState;
			}
			set
			{
				_onIdleState = value;
				Refresh();
			}
		}

		[DisplayName("OnHoverState")]
		[Category("x Properties")]
		[Editor(typeof(TextBoxStatesColorEditor), typeof(UITypeEditor))]
		[Description("Sets the Textbox's hover state design.")]
		public virtual StateProperties OnHoverState
		{
			get
			{
				Refresh();
				return _onHoverState;
			}
			set
			{
				_onHoverState = value;
				Refresh();
			}
		}

		[Editor(typeof(TextBoxStatesColorEditor), typeof(UITypeEditor))]
		[Category("x Properties")]
		[Description("Sets the Textbox's active state design.")]
		public virtual StateProperties OnActiveState
		{
			get
			{
				Refresh();
				return _onActiveState;
			}
			set
			{
				_onActiveState = value;
				Refresh();
			}
		}

		[Description("Sets the Textbox's disabled state design.")]
		[Category("x Properties")]
		[Editor(typeof(TextBoxStatesColorEditor), typeof(UITypeEditor))]
		public virtual StateProperties OnDisabledState
		{
			get
			{
				if (!_enabled)
				{
					_currentState = state.disabled;
					ForeColor = _onDisabledState.ForeColor;
					FillColor = _onDisabledState.FillColor;
					BorderColorIdle = _onDisabledState.BorderColor;
					Refresh();
				}
				return _onDisabledState;
			}
			set
			{
				_onDisabledState = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public TextBox Input
		{
			get
			{
				return edit;
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
		public bool isOnFocused
		{
			get
			{
				return edit == base.ActiveControl;
			}
		}

		[Browsable(false)]
		public new string Text
		{
			get
			{
				return DefaultText;
			}
			set
			{
				DefaultText = value;
			}
		}

		[Browsable(false)]
		public virtual string TextPlaceholder
		{
			get
			{
				return edit.PlaceholderText;
			}
			set
			{
				edit.PlaceholderText = value;
				Refresh();
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
		public Rectangle DrawingRectangle { get; private set; }

		[Browsable(false)]
		public override bool Focused
		{
			get
			{
				if (edit.Focused)
					return true;
				return false;
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

		[Category("Appearance")]
		[Browsable(true)]
		[Description("Indicates which scroll bars will be shown when the TextBox is set to multiline.")]
		public ScrollBars ScrollBars
		{
			get
			{
				return edit.ScrollBars;
			}
			set
			{
				edit.ScrollBars = value;
				edit.PlaceholderText = PlaceholderText;
				edit.Invalidate();
			}
		}

		[Description("Gets or sets a value indicating whether pressing ENTER in a multiline TextBox control creates a new line of text in the control or activates the default button for the form.")]
		[Browsable(true)]
		[Category("Behavior")]
		public bool AcceptsReturn
		{
			get
			{
				return edit.AcceptsReturn;
			}
			set
			{
				edit.AcceptsReturn = value;
			}
		}

		[Description("Gets or sets a value indicating whether pressing the TAB key in a multiline text box control types a TAB character in the control instead of moving the focus to the next control in the tab order.")]
		[Browsable(true)]
		[Category("Behavior")]
		public bool AcceptsTab
		{
			get
			{
				return edit.AcceptsTab;
			}
			set
			{
				edit.AcceptsTab = value;
			}
		}

		[Description("Gets or sets a custom System.Collections.Specialized.StringCollection to use when the AutoCompleteSource property is set to CustomSource.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public AutoCompleteStringCollection AutoCompleteCustomSource
		{
			get
			{
				return edit.AutoCompleteCustomSource;
			}
			set
			{
				edit.AutoCompleteCustomSource = value;
			}
		}

		[Description("Gets or sets an option that controls how automatic completion works for the TextBox.")]
		[Browsable(true)]
		public AutoCompleteMode AutoCompleteMode
		{
			get
			{
				return edit.AutoCompleteMode;
			}
			set
			{
				edit.AutoCompleteMode = value;
			}
		}

		[Description("Gets or sets a value specifying the source of complete strings used for automatic completion.")]
		[Browsable(true)]
		public AutoCompleteSource AutoCompleteSource
		{
			get
			{
				return edit.AutoCompleteSource;
			}
			set
			{
				edit.AutoCompleteSource = value;
			}
		}

		[Browsable(false)]
		[Description("Gets a value indicating whether the user can undo the previous operation in a text box control.")]
		public bool CanUndo
		{
			get
			{
				return edit.CanUndo;
			}
		}

		[Description("Gets or sets whether the TextBox control modifies the case of characters as they are typed.")]
		[Category("Behavior")]
		[Browsable(true)]
		public CharacterCasing CharacterCasing
		{
			get
			{
				return edit.CharacterCasing;
			}
			set
			{
				edit.CharacterCasing = value;
				if (edit.CharacterCasing == CharacterCasing.Lower)
					edit.PlaceholderText = edit.PlaceholderText.ToLower();
				else if (edit.CharacterCasing == CharacterCasing.Upper)
				{
					edit.PlaceholderText = edit.PlaceholderText.ToUpper();
				}
				else if (edit.CharacterCasing == CharacterCasing.Normal)
				{
					edit.PlaceholderText = ((!string.IsNullOrWhiteSpace(edit.PlaceholderText)) ? FirstLetterToUpper(edit.PlaceholderText.ToLower()) : "");
				}
			}
		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("Gets or sets a value indicating whether the selected text in the text box control remains highlighted when the control loses focus.")]
		public bool HideSelection
		{
			get
			{
				return edit.HideSelection;
			}
			set
			{
				edit.HideSelection = value;
				edit.PlaceholderText = PlaceholderText;
				edit.Invalidate();
			}
		}

		[Browsable(true)]
		[Description("Gets or sets the lines of text in a text box control.")]
		[Category("Appearance")]
		public string[] Lines
		{
			get
			{
				return edit.Lines;
			}
			set
			{
				edit.Lines = value;
			}
		}

		[Description("Gets or sets the maximum number of characters the user can type or paste into the text box control.")]
		[Category("Behavior")]
		[Browsable(true)]
		public int MaxLength
		{
			get
			{
				return edit.MaxLength;
			}
			set
			{
				edit.MaxLength = value;
			}
		}

		[Browsable(true)]
		[Description("Gets or sets a value that indicates that the text box control has been modified by the user since the control was created or its contents were last set.")]
		[Category("Behavior")]
		public bool Modified
		{
			get
			{
				return edit.Modified;
			}
			set
			{
				edit.Modified = value;
			}
		}

		[Description("Gets or sets a value indicating whether this is a multiline TextBox control.")]
		[Category("Behavior")]
		[Browsable(true)]
		public bool Multiline
		{
			get
			{
				return edit.Multiline;
			}
			set
			{
				edit.Multiline = value;
				edit.PlaceholderText = PlaceholderText;
				edit.Invalidate();
			}
		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("Gets or sets the character used to mask characters of a password in a single-line TextBox control.")]
		public char PasswordChar
		{
			get
			{
				return edit.PasswordChar;
			}
			set
			{
				edit.PasswordChar = value;
				edit.Invalidate();
			}
		}

		[Browsable(false)]
		public new Font Font
		{
			get
			{
				return DefaultFont;
			}
			set
			{
				DefaultFont = value;
			}
		}

		[Description("Gets the preferred height for a text box.")]
		[Category("Behavior")]
		[Browsable(false)]
		public int PreferredHeight
		{
			get
			{
				return edit.PreferredHeight;
			}
		}

		[Category("Behavior")]
		[Description("Gets or sets a value indicating the currently selected text in the control.")]
		[Browsable(true)]
		public string SelectedText
		{
			get
			{
				return edit.SelectedText;
			}
			set
			{
				edit.SelectedText = value;
				edit.Invalidate();
			}
		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("Gets or sets the number of characters selected in the text box.")]
		public int SelectionLength
		{
			get
			{
				return edit.SelectionLength;
			}
			set
			{
				edit.SelectionLength = value;
			}
		}

		[Category("Behavior")]
		[Browsable(true)]
		[Description("Gets or sets the starting point of text selected in the text box.")]
		public int SelectionStart
		{
			get
			{
				return edit.SelectionStart;
			}
			set
			{
				edit.SelectionStart = value;
			}
		}

		[Category("Behavior")]
		[Description("Gets or sets a value indicating whether the defined shortcuts are enabled.")]
		[Browsable(true)]
		public bool ShortcutsEnabled
		{
			get
			{
				return edit.ShortcutsEnabled;
			}
			set
			{
				edit.ShortcutsEnabled = value;
			}
		}

		[Category("x Properties")]
		[Description("Gets or sets how text is aligned in a TextBox control.")]
		[Browsable(true)]
		public HorizontalAlignment TextAlign
		{
			get
			{
				return edit.TextAlign;
			}
			set
			{
				edit.TextAlign = value;
				edit.PlaceholderText = PlaceholderText;
			}
		}

		[Description("Gets the length of text in the control.")]
		[Browsable(false)]
		[Category("Behavior")]
		public int TextLength
		{
			get
			{
				return edit.TextLength;
			}
		}

		[Browsable(true)]
		[Category("Behavior")]
		[Description("Gets or sets a value indicating whether the text in the TextBox control should appear as the default password character.")]
		public bool UseSystemPasswordChar
		{
			get
			{
				return edit.UseSystemPasswordChar;
			}
			set
			{
				edit.UseSystemPasswordChar = value;
				edit.PlaceholderText = PlaceholderText;
			}
		}

		[Category("Behavior")]
		[Browsable(true)]
		[Description("Indicates whether a multiline text box control automatically wraps words to the beginning of the next line when necessary.")]
		public bool WordWrap
		{
			get
			{
				return edit.WordWrap;
			}
			set
			{
				edit.WordWrap = value;
				edit.Invalidate();
			}
		}

		[Browsable(false)]
		public override bool AutoScroll
		{
			get
			{
				return base.AutoScroll;
			}
			set
			{
				base.AutoScroll = value;
			}
		}

		[Browsable(false)]
		public override AutoValidate AutoValidate
		{
			get
			{
				return base.AutoValidate;
			}
			set
			{
				base.AutoValidate = value;
			}
		}

		[Description("Occurs when the control's enabled state changes.")]
		public new event EventHandler EnabledChanged = null;

		[Description("Occurs when the property 'Text' changes.")]
		public event EventHandler TextChange = null;

		[Description("Occurs when the property 'Text' changes.")]
		public new event EventHandler TextChanged = null;

		[Description(" Occurs when the property 'AcceptsTab' changes.")]
		public event EventHandler AcceptsTabChanged;

		[Description("Occurs when the 'BorderStyle' property changes.")]
		public event EventHandler BorderStyleChanged;

		[Description("Occurs when the 'HideSelection' property changes.")]
		public event EventHandler HideSelectionChanged;

		[Description("Occurs when the 'Modified' property changes.")]
		public event EventHandler ModifiedChanged;

		[Description("Occurs when the 'ReadOnly' property changes.")]
		public event EventHandler ReadOnlyChanged;

		[Description("Occurs when the 'TextAlign' property changes.")]
		public event EventHandler TextAlignChanged;

		[Description("Occurs when the left icon is clicked.")]
		public event EventHandler OnIconLeftClick;

		[Description("Occurs when the right icon is clicked.")]
		public event EventHandler OnIconRightClick;

		[Description("Occurs when a key is first pressed.")]
		public new event KeyEventHandler KeyDown;

		[Description("Occurs when the control has focus and the user presses and releases a key.")]
		public new event KeyPressEventHandler KeyPress;

		[Description("Occurs when a key is released.")]
		public new event KeyEventHandler KeyUp;

		public xTextBox()
		{
			InitializeComponent();
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BorderThickness = 1;
			FillColor = Color.White;
			PlaceholderText = "Enter text";
			BorderColorIdle = Color.Silver;
			BorderColorActive = Color.DodgerBlue;
			BorderColorHover = Color.FromArgb(105, 181, 255);
			BorderColorDisabled = Color.FromArgb(204, 204, 204);
			DefaultFont = new Font("Segoe UI", 9.25f, FontStyle.Regular);
			_onDisabledState.BorderColor = Color.FromArgb(204, 204, 204);
			_onDisabledState.FillColor = Color.FromArgb(240, 240, 240);
			_onDisabledState.ForeColor = Color.FromArgb(109, 109, 109);
			_onDisabledState.PlaceholderForeColor = Color.DarkGray;
			_onIdleState.BorderColor = BorderColorIdle;
			_onHoverState.BorderColor = BorderColorHover;
			_onActiveState.BorderColor = BorderColorActive;
			base.Width = 260;
			base.Height = 35;
			base.Padding = new Padding(3);
			edit.Invalidate();
			edit.Refresh();
			Input.TextChanged += delegate
			{
				EventHandler textChange = this.TextChange;
				if (textChange != null)
					textChange(this, EventArgs.Empty);
				EventHandler textChanged = this.TextChanged;
				if (textChanged != null)
					textChanged(this, EventArgs.Empty);
			};
			Input.GotFocus += delegate
			{
				OnGotFocus(EventArgs.Empty);
			};
			Input.LostFocus += delegate
			{
				OnLostFocus(EventArgs.Empty);
			};
			EnabledChanged += OnEnabledChanged;
			Refresh();
			ResizeControl();
		}

		public new void Refresh()
		{
			try
			{
				int num2 = (pbLeftIcon.Top = (pbLeftIcon.Left = _padding));
				PictureBox pictureBox = pbLeftIcon;
				PictureBox pictureBox2 = pbLeftIcon;
				PictureBox pictureBox3 = pbRightIcon;
				int num4 = (pbRightIcon.Width = base.Height - _padding * 2);
				int num6 = (pictureBox3.Height = num4);
				num2 = (pictureBox.Height = (pictureBox2.Width = num6));
				int num9 = edit.Top - _borderThickness;
				if (!Multiline)
				{
					edit.Top = 0;
					edit.Top = (base.Height - edit.Height) / 2 - _textMarginBottom + _textMarginTop;
				}
				else
				{
					edit.Anchor = AnchorStyles.Top;
					edit.Top = DrawingRectangle.Top + _borderThickness * 2 + 1 + _textMarginTop;
					edit.Height = base.Height - edit.Top * 2 - 1;
					edit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				}
				pbRightIcon.Top = _padding;
				pbRightIcon.Left = base.Width - pbRightIcon.Width - _padding;
				if (pbLeftIcon.Image == null)
					edit.Left = pbLeftIcon.Left + TextMarginLeft;
				else
					edit.Left = pbLeftIcon.Right + TextMarginLeft;
				if (pbRightIcon.Image == null)
					edit.Width = pbRightIcon.Right - edit.Left - TextMarginLeft;
				else
					edit.Width = pbRightIcon.Left - edit.Left - TextMarginLeft;
				Pen pen = new Pen(BorderColorIdle, BorderThickness);
				if (_pen == null)
					_pen = pen;
				pen = ((GetState() == state.idle) ? new Pen(BorderColorIdle, BorderThickness) : ((GetState() != state.hover) ? new Pen(BorderColorActive, BorderThickness) : new Pen(BorderColorHover, BorderThickness)));
				if (!_enabled)
				{
					_pen = new Pen(BorderColorDisabled, BorderThickness);
					Animate(_onDisabledState);
					if (base.Parent != null)
					{
						Input.Cursor = base.Parent.Cursor;
						Cursor = base.Parent.Cursor;
					}
				}
				else
				{
					_pen = new Pen(BorderColorIdle, BorderThickness);
					Input.Cursor = Cursors.IBeam;
					Cursor = Cursors.IBeam;
				}
				Bitmap bitmap = new Bitmap(base.Width, base.Height);
				Rectangle rectangle = new Rectangle(BorderThickness, BorderThickness, base.Width - BorderThickness * 2 - 1, base.Height - BorderThickness * 2 - 1);
				DrawRoundedRectangle(Graphics.FromImage(bitmap), rectangle, BorderRadius, _pen, FillColor);
				BackgroundImage = bitmap;
				DrawingRectangle = rectangle;
				edit.Invalidate();
				edit.Refresh();
				pen.Dispose();
				_pen.Dispose();
			}
			catch (Exception)
			{
			}
		}

		public new void Focus()
		{
			edit.Focus();
			OnEnterTextBox(this, EventArgs.Empty);
		}

		public void ResetColors()
		{
			_tmpIdleState.ForeColor = Color.Empty;
			_tmpIdleState.FillColor = Color.Empty;
			_tmpIdleState.BorderColor = Color.Empty;
			_tmpIdleState.PlaceholderForeColor = Color.Empty;
		}

		public void ResizeControl()
		{
			if (!Multiline)
			{
				base.Height = 0;
				edit.Top = DrawingRectangle.Top + _borderThickness * 2 + 1 + _textMarginTop + base.Padding.Top;
				base.Height = edit.Height + edit.Top * 2 + _borderThickness * 2 + _textMarginBottom + 1 + base.Padding.Bottom;
				edit.Top = (base.Height - edit.Height) / 2 - _textMarginBottom;
			}
		}

		public void Reset()
		{
			edit.Reset();
		}

		public override void ResetText()
		{
			edit.ResetText();
		}

		public void Clear()
		{
			edit.Clear();
		}

		public void ClearUndo()
		{
			edit.ClearUndo();
		}

		public void AppendText(string text)
		{
			edit.AppendText(text);
		}

		public void Copy()
		{
			edit.Copy();
		}

		public void Cut()
		{
			edit.Cut();
		}

		public void DeselectAll()
		{
			edit.DeselectAll();
		}

		public virtual char GetCharFromPosition(Point pt)
		{
			return edit.GetCharFromPosition(pt);
		}

		public virtual int GetCharIndexFromPosition(Point pt)
		{
			return edit.GetCharIndexFromPosition(pt);
		}

		public int GetFirstCharIndexFromLine(int lineNumber)
		{
			return edit.GetFirstCharIndexFromLine(lineNumber);
		}

		public int GetFirstCharIndexOfCurrentLine()
		{
			return edit.GetFirstCharIndexOfCurrentLine();
		}

		public virtual int GetLineFromCharIndex(int index)
		{
			return edit.GetLineFromCharIndex(index);
		}

		public virtual Point GetPositionFromCharIndex(int index)
		{
			return edit.GetPositionFromCharIndex(index);
		}

		public void Paste()
		{
			edit.Paste();
		}

		public void ScrollToCaret()
		{
			edit.ScrollToCaret();
		}

		public void Select(int start, int length)
		{
			edit.Select(start, length);
		}

		public void SelectAll()
		{
			edit.SelectAll();
		}

		public override string ToString()
		{
			return edit.ToString();
		}

		public void Undo()
		{
			edit.Undo();
		}

		private void DrawRoundedRectangle(Graphics gfx, Rectangle Bounds, int CornerRadius, Pen DrawPen, Color FillColor)
		{
			gfx.Clear(Color.Transparent);
			gfx.SmoothingMode = SmoothingMode.HighQuality;
			GraphicsPath graphicsPath = new GraphicsPath();
			if (Style == _Style.x)
			{
				if (_borderRadius > 1)
				{
					DrawPen.StartCap = LineCap.Round;
					DrawPen.EndCap = LineCap.Round;
					graphicsPath.AddArc(Bounds.X, Bounds.Y, CornerRadius, CornerRadius, 180f, 90f);
					graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y, CornerRadius, CornerRadius, 270f, 90f);
					graphicsPath.AddArc(Bounds.X + Bounds.Width - CornerRadius, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 0f, 90f);
					graphicsPath.AddArc(Bounds.X, Bounds.Y + Bounds.Height - CornerRadius, CornerRadius, CornerRadius, 90f, 90f);
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
			}
			else
			{
				BackColor = this.FillColor;
				DrawPen.StartCap = LineCap.Round;
				DrawPen.EndCap = LineCap.Round;
				gfx.DrawLine(DrawPen, new Point(0, base.Height - _borderThickness), new Point(base.Width, base.Height - _borderThickness));
			}
			gfx.Dispose();
			graphicsPath.Dispose();
			DrawPen.Dispose();
		}

		private state GetState()
		{
			return _currentState;
		}

		private bool IsMouseWithinControl()
		{
			if (base.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
				return true;
			return false;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 7 && !edit.SelectionEnabled)
				m.Msg = 8;
			base.WndProc(ref m);
		}

		private bool IsInDesignMode()
		{
			if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
				return true;
			return false;
		}

		private string FirstLetterToUpper(string str)
		{
			if (str == null)
				return null;
			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);
			return str.ToUpper();
		}

		private void Animate(StateProperties state)
		{
			try
			{
				SaveCurrentState();
				Color fillColor = state.FillColor;
				Color borderColor = state.BorderColor;
				Color foreColor = state.ForeColor;
				Color placeholderForeColor = state.PlaceholderForeColor;
				if (!_enabled)
				{
					fillColor = _onDisabledState.FillColor;
					borderColor = _onDisabledState.BorderColor;
					foreColor = _onDisabledState.ForeColor;
					placeholderForeColor = _onDisabledState.PlaceholderForeColor;
					Input.ReadOnly = true;
				}
				else
					Input.ReadOnly = ReadOnly;
				if (!state.FillColor.IsEmpty)
					Transition.run(this, "FillColor", fillColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
				if (!state.BorderColor.IsEmpty)
					Transition.run(this, "BorderColorIdle", borderColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
				if (!state.ForeColor.IsEmpty)
					Transition.run(this, "ForeColor", foreColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
				if (!state.PlaceholderForeColor.IsEmpty)
					Transition.run(this, "PlaceholderForeColor", placeholderForeColor, new TransitionType_EaseInEaseOut(AnimationSpeed));
			}
			catch (Exception)
			{
			}
		}

		private void SaveCurrentState()
		{
			if (_tmpIdleState.BorderColor.IsEmpty)
			{
				_tmpIdleState.ForeColor = ForeColor;
				_tmpIdleState.FillColor = FillColor;
				_tmpIdleState.BorderColor = BorderColorIdle;
				_tmpIdleState.PlaceholderForeColor = PlaceholderForeColor;
			}
		}

		private void OnLoad(object sender, EventArgs e)
		{
			edit.TextChanged += this.TextChange;
			edit.TextChanged += this.TextChanged;
			edit.AcceptsTabChanged += this.AcceptsTabChanged;
			edit.BorderStyleChanged += this.BorderStyleChanged;
			edit.HideSelectionChanged += this.HideSelectionChanged;
			edit.ModifiedChanged += this.ModifiedChanged;
			edit.FontChanged += OnFontChanged;
			edit.SizeChanged += OnSizeChanged;
			edit.ReadOnlyChanged += this.ReadOnlyChanged;
			edit.TextAlignChanged += this.TextAlignChanged;
			pbRightIcon.Click += this.OnIconRightClick;
			edit.KeyDown += this.KeyDown;
			edit.KeyPress += this.KeyPress;
			edit.KeyUp += this.KeyUp;
			pbLeftIcon.Click += this.OnIconLeftClick;
		}

		private void OnResize(object sender, EventArgs e)
		{
			Refresh();
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
		}

		private void OnFontChanged(object sender, EventArgs e)
		{
			ResizeControl();
		}

		private void OnForeColorChanged(object sender, EventArgs e)
		{
			edit.ForeColor = ForeColor;
		}

		private void OnClickControls(object sender, EventArgs e)
		{
			base.OnClick(e);
		}

		private void OnEnterTextBox(object sender, EventArgs e)
		{
			base.OnEnter(e);
			if (!base.DesignMode)
				Animate(OnActiveState);
		}

		private void OnLeaveTextBox(object sender, EventArgs e)
		{
			base.OnLeave(e);
			if (!base.DesignMode)
				Animate(_tmpIdleState);
		}

		private void OnMouseDownTextBox(object sender, MouseEventArgs e)
		{
			OnMouseDown(e);
		}

		private void OnMouseUpTextBox(object sender, MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		private void Edit_MouseClick(object sender, MouseEventArgs e)
		{
			base.OnMouseClick(e);
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
		}

		private void OnMouseEnterControls(object sender, EventArgs e)
		{
			OnMouseHover(e);
		}

		private void OnMouseLeaveControls(object sender, EventArgs e)
		{
			if (!IsMouseWithinControl())
				base.OnMouseLeave(e);
		}

		private void OnChangeFont(object sender, EventArgs e)
		{
			edit.Font = DefaultFont;
		}

		private void OnEnabledChanged(object sender, EventArgs e)
		{
			SaveCurrentState();
			if (!_enabled)
			{
				Input.ReadOnly = true;
				_currentState = state.disabled;
				Animate(OnDisabledState);
			}
			else
			{
				Input.ReadOnly = false;
				_currentState = state.idle;
				Animate(_tmpIdleState);
			}
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			base.OnPaddingChanged(e);
			ResizeControl();
		}

		protected override void OnContextMenuChanged(EventArgs e)
		{
			base.OnContextMenuChanged(e);
			Input.ContextMenu = ContextMenu;
		}

		protected override void OnContextMenuStripChanged(EventArgs e)
		{
			base.OnContextMenuStripChanged(e);
			Input.ContextMenuStrip = ContextMenuStrip;
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (!edit.Focused)
				Animate(OnHoverState);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (!IsMouseWithinControl() && !edit.Focused)
				Animate(_tmpIdleState);
			if (edit.Focused)
				Animate(OnActiveState);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			edit.Focus();
		}

		private void Edit_DragDrop(object sender, DragEventArgs e)
		{
			base.OnDragDrop(e);
		}

		private void Edit_DragEnter(object sender, DragEventArgs e)
		{
			base.OnDragEnter(e);
		}

		private void Edit_DragLeave(object sender, EventArgs e)
		{
			base.OnDragLeave(e);
		}

		private void Edit_DragOver(object sender, DragEventArgs e)
		{
			base.OnDragOver(e);
		}

		private void Edit_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);
		}

		private void Edit_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
		{
			base.OnQueryContinueDrag(e);
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
			this.edit = new System.Windows.Forms.PlaceholderTextBox();
			((System.ComponentModel.ISupportInitialize)this.pbLeftIcon).BeginInit();
			((System.ComponentModel.ISupportInitialize)this.pbRightIcon).BeginInit();
			base.SuspendLayout();
			this.pbLeftIcon.BackColor = System.Drawing.Color.Transparent;
			this.pbLeftIcon.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbLeftIcon.Location = new System.Drawing.Point(6, 5);
			this.pbLeftIcon.Name = "pbLeftIcon";
			this.pbLeftIcon.Size = new System.Drawing.Size(27, 25);
			this.pbLeftIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbLeftIcon.TabIndex = 1;
			this.pbLeftIcon.TabStop = false;
			this.pbLeftIcon.Visible = false;
			this.pbLeftIcon.Click += new System.EventHandler(OnClickControls);
			this.pbLeftIcon.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.pbLeftIcon.MouseLeave += new System.EventHandler(OnMouseLeaveControls);
			this.pbRightIcon.BackColor = System.Drawing.Color.Transparent;
			this.pbRightIcon.Cursor = System.Windows.Forms.Cursors.Hand;
			this.pbRightIcon.Location = new System.Drawing.Point(163, 3);
			this.pbRightIcon.Name = "pbRightIcon";
			this.pbRightIcon.Size = new System.Drawing.Size(27, 25);
			this.pbRightIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbRightIcon.TabIndex = 2;
			this.pbRightIcon.TabStop = false;
			this.pbRightIcon.Visible = false;
			this.pbRightIcon.Click += new System.EventHandler(OnClickControls);
			this.pbRightIcon.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.pbRightIcon.MouseLeave += new System.EventHandler(OnMouseLeaveControls);
			this.edit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.edit.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.edit.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.edit.Location = new System.Drawing.Point(24, 10);
			this.edit.Name = "edit";
			this.edit.PlaceholderText = "";
			this.edit.PlaceholderTextColor = System.Drawing.Color.Silver;
			this.edit.Size = new System.Drawing.Size(150, 13);
			this.edit.TabIndex = 3;
			this.edit.Click += new System.EventHandler(OnClickControls);
			this.edit.MouseClick += new System.Windows.Forms.MouseEventHandler(Edit_MouseClick);
			this.edit.FontChanged += new System.EventHandler(OnFontChanged);
			this.edit.TextChanged += new System.EventHandler(OnTextChanged);
			this.edit.DragDrop += new System.Windows.Forms.DragEventHandler(Edit_DragDrop);
			this.edit.DragEnter += new System.Windows.Forms.DragEventHandler(Edit_DragEnter);
			this.edit.DragOver += new System.Windows.Forms.DragEventHandler(Edit_DragOver);
			this.edit.DragLeave += new System.EventHandler(Edit_DragLeave);
			this.edit.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(Edit_QueryContinueDrag);
			this.edit.Enter += new System.EventHandler(OnEnterTextBox);
			this.edit.Leave += new System.EventHandler(OnLeaveTextBox);
			this.edit.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(Edit_MouseDoubleClick);
			this.edit.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDownTextBox);
			this.edit.MouseEnter += new System.EventHandler(OnMouseEnterControls);
			this.edit.MouseLeave += new System.EventHandler(OnMouseLeaveControls);
			this.edit.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUpTextBox);
			this.BackColor = System.Drawing.Color.Transparent;
			base.Controls.Add(this.pbRightIcon);
			base.Controls.Add(this.pbLeftIcon);
			base.Controls.Add(this.edit);
			this.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.MinimumSize = new System.Drawing.Size(1, 1);
			base.Name = "xTextBox";
			base.Size = new System.Drawing.Size(200, 35);
			base.Load += new System.EventHandler(OnLoad);
			base.FontChanged += new System.EventHandler(OnFontChanged);
			base.ForeColorChanged += new System.EventHandler(OnForeColorChanged);
			base.Resize += new System.EventHandler(OnResize);
			((System.ComponentModel.ISupportInitialize)this.pbLeftIcon).EndInit();
			((System.ComponentModel.ISupportInitialize)this.pbRightIcon).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
