using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssRectImage : CssRect
	{
		private RImage _image;

		private RRect _imageRectangle;

		public override RImage Image
		{
			get
			{
				return _image;
			}
			set
			{
				_image = value;
			}
		}

		public override bool IsImage
		{
			get
			{
				return true;
			}
		}

		public RRect ImageRectangle
		{
			get
			{
				return _imageRectangle;
			}
			set
			{
				_imageRectangle = value;
			}
		}

		public CssRectImage(CssBox owner)
			: base(owner)
		{
		}

		public override string ToString()
		{
			return "Image";
		}
	}
}
