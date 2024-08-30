using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal static class BackgroundImageDrawHandler
	{
		public static void DrawBackgroundImage(RGraphics g, CssBox box, ImageLoadHandler imageLoadHandler, RRect rectangle)
		{
			RSize rSize = new RSize((imageLoadHandler.Rectangle == RRect.Empty) ? imageLoadHandler.Image.Width : imageLoadHandler.Rectangle.Width, (imageLoadHandler.Rectangle == RRect.Empty) ? imageLoadHandler.Image.Height : imageLoadHandler.Rectangle.Height);
			RPoint location = GetLocation(box.BackgroundPosition, rectangle, rSize);
			RRect srcRect = ((imageLoadHandler.Rectangle == RRect.Empty) ? new RRect(0.0, 0.0, rSize.Width, rSize.Height) : new RRect(imageLoadHandler.Rectangle.Left, imageLoadHandler.Rectangle.Top, rSize.Width, rSize.Height));
			RRect destRect = new RRect(location, rSize);
			RRect rect = rectangle;
			rect.Intersect(g.GetClip());
			g.PushClip(rect);
			switch (box.BackgroundRepeat)
			{
			default:
				DrawRepeat(g, imageLoadHandler, rectangle, srcRect, destRect, rSize);
				break;
			case "repeat-y":
				DrawRepeatY(g, imageLoadHandler, rectangle, srcRect, destRect, rSize);
				break;
			case "repeat-x":
				DrawRepeatX(g, imageLoadHandler, rectangle, srcRect, destRect, rSize);
				break;
			case "no-repeat":
				g.DrawImage(imageLoadHandler.Image, destRect, srcRect);
				break;
			}
			g.PopClip();
		}

		private static RPoint GetLocation(string backgroundPosition, RRect rectangle, RSize imgSize)
		{
			double x = rectangle.Left;
			if (backgroundPosition.IndexOf("left", StringComparison.OrdinalIgnoreCase) > -1)
				x = rectangle.Left + 0.5;
			else if (backgroundPosition.IndexOf("right", StringComparison.OrdinalIgnoreCase) > -1)
			{
				x = rectangle.Right - imgSize.Width;
			}
			else if (backgroundPosition.IndexOf("0", StringComparison.OrdinalIgnoreCase) < 0)
			{
				x = rectangle.Left + (rectangle.Width - imgSize.Width) / 2.0 + 0.5;
			}
			double y = rectangle.Top;
			if (backgroundPosition.IndexOf("top", StringComparison.OrdinalIgnoreCase) <= -1)
			{
				if (backgroundPosition.IndexOf("bottom", StringComparison.OrdinalIgnoreCase) > -1)
					y = rectangle.Bottom - imgSize.Height;
				else if (backgroundPosition.IndexOf("0", StringComparison.OrdinalIgnoreCase) < 0)
				{
					y = rectangle.Top + (rectangle.Height - imgSize.Height) / 2.0 + 0.5;
				}
			}
			else
				y = rectangle.Top;
			return new RPoint(x, y);
		}

		private static void DrawRepeatX(RGraphics g, ImageLoadHandler imageLoadHandler, RRect rectangle, RRect srcRect, RRect destRect, RSize imgSize)
		{
			while (destRect.X > rectangle.X)
			{
				destRect.X -= imgSize.Width;
			}
			using (RBrush brush = g.GetTextureBrush(imageLoadHandler.Image, srcRect, destRect.Location))
				g.DrawRectangle(brush, rectangle.X, destRect.Y, rectangle.Width, srcRect.Height);
		}

		private static void DrawRepeatY(RGraphics g, ImageLoadHandler imageLoadHandler, RRect rectangle, RRect srcRect, RRect destRect, RSize imgSize)
		{
			while (destRect.Y > rectangle.Y)
			{
				destRect.Y -= imgSize.Height;
			}
			using (RBrush brush = g.GetTextureBrush(imageLoadHandler.Image, srcRect, destRect.Location))
				g.DrawRectangle(brush, destRect.X, rectangle.Y, srcRect.Width, rectangle.Height);
		}

		private static void DrawRepeat(RGraphics g, ImageLoadHandler imageLoadHandler, RRect rectangle, RRect srcRect, RRect destRect, RSize imgSize)
		{
			while (destRect.X > rectangle.X)
			{
				destRect.X -= imgSize.Width;
			}
			while (destRect.Y > rectangle.Y)
			{
				destRect.Y -= imgSize.Height;
			}
			using (RBrush brush = g.GetTextureBrush(imageLoadHandler.Image, srcRect, destRect.Location))
				g.DrawRectangle(brush, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
		}
	}
}
