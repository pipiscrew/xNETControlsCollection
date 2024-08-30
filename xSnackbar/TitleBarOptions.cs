using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Utilities.xSnackbar
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	[Description("Provides a list of options for selecting and managing any control that is to be used as the parent form's Title Bar.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TitleBarOptions
	{
		private Control _titleBar;

		private xFormDock _xFormDock;

		private Color _initialDockingIndicatorsColor;

		private bool _allowFormDragging;

		private bool _doubleClickToExpandWindow;

		private bool _useBackColorOnDockingIndicators;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		internal xFormDock xFormDock
		{
			get
			{
				return _xFormDock;
			}
			set
			{
				_xFormDock = value;
			}
		}

		[ParenthesizePropertyName(true)]
		[Category("x Properties")]
		[Description("Sets the control to be used as the parent form's Title Bar.")]
		public Control TitleBarControl
		{
			get
			{
				return _titleBar;
			}
			set
			{
				try
				{
					_titleBar = value;
					if (value != null)
					{
						if (_allowFormDragging)
						{
							value.MouseUp += xFormDock.OnMouseUp;
							value.MouseMove += xFormDock.OnMouseMove;
							value.MouseDown += xFormDock.OnMouseDown;
						}
						else
						{
							value.MouseUp -= xFormDock.OnMouseUp;
							value.MouseMove -= xFormDock.OnMouseMove;
							value.MouseDown -= xFormDock.OnMouseDown;
						}
						if (_doubleClickToExpandWindow)
							value.MouseDoubleClick += xFormDock.OnDoubleClickTitleBar;
						else
							value.MouseDoubleClick -= xFormDock.OnDoubleClickTitleBar;
						if (_useBackColorOnDockingIndicators)
							UseBackColorOnDockingIndicators = true;
						else
							UseBackColorOnDockingIndicators = false;
					}
					else
						UseBackColorOnDockingIndicators = false;
				}
				catch (Exception)
				{
				}
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the Title Bar control will expand the parent form once a user double-clicks on it.")]
		public bool DoubleClickToExpandWindow
		{
			get
			{
				return _doubleClickToExpandWindow;
			}
			set
			{
				_doubleClickToExpandWindow = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the the docking indicators will inherit the Title Bar's background color.")]
		public bool UseBackColorOnDockingIndicators
		{
			get
			{
				return _useBackColorOnDockingIndicators;
			}
			set
			{
				_useBackColorOnDockingIndicators = value;
				if (_titleBar != null)
				{
					if (value)
					{
						_titleBar.BackColorChanged += OnTitleBarBackColorChanged;
						if (!(_xFormDock.DockingIndicatorsColor == Color.Empty))
						{
							if (_initialDockingIndicatorsColor != Color.Empty)
								_initialDockingIndicatorsColor = _xFormDock.DockingIndicatorsColor;
						}
						else
							_initialDockingIndicatorsColor = Color.FromArgb(202, 215, 233);
						_xFormDock.DockingIndicatorsColor = _titleBar.BackColor;
					}
					else
					{
						_titleBar.BackColorChanged -= OnTitleBarBackColorChanged;
						if (_initialDockingIndicatorsColor != Color.Empty)
							_xFormDock.DockingIndicatorsColor = _initialDockingIndicatorsColor;
					}
				}
				else
					_xFormDock.DockingIndicatorsColor = Color.FromArgb(202, 215, 233);
			}
		}

		[Description("Sets a value indicating whether the Title Bar control will will be allowed to drag the parent form at runtime.")]
		[Category("x Properties")]
		public bool AllowFormDragging
		{
			get
			{
				return _allowFormDragging;
			}
			set
			{
				_allowFormDragging = value;
			}
		}

		public TitleBarOptions()
		{
			_allowFormDragging = true;
			_doubleClickToExpandWindow = true;
			_useBackColorOnDockingIndicators = false;
		}

		private void OnTitleBarBackColorChanged(object sender, EventArgs e)
		{
			try
			{
				if (_useBackColorOnDockingIndicators && _titleBar != null)
					_xFormDock.DockingIndicatorsColor = _titleBar.BackColor;
			}
			catch (Exception)
			{
			}
		}

		public override string ToString()
		{
			return "Title Bar: " + TitleBarControl.Name + "; Expand: " + DoubleClickToExpandWindow + "; Allow Drag: " + AllowFormDragging;
		}
	}
}
