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
	[DefaultEvent("Click")]
	[Designer(typeof(xDesigner))]
	[DefaultProperty("Orientation")]
	[Category("x For Windows Forms")]
	[Description("Add separator lines and dividers in your designs for organizing content.")]
	[ToolboxBitmap(typeof(SplitContainer))]
	[DebuggerStepThrough]
	public class xSeparator : UserControl
	{
		public enum LineOrientation
		{
			Vertical,
			Horizontal
		}

		public enum LineStyles
		{
			Solid,
			Dash,
			Dot,
			DashDot,
			DashDotDot,
			DoubleEdgeFaded,
			LeftEdgeFaded,
			RightEdgeFaded
		}

		public enum CapStyles
		{
			Flat,
			Round,
			Triangle
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
					if (((xSeparator)base.Control).AutoSize)
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
			private xSeparator xControl;

			private DesignerActionUIService designerActionUISvc = null;

			public LineOrientation Orientation
			{
				get
				{
					return xControl.Orientation;
				}
				set
				{
					SetValue(xControl, "Orientation", value);
					designerActionUISvc.Refresh(base.Component);
				}
			}

			public CapStyles DashCap
			{
				get
				{
					return xControl.DashCap;
				}
				set
				{
					SetValue(xControl, "DashCap", value);
				}
			}

			public LineStyles LineStyle
			{
				get
				{
					return xControl.LineStyle;
				}
				set
				{
					SetValue(xControl, "LineStyle", value);
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

			public Color LineColor
			{
				get
				{
					return xControl.LineColor;
				}
				set
				{
					SetValue(xControl, "LineColor", value);
				}
			}

			public xControlDesignerActionList(IComponent component)
				: base(component)
			{
				xControl = component as xSeparator;
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
					new DesignerActionPropertyItem("Orientation", "Orientation:", "Common Tasks", GetPropertyDescriptor(base.Component, "Orientation").Description),
					new DesignerActionPropertyItem("LineColor", "LineColor:", "Common Tasks", GetPropertyDescriptor(base.Component, "LineColor").Description),
					new DesignerActionPropertyItem("LineStyle", "LineStyle:", "Common Tasks", GetPropertyDescriptor(base.Component, "LineStyle").Description),
					new DesignerActionPropertyItem("DashCap", "DashCap:", "Common Tasks", GetPropertyDescriptor(base.Component, "DashCap").Description),
					new DesignerActionPropertyItem("LineThickness", "LineThickness:", "Appearance", GetPropertyDescriptor(base.Component, "LineThickness").Description)
				};
			}
		}

		private int _lineThickness = 1;

		private Color _lineColor = Color.Silver;

		private CapStyles _dashCap = CapStyles.Flat;

		private LineStyles _lineStyle = LineStyles.Solid;

		private LineOrientation _orientation = LineOrientation.Horizontal;

		private IContainer components = null;

		[Description("Sets the line's orientation.")]
		[Category("x Properties")]
		public LineOrientation Orientation
		{
			get
			{
				return _orientation;
			}
			set
			{
				_orientation = value;
				int num = base.Width;
				int num2 = base.Height;
				switch (value)
				{
				case LineOrientation.Vertical:
					base.Width = num2;
					base.Height = num;
					break;
				case LineOrientation.Horizontal:
					base.Height = num;
					base.Width = num2;
					break;
				}
			}
		}

		[Description("Sets the line's style format.")]
		[Category("x Properties")]
		public LineStyles LineStyle
		{
			get
			{
				return _lineStyle;
			}
			set
			{
				_lineStyle = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the line's dash cap style.")]
		public CapStyles DashCap
		{
			get
			{
				return _dashCap;
			}
			set
			{
				_dashCap = value;
				Refresh();
			}
		}

		[Description("Sets the line's thickness.")]
		[Category("x Properties")]
		public int LineThickness
		{
			get
			{
				return _lineThickness;
			}
			set
			{
				_lineThickness = value;
				Refresh();
			}
		}

		[Description("Sets the line's color.")]
		[Category("x Properties")]
		public Color LineColor
		{
			get
			{
				return _lineColor;
			}
			set
			{
				_lineColor = value;
				Refresh();
			}
		}

		public xSeparator()
		{
			InitializeComponent();
			Refresh();
		}

		public new void Refresh()
		{
			Bitmap bitmap = new Bitmap(base.Width, base.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			using (graphics)
				using (SolidBrush brush = new SolidBrush(_lineColor))
				{
					Pen pen = new Pen(brush);
					DashCap dashCap = System.Drawing.Drawing2D.DashCap.Flat;
					DashStyle dashStyle = DashStyle.Solid;
					if (_lineStyle == LineStyles.Solid)
						dashStyle = DashStyle.Solid;
					else if (_lineStyle == LineStyles.Dot)
					{
						dashStyle = DashStyle.Dot;
					}
					else if (_lineStyle == LineStyles.Dash)
					{
						dashStyle = DashStyle.Dash;
					}
					else if (_lineStyle == LineStyles.DashDot)
					{
						dashStyle = DashStyle.DashDot;
					}
					else if (_lineStyle == LineStyles.DashDotDot)
					{
						dashStyle = DashStyle.DashDotDot;
					}
					if (_dashCap == CapStyles.Flat)
						dashCap = System.Drawing.Drawing2D.DashCap.Flat;
					else if (_dashCap == CapStyles.Round)
					{
						dashCap = System.Drawing.Drawing2D.DashCap.Round;
					}
					else if (_dashCap == CapStyles.Triangle)
					{
						dashCap = System.Drawing.Drawing2D.DashCap.Triangle;
					}
					pen.Width = _lineThickness;
					pen.DashStyle = dashStyle;
					pen.DashCap = dashCap;
					if (pen.DashCap != 0)
						graphics.SmoothingMode = SmoothingMode.AntiAlias;
					if (_lineStyle == LineStyles.DoubleEdgeFaded || _lineStyle == LineStyles.LeftEdgeFaded || _lineStyle == LineStyles.RightEdgeFaded)
					{
						RectangleF rect = default(RectangleF);
						RectangleF rect2 = default(RectangleF);
						float angle = 0f;
						if (_orientation == LineOrientation.Horizontal)
						{
							angle = 0f;
							rect = new RectangleF(-1f, base.Height / 2 - _lineThickness / 2, (base.Width + 1) / 2, _lineThickness);
							rect2 = new RectangleF(rect.Right - 1f, base.Height / 2 - _lineThickness / 2, (base.Width + 1) / 2, _lineThickness);
						}
						else if (_orientation == LineOrientation.Vertical)
						{
							angle = 90f;
							rect = new RectangleF(base.Width / 2 - _lineThickness / 2, -1f, _lineThickness, (base.Height + 1) / 2);
							rect2 = new RectangleF(base.Width / 2 - _lineThickness / 2, rect.Bottom - 1f, _lineThickness, (base.Height + 1) / 2);
						}
						LinearGradientBrush linearGradientBrush = new LinearGradientBrush(rect, Color.Transparent, LineColor, angle, false);
						LinearGradientBrush linearGradientBrush2 = new LinearGradientBrush(rect2, LineColor, Color.Transparent, angle, false);
						if (_lineStyle == LineStyles.DoubleEdgeFaded)
						{
							linearGradientBrush = new LinearGradientBrush(rect, Color.Transparent, LineColor, angle, false);
							linearGradientBrush2 = new LinearGradientBrush(rect2, LineColor, Color.Transparent, angle, false);
						}
						else if (_lineStyle == LineStyles.LeftEdgeFaded)
						{
							linearGradientBrush = new LinearGradientBrush(rect, Color.Transparent, LineColor, angle, false);
							linearGradientBrush2 = new LinearGradientBrush(rect2, LineColor, LineColor, angle, false);
						}
						else if (_lineStyle == LineStyles.RightEdgeFaded)
						{
							linearGradientBrush = new LinearGradientBrush(rect, LineColor, LineColor, angle, false);
							linearGradientBrush2 = new LinearGradientBrush(rect2, LineColor, Color.Transparent, angle, false);
						}
						graphics.FillRectangle(linearGradientBrush, rect);
						graphics.FillRectangle(linearGradientBrush2, rect2);
						linearGradientBrush.Dispose();
						linearGradientBrush2.Dispose();
					}
					else if (_orientation == LineOrientation.Horizontal)
					{
						graphics.DrawLine(pen, new Point(0, base.Height / 2), new Point(base.Width, base.Height / 2));
					}
					else if (_orientation == LineOrientation.Vertical)
					{
						graphics.DrawLine(pen, new Point(base.Width / 2, 0), new Point(base.Width / 2, base.Height));
					}
					pen.Dispose();
				}
			BackgroundImage = bitmap;
			BackgroundImageLayout = ImageLayout.Stretch;
		}

		private void OnResize(object sender, EventArgs e)
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
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			base.Name = "xSeparator";
			base.Size = new System.Drawing.Size(300, 14);
			base.Resize += new System.EventHandler(OnResize);
			base.ResumeLayout(false);
		}
	}
}
