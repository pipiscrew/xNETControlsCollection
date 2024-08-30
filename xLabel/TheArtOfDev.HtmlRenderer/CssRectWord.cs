namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssRectWord : CssRect
	{
		private readonly string _text;

		private readonly bool _hasSpaceBefore;

		private readonly bool _hasSpaceAfter;

		public override bool HasSpaceBefore
		{
			get
			{
				return _hasSpaceBefore;
			}
		}

		public override bool HasSpaceAfter
		{
			get
			{
				return _hasSpaceAfter;
			}
		}

		public override bool IsSpaces
		{
			get
			{
				string text = Text;
				foreach (char c in text)
				{
					if (!char.IsWhiteSpace(c))
						return false;
				}
				return true;
			}
		}

		public override bool IsLineBreak
		{
			get
			{
				return Text == "\n";
			}
		}

		public override string Text
		{
			get
			{
				return _text;
			}
		}

		public CssRectWord(CssBox owner, string text, bool hasSpaceBefore, bool hasSpaceAfter)
			: base(owner)
		{
			_text = text;
			_hasSpaceBefore = hasSpaceBefore;
			_hasSpaceAfter = hasSpaceAfter;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, (Text.Length != 1) ? "s" : string.Empty);
		}
	}
}
