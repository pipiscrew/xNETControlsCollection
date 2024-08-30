using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal static class RenderUtils
	{
		public static bool IsColorVisible(RColor color)
		{
			return color.A > 0;
		}

		public static bool ClipGraphicsByOverflow(RGraphics g, CssBox box)
		{
			CssBox cssBox = box.ContainingBlock;
			while (true)
			{
				if (!(cssBox.Overflow == "hidden"))
				{
					CssBox containingBlock = cssBox.ContainingBlock;
					if (containingBlock == cssBox)
						break;
					cssBox = containingBlock;
					continue;
				}
				RRect clip = g.GetClip();
				RRect clientRectangle = box.ContainingBlock.ClientRectangle;
				clientRectangle.X -= 2.0;
				clientRectangle.Width += 2.0;
				if (!box.IsFixed)
					clientRectangle.Offset(box.HtmlContainer.ScrollOffset);
				clientRectangle.Intersect(clip);
				g.PushClip(clientRectangle);
				return true;
			}
			return false;
		}

		public static void DrawImageLoadingIcon(RGraphics g, HtmlContainerInt htmlContainer, RRect r)
		{
			g.DrawRectangle(g.GetPen(RColor.LightGray), r.Left + 3.0, r.Top + 3.0, 13.0, 14.0);
			RImage loadingImage = htmlContainer.Adapter.GetLoadingImage();
			g.DrawImage(loadingImage, new RRect(r.Left + 4.0, r.Top + 4.0, loadingImage.Width, loadingImage.Height));
		}

		public static void DrawImageErrorIcon(RGraphics g, HtmlContainerInt htmlContainer, RRect r)
		{
			g.DrawRectangle(g.GetPen(RColor.LightGray), r.Left + 2.0, r.Top + 2.0, 15.0, 15.0);
			RImage loadingFailedImage = htmlContainer.Adapter.GetLoadingFailedImage();
			g.DrawImage(loadingFailedImage, new RRect(r.Left + 3.0, r.Top + 3.0, loadingFailedImage.Width, loadingFailedImage.Height));
		}

		public static RGraphicsPath GetRoundRect(RGraphics g, RRect rect, double nwRadius, double neRadius, double seRadius, double swRadius)
		{
			RGraphicsPath graphicsPath = g.GetGraphicsPath();
			graphicsPath.Start(rect.Left + nwRadius, rect.Top);
			graphicsPath.LineTo(rect.Right - neRadius, rect.Y);
			if (neRadius > 0.0)
				graphicsPath.ArcTo(rect.Right, rect.Top + neRadius, neRadius, RGraphicsPath.Corner.TopRight);
			graphicsPath.LineTo(rect.Right, rect.Bottom - seRadius);
			if (seRadius > 0.0)
				graphicsPath.ArcTo(rect.Right - seRadius, rect.Bottom, seRadius, RGraphicsPath.Corner.BottomRight);
			graphicsPath.LineTo(rect.Left + swRadius, rect.Bottom);
			if (swRadius > 0.0)
				graphicsPath.ArcTo(rect.Left, rect.Bottom - swRadius, swRadius, RGraphicsPath.Corner.BottomLeft);
			graphicsPath.LineTo(rect.Left, rect.Top + nwRadius);
			if (nwRadius > 0.0)
				graphicsPath.ArcTo(rect.Left + nwRadius, rect.Top, nwRadius, RGraphicsPath.Corner.TopLeft);
			return graphicsPath;
		}
	}
}
