using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Utilities.xSnackbar
{
	[DebuggerStepThrough]
	[DefaultEvent("FormDragging")]
	[Designer(typeof(xDesigner))]
	[Description("Provides the standard Windows docking capabilities to borderless Windows Forms with richer and enhanced flexibility options.")]
	[ToolboxBitmap(typeof(PageSetupDialog))]
	[DefaultProperty("DockingOptions")]
	internal class xFormDock : Component
	{
		public enum xWindowStates
		{
			Normal,
			Maximized,
			Minimized
		}

		public enum FormWindowStates
		{
			Normal,
			Maximized,
			Minimized
		}

		public enum DockIndicators
		{
			None,
			Left,
			Right,
			TopLeft,
			TopRight,
			FullScreen,
			BottomLeft,
			BottomRight
		}

		public enum DockPositions
		{
			None,
			Left,
			Right,
			TopLeft,
			TopRight,
			FullScreen,
			BottomLeft,
			BottomRight
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public struct MARGINS
		{
			public int leftWidth;

			public int rightWidth;

			public int topHeight;

			public int bottomHeight;
		}

		public class FormDraggingEventArgs : EventArgs
		{
			private Point _cursorPosition;

			private DockIndicators _dockingIndicator;

			public Point CursorPosition
			{
				get
				{
					return _cursorPosition;
				}
			}

			public DockIndicators ShownIndicator
			{
				get
				{
					return _dockingIndicator;
				}
			}

			public FormDraggingEventArgs(Point cursorPosition, DockIndicators dockingIndicator)
			{
				_cursorPosition = cursorPosition;
				_dockingIndicator = dockingIndicator;
			}
		}

		public class DockChangedEventArgs : EventArgs
		{
			private DockPositions _dockPosition;

			public DockPositions DockPosition
			{
				get
				{
					return _dockPosition;
				}
			}

			public DockChangedEventArgs(DockPositions dockPosition)
			{
				_dockPosition = dockPosition;
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		public class xDesigner : ComponentDesigner
		{
			private DesignerActionListCollection actionLists;

			public override DesignerActionListCollection ActionLists
			{
				get
				{
					if (actionLists == null)
						actionLists = new DesignerActionListCollection
						{
							new xComponentActionList(base.Component)
						};
					return actionLists;
				}
			}

			private xDesigner()
			{
			}
		}

		public class xComponentActionList : DesignerActionList
		{
			private xFormDock xComponent;

			private DesignerActionUIService designerActionUISvc = null;

			public bool AllowFormDragging
			{
				get
				{
                    return xComponent.AllowFormDragging;
				}
				set
				{
                    SetValue(xComponent, "AllowFormDragging", value);
				}
			}

			public bool AllowFormResizing
			{
				get
				{
                    return xComponent.AllowFormResizing;
				}
				set
				{
                    SetValue(xComponent, "AllowFormResizing", value);
				}
			}

			public bool ShowCursorChanges
			{
				get
				{
                    return xComponent.ShowCursorChanges;
				}
				set
				{
                    SetValue(xComponent, "ShowCursorChanges", value);
				}
			}

			public bool ShowDockingIndicators
			{
				get
				{
                    return xComponent.ShowDockingIndicators;
				}
				set
				{
                    SetValue(xComponent, "ShowDockingIndicators", value);
				}
			}

			public xComponentActionList(IComponent component)
				: base(component)
			{
                xComponent = component as xFormDock;
				designerActionUISvc = GetService(typeof(DesignerActionUIService)) as DesignerActionUIService;
				AutoShow = true;
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
					new DesignerActionTextItem("(Press \"Tab\" to navigate the properties list)      ", "Common Tasks"),
					new DesignerActionPropertyItem("AllowFormDragging", "AllowFormDragging", "Common Tasks", GetPropertyDescriptor(base.Component, "AllowFormDragging").Description),
					new DesignerActionPropertyItem("AllowFormResizing", "AllowFormResizing", "Common Tasks", GetPropertyDescriptor(base.Component, "AllowFormResizing").Description),
					new DesignerActionPropertyItem("ShowCursorChanges", "ShowCursorChanges", "Common Tasks", GetPropertyDescriptor(base.Component, "ShowCursorChanges").Description),
					new DesignerActionPropertyItem("ShowDockingIndicators", "ShowDockingIndicators", "Common Tasks", GetPropertyDescriptor(base.Component, "ShowDockingIndicators").Description)
				};
			}
		}

		private bool _drag;

		private bool _dockedLeft;

		private bool _dockedRight;

		private bool _inUndockingMode;

		private bool _allowDockingKeys;

		private bool _allowFormDragging;

		private bool _allowFormResizing;

		private bool _showCursorChanges;

		private bool _allowFormDropShadow;

		private bool _showDockingIndicators;

		private bool _allowOpacityChangesWhileDragging;

		private Size _lastSize;

		private Size _currentSize;

		private Point _currentLocation;

		private Color _dockingIndicatorsColor;

		private int _usages;

		private int _mouseX;

		private int _mouseY;

		private int _normalScreenHeight = Screen.PrimaryScreen.Bounds.Height;

		private int _workspaceScreenHeight = Screen.PrimaryScreen.WorkingArea.Height;

		private double _formDraggingOpacity;

		private double _dockingIndicatorsOpacity;

		private double _initialParentFormOpacity;

		private Dictionary<string, bool> _dragControls = new Dictionary<string, bool>();

		private Screen _screen;

		private WindowHandler _windowHandler;

		private FormWindowStates _activeState;

		private xWindowStates _legacyState;

		private FormDraggingEventArgs _dragArguments;

		private Indicator _snapArea = new Indicator();

		private FormBorderStyle _initialFormBorderStyle;

		private ContainerControl _containerControl = null;

		private DockPositions _position = DockPositions.None;

		private DockOptions _dockingOptions = new DockOptions();

		private DockIndicators _dockingIndicator = DockIndicators.None;

		private TitleBarOptions _titleBarOptions = new TitleBarOptions();

		private FormBorderOptions _formBorderOptions = new FormBorderOptions();

		private List<Control> _dragControlsCollection = new List<Control>();

		private bool _isAeroEnabled = false;

		private bool _isDraggingEnabled = false;

		private const int WM_NCHITTEST = 132;

		private const int WS_MINIMIZEBOX = 131072;

		private const int HTCLIENT = 1;

		private const int HTCAPTION = 2;

		private const int CS_DBLCLKS = 8;

		private const int CS_DROPSHADOW = 131072;

		private const int WM_NCPAINT = 133;

		private const int WM_ACTIVATEAPP = 28;

		private IContainer components = null;

		[Browsable(false)]
		[Category("x Properties")]
        private bool allowHidingBottomRegion = true;

        public bool AllowHidingBottomRegion
        {
            get { return allowHidingBottomRegion; }
            set { allowHidingBottomRegion = value; }
        }


		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Description("Sets Form border options that can be used to add and style borders on the parent form.")]
		[Category("x Properties")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormBorderOptions BorderOptions
		{
			get
			{
				return _formBorderOptions;
			}
		}

		[Category("x Properties")]
		[Description("Sets the docking options to be used in the parent form.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DockOptions DockingOptions
		{
			get
			{
				return _dockingOptions;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("Sets the list of options for selecting and managing any control that is set as the parent form's Title Bar.")]
		[Category("x Properties")]
		public TitleBarOptions TitleBarOptions
		{
			get
			{
				return _titleBarOptions;
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("Miscellaneous")]
		[Description("Sets a value indicating whether the parent form will allow the standard Windows docking keys to be supported at runtime.")]
		private bool AllowDockingKeys
		{
			get
			{
				return _allowDockingKeys;
			}
			set
			{
				_allowDockingKeys = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form will be draggable.")]
		public bool AllowFormDragging
		{
			get
			{
				return _allowFormDragging;
			}
			set
			{
				_allowFormDragging = value;
				try
				{
					if (ParentForm != null)
					{
						if (value)
						{
							ParentForm.MouseUp += OnMouseUp;
							ParentForm.MouseMove += OnMouseMove;
							ParentForm.MouseDown += OnMouseDown;
						}
						else
						{
							ParentForm.MouseUp -= OnMouseUp;
							ParentForm.MouseMove -= OnMouseMove;
							ParentForm.MouseDown -= OnMouseDown;
						}
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form will have a drop-shadow along its borders.")]
		public bool AllowFormDropShadow
		{
			get
			{
				return _allowFormDropShadow;
			}
			set
			{
				_allowFormDropShadow = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form will be resizable if borderless.")]
		public bool AllowFormResizing
		{
			get
			{
				return _allowFormResizing;
			}
			set
			{
				_allowFormResizing = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form's opacity or transparency will be changed whenever it's being dragged on screen.")]
		public bool AllowOpacityChangesWhileDragging
		{
			get
			{
				return _allowOpacityChangesWhileDragging;
			}
			set
			{
				_allowOpacityChangesWhileDragging = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether cursor-changes will be provided when about to dock the parent form.")]
		public bool ShowCursorChanges
		{
			get
			{
				return _showCursorChanges;
			}
			set
			{
				_showCursorChanges = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the docking indicators will be previewed before the parent form is docked.")]
		public bool ShowDockingIndicators
		{
			get
			{
				return _showDockingIndicators;
			}
			set
			{
				_showDockingIndicators = value;
			}
		}

		[Description("Sets the color of the docking indicators.")]
		[Category("x Properties")]
		public Color DockingIndicatorsColor
		{
			get
			{
				return _dockingIndicatorsColor;
			}
			set
			{
				_dockingIndicatorsColor = value;
			}
		}

		[Description("Sets the opacity or transparency of the docking indicators.")]
		[Category("x Properties")]
		[TypeConverter(typeof(OpacityConverter))]
		public double DockingIndicatorsOpacity
		{
			get
			{
				return _dockingIndicatorsOpacity;
			}
			set
			{
				_dockingIndicatorsOpacity = value;
			}
		}

		[TypeConverter(typeof(OpacityConverter))]
		[Description("Sets the opacity or transparency of the parent form when dragging.")]
		[Category("x Properties")]
		public double FormDraggingOpacity
		{
			get
			{
				return _formDraggingOpacity;
			}
			set
			{
				_formDraggingOpacity = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[Category("x Properties")]
		[Description("Sets the parent form's window-state. This mimics the standard \"System.Windows.Forms.FormWindowState\" enumeration but has been tailored for handling borderless forms.")]
		public FormWindowStates WindowState
		{
			get
			{
				return _activeState;
			}
			set
			{
				_activeState = value;
				try
				{
					if (base.DesignMode)
						return;
					_screen = Screen.FromControl(ParentForm);
					switch (value)
					{
					case FormWindowStates.Normal:
						ParentForm.Size = _currentSize;
						ParentForm.Location = _currentLocation;
						break;
					case FormWindowStates.Maximized:
						_currentLocation = ParentForm.Location;
						ParentForm.Top = _screen.WorkingArea.Top;
						ParentForm.Left = _screen.WorkingArea.Left;
						_normalScreenHeight = _screen.Bounds.Height;
						_workspaceScreenHeight = _screen.WorkingArea.Height;
						ParentForm.Width = _screen.WorkingArea.Width;
						if (_normalScreenHeight == _workspaceScreenHeight)
							ParentForm.Height = _screen.WorkingArea.Height - 1;
						else
							ParentForm.Height = _screen.WorkingArea.Height;
						break;
					case FormWindowStates.Minimized:
						ParentForm.WindowState = FormWindowState.Minimized;
						break;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[Obsolete("This property has been deprecated. Please use 'WindowState' instead.")]
		[Category("x Properties")]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Sets the parent form's window-state. This mimics the standard \"System.Windows.Forms.FormWindowState\" enumeration but has been tailored for handling borderless forms.")]
		public xWindowStates xWindowState
		{
			get
			{
				return _legacyState;
			}
			set
			{
				_legacyState = value;
				try
				{
					if (base.DesignMode)
						return;
					_screen = Screen.FromControl(ParentForm);
					switch (value)
					{
					case xWindowStates.Normal:
						ParentForm.Size = _currentSize;
						ParentForm.Location = _currentLocation;
						break;
					case xWindowStates.Maximized:
						_currentLocation = ParentForm.Location;
						ParentForm.Top = _screen.WorkingArea.Top;
						ParentForm.Left = _screen.WorkingArea.Left;
						_normalScreenHeight = _screen.Bounds.Height;
						_workspaceScreenHeight = _screen.WorkingArea.Height;
						ParentForm.Width = _screen.WorkingArea.Width;
						if (_normalScreenHeight == _workspaceScreenHeight)
							ParentForm.Height = _screen.WorkingArea.Height - 1;
						else
							ParentForm.Height = _screen.WorkingArea.Height;
						break;
					case xWindowStates.Minimized:
						ParentForm.WindowState = FormWindowState.Minimized;
						break;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ContainerControl ContainerControl
		{
			get
			{
				return _containerControl;
			}
			set
			{
				try
				{
					_containerControl = value;
					if (value != null)
					{
						if (_allowFormDragging)
						{
							((Form)value).MouseUp += OnMouseUp;
							((Form)value).MouseMove += OnMouseMove;
							((Form)value).MouseDown += OnMouseDown;
						}
						else
						{
							((Form)value).MouseUp -= OnMouseUp;
							((Form)value).MouseMove -= OnMouseMove;
							((Form)value).MouseDown -= OnMouseDown;
						}
						if (_allowDockingKeys)
							((Form)value).KeyDown += OnKeyDown;
						else
							((Form)value).KeyDown -= OnKeyDown;
						((Form)value).Load += OnLoad;
						((Form)value).ResizeEnd += OnResizeEnd;
						ProvideFirstTimeOptionsGuide();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public Form ParentForm
		{
			get
			{
				return (Form)ContainerControl;
			}
			set
			{
				ContainerControl = value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				base.Site = value;
				if (value == null)
					return;
				IDesignerHost designerHost = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (designerHost != null)
				{
					IComponent rootComponent = designerHost.RootComponent;
					if (rootComponent is ContainerControl)
						ContainerControl = rootComponent as ContainerControl;
				}
			}
		}

		[Description("Occurs whenever the parent form is being dragged.")]
		[Category("x Events")]
		public event EventHandler<FormDraggingEventArgs> FormDragging = null;

		[Category("x Events")]
		[Description("Occurs whenever the parent form's dock position has been changed.")]
		public event EventHandler<DockChangedEventArgs> DockChanged = null;

		public xFormDock()
		{
			InitializeComponent();
			AllowDockingKeys = false;
			ShowCursorChanges = true;
			AllowFormDropShadow = true;
			ShowDockingIndicators = true;
			DockingOptions.DockAll = true;
			AllowOpacityChangesWhileDragging = false;
			FormDraggingOpacity = 0.9;
			DockingIndicatorsOpacity = 0.5;
			DockingIndicatorsColor = Color.FromArgb(202, 215, 233);
			_titleBarOptions.xFormDock = this;
		}

		public void AddBottomSeparator(Color color, int height = 1, bool dockSeparator = false)
		{
			try
			{
				Panel panel = new Panel
				{
					Enabled = false,
					BackColor = color,
					Size = new Size(ParentForm.Width, height)
				};
				if (dockSeparator)
					panel.Dock = DockStyle.Bottom;
				else
				{
					panel.Location = new Point(0, ParentForm.Height - height);
					panel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				}
				panel.BringToFront();
				ParentForm.Controls.Add(panel);
			}
			catch (Exception)
			{
			}
		}

		public void SubscribeControlToDragEvents(Control control, bool isPartOfTitleBar = false)
		{
			try
			{
				control.MouseUp += OnMouseUp;
				control.MouseMove += OnMouseMove;
				control.MouseDown += OnMouseDown;
				if (isPartOfTitleBar)
					control.MouseDoubleClick += OnDoubleClickTitleBar;
			}
			catch (Exception)
			{
			}
		}

		public void SubscribeControlsToDragEvents(Control[] controls, bool arePartOfTitleBar = false)
		{
			try
			{
				foreach (Control control in controls)
				{
					control.MouseUp += OnMouseUp;
					control.MouseMove += OnMouseMove;
					control.MouseDown += OnMouseDown;
					if (arePartOfTitleBar)
						control.MouseDoubleClick += OnDoubleClickTitleBar;
				}
			}
			catch (Exception)
			{
			}
		}

		public void UnsubscribeControlToDragEvents(Control control, bool wasPartOfTitleBar = false)
		{
			try
			{
				control.MouseUp -= OnMouseUp;
				control.MouseMove -= OnMouseMove;
				control.MouseDown -= OnMouseDown;
				if (wasPartOfTitleBar)
					control.MouseDoubleClick -= OnDoubleClickTitleBar;
			}
			catch (Exception)
			{
			}
		}

		public void UnsubscribeControlsToDragEvents(Control[] controls, bool werePartOfTitleBar = false)
		{
			try
			{
				foreach (Control control in controls)
				{
					control.MouseUp -= OnMouseUp;
					control.MouseMove -= OnMouseMove;
					control.MouseDown -= OnMouseDown;
					if (werePartOfTitleBar)
						control.MouseDoubleClick -= OnDoubleClickTitleBar;
				}
			}
			catch (Exception)
			{
			}
		}

		private bool ParentFormIsNormal()
		{
			if (ParentForm.Location == _currentLocation && ParentForm.Size == _currentSize)
				return true;
			return false;
		}

		private bool DockedLeft()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(0) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height)
				return true;
			return false;
		}

		private bool DockedTopLeft()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(0) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height / 2)
				return true;
			return false;
		}

		private bool DockedBottomLeft()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(0, Convert.ToInt32((double)_screen.WorkingArea.Height / 2.0)) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height / 2)
				return true;
			return false;
		}

		private bool DockedRight()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(Convert.ToInt32((double)_screen.WorkingArea.Width / 2.0)) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height)
				return true;
			return false;
		}

		private bool DockedTopRight()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(_screen.WorkingArea.Width / 2) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height / 2)
				return true;
			return false;
		}

		private bool DockedBottomRight()
		{
			_screen = Screen.FromControl(ParentForm);
			if (ParentForm.Location == new Point(Convert.ToInt32((double)_screen.WorkingArea.Width / 2.0), Convert.ToInt32((double)_screen.WorkingArea.Height / 2.0)) && ParentForm.Width == _screen.WorkingArea.Width / 2 && ParentForm.Height == _screen.WorkingArea.Height / 2)
				return true;
			return false;
		}

		private void ExpandWindow()
		{
			if (DockingOptions.DockFullScreen)
			{
				if (_inUndockingMode)
				{
					WindowState = FormWindowStates.Normal;
					_inUndockingMode = false;
				}
				if (WindowState == FormWindowStates.Normal)
				{
					_currentSize = ParentForm.Size;
					_position = DockPositions.FullScreen;
					_currentLocation = ParentForm.Location;
					if (_currentLocation.Y <= -1)
						_currentLocation.Y = 0;
					if (_currentLocation.X <= -1)
						_currentLocation.X = 0;
					WindowState = FormWindowStates.Maximized;
				}
				else if (WindowState == FormWindowStates.Maximized)
				{
					_position = DockPositions.None;
					WindowState = FormWindowStates.Normal;
				}
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.FullScreen;
				this.DockChanged(this, new DockChangedEventArgs(DockPositions.FullScreen));
			}
		}

		private void DockLeft()
		{
			if (DockingOptions.DockLeft)
			{
				_dockedLeft = true;
				_lastSize = ParentForm.Size;
				_screen = Screen.FromControl(ParentForm);
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height;
				ParentForm.Location = new Point(_screen.WorkingArea.Left);
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.Left;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void DockTopLeft()
		{
			if (DockingOptions.DockTopLeft)
			{
				_dockedLeft = true;
				_lastSize = ParentForm.Size;
				_screen = Screen.FromControl(ParentForm);
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height / 2;
				ParentForm.Location = new Point(_screen.WorkingArea.Left, _screen.WorkingArea.Top);
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.TopLeft;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void DockBottomLeft()
		{
			if (DockingOptions.DockBottomLeft)
			{
				_dockedLeft = true;
				_screen = Screen.FromControl(ParentForm);
				_lastSize = ParentForm.Size;
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height / 2;
				ParentForm.Location = new Point(_screen.WorkingArea.Left, Convert.ToInt32(_screen.WorkingArea.Bottom - ParentForm.Height));
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.BottomLeft;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void DockRight()
		{
			if (DockingOptions.DockRight)
			{
				_dockedRight = true;
				_screen = Screen.FromControl(ParentForm);
				_lastSize = ParentForm.Size;
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height;
				ParentForm.Location = new Point(_screen.WorkingArea.Right - ParentForm.Width);
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.Right;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void DockTopRight()
		{
			if (DockingOptions.DockTopRight)
			{
				_dockedRight = true;
				_screen = Screen.FromControl(ParentForm);
				_lastSize = ParentForm.Size;
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height / 2;
				ParentForm.Location = new Point(Convert.ToInt32(_screen.WorkingArea.Right - ParentForm.Width));
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.TopRight;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void DockBottomRight()
		{
			if (DockingOptions.DockBottomRight)
			{
				_dockedRight = true;
				_screen = Screen.FromControl(ParentForm);
				_lastSize = ParentForm.Size;
				ParentForm.Width = _screen.WorkingArea.Width / 2;
				ParentForm.Height = _screen.WorkingArea.Height / 2;
				ParentForm.Location = new Point(Convert.ToInt32(_screen.WorkingArea.Right - ParentForm.Width), Convert.ToInt32(_screen.WorkingArea.Bottom - ParentForm.Height));
			}
			if (this.DockChanged != null)
			{
				_position = DockPositions.BottomRight;
				this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		private void IndicateFullScreenDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockFullScreen)
			{
				int num = 0;
				int num2 = 0;
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Top = _screen.WorkingArea.Top;
				_snapArea.Left = _screen.WorkingArea.Left;
				num = _screen.Bounds.Height;
				num2 = _screen.WorkingArea.Height;
				_snapArea.Width = _screen.WorkingArea.Width;
				if (num != num2)
					_snapArea.Height = _screen.WorkingArea.Height;
				else
					_snapArea.Height = _screen.WorkingArea.Height - 1;
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateLeftDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockLeft)
			{
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Location = new Point(_screen.WorkingArea.Left);
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height;
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateTopLeftDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockTopLeft)
			{
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height / 2;
				_snapArea.Location = new Point(_screen.WorkingArea.Left, _screen.WorkingArea.Top);
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateBottomLeftDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockBottomLeft)
			{
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height / 2;
				_snapArea.Location = new Point(_screen.WorkingArea.Left, Convert.ToInt32(_screen.WorkingArea.Bottom - _snapArea.Height));
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateRightDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockRight)
			{
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height;
				_snapArea.Location = new Point(_screen.WorkingArea.Right - _snapArea.Width);
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateTopRightDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockTopRight)
			{
				_screen = Screen.FromControl(ParentForm);
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height / 2;
				_snapArea.Location = new Point(Convert.ToInt32(_screen.WorkingArea.Right - _snapArea.Width));
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateBottomRightDocking()
		{
			if (ShowDockingIndicators && DockingOptions.DockBottomRight)
			{
				_snapArea.Width = _screen.WorkingArea.Width / 2;
				_snapArea.Height = _screen.WorkingArea.Height / 2;
				_snapArea.Location = new Point(Convert.ToInt32(_screen.WorkingArea.Right - _snapArea.Width), Convert.ToInt32(_screen.WorkingArea.Bottom - _snapArea.Height));
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void IndicateNormalPositioning()
		{
			if (!(_currentSize == ParentForm.Size) && ShowDockingIndicators)
			{
				_snapArea.Size = _currentSize;
				_snapArea.Location = _currentLocation;
				_snapArea.Opacity = DockingIndicatorsOpacity;
				_snapArea.BackColor = DockingIndicatorsColor;
				_snapArea.Show();
				ParentForm.Activate();
			}
		}

		private void HideDockingIndicators()
		{
			_snapArea.Hide();
		}

		private void ShowDockingAreaIndicators()
		{
		}

		public void ApplyShadows()
		{
			int attrValue = 2;
			DwmSetWindowAttribute(ParentForm.Handle, 2, ref attrValue, 4);
			MARGINS mARGINS = default(MARGINS);
			mARGINS.bottomHeight = 1;
			mARGINS.leftWidth = 0;
			mARGINS.rightWidth = 0;
			mARGINS.topHeight = 0;
			MARGINS pMarInset = mARGINS;
			DwmExtendFrameIntoClientArea(ParentForm.Handle, ref pMarInset);
		}

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsCompositionEnabled()
		{
			if (Environment.OSVersion.Version.Major < 6)
				return false;
			bool enabled;
			DwmIsCompositionEnabled(out enabled);
			return enabled;
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(out bool enabled);

		[DllImport("Gdi32.dll")]
		private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		private bool CheckIfAeroIsEnabled()
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				int pfEnabled = 0;
				DwmIsCompositionEnabled(ref pfEnabled);
				return pfEnabled == 1;
			}
			return false;
		}

		private void AddFormBorders(bool top, bool bottom, bool left, bool right)
		{
			try
			{
				if (top)
				{
					Panel value = new Panel
					{
						Enabled = false,
						Dock = DockStyle.Top,
						BackColor = BorderOptions.TopBorder.BorderColor,
						Size = new Size(ParentForm.Width, BorderOptions.TopBorder.BorderThickness)
					};
					ParentForm.Controls.Add(value);
				}
				if (bottom)
				{
					Button value2 = new Button
					{
						Enabled = true,
						BackColor = BorderOptions.BottomBorder.BorderColor,
						Size = new Size(ParentForm.Width, BorderOptions.BottomBorder.BorderThickness + 20),
						Location = new Point(0, ParentForm.Height - BorderOptions.BottomBorder.BorderThickness)
					};
					ParentForm.Controls.Add(value2);
				}
				if (left)
				{
					Panel value3 = new Panel
					{
						Enabled = false,
						Dock = DockStyle.Left,
						BackColor = BorderOptions.LeftBorder.BorderColor,
						Size = new Size(BorderOptions.LeftBorder.BorderThickness, ParentForm.Height)
					};
					ParentForm.Controls.Add(value3);
				}
				if (right)
				{
					Panel value4 = new Panel
					{
						Enabled = false,
						Dock = DockStyle.Right,
						BackColor = BorderOptions.RightBorder.BorderColor,
						Size = new Size(BorderOptions.LeftBorder.BorderThickness, ParentForm.Height)
					};
					ParentForm.Controls.Add(value4);
				}
			}
			catch (Exception)
			{
			}
		}

		private void MaskBottomMargin()
		{
			try
			{
				if (AllowHidingBottomRegion)
				{
					xImageButtonExtended _xImageButtonExtended = new xImageButtonExtended();
					if (AllowFormResizing)
						_xImageButtonExtended.Cursor = Cursors.SizeNS;
					else
						_xImageButtonExtended.Cursor = ParentForm.Cursor;
					_xImageButtonExtended.Enabled = false;
					_xImageButtonExtended.AllowZooming = false;
					_xImageButtonExtended.AllowAnimations = false;
					_xImageButtonExtended.FadeWhenInactive = false;
					_xImageButtonExtended.ShowCursorChanges = false;
					_xImageButtonExtended.Size = new Size(ParentForm.Width, 2);
					_xImageButtonExtended.Location = new Point(0, ParentForm.Height - 2);
					_xImageButtonExtended.Image = CreateBitmap(Color.Transparent, _xImageButtonExtended.Size);
					_xImageButtonExtended.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
					_xImageButtonExtended.BringToFront();
					ParentForm.Controls.Add(_xImageButtonExtended);
				}
			}
			catch (Exception)
			{
			}
		}

		private Bitmap CreateBitmap(Color color, Size size)
		{
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			bitmap.SetPixel(0, 0, color);
			return new Bitmap(bitmap, size);
		}

		private bool IsInDesignMode()
		{
			if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
				return true;
			return false;
		}

		private void ProvideFirstTimeOptionsGuide()
		{
			try
			{
				if (_usages != 1 || ParentForm.FormBorderStyle == FormBorderStyle.None)
					return;
				if (IsInDesignMode())
				{
					DialogResult dialogResult = MessageBox.Show(ParentForm, "Your form (" + ParentForm.Name + "), is currently not set to a borderless window.\n\nDo you wish to set it to a borderless window?\n(Automatic resizing will be enabled by default)", "x Form Dock", MessageBoxButtons.YesNo);
					if (dialogResult == DialogResult.Yes)
					{
						ParentForm.FormBorderStyle = FormBorderStyle.None;
						AllowFormResizing = true;
					}
				}
				_usages++;
			}
			catch (Exception)
			{
			}
		}

		private void OnLoad(object sender, EventArgs e)
		{
			_currentSize = ParentForm.Size;
			_currentLocation = ParentForm.Location;
			_initialParentFormOpacity = ParentForm.Opacity;
			if (AllowFormResizing && ParentForm.FormBorderStyle == FormBorderStyle.None)
				_windowHandler = new WindowHandler(ParentForm, true);
			if (AllowFormDropShadow)
			{
				ApplyShadows();
				MaskBottomMargin();
			}
		}

		private void OnResizeEnd(object sender, EventArgs e)
		{
			_currentSize = ParentForm.Size;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (AllowDockingKeys)
				;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public void OnMouseUp(object sender, MouseEventArgs e)
		{
			_drag = false;
			_screen = Screen.FromControl(ParentForm);
			if (ShowCursorChanges)
				ParentForm.Cursor = Cursors.Default;
			if (AllowOpacityChangesWhileDragging)
				ParentForm.Opacity = _initialParentFormOpacity;
			HideDockingIndicators();
			if (_inUndockingMode)
				_inUndockingMode = false;
			if (_dockingIndicator != 0 && this.FormDragging != null)
			{
				_dockingIndicator = DockIndicators.None;
				_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
				this.FormDragging(this, _dragArguments);
			}
			if (Cursor.Position.X == _screen.WorkingArea.Right - 1 || Cursor.Position.X <= 0 || Cursor.Position.Y != _screen.WorkingArea.Top)
			{
				if (Cursor.Position.X != _screen.WorkingArea.Left || Cursor.Position.Y <= 0 || Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
				{
					if (Cursor.Position.X == _screen.WorkingArea.Left && Cursor.Position.Y == 0)
						DockTopLeft();
					else if (Cursor.Position.X != _screen.WorkingArea.Right - 1 || Cursor.Position.Y <= 0 || Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
					{
						if (Cursor.Position.X != _screen.WorkingArea.Right - 1 || Cursor.Position.Y != 0)
						{
							if (Cursor.Position.X == _screen.WorkingArea.Left && Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
							{
								DockBottomLeft();
								return;
							}
							if (Cursor.Position.X == _screen.WorkingArea.Right - 1 && Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
							{
								DockBottomRight();
								return;
							}
							_dockedLeft = false;
							_dockedRight = false;
							if (_position != 0 && _position != DockPositions.FullScreen && this.DockChanged != null)
								this.DockChanged(this, new DockChangedEventArgs(DockPositions.None));
							_position = DockPositions.None;
						}
						else
							DockTopRight();
					}
					else
					{
						DockRight();
					}
				}
				else
					DockLeft();
			}
			else
				ExpandWindow();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public void OnMouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if (!_drag)
					_initialParentFormOpacity = ParentForm.Opacity;
				if (!_drag)
					return;
				_screen = Screen.FromControl(ParentForm);
				_normalScreenHeight = _screen.Bounds.Height;
				_workspaceScreenHeight = _screen.WorkingArea.Height;
				if (ShowCursorChanges)
					ParentForm.Cursor = Cursors.SizeAll;
				if (AllowOpacityChangesWhileDragging)
					ParentForm.Opacity = FormDraggingOpacity;
				if (_inUndockingMode || _dockedLeft || _dockedRight)
				{
					if (TitleBarOptions.TitleBarControl.Name == ParentForm.Name)
						ParentForm.Top = Cursor.Position.Y + (ParentForm.Top - ParentForm.Bottom) / 2;
					else
						ParentForm.Top = Cursor.Position.Y - _mouseY;
					ParentForm.Left = Cursor.Position.X - (ParentForm.Right - ParentForm.Left) / 2;
					if (_dockedLeft || _dockedRight)
						ParentForm.Size = _currentSize;
				}
				else
				{
					ParentForm.Top = Cursor.Position.Y - _mouseY;
					ParentForm.Left = Cursor.Position.X - _mouseX;
				}
				if (WindowState != FormWindowStates.Maximized)
				{
					if (Cursor.Position.X != _screen.WorkingArea.Right - 1 && Cursor.Position.X > 0 && Cursor.Position.Y == _screen.WorkingArea.Top)
					{
						ShowDockingAreaIndicators();
						IndicateFullScreenDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeWE;
						_dockingIndicator = DockIndicators.FullScreen;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Left && Cursor.Position.Y > 0 && Cursor.Position.Y < _screen.WorkingArea.Bottom - 3)
					{
						ShowDockingAreaIndicators();
						IndicateLeftDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.Left;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Left && Cursor.Position.Y == 0)
					{
						ShowDockingAreaIndicators();
						IndicateTopLeftDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.TopLeft;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Right - 1 && Cursor.Position.Y > 0 && Cursor.Position.Y < _screen.WorkingArea.Bottom - 3)
					{
						ShowDockingAreaIndicators();
						IndicateRightDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.Right;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Right - 1 && Cursor.Position.Y == 0)
					{
						ShowDockingAreaIndicators();
						IndicateTopRightDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.TopRight;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Left && Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
					{
						ShowDockingAreaIndicators();
						IndicateBottomLeftDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.BottomLeft;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else if (Cursor.Position.X == _screen.WorkingArea.Right - 1 && Cursor.Position.Y >= _screen.WorkingArea.Bottom - 3)
					{
						ShowDockingAreaIndicators();
						IndicateBottomRightDocking();
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeNS;
						_dockingIndicator = DockIndicators.BottomRight;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					else
					{
						HideDockingIndicators();
						_dockingIndicator = DockIndicators.None;
						_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
					}
					if (ParentForm.Width == _screen.WorkingArea.Width)
					{
						ParentForm.Width = _lastSize.Width;
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeAll;
						HideDockingIndicators();
					}
					else if (ParentForm.Height == _screen.WorkingArea.Height)
					{
						if (_normalScreenHeight == _workspaceScreenHeight)
							ParentForm.Height = _currentSize.Height;
						else
							ParentForm.Height = _currentSize.Height + (_normalScreenHeight - _workspaceScreenHeight);
						if (ShowCursorChanges)
							ParentForm.Cursor = Cursors.SizeAll;
						HideDockingIndicators();
					}
				}
				else
				{
					_inUndockingMode = true;
					HideDockingIndicators();
					ParentForm.Size = _currentSize;
					if (ShowCursorChanges)
						ParentForm.Cursor = Cursors.SizeAll;
					WindowState = FormWindowStates.Normal;
					_dockingIndicator = DockIndicators.None;
					_dragArguments = new FormDraggingEventArgs(new Point(Cursor.Position.X - _mouseX, Cursor.Position.Y - _mouseY), _dockingIndicator);
				}
				if (this.FormDragging != null)
					this.FormDragging(this, _dragArguments);
			}
			catch (Exception)
			{
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public void OnMouseDown(object sender, MouseEventArgs e)
		{
			_drag = true;
			_mouseX = Cursor.Position.X - ParentForm.Left;
			_mouseY = Cursor.Position.Y - ParentForm.Top;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnDoubleClickTitleBar(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				ExpandWindow();
				if (this.DockChanged != null)
					this.DockChanged(this, new DockChangedEventArgs(_position));
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			components = new Container();
		}
	}
}
