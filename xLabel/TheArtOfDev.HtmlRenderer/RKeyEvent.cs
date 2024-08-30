namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public sealed class RKeyEvent
	{
		private readonly bool _control;

		private readonly bool _aKeyCode;

		private readonly bool _cKeyCode;

		public bool Control
		{
			get
			{
				return _control;
			}
		}

		public bool AKeyCode
		{
			get
			{
				return _aKeyCode;
			}
		}

		public bool CKeyCode
		{
			get
			{
				return _cKeyCode;
			}
		}

		public RKeyEvent(bool control, bool aKeyCode, bool cKeyCode)
		{
			_control = control;
			_aKeyCode = aKeyCode;
			_cKeyCode = cKeyCode;
		}
	}
}
