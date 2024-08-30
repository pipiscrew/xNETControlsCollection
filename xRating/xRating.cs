using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace x.UI.WinForms
{
	[Description("Add awesome rating inputs with tons of customization options and UI enhancements.")]
	[ToolboxBitmap(typeof(RadioButton))]
	[DebuggerStepThrough]
	[Designer(typeof(xDesigner))]
	[DefaultEvent("ValueChanged")]
	[DefaultProperty("Value")]
	[Category("x UI For Windows Forms")]
	public class xRating : Control
	{
		public enum RatingShapes
		{
			Star,
			Circle,
			Square
		}

		public class ValueHoveredEventArgs : EventArgs
		{
			private bool _focused;

			private int _maximum;

			private int _hoveredValue;

			private int _currentValue;

			private Point _position;

			public bool Focused
			{
				get
				{
					return _focused;
				}
			}

			public int Maximum
			{
				get
				{
					return _maximum;
				}
			}

			public int CurrentValue
			{
				get
				{
					return _currentValue;
				}
			}

			public int HoveredValue
			{
				get
				{
					return _hoveredValue;
				}
			}

			public Point Position
			{
				get
				{
					return _position;
				}
			}

			public ValueHoveredEventArgs(bool focused, int currentValue, int hoveredValue, int maximum, Point position)
			{
				_focused = focused;
				_currentValue = currentValue;
				_hoveredValue = hoveredValue;
				_maximum = maximum;
				_position = position;
			}
		}

		public class ValueChangedEventArgs : EventArgs
		{
			private int _value;

			private int _maximum;

			public int Maximum
			{
				get
				{
					return _maximum;
				}
			}

			public int Value
			{
				get
				{
					return _value;
				}
			}

			public ValueChangedEventArgs(int value, int maximum)
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
					xRating xRating = (xRating)base.Control;
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
							new xRatingActionList(base.Component)
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
		public class xRatingActionList : DesignerActionList
		{
			private xRating xControl;

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
				}
			}

			public float OuterRadius
			{
				get
				{
					return xControl.OuterRadius;
				}
				set
				{
					SetValue(xControl, "OuterRadius", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Color BackColor
			{
				get
				{
					return xControl.BackColor;
				}
				set
				{
					SetValue(xControl, "BackColor", value);
				}
			}

			public Color EmptyFillColor
			{
				get
				{
					return xControl.EmptyFillColor;
				}
				set
				{
					SetValue(xControl, "EmptyFillColor", value);
				}
			}

			public Color EmptyBorderColor
			{
				get
				{
					return xControl.EmptyBorderColor;
				}
				set
				{
					SetValue(xControl, "EmptyBorderColor", value);
				}
			}

			public Color HoverFillColor
			{
				get
				{
					return xControl.HoverFillColor;
				}
				set
				{
					SetValue(xControl, "HoverFillColor", value);
				}
			}

			public Color HoverBorderColor
			{
				get
				{
					return xControl.HoverBorderColor;
				}
				set
				{
					SetValue(xControl, "HoverBorderColor", value);
				}
			}

			public Color RatedFillColor
			{
				get
				{
					return xControl.RatedFillColor;
				}
				set
				{
					SetValue(xControl, "RatedFillColor", value);
				}
			}

			public Color RatedBorderColor
			{
				get
				{
					return xControl.RatedBorderColor;
				}
				set
				{
					SetValue(xControl, "RatedBorderColor", value);
				}
			}

			public RatingShapes Shape
			{
				get
				{
					return xControl.Shape;
				}
				set
				{
					SetValue(xControl, "Shape", value);
				}
			}

			public xRatingActionList(IComponent component)
				: base(component)
			{
				xControl = component as xRating;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)          ", "Common Tasks"),
					new DesignerActionPropertyItem("Shape", "Shape:", "Common Tasks", GetPropertyDescriptor(base.Component, "Shape").Description),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("OuterRadius", "OuterRadius:", "Common Tasks", GetPropertyDescriptor(base.Component, "OuterRadius").Description),
					new DesignerActionPropertyItem("BorderThickness", "BorderThickness:", "Appearance", GetPropertyDescriptor(base.Component, "BorderThickness").Description),
					new DesignerActionPropertyItem("BackColor", "BackColor:", "Appearance", GetPropertyDescriptor(base.Component, "BackColor").Description),
					new DesignerActionPropertyItem("EmptyFillColor", "EmptyFillColor:", "Appearance", GetPropertyDescriptor(base.Component, "EmptyFillColor").Description),
					new DesignerActionPropertyItem("EmptyBorderColor", "EmptyBorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "EmptyBorderColor").Description),
					new DesignerActionPropertyItem("RatedFillColor", "RatedFillColor:", "Appearance", GetPropertyDescriptor(base.Component, "RatedFillColor").Description),
					new DesignerActionPropertyItem("RatedBorderColor", "RatedBorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "RatedBorderColor").Description),
					new DesignerActionPropertyItem("HoverFillColor", "HoverFillColor:", "Appearance", GetPropertyDescriptor(base.Component, "HoverFillColor").Description),
					new DesignerActionPropertyItem("HoverBorderColor", "HoverBorderColor:", "Appearance", GetPropertyDescriptor(base.Component, "HoverBorderColor").Description)
				};
			}
		}

		private bool _focused = true;

		private bool _readOnly = false;

		private bool _isPainting = false;

		private bool _rightClickToClear = true;

		private float _outerRadius = 10f;

		private float _innerRadius = 2f;

		private const int DEFAULTVALUE = 0;

		private int _hoveredRating = 0;

		private int _currentRating = 2;

		private int _paintRating;

		private int _spacing = 4;

		private int _maxRating = 5;

		private int _borderThickness = 1;

		private Point _hoverPoint = default(Point);

		private Color _paintColor;

		private Color _paintBorderColor;

		private Color _disabledEmptyFillColor = Color.FromArgb(212, 212, 212);

		private Color _disabledRatedFillColor = Color.DarkGray;

		private Color _emptyFillColor = Color.FromArgb(212, 212, 212);

		private Color _hoverBorderColor = Color.FromArgb(248, 217, 20);

		private Color _hoverFillColor = Color.FromArgb(248, 217, 20);

		private Color _ratedFillColor = Color.FromArgb(248, 217, 20);

		private Color _ratedBorderColor = Color.FromArgb(248, 217, 20);

		private Color _emptyBorderColor = Color.FromArgb(212, 212, 212);

		private RatingShapes _Shape = RatingShapes.Star;

		private IContainer components = null;

		[Category("x Properties")]
		[Description("Sets a value indicating whether the rating is in read-only mode.")]
		public virtual bool ReadOnly
		{
			get
			{
				return _readOnly;
			}
			set
			{
				_readOnly = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the rating will be cleared after right-clicking it.")]
		public virtual bool RightClickToClear
		{
			get
			{
				return _rightClickToClear;
			}
			set
			{
				_rightClickToClear = value;
			}
		}

		[Bindable(true)]
		[Description("Sets the current rating value.")]
		[Category("x Properties")]
		public virtual int Value
		{
			get
			{
				return _currentRating;
			}
			set
			{
				if (value != _currentRating)
				{
					if (value > _maxRating)
						value = _maxRating;
					_currentRating = value;
					EventHandler<ValueChangedEventArgs> valueChanged = this.ValueChanged;
					if (valueChanged != null)
						valueChanged(this, new ValueChangedEventArgs(value, _maxRating));
					_paintRating = value;
					Invalidate();
				}
			}
		}

		[Description("Sets the maximum rating count.")]
		[DefaultValue(5)]
		[Category("x Properties")]
		[RefreshProperties(RefreshProperties.Repaint)]
		public virtual int Maximum
		{
			get
			{
				return _maxRating;
			}
			set
			{
				_maxRating = value;
				Invalidate();
				ResizeControl();
			}
		}

		[RefreshProperties(RefreshProperties.Repaint)]
		[DefaultValue(10)]
		[Category("x Properties")]
		[Description("Sets the rating's shape radius.")]
		public virtual float OuterRadius
		{
			get
			{
				return _outerRadius;
			}
			set
			{
				_outerRadius = value;
				Invalidate();
				ResizeControl();
			}
		}

		[DefaultValue(2)]
		[Description("Sets the inner radius of the star shape.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Category("x Properties")]
		public virtual float InnerRadius
		{
			get
			{
				return _innerRadius;
			}
			set
			{
				_innerRadius = value;
				Invalidate();
				ResizeControl();
			}
		}

		[Description("Sets the spacing between rating shapes.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Category("x Properties")]
		[DefaultValue(4)]
		public virtual int Spacing
		{
			get
			{
				return _spacing;
			}
			set
			{
				_spacing = value;
				Invalidate();
				ResizeControl();
			}
		}

		[Category("x Properties")]
		[Description("Sets the border thickness of rating shapes.")]
		[DefaultValue(1)]
		public virtual int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				_borderThickness = value;
				Invalidate();
				ResizeControl();
			}
		}

		[Category("x Properties")]
		[Description("Sets the rating shape style.")]
		[DefaultValue(RatingShapes.Star)]
		[RefreshProperties(RefreshProperties.Repaint)]
		public virtual RatingShapes Shape
		{
			get
			{
				return _Shape;
			}
			set
			{
				_Shape = value;
				Invalidate();
				ResizeControl();
			}
		}

		[Category("x Properties")]
		[Description("Sets the fill color of unrated shapes when disabled.")]
		public virtual Color DisabledEmptyFillColor
		{
			get
			{
				return _disabledEmptyFillColor;
			}
			set
			{
				_disabledEmptyFillColor = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the fill color of rated shapes when disabled.")]
		public virtual Color DisabledRatedFillColor
		{
			get
			{
				return _disabledRatedFillColor;
			}
			set
			{
				_disabledRatedFillColor = value;
				Invalidate();
			}
		}

		[Description("Sets the fill color of unrated shapes.")]
		[Category("x Properties")]
		public virtual Color EmptyFillColor
		{
			get
			{
				return _emptyFillColor;
			}
			set
			{
				_emptyFillColor = value;
				Invalidate();
			}
		}

		[Description("Sets the border color of unrated shapes.")]
		[Category("x Properties")]
		public virtual Color EmptyBorderColor
		{
			get
			{
				return _emptyBorderColor;
			}
			set
			{
				_emptyBorderColor = value;
				Invalidate();
			}
		}

		[Description("Sets the fill color of rating shapes on hover.")]
		[Category("x Properties")]
		public virtual Color HoverFillColor
		{
			get
			{
				return _hoverFillColor;
			}
			set
			{
				_hoverFillColor = value;
				Invalidate();
			}
		}

		[Description("Sets the border color of rating shapes on hover.")]
		[Category("x Properties")]
		public virtual Color HoverBorderColor
		{
			get
			{
				return _hoverBorderColor;
			}
			set
			{
				_hoverBorderColor = value;
				Invalidate();
			}
		}

		[Description("Sets the fill color of rated shapes.")]
		[Category("x Properties")]
		public virtual Color RatedFillColor
		{
			get
			{
				return _ratedFillColor;
			}
			set
			{
				_ratedFillColor = value;
				_paintColor = value;
				Invalidate();
			}
		}

		[Description("Sets the border color of rated shapes.")]
		[Category("x Properties")]
		public virtual Color RatedBorderColor
		{
			get
			{
				return _ratedBorderColor;
			}
			set
			{
				_ratedBorderColor = value;
				_paintBorderColor = value;
				Invalidate();
			}
		}

		[Description("Occurs when the rating value has been changed.")]
		[Category("x Events")]
		public virtual event EventHandler<ValueChangedEventArgs> ValueChanged;

		[Category("x Events")]
		[Description("Occurs when a rating value has been hovered onto.")]
		public virtual event EventHandler<ValueHoveredEventArgs> ValueHovered;

		public xRating()
		{
			SuspendLayout();
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
			Value = _currentRating;
			_paintColor = RatedFillColor;
			_paintRating = Value;
			_paintRating = Value;
			_paintBorderColor = RatedBorderColor;
			ResizeControl();
		}

		public void Clear()
		{
			Value = 0;
		}

		public void Reset()
		{
			Clear();
		}

		public void ResizeControl()
		{
			base.Width = Maximum * (int)_outerRadius * 2 + _spacing * Maximum + _borderThickness;
			base.Height = (int)_outerRadius * 2 + _borderThickness * 2;
		}

		private void DrawShapes(Graphics graphics)
		{
			int i = 0;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			Color color = default(Color);
			Color color2 = default(Color);
			GraphicsPath graphicsPath = new GraphicsPath();
			float value = ((float)base.Height - OuterRadius * 2f) / 2f;
			for (; i != Maximum; i++)
			{
				float value2 = (float)((double)((float)i * (OuterRadius * 2f + (float)Spacing) + (float)base.Padding.Left) + (double)Spacing / 2.0);
				if (_paintRating > i)
				{
					if (_isPainting & (_paintRating != i + 1))
					{
						color = HoverFillColor;
						color2 = HoverBorderColor;
					}
					else if (!_isPainting & (_paintRating == i + 1))
					{
						if (!base.Enabled)
						{
							color = _disabledRatedFillColor;
							color2 = _disabledRatedFillColor;
						}
						else
						{
							color = RatedFillColor;
							color2 = RatedBorderColor;
						}
					}
					else if (!base.Enabled)
					{
						color = _disabledRatedFillColor;
						color2 = _disabledRatedFillColor;
					}
					else
					{
						color = _paintColor;
						color2 = _paintBorderColor;
					}
				}
				else if (!base.Enabled)
				{
					color = _disabledEmptyFillColor;
					color2 = _disabledEmptyFillColor;
				}
				else
				{
					color = EmptyFillColor;
					color2 = EmptyBorderColor;
				}
				graphicsPath.Reset();
				switch (Shape)
				{
				case RatingShapes.Star:
					graphicsPath = DrawStar(value2, value);
					break;
				case RatingShapes.Circle:
					graphicsPath.AddEllipse(value2, value, OuterRadius * 2f, OuterRadius * 2f);
					break;
				case RatingShapes.Square:
					graphicsPath.AddRectangle(new Rectangle(Convert.ToInt32(value2), Convert.ToInt32(value), (int)(OuterRadius * 2f), (int)(OuterRadius * 2f)));
					break;
				}
				graphics.FillPath(new SolidBrush(color), graphicsPath);
				graphics.DrawPath(new Pen(color2, BorderThickness), graphicsPath);
			}
		}

		private GraphicsPath DrawStar(float x, float y)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			x += OuterRadius;
			y += OuterRadius;
			float num = Convert.ToSingle(Math.Sin(Math.PI / 5.0));
			float num2 = Convert.ToSingle(Math.Sin(Math.PI * 2.0 / 5.0));
			float num3 = Convert.ToSingle(Math.Cos(Math.PI / 5.0));
			float num4 = Convert.ToSingle(Math.Cos(Math.PI * 2.0 / 5.0));
			float num5 = OuterRadius * num4 / num3 + InnerRadius;
			graphicsPath.AddPolygon(new PointF[10]
			{
				new PointF(x, y - OuterRadius),
				new PointF(x + num5 * num, y - num5 * num3),
				new PointF(x + OuterRadius * num2, y - OuterRadius * num4),
				new PointF(x + num5 * num2, y + num5 * num4),
				new PointF(x + OuterRadius * num, y + OuterRadius * num3),
				new PointF(x, y + num5),
				new PointF(x - OuterRadius * num, y + OuterRadius * num3),
				new PointF(x - num5 * num2, y + num5 * num4),
				new PointF(x - OuterRadius * num2, y - OuterRadius * num4),
				new PointF(x - num5 * num, y - num5 * num3)
			});
			return graphicsPath;
		}

		private int GetRating(int x)
		{
            int i;
            for (i = 0; !(((float)x >= (float)(Spacing * i) + OuterRadius * 2f * (float)i + (float)base.Padding.Left) & ((float)x <= (float)(Spacing * (i + 1)) + OuterRadius * 2f * (float)(i + 1) + (float)base.Padding.Left)); i++)
            {
            }
            return i + 1;
            
		}

		private bool IsMouseWithinControl()
		{
			if (base.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
				return true;
			return false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawShapes(e.Graphics);
			if (!base.DesignMode)
			{
				if (IsMouseWithinControl())
					_focused = true;
				else
					_focused = false;
				EventHandler<ValueHoveredEventArgs> valueHovered = this.ValueHovered;
				if (valueHovered != null)
					valueHovered(this, new ValueHoveredEventArgs(_focused, _currentRating, _hoveredRating, _maxRating, _hoverPoint));
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (ReadOnly)
				return;
			if (e.Button == MouseButtons.Right)
			{
				if (_rightClickToClear)
					Value = 0;
			}
			else if (((float)e.Y > ((float)base.Height - OuterRadius * 2f) / 2f) & ((float)e.Y < ((float)base.Height - OuterRadius * 2f) / 2f + OuterRadius * 2f - 1f) & (e.X > base.Padding.Left) & ((float)e.X < OuterRadius * 2f * (float)Maximum + (float)(Spacing * Maximum) + (float)base.Padding.Left))
			{
				Value = GetRating(e.X);
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (!ReadOnly)
			{
				_paintRating = Value;
				_isPainting = false;
				_paintBorderColor = RatedBorderColor;
				_paintColor = RatedFillColor;
				Refresh();
			}
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!ReadOnly)
			{
				_hoveredRating = _currentRating;
				_hoverPoint = e.Location;
				if (((float)e.Y > ((float)base.Height - OuterRadius * 2f) / 2f) & ((float)e.Y < ((float)base.Height - OuterRadius * 2f) / 2f + OuterRadius * 2f - 1f) & (e.X > base.Padding.Left) & ((float)e.X < OuterRadius * 2f * (float)Maximum + (float)(Spacing * Maximum) + (float)base.Padding.Left))
				{
					_hoveredRating = GetRating(e.X);
					_paintRating = _hoveredRating;
					_isPainting = true;
					_paintColor = HoverFillColor;
					_paintBorderColor = RatedBorderColor;
				}
				Refresh();
			}
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			ResizeControl();
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.ResumeLayout(false);
		}
	}
}
