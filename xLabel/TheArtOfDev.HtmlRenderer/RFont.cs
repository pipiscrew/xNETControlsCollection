namespace TheArtOfDev.HtmlRenderer.Adapters
{
	public abstract class RFont
	{
		public abstract double Size { get; }

		public abstract double Height { get; }

		public abstract double UnderlineOffset { get; }

		public abstract double LeftPadding { get; }

		public abstract double GetWhitespaceWidth(RGraphics graphics);
	}
}
