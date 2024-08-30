using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal static class CssLayoutEngine
	{
		public static void MeasureImageSize(CssRectImage imageWord)
		{
			ArgChecker.AssertArgNotNull(imageWord, "imageWord");
			ArgChecker.AssertArgNotNull(imageWord.OwnerBox, "imageWord.OwnerBox");
			CssLength cssLength = new CssLength(imageWord.OwnerBox.Width);
			CssLength cssLength2 = new CssLength(imageWord.OwnerBox.Height);
			bool flag = cssLength.Number > 0.0 && cssLength.Unit == CssUnit.Pixels;
			bool flag2 = cssLength2.Number > 0.0 && cssLength2.Unit == CssUnit.Pixels;
			bool flag3 = false;
			if (flag)
				imageWord.Width = cssLength.Number;
			else if (cssLength.Number > 0.0 && cssLength.IsPercentage)
			{
				imageWord.Width = cssLength.Number * imageWord.OwnerBox.ContainingBlock.Size.Width;
				flag3 = true;
			}
			else if (imageWord.Image != null)
			{
				imageWord.Width = ((imageWord.ImageRectangle == RRect.Empty) ? imageWord.Image.Width : imageWord.ImageRectangle.Width);
			}
			else
			{
				imageWord.Width = (flag2 ? (cssLength2.Number / 1.1399999856948853) : 20.0);
			}
			CssLength cssLength3 = new CssLength(imageWord.OwnerBox.MaxWidth);
			if (cssLength3.Number > 0.0)
			{
				double num = -1.0;
				if (cssLength3.Unit != CssUnit.Pixels)
				{
					if (cssLength3.IsPercentage)
						num = cssLength3.Number * imageWord.OwnerBox.ContainingBlock.Size.Width;
				}
				else
					num = cssLength3.Number;
				if (num > -1.0 && imageWord.Width > num)
				{
					imageWord.Width = num;
					flag3 = !flag2;
				}
			}
			if (flag2)
				imageWord.Height = cssLength2.Number;
			else if (imageWord.Image != null)
			{
				imageWord.Height = ((imageWord.ImageRectangle == RRect.Empty) ? imageWord.Image.Height : imageWord.ImageRectangle.Height);
			}
			else
			{
				imageWord.Height = ((imageWord.Width > 0.0) ? (imageWord.Width * 1.1399999856948853) : 22.799999237060547);
			}
			if (imageWord.Image != null)
			{
				if ((flag && !flag2) || flag3)
				{
					double num2 = imageWord.Width / imageWord.Image.Width;
					imageWord.Height = imageWord.Image.Height * num2;
				}
				else if (flag2 && !flag)
				{
					double num3 = imageWord.Height / imageWord.Image.Height;
					imageWord.Width = imageWord.Image.Width * num3;
				}
			}
			imageWord.Height += imageWord.OwnerBox.ActualBorderBottomWidth + imageWord.OwnerBox.ActualBorderTopWidth + imageWord.OwnerBox.ActualPaddingTop + imageWord.OwnerBox.ActualPaddingBottom;
		}

		public static void CreateLineBoxes(RGraphics g, CssBox blockBox)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			ArgChecker.AssertArgNotNull(blockBox, "blockBox");
			blockBox.LineBoxes.Clear();
			double limitRight = blockBox.ActualRight - blockBox.ActualPaddingRight - blockBox.ActualBorderRightWidth;
			double num = blockBox.Location.X + blockBox.ActualPaddingLeft - 0.0 + blockBox.ActualBorderLeftWidth;
			double num2 = blockBox.Location.Y + blockBox.ActualPaddingTop - 0.0 + blockBox.ActualBorderTopWidth;
			double curx = num + blockBox.ActualTextIndent;
			double cury = num2;
			double maxRight = num;
			double maxbottom = num2;
			CssLineBox line = new CssLineBox(blockBox);
			FlowBox(g, blockBox, blockBox, limitRight, 0.0, num, ref line, ref curx, ref cury, ref maxRight, ref maxbottom);
			if (blockBox.ActualRight >= 90999.0)
				blockBox.ActualRight = maxRight + blockBox.ActualPaddingRight + blockBox.ActualBorderRightWidth;
			foreach (CssLineBox lineBox in blockBox.LineBoxes)
			{
				ApplyHorizontalAlignment(g, lineBox);
				ApplyRightToLeft(blockBox, lineBox);
				BubbleRectangles(blockBox, lineBox);
				ApplyVerticalAlignment(g, lineBox);
				lineBox.AssignRectanglesToBoxes();
			}
			blockBox.ActualBottom = maxbottom + blockBox.ActualPaddingBottom + blockBox.ActualBorderBottomWidth;
			if (blockBox.Height != null && blockBox.Height != "auto" && blockBox.Overflow == "hidden" && blockBox.ActualBottom - blockBox.Location.Y > blockBox.ActualHeight)
				blockBox.ActualBottom = blockBox.Location.Y + blockBox.ActualHeight;
		}

		public static void ApplyCellVerticalAlignment(RGraphics g, CssBox cell)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			ArgChecker.AssertArgNotNull(cell, "cell");
			if (cell.VerticalAlign == "top" || cell.VerticalAlign == "baseline")
				return;
			double clientBottom = cell.ClientBottom;
			double maximumBottom = cell.GetMaximumBottom(cell, 0.0);
			double amount = 0.0;
			if (cell.VerticalAlign == "bottom")
				amount = clientBottom - maximumBottom;
			else if (cell.VerticalAlign == "middle")
			{
				amount = (clientBottom - maximumBottom) / 2.0;
			}
			foreach (CssBox box in cell.Boxes)
			{
				box.OffsetTop(amount);
			}
		}

		private static void FlowBox(RGraphics g, CssBox blockbox, CssBox box, double limitRight, double linespacing, double startx, ref CssLineBox line, ref double curx, ref double cury, ref double maxRight, ref double maxbottom)
		{
			double num = curx;
			double num2 = cury;
			box.FirstHostingLineBox = line;
			double num3 = curx;
			double num4 = maxRight;
			double num5 = maxbottom;
			foreach (CssBox box2 in box.Boxes)
			{
				double num6 = ((!(box2.Position != "absolute") || !(box2.Position != "fixed")) ? 0.0 : (box2.ActualMarginLeft + box2.ActualBorderLeftWidth + box2.ActualPaddingLeft));
				double num7 = ((!(box2.Position != "absolute") || !(box2.Position != "fixed")) ? 0.0 : (box2.ActualMarginRight + box2.ActualBorderRightWidth + box2.ActualPaddingRight));
				box2.RectanglesReset();
				box2.MeasureWordsSize(g);
				curx += num6;
				if (box2.Words.Count > 0)
				{
					bool flag = false;
					if (box2.WhiteSpace == "nowrap" && curx > startx)
					{
						double num8 = curx;
						foreach (CssRect word in box2.Words)
						{
							num8 += word.FullWidth;
						}
						if (num8 > limitRight)
							flag = true;
					}
					if (DomUtils.IsBoxHasWhitespace(box2))
						curx += box.ActualWordSpacing;
					foreach (CssRect word2 in box2.Words)
					{
						if (maxbottom - cury < box.ActualLineHeight)
							maxbottom += box.ActualLineHeight - (maxbottom - cury);
						if ((box2.WhiteSpace != "nowrap" && box2.WhiteSpace != "pre" && curx + word2.Width + num7 > limitRight && (box2.WhiteSpace != "pre-wrap" || !word2.IsSpaces)) || word2.IsLineBreak || flag)
						{
							flag = false;
							curx = startx;
							if (box2 == box.Boxes[0] && !word2.IsLineBreak && (word2 == box2.Words[0] || (box.ParentBox != null && box.ParentBox.IsBlock)))
								curx += box.ActualMarginLeft + box.ActualBorderLeftWidth + box.ActualPaddingLeft;
							cury = maxbottom + linespacing;
							line = new CssLineBox(blockbox);
							if (word2.IsImage || word2.Equals(box2.FirstWord))
								curx += num6;
						}
						line.ReportExistanceOf(word2);
						word2.Left = curx;
						word2.Top = cury;
						if (!box.IsFixed)
							word2.BreakPage();
						curx = word2.Left + word2.FullWidth;
						maxRight = Math.Max(maxRight, word2.Right);
						maxbottom = Math.Max(maxbottom, word2.Bottom);
						if (box2.Position == "absolute")
						{
							word2.Left += box.ActualMarginLeft;
							word2.Top += box.ActualMarginTop;
						}
					}
				}
				else
					FlowBox(g, blockbox, box2, limitRight, linespacing, startx, ref line, ref curx, ref cury, ref maxRight, ref maxbottom);
				curx += num7;
			}
			if (maxbottom - num2 < box.ActualHeight)
				maxbottom += box.ActualHeight - (maxbottom - num2);
			if (box.IsInline && 0.0 <= curx - num && curx - num < box.ActualWidth)
			{
				curx += box.ActualWidth - (curx - num);
				line.Rectangles.Add(box, new RRect(num, num2, box.ActualWidth, box.ActualHeight));
			}
			if (box.Text != null && box.Text.IsWhitespace() && !box.IsImage && box.IsInline && box.Boxes.Count == 0 && box.Words.Count == 0)
				curx += box.ActualWordSpacing;
			if (box.Position == "absolute")
			{
				curx = num3;
				maxRight = num4;
				maxbottom = num5;
				AdjustAbsolutePosition(box, 0.0, 0.0);
			}
			box.LastHostingLineBox = line;
		}

		private static void AdjustAbsolutePosition(CssBox box, double left, double top)
		{
			left += box.ActualMarginLeft;
			top += box.ActualMarginTop;
			if (box.Words.Count > 0)
			{
				foreach (CssRect word in box.Words)
				{
					word.Left += left;
					word.Top += top;
				}
				return;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				AdjustAbsolutePosition(box2, left, top);
			}
		}

		private static void BubbleRectangles(CssBox box, CssLineBox line)
		{
			if (box.Words.Count > 0)
			{
				double num = 3.4028234663852886E+38;
				double num2 = 3.4028234663852886E+38;
				double num3 = -3.4028234663852886E+38;
				double num4 = -3.4028234663852886E+38;
				List<CssRect> list = line.WordsOf(box);
				if (list.Count <= 0)
					return;
				foreach (CssRect item in list)
				{
					double num5 = item.Left;
					if (box == box.ParentBox.Boxes[0] && item == box.Words[0] && item == line.Words[0] && line != line.OwnerBox.LineBoxes[0] && !item.IsLineBreak)
						num5 -= box.ParentBox.ActualMarginLeft + box.ParentBox.ActualBorderLeftWidth + box.ParentBox.ActualPaddingLeft;
					num = Math.Min(num, num5);
					num3 = Math.Max(num3, item.Right);
					num2 = Math.Min(num2, item.Top);
					num4 = Math.Max(num4, item.Bottom);
				}
				line.UpdateRectangle(box, num, num2, num3, num4);
				return;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				BubbleRectangles(box2, line);
			}
		}

		private static void ApplyHorizontalAlignment(RGraphics g, CssLineBox lineBox)
		{
			switch (lineBox.OwnerBox.TextAlign)
			{
			case "justify":
				ApplyJustifyAlignment(g, lineBox);
				break;
			default:
				ApplyLeftAlignment(g, lineBox);
				break;
			case "center":
				ApplyCenterAlignment(g, lineBox);
				break;
			case "right":
				ApplyRightAlignment(g, lineBox);
				break;
			}
		}

		private static void ApplyRightToLeft(CssBox blockBox, CssLineBox lineBox)
		{
			if (blockBox.Direction == "rtl")
			{
				ApplyRightToLeftOnLine(lineBox);
				return;
			}
			foreach (CssBox relatedBox in lineBox.RelatedBoxes)
			{
				if (relatedBox.Direction == "rtl")
					ApplyRightToLeftOnSingleBox(lineBox, relatedBox);
			}
		}

		private static void ApplyRightToLeftOnLine(CssLineBox line)
		{
			if (line.Words.Count <= 0)
				return;
			double left = line.Words[0].Left;
			double right = line.Words[line.Words.Count - 1].Right;
			foreach (CssRect word in line.Words)
			{
				double num = word.Left - left;
				double num2 = right - num;
				word.Left = num2 - word.Width;
			}
		}

		private static void ApplyRightToLeftOnSingleBox(CssLineBox lineBox, CssBox box)
		{
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < lineBox.Words.Count; i++)
			{
				if (lineBox.Words[i].OwnerBox == box)
				{
					if (num < 0)
						num = i;
					num2 = i;
				}
			}
			if (num > -1 && num2 > num)
			{
				double left = lineBox.Words[num].Left;
				double right = lineBox.Words[num2].Right;
				for (int j = num; j <= num2; j++)
				{
					double num3 = lineBox.Words[j].Left - left;
					double num4 = right - num3;
					lineBox.Words[j].Left = num4 - lineBox.Words[j].Width;
				}
			}
		}

		private static void ApplyVerticalAlignment(RGraphics g, CssLineBox lineBox)
		{
			double num = -3.4028234663852886E+38;
			foreach (CssBox key in lineBox.Rectangles.Keys)
			{
				num = Math.Max(num, lineBox.Rectangles[key].Top);
			}
			List<CssBox> list = new List<CssBox>(lineBox.Rectangles.Keys);
			foreach (CssBox item in list)
			{
				switch (item.VerticalAlign)
				{
				case "super":
					lineBox.SetBaseLine(g, item, num - lineBox.Rectangles[item].Height * 0.20000000298023224);
					break;
				default:
					lineBox.SetBaseLine(g, item, num);
					break;
				case "sub":
					lineBox.SetBaseLine(g, item, num + lineBox.Rectangles[item].Height * 0.5);
					break;
				case "top":
				case "bottom":
				case "text-bottom":
				case "middle":
				case "text-top":
					break;
				}
			}
		}

		private static void ApplyJustifyAlignment(RGraphics g, CssLineBox lineBox)
		{
			if (lineBox.Equals(lineBox.OwnerBox.LineBoxes[lineBox.OwnerBox.LineBoxes.Count - 1]))
				return;
			double num = (lineBox.Equals(lineBox.OwnerBox.LineBoxes[0]) ? lineBox.OwnerBox.ActualTextIndent : 0.0);
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = lineBox.OwnerBox.ClientRectangle.Width - num;
			foreach (CssRect word in lineBox.Words)
			{
				num2 += word.Width;
				num3 += 1.0;
			}
			if (num3 <= 0.0)
				return;
			double num5 = (num4 - num2) / num3;
			double left = lineBox.OwnerBox.ClientLeft + num;
			foreach (CssRect word2 in lineBox.Words)
			{
				word2.Left = left;
				left = word2.Right + num5;
				if (word2 == lineBox.Words[lineBox.Words.Count - 1])
					word2.Left = lineBox.OwnerBox.ClientRight - word2.Width;
			}
		}

		private static void ApplyCenterAlignment(RGraphics g, CssLineBox line)
		{
			if (line.Words.Count == 0)
				return;
			CssRect cssRect = line.Words[line.Words.Count - 1];
			double num = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
			double num2 = num - cssRect.Right - cssRect.OwnerBox.ActualBorderRightWidth - cssRect.OwnerBox.ActualPaddingRight;
			num2 /= 2.0;
			if (!(num2 > 0.0))
				return;
			foreach (CssRect word in line.Words)
			{
				word.Left += num2;
			}
			if (line.Rectangles.Count <= 0)
				return;
			foreach (CssBox item in ToList(line.Rectangles.Keys))
			{
				RRect rRect = line.Rectangles[item];
				line.Rectangles[item] = new RRect(rRect.X + num2, rRect.Y, rRect.Width, rRect.Height);
			}
		}

		private static void ApplyRightAlignment(RGraphics g, CssLineBox line)
		{
			if (line.Words.Count == 0)
				return;
			CssRect cssRect = line.Words[line.Words.Count - 1];
			double num = line.OwnerBox.ActualRight - line.OwnerBox.ActualPaddingRight - line.OwnerBox.ActualBorderRightWidth;
			double num2 = num - cssRect.Right - cssRect.OwnerBox.ActualBorderRightWidth - cssRect.OwnerBox.ActualPaddingRight;
			if (!(num2 > 0.0))
				return;
			foreach (CssRect word in line.Words)
			{
				word.Left += num2;
			}
			if (line.Rectangles.Count <= 0)
				return;
			foreach (CssBox item in ToList(line.Rectangles.Keys))
			{
				RRect rRect = line.Rectangles[item];
				line.Rectangles[item] = new RRect(rRect.X + num2, rRect.Y, rRect.Width, rRect.Height);
			}
		}

		private static void ApplyLeftAlignment(RGraphics g, CssLineBox line)
		{
		}

		private static List<T> ToList<T>(IEnumerable<T> collection)
		{
			List<T> list = new List<T>();
			foreach (T item in collection)
			{
				list.Add(item);
			}
			return list;
		}
	}
}
