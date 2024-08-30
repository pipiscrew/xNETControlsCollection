namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class LinkElementData<T>
	{
		private readonly string _id;

		private readonly string _href;

		private readonly T _rectangle;

		public string Id
		{
			get
			{
				return _id;
			}
		}

		public string Href
		{
			get
			{
				return _href;
			}
		}

		public T Rectangle
		{
			get
			{
				return _rectangle;
			}
		}

		public bool IsAnchor
		{
			get
			{
				return _href.Length > 0 && _href[0] == '#';
			}
		}

		public string AnchorId
		{
			get
			{
				return (!IsAnchor || _href.Length <= 1) ? string.Empty : _href.Substring(1);
			}
		}

		public LinkElementData(string id, string href, T rectangle)
		{
			_id = id;
			_href = href;
			_rectangle = rectangle;
		}

		public override string ToString()
		{
			return string.Format("Id: {0}, Href: {1}, Rectangle: {2}", _id, _href, _rectangle);
		}
	}
}
