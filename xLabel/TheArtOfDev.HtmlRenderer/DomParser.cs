using System;
using System.Collections.Generic;
using System.Globalization;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Parse
{
	internal sealed class DomParser
	{
		private readonly CssParser _cssParser;

		public DomParser(CssParser cssParser)
		{
			ArgChecker.AssertArgNotNull(cssParser, "cssParser");
			_cssParser = cssParser;
		}

		public CssBox GenerateCssTree(string html, HtmlContainerInt htmlContainer, ref CssData cssData)
		{
			CssBox cssBox = HtmlParser.ParseDocument(html);
			if (cssBox != null)
			{
				cssBox.HtmlContainer = htmlContainer;
				bool cssDataChanged = false;
				CascadeParseStyles(cssBox, htmlContainer, ref cssData, ref cssDataChanged);
				CascadeApplyStyles(cssBox, cssData);
				SetTextSelectionStyle(htmlContainer, cssData);
				CorrectTextBoxes(cssBox);
				CorrectImgBoxes(cssBox);
				bool followingBlock = true;
				CorrectLineBreaksBlocks(cssBox, ref followingBlock);
				CorrectInlineBoxesParent(cssBox);
				CorrectBlockInsideInline(cssBox);
				CorrectInlineBoxesParent(cssBox);
			}
			return cssBox;
		}

		private void CascadeParseStyles(CssBox box, HtmlContainerInt htmlContainer, ref CssData cssData, ref bool cssDataChanged)
		{
			if (box.HtmlTag != null)
			{
				if (box.HtmlTag.Name.Equals("link", StringComparison.CurrentCultureIgnoreCase) && box.GetAttribute("rel", string.Empty).Equals("stylesheet", StringComparison.CurrentCultureIgnoreCase))
				{
					CloneCssData(ref cssData, ref cssDataChanged);
					string stylesheet;
					CssData stylesheetData;
					StylesheetLoadHandler.LoadStylesheet(htmlContainer, box.GetAttribute("href", string.Empty), box.HtmlTag.Attributes, out stylesheet, out stylesheetData);
					if (stylesheet != null)
						_cssParser.ParseStyleSheet(cssData, stylesheet);
					else if (stylesheetData != null)
					{
						cssData.Combine(stylesheetData);
					}
				}
				if (box.HtmlTag.Name.Equals("style", StringComparison.CurrentCultureIgnoreCase) && box.Boxes.Count > 0)
				{
					CloneCssData(ref cssData, ref cssDataChanged);
					foreach (CssBox box2 in box.Boxes)
					{
						_cssParser.ParseStyleSheet(cssData, box2.Text.CutSubstring());
					}
				}
			}
			foreach (CssBox box3 in box.Boxes)
			{
				CascadeParseStyles(box3, htmlContainer, ref cssData, ref cssDataChanged);
			}
		}

		private void CascadeApplyStyles(CssBox box, CssData cssData)
		{
			box.InheritStyle();
			if (box.HtmlTag != null)
			{
				AssignCssBlocks(box, cssData, "*");
				AssignCssBlocks(box, cssData, box.HtmlTag.Name);
				if (box.HtmlTag.HasAttribute("class"))
					AssignClassCssBlocks(box, cssData);
				if (box.HtmlTag.HasAttribute("id"))
				{
					string text = box.HtmlTag.TryGetAttribute("id");
					AssignCssBlocks(box, cssData, "#" + text);
				}
				TranslateAttributes(box.HtmlTag, box);
				if (box.HtmlTag.HasAttribute("style"))
				{
					CssBlock cssBlock = _cssParser.ParseCssBlock(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
					if (cssBlock != null)
						AssignCssBlock(box, cssBlock);
				}
			}
			if (box.TextDecoration != string.Empty && box.Text == null)
			{
				foreach (CssBox box2 in box.Boxes)
				{
					box2.TextDecoration = box.TextDecoration;
				}
				box.TextDecoration = string.Empty;
			}
			foreach (CssBox box3 in box.Boxes)
			{
				CascadeApplyStyles(box3, cssData);
			}
		}

		private void SetTextSelectionStyle(HtmlContainerInt htmlContainer, CssData cssData)
		{
			htmlContainer.SelectionForeColor = RColor.Empty;
			htmlContainer.SelectionBackColor = RColor.Empty;
			if (!cssData.ContainsCssBlock("::selection"))
				return;
			IEnumerable<CssBlock> cssBlock = cssData.GetCssBlock("::selection");
			foreach (CssBlock item in cssBlock)
			{
				if (item.Properties.ContainsKey("color"))
					htmlContainer.SelectionForeColor = _cssParser.ParseColor(item.Properties["color"]);
				if (item.Properties.ContainsKey("background-color"))
					htmlContainer.SelectionBackColor = _cssParser.ParseColor(item.Properties["background-color"]);
			}
		}

		private static void AssignClassCssBlocks(CssBox box, CssData cssData)
		{
			string text = box.HtmlTag.TryGetAttribute("class");
			int i = 0;
			while (i < text.Length)
			{
				for (; i < text.Length && text[i] == ' '; i++)
				{
				}
				if (i < text.Length)
				{
					int num = text.IndexOf(' ', i);
					if (num < 0)
						num = text.Length;
					string text2 = "." + text.Substring(i, num - i);
					AssignCssBlocks(box, cssData, text2);
					AssignCssBlocks(box, cssData, box.HtmlTag.Name + text2);
					i = num + 1;
				}
			}
		}

		private static void AssignCssBlocks(CssBox box, CssData cssData, string className)
		{
			IEnumerable<CssBlock> cssBlock = cssData.GetCssBlock(className);
			foreach (CssBlock item in cssBlock)
			{
				if (IsBlockAssignableToBox(box, item))
					AssignCssBlock(box, item);
			}
		}

		private static bool IsBlockAssignableToBox(CssBox box, CssBlock block)
		{
			bool flag = true;
			if (block.Selectors != null)
				flag = IsBlockAssignableToBoxWithSelector(box, block);
			else if (box.HtmlTag.Name.Equals("a", StringComparison.OrdinalIgnoreCase) && block.Class.Equals("a", StringComparison.OrdinalIgnoreCase) && !box.HtmlTag.HasAttribute("href"))
			{
				flag = false;
			}
			if (flag && block.Hover)
			{
				box.HtmlContainer.AddHoverBox(box, block);
				flag = false;
			}
			return flag;
		}

		private static bool IsBlockAssignableToBoxWithSelector(CssBox box, CssBlock block)
		{
			foreach (CssBlockSelectorItem selector in block.Selectors)
			{
				bool flag = false;
				while (!flag)
				{
					box = box.ParentBox;
					while (box != null && box.HtmlTag == null)
					{
						box = box.ParentBox;
					}
					if (box != null)
					{
						if (box.HtmlTag.Name.Equals(selector.Class, StringComparison.InvariantCultureIgnoreCase))
							flag = true;
						if (!flag && box.HtmlTag.HasAttribute("class"))
						{
							string text = box.HtmlTag.TryGetAttribute("class");
							if (selector.Class.Equals("." + text, StringComparison.InvariantCultureIgnoreCase) || selector.Class.Equals(box.HtmlTag.Name + "." + text, StringComparison.InvariantCultureIgnoreCase))
								flag = true;
						}
						if (!flag && box.HtmlTag.HasAttribute("id"))
						{
							string text2 = box.HtmlTag.TryGetAttribute("id");
							if (selector.Class.Equals("#" + text2, StringComparison.InvariantCultureIgnoreCase))
								flag = true;
						}
						if (!flag && selector.DirectParent)
							return false;
						continue;
					}
					return false;
				}
			}
			return true;
		}

		private static void AssignCssBlock(CssBox box, CssBlock block)
		{
			foreach (KeyValuePair<string, string> property in block.Properties)
			{
				string value = property.Value;
				if (property.Value == "inherit" && box.ParentBox != null)
					value = CssUtils.GetPropertyValue(box.ParentBox, property.Key);
				if (IsStyleOnElementAllowed(box, property.Key, value))
					CssUtils.SetPropertyValue(box, property.Key, value);
			}
		}

		private static bool IsStyleOnElementAllowed(CssBox box, string key, string value)
		{
			if (box.HtmlTag != null && key == "display")
			{
				switch (box.HtmlTag.Name)
				{
				case "table":
					return value == "table";
				case "tr":
					return value == "table-row";
				case "th":
				case "td":
					return value == "table-cell";
				case "colgroup":
					return value == "table-column-group";
				case "thead":
					return value == "table-header-group";
				case "tbody":
					return value == "table-row-group";
				case "col":
					return value == "table-column";
				case "caption":
					return value == "table-caption";
				case "tfoot":
					return value == "table-footer-group";
				}
			}
			return true;
		}

		private static void CloneCssData(ref CssData cssData, ref bool cssDataChanged)
		{
			if (!cssDataChanged)
			{
				cssDataChanged = true;
				cssData = cssData.Clone();
			}
		}

		private void TranslateAttributes(HtmlTag tag, CssBox box)
		{
			if (!tag.HasAttributes())
				return;
			foreach (string key in tag.Attributes.Keys)
			{
				string text = tag.Attributes[key];
				switch (key)
				{
				case "hspace":
				{
					string text4 = (box.MarginRight = (box.MarginLeft = TranslateLength(text)));
					break;
				}
				case "face":
					box.FontFamily = _cssParser.ParseFontFamily(text);
					break;
				case "size":
					if (tag.Name.Equals("hr", StringComparison.OrdinalIgnoreCase))
						box.Height = TranslateLength(text);
					else if (tag.Name.Equals("font", StringComparison.OrdinalIgnoreCase))
					{
						box.FontSize = text;
					}
					break;
				case "border":
				{
					string text6;
					string text8;
					string text4;
					if (!string.IsNullOrEmpty(text) && text != "0")
					{
						text6 = (box.BorderBottomStyle = "solid");
						text8 = (box.BorderRightStyle = text6);
						text4 = (box.BorderLeftStyle = (box.BorderTopStyle = text8));
					}
					text6 = (box.BorderBottomWidth = TranslateLength(text));
					text8 = (box.BorderRightWidth = text6);
					text4 = (box.BorderLeftWidth = (box.BorderTopWidth = text8));
					if (tag.Name == "table")
					{
						if (text != "0")
							ApplyTableBorder(box, "1px");
					}
					else
					{
						text6 = (box.BorderBottomStyle = "solid");
						text8 = (box.BorderRightStyle = text6);
						text4 = (box.BorderTopStyle = (box.BorderLeftStyle = text8));
					}
					break;
				}
				case "background":
					box.BackgroundImage = text.ToLower();
					break;
				case "color":
					box.Color = text.ToLower();
					break;
				case "align":
					switch (text)
					{
					default:
						if (!(text == "justify"))
						{
							box.VerticalAlign = text.ToLower();
							break;
						}
						goto case "left";
					case "left":
					case "center":
					case "right":
						box.TextAlign = text.ToLower();
						break;
					}
					break;
				case "valign":
					box.VerticalAlign = text.ToLower();
					break;
				case "bordercolor":
				{
					string text6 = (box.BorderBottomColor = text.ToLower());
					string text8 = (box.BorderRightColor = text6);
					string text4 = (box.BorderLeftColor = (box.BorderTopColor = text8));
					break;
				}
				case "cellpadding":
					ApplyTablePadding(box, text);
					break;
				case "width":
					box.Width = TranslateLength(text);
					break;
				case "nowrap":
					box.WhiteSpace = "nowrap";
					break;
				case "bgcolor":
					box.BackgroundColor = text.ToLower();
					break;
				case "cellspacing":
					box.BorderSpacing = TranslateLength(text);
					break;
				case "vspace":
				{
					string text4 = (box.MarginTop = (box.MarginBottom = TranslateLength(text)));
					break;
				}
				case "dir":
					box.Direction = text.ToLower();
					break;
				case "height":
					box.Height = TranslateLength(text);
					break;
				}
			}
		}

		private static string TranslateLength(string htmlLength)
		{
			CssLength cssLength = new CssLength(htmlLength);
			if (cssLength.HasError)
				return string.Format(NumberFormatInfo.InvariantInfo, "{0}px", new object[1] { htmlLength });
			return htmlLength;
		}

		private static void ApplyTableBorder(CssBox table, string border)
		{
			SetForAllCells(table, delegate(CssBox cell)
			{
				string text2 = (cell.BorderBottomStyle = "solid");
				string text4 = (cell.BorderRightStyle = text2);
				string text7 = (cell.BorderLeftStyle = (cell.BorderTopStyle = text4));
				text2 = (cell.BorderBottomWidth = border);
				text4 = (cell.BorderRightWidth = text2);
				text7 = (cell.BorderLeftWidth = (cell.BorderTopWidth = text4));
			});
		}

		private static void ApplyTablePadding(CssBox table, string padding)
		{
			string length = TranslateLength(padding);
			SetForAllCells(table, delegate(CssBox cell)
			{
				string text2 = (cell.PaddingBottom = length);
				string text4 = (cell.PaddingRight = text2);
				string text7 = (cell.PaddingLeft = (cell.PaddingTop = text4));
			});
		}

		private static void SetForAllCells(CssBox table, ActionInt<CssBox> action)
		{
			foreach (CssBox box in table.Boxes)
			{
				foreach (CssBox box2 in box.Boxes)
				{
					if (box2.HtmlTag != null && box2.HtmlTag.Name == "td")
					{
						action(box2);
						continue;
					}
					foreach (CssBox box3 in box2.Boxes)
					{
						action(box3);
					}
				}
			}
		}

		private static void CorrectTextBoxes(CssBox box)
		{
			for (int num = box.Boxes.Count - 1; num >= 0; num--)
			{
				CssBox cssBox = box.Boxes[num];
				if (cssBox.Text == null)
					CorrectTextBoxes(cssBox);
				else if (!cssBox.Text.IsEmptyOrWhitespace() || cssBox.WhiteSpace == "pre" || cssBox.WhiteSpace == "pre-wrap" || box.Boxes.Count == 1 || (num > 0 && num < box.Boxes.Count - 1 && box.Boxes[num - 1].IsInline && box.Boxes[num + 1].IsInline) || (num == 0 && box.Boxes.Count > 1 && box.Boxes[1].IsInline && box.IsInline) || (num == box.Boxes.Count - 1 && box.Boxes.Count > 1 && box.Boxes[num - 1].IsInline && box.IsInline))
				{
					cssBox.ParseToWords();
				}
				else
				{
					cssBox.ParentBox.Boxes.RemoveAt(num);
				}
			}
		}

		private static void CorrectImgBoxes(CssBox box)
		{
			for (int num = box.Boxes.Count - 1; num >= 0; num--)
			{
				CssBox cssBox = box.Boxes[num];
				if (cssBox is CssBoxImage && cssBox.Display == "block")
				{
					CssBox cssBox3 = (cssBox.ParentBox = CssBox.CreateBlock(cssBox.ParentBox, null, cssBox));
					cssBox.Display = "inline";
				}
				else
					CorrectImgBoxes(cssBox);
			}
		}

		private static void CorrectLineBreaksBlocks(CssBox box, ref bool followingBlock)
		{
			followingBlock = followingBlock || box.IsBlock;
			foreach (CssBox box2 in box.Boxes)
			{
				CorrectLineBreaksBlocks(box2, ref followingBlock);
				followingBlock = box2.Words.Count == 0 && (followingBlock || box2.IsBlock);
			}
			int num = -1;
			CssBox cssBox;
			do
			{
				cssBox = null;
				for (int i = 0; i < box.Boxes.Count; i++)
				{
					if (cssBox != null)
						break;
					if (i > num && box.Boxes[i].IsBrElement)
					{
						cssBox = box.Boxes[i];
						num = i;
					}
					else if (box.Boxes[i].Words.Count > 0)
					{
						followingBlock = false;
					}
					else if (box.Boxes[i].IsBlock)
					{
						followingBlock = true;
					}
				}
				if (cssBox != null)
				{
					cssBox.Display = "block";
					if (followingBlock)
						cssBox.Height = ".95em";
				}
			}
			while (cssBox != null);
		}

		private static void CorrectBlockInsideInline(CssBox box)
		{
			try
			{
				if (DomUtils.ContainsInlinesOnly(box) && !ContainsInlinesOnlyDeep(box))
				{
					CssBox cssBox = CorrectBlockInsideInlineImp(box);
					while (cssBox != null)
					{
						CssBox cssBox2 = null;
						if (DomUtils.ContainsInlinesOnly(cssBox) && !ContainsInlinesOnlyDeep(cssBox))
							cssBox2 = CorrectBlockInsideInlineImp(cssBox);
						cssBox.ParentBox.SetAllBoxes(cssBox);
						cssBox.ParentBox = null;
						cssBox = cssBox2;
					}
				}
				if (DomUtils.ContainsInlinesOnly(box))
					return;
				foreach (CssBox box2 in box.Boxes)
				{
					CorrectBlockInsideInline(box2);
				}
			}
			catch (Exception exception)
			{
				box.HtmlContainer.ReportError(HtmlRenderErrorType.HtmlParsing, "Failed in block inside inline box correction", exception);
			}
		}

		private static CssBox CorrectBlockInsideInlineImp(CssBox box)
		{
			if (box.Display == "inline")
				box.Display = "block";
			if (box.Boxes.Count > 1 || box.Boxes[0].Boxes.Count > 1)
			{
				CssBox cssBox = CssBox.CreateBlock(box);
				while (ContainsInlinesOnlyDeep(box.Boxes[0]))
				{
					box.Boxes[0].ParentBox = cssBox;
				}
				cssBox.SetBeforeBox(box.Boxes[0]);
				CssBox cssBox2 = box.Boxes[1];
				cssBox2.ParentBox = null;
				CorrectBlockSplitBadBox(box, cssBox2, cssBox);
				if (cssBox.Boxes.Count < 1)
					cssBox.ParentBox = null;
				int num = ((cssBox.ParentBox == null) ? 1 : 2);
				if (box.Boxes.Count > num)
				{
					CssBox cssBox3 = CssBox.CreateBox(box, null, box.Boxes[num]);
					while (box.Boxes.Count > num + 1)
					{
						box.Boxes[num + 1].ParentBox = cssBox3;
					}
					return cssBox3;
				}
			}
			else if (box.Boxes[0].Display == "inline")
			{
				box.Boxes[0].Display = "block";
			}
			return null;
		}

		private static void CorrectBlockSplitBadBox(CssBox parentBox, CssBox badBox, CssBox leftBlock)
		{
			CssBox cssBox = null;
			while (badBox.Boxes[0].IsInline && ContainsInlinesOnlyDeep(badBox.Boxes[0]))
			{
				if (cssBox == null)
				{
					cssBox = CssBox.CreateBox(leftBlock, badBox.HtmlTag);
					cssBox.InheritStyle(badBox, true);
				}
				badBox.Boxes[0].ParentBox = cssBox;
			}
			CssBox cssBox2 = badBox.Boxes[0];
			if (!ContainsInlinesOnlyDeep(cssBox2))
			{
				CorrectBlockSplitBadBox(parentBox, cssBox2, leftBlock);
				cssBox2.ParentBox = null;
			}
			else
				cssBox2.ParentBox = parentBox;
			if (badBox.Boxes.Count > 0)
			{
				CssBox cssBox3;
				if (cssBox2.ParentBox == null && parentBox.Boxes.Count >= 3)
					cssBox3 = parentBox.Boxes[2];
				else
				{
					cssBox3 = CssBox.CreateBox(parentBox, badBox.HtmlTag);
					cssBox3.InheritStyle(badBox, true);
					if (parentBox.Boxes.Count > 2)
						cssBox3.SetBeforeBox(parentBox.Boxes[1]);
					if (cssBox2.ParentBox != null)
						cssBox2.SetBeforeBox(cssBox3);
				}
				cssBox3.SetAllBoxes(badBox);
			}
			else if (cssBox2.ParentBox != null && parentBox.Boxes.Count > 1)
			{
				cssBox2.SetBeforeBox(parentBox.Boxes[1]);
				if (cssBox2.HtmlTag != null && cssBox2.HtmlTag.Name == "br" && (cssBox != null || leftBlock.Boxes.Count > 1))
					cssBox2.Display = "inline";
			}
		}

		private static void CorrectInlineBoxesParent(CssBox box)
		{
			if (ContainsVariantBoxes(box))
			{
				for (int i = 0; i < box.Boxes.Count; i++)
				{
					if (box.Boxes[i].IsInline)
					{
						CssBox parentBox = CssBox.CreateBlock(box, null, box.Boxes[i++]);
						while (i < box.Boxes.Count && box.Boxes[i].IsInline)
						{
							box.Boxes[i].ParentBox = parentBox;
						}
					}
				}
			}
			if (DomUtils.ContainsInlinesOnly(box))
				return;
			foreach (CssBox box2 in box.Boxes)
			{
				CorrectInlineBoxesParent(box2);
			}
		}

		private static bool ContainsInlinesOnlyDeep(CssBox box)
		{
			foreach (CssBox box2 in box.Boxes)
			{
				if (!box2.IsInline || !ContainsInlinesOnlyDeep(box2))
					return false;
			}
			return true;
		}

		private static bool ContainsVariantBoxes(CssBox box)
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < box.Boxes.Count; i++)
			{
				if (flag && flag2)
					break;
				bool flag3 = !box.Boxes[i].IsInline;
				flag = flag || flag3;
				flag2 = flag2 || !flag3;
			}
			return flag && flag2;
		}
	}
}
