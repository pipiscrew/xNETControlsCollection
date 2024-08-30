namespace TheArtOfDev.HtmlRenderer.WinForms.Utilities
{
	internal struct BlendFunction
	{
		public byte BlendOp;

		public byte BlendFlags;

		public byte SourceConstantAlpha;

		public byte AlphaFormat;

		public BlendFunction(byte alpha)
		{
			BlendOp = 0;
			BlendFlags = 0;
			AlphaFormat = 0;
			SourceConstantAlpha = alpha;
		}
	}
}
