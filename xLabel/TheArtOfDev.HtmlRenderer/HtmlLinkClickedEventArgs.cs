using System;
using System.Collections.Generic;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlLinkClickedEventArgs : EventArgs
	{
		private readonly string _link;

		private readonly Dictionary<string, string> _attributes;

		private bool _handled;

		public string Link
		{
			get
			{
				return _link;
			}
		}

		public Dictionary<string, string> Attributes
		{
			get
			{
				return _attributes;
			}
		}

		public bool Handled
		{
			get
			{
				return _handled;
			}
			set
			{
				_handled = value;
			}
		}

		public HtmlLinkClickedEventArgs(string link, Dictionary<string, string> attributes)
		{
			_link = link;
			_attributes = attributes;
		}

		public override string ToString()
		{
			return string.Format("Link: {0}, Handled: {1}", _link, _handled);
		}
	}
}
