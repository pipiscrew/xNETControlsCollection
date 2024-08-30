using System;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class HtmlScrollEventArgs : EventArgs
	{
		private readonly RPoint _location;

		public double X
		{
			get
			{
				return _location.X;
			}
		}

		public double Y
		{
			get
			{
				return _location.Y;
			}
		}

		public HtmlScrollEventArgs(RPoint location)
		{
			_location = location;
		}

		public override string ToString()
		{
			return string.Format("Location: {0}", _location);
		}
	}
}
