using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Messaging;
using System.Windows.Forms;
using xCollection.Properties;
using Utilities.xSnackbar.Views;

namespace xCollection
{
	[Category("x UI For Windows Forms")]
	[Description("Deliver stunningly neat, time-bound, and brief messagesabout application processes during or after execution.")]
	[ToolboxBitmap(typeof(ContextMenu))]
	[DefaultProperty("Host")]
	[DebuggerStepThrough]
	public sealed class xSnackbar : Component
	{
        
		[Description("Provides a list of supported customization options for Snackbar messages.")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DebuggerStepThrough]
		public class CustomizationOptions
{
    [Description("Sets the action button's foreground color.")]
    private Color actionForeColor = Color.FromArgb(64, 64, 64);
    public Color ActionForeColor
    {
        get { return actionForeColor; }
        set { actionForeColor = value; }
    }

    [Description("Sets the action button's background color.")]
    private Color actionBackColor = Color.WhiteSmoke;
    public Color ActionBackColor
    {
        get { return actionBackColor; }
        set { actionBackColor = value; }
    }

    [Description("Sets the action button's border color.")]
    private Color actionBorderColor = Color.White;
    public Color ActionBorderColor
    {
        get { return actionBorderColor; }
        set { actionBorderColor = value; }
    }

    [Description("Sets the Snackbar's close icon color.")]
    private Color closeIconColor = Color.FromArgb(64, 64, 64);
    public Color CloseIconColor
    {
        get { return closeIconColor; }
        set { closeIconColor = value; }
    }

    [Description("Sets the Snackbar's background color.")]
    private Color backColor = Color.White;
    public Color BackColor
    {
        get { return backColor; }
        set { backColor = value; }
    }

    [Description("Sets the Snackbar's border color.")]
    private Color borderColor = Color.White;
    public Color BorderColor
    {
        get { return borderColor; }
        set { borderColor = value; }
    }

    [Description("Sets the message's foreground color.")]
    private Color foreColor = Color.Black;
    public Color ForeColor
    {
        get { return foreColor; }
        set { foreColor = value; }
    }

    [Description("Sets the action button's border radius.")]
    private int actionBorderRadius = 1;
    public int ActionBorderRadius
    {
        get { return actionBorderRadius; }
        set { actionBorderRadius = value; }
    }

    [Description("Sets the Snackbar icon's left margin.")]
    private int iconLeftMargin = 12;
    public int IconLeftMargin
    {
        get { return iconLeftMargin; }
        set { iconLeftMargin = value; }
    }

    [Description("Sets the Snackbar's icon.")]
    private Image icon = Resources.info; // Adjust the Resources namespace as needed
    public Image Icon
    {
        get { return icon; }
        set { icon = value; }
    }

    [Description("Sets the message's font.")]
    private Font font = new Font("Segoe UI", 11.25f, FontStyle.Regular);
    public Font Font
    {
        get { return font; }
        set { font = value; }
    }

    [Description("Sets the action button's font.")]
    private Font actionFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
    public Font ActionFont
    {
        get { return actionFont; }
        set { actionFont = value; }
    }

    public override string ToString()
    {
        return string.Format("BackColor: {0}, BorderColor: {1}, ActionForeColor: {2}, ", BackColor, BorderColor, ActionForeColor) +
               string.Format("ActionBackColor: {0}, ActionBorderColor: {1}, ", ActionBackColor, ActionBorderColor) +
               string.Format("MessageForeColor: {0}, CloseIconColor: {1}, ", ForeColor, CloseIconColor) +
               string.Format("ActionBorderRadius: {0}, SnackbarIconPadding: {1}, ", ActionBorderRadius, IconLeftMargin) +
               string.Format("MessageFont: {0}, ActionFont: {1}", Font, ActionFont);
    }
}

		public enum Hosts
		{
			Screen,
			Control,
			FormOwner
		}

		public enum Positions
		{
			TopLeft,
			TopCenter,
			TopRight,
			MiddleLeft,
			MiddleCenter,
			MiddleRight,
			BottomLeft,
			BottomCenter,
			BottomRight,
			Custom
		}

		public enum MessageTypes
		{
			Information,
			Success,
			Warning,
			Error
		}

