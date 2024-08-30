using System;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlLinkClickedException : Exception
	{
		public HtmlLinkClickedException()
		{
		}

		public HtmlLinkClickedException(string message)
			: base(message)
		{
		}

		public HtmlLinkClickedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
