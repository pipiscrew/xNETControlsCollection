using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace xCollection
{
	[ToolboxBitmap(typeof(ProgressBar))]
	[Description("Provides customizable circular progress bars for use in value ranges and progressive movement of data.")]
	[Category("x UI For Windows Forms")]
	[Designer(typeof(xDesigner))]
	[DefaultEvent("ProgressChanged")]
	[DefaultProperty("Value")]
	[DebuggerStepThrough]
	public class xCircleProgress : ProgressBar
	{
		public enum CapStyles
		{
			Flat,
			Round,
			Arrow,
			Triangle
		}

		public enum FillStyles
		{
			Solid,
			Gradient
		}

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

		[EditorBrowsable(EditorBrowsableState.Never)]
		[DebuggerStepThrough]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					if (((xCircleProgress)base.Control).AutoSize)
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
			private xCircleProgress xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public bool Animated
			{
				get
				{
					return xControl.Animated;
				}
				set
				{
					SetValue(xControl, "Animated", value);
				}
			}

			public bool IsPercentage
			{
				get
				{
					return xControl.IsPercentage;
				}
				set
				{
					SetValue(xControl, "IsPercentage", value);
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

			public int LineThickness
			{
				get
				{
					return xControl.LineThickness;
				}
				set
				{
					SetValue(xControl, "LineThickness", value);
				}
			}

			public int LineProgressThickness
			{
				get
				{
					return xControl.LineProgressThickness;
				}
				set
				{
					SetValue(xControl, "LineProgressThickness", value);
				}
			}

			public string SubScriptText
			{
				get
				{
					return xControl.SubScriptText;
				}
				set
				{
					SetValue(xControl, "SubScriptText", value);
				}
			}

			public string SuperScriptText
			{
				get
				{
					return xControl.SuperScriptText;
				}
				set
				{
					SetValue(xControl, "SuperScriptText", value);
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

			public Font SecondaryFont
			{
				get
				{
					return xControl.SecondaryFont;
				}
				set
				{
					SetValue(xControl, "SecondaryFont", value);
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

			public Color SubScriptColor
			{
				get
				{
					return xControl.SubScriptColor;
				}
				set
				{
					SetValue(xControl, "SubScriptColor", value);
				}
			}

			public Color SuperScriptColor
			{
				get
				{
					return xControl.SuperScriptColor;
				}
				set
				{
					SetValue(xControl, "SuperScriptColor", value);
				}
			}

			public Color ProgressColor
			{
				get
				{
					return xControl.ProgressColor;
				}
				set
				{
					SetValue(xControl, "ProgressColor", value);
				}
			}

			public Color ProgressColor2
			{
				get
				{
					return xControl.ProgressColor2;
				}
				set
				{
					SetValue(xControl, "ProgressColor2", value);
				}
			}

			public Color ProgressBackColor
			{
				get
				{
					return xControl.ProgressBackColor;
				}
				set
				{
					SetValue(xControl, "ProgressBackColor", value);
				}
			}

			public Padding ValueMargin
			{
				get
				{
					return xControl.ValueMargin;
				}
				set
				{
					SetValue(xControl, "ValueMargin", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Padding SubScriptMargin
			{
				get
				{
					return xControl.SubScriptMargin;
				}
				set
				{
					SetValue(xControl, "SubScriptMargin", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public Padding SuperScriptMargin
			{
				get
				{
					return xControl.SuperScriptMargin;
				}
				set
				{
					SetValue(xControl, "SuperScriptMargin", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public FillStyles ProgressFillStyle
			{
				get
				{
					return xControl.ProgressFillStyle;
				}
				set
				{
					SetValue(xControl, "ProgressFillStyle", value);
				}
			}

			public CapStyles ProgressStartCap
			{
				get
				{
					return xControl.ProgressStartCap;
				}
				set
				{
					SetValue(xControl, "ProgressStartCap", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public CapStyles ProgressEndCap
			{
				get
				{
					return xControl.ProgressEndCap;
				}
				set
				{
					SetValue(xControl, "ProgressEndCap", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public xControlDesignerActionList(IComponent component)
				: base(component)
			{
				xControl = component as xCircleProgress;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)                             ", "Common Tasks"),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Minimum", "Minimum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Minimum").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("Animated", "Animated", "Common Tasks", GetPropertyDescriptor(base.Component, "Animated").Description),
					new DesignerActionPropertyItem("IsPercentage", "IsPercentage (Uses Superscript as a percentage)", "Common Tasks", GetPropertyDescriptor(base.Component, "IsPercentage").Description),
					new DesignerActionPropertyItem("Font", "Font:", "Appearance", GetPropertyDescriptor(base.Component, "Font").Description),
					new DesignerActionPropertyItem("SecondaryFont", "SecondaryFont:", "Appearance", GetPropertyDescriptor(base.Component, "SecondaryFont").Description),
					new DesignerActionPropertyItem("ProgressFillStyle", "ProgressFillStyle:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressFillStyle").Description),
					new DesignerActionPropertyItem("ProgressColor", "ProgressColor:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColor").Description),
					new DesignerActionPropertyItem("ProgressColor2", "ProgressColor2:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColor2").Description),
					new DesignerActionPropertyItem("ProgressBackColor", "ProgressBackColor:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressBackColor").Description),
					new DesignerActionPropertyItem("ForeColor", "ForeColor:", "Appearance", GetPropertyDescriptor(base.Component, "ForeColor").Description),
					new DesignerActionPropertyItem("SuperScriptColor", "SuperScriptColor:", "Appearance", GetPropertyDescriptor(base.Component, "SuperScriptColor").Description),
					new DesignerActionPropertyItem("SubScriptColor", "SubScriptColor:", "Appearance", GetPropertyDescriptor(base.Component, "SubScriptColor").Description),
					new DesignerActionPropertyItem("SubScriptText", "SubScriptText:", "Appearance", GetPropertyDescriptor(base.Component, "SubScriptText").Description),
					new DesignerActionPropertyItem("SuperScriptText", "SuperScriptText:", "Appearance", GetPropertyDescriptor(base.Component, "SuperScriptText").Description),
					new DesignerActionPropertyItem("LineThickness", "LineThickness:", "Appearance", GetPropertyDescriptor(base.Component, "LineThickness").Description),
					new DesignerActionPropertyItem("LineProgressThickness", "LineProgressThickness:", "Appearance", GetPropertyDescriptor(base.Component, "LineProgressThickness").Description),
					new DesignerActionPropertyItem("ProgressStartCap", "ProgressStartCap:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressStartCap").Description),
					new DesignerActionPropertyItem("ProgressEndCap", "ProgressEndCap:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressEndCap").Description),
					new DesignerActionPropertyItem("ValueMargin", "ValueMargin:", "Behaviour", GetPropertyDescriptor(base.Component, "ValueMargin").Description),
					new DesignerActionPropertyItem("SuperScriptMargin", "SuperScriptMargin:", "Behaviour", GetPropertyDescriptor(base.Component, "SuperScriptMargin").Description),
					new DesignerActionPropertyItem("SubScriptMargin", "SubScriptMargin:", "Behaviour", GetPropertyDescriptor(base.Component, "SubScriptMargin").Description)
				};
			}
		}

		private int _angle = -90;

		private int _interval = 5;

		private int _circleMargin = 10;

		private int _lineThickness = 10;

		private int _lineProgressThickness = 10;

		private int _progressAnimationSpeed = 200;

		private bool _animated = false;

		private bool _isPercentage = false;

		private string _subscriptText = "";

		private string _superscriptText = "";

		private Color _subscriptColor;

		private Color _superscriptcolor;

		private Color _progressColor = Color.DodgerBlue;

		private Color _progressColor2 = Color.DodgerBlue;

		private Color _progressBackColor = Color.Gainsboro;

		private Font _secondaryFont;

		private Padding _textMargin;

		private Padding _superscriptMargin;

		private Padding _subscriptMargin;

		private CapStyles _progressStartCap;

		private CapStyles _progressEndCap;

		private FillStyles _progressFillStyle = FillStyles.Solid;

		private IContainer components = null;

		private Timer animationTimer;

		[Category("x Properties")]
		[Description("Sets a value indicating whether the progress movement will be animated.")]
		public virtual bool Animated
		{
			get
			{
				return _animated;
			}
			set
			{
				_animated = value;
				if (_animated)
					animationTimer.Start();
				else
				{
					animationTimer.Stop();
					_angle = -90;
				}
				Refresh();
			}
		}

		[Description("Sets a value indicating whether the progress value will be in percentage format.")]
		[Category("x Properties")]
		public virtual bool IsPercentage
		{
			get
			{
				return _isPercentage;
			}
			set
			{
				_isPercentage = value;
				if (value)
				{
					_superscriptText = "%";
					_superscriptMargin = new Padding(5, 50, 0, 0);
				}
				else
				{
					_subscriptMargin = new Padding(5, -20, 0, 0);
					_superscriptMargin = new Padding(5, 20, 0, 0);
				}
				Refresh();
			}
		}

		[Description("Sets the progress value.")]
		[Category("x Properties")]
		public new virtual int Value
		{
			get
			{
				return base.Value;
			}
			set
			{
				base.Value = value;
				if (IsPercentage)
					Text = ((int)((double)value / (double)Maximum * 100.0)).ToString();
				else
					Text = value.ToString();
				Refresh();
			}
		}

		[Description("Sets the minimum progress value.")]
		public new virtual int Minimum
		{
			get
			{
				return base.Minimum;
			}
			set
			{
				base.Minimum = value;
				Refresh();
			}
		}

		[Description("Sets the maximum progress value.")]
		public new virtual int Maximum
		{
			get
			{
				return base.Maximum;
			}
			set
			{
				base.Maximum = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the circle margin.")]
		public virtual int CircleMargin
		{
			get
			{
				return _circleMargin;
			}
			set
			{
				_circleMargin = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the line thickness.")]
		public virtual int LineThickness
		{
			get
			{
				return _lineThickness;
			}
			set
			{
				if (value > base.Height / 5 || value / 2 > _circleMargin)
				{
					value = _circleMargin * 2;
					return;
				}
				_lineThickness = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the line progress thickness.")]
		public virtual int LineProgressThickness
		{
			get
			{
				return _lineProgressThickness;
			}
			set
			{
				if (value > _circleMargin * 2)
					value = _circleMargin * 2;
				else
					_lineProgressThickness = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the animation interval.")]
		public virtual int AnimationInterval
		{
			get
			{
				return _interval;
			}
			set
			{
				_interval = value;
				Refresh();
			}
		}

		[Description("Sets the animation speed.")]
		[Category("x Properties")]
		public virtual int AnimationSpeed
		{
			get
			{
				return animationTimer.Interval;
			}
			set
			{
				animationTimer.Interval = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the progress animation speed.")]
		public virtual int ProgressAnimationSpeed
		{
			get
			{
				return _progressAnimationSpeed;
			}
			set
			{
				_progressAnimationSpeed = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the subscript text.")]
		public virtual string SubScriptText
		{
			get
			{
				return _subscriptText;
			}
			set
			{
				_subscriptText = value;
				Refresh();
			}
		}

		[Description("Sets the superscript text.")]
		[Category("x Properties")]
		public virtual string SuperScriptText
		{
			get
			{
				return _superscriptText;
			}
			set
			{
				_superscriptText = value;
				Refresh();
			}
		}

		[Browsable(true)]
		[Description("Sets the control's primary font.")]
		[Category("x Properties")]
		public new virtual Font Font
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
		[Description("Sets the secondary font.")]
		public virtual Font SecondaryFont
		{
			get
			{
				return _secondaryFont;
			}
			set
			{
				_secondaryFont = value;
				Refresh();
			}
		}

		[Description("Sets the subscript color.")]
		[Category("x Properties")]
		public virtual Color SubScriptColor
		{
			get
			{
				return _subscriptColor;
			}
			set
			{
				_subscriptColor = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the superscript color.")]
		public virtual Color SuperScriptColor
		{
			get
			{
				return _superscriptcolor;
			}
			set
			{
				_superscriptcolor = value;
				Refresh();
			}
		}

		[Description("Sets the progress color.")]
		[Category("x Properties")]
		public virtual Color ProgressColor
		{
			get
			{
				return _progressColor;
			}
			set
			{
				_progressColor = value;
				Refresh();
			}
		}

		[Description("Sets the secondary progress color, providing a gradient color fill.")]
		[Category("x Properties")]
		public virtual Color ProgressColor2
		{
			get
			{
				return _progressColor2;
			}
			set
			{
				_progressColor2 = value;
				Refresh();
			}
		}

		[Description("Sets the progress background color.")]
		[Category("x Properties")]
		public virtual Color ProgressBackColor
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

		[Description("Sets the value text margin.")]
		[Category("x Properties")]
		public virtual Padding ValueMargin
		{
			get
			{
				return _textMargin;
			}
			set
			{
				_textMargin = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the superscript margin.")]
		public virtual Padding SuperScriptMargin
		{
			get
			{
				return _superscriptMargin;
			}
			set
			{
				_superscriptMargin = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the subscript margin.")]
		public virtual Padding SubScriptMargin
		{
			get
			{
				return _subscriptMargin;
			}
			set
			{
				_subscriptMargin = value;
				Refresh();
			}
		}

		[Description("Sets the color fill style to be used when rendering the progress bar.")]
		[Category("x Properties")]
		public virtual FillStyles ProgressFillStyle
		{
			get
			{
				return _progressFillStyle;
			}
			set
			{
				_progressFillStyle = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the rendering style of the progress starting point.")]
		public virtual CapStyles ProgressStartCap
		{
			get
			{
				return _progressStartCap;
			}
			set
			{
				_progressStartCap = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the rendering style of the progress ending point.")]
		public virtual CapStyles ProgressEndCap
		{
			get
			{
				return _progressEndCap;
			}
			set
			{
				_progressEndCap = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public int ValueByTransition
		{
			get
			{
				return base.Value;
			}
			set
			{
				if (!base.DesignMode)
					TransitionValue(value);
			}
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
				base.Text = Value.ToString();
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual Padding TextMargin
		{
			get
			{
				return _textMargin;
			}
			set
			{
				_textMargin = value;
				Refresh();
			}
		}

		public event EventHandler<ProgressChangedEventArgs> ProgressChanged = null;

		public xCircleProgress()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.Selectable, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.ContainerControl, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			AnimationSpeed = 1;
			AnimationInterval = 1;
			SetDefaults();
		}

		public void TransitionValue(int value)
		{
			Transition.run(this, "Value", value, new TransitionType_EaseInEaseOut(_progressAnimationSpeed));
		}

		public void TransitionValue(int value, int transitionSpeed = 200)
		{
			Transition.run(this, "Value", value, new TransitionType_EaseInEaseOut(transitionSpeed));
		}

		private void RenderProgress(int value, Graphics graphics)
		{
			try
			{
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				SolidBrush solidBrush = new SolidBrush(ForeColor);
				Rectangle rect = new Rectangle(CircleMargin, CircleMargin, base.Width - CircleMargin * 2, base.Width - CircleMargin * 2);
				Pen pen = new Pen(_progressBackColor);
				pen.Alignment = PenAlignment.Outset;
				pen.Width = _lineThickness;
				graphics.DrawArc(pen, rect, 0f, 360f);
				Rectangle rect2 = new Rectangle(CircleMargin, CircleMargin, base.Width - CircleMargin * 2, base.Width - CircleMargin * 2);
				Pen pen2 = new Pen(_progressColor);
				pen2.Width = _lineProgressThickness;
				LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new PointF(rect2.X, rect2.Y), new PointF(rect2.Width, rect2.Height), _progressColor, _progressColor2);
				if (_progressFillStyle == FillStyles.Gradient)
					pen2.Brush = linearGradientBrush;
				if (_progressStartCap == CapStyles.Arrow)
					pen2.StartCap = LineCap.ArrowAnchor;
				else if (_progressStartCap == CapStyles.Flat)
				{
					pen2.StartCap = LineCap.Flat;
				}
				else if (_progressStartCap == CapStyles.Round)
				{
					pen2.StartCap = LineCap.Round;
				}
				else if (_progressStartCap == CapStyles.Triangle)
				{
					pen2.StartCap = LineCap.Triangle;
				}
				if (_progressEndCap == CapStyles.Arrow)
					pen2.EndCap = LineCap.ArrowAnchor;
				else if (_progressEndCap == CapStyles.Flat)
				{
					pen2.EndCap = LineCap.Flat;
				}
				else if (_progressEndCap == CapStyles.Round)
				{
					pen2.EndCap = LineCap.Round;
				}
				else if (_progressEndCap == CapStyles.Triangle)
				{
					pen2.EndCap = LineCap.Triangle;
				}
				graphics.DrawArc(pen2, rect2, _angle, Calculate(Value));
				DrawText(graphics);
				solidBrush.Dispose();
				pen.Dispose();
				linearGradientBrush.Dispose();
				pen2.Dispose();
				InvokeProgressChangedEvent();
			}
			catch (Exception)
			{
			}
		}

		private void DrawText(Graphics graphics)
		{
			PointF pointF = AddPoint(Point.Empty, 2);
			SizeF sizeF = AddSize(base.Size, -4);
			if (Text == string.Empty)
				return;
			pointF.X += TextMargin.Left;
			pointF.Y += TextMargin.Top;
			sizeF.Width -= TextMargin.Right;
			sizeF.Height -= TextMargin.Bottom;
			StringFormat format = new StringFormat((RightToLeft == RightToLeft.Yes) ? StringFormatFlags.DirectionRightToLeft : ((StringFormatFlags)0))
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Near
			};
			SizeF size = graphics.MeasureString(Text, Font);
			PointF location = new PointF(pointF.X + (sizeF.Width - size.Width) / 2f, pointF.Y + (sizeF.Height - size.Height) / 2f);
			if (SubScriptText != string.Empty || SuperScriptText != string.Empty)
			{
				float num = 0f;
				SizeF size2 = SizeF.Empty;
				SizeF size3 = SizeF.Empty;
				if (SuperScriptText != string.Empty)
				{
					size2 = graphics.MeasureString(SuperScriptText, SecondaryFont);
					num = Math.Max(size2.Width, num);
					size2.Width -= SuperScriptMargin.Right;
					size2.Height -= SuperScriptMargin.Bottom;
				}
				if (SubScriptText != string.Empty && !_isPercentage)
				{
					size3 = graphics.MeasureString(SubScriptText, SecondaryFont);
					num = Math.Max(size3.Width, num);
					size3.Width -= SubScriptMargin.Right;
					size3.Height -= SubScriptMargin.Bottom;
				}
				location.X -= num / 4f;
				if (SuperScriptText != string.Empty)
				{
					PointF location2 = new PointF(location.X + size.Width - size2.Width / 2f, location.Y - size2.Height * 0.85f);
					location2.X += SuperScriptMargin.Left;
					location2.Y += SuperScriptMargin.Top;
					graphics.DrawString(SuperScriptText, SecondaryFont, new SolidBrush(SuperScriptColor), new RectangleF(location2, size2), format);
				}
				if (SubScriptText != string.Empty && !_isPercentage)
				{
					PointF location3 = new PointF(location.X + size.Width - size3.Width / 2f, location.Y + size.Height * 0.85f);
					location3.X += SubScriptMargin.Left;
					location3.Y += SubScriptMargin.Top;
					graphics.DrawString(SubScriptText, SecondaryFont, new SolidBrush(SubScriptColor), new RectangleF(location3, size3), format);
				}
			}
			graphics.DrawString(Text, Font, new SolidBrush(ForeColor), new RectangleF(location, size), format);
		}

		private void SetDefaults()
		{
			Animated = false;
			IsPercentage = false;
			ProgressStartCap = CapStyles.Round;
			ProgressEndCap = CapStyles.Round;
			base.TabIndex = 16;
			AnimationSpeed = 1;
			AnimationInterval = 1;
			ValueByTransition = 30;
			base.Size = new Size(184, 184);
			SecondaryFont = new Font("Microsoft Sans Serif", 16f);
			Font = new Font("Microsoft Sans Serif", 40f, FontStyle.Bold);
			Text = "0";
			SuperScriptText = "°C";
			SubScriptText = ".00";
			BackColor = Color.Transparent;
			ForeColor = Color.FromArgb(64, 64, 64);
			ProgressColor = Color.DodgerBlue;
			ProgressBackColor = Color.Gainsboro;
			SubScriptColor = Color.FromArgb(166, 166, 166);
			SuperScriptColor = Color.FromArgb(166, 166, 166);
			TextMargin = new Padding(0, 5, 0, 0);
			SubScriptMargin = new Padding(5, -20, 0, 0);
			SuperScriptMargin = new Padding(5, 20, 0, 0);
		}

		private void InvokeProgressChangedEvent()
		{
			if (this.ProgressChanged != null)
				this.ProgressChanged(this, new ProgressChangedEventArgs(base.Value, base.Maximum));
		}

		private int Calculate(int value)
		{
			return 360 * value / Maximum;
		}

		private static PointF AddPoint(PointF point, int value)
		{
			point.X += value;
			point.Y += value;
			return point;
		}

		private static SizeF AddSize(SizeF size, int value)
		{
			size.Height += value;
			size.Width += value;
			return size;
		}

		protected override void OnPaint(PaintEventArgs pe)
		{
			RenderProgress(Value, pe.Graphics);
			base.OnPaint(pe);
		}

		private void OnTimerTick(object sender, EventArgs e)
		{
			if (Value != Maximum && Value > 0)
			{
				_angle += AnimationInterval;
				Refresh();
			}
		}

		private void OnResizeControl(object sender, EventArgs e)
		{
			base.Height = base.Width;
			Refresh();
		}

		private void OnFontChanged(object sender, EventArgs e)
		{
			Refresh();
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
			this.animationTimer = new System.Windows.Forms.Timer(this.components);
			base.SuspendLayout();
			this.animationTimer.Interval = 300;
			this.animationTimer.Tick += new System.EventHandler(OnTimerTick);
			this.Text = "0";
			base.FontChanged += new System.EventHandler(OnFontChanged);
			base.Resize += new System.EventHandler(OnResizeControl);
			base.ResumeLayout(false);
		}
	}
}