		public enum SnackbarResult
		{
			AutoClosed,
			UserClosed,
			ActionClicked
		}


		private int _darkScaleFactor = 7;

		private int _lightScaleFactor = 2;

		private int _lightWarningScaleFactor = 4;

		private SnackbarView _currentSnackbar;

		private CustomizationOptions _options = new CustomizationOptions();

		private IContainer components = null;

		[Category("Behavior")]
		[Description("Sets a value indicating whether the Snackbar will be draggable once displayed.")]
		public bool AllowDragging { get; set; }

    [Description("Sets a value indicating whether the Snackbar will be closed once clicked by the user.")]
    [Category("Behavior")]
    private bool clickToClose = true;
    public bool ClickToClose
    {
        get { return clickToClose; }
        set { clickToClose = value; }
    }

    [Category("Behavior")]
    [Description("Sets a value indicating whether the Snackbar will be closed once double-clicked by the user.")]
    private bool doubleClickToClose = true;
    public bool DoubleClickToClose
    {
        get { return doubleClickToClose; }
        set { doubleClickToClose = value; }
    }

    [Category("Behavior")]
    [Description("Sets a value indicating whether multiple instances of the Snackbar will be displayed at a time.")]
    private bool allowMultipleViews = true;
    public bool AllowMultipleViews
    {
        get { return allowMultipleViews; }
        set { allowMultipleViews = value; }
    }

    [Description("Sets a value indicating whether the close icon will be displayed.")]
    [Category("Appearance")]
    private bool showCloseIcon = false;
    public bool ShowCloseIcon
    {
        get { return showCloseIcon; }
        set { showCloseIcon = value; }
    }

    [Category("Appearance")]
    [Description("Sets a value indicating whether the Snackbar icon will be displayed.")]
    private bool showIcon = true;
    public bool ShowIcon
    {
        get { return showIcon; }
        set { showIcon = value; }
    }

    [Description("Sets a value indicating whether the Snackbar will display its borders rather than shadows.")]
    [Category("Appearance")]
    private bool showBorders = false;
    public bool ShowBorders
    {
        get { return showBorders; }
        set { showBorders = value; }
    }

    [Category("Appearance")]
    [Description("Sets a value indicating whether the Snackbar will display its shadows rather than borders.")]
    private bool showShadows = true;
    public bool ShowShadows
    {
        get { return showShadows; }
        set { showShadows = value; }
    }

    [Description("Sets a value indicating whether the close icon will fade when inactive.")]
    [Category("Behavior")]
    private bool fadeCloseIcon = false;
    public bool FadeCloseIcon
    {
        get { return fadeCloseIcon; }
        set { fadeCloseIcon = value; }
    }

    [Category("Behavior")]
    [Description("Sets a value indicating whether the close icon will zoom-in when hovered onto.")]
    private bool zoomCloseIcon = true;
    public bool ZoomCloseIcon
    {
        get { return zoomCloseIcon; }
        set { zoomCloseIcon = value; }
    }

    [Category("Appearance")]
    [Description("Sets the overall Snackbar's overall distance between its host and its edges.")]
    private int margin = 10;
    public int Margin
    {
        get { return margin; }
        set { margin = value; }
    }

    [Description("Sets the distance between Snackbars whenever multiple views are enabled using the property 'AllowMultipleViews'.")]
    [Category("Appearance")]
    private int viewsMargin = 7;
    public int ViewsMargin
    {
        get { return viewsMargin; }
        set { viewsMargin = value; }
    }

    [Description("Sets the maximum number of Snackbars that can be visible at a given time.")]
    [Category("Behavior")]
    private int maximumViews = 7;
    public int MaximumViews
    {
        get { return maximumViews; }
        set { maximumViews = value; }
    }

    [Description("Sets the duration the Snackbar will remain visible after it receives and then loses mouse focus.")]
    [Category("Behavior")]
    private int durationAfterIdle = 3000;
    public int DurationAfterIdle
    {
        get { return durationAfterIdle; }
        set { durationAfterIdle = value; }
    }

