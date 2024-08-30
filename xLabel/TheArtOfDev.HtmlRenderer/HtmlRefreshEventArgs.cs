using System;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlRefreshEventArgs : EventArgs
	{
		private readonly bool _layout;

		public bool Layout
		{
			get
			{
				return _layout;
			}
		}

		public HtmlRefreshEventArgs(bool layout)
		{
			_layout = layout;
		}

		public override string ToString()
		{
			return string.Format("Layout: {0}", _layout);
		}
	}
}
