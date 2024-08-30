using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace xCollection
{
	[Description("Create stylish gradient panels with extendable color customization options.")]
	[Designer(typeof(xDesigner))]
	[ToolboxBitmap(typeof(Panel))]
	[DefaultEvent("Click")]
	[DefaultProperty("Dock")]
	[Category("x UI For Windows Forms")]
	[DebuggerStepThrough]
	public class xGradientPanel : Panel
	{
		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ParentControlDesigner
		{
			private DesignerActionListCollection actionLists;

			public override SelectionRules SelectionRules
			{
				get
				{
					if (((xGradientPanel)base.Control).AutoSize)
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

		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public class xControlDesignerActionList : DesignerActionList
		{
			private xGradientPanel Control;

			private DesignerActionUIService designerActionUISvc = null;

			public int BorderRadius
			{
				get
				{
					return Control.BorderRadius;
				}
				set
				{
					SetValue(Control, "BorderRadius", value);
				}
			}

			public DockStyle Dock
			{
				get
				{
					return Control.Dock;
				}
				set
				{
					SetValue(Control, "Dock", value);
				}
			}

			public Color GradientTopLeft
			{
				get
				{
					return Control.GradientTopLeft;
				}
				set
				{
					SetValue(Control, "GradientTopLeft", value);
				}
			}

			public Color GradientTopRight
			{
				get
				{
					return Control.GradientTopRight;
				}
				set
				{
					SetValue(Control, "GradientTopRight", value);
				}
			}

			public Color GradientBottomLeft
			{
				get
				{
					return Control.GradientBottomLeft;
				}
				set
				{
					SetValue(Control, "GradientBottomLeft", value);
				}
			}

			public Color GradientBottomRight
			{
				get
				{
					return Control.GradientBottomRight;
				}
				set
				{
					SetValue(Control, "GradientBottomRight", value);
				}
			}

			public xControlDesignerActionList(IComponent component)
				: base(component)
			{
				Control = component as xGradientPanel;
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
					new DesignerActionMethodItem(this, "GenerateColorScheme", "Generate Color Scheme", "Common Tasks"),
					new DesignerActionPropertyItem("Dock", "Dock:", "Common Tasks", GetPropertyDescriptor(base.Component, "Dock").Description),
					new DesignerActionPropertyItem("BorderRadius", "BorderRadius:", "Common Tasks", GetPropertyDescriptor(base.Component, "BorderRadius").Description),
					new DesignerActionMethodItem(this, "Refresh", "Refresh Control Radius", "Common Tasks"),
					new DesignerActionPropertyItem("GradientTopLeft", "GradientTopLeft:", "Appearance", GetPropertyDescriptor(base.Component, "GradientTopLeft").Description),
					new DesignerActionPropertyItem("GradientTopRight", "GradientTopRight:", "Appearance", GetPropertyDescriptor(base.Component, "GradientTopRight").Description),
					new DesignerActionPropertyItem("GradientBottomLeft", "GradientBottomLeft:", "Appearance", GetPropertyDescriptor(base.Component, "GradientBottomLeft").Description),
					new DesignerActionPropertyItem("GradientBottomRight", "GradientBottomRight:", "Appearance", GetPropertyDescriptor(base.Component, "GradientBottomRight").Description)
				};
			}

			public void GenerateColorScheme()
			{
				try
				{
					Random random = new Random();
					Color color = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
					Color color2 = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
					Color color3 = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
					Color color4 = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
					Transition transition = new Transition(new TransitionType_EaseInEaseOut(200));
					transition.add(Control, "GradientTopLeft", color);
					transition.add(Control, "GradientTopRight", color2);
					transition.add(Control, "GradientBottomLeft", color3);
					transition.add(Control, "GradientBottomRight", color4);
					transition.run();
				}
				catch (Exception)
				{
				}
			}

			public void Refresh()
			{
				try
				{
					Control.Refresh();
				}
				catch (Exception)
				{
				}
			}
		}

		private int _quality = 10;

		private int _borderRadius = 1;

		private Color _gradientTopLeft = Color.DodgerBlue;

		private Color _gradientTopRight = Color.FromArgb(198, 60, 212);

		private Color _gradientBottomLeft = Color.FromArgb(236, 92, 188);

		private Color _gradientBottomRight = Color.DeepPink;

		private IContainer components = null;

		[Description("Sets the panel's border radius.")]
		[Category("x Properties")]
		public virtual int BorderRadius
		{
			get
			{
				return _borderRadius;
			}
			set
			{
				_borderRadius = value;
				Refresh();
			}
		}

		[Description("Sets the top-left gradient color.")]
		[Category("x Properties")]
		public virtual Color GradientTopLeft
		{
			get
			{
				return _gradientTopLeft;
			}
			set
			{
				_gradientTopLeft = value;
				Refresh();
			}
		}

		[Description("Sets the top-right gradient color.")]
		[Category("x Properties")]
		public virtual Color GradientTopRight
		{
			get
			{
				return _gradientTopRight;
			}
			set
			{
				_gradientTopRight = value;
				Refresh();
			}
		}

		[Description("Sets the bottom-left gradient color.")]
		[Category("x Properties")]
		public virtual Color GradientBottomLeft
		{
			get
			{
				return _gradientBottomLeft;
			}
			set
			{
				_gradientBottomLeft = value;
				Refresh();
			}
		}

		[Category("x Properties")]
		[Description("Sets the bottom-right gradient color.")]
		public virtual Color GradientBottomRight
		{
			get
			{
				return _gradientBottomRight;
			}
			set
			{
				_gradientBottomRight = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual int Quality
		{
			get
			{
				return _quality;
			}
			set
			{
				_quality = value;
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

		public xGradientPanel()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			BackColor = Color.Transparent;
			Refresh();
		}

		public new void Refresh()
		{
			try
			{
				Color[] array = new Color[4] { _gradientTopLeft, _gradientTopRight, _gradientBottomRight, _gradientBottomLeft };
				GraphicsPath graphicsPath = new GraphicsPath();
				graphicsPath.AddRectangle(base.ClientRectangle);
				Bitmap bitmap = new Bitmap(base.Width, base.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				using (graphics)
					using (PathGradientBrush brush = new PathGradientBrush(graphicsPath)
					{
						CenterColor = Color.FromArgb((int)array.Average((Color a) => a.R), (int)array.Average((Color a) => a.G), (int)array.Average((Color a) => a.B)),
						SurroundColors = array
					})
						graphics.FillPath(brush, RoundRectangle(base.ClientRectangle, _borderRadius));
				BackgroundImage = bitmap;
				BackgroundImageLayout = ImageLayout.Stretch;
			}
			catch (Exception)
			{
			}
		}

		public void GenerateColorScheme()
		{
			try
			{
				Random random = new Random();
				Color topLeft = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
				Color topRight = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
				Color bottomLeft = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
				Color bottomRight = Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(200));
				transition.add(this, "GradientTopLeft", topLeft);
				transition.add(this, "GradientTopRight", topRight);
				transition.add(this, "GradientBottomLeft", bottomLeft);
				transition.add(this, "GradientBottomRight", bottomRight);
				transition.run();
				transition.TransitionCompletedEvent += delegate
				{
					GradientTopLeft = topLeft;
					GradientTopRight = topRight;
					GradientBottomLeft = bottomLeft;
					GradientBottomRight = bottomRight;
				};
			}
			catch (Exception)
			{
			}
		}

		private GraphicsPath RoundRectangle(Rectangle rectangle, int radius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			try
			{
				if (radius > 1)
				{
					graphicsPath.AddArc(rectangle.X, rectangle.Y, radius, radius, 180f, 90f);
					graphicsPath.AddArc(rectangle.X + rectangle.Width - radius - 1, rectangle.Y, radius, radius, 270f, 90f);
					graphicsPath.AddArc(rectangle.X + rectangle.Width - radius - 1, rectangle.Y + rectangle.Height - radius - 1, radius, radius, 0f, 90f);
					graphicsPath.AddArc(rectangle.X, rectangle.Y + rectangle.Height - radius - 1, radius, radius, 90f, 90f);
					graphicsPath.CloseAllFigures();
				}
				else
					graphicsPath.AddRectangle(base.ClientRectangle);
			}
			catch (Exception)
			{
			}
			return graphicsPath;
		}

		protected override void OnResize(EventArgs eventargs)
		{
			base.OnResize(eventargs);
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
			base.ResumeLayout(false);
		}
	}
}
