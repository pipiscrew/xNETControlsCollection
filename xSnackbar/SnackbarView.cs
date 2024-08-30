using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using xCollection;
using Utilities.xSnackbar.Win32API;

namespace Utilities.xSnackbar.Views
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	internal class SnackbarView : Form
	{
		internal struct LASTINPUTINFO
		{
			public uint cbSize;

			public uint dwTime;
		}

		private string _content;

		private string _actionContent = string.Empty;

		private bool _hasFinished;

		private bool _showBorders;

		private bool _clickToClose;

		private bool _hasBeenInvoked;

		private bool _showClosingIcon;

		private bool _showSnackbarIcon;

		private bool _doubleClickToClose;

		private bool _allowIdleWaitTime = true;

		private int _margin;

		private uint _timeout;

		private int _duration;

		private int _durationAfterIdle;

		private int _maximumViewCount = 7;

		private int _countTopLeft = 0;

		private int _countTopCenter = 0;

		private int _countTopRight = 0;

		private int _countMiddleLeft = 0;

		private int _countMiddleCenter = 0;

		private int _countMiddleRight = 0;

		private int _countBottomLeft = 0;

		private int _countBottomCenter = 0;

		private int _countBottomRight = 0;

		private int _visibleSizeTopLeft = 0;

		private int _visibleSizeTopCenter = 0;

		private int _visibleSizeTopRight = 0;

		private int _visibleSizeMiddleLeft = 0;

		private int _visibleSizeMiddleCenter = 0;

		private int _visibleSizeMiddleRight = 0;

		private int _visibleSizeBottomLeft = 0;

		private int _visibleSizeBottomCenter = 0;

		private int _visibleSizeBottomRight = 0;

		private const int SW_SHOWNOACTIVATE = 4;

		private const int HWND_TOPMOST = -1;

		private const uint SWP_NOACTIVATE = 16u;

		private float _darkScaleFactor = 0.15f;

		private float _lightScaleFactor = 0.5f;

		private float _lightWarningScaleFactor = 0.13f;

		private Point _lastPoint;

		private Point _exitPoint;

		private Point _entryPoint;

		private Point _host = default(Point);

		private Color _iconColor = Color.Black;

		private Color _contentColor = Color.White;

		private Color _borderColor = Color.Gainsboro;

		private Color _closingIconColor = Color.Black;

		private Color _actionContentColor = Color.Black;

		private Color _actionBackColor = Color.Gainsboro;

		private xCollection.xSnackbar.Hosts _scope;

        private xCollection.xSnackbar.MessageTypes _flag;

        private xCollection.xSnackbar.Positions _placement;

		private Control _hostingControl;

		private System.Threading.Timer _timer;

		public static bool _isDirectClosure;

		public static xCollection.xSnackbar.SnackbarResult _result;

		public static Action<xCollection.xSnackbar.SnackbarResult> _action;

		private IContainer components = null;

		private System.Windows.Forms.Timer tmrTimeout;

		public Label lblMessage;

		public xImageButtonExtended pbClose;

		public xButton btnAction;

		public PictureBox pbIcon;

		protected internal Label lblBorderRight;

		protected internal Label lblBorderLeft;

		protected internal Label lblBorderBottom;

		protected internal Label lblBorderTop;

		public xFormDock dckFormDock;

		public new int Margin
		{
			get
			{
				return _margin;
			}
			set
			{
				_margin = value;
			}
		}

		private int viewsMargin = 3;

        public int ViewsMargin
        {
            get { return viewsMargin; }
            set { viewsMargin = value; }
        }


		public int Duration
		{
			get
			{
				return _duration;
			}
			set
			{
				_duration = value;
			}
		}

		public int DurationAfterIdle
		{
			get
			{
				return _durationAfterIdle;
			}
			set
			{
				_durationAfterIdle = value;
			}
		}

		public int MaximumViewCount
		{
			get
			{
				return _maximumViewCount;
			}
			set
			{
				if (value < 1)
					throw new ArgumentOutOfRangeException("The least maximum view count allowed is 1.");
				_maximumViewCount = value;
			}
		}

    private int snackbarIconLeftMargin = 12;
    public int SnackbarIconLeftMargin
    {
        get { return snackbarIconLeftMargin; }
        set { snackbarIconLeftMargin = value; }
    }


		public int ClosingIconPadding { get; set; }

    private int messageRightMargin = 20;
    public int MessageRightMargin
    {
        get { return messageRightMargin; }
        set { messageRightMargin = value; }
    }


		public string Message
		{
			get
			{
				return _content;
			}
			set
			{
				_content = value;
				lblMessage.Text = value;
				RenderSize();
			}
		}

		public string ActionText
		{
			get
			{
				return _actionContent;
			}
			set
			{
				_actionContent = value;
				btnAction.ButtonText = value;
				if (string.IsNullOrWhiteSpace(value))
					btnAction.Visible = false;
				else
					btnAction.Visible = true;
				RenderSize();
			}
		}

		public xCollection.xSnackbar.MessageTypes MessageType
		{
			get
			{
				return _flag;
			}
			set
			{
				_flag = value;
			}
		}

		public xCollection.xSnackbar.Hosts Host
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
				try
				{
					switch (value)
					{
					case xCollection.xSnackbar.Hosts.FormOwner:
						_host = base.Owner.Location;
						base.Owner.LocationChanged += OnOwnerLocationChanged;
						break;
					case xCollection.xSnackbar.Hosts.Control:
						_host = Screen.FromControl(_hostingControl).WorkingArea.Location;
						_hostingControl.LocationChanged -= OnOwnerLocationChanged;
						break;
					case xCollection.xSnackbar.Hosts.Screen:
						_host = Screen.FromControl(this).WorkingArea.Location;
						base.Owner.LocationChanged -= OnOwnerLocationChanged;
						break;
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public xCollection.xSnackbar.Positions Positioning
		{
			get
			{
				return _placement;
			}
			set
			{
				_placement = value;
			}
		}

		public Point EntryPoint
		{
			get
			{
				return _entryPoint;
			}
			set
			{
				_entryPoint = value;
			}
		}

		public Point ExitPoint
		{
			get
			{
				return _exitPoint;
			}
			set
			{
				_exitPoint = value;
			}
		}

		public Color MessageForeColor
		{
			get
			{
				return _contentColor;
			}
			set
			{
				_contentColor = value;
				lblMessage.ForeColor = value;
			}
		}

		public Color ActionForeColor
		{
			get
			{
				return _actionContentColor;
			}
			set
			{
				_actionContentColor = value;
				btnAction.ForeColor = value;
			}
		}

		public Color ClosingIconColor
		{
			get
			{
				return _closingIconColor;
			}
			set
			{
				_closingIconColor = value;
				pbClose.Image = ChangeBitmapColor((Bitmap)pbClose.Image, value);
			}
		}

		public Color ActionBackColor
		{
			get
			{
				return _actionBackColor;
			}
			set
			{
				_actionBackColor = value;
				btnAction.IdleFillColor = value;
				btnAction.IdleBorderColor = value;
				btnAction.onHoverState.FillColor = ControlPaint.Dark(value, _darkScaleFactor);
				btnAction.onHoverState.BorderColor = ControlPaint.Dark(value, _darkScaleFactor);
			}
		}

		public Color ActionBorderColor
		{
			get
			{
				return btnAction.IdleBorderColor;
			}
			set
			{
				if (value.IsEmpty)
				{
					btnAction.OnIdleState.BorderColor = ActionBackColor;
					btnAction.onHoverState.BorderColor = ActionBackColor;
					btnAction.OnPressedState.BorderColor = ActionBackColor;
					btnAction.IdleBorderColor = ActionBackColor;
				}
				else
				{
					btnAction.OnIdleState.BorderColor = value;
					btnAction.onHoverState.BorderColor = value;
					btnAction.OnPressedState.BorderColor = value;
					btnAction.IdleBorderColor = value;
				}
			}
		}

		public Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				lblBorderTop.BackColor = value;
				lblBorderLeft.BackColor = value;
				lblBorderRight.BackColor = value;
				lblBorderBottom.BackColor = value;
			}
		}

		public Font MessageFont
		{
			get
			{
				return lblMessage.Font;
			}
			set
			{
				lblMessage.Font = value;
			}
		}

		public Font ActionTextFont
		{
			get
			{
				return btnAction.Font;
			}
			set
			{
				btnAction.Font = value;
			}
		}

		public Image SnackbarIcon
		{
			get
			{
				return pbIcon.Image;
			}
			set
			{
				try
				{
					pbIcon.Image = value;
					if (value != null)
						pbIcon.Visible = true;
					else
						pbIcon.Visible = false;
					RenderSize();
				}
				catch (Exception)
				{
				}
			}
		}

		public Image SnackbarClosingIcon
		{
			get
			{
				return pbClose.Image;
			}
			set
			{
				try
				{
					pbClose.Image = value;
					RenderSize();
				}
				catch (Exception)
				{
				}
			}
		}

		public Control DisplayControl { get; set; }

		public bool FadeClosingIcon
		{
			get
			{
				return pbClose.FadeWhenInactive;
			}
			set
			{
				pbClose.FadeWhenInactive = value;
			}
		}

		public bool ZoomClosingIcon
		{
			get
			{
				return pbClose.AllowZooming;
			}
			set
			{
				pbClose.AllowZooming = value;
			}
		}

		public bool HasAvailableAction
		{
			get
			{
				if (string.IsNullOrWhiteSpace(btnAction.ButtonText))
					return false;
				return true;
			}
		}

		public bool ShowSnackbarIcon
		{
			get
			{
				return _showSnackbarIcon;
			}
			set
			{
				_showSnackbarIcon = value;
				pbIcon.Visible = value;
			}
		}

		public bool ShowClosingIcon
		{
			get
			{
				return _showClosingIcon;
			}
			set
			{
				_showClosingIcon = value;
				pbClose.Visible = value;
			}
		}

        private bool allowCustomizations = true;
        public bool AllowCustomizations
        {
            get { return allowCustomizations; }
            set { allowCustomizations = value; }
        }

        private bool allowMultipleViews = true;
        public bool AllowMultipleViews
        {
            get { return allowMultipleViews; }
            set { allowMultipleViews = value; }
        }


		public bool DoubleClickToClose
		{
			get
			{
				return _doubleClickToClose;
			}
			set
			{
				_doubleClickToClose = value;
			}
		}

		public bool ClickToClose
		{
			get
			{
				return _clickToClose;
			}
			set
			{
				_clickToClose = value;
			}
		}

		public bool IsTopMost
		{
			get
			{
				return base.TopMost;
			}
			set
			{
				base.TopMost = value;
			}
		}

		public bool ShowBorders
		{
			get
			{
				return _showBorders;
			}
			set
			{
				_showBorders = value;
				if (value)
				{
					lblBorderTop.Show();
					lblBorderLeft.Show();
					lblBorderRight.Show();
					lblBorderBottom.Show();
				}
				else
				{
					lblBorderTop.Hide();
					lblBorderLeft.Hide();
					lblBorderRight.Hide();
					lblBorderBottom.Hide();
				}
			}
		}

		public bool ShowShadows { get; set; }

		public bool AllowDragging { get; set; }

		protected override bool ShowWithoutActivation
		{
			get
			{
				return true;
			}
		}

		public SnackbarView()
		{
			InitializeComponent();
			Margin = 10;
			Message = "...";
			ActionText = "";
			ClickToClose = false;
			ShowClosingIcon = true;
			ShowSnackbarIcon = true;
			Host = xCollection.xSnackbar.Hosts.FormOwner;
			Positioning = xCollection.xSnackbar.Positions.TopCenter;
			MessageType = xCollection.xSnackbar.MessageTypes.Information;
			_duration = 7000;
			_durationAfterIdle = 3000;
			tmrTimeout.Tick += OnTick;
		}

		[DllImport("user32.dll")]
		private static extern bool SetWindowPos(int hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		private static void ShowInactiveTopMost(Form form)
		{
			ShowWindow(form.Handle, 4);
			SetWindowPos(form.Handle.ToInt32(), -1, form.Left, form.Top, form.Width, form.Height, 16u);
		}

		public new xCollection.xSnackbar.SnackbarResult Show()
		{
			try
			{
				Action action = delegate
				{
					RenderSize();
					Point entryPoint = GetEntryPoint();
					if (Host == xCollection.xSnackbar.Hosts.FormOwner)
						base.Owner.LocationChanged += OnOwnerLocationChanged;
					SetDefaultLocation(entryPoint);
					ShowInactiveTopMost(this);
					base.Show();
					base.Owner.Activate();
					if (VisibleCount() != 0)
						EntryPoint = AlignToVisibleOthers(entryPoint, Positioning);
					else
						EntryPoint = entryPoint;
					PerformTransition(false);
					_hasFinished = false;
					_hasBeenInvoked = false;
					StartNewTimer(_duration);
					tmrTimeout.Start();
				};
				if (AllowMultipleViews)
				{
					if (VisibleCount() <= MaximumViewCount - 1)
						action();
				}
				else if (VisibleCount() <= 0)
				{
					action();
				}
			}
			catch (Exception)
			{
			}
			return _result;
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner)
		{
			base.Owner = owner;
			return Show();
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content)
		{
			Message = content;
			return Show(owner);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds)
		{
			Duration = seconds;
			return Show(owner, content);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type)
		{
			MessageType = type;
			return Show(owner, content, seconds);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action)
		{
			ActionText = action;
			return Show(owner, content, seconds, type);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action, xCollection.xSnackbar.Hosts scope)
		{
			Host = scope;
			return Show(owner, content, seconds, type, action);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action, xCollection.xSnackbar.Hosts scope, xCollection.xSnackbar.Positions placement)
		{
			Positioning = placement;
			return Show(owner, content, seconds, type, action, scope);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action, xCollection.xSnackbar.Hosts scope, xCollection.xSnackbar.Positions placement, int margin)
		{
			Margin = margin;
			return Show(owner, content, seconds, type, action, scope, placement);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action, xCollection.xSnackbar.Hosts scope, xCollection.xSnackbar.Positions placement, int margin, bool showClosingIcon)
		{
			ShowClosingIcon = showClosingIcon;
			return Show(owner, content, seconds, type, action, scope, placement, margin);
		}

		public xCollection.xSnackbar.SnackbarResult Show(Form owner, string content, int seconds, xCollection.xSnackbar.MessageTypes type, string action, xCollection.xSnackbar.Hosts scope, xCollection.xSnackbar.Positions placement, int margin, bool showClosingIcon, bool clickToClose)
		{
			ClickToClose = clickToClose;
			return Show(owner, content, seconds, type, action, scope, placement, margin, showClosingIcon);
		}

		public new void Close()
		{
			try
			{
				base.Owner.LocationChanged -= OnOwnerLocationChanged;
				ExitPoint = GetExitPoint();
				PerformTransition(true);
				UpdateVisibleOthers();
			}
			catch (Exception)
			{
			}
		}

		public void CloseSnackbar()
		{
			OnClickClosingIcon(this, EventArgs.Empty);
		}

		public void EndTimeout(object state)
		{
			try
			{
				if (!_allowIdleWaitTime)
				{
					if (IsMouseWithinSnackbar() || _hasBeenInvoked)
						return;
					Invoke((Action)delegate
					{
						Close();
					});
					if (_action != null)
					{
						Invoke((Action)delegate
						{
							_action.DynamicInvoke(_result);
						});
						_hasBeenInvoked = true;
					}
				}
				else if ((_timeout < _duration && !IsMouseWithinSnackbar()) || (_isDirectClosure && !_hasBeenInvoked))
				{
					Invoke((Action)delegate
					{
						Close();
					});
					if (_action != null)
					{
						Invoke((Action)delegate
						{
							_action.DynamicInvoke(_result);
						});
						_hasBeenInvoked = true;
					}
				}
				else
				{
					_hasFinished = true;
					_timeout = 0u;
				}
			}
			catch (Exception)
			{
			}
		}

		[DllImport("User32.dll")]
		private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

		private void PerformTransition(bool isClosing)
		{
			try
			{
				if (!isClosing)
				{
					base.Opacity = 0.0;
					Transition.run(this, "Opacity", 1.0, new TransitionType_EaseInEaseOut(500));
					Transition.run(this, "Left", EntryPoint.X, new TransitionType_EaseInEaseOut(220));
					Transition.run(this, "Top", EntryPoint.Y, new TransitionType_EaseInEaseOut(220));
					return;
				}
				Transition.run(this, "Opacity", 0.0, new TransitionType_EaseInEaseOut(200));
				Transition transition = new Transition(new TransitionType_EaseInEaseOut(220));
				transition.add(this, "Left", ExitPoint.X);
				transition.add(this, "Top", ExitPoint.Y);
				transition.TransitionCompletedEvent += delegate
				{
					base.Close();
				};
				transition.run();
			}
			catch (Exception)
			{
			}
		}

		private Size RenderSize()
		{
			try
			{
				if (pbIcon.Image != null)
					lblMessage.Left = pbIcon.Right + pbIcon.Left / 2;
				else
					lblMessage.Left = 12;
				btnAction.Width = (int)CreateGraphics().MeasureString(btnAction.ButtonText, btnAction.Font).Width + 35;
				btnAction.Height = (int)CreateGraphics().MeasureString(btnAction.ButtonText, btnAction.Font).Height + 15;
				btnAction.Location = new Point(_showClosingIcon ? (pbClose.Left - btnAction.Width - 1) : (base.Width - btnAction.Width - btnAction.Top), btnAction.Top);
				if (string.IsNullOrWhiteSpace(_actionContent))
				{
					if (_showClosingIcon)
						base.Width = lblMessage.Right + MessageRightMargin + (_showClosingIcon ? pbClose.Width : 0);
					else
						base.Width = lblMessage.Right + pbIcon.Left;
				}
				else
					base.Width = lblMessage.Right + MessageRightMargin + btnAction.Width + 5 + (_showClosingIcon ? pbClose.Width : 0);
				pbIcon.Left = SnackbarIconLeftMargin;
				base.Height = lblMessage.Bottom + 12;
				pbIcon.Top = lblMessage.Height / 2 + 2;
				if (lblMessage.Bottom < btnAction.Bottom && !string.IsNullOrWhiteSpace(ActionText))
				{
					base.Height = btnAction.Bottom + 7;
					if (!lblMessage.Font.FontFamily.Name.Contains("Segoe UI"))
					{
						pbIcon.Top = btnAction.Height / 2 - 2;
						lblMessage.Top = btnAction.Height / 2;
					}
					lblMessage.Top = (base.Height - lblMessage.Height) / 2;
					pbClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
				}
				else
				{
					lblMessage.Top = (base.Height - lblMessage.Height) / 2;
					pbClose.Anchor = AnchorStyles.Right;
				}
				if (lblMessage.Text.Contains(Environment.NewLine) || lblMessage.Text.Contains("\n"))
					pbClose.Top = -1;
				if (DisplayControl != null)
				{
					pbIcon.Hide();
					btnAction.Hide();
					lblMessage.Hide();
					base.Size = DisplayControl.Size;
					DisplayControl.Height--;
					BackColor = DisplayControl.BackColor;
					base.Controls.Add(DisplayControl);
					DisplayControl.Left = 0;
					DisplayControl.Top = 0;
				}
			}
			catch (Exception)
			{
			}
			return base.Size;
		}

		private Size GetFontSize(Control control, string content)
		{
			return CreateGraphics().MeasureString(content, control.Font).ToSize();
		}

		private Bitmap CreateBitmap(Color color, Size size)
		{
			Bitmap bitmap = new Bitmap(size.Width, size.Height);
			bitmap.SetPixel(0, 0, color);
			return new Bitmap(bitmap, size);
		}

		private Bitmap ChangeBitmapColor(Bitmap scrBitmap, Color color)
		{
			Bitmap bitmap = new Bitmap(scrBitmap.Width, scrBitmap.Height);
			for (int i = 0; i < scrBitmap.Width; i++)
			{
				for (int j = 0; j < scrBitmap.Height; j++)
				{
					Color pixel = scrBitmap.GetPixel(i, j);
					if (pixel.A <= 150)
						bitmap.SetPixel(i, j, pixel);
					else
						bitmap.SetPixel(i, j, color);
				}
			}
			return bitmap;
		}

		private int VisibleCount()
		{
			int num = 0;
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(SnackbarView) && openForm.Visible)
					num++;
			}
			return num;
		}

		private Point GetEntryPoint()
		{
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = SystemInformation.CaptionHeight;
				int num4 = SystemInformation.FrameBorderSize.Width;
				Control control = base.Parent;
				Point point = default(Point);
				Rectangle workingArea = Screen.FromControl(this).WorkingArea;
				if (_scope == xCollection.xSnackbar.Hosts.FormOwner)
				{
					if (base.Owner.FormBorderStyle == FormBorderStyle.Sizable)
					{
						num3 = SystemInformation.CaptionHeight;
						num4 = SystemInformation.FrameBorderSize.Width;
					}
					else if (base.Owner.FormBorderStyle == FormBorderStyle.SizableToolWindow)
					{
						num3 = SystemInformation.ToolWindowCaptionHeight;
						num4 = SystemInformation.FrameBorderSize.Width;
					}
					else if (base.Owner.FormBorderStyle == FormBorderStyle.Fixed3D)
					{
						num3 = SystemInformation.Border3DSize.Height;
						num4 = SystemInformation.Border3DSize.Width;
					}
					else if (base.Owner.FormBorderStyle == FormBorderStyle.FixedDialog || base.Owner.FormBorderStyle == FormBorderStyle.FixedSingle)
					{
						num3 = SystemInformation.FixedFrameBorderSize.Height;
						num4 = SystemInformation.FixedFrameBorderSize.Width;
					}
					else if (base.Owner.FormBorderStyle == FormBorderStyle.FixedToolWindow)
					{
						num3 = SystemInformation.ToolWindowCaptionHeight;
						num4 = SystemInformation.FixedFrameBorderSize.Width;
					}
					else if (base.Owner.FormBorderStyle == FormBorderStyle.None)
					{
						num4 = 0;
						num3 = 0;
					}
					control = base.Owner;
					point = new Point(base.Owner.Left + base.Owner.Width / 2 - base.Width / 2, base.Owner.Top + base.Owner.Height / 2 - base.Height / 2);
				}
				else if (_scope == xCollection.xSnackbar.Hosts.Screen)
				{
					point = new Point(workingArea.Left + workingArea.Width / 2 - base.Width / 2, workingArea.Top + workingArea.Height / 2 - base.Height / 2);
				}
				else if (_scope == xCollection.xSnackbar.Hosts.Control)
				{
					num3 = 1;
					num4 = 1;
					control = _hostingControl;
					point = new Point(_hostingControl.Left + _hostingControl.Width / 2 - base.Width / 2, _hostingControl.Top + _hostingControl.Height / 2 - base.Height / 2);
				}
				if (_placement == xCollection.xSnackbar.Positions.TopLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left + num4 + _margin;
						num2 = control.Top + num3 + _margin;
					}
					else
					{
						num = Screen.PrimaryScreen.WorkingArea.Left + _margin;
						num2 = Screen.PrimaryScreen.WorkingArea.Top + _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopCenter)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = point.X;
						num2 = control.Top + num3 + _margin;
						base.StartPosition = FormStartPosition.CenterParent;
					}
					else
					{
						num = point.X;
						num2 = workingArea.Top + _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right - base.Width - _margin - num4 + 10;
						num2 = control.Top + num3 + _margin;
					}
					else
					{
						num = workingArea.Right - base.Width - _margin;
						num2 = workingArea.Top + _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left + num4 + _margin;
						num2 = point.Y + num3;
					}
					else
					{
						num = workingArea.Left + _margin;
						num2 = (workingArea.Height - base.Height) / 2;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleCenter)
				{
					num = point.X;
					num2 = point.Y;
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right - base.Width - _margin + ((((Form)control).FormBorderStyle == FormBorderStyle.None) ? 10 : 0);
						num2 = point.Y;
					}
					else
					{
						num = workingArea.Right - base.Width - _margin;
						num2 = point.Y;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left + num4 + _margin;
						num2 = control.Bottom - base.Height - _margin;
					}
					else
					{
						num = workingArea.Left + _margin;
						num2 = workingArea.Bottom - base.Height - _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomCenter)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = point.X;
						num2 = control.Bottom - base.Height - _margin;
					}
					else
					{
						num = point.X;
						num2 = workingArea.Bottom - base.Height - _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right - base.Width - _margin + ((((Form)control).FormBorderStyle == FormBorderStyle.None) ? 10 : 0);
						num2 = control.Bottom - base.Height - _margin;
					}
					else
					{
						num = workingArea.Right - base.Width - _margin;
						num2 = workingArea.Bottom - base.Height - _margin;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.Custom)
				{
					num = EntryPoint.X;
					num2 = EntryPoint.Y;
				}
				else
				{
					num = point.X;
					num2 = point.Y;
				}
				return new Point(num, num2);
			}
			catch (Exception)
			{
				return EntryPoint;
			}
		}

		private Point GetExitPoint()
		{
			try
			{
				int num = 0;
				int num2 = 0;
				Control control = base.Parent;
				Point point = default(Point);
				Rectangle workingArea = Screen.FromControl(this).WorkingArea;
				if (_scope == xCollection.xSnackbar.Hosts.FormOwner)
				{
					control = base.Owner;
					point = new Point(base.Owner.Left + base.Owner.Width / 2 - base.Width / 2, base.Owner.Top + base.Owner.Height / 2 - base.Height / 2);
				}
				else if (_scope == xCollection.xSnackbar.Hosts.Screen)
				{
					point = new Point(workingArea.Left + workingArea.Width / 2 - base.Width / 2, workingArea.Top + workingArea.Height / 2 - base.Height / 2);
				}
				else if (_scope == xCollection.xSnackbar.Hosts.Control)
				{
					control = _hostingControl;
					point = new Point(_hostingControl.Left + _hostingControl.Width / 2 - base.Width / 2, _hostingControl.Top + _hostingControl.Height / 2 - base.Height / 2);
				}
				if (_placement == xCollection.xSnackbar.Positions.TopLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left - base.Width;
						num2 = base.Top;
					}
					else
					{
						num = Screen.PrimaryScreen.WorkingArea.Left - base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopCenter)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = base.Left;
						num2 = control.Top - base.Height;
						base.StartPosition = FormStartPosition.CenterParent;
					}
					else
					{
						num = base.Left;
						num2 = workingArea.Top - base.Height;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right + base.Width;
						num2 = base.Top;
					}
					else
					{
						num = workingArea.Right + base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left - base.Width;
						num2 = base.Top;
					}
					else
					{
						num = workingArea.Left - base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleCenter)
				{
					num = base.Left;
					num2 = point.Y - base.Height;
				}
				else if (_placement == xCollection.xSnackbar.Positions.MiddleRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right + base.Width;
						num2 = base.Top;
					}
					else
					{
						num = workingArea.Right + base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomLeft)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Left - base.Width;
						num2 = base.Top;
					}
					else
					{
						num = workingArea.Left - base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomCenter)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = base.Left;
						num2 = control.Bottom + base.Height;
					}
					else
					{
						num = base.Left;
						num2 = workingArea.Bottom + base.Height;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomRight)
				{
					if (_scope == xCollection.xSnackbar.Hosts.FormOwner || _scope == xCollection.xSnackbar.Hosts.Control)
					{
						num = control.Right + base.Width;
						num2 = base.Top;
					}
					else
					{
						num = workingArea.Right + base.Width;
						num2 = base.Top;
					}
				}
				else if (_placement == xCollection.xSnackbar.Positions.Custom)
				{
					num = ExitPoint.X;
					num2 = ExitPoint.Y;
				}
				else
				{
					num = point.X;
					num2 = point.Y;
				}
				return new Point(num, num2);
			}
			catch (Exception)
			{
				return EntryPoint;
			}
		}

		private Point LastVisibleLocation()
		{
			return Application.OpenForms[VisibleCount()].Location;
		}

		private int LastVisibleHeight()
		{
			return Application.OpenForms[VisibleCount()].Height;
		}

		private Point AlignToVisibleOthers(Point currentLocation, xCollection.xSnackbar.Positions placementArea)
		{
			try
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (openForm.GetType() == typeof(SnackbarView) && openForm.Visible)
					{
						switch (placementArea)
						{
						case xCollection.xSnackbar.Positions.BottomCenter:
							_countBottomCenter++;
							break;
						case xCollection.xSnackbar.Positions.BottomLeft:
							_countBottomLeft++;
							break;
						case xCollection.xSnackbar.Positions.BottomRight:
							_countBottomRight++;
							break;
						case xCollection.xSnackbar.Positions.MiddleCenter:
							_countMiddleCenter++;
							break;
						case xCollection.xSnackbar.Positions.MiddleLeft:
							_countMiddleLeft++;
							break;
						case xCollection.xSnackbar.Positions.MiddleRight:
							_countMiddleRight++;
							break;
						case xCollection.xSnackbar.Positions.TopCenter:
							_countTopCenter++;
							break;
						case xCollection.xSnackbar.Positions.TopLeft:
							_countTopLeft++;
							break;
						case xCollection.xSnackbar.Positions.TopRight:
							_countTopRight++;
							break;
						}
						if (placementArea == xCollection.xSnackbar.Positions.BottomCenter && _countBottomCenter > 1)
							_visibleSizeBottomCenter += openForm.Height + ViewsMargin;
						else if (placementArea == xCollection.xSnackbar.Positions.BottomLeft && _countBottomLeft > 1)
						{
							_visibleSizeBottomLeft += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.BottomRight && _countBottomRight > 1)
						{
							_visibleSizeBottomRight += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.MiddleCenter && _countMiddleCenter > 1)
						{
							_visibleSizeMiddleCenter += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.MiddleLeft && _countMiddleLeft > 1)
						{
							_visibleSizeMiddleLeft += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.MiddleRight && _countMiddleRight > 1)
						{
							_visibleSizeMiddleRight += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.TopCenter && _countTopCenter > 1)
						{
							_visibleSizeTopCenter += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.TopLeft && _countTopLeft > 1)
						{
							_visibleSizeTopLeft += openForm.Height + ViewsMargin;
						}
						else if (placementArea == xCollection.xSnackbar.Positions.TopRight && _countTopRight > 1)
						{
							_visibleSizeTopRight += openForm.Height + ViewsMargin;
						}
					}
				}
				switch (placementArea)
				{
				case xCollection.xSnackbar.Positions.BottomCenter:
					currentLocation.Y -= _visibleSizeBottomCenter;
					break;
				case xCollection.xSnackbar.Positions.BottomLeft:
					currentLocation.Y -= _visibleSizeBottomLeft;
					break;
				case xCollection.xSnackbar.Positions.BottomRight:
					currentLocation.Y -= _visibleSizeBottomRight;
					break;
				case xCollection.xSnackbar.Positions.MiddleCenter:
					currentLocation.Y += _visibleSizeMiddleCenter;
					break;
				case xCollection.xSnackbar.Positions.MiddleLeft:
					currentLocation.Y += _visibleSizeMiddleLeft;
					break;
				case xCollection.xSnackbar.Positions.MiddleRight:
					currentLocation.Y += _visibleSizeMiddleRight;
					break;
				case xCollection.xSnackbar.Positions.TopCenter:
					currentLocation.Y += _visibleSizeTopCenter;
					break;
				case xCollection.xSnackbar.Positions.TopLeft:
					currentLocation.Y += _visibleSizeTopLeft;
					break;
				case xCollection.xSnackbar.Positions.TopRight:
					currentLocation.Y += _visibleSizeTopRight;
					break;
				}
				return currentLocation;
			}
			catch (Exception)
			{
				return currentLocation;
			}
		}

		private void UpdateVisibleOthers()
		{
			try
			{
				if (Positioning == xCollection.xSnackbar.Positions.BottomCenter)
				{
					_countBottomCenter--;
					_visibleSizeBottomCenter -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.BottomLeft)
				{
					_countBottomLeft--;
					_visibleSizeBottomLeft -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.BottomRight)
				{
					_countBottomRight--;
					_visibleSizeBottomRight -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.MiddleCenter)
				{
					_countMiddleCenter--;
					_visibleSizeMiddleCenter -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.MiddleLeft)
				{
					_countMiddleLeft--;
					_visibleSizeMiddleLeft -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.MiddleRight)
				{
					_countMiddleRight--;
					_visibleSizeMiddleRight -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.TopCenter)
				{
					_countTopCenter--;
					_visibleSizeTopCenter -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.TopLeft)
				{
					_countTopLeft--;
					_visibleSizeTopLeft -= base.Height - ViewsMargin;
				}
				else if (Positioning == xCollection.xSnackbar.Positions.TopRight)
				{
					_countTopRight--;
					_visibleSizeTopRight -= base.Height - ViewsMargin;
				}
			}
			catch (Exception)
			{
			}
		}

		private void StartNewTimer(int duration)
		{
			_timer = new System.Threading.Timer(EndTimeout, null, duration, 0);
		}

		private void SetDefaultLocation(Point location)
		{
			try
			{
				if (_placement == xCollection.xSnackbar.Positions.TopLeft || _placement == xCollection.xSnackbar.Positions.MiddleLeft || _placement == xCollection.xSnackbar.Positions.BottomLeft)
				{
					base.Left = location.X - base.Width;
					base.Top = location.Y;
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopRight || _placement == xCollection.xSnackbar.Positions.MiddleRight || _placement == xCollection.xSnackbar.Positions.BottomRight)
				{
					base.Left = location.X + base.Width;
					base.Top = location.Y;
				}
				else if (_placement == xCollection.xSnackbar.Positions.TopCenter || _placement == xCollection.xSnackbar.Positions.MiddleCenter)
				{
					base.Left = location.X;
					base.Top = location.Y - base.Height;
				}
				else if (_placement == xCollection.xSnackbar.Positions.BottomCenter)
				{
					base.Left = location.X;
					base.Top = location.Y + base.Height;
				}
			}
			catch (Exception)
			{
			}
		}

		private bool IsMouseInControl(Control control)
		{
			return IsPointInRect(PointToClient(Control.MousePosition), control.ClientRectangle);
		}

		private bool IsMouseWithinSnackbar()
		{
			bool result = false;
			try
			{
				Invoke((Action)delegate
				{
					if (IsMouseInControl(this) || IsMouseInControl(lblMessage) || IsMouseInControl(btnAction) || IsMouseInControl(pbIcon) || IsMouseInControl(pbClose))
						result = true;
					else
						result = false;
				});
			}
			catch (Exception)
			{
			}
			return result;
		}

		private static bool IsPointInRect(Point pt, Rectangle rect)
		{
			if ((pt.X > rect.Left) & (pt.X < rect.Right) & (pt.Y > rect.Top) & (pt.Y < rect.Bottom))
				return true;
			return false;
		}

		private void OnLoad(object sender, EventArgs e)
		{
			if (ShowShadows)
			{
				if (ShowBorders)
					dckFormDock.AllowFormDropShadow = false;
				else
					dckFormDock.AllowFormDropShadow = true;
			}
			dckFormDock.AllowFormDragging = AllowDragging;
			if (AllowDragging)
				dckFormDock.SubscribeControlsToDragEvents(new Control[2] { this, lblMessage });
		}

		private void OnFormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
				Hide();
				base.Parent = null;
				tmrTimeout.Stop();
				base.Owner.Activate();
			}
			catch (Exception)
			{
			}
		}

		private void OnOwnerFormClosing(object sender, FormClosingEventArgs e)
		{
			try
			{
			}
			catch (Exception)
			{
			}
		}

		private void OnClick(object sender, EventArgs e)
		{
			if (ClickToClose)
				OnClickClosingIcon(sender, e);
		}

		private void OnDoubleClick(object sender, EventArgs e)
		{
			if (DoubleClickToClose)
				OnClickClosingIcon(sender, e);
		}

		private void OnOwnerLocationChanged(object sender, EventArgs e)
		{
			try
			{
				base.Location = GetEntryPoint();
			}
			catch (Exception)
			{
			}
		}

		private void OnTick(object sender, EventArgs e)
		{
			_timeout = Win32.GetIdleTime();
			if (_allowIdleWaitTime)
			{
				if (_timeout < _duration && _hasFinished)
				{
					StartNewTimer(_durationAfterIdle);
					tmrTimeout.Stop();
				}
				else if (_hasFinished)
				{
					EndTimeout(null);
				}
			}
		}

		private void OnClickSnackbarAction(object sender, EventArgs e)
		{
			_timer.Change(0, 0);
			_result = xCollection.xSnackbar.SnackbarResult.ActionClicked;
			_isDirectClosure = true;
			Close();
		}

		private void OnClickClosingIcon(object sender, EventArgs e)
		{
			_timer.Change(0, 0);
			_result = xCollection.xSnackbar.SnackbarResult.UserClosed;
			_isDirectClosure = true;
			Close();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Utilities.xSnackbar.Views.SnackbarView));
			Utilities.xSnackbar.xButton.BorderEdges borderEdges = new Utilities.xSnackbar.xButton.BorderEdges();
			this.lblMessage = new System.Windows.Forms.Label();
			this.pbIcon = new System.Windows.Forms.PictureBox();
			this.tmrTimeout = new System.Windows.Forms.Timer(this.components);
			this.lblBorderRight = new System.Windows.Forms.Label();
			this.lblBorderLeft = new System.Windows.Forms.Label();
			this.lblBorderBottom = new System.Windows.Forms.Label();
			this.lblBorderTop = new System.Windows.Forms.Label();
			this.btnAction = new Utilities.xSnackbar.xButton();
			this.pbClose = new Utilities.xSnackbar.xImageButtonExtended();
			this.dckFormDock = new Utilities.xSnackbar.xFormDock();
			((System.ComponentModel.ISupportInitialize)this.pbIcon).BeginInit();
			base.SuspendLayout();
			this.lblMessage.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			this.lblMessage.AutoSize = true;
			this.lblMessage.BackColor = System.Drawing.Color.Transparent;
			this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 11.25f);
			this.lblMessage.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.lblMessage.Location = new System.Drawing.Point(12, 11);
			this.lblMessage.Name = "lblMessage";
			this.lblMessage.Size = new System.Drawing.Size(69, 20);
			this.lblMessage.TabIndex = 2;
			this.lblMessage.Text = "{content}";
			this.lblMessage.Click += new System.EventHandler(OnClick);
			this.lblMessage.DoubleClick += new System.EventHandler(OnDoubleClick);
			this.pbIcon.Location = new System.Drawing.Point(12, 12);
			this.pbIcon.Name = "pbIcon";
			this.pbIcon.Size = new System.Drawing.Size(20, 20);
			this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pbIcon.TabIndex = 5;
			this.pbIcon.TabStop = false;
			this.pbIcon.Visible = false;
			this.pbIcon.Click += new System.EventHandler(OnClick);
			this.pbIcon.DoubleClick += new System.EventHandler(OnDoubleClick);
			this.tmrTimeout.Interval = 1000;
			this.lblBorderRight.BackColor = System.Drawing.Color.Gainsboro;
			this.lblBorderRight.Dock = System.Windows.Forms.DockStyle.Right;
			this.lblBorderRight.Location = new System.Drawing.Point(422, 0);
			this.lblBorderRight.Name = "lblBorderRight";
			this.lblBorderRight.Size = new System.Drawing.Size(1, 43);
			this.lblBorderRight.TabIndex = 10;
			this.lblBorderRight.Visible = false;
			this.lblBorderLeft.BackColor = System.Drawing.Color.Gainsboro;
			this.lblBorderLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblBorderLeft.Location = new System.Drawing.Point(0, 0);
			this.lblBorderLeft.Name = "lblBorderLeft";
			this.lblBorderLeft.Size = new System.Drawing.Size(1, 43);
			this.lblBorderLeft.TabIndex = 9;
			this.lblBorderLeft.Visible = false;
			this.lblBorderBottom.BackColor = System.Drawing.Color.Gainsboro;
			this.lblBorderBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.lblBorderBottom.Location = new System.Drawing.Point(0, 43);
			this.lblBorderBottom.Name = "lblBorderBottom";
			this.lblBorderBottom.Size = new System.Drawing.Size(423, 1);
			this.lblBorderBottom.TabIndex = 8;
			this.lblBorderBottom.Visible = false;
			this.lblBorderTop.BackColor = System.Drawing.Color.Gainsboro;
			this.lblBorderTop.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblBorderTop.Location = new System.Drawing.Point(1, 0);
			this.lblBorderTop.Name = "lblBorderTop";
			this.lblBorderTop.Size = new System.Drawing.Size(421, 1);
			this.lblBorderTop.TabIndex = 11;
			this.lblBorderTop.Visible = false;
			this.btnAction.AllowToggling = false;
			this.btnAction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			this.btnAction.AnimationSpeed = 200;
			this.btnAction.AutoGenerateColors = false;
			this.btnAction.BackColor = System.Drawing.Color.Transparent;
			this.btnAction.BackColor1 = System.Drawing.Color.FromArgb(18, 118, 58);
			this.btnAction.BackgroundImage = (System.Drawing.Image)resources.GetObject("btnAction.BackgroundImage");
			this.btnAction.BorderStyle = Utilities.xSnackbar.xButton.BorderStyles.Solid;
			this.btnAction.ButtonText = "{ACTION}";
			this.btnAction.ButtonTextMarginLeft = 0;
			this.btnAction.ColorContrastOnClick = 45;
			this.btnAction.ColorContrastOnHover = 45;
			this.btnAction.Cursor = System.Windows.Forms.Cursors.Hand;
			borderEdges.BottomLeft = true;
			borderEdges.BottomRight = true;
			borderEdges.TopLeft = true;
			borderEdges.TopRight = true;
			this.btnAction.CustomizableEdges = borderEdges;
			this.btnAction.DialogResult = System.Windows.Forms.DialogResult.None;
			this.btnAction.DisabledBorderColor = System.Drawing.Color.FromArgb(161, 161, 161);
			this.btnAction.DisabledFillColor = System.Drawing.Color.Gray;
			this.btnAction.DisabledForecolor = System.Drawing.Color.White;
			this.btnAction.FocusState = Utilities.xSnackbar.xButton.ButtonStates.Idle;
			this.btnAction.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Bold);
			this.btnAction.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.btnAction.IconLeftCursor = System.Windows.Forms.Cursors.Default;
			this.btnAction.IconMarginLeft = 11;
			this.btnAction.IconPadding = 10;
			this.btnAction.IconRightCursor = System.Windows.Forms.Cursors.Default;
			this.btnAction.IdleBorderColor = System.Drawing.Color.FromArgb(18, 118, 58);
			this.btnAction.IdleBorderRadius = 1;
			this.btnAction.IdleBorderThickness = 1;
			this.btnAction.IdleFillColor = System.Drawing.Color.FromArgb(18, 118, 58);
			this.btnAction.IdleIconLeftImage = null;
			this.btnAction.IdleIconRightImage = null;
			this.btnAction.IndicateFocus = false;
			this.btnAction.Location = new System.Drawing.Point(302, 7);
			this.btnAction.Name = "btnAction";
			this.btnAction.onHoverState.BorderColor = System.Drawing.Color.FromArgb(124, 179, 146);
			this.btnAction.onHoverState.BorderRadius = 1;
			this.btnAction.onHoverState.BorderStyle = Utilities.xSnackbar.xButton.BorderStyles.Solid;
			this.btnAction.onHoverState.BorderThickness = 1;
			this.btnAction.onHoverState.FillColor = System.Drawing.Color.FromArgb(124, 179, 146);
			this.btnAction.onHoverState.ForeColor = System.Drawing.Color.Empty;
			this.btnAction.onHoverState.IconLeftImage = null;
			this.btnAction.onHoverState.IconRightImage = null;
			this.btnAction.OnIdleState.BorderColor = System.Drawing.Color.FromArgb(18, 118, 58);
			this.btnAction.OnIdleState.BorderRadius = 1;
			this.btnAction.OnIdleState.BorderStyle = Utilities.xSnackbar.xButton.BorderStyles.Solid;
			this.btnAction.OnIdleState.BorderThickness = 1;
			this.btnAction.OnIdleState.FillColor = System.Drawing.Color.FromArgb(18, 118, 58);
			this.btnAction.OnIdleState.ForeColor = System.Drawing.Color.WhiteSmoke;
			this.btnAction.OnIdleState.IconLeftImage = null;
			this.btnAction.OnIdleState.IconRightImage = null;
			this.btnAction.OnPressedState.BorderColor = System.Drawing.Color.FromArgb(9, 64, 31);
			this.btnAction.OnPressedState.BorderRadius = 1;
			this.btnAction.OnPressedState.BorderStyle = Utilities.xSnackbar.xButton.BorderStyles.Solid;
			this.btnAction.OnPressedState.BorderThickness = 1;
			this.btnAction.OnPressedState.FillColor = System.Drawing.Color.FromArgb(9, 64, 31);
			this.btnAction.OnPressedState.ForeColor = System.Drawing.Color.White;
			this.btnAction.OnPressedState.IconLeftImage = null;
			this.btnAction.OnPressedState.IconRightImage = null;
			this.btnAction.Size = new System.Drawing.Size(70, 31);
			this.btnAction.TabIndex = 4;
			this.btnAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.btnAction.TextMarginLeft = 0;
			this.btnAction.UseDefaultRadiusAndThickness = true;
			this.btnAction.Visible = false;
			this.btnAction.Click += new System.EventHandler(OnClickSnackbarAction);
			this.pbClose.ActiveImage = null;
			this.pbClose.AllowAnimations = true;
			this.pbClose.AllowZooming = true;
			this.pbClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.pbClose.BackColor = System.Drawing.Color.Transparent;
			this.pbClose.ErrorImage = (System.Drawing.Image)resources.GetObject("pbClose.ErrorImage");
			this.pbClose.FadeWhenInactive = false;
			this.pbClose.Flip = Utilities.xSnackbar.xImageButtonExtended.FlipOrientation.Normal;
			this.pbClose.Image = (System.Drawing.Image)resources.GetObject("pbClose.Image");
			this.pbClose.ImageActive = null;
			this.pbClose.ImageLocation = null;
			this.pbClose.ImageMargin = 20;
			this.pbClose.ImageSize = new System.Drawing.Size(25, 24);
			this.pbClose.ImageZoomSize = new System.Drawing.Size(45, 44);
			this.pbClose.InitialImage = (System.Drawing.Image)resources.GetObject("pbClose.InitialImage");
			this.pbClose.Location = new System.Drawing.Point(378, 0);
			this.pbClose.Name = "pbClose";
			this.pbClose.Rotation = 0;
			this.pbClose.ShowActiveImage = true;
			this.pbClose.ShowCursorChanges = true;
			this.pbClose.ShowImageBorders = true;
			this.pbClose.ShowSizeMarkers = false;
			this.pbClose.Size = new System.Drawing.Size(45, 44);
			this.pbClose.TabIndex = 3;
			this.pbClose.ToolTipText = "Close";
			this.pbClose.WaitOnLoad = false;
			this.pbClose.Zoom = 20;
			this.pbClose.ZoomSpeed = 10;
			this.pbClose.Click += new System.EventHandler(OnClickClosingIcon);
			this.pbClose.DoubleClick += new System.EventHandler(OnDoubleClick);
			this.dckFormDock.AllowFormDragging = false;
			this.dckFormDock.AllowFormDropShadow = false;
			this.dckFormDock.AllowFormResizing = false;
			this.dckFormDock.AllowHidingBottomRegion = true;
			this.dckFormDock.AllowOpacityChangesWhileDragging = false;
			this.dckFormDock.BorderOptions.BottomBorder.BorderColor = System.Drawing.Color.Silver;
			this.dckFormDock.BorderOptions.BottomBorder.BorderThickness = 1;
			this.dckFormDock.BorderOptions.BottomBorder.ShowBorder = true;
			this.dckFormDock.BorderOptions.LeftBorder.BorderColor = System.Drawing.Color.Silver;
			this.dckFormDock.BorderOptions.LeftBorder.BorderThickness = 1;
			this.dckFormDock.BorderOptions.LeftBorder.ShowBorder = true;
			this.dckFormDock.BorderOptions.RightBorder.BorderColor = System.Drawing.Color.Silver;
			this.dckFormDock.BorderOptions.RightBorder.BorderThickness = 1;
			this.dckFormDock.BorderOptions.RightBorder.ShowBorder = true;
			this.dckFormDock.BorderOptions.TopBorder.BorderColor = System.Drawing.Color.Silver;
			this.dckFormDock.BorderOptions.TopBorder.BorderThickness = 1;
			this.dckFormDock.BorderOptions.TopBorder.ShowBorder = true;
			this.dckFormDock.ContainerControl = this;
			this.dckFormDock.DockingIndicatorsColor = System.Drawing.Color.FromArgb(202, 215, 233);
			this.dckFormDock.DockingIndicatorsOpacity = 0.5;
			this.dckFormDock.DockingOptions.DockAll = false;
			this.dckFormDock.DockingOptions.DockBottomLeft = false;
			this.dckFormDock.DockingOptions.DockBottomRight = false;
			this.dckFormDock.DockingOptions.DockFullScreen = false;
			this.dckFormDock.DockingOptions.DockLeft = false;
			this.dckFormDock.DockingOptions.DockRight = false;
			this.dckFormDock.DockingOptions.DockTopLeft = false;
			this.dckFormDock.DockingOptions.DockTopRight = false;
			this.dckFormDock.FormDraggingOpacity = 0.9;
			this.dckFormDock.ParentForm = this;
			this.dckFormDock.ShowCursorChanges = true;
			this.dckFormDock.ShowDockingIndicators = false;
			this.dckFormDock.TitleBarOptions.AllowFormDragging = false;
			this.dckFormDock.TitleBarOptions.DoubleClickToExpandWindow = false;
			this.dckFormDock.TitleBarOptions.TitleBarControl = null;
			this.dckFormDock.TitleBarOptions.UseBackColorOnDockingIndicators = false;
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.FromArgb(27, 177, 87);
			base.ClientSize = new System.Drawing.Size(423, 44);
			base.Controls.Add(this.lblBorderTop);
			base.Controls.Add(this.lblBorderRight);
			base.Controls.Add(this.lblBorderLeft);
			base.Controls.Add(this.lblBorderBottom);
			base.Controls.Add(this.btnAction);
			base.Controls.Add(this.pbClose);
			base.Controls.Add(this.lblMessage);
			base.Controls.Add(this.pbIcon);
			this.DoubleBuffered = true;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.Name = "SnackbarView";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Snackbar";
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OnFormClosing);
			base.Load += new System.EventHandler(OnLoad);
			base.Click += new System.EventHandler(OnClick);
			base.DoubleClick += new System.EventHandler(OnDoubleClick);
			((System.ComponentModel.ISupportInitialize)this.pbIcon).EndInit();
			base.ResumeLayout(false);
			base.PerformLayout();
		}
	}
}
