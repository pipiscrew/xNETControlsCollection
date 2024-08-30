using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class ImageAdapter : RImage
	{
		private readonly Image _image;

		public Image Image
		{
			get
			{
				return _image;
			}
		}

		public override double Width
		{
			get
			{
				return _image.Width;
			}
		}

		public override double Height
		{
			get
			{
				return _image.Height;
			}
		}

		public ImageAdapter(Image image)
		{
			_image = image;
		}

		public override void Dispose()
		{
			_image.Dispose();
		}
	}
}
