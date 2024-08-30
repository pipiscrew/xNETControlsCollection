using System;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlRenderErrorEventArgs : EventArgs
	{
		private readonly HtmlRenderErrorType _type;

		private readonly string _message;

		private readonly Exception _exception;

		public HtmlRenderErrorType Type
		{
			get
			{
				return _type;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
		}

		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		public HtmlRenderErrorEventArgs(HtmlRenderErrorType type, string message, Exception exception = null)
		{
			_type = type;
			_message = message;
			_exception = exception;
		}

		public override string ToString()
		{
			return string.Format("Type: {0}", _type);
		}
	}
}
