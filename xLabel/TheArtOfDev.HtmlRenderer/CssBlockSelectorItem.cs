using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public struct CssBlockSelectorItem
	{
		private readonly string _class;

		private readonly bool _directParent;

		public string Class
		{
			get
			{
				return _class;
			}
		}

		public bool DirectParent
		{
			get
			{
				return _directParent;
			}
		}

		public CssBlockSelectorItem(string @class, bool directParent)
		{
			ArgChecker.AssertArgNotNullOrEmpty(@class, "@class");
			_class = @class;
			_directParent = directParent;
		}

		public override string ToString()
		{
			return _class + (_directParent ? " > " : string.Empty);
		}
	}
}
