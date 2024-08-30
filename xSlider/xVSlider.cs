using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Utilities.xSlider;

namespace xCollection
{
	[Designer(typeof(xDesigner))]
	[Description("Provides enhanced vertical ranged value selections and extra customization options.")]
	[Category("x UI For Windows Forms")]
	[DefaultEvent("Scroll")]
	[ToolboxBitmap(typeof(TrackBar))]
	[DefaultProperty("Value")]
	[ToolboxItem(true)]
	[DebuggerStepThrough]
	public class xVSlider : xVScrollBar
	{
		private new enum DirectionalMovements
		{
			TopDown,
			BottomUp
		}

		public new enum ThumbStyles
		{
			Fill,
			Outline
		}

		public enum ThumbSizes
		{
			Small,
			Medium,
			Large
		}

		public enum SliderStyles
		{
			Thick,
			Thin
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public new class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					xVSlider VSlider = (xVSlider)base.Control;
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
							new xVSliderActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		public class xVSliderActionList : DesignerActionList
		{
			private xVSlider xControl;

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

			public xVSliderActionList(IComponent component)
				: base(component)
			{
				xControl = component as xVSlider;
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

		[DebuggerStepThrough]
		[Browsable(false)]
		[Description("An abstract class used to define various states within x Slider.")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class SliderState
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			private readonly string _003CName_003Ek__BackingField;

			[Browsable(false)]
			[EditorBrowsable(EditorBrowsableState.Always)]
			public virtual string Name
			{
				[CompilerGenerated]
				get
				{
					return _003CName_003Ek__BackingField;
				}
			}

			[Category("x Properties")]
			[Description("Sets the background color of the Slider.")]
			public Color SliderColor { get; set; }

			[Description("Sets the slider's elapsed region color.")]
			[Category("x Properties")]
			public Color ElapsedColor { get; set; }

			[Category("x Properties")]
			[Description("Sets the background color of the Thumb.")]
			public Color ThumbColor { get; set; }

			public SliderState(string name = "(Undefined)")
			{
				_003CName_003Ek__BackingField = name;
			}

			public override string ToString()
			{
				return SliderColor.ToString() + "; " + ElapsedColor.ToString() + "; " + ThumbColor.ToString();
			}
		}

		private Color _elapsedColor;

		private ThumbStyles _thumbStyle;

		private SliderStyles _sliderStyle;

		private DirectionalMovements _direction;

		private SliderState _onDisable = new SliderState("OnDisable");

		private IContainer components = null;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("x Properties")]
		[Description("Sets the directional movement of the slider.")]
		private DirectionalMovements ScrollDirection
		{
			get
			{
				return _direction;
			}
			set
			{
				_direction = value;
				switch (value)
				{
				case DirectionalMovements.BottomUp:
					base.DirectionalScroll = xVScrollBar.DirectionalMovements.BottomUp;
					break;
				case DirectionalMovements.TopDown:
					base.DirectionalScroll = xVScrollBar.DirectionalMovements.TopDown;
					break;
				}
				Refresh();
			}
		}

		[Description("Sets the slider's BackColor.")]
		[Category("x Properties")]
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

		[Description("Sets the slider's elapsed region color.")]
		[Category("x Properties")]
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

		[Category("x Properties")]
		[Description("Sets the thumb's inner fill color. This is only necessary when the property ThumbStyle' is set to 'Outline'.")]
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

		[Browsable(true)]
		[DisplayName("OnDisable")]
		[Description("Represents the disabled or inactive state of the control.")]
		[Category("x Properties")]
		[Editor(typeof(VSliderStatesColorEditor), typeof(UITypeEditor))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override xVScrollBar.DirectionalMovements DirectionalScroll
		{
			get
			{
				return base.DirectionalScroll;
			}
			set
			{
				base.DirectionalScroll = value;
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
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

		public xVSlider()
		{
			InitializeComponent();
			SetDefaults();
		}

		private void SetDefaults()
		{
			MinimumSize = new Size(31, 0);
			OnDisableSlider.ThumbColor = Color.Silver;
			OnDisableSlider.ElapsedColor = Color.Silver;
			OnDisableSlider.SliderColor = Color.Gainsboro;
			Value = 50;
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
			ThumbDrawMode = DrawModes.Outline;
			VisualStyle = ScrollBarVisualStyles.Slider;
			SliderColor = Color.FromArgb(210, 232, 255);
			SliderThumbStyle = SliderThumbStyles.Circular;
			ScrollDirection = DirectionalMovements.BottomUp;
			base.Size = new Size(31, 200);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			SolidBrush solidBrush = new SolidBrush(ElapsedColor);
			e.Graphics.FillRectangle(solidBrush, new Rectangle(base.ScrollBarRectangle.X, _thumb.Bottom, base.ScrollBarRectangle.Width + 1, base.ScrollBarRectangle.Bottom));
			solidBrush.Dispose();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(xVSlider));
			((System.ComponentModel.ISupportInitialize)base._thumb).BeginInit();
			base.SuspendLayout();
			base._thumb.Image = (System.Drawing.Image)resources.GetObject("_thumb.Image");
			base._thumb.Location = new System.Drawing.Point(2, 2);
			base._thumb.Size = new System.Drawing.Size(26, 19);
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
			this.MinimumSize = new System.Drawing.Size(31, 0);
			base.Name = "xVSlider";
			this.OnDisable.ScrollBarBorderColor = System.Drawing.Color.Silver;
			this.OnDisable.ScrollBarColor = System.Drawing.Color.Transparent;
			this.OnDisable.ThumbColor = System.Drawing.Color.Silver;
			base.Size = new System.Drawing.Size(31, 200);
			((System.ComponentModel.ISupportInitialize)base._thumb).EndInit();
			base.ResumeLayout(false);
		}
	}
}