    [Description("Sets the Snackbar's message right margin, or distance between the message and the closing icon. Please note that the minimum size for display will be established beyond the user-provided margin.")]
    [Category("Appearance")]
    private int messageRightMargin = 15;
    public int MessageRightMargin
    {
        get { return messageRightMargin; }
        set { messageRightMargin = value; }
    }



   [Description("Sets the list of customization options provided for the Snackbar's informational messages.")]
    [Category("Customization")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    private CustomizationOptions informationOptions = new CustomizationOptions();
    public CustomizationOptions InformationOptions
    {
        get { return informationOptions; }
        set { informationOptions = value; }
    }

    [Description("Sets the list of customization options provided for the Snackbar's success messages.")]
    [Category("Customization")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    private CustomizationOptions successOptions = new CustomizationOptions();
    public CustomizationOptions SuccessOptions
    {
        get { return successOptions; }
        set { successOptions = value; }
    }

    [Description("Sets the list of customization options provided for the Snackbar's warning messages.")]
    [Category("Customization")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    private CustomizationOptions warningOptions = new CustomizationOptions();
    public CustomizationOptions WarningOptions
    {
        get { return warningOptions; }
        set { warningOptions = value; }
    }

    [Description("Sets the list of customization options provided for the Snackbar's error messages.")]
    [Category("Customization")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    private CustomizationOptions errorOptions = new CustomizationOptions();
    public CustomizationOptions ErrorOptions
    {
        get { return errorOptions; }
        set { errorOptions = value; }
    }


		[Description("Sets the minimum size the Snackbar can be auto-resized to.")]
		[Category("Layout")]
		public Size MinimumSize { get; set; }

		[Description("Sets the maximum size the Snackbar can be auto-resized to.")]
		[Category("Layout")]
		public Size MaximumSize { get; set; }

		[Description("Sets the host that will be used to contain the Snackbar.")]
		[Category("Behavior")]
		private Hosts host = Hosts.FormOwner;

        public Hosts Host
        {
            get { return host; }
            set { host = value; }
        }


		[Browsable(false)]
        private bool hasAvailableAction = false;

        public bool HasAvailableAction
        {
            get { return hasAvailableAction; }
            private set { hasAvailableAction = value; }
        }


		public xSnackbar()
		{
			InitializeComponent();
			SetDefaults();
		}

		public xSnackbar(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
		}

		public SnackbarResult Show(Form owner, string message)
		{
			return Show(owner, message, MessageTypes.Information, 3000, "", Positions.TopCenter, Hosts.FormOwner);
		}

		public SnackbarResult Show(Form owner, string message, MessageTypes type)
		{
			return Show(owner, message, type, 3000, "", Positions.TopCenter, Hosts.FormOwner);
		}

		public SnackbarResult Show(Form owner, string message, MessageTypes type, int duration = 3000)
		{
			return Show(owner, message, type, duration, "", Positions.TopCenter, Hosts.FormOwner);
		}

		public SnackbarResult Show(Form owner, string message, MessageTypes type, int duration, string action)
		{
			return Show(owner, message, type, duration, action, Positions.TopCenter);
		}

		public SnackbarResult Show(Form owner, string message, MessageTypes type, int duration, string action, Positions position)
		{
			return Show(owner, message, type, duration, action, position, Hosts.FormOwner);
		}

		public SnackbarResult Show(Form owner, string message, MessageTypes type = MessageTypes.Information, int duration = 3000, string action = "", Positions position = Positions.TopCenter, Hosts host = Hosts.FormOwner)
		{
			SnackbarView snackbar = (_currentSnackbar = new SnackbarView());
			snackbar.Host = host;
			snackbar.Owner = owner;
			snackbar.Margin = Margin;
			snackbar.MessageType = type;
			snackbar.Duration = duration;
			snackbar.Positioning = position;
			snackbar.ClickToClose = ClickToClose;
			snackbar.ShowClosingIcon = ShowCloseIcon;
			snackbar.AllowDragging = AllowDragging;
			snackbar.ShowSnackbarIcon = ShowIcon;
			snackbar.ViewsMargin = ViewsMargin;
			snackbar.ShowShadows = ShowShadows;
			snackbar.ZoomClosingIcon = ZoomCloseIcon;
			snackbar.FadeClosingIcon = FadeCloseIcon;
			snackbar.MaximumViewCount = MaximumViews;
			snackbar.DurationAfterIdle = DurationAfterIdle;
			snackbar.MessageRightMargin = MessageRightMargin;
			snackbar.AllowMultipleViews = AllowMultipleViews;
			snackbar.DoubleClickToClose = DoubleClickToClose;
			snackbar.lblMessage.MaximumSize = MaximumSize;
			snackbar.lblMessage.MinimumSize = MinimumSize;
			snackbar.Message = message;
			snackbar.ActionText = action;
			Customize(ref snackbar, type);
			if (string.IsNullOrWhiteSpace(action))
				HasAvailableAction = false;
			else
				HasAvailableAction = true;
			return snackbar.Show();
		}

		public SnackbarResult Show(Form owner, Control displayControl = null, int duration = 3000, Positions position = Positions.TopCenter, Hosts host = Hosts.FormOwner)
		{
			SnackbarView snackbar = (_currentSnackbar = new SnackbarView());
			snackbar.Host = host;
			snackbar.Owner = owner;
			snackbar.Margin = Margin;
			snackbar.Duration = duration;
			snackbar.Positioning = position;
			snackbar.ClickToClose = ClickToClose;
			snackbar.ShowClosingIcon = ShowCloseIcon;
			snackbar.AllowDragging = AllowDragging;
			snackbar.ShowSnackbarIcon = ShowIcon;
			snackbar.ViewsMargin = ViewsMargin;
			snackbar.ShowShadows = ShowShadows;
			snackbar.ZoomClosingIcon = ZoomCloseIcon;
			snackbar.FadeClosingIcon = FadeCloseIcon;
			snackbar.MaximumViewCount = MaximumViews;
			snackbar.DurationAfterIdle = DurationAfterIdle;
			snackbar.MessageRightMargin = MessageRightMargin;
			snackbar.AllowMultipleViews = AllowMultipleViews;
			snackbar.DoubleClickToClose = DoubleClickToClose;
			snackbar.lblMessage.MaximumSize = MaximumSize;
			snackbar.lblMessage.MinimumSize = MinimumSize;
			if (displayControl != null)
				snackbar.DisplayControl = displayControl;
			Customize(ref snackbar, MessageTypes.Information);
			HasAvailableAction = false;
			return snackbar.Show();
		}

		public void Close()
		{
			try
			{
				_currentSnackbar.CloseSnackbar();
			}
			catch (Exception)
			{
			}
		}

		public void CloseAll()
		{
			try
			{
				foreach (Form openForm in Application.OpenForms)
				{
					if (openForm.GetType() == typeof(SnackbarView) && openForm.Visible)
						((SnackbarView)openForm).CloseSnackbar();
				}
			}
			catch (Exception)
			{
			}
		}

		public Bitmap ChangeBitmapColor(Bitmap bitmapImage, Color color)
		{
			Bitmap bitmap = new Bitmap(bitmapImage.Width, bitmapImage.Height);
			for (int i = 0; i < bitmapImage.Width; i++)
			{
				for (int j = 0; j < bitmapImage.Height; j++)
				{
					Color pixel = bitmapImage.GetPixel(i, j);
					if (pixel.A <= 150)
						bitmap.SetPixel(i, j, pixel);
					else
						bitmap.SetPixel(i, j, color);
				}
			}
			return bitmap;
		}

		private void SetDefaults()
		{
			try
			{
				InformationOptions.ActionBorderRadius = 1;
				InformationOptions.IconLeftMargin = 12;
				InformationOptions.ForeColor = Color.Black;
				InformationOptions.CloseIconColor = Color.FromArgb(145, 213, 255);
				InformationOptions.ActionForeColor = Color.Black;
				InformationOptions.BackColor = Color.White;
				InformationOptions.BorderColor = Color.White;
				InformationOptions.Icon = Resources.info;
				InformationOptions.ActionBackColor = InformationOptions.BackColor.LightenBy(_lightScaleFactor);
				InformationOptions.ActionBorderColor = InformationOptions.ActionBackColor;
				InformationOptions.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
				InformationOptions.ActionFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
				SuccessOptions.ActionBorderRadius = 1;
				SuccessOptions.IconLeftMargin = 12;
				SuccessOptions.ForeColor = Color.Black;
				SuccessOptions.CloseIconColor = Color.FromArgb(246, 255, 237);
				SuccessOptions.ActionForeColor = Color.Black;
				SuccessOptions.BackColor = Color.White;
				SuccessOptions.BorderColor = Color.White;
				SuccessOptions.Icon = Resources.success;
				SuccessOptions.ActionBackColor = SuccessOptions.BackColor.LightenBy(_lightScaleFactor);
				SuccessOptions.ActionBorderColor = SuccessOptions.ActionBackColor;
				SuccessOptions.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
				SuccessOptions.ActionFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
				WarningOptions.ActionBorderRadius = 1;
				WarningOptions.IconLeftMargin = 12;
				WarningOptions.ForeColor = Color.Black;
				WarningOptions.CloseIconColor = Color.FromArgb(255, 229, 143);
				WarningOptions.ActionForeColor = Color.Black;
				WarningOptions.BackColor = Color.White;
				WarningOptions.BorderColor = Color.White;
				WarningOptions.Icon = Resources.warning;
				WarningOptions.ActionBackColor = WarningOptions.BackColor.LightenBy(_lightWarningScaleFactor);
				WarningOptions.ActionBorderColor = WarningOptions.ActionBackColor;
				WarningOptions.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
				WarningOptions.ActionFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
				ErrorOptions.ActionBorderRadius = 1;
				ErrorOptions.IconLeftMargin = 12;
				ErrorOptions.ForeColor = Color.Black;
				ErrorOptions.CloseIconColor = Color.FromArgb(255, 204, 199);
				ErrorOptions.ActionForeColor = Color.Black;
				ErrorOptions.BackColor = Color.White;
				ErrorOptions.BorderColor = Color.White;
				ErrorOptions.Icon = Resources.error;
				ErrorOptions.ActionBackColor = ErrorOptions.BackColor.LightenBy(_lightScaleFactor);
				ErrorOptions.ActionBorderColor = ErrorOptions.ActionBackColor;
				ErrorOptions.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
				ErrorOptions.ActionFont = new Font("Segoe UI", 8.25f, FontStyle.Bold);
			}
			catch (Exception)
			{
			}
		}

		private void Customize(ref SnackbarView snackbar, MessageTypes type)
		{
			_options.Icon = null;
			switch (type)
			{
			case MessageTypes.Information:
				_options.ActionBackColor = InformationOptions.ActionBackColor;
				_options.ActionBorderColor = InformationOptions.ActionBorderColor;
				_options.ActionBorderRadius = InformationOptions.ActionBorderRadius;
				_options.ActionFont = InformationOptions.ActionFont;
				_options.ActionForeColor = InformationOptions.ActionForeColor;
				_options.BackColor = InformationOptions.BackColor;
				_options.BorderColor = InformationOptions.BorderColor;
				_options.CloseIconColor = InformationOptions.CloseIconColor;
				_options.Font = InformationOptions.Font;
				_options.ForeColor = InformationOptions.ForeColor;
				_options.IconLeftMargin = InformationOptions.IconLeftMargin;
				break;
			case MessageTypes.Success:
				_options.ActionBackColor = SuccessOptions.ActionBackColor;
				_options.ActionBorderColor = SuccessOptions.ActionBorderColor;
				_options.ActionBorderRadius = SuccessOptions.ActionBorderRadius;
				_options.ActionFont = SuccessOptions.ActionFont;
				_options.ActionForeColor = SuccessOptions.ActionForeColor;
				_options.BackColor = SuccessOptions.BackColor;
				_options.BorderColor = SuccessOptions.BorderColor;
				_options.CloseIconColor = SuccessOptions.CloseIconColor;
				_options.Font = SuccessOptions.Font;
				_options.ForeColor = SuccessOptions.ForeColor;
				_options.IconLeftMargin = SuccessOptions.IconLeftMargin;
				break;
			case MessageTypes.Warning:
				_options.ActionBackColor = WarningOptions.ActionBackColor;
				_options.ActionBorderColor = WarningOptions.ActionBorderColor;
				_options.ActionBorderRadius = WarningOptions.ActionBorderRadius;
				_options.ActionFont = WarningOptions.ActionFont;
				_options.ActionForeColor = WarningOptions.ActionForeColor;
				_options.BackColor = WarningOptions.BackColor;
				_options.BorderColor = WarningOptions.BorderColor;
				_options.CloseIconColor = WarningOptions.CloseIconColor;
				_options.Font = WarningOptions.Font;
				_options.ForeColor = WarningOptions.ForeColor;
				_options.IconLeftMargin = WarningOptions.IconLeftMargin;
				break;
			case MessageTypes.Error:
				_options.ActionBackColor = ErrorOptions.ActionBackColor;
				_options.ActionBorderColor = ErrorOptions.ActionBorderColor;
				_options.ActionBorderRadius = ErrorOptions.ActionBorderRadius;
				_options.ActionFont = ErrorOptions.ActionFont;
				_options.ActionForeColor = ErrorOptions.ActionForeColor;
				_options.BackColor = ErrorOptions.BackColor;
				_options.BorderColor = ErrorOptions.BorderColor;
				_options.CloseIconColor = ErrorOptions.CloseIconColor;
				_options.Font = ErrorOptions.Font;
				_options.ForeColor = ErrorOptions.ForeColor;
				_options.IconLeftMargin = ErrorOptions.IconLeftMargin;
				break;
			}
			if (ShowIcon)
			{
				if (_options.Icon != null)
					snackbar.pbIcon.Image = _options.Icon;
				else
				{
					switch (type)
					{
					case MessageTypes.Information:
						snackbar.pbIcon.Image = Resources.info;
						break;
					case MessageTypes.Success:
						snackbar.pbIcon.Image = Resources.success;
						break;
					case MessageTypes.Warning:
						snackbar.pbIcon.Image = Resources.warning;
						break;
					case MessageTypes.Error:
						snackbar.pbIcon.Image = Resources.error;
						break;
					}
				}
			}
			else
				snackbar.pbIcon.Image = null;
			snackbar.ShowShadows = ShowShadows;
			snackbar.ShowBorders = ShowBorders;
			snackbar.dckFormDock.AllowFormDropShadow = ShowShadows;
			snackbar.BackColor = _options.BackColor;
			snackbar.BorderColor = _options.BorderColor;
			snackbar.MessageForeColor = _options.ForeColor;
			snackbar.ClosingIconColor = _options.CloseIconColor;
			snackbar.SnackbarIconLeftMargin = _options.IconLeftMargin;
			snackbar.lblMessage.Font = _options.Font;
			snackbar.btnAction.Font = _options.ActionFont;
			snackbar.btnAction.IdleBorderRadius = _options.ActionBorderRadius;
			snackbar.btnAction.OnIdleState.ForeColor = _options.ActionForeColor;
			snackbar.btnAction.onHoverState.ForeColor = _options.ActionForeColor;
			snackbar.btnAction.OnPressedState.ForeColor = _options.ActionForeColor;
			snackbar.btnAction.OnIdleState.FillColor = _options.ActionBackColor;
			snackbar.btnAction.onHoverState.FillColor = _options.ActionBackColor;
			snackbar.btnAction.OnPressedState.FillColor = _options.ActionBackColor.DarkenBy(7);
			if (_options.ActionBorderColor.IsEmpty)
			{
				snackbar.btnAction.OnIdleState.BorderColor = _options.ActionBackColor;
				snackbar.btnAction.onHoverState.BorderColor = _options.ActionBackColor;
				snackbar.btnAction.OnPressedState.BorderColor = _options.ActionBackColor;
				snackbar.btnAction.IdleBorderColor = _options.ActionBackColor;
			}
			else
			{
				snackbar.btnAction.OnIdleState.BorderColor = _options.ActionBorderColor;
				snackbar.btnAction.onHoverState.BorderColor = _options.ActionBorderColor;
				snackbar.btnAction.OnPressedState.BorderColor = _options.ActionBorderColor;
				snackbar.btnAction.IdleBorderColor = _options.ActionBorderColor;
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
