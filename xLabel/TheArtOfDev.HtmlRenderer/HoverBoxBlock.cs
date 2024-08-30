using TheArtOfDev.HtmlRenderer.Core.Entities;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class HoverBoxBlock
	{
		private readonly CssBox _cssBox;

		private readonly CssBlock _cssBlock;

		public CssBox CssBox
		{
			get
			{
				return _cssBox;
			}
		}

		public CssBlock CssBlock
		{
			get
			{
				return _cssBlock;
			}
		}

		public HoverBoxBlock(CssBox cssBox, CssBlock cssBlock)
		{
			_cssBox = cssBox;
			_cssBlock = cssBlock;
		}
	}
}
