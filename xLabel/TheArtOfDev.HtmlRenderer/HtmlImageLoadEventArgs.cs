using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlImageLoadEventArgs : EventArgs
	{
		private bool _handled;

		private readonly string _src;

		private readonly Dictionary<string, string> _attributes;

		private readonly HtmlImageLoadCallback _callback;

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

		internal HtmlImageLoadEventArgs(string src, Dictionary<string, string> attributes, HtmlImageLoadCallback callback)
		{
			_src = src;
			_attributes = attributes;
			_callback = callback;
		}

		public void Callback()
		{
			_handled = true;
			_callback(null, null, default(RRect));
		}

		public void Callback(string path)
		{
			ArgChecker.AssertArgNotNullOrEmpty(path, "path");
			_handled = true;
			_callback(path, null, RRect.Empty);
		}

		public void Callback(string path, double x, double y, double width, double height)
		{
			ArgChecker.AssertArgNotNullOrEmpty(path, "path");
			_handled = true;
			_callback(path, null, new RRect(x, y, width, height));
		}

		public void Callback(object image)
		{
			ArgChecker.AssertArgNotNull(image, "image");
			_handled = true;
			_callback(null, image, RRect.Empty);
		}

		public void Callback(object image, double x, double y, double width, double height)
		{
			ArgChecker.AssertArgNotNull(image, "image");
			_handled = true;
			_callback(null, image, new RRect(x, y, width, height));
		}
	}
}
