using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	[Browsable(false)]
	[ToolboxItem(false)]
	internal class PlaceholderTextBox : TextBox
	{
		private const int WM_SETFOCUS = 7;

		private const int WM_KILLFOCUS = 8;

		private int customMaxLength;

		private bool _isPlaceholderActive = true;

		private bool selectionSet;

		private bool avoidTextChanged;

		private const string DEFAULT_PLACEHOLDER = "Enter text";

		private string _placeholderText = "Enter text";

		private Color _placeholderTextColor;

		private Color _backColorDisabled = Color.AliceBlue;

		private Color _foreColorDisabled = Color.Green;

        private bool selectionEnabled = true;

        public bool SelectionEnabled
        {
            get { return selectionEnabled; }
            set { selectionEnabled = value; }
        }


		[Browsable(false)]
		public bool IsPlaceholderActive
		{
			get
			{
				return _isPlaceholderActive;
			}
			private set
			{
				if (_isPlaceholderActive != value)
				{
					SetStyle(ControlStyles.UserMouse, value);
					Invalidate();
					_isPlaceholderActive = value;
					OnPlaceholderActiveChanged(value);
				}
			}
		}

		[Description("The placeholder associated with the control.")]
		[DefaultValue("Enter text")]
		public string PlaceholderText
		{
			get
			{
				return _placeholderText;
			}
			set
			{
				_placeholderText = value;
				if (IsPlaceholderActive)
				{
					if (PlaceholderText.Length >= MaxLength)
						Text = string.Empty;
					else
						Text = value;
				}
			}
		}

		[Browsable(false)]
		public override string Text
		{
			get
			{
				if (IsPlaceholderActive && base.Text == PlaceholderText)
					return "";
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		public override int MaxLength
		{
			get
			{
				return base.MaxLength;
			}
			set
			{
				base.MaxLength = value;
				if (IsPlaceholderActive && PlaceholderText.Length >= value)
					PlaceholderText = string.Empty;
			}
		}

		public Color PlaceholderTextColor
		{
			get
			{
				return _placeholderTextColor;
			}
			set
			{
				if (!(_placeholderTextColor == value))
				{
					_placeholderTextColor = value;
					if (base.DesignMode)
						Invalidate();
				}
			}
		}

		public Color TextColor { get; set; }

		[Browsable(false)]
		public override Color ForeColor
		{
			get
			{
				if (IsPlaceholderActive)
					return PlaceholderTextColor;
				return TextColor;
			}
			set
			{
				TextColor = value;
			}
		}

		[Description("Occurs when the value of the IsPlaceholderInside property has changed.")]
		public event EventHandler<PlaceholderActiveChangedEventArgs> PlaceholderActiveChanged;

		public PlaceholderTextBox()
		{
			base.Text = PlaceholderText;
			TextColor = SystemColors.WindowText;
			PlaceholderTextColor = SystemColors.InactiveCaption;
			SetStyle(ControlStyles.UserMouse, true);
		}

		public void Reset()
		{
			if (!IsPlaceholderActive)
			{
				IsPlaceholderActive = true;
				Text = PlaceholderText;
				Select(0, 0);
			}
		}

		private void ActionWithoutTextChanged(Action act)
		{
			avoidTextChanged = true;
			act();
			avoidTextChanged = false;
		}

		private void UpdateText()
		{
			ActionWithoutTextChanged(delegate
			{
				if (!IsPlaceholderActive && string.IsNullOrEmpty(Text))
					Reset();
				else if (IsPlaceholderActive && Text.Length > 0)
				{
					IsPlaceholderActive = false;
					if (Text.EndsWith(PlaceholderText))
						Text = Text.Substring(0, TextLength - PlaceholderText.Length);
					if (Text.Length > MaxLength)
						Text = Text.Substring(0, MaxLength);
					Select(TextLength, 0);
				}
			});
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 7 && !SelectionEnabled)
				m.Msg = 8;
			base.WndProc(ref m);
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
		}

		protected override void OnTextAlignChanged(EventArgs e)
		{
			base.OnTextAlignChanged(e);
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if (!avoidTextChanged)
			{
				UpdateText();
				base.OnTextChanged(e);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (IsPlaceholderActive && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Delete))
				e.Handled = true;
			if (IsPlaceholderActive && e.KeyCode == Keys.A && e.Modifiers.HasFlag(Keys.Control))
			{
				e.Handled = true;
				e.SuppressKeyPress = true;
			}
			base.OnKeyDown(e);
		}

		protected virtual void OnPlaceholderActiveChanged(bool newValue)
		{
			if (this.PlaceholderActiveChanged != null)
				this.PlaceholderActiveChanged(this, new PlaceholderActiveChangedEventArgs(newValue));
		}

		protected override void OnGotFocus(EventArgs e)
		{
			bool flag = false;
			if (!selectionSet)
			{
				selectionSet = true;
				if (SelectionLength == 0 && Control.MouseButtons == MouseButtons.None)
					flag = true;
			}
			base.OnGotFocus(e);
			if (flag)
			{
				base.SelectionStart = 0;
				DeselectAll();
			}
		}
	}
}
