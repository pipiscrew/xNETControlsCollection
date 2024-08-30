using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace xCollection
{
	[Description("Provides a highly customizable, arc-based, range value display meter for previewing value movements within definite ranges of motion.")]
	[DebuggerStepThrough]
	[DefaultProperty("Value")]
	[DefaultEvent("ValueChanged")]
	[ToolboxBitmap(typeof(TrackBar))]
	[Designer(typeof(xDesigner))]
	[Category("x UI For Windows Forms")]
	public class xRadialGauge : UserControl
	{
		public enum CapStyles
		{
			Flat,
			Round
		}

		public class ValueChangedEventArgs : EventArgs
		{
			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			[CompilerGenerated]
			private readonly int _003CValue_003Ek__BackingField;

			public int Value
			{
				[CompilerGenerated]
				get
				{
					return _003CValue_003Ek__BackingField;
				}
			}

			public ValueChangedEventArgs(int value)
			{
				_003CValue_003Ek__BackingField = value;
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
					xRadialGauge xRadialGauge = (xRadialGauge)base.Control;
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
							new xRadialGaugeActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
				base.AutoResizeHandles = true;
			}
		}

		public class xRadialGaugeActionList : DesignerActionList
		{
			private xRadialGauge xControl;

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

			public int Thickness
			{
				get
				{
					return xControl.Thickness;
				}
				set
				{
					SetValue(xControl, "Thickness", value);
				}
			}

			public int WarningMark
			{
				get
				{
					return xControl.WarningMark;
				}
				set
				{
					SetValue(xControl, "WarningMark", value);
				}
			}

			public bool ShowRangeLabels
			{
				get
				{
					return xControl.ShowRangeLabels;
				}
				set
				{
					SetValue(xControl, "ShowRangeLabels", value);
				}
			}

			public bool ShowValueLabel
			{
				get
				{
					return xControl.ShowValueLabel;
				}
				set
				{
					SetValue(xControl, "ShowValueLabel", value);
				}
			}

			public string Suffix
			{
				get
				{
					return xControl.Suffix;
				}
				set
				{
					SetValue(xControl, "Suffix", value);
				}
			}

			public Color ProgressColorHigh
			{
				get
				{
					return xControl.ProgressColorHigh;
				}
				set
				{
					SetValue(xControl, "ProgressColorHigh", value);
				}
			}

			public Color ProgressColorLow
			{
				get
				{
					return xControl.ProgressColorLow;
				}
				set
				{
					SetValue(xControl, "ProgressColorLow", value);
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

			public Color RangeLabelsColor
			{
				get
				{
					return xControl.RangeLabelsColor;
				}
				set
				{
					SetValue(xControl, "RangeLabelsColor", value);
				}
			}

			public Color ValueLabelColor
			{
				get
				{
					return xControl.ValueLabelColor;
				}
				set
				{
					SetValue(xControl, "ValueLabelColor", value);
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

			public CapStyles ProgressCap
			{
				get
				{
					return xControl.ProgressCap;
				}
				set
				{
					SetValue(xControl, "ProgressCap", value);
				}
			}

			public xRadialGaugeActionList(IComponent component)
				: base(component)
			{
				xControl = component as xRadialGauge;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)                  ", "Common Tasks"),
					new DesignerActionPropertyItem("Value", "Value:", "Common Tasks", GetPropertyDescriptor(base.Component, "Value").Description),
					new DesignerActionPropertyItem("Minimum", "Minimum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Minimum").Description),
					new DesignerActionPropertyItem("Maximum", "Maximum:", "Common Tasks", GetPropertyDescriptor(base.Component, "Maximum").Description),
					new DesignerActionPropertyItem("WarningMark", "WarningMark:", "Common Tasks", GetPropertyDescriptor(base.Component, "WarningMark").Description),
					new DesignerActionPropertyItem("ProgressCap", "ProgressCap:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressCap").Description),
					new DesignerActionPropertyItem("ProgressColorHigh", "ProgressColorHigh:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColorHigh").Description),
					new DesignerActionPropertyItem("ProgressColorLow", "ProgressColorLow:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressColorLow").Description),
					new DesignerActionPropertyItem("ProgressBackColor", "ProgressBackColor:", "Appearance", GetPropertyDescriptor(base.Component, "ProgressBackColor").Description),
					new DesignerActionPropertyItem("RangeLabelsColor", "RangeLabelsColor:", "Appearance", GetPropertyDescriptor(base.Component, "RangeLabelsColor").Description),
					new DesignerActionPropertyItem("ValueLabelColor", "ValueLabelColor:", "Appearance", GetPropertyDescriptor(base.Component, "ValueLabelColor").Description),
					new DesignerActionPropertyItem("Suffix", "Suffix:", "Appearance", GetPropertyDescriptor(base.Component, "Suffix").Description),
					new DesignerActionPropertyItem("Thickness", "Thickness:", "Appearance", GetPropertyDescriptor(base.Component, "Thickness").Description),
					new DesignerActionPropertyItem("Font", "Font:", "Appearance", GetPropertyDescriptor(base.Component, "Font").Description),
					new DesignerActionPropertyItem("ShowRangeLabels", "ShowRangeLabels", "Appearance", GetPropertyDescriptor(base.Component, "ShowRangeLabels").Description),
					new DesignerActionPropertyItem("ShowValueLabel", "ShowValueLabel", "Appearance", GetPropertyDescriptor(base.Component, "ShowValueLabel").Description)
				};
			}
		}

		private bool _showValueLabel = true;

		private bool _showRangeLabels = true;

		private bool _autoGenerateProgressColorWhenLow = false;

		private bool _autoGenerateProgressColorWhenHigh = true;

		private int _value = 30;

		private int _minimum = 0;

		private int _maximum = 100;

		private int _thickness = 30;

		private int _lighteningFactor = 70;

		private int _progressHighValueMark = 70;

		private string _prefix = "";

		private string _suffix = "%";

		private Color _progressColorHigh = Color.Crimson;

		private Color _progressColorLow = Color.DodgerBlue;

		private Color _progressBackColor = Color.LightBlue;

		private CapStyles _progressCap;

		private IContainer components = null;

		private Label lblpass;

		private Label lblmin;

		private Label lblmax;

		[Description("Sets a value indicating whether the gauge's range labels will be displayed.")]
		[Category("x Properties")]
		public virtual bool ShowRangeLabels
		{
			get
			{
				return _showRangeLabels;
			}
			set
			{
				_showRangeLabels = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the gauge's value label will be displayed.")]
		public virtual bool ShowValueLabel
		{
			get
			{
				return _showValueLabel;
			}
			set
			{
				_showValueLabel = value;
				Refresh();
			}
		}

		[Description("When set to true, a lighter background color will be automatically generated for the progress background color that is based on the set 'ProgressColorLow' color.")]
		[Category("x Properties")]
		public virtual bool AutoGenerateProgressColorWhenLow
		{
			get
			{
				return _autoGenerateProgressColorWhenLow;
			}
			set
			{
				_autoGenerateProgressColorWhenLow = value;
				Invalidate();
			}
		}

		[Description("When set to true, a lighter background color will be automatically generated for the progress background color that is based on the set 'ProgressColorHigh' color.")]
		[Category("x Properties")]
		public virtual bool AutoGenerateProgressColorWhenHigh
		{
			get
			{
				return _autoGenerateProgressColorWhenHigh;
			}
			set
			{
				_autoGenerateProgressColorWhenHigh = value;
				Invalidate();
			}
		}

		[Description("Sets the gauge's pointer value.")]
		[Category("x Properties")]
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
					EventHandler<ValueChangedEventArgs> valueChanged = this.ValueChanged;
					if (valueChanged != null)
						valueChanged(this, new ValueChangedEventArgs(value));
					EventHandler onValueChanged = this.OnValueChanged;
					if (onValueChanged != null)
						onValueChanged(this, EventArgs.Empty);
					Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("The value is outside the expected range, that is, between the Minimum and Maximum values.");
			}
		}

		[Description("Sets the gauge's pointer value using a smooth transition.")]
		[Browsable(false)]
		public virtual int ValueByTransition
		{
			get
			{
				return Value;
			}
			set
			{
				TransitionValue(value);
			}
		}

		[Description("Sets the minimum range value.")]
		[Category("x Properties")]
		public virtual int Minimum
		{
			get
			{
				return RangeStart;
			}
			set
			{
				RangeStart = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the maximum range value.")]
		public virtual int Maximum
		{
			get
			{
				return RangeEnd;
			}
			set
			{
				RangeEnd = value;
			}
		}

		[Description("Sets the point at which the gauge should mark as the beginning of high value ranges.")]
		[Category("x Properties")]
		public virtual int WarningMark
		{
			get
			{
				return ProgressHighValueMark;
			}
			set
			{
				ProgressHighValueMark = value;
			}
		}

		[Category("x Properties")]
		[Description("Applies a lightening value/factor that will be used to generate the progress background color when the value set is either high or low. (Default is 70)")]
		public virtual int LighteningFactor
		{
			get
			{
				return _lighteningFactor;
			}
			set
			{
				_lighteningFactor = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the Gauge's progress color whenever it is within the low or minimum value ranges.")]
		public virtual Color ProgressColorLow
		{
			get
			{
				return _progressColorLow;
			}
			set
			{
				_progressColorLow = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's progress color whenever it is within the high or maximum value ranges as specified by the property 'ProgressHighValueMark'.")]
		public virtual Color ProgressColorHigh
		{
			get
			{
				return _progressColorHigh;
			}
			set
			{
				_progressColorHigh = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's range labels' color.")]
		public Color RangeLabelsColor
		{
			get
			{
				return lblmin.ForeColor;
			}
			set
			{
				lblmin.ForeColor = value;
				lblmax.ForeColor = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's value label color.")]
		public Color ValueLabelColor
		{
			get
			{
				return lblpass.ForeColor;
			}
			set
			{
				lblpass.ForeColor = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's progress bakcground color.")]
		public virtual Color ProgressBackColor
		{
			get
			{
				return _progressBackColor;
			}
			set
			{
				_progressBackColor = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's prefix text that precedes the gauge value.")]
		public virtual string Prefix
		{
			get
			{
				return _prefix;
			}
			set
			{
				_prefix = value;
				lblpass.Text = _prefix + Value + _suffix;
				Invalidate();
			}
		}

		[Description("Sets the gauge's suffix text that is displayed besides the gauge value.")]
		[Category("x Properties")]
		public virtual string Suffix
		{
			get
			{
				return _suffix;
			}
			set
			{
				_suffix = value;
				lblpass.Text = _prefix + Value + _suffix;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's progress thickness.")]
		public virtual int Thickness
		{
			get
			{
				return _thickness;
			}
			set
			{
				_thickness = value;
				Invalidate();
			}
		}

		[Category("x Properties")]
		[Description("Sets the gauge's standard font.")]
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
		[Description("Sets the rendering style of the progress edges.")]
		public virtual CapStyles ProgressCap
		{
			get
			{
				return _progressCap;
			}
			set
			{
				_progressCap = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public Font RangeLabelsFont
		{
			get
			{
				return lblmin.Font;
			}
			set
			{
				lblmin.Font = value;
				lblmax.Font = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual int RangeEnd
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
					lblmax.Text = value.ToString();
					Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("The Maximum value is less than the Minimum value.");
			}
		}

		[Browsable(false)]
		public virtual int RangeStart
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
					lblmin.Text = value.ToString();
					Invalidate();
					return;
				}
				throw new ArgumentOutOfRangeException("The Minimum value is greater than the Maximum value.");
			}
		}

		[Browsable(false)]
		public virtual int ProgressHighValueMark
		{
			get
			{
				return _progressHighValueMark;
			}
			set
			{
				if ((value >= _minimum) & (value <= _maximum))
					_progressHighValueMark = value;
				Invalidate();
			}
		}

		[Browsable(false)]
		public virtual Rectangle GaugeRectangle { get; private set; }

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

		[Obsolete("This property has been deprecated as of version 1.10.0.1. Please use the property 'ProgressColorLow' instead.")]
		[Browsable(false)]
		public Color ProgressColor1
		{
			get
			{
				return ProgressColorLow;
			}
			set
			{
				ProgressColorLow = value;
			}
		}

		[Obsolete("This property has been deprecated as of version 1.10.0.1. Please use the property 'ProgressColorHigh' instead.")]
		[Browsable(false)]
		public Color ProgressColor2
		{
			get
			{
				return ProgressColorHigh;
			}
			set
			{
				ProgressColorHigh = value;
			}
		}

		[Browsable(false)]
		[Obsolete("This property has been deprecated as of version 1.10.0.1. Please use the property 'ProgressBackColor' instead.")]
		public Color ProgressBgColor
		{
			get
			{
				return ProgressBackColor;
			}
			set
			{
				ProgressBackColor = value;
			}
		}

		[Browsable(false)]
		[Obsolete("This property has been deprecated as of version 1.10.0.1. Please use the property 'ProgressColorLow' instead.")]
		public Color ProgressColor
		{
			get
			{
				return ProgressColorLow;
			}
			set
			{
				ProgressColorLow = value;
			}
		}

		[Category("x Events")]
		[Description("Occurs when the 'Value' property is changed.")]
		public virtual event EventHandler<ValueChangedEventArgs> ValueChanged = null;

		[Category("x Events")]
		[Description("Occurs when the 'Value' property is changed.")]
		public virtual event EventHandler OnValueChanged = null;

		public xRadialGauge()
		{
			InitializeComponent();
			SetDefaults();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			int num = base.Size.Width - _thickness * 2;
			GaugeRectangle = new Rectangle(_thickness, base.Size.Height / 4, num, num);
			Pen pen = new Pen(_progressBackColor, _thickness);
			Pen pen2 = new Pen(_progressColorLow.LightenBy(LighteningFactor), _thickness);
			Pen pen3 = new Pen(_progressColorHigh.LightenBy(LighteningFactor), _thickness);
			Pen pen4 = new Pen(_progressColorLow, _thickness);
			Pen pen5 = new Pen(_progressColorHigh, _thickness);
			LineCap startCap = LineCap.Flat;
			LineCap endCap = LineCap.Flat;
			if (_progressCap == CapStyles.Flat)
			{
				startCap = LineCap.Flat;
				endCap = LineCap.Flat;
			}
			else if (_progressCap == CapStyles.Round)
			{
				startCap = LineCap.Round;
				endCap = LineCap.Round;
			}
			pen4.StartCap = startCap;
			pen4.EndCap = endCap;
			pen5.StartCap = startCap;
			pen5.EndCap = endCap;
			pen.StartCap = startCap;
			pen.EndCap = endCap;
			pen2.StartCap = startCap;
			pen2.EndCap = endCap;
			pen3.StartCap = startCap;
			pen3.EndCap = endCap;
			if (AutoGenerateProgressColorWhenLow)
			{
				if (_value < _progressHighValueMark)
					e.Graphics.DrawArc(pen2, GaugeRectangle, 180f, 180f);
			}
			else
				e.Graphics.DrawArc(pen, GaugeRectangle, 180f, 180f);
			if (AutoGenerateProgressColorWhenHigh)
			{
				if (_value >= _progressHighValueMark)
					e.Graphics.DrawArc(pen3, GaugeRectangle, 180f, 180f);
			}
			else if (_value >= _progressHighValueMark)
			{
				e.Graphics.DrawArc(pen, GaugeRectangle, 180f, 180f);
			}
			if (_value >= _progressHighValueMark)
				e.Graphics.DrawArc(pen5, GaugeRectangle, 180f, ConvertToDegrees(_value));
			else
				e.Graphics.DrawArc(pen4, GaugeRectangle, 180f, ConvertToDegrees(_value));
			lblpass.Text = _prefix + _value + _suffix;
			lblpass.Top = base.Height - lblpass.Height - 30;
			if (_progressCap == CapStyles.Flat)
			{
				int num4 = (lblmin.Top = (lblmax.Top = base.Height - lblmax.Height - 10));
			}
			else if (_progressCap == CapStyles.Round)
			{
				int num4 = (lblmin.Top = (lblmax.Top = base.Height - lblmax.Height));
			}
			lblmin.Left = 21;
			lblmax.Left = base.Size.Width - lblmax.Width - 20 + 5;
			lblpass.Left = (base.Width - lblpass.Width) / 2 + 1;
			Label label = lblpass;
			Label label2 = lblmin;
			lblmax.Visible = false;
			label2.Visible = false;
			label.Visible = false;
			if (_showValueLabel)
				TextRenderer.DrawText(e.Graphics, _prefix + _value + _suffix, Font, lblpass.Location, ValueLabelColor);
			if (_showRangeLabels)
			{
				TextRenderer.DrawText(e.Graphics, _minimum.ToString(), RangeLabelsFont, lblmin.Location, RangeLabelsColor);
				TextRenderer.DrawText(e.Graphics, _maximum.ToString(), RangeLabelsFont, lblmax.Location, RangeLabelsColor);
			}
			pen.Dispose();
			pen2.Dispose();
			pen3.Dispose();
			pen4.Dispose();
			pen5.Dispose();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			lblpass.Top = base.Height - lblpass.Height - 30;
			int num3 = (lblmin.Top = (lblmax.Top = base.Height - lblmax.Height - 10));
			lblmin.Left = 20;
			lblmax.Left = base.Size.Width - lblmax.Width - 20;
			lblpass.Left = (base.Width - lblpass.Width) / 2;
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			base.OnForeColorChanged(e);
			Label label = lblpass;
			Label label2 = lblmin;
			Color color = (lblmax.ForeColor = ForeColor);
			Color color4 = (label.ForeColor = (label2.ForeColor = color));
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			lblpass.Font = Font;
		}

		public void TransitionValue(int value, int transitionSpeed = 270)
		{
			Transition transition = new Transition(new TransitionType_EaseInEaseOut(transitionSpeed));
			transition.add(this, "Value", value);
			transition.run();
		}

		private void SetDefaults()
		{
			try
			{
				SetStyle(ControlStyles.UserPaint, true);
				SetStyle(ControlStyles.ResizeRedraw, true);
				SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
				SetStyle(ControlStyles.SupportsTransparentBackColor, true);
				ValueLabelColor = Color.Black;
				RangeLabelsColor = Color.Black;
				Value = 0;
				Maximum = 100;
				Suffix = "%";
				ProgressHighValueMark = 300;
				ProgressCap = CapStyles.Flat;
				BackColor = Color.Transparent;
				Font = new Font("Century Gothic", 18f, FontStyle.Regular);
				RangeLabelsFont = new Font("Segoe UI", 8f, FontStyle.Regular);
				TransitionValue(40);
				base.Size = new Size(189, 126);
				Invalidate();
			}
			catch (Exception)
			{
			}
		}

		private void AlignLabels()
		{
			OnResize(EventArgs.Empty);
		}

		private int ConvertToDegrees(int value)
		{
			return int.Parse(Math.Round((double)value * 180.0 / (double)_maximum, 0).ToString());
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
			this.lblpass = new System.Windows.Forms.Label();
			this.lblmin = new System.Windows.Forms.Label();
			this.lblmax = new System.Windows.Forms.Label();
			base.SuspendLayout();
			this.lblpass.AutoSize = true;
			this.lblpass.Font = new System.Drawing.Font("Century Gothic", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblpass.Location = new System.Drawing.Point(83, 34);
			this.lblpass.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblpass.Name = "lblpass";
			this.lblpass.Size = new System.Drawing.Size(22, 24);
			this.lblpass.TabIndex = 1;
			this.lblpass.Text = "0";
			this.lblmin.AutoSize = true;
			this.lblmin.Font = new System.Drawing.Font("Segoe UI", 8f);
			this.lblmin.Location = new System.Drawing.Point(26, 86);
			this.lblmin.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblmin.Name = "lblmin";
			this.lblmin.Size = new System.Drawing.Size(13, 13);
			this.lblmin.TabIndex = 2;
			this.lblmin.Text = "0";
			this.lblmax.AutoSize = true;
			this.lblmax.Font = new System.Drawing.Font("Segoe UI", 8f);
			this.lblmax.Location = new System.Drawing.Point(145, 86);
			this.lblmax.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.lblmax.Name = "lblmax";
			this.lblmax.Size = new System.Drawing.Size(25, 13);
			this.lblmax.TabIndex = 3;
			this.lblmax.Text = "100";
			base.AutoScaleDimensions = new System.Drawing.SizeF(12f, 24f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.lblmax);
			base.Controls.Add(this.lblmin);
			base.Controls.Add(this.lblpass);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Century Gothic", 15.75f);
			base.Margin = new System.Windows.Forms.Padding(6);
			base.Name = "xRadialGauge";
			base.Size = new System.Drawing.Size(189, 125);
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
