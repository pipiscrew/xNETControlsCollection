namespace TheArtOfDev.HtmlRenderer.Adapters.Entities
{
	public sealed class RMouseEvent
	{
		private readonly bool _leftButton;

		public bool LeftButton
		{
			get
			{
				return _leftButton;
			}
		}

		public RMouseEvent(bool leftButton)
		{
			_leftButton = leftButton;
		}
	}
}
