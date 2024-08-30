using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class FontFamilyAdapter : RFontFamily
	{
		private readonly FontFamily _fontFamily;

		public FontFamily FontFamily
		{
			get
			{
				return _fontFamily;
			}
		}

		public override string Name
		{
			get
			{
				return _fontFamily.Name;
			}
		}

		public FontFamilyAdapter(FontFamily fontFamily)
		{
			_fontFamily = fontFamily;
		}
	}
}
