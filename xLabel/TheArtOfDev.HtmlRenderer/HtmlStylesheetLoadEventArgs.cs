using System;
using System.Collections.Generic;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlStylesheetLoadEventArgs : EventArgs
	{
		private readonly string _src;

		private readonly Dictionary<string, string> _attributes;

		private string _setSrc;

		private string _setStyleSheet;

		private CssData _setStyleSheetData;

		public string Src
		{
			get
			{
				return _src;
			}
		}

		public Dictionary<string, string> Attributes
		{
			get
			{
				return _attributes;
			}
		}

		public string SetSrc
		{
			get
			{
				return _setSrc;
			}
			set
			{
				_setSrc = value;
			}
		}

		public string SetStyleSheet
		{
			get
			{
				return _setStyleSheet;
			}
			set
			{
				_setStyleSheet = value;
			}
		}

		public CssData SetStyleSheetData
		{
			get
			{
				return _setStyleSheetData;
			}
			set
			{
				_setStyleSheetData = value;
			}
		}

		internal HtmlStylesheetLoadEventArgs(string src, Dictionary<string, string> attributes)
		{
			_src = src;
			_attributes = attributes;
		}
	}
}
