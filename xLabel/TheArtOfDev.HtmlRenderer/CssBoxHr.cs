using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssBoxHr : CssBox
	{
		public CssBoxHr(CssBox parent, HtmlTag tag)
			: base(parent, tag)
		{
			base.Display = "block";
		}

		protected override void PerformLayoutImp(RGraphics g)
		{
			if (!(base.Display == "none"))
			{
				RectanglesReset();
				CssBox previousSibling = DomUtils.GetPreviousSibling(this);
				double x = base.ContainingBlock.Location.X + base.ContainingBlock.ActualPaddingLeft + base.ActualMarginLeft + base.ContainingBlock.ActualBorderLeftWidth;
				double num = ((previousSibling == null && base.ParentBox != null) ? base.ParentBox.ClientTop : ((base.ParentBox == null) ? base.Location.Y : 0.0)) + MarginTopCollapse(previousSibling) + ((previousSibling != null) ? (previousSibling.ActualBottom + previousSibling.ActualBorderBottomWidth) : 0.0);
				base.Location = new RPoint(x, num);
				base.ActualBottom = num;
				double minimumWidth = GetMinimumWidth();
				double num2 = base.ContainingBlock.Size.Width - base.ContainingBlock.ActualPaddingLeft - base.ContainingBlock.ActualPaddingRight - base.ContainingBlock.ActualBorderLeftWidth - base.ContainingBlock.ActualBorderRightWidth - base.ActualMarginLeft - base.ActualMarginRight - base.ActualBorderLeftWidth - base.ActualBorderRightWidth;
				if (base.Width != "auto" && !string.IsNullOrEmpty(base.Width))
					num2 = CssValueParser.ParseLength(base.Width, num2, this);
				if (num2 < minimumWidth || num2 >= 9999.0)
					num2 = minimumWidth;
				double num3 = base.ActualHeight;
				if (num3 < 1.0)
					num3 = base.Size.Height + base.ActualBorderTopWidth + base.ActualBorderBottomWidth;
				if (num3 < 1.0)
					num3 = 2.0;
				if (num3 <= 2.0 && base.ActualBorderTopWidth < 1.0 && base.ActualBorderBottomWidth < 1.0)
				{
					string text3 = (base.BorderTopStyle = (base.BorderBottomStyle = "solid"));
					base.BorderTopWidth = "1px";
					base.BorderBottomWidth = "1px";
				}
				base.Size = new RSize(num2, num3);
				base.ActualBottom = base.Location.Y + base.ActualPaddingTop + base.ActualPaddingBottom + num3;
			}
		}

		protected override void PaintImp(RGraphics g)
		{
			RPoint rPoint = ((base.HtmlContainer == null || IsFixed) ? RPoint.Empty : base.HtmlContainer.ScrollOffset);
			RRect rectangle = new RRect(base.Bounds.X + rPoint.X, base.Bounds.Y + rPoint.Y, base.Bounds.Width, base.Bounds.Height);
			if (rectangle.Height > 2.0 && RenderUtils.IsColorVisible(base.ActualBackgroundColor))
				g.DrawRectangle(g.GetSolidBrush(base.ActualBackgroundColor), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			RBrush solidBrush = g.GetSolidBrush(base.ActualBorderTopColor);
			BordersDrawHandler.DrawBorder(Border.Top, g, this, solidBrush, rectangle);
			if (rectangle.Height > 1.0)
			{
				RBrush solidBrush2 = g.GetSolidBrush(base.ActualBorderLeftColor);
				BordersDrawHandler.DrawBorder(Border.Left, g, this, solidBrush2, rectangle);
				RBrush solidBrush3 = g.GetSolidBrush(base.ActualBorderRightColor);
				BordersDrawHandler.DrawBorder(Border.Right, g, this, solidBrush3, rectangle);
				RBrush solidBrush4 = g.GetSolidBrush(base.ActualBorderBottomColor);
				BordersDrawHandler.DrawBorder(Border.Bottom, g, this, solidBrush4, rectangle);
			}
		}
	}
}
