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

namespace Utilities.xSlider
{
	[DebuggerStepThrough]
	[Browsable(false)]
	[Description("Provides enhanced vertical content-scrolling capabilities and extended customization options at design time.")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Designer(typeof(xDesigner))]
	[ToolboxItem(false)]
	[DefaultProperty("Value")]
	[DefaultEvent("Scroll")]
	public class xVScrollBar : UserControl
	{
		public enum DirectionalMovements
		{
			TopDown,
			BottomUp
		}

		private enum ArrowType
		{
			TopArrow,
			BottomArrow
		}

		private enum ArrowStyle
		{
			Line,
			Fill
		}

		private enum EffectType
		{
			OnEnter,
			OnPress,
			OnLeave
		}

		public enum ThumbStyles
		{
			Inset,
			Proportional
		}

		public enum Direction
		{
			Top,
			Bottom,
			InCursor
		}

		public enum ScrollBarVisualStyles
		{
			ScrollBar,
			Slider
		}

		protected enum DrawModes
		{
			Fill,
			Outline
		}

		public enum SliderThumbStyles
		{
			Thin,
			Circular
		}

		public class ValueChangedEventArgs : EventArgs
		{
			private int _value;

			public int Value
			{
				get
				{
					return _value;
				}
			}

			public ValueChangedEventArgs(int value)
			{
				_value = value;
			}
		}

		public class MouseCaptureChangedEventArgs : EventArgs
		{
			private int _value;

			private Direction _direction;

			public int Value
			{
				get
				{
					return _value;
				}
			}

			public Direction ThumbDirection
			{
				get
				{
					return _direction;
				}
			}

			public MouseCaptureChangedEventArgs(int value, Direction direction)
			{
				_value = value;
				_direction = direction;
			}
		}

		public class ScrollEventArgs : EventArgs
		{
			private int _value;

			public int Value
			{
				get
				{
					return _value;
				}
			}

			public ScrollEventArgs(int value)
			{
				_value = value;
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					xVScrollBar VScrollBar = (xVScrollBar)base.Control;
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
							new xVScrollBarActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		public class xVScrollBarActionList : DesignerActionList
		{
			private xVScrollBar xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public Control BindingContainer
			{
				get
				{
					return xControl.BindingContainer;
				}
				set
				{
					SetValue(xControl, "BindingContainer", value);
				}
			}

			public DockStyle Dock
			{
				get
				{
					return xControl.Dock;
				}
				set
				{
					SetValue(xControl, "Dock", value);
				}
			}

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

			public int BorderThickness
			{
				get
				{
					return xControl.BorderThickness;
				}
				set
				{
					SetValue(xControl, "BorderThickness", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color ScrollBarBorderColor
			{
				get
				{
					return xControl.ScrollBarBorderColor;
				}
				set
				{
					SetValue(xControl, "ScrollBarBorderColor", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color ScrollBarColor
			{
				get
				{
					return xControl.ScrollBarColor;
				}
				set
				{
					SetValue(xControl, "ScrollBarColor", value);
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

			public int ThumbMargin
			{
				get
				{
					return xControl.ThumbMargin;
				}
				set
				{
					SetValue(xControl, "ThumbMargin", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public ThumbStyles ThumbStyle
			{
				get
				{
					return xControl.ThumbStyle;
				}
				set
				{
					SetValue(xControl, "ThumbStyle", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public xVScrollBarActionList(IComponent component)
				: base(component)
			{
				xControl = component as xVScrollBar;
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
					new DesignerActionPropertyItem("BindingContainer", "BindingContainer:", "Common Tasks", GetPropertyDescriptor(base.Component, "BindingContainer").Description),
					new DesignerActionPropertyItem("Dock", "Dock:", "Common Tasks", GetPropertyDescriptor(base.Component, "Dock").Description),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("Minimum", "Minimum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Minimum").Description),
					new DesignerActionPropertyItem("LargeChange", "LargeChange:", "Behaviour", GetPropertyDescriptor(base.Component, "LargeChange").Description),
					new DesignerActionPropertyItem("SmallChange", "SmallChange:", "Behaviour", GetPropertyDescriptor(base.Component, "SmallChange").Description),
					new DesignerActionPropertyItem("BorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "BorderThickness").Description),
					new DesignerActionPropertyItem("ScrollBarBorderColor", "BorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "ScrollBarBorderColor").Description),
					new DesignerActionPropertyItem("ScrollBarColor", "Color:", "Appearance", GetPropertyDescriptor(base.Component, "ScrollBarColor").Description),
					new DesignerActionPropertyItem("ThumbColor", "ThumbColor:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbColor").Description),
					new DesignerActionPropertyItem("ThumbMargin", "ThumbMargin:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbMargin").Description),
					new DesignerActionPropertyItem("ThumbStyle", "ThumbStyle:", "Appearance", GetPropertyDescriptor(base.Component, "ThumbStyle").Description)
				};
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerStepThrough]
		internal class StatesColorEditor : UITypeEditor
		{
			public override bool GetPaintValueSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				return base.EditValue(context, provider, value);
			}

			public override void PaintValue(PaintValueEventArgs e)
			{
				ScrollBarState scrollBarState = ((e.Context != null) ? ((ScrollBarState)e.Value) : new ScrollBarState("Painter"));
				int num = e.Bounds.Width / 3;
				using (SolidBrush brush = new SolidBrush(scrollBarState.ScrollBarBorderColor))
					e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, num, e.Bounds.Height - 1));
				using (SolidBrush brush2 = new SolidBrush(scrollBarState.ScrollBarColor))
					e.Graphics.FillRectangle(brush2, new Rectangle(e.Bounds.X + num, e.Bounds.Y, num, e.Bounds.Height - 1));
				using (SolidBrush brush3 = new SolidBrush(scrollBarState.ThumbColor))
					e.Graphics.FillRectangle(brush3, new Rectangle(e.Bounds.X + num * 2, e.Bounds.Y, e.Bounds.Width - num * 2, e.Bounds.Height - 1));
				base.PaintValue(e);
			}
		}

		[Description("An abstract class used to define various states within x ScrollBars.")]
		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public class ScrollBarState
		{
			private string _name;

			private Color _backColor;

			private Color _thumbColor;

			private Color _scrollbarBorderColor;

			[EditorBrowsable(EditorBrowsableState.Always)]
			[Browsable(false)]
			public virtual string Name
			{
				get
				{
					return _name;
				}
			}

			[Description("Sets the background color of the ScrollBar.")]
			[Category("x Properties")]
			public Color ScrollBarColor
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

			[Description("Sets the border color of the ScrollBar.")]
			[Category("x Properties")]
			public Color ScrollBarBorderColor
			{
				get
				{
					return _scrollbarBorderColor;
				}
				set
				{
					_scrollbarBorderColor = value;
				}
			}

			[Category("x Properties")]
			[Description("Sets the background color of the thumb.")]
			public Color ThumbColor
			{
				get
				{
					return _thumbColor;
				}
				set
				{
					_thumbColor = value;
				}
			}

			public ScrollBarState(string name = "(Undefined)")
			{
				_name = name;
			}

			public override string ToString()
			{
				return ScrollBarBorderColor.ToString() + "; " + ScrollBarColor.ToString() + "; " + ThumbColor.ToString();
			}
		}

		private const int ARROW_LENGTH = 20;

		private const int START_DURATION = 5;

		private int _value;

		private int _mouseY;

		private int _maximum;

		private int _minimum;

		private int _thumbLength;

		private int _thumbMargin;

		private int _largeChange;

		private int _smallChange;

		private int _borderRadius;

		private int _scrollBarSize;

		private int _thicknessBound;

		private int _borderThickness;

		private int _clickedPosition;

		private int _shrinkSizeLimit;

		private int _minimumThumbLength;

		private int _durationBeforeShrink;

		private int _thumbBorderThickness;

		private int _bottomThumbHeightBound;

		private int _topScrollbarHeightBound;

		private int _bottomScrollbarHeightBound;

		private int _thumbMovementStartDuration = 0;

		private bool _isGrowing;

		private bool _autoHidden;

		private bool _isShrinking;

		private bool _isOnKeyDown;

		private bool _raisesScroll;

		protected bool _readyToFill;

		private bool _thumbIsMoving;

		private bool _drawThickBorder;

		private bool _shrinkingEnabled;

		private bool _allowMouseCaptures;

		private bool _allowCursorChanges;

		private bool _leftMouseWasClicked;

		private bool _thumbIsMovingInTimer;

		private bool _allowMouseDownEffects;

		private bool _allowMouseHoverEffects;

		private bool _allowScrollOptionsMenu;

		private bool _allowScrollingAnimations;

		private bool _allowScrollKeysDetection;

		private bool _allowHomeEndKeysDetection;

		private bool _allowShrinkingOnFocusLost;

		private bool _allowIncrementalClickMoves;

		private bool _allowDataGridViewSelections = true;

		private Color _backColor;

		private Color _thumbColor;

		private Color _tempThumbColor;

		private Color _thumbBorderColor;

		private Color _tempScrollBarColor;

		private Color _scrollbarBorderColor;

		private Color _tempScrollBarBorderColor;

		private DrawModes _thumbMode;

		private ThumbStyles _thumbStyle;

		private Direction _thumbDirection;

		private SliderThumbStyles _sliderStyle;

		private DirectionalMovements _direction;

		private ScrollBarVisualStyles _visualStyle;

		private xVSlider.ThumbSizes _thumbSize = xVSlider.ThumbSizes.Medium;

		private Orientation _orientation = Orientation.Vertical;

		private ScrollBarState _onDisable = new ScrollBarState("OnDisable");

		private Control _bindingContainer;

		private System.Threading.Timer _timer;

		protected PictureBox _thumb = new PictureBox();

		private PictureBox _topArrow = new PictureBox();

		private PictureBox _bottomArrow = new PictureBox();

		private IContainer components = null;

		private System.Windows.Forms.Timer Counter;

		private ContextMenuStrip scroll_options;

		private ToolStripMenuItem scroll_here;

		private ToolStripSeparator separator_one;

		private ToolStripMenuItem scroll_to_top;

		private ToolStripMenuItem scroll_to_bottom;

		private ToolStripSeparator toolStripSeparator1;

		private ToolStripMenuItem page_up;

		private ToolStripMenuItem page_down;

		private ToolStripSeparator separator_two;

		private ToolStripMenuItem scroll_up;

		private ToolStripMenuItem scroll_down;

		[Description("Sets the directional movement of the ScrollBar.")]
		[Category("x Properties")]
		public virtual DirectionalMovements DirectionalScroll
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

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(StatesColorEditor), typeof(UITypeEditor))]
		[Description("Represents the disabled or inactive state of the control.")]
		[Category("x Properties")]
		[Browsable(true)]
		public virtual ScrollBarState OnDisable
		{
			get
			{
				return _onDisable;
			}
		}

		[Category("x Properties")]
		[ParenthesizePropertyName(true)]
		[Description("Attaches the ScrollBar to the scroll events of any scrollable container-control or DataGridView.")]
		public virtual Control BindingContainer
		{
			get
			{
				return _bindingContainer;
			}
			set
			{
				try
				{
					if (value != null && IsControlScrollBindable(value))
						_bindingContainer = value;
					else if (value != null && !IsControlScrollBindable(value))
					{
						if (!base.DesignMode)
							throw new Exception("The selected control is not scrollable.\nTry using a scrollable control, e.g. a 'Panel' or 'FlowLayoutPanel'.");
						MessageBox.Show(this, "The selected control is not scrollable.\nTry using a scrollable control, e.g. a 'Panel' or 'FlowLayoutPanel'.", "Non-Scrollable Control");
					}
					else if (value == null)
					{
						_bindingContainer = value;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[Category("Behaviour")]
		[Bindable(true)]
		[Description("Sets the thumb's value or position in the ScrollBar.")]
		public virtual int Value
		{
			get
			{
				return _value;
			}
			set
			{
				try
				{
					if (value > _maximum)
						value = _maximum;
					if (value < _minimum)
						value = _minimum;
				}
				catch (Exception)
				{
				}
				if ((value >= _minimum) & (value <= _maximum))
				{
					_value = value;
					if (this.ValueChanged != null)
						this.ValueChanged(this, new ValueChangedEventArgs(value));
					if (_raisesScroll && this.Scroll != null)
						this.Scroll(this, new ScrollEventArgs(value));
					if (_isOnKeyDown)
						MoveThumb(value, false);
					else
						MoveThumb(value);
					return;
				}
				throw new ArgumentOutOfRangeException("The thumb's value is outside the appropriate range, that is, between the Minimum and Maximum values.");
			}
		}

		[Description("Sets the ScrollBar's maximum scrollable range.")]
		[Category("Behaviour")]
		public virtual int Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				if (value > _minimum)
				{
					_maximum = value;
					if (_value > _maximum)
						_value = _maximum;
					Refresh();
					return;
				}
				throw new ArgumentOutOfRangeException("The Maximum value is less than the Minimum value.");
			}
		}

		[Description("Sets the ScrollBar's minimum scrollable range.")]
		[Category("Behaviour")]
		public virtual int Minimum
		{
			get
			{
				return _minimum;
			}
			set
			{
				if (value < _maximum)
				{
					_minimum = value;
					if (_value < _minimum)
						_value = _minimum;
					Refresh();
					return;
				}
				throw new ArgumentOutOfRangeException("The Minimum value is greater than the Maximum value.");
			}
		}

		[Description("Sets the amount by which the thumb's value changes when the user presses an arrow key.")]
		[Category("Behaviour")]
		public virtual int SmallChange
		{
			get
			{
				return _smallChange;
			}
			set
			{
				_smallChange = value;
			}
		}

		[Description("Sets the amount by which the thumb's value changes when the user clicks in the scroll bar or presses the PageUp or PageDown keys.")]
		[Category("Behaviour")]
		public virtual int LargeChange
		{
			get
			{
				return _largeChange;
			}
			set
			{
				_largeChange = value;
				Refresh();
			}
		}

		[Description("Sets the border-radius of the ScrollBar and the thumb.")]
		[Category("x Properties")]
		public virtual int BorderRadius
		{
			get
			{
				return _borderRadius;
			}
			set
			{
				if (VisualStyle == ScrollBarVisualStyles.ScrollBar)
					_borderRadius = value;
				else if (VisualStyle == ScrollBarVisualStyles.Slider)
				{
					_borderRadius = 2;
				}
				Refresh();
			}
		}

		[Description("[Experimental] Sets the border-thickness of the ScrollBar.")]
		[Category("x Properties")]
		public virtual int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				if (value == 0)
					_borderThickness = 1;
				else
					_borderThickness = value;
				Refresh();
			}
		}

		[Description("Sets the thumb's style in relation to the ScrollBar.")]
		[Category("x Properties")]
		public virtual ThumbStyles ThumbStyle
		{
			get
			{
				return _thumbStyle;
			}
			set
			{
				_thumbStyle = value;
				Refresh();
			}
		}

		[Description("Sets the length of the ScrollBar's thumb.")]
		[Category("x Properties")]
		[Browsable(false)]
		public virtual int ThumbLength
		{
			get
			{
				return _thumbLength;
			}
			set
			{
				if ((value >= _minimumThumbLength) & (value < base.ClientRectangle.Height))
				{
					_thumbLength = value;
					_thumb.Height = value;
				}
				else if (value < _minimumThumbLength)
				{
					_thumbLength = _minimumThumbLength;
					_thumb.Height = _minimumThumbLength;
				}
				MoveThumb(_value, AllowScrollingAnimations);
			}
		}

		[Description("Sets the distance between the thumb and the ScrollBar's edges.")]
		[Category("x Properties")]
		public virtual int ThumbMargin
		{
			get
			{
				return _thumbMargin;
			}
			set
			{
				_thumbMargin = value;
				Refresh();
			}
		}

		[Browsable(false)]
		[Category("x Properties")]
		[Description("Sets the minimum length of the ScrollBar's thumb.")]
		public virtual int MinimumThumbLength
		{
			get
			{
				return _minimumThumbLength;
			}
			set
			{
				_minimumThumbLength = value;
			}
		}

		[Description("Sets the duration the ScrollBar will wait before it shrinks back when focus is lost.")]
		[Browsable(false)]
		[Category("x Properties")]
		public virtual int DurationBeforeShrink
		{
			get
			{
				return _durationBeforeShrink;
			}
			set
			{
				_durationBeforeShrink = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the shrink-size limit of the ScrollBar when focus is lost.")]
		public virtual int ShrinkSizeLimit
		{
			get
			{
				return _shrinkSizeLimit;
			}
			set
			{
				try
				{
					if (value < 0 || value > base.Width)
						throw new ArgumentOutOfRangeException("The shrink-size limit is outside the appropriate range, that is, between 0 and the ScrollBar's Width.");
					_shrinkSizeLimit = value;
				}
				catch (Exception)
				{
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the ScrollBar will automatically shrink when focus is lost.")]
		public virtual bool AllowShrinkingOnFocusLost
		{
			get
			{
				return _allowShrinkingOnFocusLost;
			}
			set
			{
				_allowShrinkingOnFocusLost = value;
			}
		}

		[Description("Sets a value indicating whether the ScrollBar will allow changes to the cursor during the thumb's movement.")]
		[Category("x Properties")]
		public virtual bool AllowCursorChanges
		{
			get
			{
				return _allowCursorChanges;
			}
			set
			{
				_allowCursorChanges = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the ScrollBar will allow displaying of the scroll options menu on right-clicking it.")]
		public virtual bool AllowScrollOptionsMenu
		{
			get
			{
				return _allowScrollOptionsMenu;
			}
			set
			{
				_allowScrollOptionsMenu = value;
			}
		}

		[Description("Sets a value indicating whether the ScrollBar will allow detection of the standard scroll navigation keys which include the Up/Down arrow keys and the Page-up/Page-down keys.")]
		[Category("x Properties")]
		public virtual bool AllowScrollKeysDetection
		{
			get
			{
				return _allowScrollKeysDetection;
			}
			set
			{
				_allowScrollKeysDetection = value;
			}
		}

		[Description("Sets a value indicating whether the ScrollBar will allow animations whenever the thumb is moved.")]
		[Category("x Properties")]
		public virtual bool AllowScrollingAnimations
		{
			get
			{
				return _allowScrollingAnimations;
			}
			set
			{
				_allowScrollingAnimations = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the ScrollBar will allow detection of the Home/End special navigation keys.")]
		public virtual bool AllowHomeEndKeysDetection
		{
			get
			{
				return _allowHomeEndKeysDetection;
			}
			set
			{
				_allowHomeEndKeysDetection = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the ScrollBar will allow incremental thumb movements whenever the scrolling region is clicked at any one point.")]
		public virtual bool AllowIncrementalClickMoves
		{
			get
			{
				return _allowIncrementalClickMoves;
			}
			set
			{
				_allowIncrementalClickMoves = value;
				if (value)
					_allowMouseCaptures = false;
				else if (!value)
				{
					_allowMouseCaptures = true;
				}
			}
		}

		[Description("Sets a value indicating whether the ScrollBar will allow mouse-down effects when active.")]
		[Category("x Properties")]
		public virtual bool AllowMouseDownEffects
		{
			get
			{
				return _allowMouseDownEffects;
			}
			set
			{
				_allowMouseDownEffects = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the ScrollBar will allow mouse-hover effects when active.")]
		public virtual bool AllowMouseHoverEffects
		{
			get
			{
				return _allowMouseHoverEffects;
			}
			set
			{
				_allowMouseHoverEffects = value;
			}
		}

		[Description("Gets a value indicating whether the ScrollBar's options menu is currently visible.")]
		[Browsable(false)]
		[Category("x Properties")]
		public virtual bool ScrollOptionsMenuVisible
		{
			get
			{
				return scroll_options.Visible;
			}
		}

		[Category("x Properties")]
		[Description("Sets the background color of the ScrollBar.")]
		public virtual Color ScrollBarColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
				_tempScrollBarColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the border color of the ScrollBar.")]
		public virtual Color ScrollBarBorderColor
		{
			get
			{
				return _scrollbarBorderColor;
			}
			set
			{
				_scrollbarBorderColor = value;
				_tempScrollBarBorderColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the background color of the thumb.")]
		public virtual Color ThumbColor
		{
			get
			{
				return _thumbColor;
			}
			set
			{
				_thumbColor = value;
				_tempThumbColor = value;
				Refresh();
			}
		}

		[Description("Sets the default thumb size.")]
		[Browsable(true)]
		[Category("x Properties")]
		public virtual xVSlider.ThumbSizes ThumbSize
		{
			get
			{
				return _thumbSize;
			}
			set
			{
				_thumbSize = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Orientation Orientation
		{
			get
			{
				return _orientation;
			}
		}

		[Browsable(false)]
		public Rectangle ScrollBarRectangle { get; internal set; }

		[Browsable(false)]
		public virtual bool DrawThickBorder
		{
			get
			{
				return _drawThickBorder;
			}
			set
			{
				_drawThickBorder = value;
				Refresh();
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

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 33554432;
				return createParams;
			}
		}

		[Browsable(false)]
		protected virtual ScrollBarVisualStyles VisualStyle
		{
			get
			{
				return _visualStyle;
			}
			set
			{
				_visualStyle = value;
				if (value == ScrollBarVisualStyles.Slider)
				{
					_allowMouseDownEffects = false;
					_allowMouseHoverEffects = false;
					base.Height = 31;
				}
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual SliderThumbStyles SliderThumbStyle
		{
			get
			{
				return _sliderStyle;
			}
			set
			{
				_sliderStyle = value;
				Refresh();
			}
		}

		[Browsable(false)]
		protected virtual int ThumbBorderThickness
		{
			get
			{
				return _thumbBorderThickness;
			}
			set
			{
				_thumbBorderThickness = value;
				Refresh();
			}
		}

		public virtual Color ThumbFillColor
		{
			get
			{
				return _thumbBorderColor;
			}
			set
			{
				_thumbBorderColor = value;
				Refresh();
			}
		}

		[Browsable(false)]
		protected virtual DrawModes ThumbDrawMode
		{
			get
			{
				return _thumbMode;
			}
			set
			{
				_thumbMode = value;
				Refresh();
			}
		}

		[Description("Occurs whenever the Value property has been changed.")]
		[Category("x Events")]
		public virtual event EventHandler<ValueChangedEventArgs> ValueChanged = null;

		[Description("Occurs whenever the Scroll event has been raised.")]
		[Category("x Events")]
		public new event EventHandler<ScrollEventArgs> Scroll = null;

		[Description("Occurs after the mouse capture is changed.")]
		[Category("x Events")]
		public new event EventHandler<MouseCaptureChangedEventArgs> MouseCaptureChanged = null;

		public xVScrollBar()
		{
			InitializeComponent();
			SuspendLayout();
			base.Controls.Add(_thumb);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			_shrinkSizeLimit = 3;
			_allowCursorChanges = true;
			_durationBeforeShrink = 2000;
			_allowMouseDownEffects = true;
			_allowMouseHoverEffects = true;
			_allowScrollOptionsMenu = true;
			_allowScrollingAnimations = true;
			_allowScrollKeysDetection = true;
			_allowShrinkingOnFocusLost = false;
			_allowIncrementalClickMoves = true;
			_backColor = Color.Thistle;
			_thumbColor = Color.MediumOrchid;
			_scrollbarBorderColor = Color.Thistle;
			_tempThumbColor = _thumbColor;
			_tempScrollBarColor = _backColor;
			_tempScrollBarBorderColor = _scrollbarBorderColor;
			_onDisable.ThumbColor = Color.Silver;
			_onDisable.ScrollBarColor = Color.Transparent;
			_onDisable.ScrollBarBorderColor = Color.Silver;
			_direction = DirectionalMovements.BottomUp;
			_thumbStyle = ThumbStyles.Inset;
			_minimumThumbLength = 18;
			_borderThickness = 1;
			_borderRadius = 1;
			_thumbLength = 20;
			_thumbMargin = 1;
			_largeChange = 10;
			_smallChange = 1;
			_maximum = 100;
			_minimum = 0;
			_value = 0;
			base.Size = new Size(17, 200);
			Refresh();
			ResumeLayout(false);
			try
			{
				_thumb.Move += delegate
				{
					Invalidate();
				};
			}
			catch (Exception)
			{
			}
		}

		private void OnTickCounter(object sender, EventArgs e)
		{
			_thumbMovementStartDuration++;
			if (_thumbMovementStartDuration > 5 && IsMouseInScrollBar())
			{
				_isOnKeyDown = true;
				HandleMouseCapture(true);
			}
		}

		private void OnLoadParentForm(object sender, EventArgs e)
		{
			if (_allowShrinkingOnFocusLost)
				AutoShrinkScrollBar();
			BindToContainerControl();
		}

		private void OnAnimateGrowthTransitionCompleted(object sender, Transition.Args e)
		{
			_isGrowing = false;
			_autoHidden = false;
			Refresh();
		}

		private void OnAnimateShrinkTransitionCompleted(object sender, Transition.Args e)
		{
			_isShrinking = false;
			_autoHidden = true;
			Refresh();
		}

		private void OnCloseScrollOptions(object sender, ToolStripDropDownClosedEventArgs e)
		{
			if (_allowShrinkingOnFocusLost)
				AutoShrinkScrollBar();
		}

		private void OnClickScrollHere(object sender, EventArgs e)
		{
			bool allowIncrementalClickMoves = AllowIncrementalClickMoves;
			AllowIncrementalClickMoves = false;
			HandleMouseCapture();
			AllowIncrementalClickMoves = allowIncrementalClickMoves;
		}

		private void OnClickScrollToTop(object sender, EventArgs e)
		{
			Value = Maximum;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		private void OnClickScrollToBottom(object sender, EventArgs e)
		{
			Value = Minimum;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		private void OnClickPageUp(object sender, EventArgs e)
		{
			Value += _largeChange;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		private void OnClickPageDown(object sender, EventArgs e)
		{
			Value -= _largeChange;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		private void OnClickScrollUp(object sender, EventArgs e)
		{
			Value++;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		private void OnClickScrollDown(object sender, EventArgs e)
		{
			Value--;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				base.ParentForm.Load += OnLoadParentForm;
			}
			catch (Exception)
			{
			}
		}

		protected override void OnDockChanged(EventArgs e)
		{
			base.OnDockChanged(e);
			Refresh();
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			Refresh();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (_allowMouseHoverEffects)
				PerformMouseEffect(EffectType.OnEnter);
			if (_allowShrinkingOnFocusLost && _autoHidden)
				AutoGrowScrollBar();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (_allowMouseHoverEffects)
				PerformMouseEffect(EffectType.OnLeave);
			if (_allowShrinkingOnFocusLost)
				AutoShrinkScrollBar();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (base.Capture && e.Button == MouseButtons.Left && !_thumbIsMovingInTimer)
			{
				if (AllowCursorChanges && IsMouseInThumb())
					Cursor = Cursors.Hand;
				SetScrollBounds();
				_thumb.Top = e.Y - _mouseY;
				if (_thumb.Bottom >= _bottomScrollbarHeightBound)
					_thumb.Top = _bottomScrollbarHeightBound - _thumb.Height + (_thicknessBound - 1);
				if (_thumb.Top <= _topScrollbarHeightBound)
					_thumb.Top = _topScrollbarHeightBound;
				_value = ConvertRange(_bottomScrollbarHeightBound - _thumb.Height, _topScrollbarHeightBound, Minimum, Maximum, _thumb.Top);
				_thumbIsMoving = true;
				if (this.Scroll != null)
					this.Scroll(this, new ScrollEventArgs(_value));
				if (this.ValueChanged != null)
					this.ValueChanged(this, new ValueChangedEventArgs(_value));
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			AllowShrinkingOnFocusLost = _shrinkingEnabled;
			if (base.Capture && e.Button == MouseButtons.Left)
			{
				_thumbIsMoving = false;
				_mouseY = e.Y - _thumb.Top;
				base.Capture = false;
				EndThumbMovement();
				if (AllowCursorChanges)
					Cursor = Cursors.Default;
				if (_allowMouseDownEffects)
					PerformMouseEffect(EffectType.OnLeave);
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			_shrinkingEnabled = _allowShrinkingOnFocusLost;
			AllowShrinkingOnFocusLost = false;
			if (e.Button == MouseButtons.Left)
			{
				base.Capture = true;
				_mouseY = e.Y - _thumb.Top;
				if (AllowCursorChanges && IsMouseInThumb())
					Cursor = Cursors.Hand;
				if (!IsMouseInThumb())
					BeginThumbMovement();
				if (_allowMouseDownEffects)
					PerformMouseEffect(EffectType.OnPress);
			}
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);
			if (!IsInDesignMode() && IsMouseInScrollBar() && _leftMouseWasClicked && _allowMouseCaptures)
				HandleMouseCapture();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Left)
				_leftMouseWasClicked = true;
			else if (e.Button == MouseButtons.Right)
			{
				_leftMouseWasClicked = false;
			}
			if (AllowIncrementalClickMoves)
				_allowMouseCaptures = true;
			else
				_allowMouseCaptures = false;
			if (e.Button == MouseButtons.Left && !_allowMouseCaptures)
			{
				SetScrollBounds();
				_thumb.Top = e.Y - _mouseY;
				if (_thumb.Bottom >= _bottomScrollbarHeightBound)
					_thumb.Top = _bottomScrollbarHeightBound - _thumb.Height + (_thicknessBound - 1);
				if (_thumb.Top <= _topScrollbarHeightBound)
					_thumb.Top = _topScrollbarHeightBound;
				if (!IsPointInRect(Control.MousePosition, _thumb.DisplayRectangle) && !_thumbIsMoving && !AllowIncrementalClickMoves)
				{
					_clickedPosition = ConvertRange(_bottomScrollbarHeightBound - _thumb.Height + 1, _topScrollbarHeightBound, Minimum, Maximum, e.Y - (_thumbLength / 2 - _thicknessBound * 2));
					Value = _clickedPosition;
				}
				if (this.Scroll != null)
					this.Scroll(this, new ScrollEventArgs(_value));
			}
			else if (e.Button == MouseButtons.Right && AllowScrollOptionsMenu)
			{
				scroll_options.Show(Cursor.Position);
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (e.Delta > 0)
				Value += _largeChange;
			if (e.Delta < 0)
				Value -= _largeChange;
			if (_allowMouseHoverEffects)
				PerformMouseEffect(EffectType.OnLeave);
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
			if (this.ValueChanged != null)
				this.ValueChanged(this, new ValueChangedEventArgs(_value));
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if (AllowScrollKeysDetection)
			{
				if (e.KeyCode == Keys.Up)
				{
					_isOnKeyDown = true;
					_raisesScroll = true;
					Value -= SmallChange;
				}
				else if (e.KeyCode == Keys.Down)
				{
					_isOnKeyDown = true;
					_raisesScroll = true;
					Value += SmallChange;
				}
				else if (e.KeyCode == Keys.Prior)
				{
					_isOnKeyDown = true;
					_raisesScroll = true;
					Value -= LargeChange;
				}
				else if (e.KeyCode == Keys.Next)
				{
					_isOnKeyDown = true;
					_raisesScroll = true;
					Value += LargeChange;
				}
			}
			if (AllowHomeEndKeysDetection)
			{
				if (e.KeyCode == Keys.Home)
				{
					_raisesScroll = true;
					Value = Minimum;
					Refresh();
				}
				else if (e.KeyCode == Keys.End)
				{
					_raisesScroll = true;
					Value = Maximum;
					Refresh();
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			_isOnKeyDown = false;
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (!_isShrinking || !_isGrowing)
			{
				SetScrollBounds();
				Refresh();
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (AllowScrollKeysDetection)
			{
				if ((keyData == Keys.Tab) | (Control.ModifierKeys == Keys.Shift))
					return base.ProcessDialogKey(keyData);
				OnKeyDown(new KeyEventArgs(keyData));
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		public override void Refresh()
		{
			SetScrollBounds();
			ThumbLength = _bottomScrollbarHeightBound * _largeChange / (Maximum - Minimum + 1);
			if (base.Enabled)
			{
				_thumbColor = _tempThumbColor;
				_backColor = _tempScrollBarColor;
				_scrollbarBorderColor = _tempScrollBarBorderColor;
			}
			else
			{
				_thumbColor = OnDisable.ThumbColor;
				_backColor = OnDisable.ScrollBarColor;
				_scrollbarBorderColor = OnDisable.ScrollBarBorderColor;
			}
			DrawScrollBar();
		}

		public void BindTo(Control control)
		{
			try
			{
				if (!IsControlScrollBindable(control) || control == null)
					return;
				string text = control.GetType().ToString();
				if (!text.Contains("DataGridView"))
				{
					ScrollableControl scrollableControl = (ScrollableControl)control;
					scrollableControl.AutoScroll = true;
					LargeChange = scrollableControl.VerticalScroll.LargeChange;
					Maximum = scrollableControl.VerticalScroll.Maximum - scrollableControl.VerticalScroll.LargeChange;
					scrollableControl.AutoScroll = false;
					scrollableControl.MouseWheel += delegate(object sid, MouseEventArgs args)
					{
						OnMouseWheel(args);
					};
					Scroll += delegate(object sid, ScrollEventArgs args)
					{
						scrollableControl.AutoScrollPosition = new Point(scrollableControl.AutoScrollPosition.X, args.Value);
					};
				}
				else
				{
					if (!text.Contains("DataGridView"))
						return;
					DataGridView dataGridView = (DataGridView)control;
					Maximum = dataGridView.RowCount;
					dataGridView.ScrollBars = ScrollBars.None;
					dataGridView.MouseWheel += delegate(object sid, MouseEventArgs args)
					{
						OnMouseWheel(args);
					};
					Scroll += delegate
					{
						try
						{
							dataGridView.CurrentCell = dataGridView.Rows[Value - 1].Cells[0];
							dataGridView.CurrentCell.Selected = _allowDataGridViewSelections;
						}
						catch (Exception)
						{
						}
					};
					dataGridView.RowsAdded += delegate
					{
						Maximum = dataGridView.RowCount;
					};
					dataGridView.RowsRemoved += delegate
					{
						Maximum = dataGridView.RowCount;
					};
				}
			}
			catch (Exception)
			{
			}
		}

		public void BindTo(ScrollableControl scrollableControl)
		{
			BindTo(scrollableControl);
		}

		public void BindTo(DataGridView dataGridView, bool allowSelection)
		{
			_allowDataGridViewSelections = allowSelection;
			BindTo(dataGridView);
		}

		private void DrawScrollBar()
		{
			try
			{
				SetScrollBounds();
				_thumb.Enabled = false;
				int borderRadius = BorderRadius;
				int num = (_thicknessBound = BorderThickness);
				Bitmap bitmap = new Bitmap(base.Size.Width, base.Size.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.Clear(BackColor);
				Rectangle rectangle = ((borderRadius <= 1) ? new Rectangle(DisplayRectangle.X, DisplayRectangle.Y, base.Width - 1, base.Height - 1) : ((num <= 1) ? new Rectangle(DisplayRectangle.X + (int)Math.Floor(Convert.ToDouble(num / 2)), DisplayRectangle.Y + (int)Math.Floor(Convert.ToDouble(num / 2)), base.Width - num, base.Height - num) : ((num % 2 != 0) ? new Rectangle(DisplayRectangle.X + num / 2, DisplayRectangle.Y + num / 2, base.Width - num, base.Height - num) : new Rectangle(DisplayRectangle.X + num / 2, DisplayRectangle.Y + num / 2, base.Width - num - 1, base.Height - num - 1))));
				if (VisualStyle == ScrollBarVisualStyles.Slider)
				{
					int num2 = 2;
					num2 = ((!DrawThickBorder) ? 1 : 2);
					rectangle = new Rectangle(DisplayRectangle.Width / 2 - num2, DisplayRectangle.Y + num / 2, num2, base.Height - num);
				}
				ScrollBarRectangle = rectangle;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				DrawLayout(graphics, rectangle, borderRadius);
				BackgroundImage = bitmap;
				if (_visualStyle == ScrollBarVisualStyles.Slider)
				{
					if (SliderThumbStyle == SliderThumbStyles.Circular)
						_thumb.Height = 21;
					else if (SliderThumbStyle == SliderThumbStyles.Thin)
					{
						_thumb.Height = 3;
					}
				}
				DrawThumb(ref graphics, new Pen(new SolidBrush(ThumbColor), num), rectangle, num, _thumbLength);
				MoveThumb(_value, AllowScrollingAnimations);
				graphics.Dispose();
			}
			catch (Exception)
			{
			}
		}

		private void DrawThumb(ref Graphics graphics, Pen thumbPen, Rectangle controlRectangle, int thickness, int height)
		{
			if (thickness * 2 + 2 > base.Width)
				thickness = 1;
			Bitmap image = null;
			try
			{
				image = new Bitmap(base.Width - (thickness * 2 + 2), _thumb.Height);
			}
			catch (Exception)
			{
			}
			if (_thumbStyle == ThumbStyles.Inset)
			{
				_thicknessBound = _thumbMargin;
				_thumb.Left = thickness + _thumbMargin;
				_thumb.Width = base.Width - (_thumbMargin + 1) - (thickness * 2 + (_thumbMargin - 1));
			}
			else if (_thumbStyle == ThumbStyles.Proportional)
			{
				_thicknessBound -= _thicknessBound + 1;
				_thumb.Left = thickness - 1;
				_thumb.Width = base.Width - thickness + 1;
			}
			if (_thumb.Width <= 0)
			{
				_thicknessBound = thickness + base.Width / 2 - 1;
				_thumb.Left = thickness + base.Width / 2 - 1;
				_thumb.Width = 1;
			}
			using (graphics = Graphics.FromImage(image))
			{
				if (VisualStyle == ScrollBarVisualStyles.Slider)
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					RectangleF rect = new RectangleF(new Point((_thumb.Width - 17) / 2 - 1, 3), new Size(16, 16));
					Rectangle rect2 = new Rectangle(new Point((_thumb.Width - 17) / 2 - 1, 2), new Size(16, 2));
					if (_thumbSize == xVSlider.ThumbSizes.Large)
					{
						rect = new RectangleF(new Point((_thumb.Width - 17) / 2 - 1, 3), new Size(16, 16));
						rect2 = new Rectangle(new Point((_thumb.Width - 17) / 2 - 1, 2), new Size(16, 2));
					}
					else if (_thumbSize == xVSlider.ThumbSizes.Medium)
					{
						rect = new RectangleF(new Point((_thumb.Width - 17) / 2, 5), new Size(14, 14));
						rect2 = new Rectangle(new Point((_thumb.Width - 17) / 2, 5), new Size(14, 2));
					}
					else if (_thumbSize == xVSlider.ThumbSizes.Small)
					{
						rect = new RectangleF(new Point((_thumb.Width - 17) / 2 + 1, 7), new Size(12, 12));
						rect2 = new Rectangle(new Point((_thumb.Width - 17) / 2 + 1, 7), new Size(12, 2));
					}
					thumbPen.Width = _thumbBorderThickness;
					if (ThumbDrawMode == DrawModes.Outline)
						thumbPen.Width = _thumbBorderThickness;
					else if (ThumbDrawMode == DrawModes.Fill)
					{
						thumbPen.Width = _thumbBorderThickness;
					}
					SolidBrush brush = new SolidBrush(ThumbColor);
					if (SliderThumbStyle == SliderThumbStyles.Circular)
						graphics.DrawEllipse(thumbPen, rect);
					else if (SliderThumbStyle == SliderThumbStyles.Thin)
					{
						SolidBrush brush2 = new SolidBrush(ThumbColor);
						Pen pen = new Pen(brush2);
						pen.Width = 3f;
						graphics.DrawRectangle(pen, rect2);
					}
					if (ThumbDrawMode == DrawModes.Fill)
					{
						thumbPen.Width = _thumbBorderThickness;
						graphics.FillEllipse(brush, rect);
					}
					else
					{
						Pen pen2 = new Pen(ThumbColor);
						pen2.Width = 3f;
						SolidBrush brush3 = new SolidBrush(ThumbFillColor);
						graphics.DrawEllipse(pen2, rect);
						graphics.FillEllipse(brush3, rect);
					}
				}
			}
			_thumb.Image = image;
			if (VisualStyle == ScrollBarVisualStyles.ScrollBar)
				_thumb.BackColor = thumbPen.Color;
			else if (VisualStyle == ScrollBarVisualStyles.Slider)
			{
				_thumb.BackColor = Color.Transparent;
			}
			_thumb.SizeMode = PictureBoxSizeMode.StretchImage;
			thumbPen.Dispose();
		}

		private void DrawTopArrow(ref Graphics graphics, Pen arrowPen, Rectangle controlRectangle, int thickness)
		{
			if (thickness * 2 + 2 > base.Width)
				thickness = 1;
			Bitmap image = null;
			try
			{
				image = new Bitmap(base.Width - (thickness * 2 + 2), _topArrow.Height);
			}
			catch (Exception)
			{
			}
			if (_thumbStyle == ThumbStyles.Inset)
			{
				_topArrow.Left = thickness + _thumbMargin;
				_topArrow.Width = base.Width - (_thumbMargin + 1) - (thickness * 2 + (_thumbMargin - 1));
			}
			else if (_thumbStyle == ThumbStyles.Proportional)
			{
				_topArrow.Left = thickness - 1;
				_topArrow.Size = new Size(base.Width - thickness, 20);
			}
			if (_thumb.Width <= 0)
			{
				_topArrow.Left = thickness + base.Width / 2 - 1;
				_topArrow.Width = 1;
			}
			using (graphics = Graphics.FromImage(image))
			{
			}
			_topArrow.Image = image;
			_topArrow.Height = 20;
			_topArrow.Top = thickness + _thumbMargin;
			_topArrow.SizeMode = PictureBoxSizeMode.StretchImage;
			arrowPen.Dispose();
		}

		private void DrawArrow(Graphics graphics, Rectangle controlRectangle, ArrowType type, ArrowStyle style, Color arrowColor)
		{
		}

		private Bitmap DrawArrow(ref Graphics graphics, Pen arrowPen, ArrowType type, Rectangle controlRectangle, int thickness, int width, int height)
		{
			Bitmap bitmap = new Bitmap(width, height);
			arrowPen.StartCap = LineCap.ArrowAnchor;
			arrowPen.EndCap = LineCap.NoAnchor;
			using (graphics = Graphics.FromImage(bitmap))
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				using (arrowPen)
				{
					if (controlRectangle.Width % 2 == 0)
					{
						Point point = new Point(controlRectangle.Left + controlRectangle.Width / 2, (int)((double)(controlRectangle.Bottom - controlRectangle.Bottom / 2) + (double)controlRectangle.Bottom * 0.25) - 1);
						Point point2 = new Point((int)((double)controlRectangle.Width - (double)controlRectangle.Width * 0.5 - (double)controlRectangle.Width * 0.25) + thickness - 1, controlRectangle.Top + controlRectangle.Height / 2 + thickness - 2);
						Point point3 = new Point((int)((double)controlRectangle.Right - (double)controlRectangle.Width * 0.2) - thickness + 2, (int)((double)controlRectangle.Top + (double)controlRectangle.Height * 0.2) + thickness - 1);
						graphics.DrawPolygon(arrowPen, new Point[4] { point, point2, point, point3 });
						graphics.DrawLine(arrowPen, controlRectangle.Width / 2 - 2, 2, 10, 10);
						graphics.DrawLine(arrowPen, (int)((double)controlRectangle.X - (double)controlRectangle.X / 0.75), (int)((double)controlRectangle.Y - (double)controlRectangle.Y / 0.75), 15, 30);
					}
					else
					{
						graphics.DrawLine(arrowPen, base.Width / 2, 4, 15, 15);
						graphics.DrawLine(arrowPen, controlRectangle.Width / 2, controlRectangle.Top + 4, 15, 15);
					}
				}
			}
			arrowPen.Dispose();
			return bitmap;
		}

		private GraphicsPath DrawRoundedRectangle(RectangleF rect, float xradius, float yradius, bool round_ul, bool round_ur, bool round_lr, bool round_ll)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			PointF pt;
			if (round_ul)
			{
				RectangleF rect2 = new RectangleF(rect.X, rect.Y, 2f * xradius, 2f * yradius);
				graphicsPath.AddArc(rect2, 180f, 90f);
				pt = new PointF(rect.X + xradius, rect.Y);
			}
			else
				pt = new PointF(rect.X, rect.Y);
			PointF pt2 = ((!round_ur) ? new PointF(rect.Right, rect.Y) : new PointF(rect.Right - xradius, rect.Y));
			graphicsPath.AddLine(pt, pt2);
			if (round_ur)
			{
				RectangleF rect3 = new RectangleF(rect.Right - 2f * xradius, rect.Y, 2f * xradius, 2f * yradius);
				graphicsPath.AddArc(rect3, 270f, 90f);
				pt = new PointF(rect.Right, rect.Y + yradius);
			}
			else
				pt = new PointF(rect.Right, rect.Y);
			pt2 = ((!round_lr) ? new PointF(rect.Right, rect.Bottom) : new PointF(rect.Right, rect.Bottom - yradius));
			graphicsPath.AddLine(pt, pt2);
			if (round_lr)
			{
				RectangleF rect4 = new RectangleF(rect.Right - 2f * xradius, rect.Bottom - 2f * yradius, 2f * xradius, 2f * yradius);
				graphicsPath.AddArc(rect4, 0f, 90f);
				pt = new PointF(rect.Right - xradius, rect.Bottom);
			}
			else
				pt = new PointF(rect.Right, rect.Bottom);
			pt2 = ((!round_ll) ? new PointF(rect.X, rect.Bottom) : new PointF(rect.X + xradius, rect.Bottom));
			graphicsPath.AddLine(pt, pt2);
			if (round_ll)
			{
				RectangleF rect5 = new RectangleF(rect.X, rect.Bottom - 2f * yradius, 2f * xradius, 2f * yradius);
				graphicsPath.AddArc(rect5, 90f, 90f);
				pt = new PointF(rect.X, rect.Bottom - yradius);
			}
			else
				pt = new PointF(rect.X, rect.Bottom);
			pt2 = ((!round_ul) ? new PointF(rect.X, rect.Y) : new PointF(rect.X, rect.Y + yradius));
			graphicsPath.AddLine(pt, pt2);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private void DrawLayout(Graphics graphics, Rectangle Bounds, int radius)
		{
			graphics.Clear(Color.Transparent);
			Pen pen = new Pen(ScrollBarBorderColor, BorderThickness);
			pen.Alignment = PenAlignment.Inset;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			GraphicsPath graphicsPath = new GraphicsPath();
			SolidBrush solidBrush = new SolidBrush(ScrollBarColor);
			graphicsPath.AddRectangle(new Rectangle(Bounds.X, Bounds.Y + 3, Bounds.Width, Bounds.Height - 6));
			graphicsPath.CloseAllFigures();
			graphics.FillPath(solidBrush, graphicsPath);
			graphics.DrawPath(pen, graphicsPath);
			solidBrush.Dispose();
			pen.Dispose();
			graphicsPath.Dispose();
			graphics.Dispose();
		}

		private void HandleMouseCapture(bool isInTimerMode = false)
		{
			_clickedPosition = ConvertRange(_bottomScrollbarHeightBound - _thumb.Height + 1, _topScrollbarHeightBound, Minimum, Maximum, PointToClient(Control.MousePosition).Y - (_thumbLength / 2 - _thicknessBound * 2));
			if (AllowIncrementalClickMoves)
			{
				if (_clickedPosition > _value)
				{
					if (!IsMouseInThumb(true) && !_thumbIsMoving)
					{
						if (_thumb.Top - _largeChange < PointToClient(Cursor.Position).Y)
						{
							Value = _clickedPosition;
							_thumbDirection = Direction.Top;
						}
						else
						{
							Value += _largeChange;
							_thumbDirection = Direction.Top;
						}
					}
				}
				else if (_clickedPosition < _value && !IsMouseInThumb(true) && !_thumbIsMoving)
				{
					if (_thumb.Bottom + _largeChange <= PointToClient(Cursor.Position).Y)
					{
						Value -= _largeChange;
						_thumbDirection = Direction.Bottom;
					}
					else
					{
						Value = _clickedPosition;
						_thumbDirection = Direction.Bottom;
					}
				}
			}
			else
				Value = _clickedPosition;
			if (IsMouseInThumb(true))
				_thumbDirection = Direction.InCursor;
			if (this.Scroll != null)
				this.Scroll(this, new ScrollEventArgs(_value));
			if (this.MouseCaptureChanged != null)
				this.MouseCaptureChanged(this, new MouseCaptureChangedEventArgs(_value, _thumbDirection));
		}

		private void BindToContainerControl()
		{
			BindTo(_bindingContainer);
		}

		private bool IsControlScrollBindable(Control control)
		{
			try
			{
				if (control.GetType().IsSubclassOf(typeof(ScrollableControl)) || control is DataGridView)
					return true;
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool IsInDesignMode()
		{
			if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
				return true;
			return false;
		}

		private void DebugString(string value)
		{
			base.ParentForm.Text = value;
		}

		private void BeginThumbMovement(int movementIntervals = 70)
		{
			if (IsMouseInScrollBar())
			{
				_thumbIsMovingInTimer = true;
				Counter.Interval = movementIntervals;
				Counter.Enabled = true;
				Counter.Start();
			}
		}

		private void EndThumbMovement()
		{
			if (_thumbIsMovingInTimer)
			{
				_isOnKeyDown = false;
				_thumbIsMovingInTimer = false;
				_thumbMovementStartDuration = 0;
				Counter.Stop();
				Counter.Enabled = false;
			}
		}

		private void AutoGrowScrollBar()
		{
			if (_autoHidden || base.Width == _shrinkSizeLimit)
			{
				_isGrowing = true;
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(200));
				transition.TransitionCompletedEvent += OnAnimateGrowthTransitionCompleted;
				transition.add(this, "Width", _scrollBarSize);
				transition.run();
			}
		}

		private void AutoShrinkScrollBar()
		{
			if (!_autoHidden)
				_scrollBarSize = base.Width;
			_timer = new System.Threading.Timer(Shrink, null, _durationBeforeShrink, 0);
		}

		private void Shrink(object state)
		{
			bool mouseInScrollBar = false;
			bool scrollOptionsVisible = false;
			Invoke((Action)delegate
			{
				mouseInScrollBar = IsMouseInScrollBar();
				scrollOptionsVisible = scroll_options.Visible;
			});
			if (!mouseInScrollBar && !scrollOptionsVisible && !_thumbIsMoving)
			{
				_isShrinking = true;
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(200));
				transition.TransitionCompletedEvent += OnAnimateShrinkTransitionCompleted;
				transition.add(this, "Width", _shrinkSizeLimit);
				transition.run();
				_autoHidden = true;
				_timer.Dispose();
			}
		}

		private void MoveThumb(int to, bool allowTransition = true)
		{
			SetScrollBounds();
			_readyToFill = false;
			int num = ConvertRange(Minimum, Maximum, _bottomScrollbarHeightBound - _thumb.Height + (_thicknessBound - 1), _topScrollbarHeightBound, to);
			if (ThumbStyle == ThumbStyles.Proportional)
				_topScrollbarHeightBound = BorderThickness;
			if (allowTransition)
			{
				if (AllowScrollingAnimations)
				{
					Transition transition = new Transition(new TransitionType_EaseInEaseOut(270));
					transition.add(_thumb, "Top", num);
					transition.run();
					transition.TransitionCompletedEvent += delegate
					{
						_readyToFill = true;
						Invalidate();
					};
				}
				else
				{
					Transition transition2 = new Transition(new TransitionType_EaseInEaseOut(1));
					transition2.add(_thumb, "Top", num);
					transition2.run();
					transition2.TransitionCompletedEvent += delegate
					{
						_readyToFill = true;
						Invalidate();
					};
				}
			}
			else
			{
				Transition transition3 = new Transition(new TransitionType_EaseInEaseOut(1));
				transition3.add(_thumb, "Top", num);
				transition3.run();
				transition3.TransitionCompletedEvent += delegate
				{
					_readyToFill = true;
					Invalidate();
				};
			}
		}

		private void MoveValue(int to)
		{
			Transition transition = new Transition(new TransitionType_EaseInEaseOut(270));
			transition.add(this, "Value", to);
			transition.run();
		}

		private void PerformMouseEffect(EffectType effect)
		{
			Transition transition = new Transition(new TransitionType_EaseInEaseOut(220));
			switch (effect)
			{
			case EffectType.OnEnter:
				_tempThumbColor = ThumbColor;
				_tempScrollBarColor = ScrollBarColor;
				_tempScrollBarBorderColor = ScrollBarBorderColor;
				if (_allowScrollingAnimations)
				{
					transition.add(_thumb, "BackColor", ControlPaint.Light(_thumb.BackColor, 0.7f));
					transition.run();
				}
				else
					_thumb.BackColor = ControlPaint.Light(_thumb.BackColor, 0.7f);
				break;
			case EffectType.OnPress:
				if (!_allowMouseHoverEffects)
				{
					_tempThumbColor = ThumbColor;
					_tempScrollBarColor = ScrollBarColor;
					_tempScrollBarBorderColor = ScrollBarBorderColor;
				}
				if (_allowScrollingAnimations)
				{
					transition.add(_thumb, "BackColor", ControlPaint.Dark(_thumb.BackColor, 0.2f));
					transition.run();
				}
				else
					_thumb.BackColor = ControlPaint.Dark(_thumb.BackColor, 0.2f);
				break;
			case EffectType.OnLeave:
				if (_allowScrollingAnimations)
				{
					transition.add(_thumb, "BackColor", _tempThumbColor);
					transition.run();
				}
				else
					_thumb.BackColor = _tempThumbColor;
				break;
			}
		}

		private void SetThumbLength()
		{
			SetScrollBounds();
			ThumbLength = _bottomScrollbarHeightBound * _largeChange / (Maximum - Minimum + 1);
			Refresh();
		}

		private void SetScrollBounds()
		{
			if (_thumbStyle == ThumbStyles.Inset)
			{
				_bottomScrollbarHeightBound = base.Height - _thicknessBound * 2 - BorderThickness + 1 + 1;
				_bottomThumbHeightBound = _thumb.Height - _thicknessBound + 1;
				_topScrollbarHeightBound = _thicknessBound + BorderThickness - 4;
			}
			else
			{
				_thicknessBound = 2;
				_bottomScrollbarHeightBound = base.Height - _thicknessBound * 2 - BorderThickness + 4 + 1;
				_bottomThumbHeightBound = _thumb.Height - _thicknessBound + 1;
				_topScrollbarHeightBound = _thicknessBound + BorderThickness - 3 - 4;
			}
		}

		private bool IsPointInRect(Point pt, Rectangle rect)
		{
			if ((pt.X > rect.Left) & (pt.X < rect.Right) & (pt.Y > rect.Top) & (pt.Y < rect.Bottom))
				return true;
			return false;
		}

		private bool IsMouseInThumb(bool applyUniqueCheck = false)
		{
			if (applyUniqueCheck)
				return PointToClient(Control.MousePosition).Y >= _thumb.Top && PointToClient(Control.MousePosition).Y <= _thumb.Bottom;
			return IsPointInRect(_thumb.PointToClient(Control.MousePosition), _thumb.ClientRectangle) && IsPointInRect(PointToClient(Control.MousePosition), base.ClientRectangle);
		}

		private bool IsMouseInScrollBar()
		{
			return IsPointInRect(PointToClient(Control.MousePosition), base.ClientRectangle);
		}

		private int ConvertRange(int originalStart, int originalEnd, int newStart, int newEnd, int value)
		{
			double num = (double)(newEnd - newStart) / (double)(originalEnd - originalStart);
			return (int)((double)newStart + (double)(value - originalStart) * num);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Utilities.xSlider.xVScrollBar));
			this.Counter = new System.Windows.Forms.Timer(this.components);
			this.scroll_options = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.scroll_here = new System.Windows.Forms.ToolStripMenuItem();
			this.separator_one = new System.Windows.Forms.ToolStripSeparator();
			this.scroll_to_top = new System.Windows.Forms.ToolStripMenuItem();
			this.scroll_to_bottom = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.page_up = new System.Windows.Forms.ToolStripMenuItem();
			this.page_down = new System.Windows.Forms.ToolStripMenuItem();
			this.separator_two = new System.Windows.Forms.ToolStripSeparator();
			this.scroll_up = new System.Windows.Forms.ToolStripMenuItem();
			this.scroll_down = new System.Windows.Forms.ToolStripMenuItem();
			this.scroll_options.SuspendLayout();
			base.SuspendLayout();
			this.Counter.Interval = 70;
			this.Counter.Tick += new System.EventHandler(OnTickCounter);
			this.scroll_options.Items.AddRange(new System.Windows.Forms.ToolStripItem[10] { this.scroll_here, this.separator_one, this.scroll_to_top, this.scroll_to_bottom, this.toolStripSeparator1, this.page_up, this.page_down, this.separator_two, this.scroll_up, this.scroll_down });
			this.scroll_options.Name = "scroll_options";
			resources.ApplyResources(this.scroll_options, "scroll_options");
			this.scroll_options.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(OnCloseScrollOptions);
			this.scroll_here.Name = "scroll_here";
			resources.ApplyResources(this.scroll_here, "scroll_here");
			this.scroll_here.Click += new System.EventHandler(OnClickScrollHere);
			this.separator_one.Name = "separator_one";
			resources.ApplyResources(this.separator_one, "separator_one");
			this.scroll_to_top.Name = "scroll_to_top";
			resources.ApplyResources(this.scroll_to_top, "scroll_to_top");
			this.scroll_to_top.Click += new System.EventHandler(OnClickScrollToTop);
			this.scroll_to_bottom.Name = "scroll_to_bottom";
			resources.ApplyResources(this.scroll_to_bottom, "scroll_to_bottom");
			this.scroll_to_bottom.Click += new System.EventHandler(OnClickScrollToBottom);
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
			this.page_up.Name = "page_up";
			resources.ApplyResources(this.page_up, "page_up");
			this.page_up.Click += new System.EventHandler(OnClickPageUp);
			this.page_down.Name = "page_down";
			resources.ApplyResources(this.page_down, "page_down");
			this.page_down.Click += new System.EventHandler(OnClickPageDown);
			this.separator_two.Name = "separator_two";
			resources.ApplyResources(this.separator_two, "separator_two");
			this.scroll_up.Name = "scroll_up";
			resources.ApplyResources(this.scroll_up, "scroll_up");
			this.scroll_up.Click += new System.EventHandler(OnClickScrollUp);
			this.scroll_down.Name = "scroll_down";
			resources.ApplyResources(this.scroll_down, "scroll_down");
			this.scroll_down.Click += new System.EventHandler(OnClickScrollDown);
			resources.ApplyResources(this, "$this");
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Name = "xVScrollBar";
			this.scroll_options.ResumeLayout(false);
			base.ResumeLayout(false);
		}
	}
}
