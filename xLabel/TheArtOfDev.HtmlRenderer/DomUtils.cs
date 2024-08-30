using System;
using System.Collections.Generic;
using System.Text;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Parse;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal sealed class DomUtils
	{
		public static bool IsInBox(CssBox box, RPoint location)
		{
			foreach (KeyValuePair<CssLineBox, RRect> rectangle in box.Rectangles)
			{
				if (rectangle.Value.Contains(location))
					return true;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				if (IsInBox(box2, location))
					return true;
			}
			return false;
		}

		public static bool ContainsInlinesOnly(CssBox box)
		{
			foreach (CssBox box2 in box.Boxes)
			{
				if (!box2.IsInline)
					return false;
			}
			return true;
		}

		public static CssBox FindParent(CssBox root, string tagName, CssBox box)
		{
			if (box == null)
				return root;
			if (box.HtmlTag != null && box.HtmlTag.Name.Equals(tagName, StringComparison.CurrentCultureIgnoreCase))
				return box.ParentBox ?? root;
			return FindParent(root, tagName, box.ParentBox);
		}

		public static CssBox GetPreviousSibling(CssBox b)
		{
			if (b.ParentBox != null)
			{
				int num = b.ParentBox.Boxes.IndexOf(b);
				if (num > 0)
				{
					int num2 = 1;
					CssBox cssBox = b.ParentBox.Boxes[num - 1];
					while ((cssBox.Display == "none" || cssBox.Position == "absolute" || cssBox.Position == "fixed") && num - num2 - 1 >= 0)
					{
						cssBox = b.ParentBox.Boxes[num - ++num2];
					}
					return (cssBox.Display == "none" || cssBox.Position == "fixed") ? null : cssBox;
				}
			}
			return null;
		}

		public static CssBox GetPreviousContainingBlockSibling(CssBox b)
		{
			CssBox cssBox = b;
			int num = cssBox.ParentBox.Boxes.IndexOf(cssBox);
			while (cssBox.ParentBox != null && num < 1 && cssBox.Display != "block" && cssBox.Display != "table" && cssBox.Display != "table-cell" && cssBox.Display != "list-item")
			{
				cssBox = cssBox.ParentBox;
				num = ((cssBox.ParentBox != null) ? cssBox.ParentBox.Boxes.IndexOf(cssBox) : (-1));
			}
			cssBox = cssBox.ParentBox;
			if (cssBox != null && num > 0)
			{
				int num2 = 1;
				CssBox cssBox2 = cssBox.Boxes[num - 1];
				while ((cssBox2.Display == "none" || cssBox2.Position == "absolute" || cssBox2.Position == "fixed") && num - num2 - 1 >= 0)
				{
					cssBox2 = cssBox.Boxes[num - ++num2];
				}
				return (cssBox2.Display == "none") ? null : cssBox2;
			}
			return null;
		}

		public static bool IsBoxHasWhitespace(CssBox box)
		{
			if (!box.Words[0].IsImage && box.Words[0].HasSpaceBefore && box.IsInline)
			{
				CssBox previousContainingBlockSibling = GetPreviousContainingBlockSibling(box);
				if (previousContainingBlockSibling != null && previousContainingBlockSibling.IsInline)
					return true;
			}
			return false;
		}

		public static CssBox GetNextSibling(CssBox b)
		{
			CssBox result = null;
			if (b.ParentBox != null)
			{
				for (int i = b.ParentBox.Boxes.IndexOf(b) + 1; i <= b.ParentBox.Boxes.Count - 1; i++)
				{
					CssBox cssBox = b.ParentBox.Boxes[i];
					if (cssBox.Display != "none" && cssBox.Position != "absolute" && cssBox.Position != "fixed")
					{
						result = cssBox;
						break;
					}
				}
			}
			return result;
		}

		public static string GetAttribute(CssBox box, string attribute)
		{
			string text = null;
			while (box != null && text == null)
			{
				text = box.GetAttribute(attribute, null);
				box = box.ParentBox;
			}
			return text;
		}

		public static CssBox GetCssBox(CssBox box, RPoint location, bool visible = true)
		{
			if (box != null && (!visible || box.Visibility == "visible") && (box.Bounds.IsEmpty || box.Bounds.Contains(location)))
			{
				foreach (CssBox box2 in box.Boxes)
				{
					if (CommonUtils.GetFirstValueOrDefault(box.Rectangles, box.Bounds).Contains(location))
						return GetCssBox(box2, location) ?? box2;
				}
			}
			return null;
		}

		public static void GetAllLinkBoxes(CssBox box, List<CssBox> linkBoxes)
		{
			if (box == null)
				return;
			if (box.IsClickable && box.Visibility == "visible")
				linkBoxes.Add(box);
			foreach (CssBox box2 in box.Boxes)
			{
				GetAllLinkBoxes(box2, linkBoxes);
			}
		}

		public static CssBox GetLinkBox(CssBox box, RPoint location)
		{
			if (box != null)
			{
				if (box.IsClickable && box.Visibility == "visible" && IsInBox(box, location))
					return box;
				if (box.ClientRectangle.IsEmpty || box.ClientRectangle.Contains(location))
				{
					foreach (CssBox box2 in box.Boxes)
					{
						CssBox linkBox = GetLinkBox(box2, location);
						if (linkBox != null)
							return linkBox;
					}
				}
			}
			return null;
		}

		public static CssBox GetBoxById(CssBox box, string id)
		{
			if (box != null && !string.IsNullOrEmpty(id))
			{
				if (box.HtmlTag != null && id.Equals(box.HtmlTag.TryGetAttribute("id"), StringComparison.OrdinalIgnoreCase))
					return box;
				foreach (CssBox box2 in box.Boxes)
				{
					CssBox boxById = GetBoxById(box2, id);
					if (boxById != null)
						return boxById;
				}
			}
			return null;
		}

		public static CssLineBox GetCssLineBox(CssBox box, RPoint location)
		{
			CssLineBox cssLineBox = null;
			if (box != null)
			{
				if (box.LineBoxes.Count > 0 && (box.HtmlTag == null || box.HtmlTag.Name != "td" || box.Bounds.Contains(location)))
				{
					foreach (CssLineBox lineBox in box.LineBoxes)
					{
						foreach (KeyValuePair<CssBox, RRect> rectangle in lineBox.Rectangles)
						{
							if (rectangle.Value.Top <= location.Y)
								cssLineBox = lineBox;
							if (rectangle.Value.Top > location.Y)
								return cssLineBox;
						}
					}
				}
				foreach (CssBox box2 in box.Boxes)
				{
					cssLineBox = GetCssLineBox(box2, location) ?? cssLineBox;
				}
			}
			return cssLineBox;
		}

		public static CssRect GetCssBoxWord(CssBox box, RPoint location)
		{
			if (box != null && box.Visibility == "visible")
			{
				if (box.LineBoxes.Count > 0)
				{
					foreach (CssLineBox lineBox in box.LineBoxes)
					{
						CssRect cssBoxWord = GetCssBoxWord(lineBox, location);
						if (cssBoxWord != null)
							return cssBoxWord;
					}
				}
				if (box.ClientRectangle.IsEmpty || box.ClientRectangle.Contains(location))
				{
					foreach (CssBox box2 in box.Boxes)
					{
						CssRect cssBoxWord2 = GetCssBoxWord(box2, location);
						if (cssBoxWord2 != null)
							return cssBoxWord2;
					}
				}
			}
			return null;
		}

		public static CssRect GetCssBoxWord(CssLineBox lineBox, RPoint location)
		{
			foreach (KeyValuePair<CssBox, RRect> rectangle2 in lineBox.Rectangles)
			{
				foreach (CssRect word in rectangle2.Key.Words)
				{
					RRect rectangle = word.Rectangle;
					rectangle.Width += word.OwnerBox.ActualWordSpacing;
					if (rectangle.Contains(location))
						return word;
				}
			}
			return null;
		}

		public static CssLineBox GetCssLineBoxByWord(CssRect word)
		{
			CssBox cssBox = word.OwnerBox;
			while (cssBox.LineBoxes.Count == 0)
			{
				cssBox = cssBox.ParentBox;
			}
			foreach (CssLineBox lineBox in cssBox.LineBoxes)
			{
				foreach (CssRect word2 in lineBox.Words)
				{
					if (word2 == word)
						return lineBox;
				}
			}
			return cssBox.LineBoxes[0];
		}

		public static string GetSelectedPlainText(CssBox root)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int selectedPlainText = GetSelectedPlainText(stringBuilder, root);
			return stringBuilder.ToString(0, selectedPlainText).Trim();
		}

		public static string GenerateHtml(CssBox root, HtmlGenerationStyle styleGen = HtmlGenerationStyle.Inline, bool onlySelected = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (root != null)
			{
				Dictionary<CssBox, bool> selectedBoxes = (onlySelected ? CollectSelectedBoxes(root) : null);
				CssBox selectionRoot = (onlySelected ? GetSelectionRoot(root, selectedBoxes) : null);
				WriteHtml(root.HtmlContainer.CssParser, stringBuilder, root, styleGen, selectedBoxes, selectionRoot);
			}
			return stringBuilder.ToString();
		}

		public static string GenerateBoxTree(CssBox root)
		{
			StringBuilder stringBuilder = new StringBuilder();
			GenerateBoxTree(root, stringBuilder, 0);
			return stringBuilder.ToString();
		}

		private static int GetSelectedPlainText(StringBuilder sb, CssBox box)
		{
			int num = 0;
			foreach (CssRect word in box.Words)
			{
				if (word.Selected)
				{
					sb.Append(GetSelectedWord(word, true));
					num = sb.Length;
				}
			}
			if (box.Boxes.Count < 1 && box.Text != null && box.Text.IsWhitespace())
				sb.Append(' ');
			if (box.Visibility != "hidden" && box.Display != "none")
			{
				foreach (CssBox box2 in box.Boxes)
				{
					int selectedPlainText = GetSelectedPlainText(sb, box2);
					num = Math.Max(num, selectedPlainText);
				}
			}
			if (sb.Length > 0)
			{
				if (box.HtmlTag != null && box.HtmlTag.Name == "hr")
				{
					if (sb.Length > 1 && sb[sb.Length - 1] != '\n')
						sb.AppendLine();
					sb.AppendLine(new string('-', 80));
				}
				if ((box.Display == "block" || box.Display == "list-item" || box.Display == "table-row") && (!box.IsBrElement || sb.Length <= 1 || sb[sb.Length - 1] != '\n'))
					sb.AppendLine();
				if (box.Display == "table-cell")
					sb.Append(' ');
				if (box.HtmlTag != null && box.HtmlTag.Name == "p")
				{
					int num2 = 0;
					int num3 = sb.Length - 1;
					while (num3 >= 0 && char.IsWhiteSpace(sb[num3]))
					{
						num2 += ((sb[num3] == '\n') ? 1 : 0);
						num3--;
					}
					if (num2 < 2)
						sb.AppendLine();
				}
			}
			return num;
		}

		private static Dictionary<CssBox, bool> CollectSelectedBoxes(CssBox root)
		{
			Dictionary<CssBox, bool> dictionary = new Dictionary<CssBox, bool>();
			Dictionary<CssBox, bool> maybeBoxes = new Dictionary<CssBox, bool>();
			CollectSelectedBoxes(root, dictionary, maybeBoxes);
			return dictionary;
		}

		private static bool CollectSelectedBoxes(CssBox box, Dictionary<CssBox, bool> selectedBoxes, Dictionary<CssBox, bool> maybeBoxes)
		{
			bool result = false;
			foreach (CssRect word in box.Words)
			{
				if (!word.Selected)
					continue;
				selectedBoxes[box] = true;
				foreach (KeyValuePair<CssBox, bool> maybeBox in maybeBoxes)
				{
					selectedBoxes[maybeBox.Key] = maybeBox.Value;
				}
				maybeBoxes.Clear();
				result = true;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				if (CollectSelectedBoxes(box2, selectedBoxes, maybeBoxes))
				{
					selectedBoxes[box] = true;
					result = true;
				}
			}
			if (box.HtmlTag != null && selectedBoxes.Count > 0)
				maybeBoxes[box] = true;
			return result;
		}

		private static CssBox GetSelectionRoot(CssBox root, Dictionary<CssBox, bool> selectedBoxes)
		{
			CssBox cssBox = root;
			CssBox cssBox2 = root;
			while (true)
			{
				bool flag = false;
				CssBox cssBox3 = null;
				foreach (CssBox box in cssBox2.Boxes)
				{
					if (selectedBoxes.ContainsKey(box))
					{
						if (cssBox3 != null)
						{
							flag = true;
							break;
						}
						cssBox3 = box;
					}
				}
				if (flag || cssBox3 == null)
					break;
				cssBox2 = cssBox3;
				if (cssBox2.HtmlTag != null)
					cssBox = cssBox2;
			}
			if (!ContainsNamedBox(cssBox))
			{
				cssBox2 = cssBox.ParentBox;
				while (cssBox2.ParentBox != null && cssBox2.HtmlTag == null)
				{
					cssBox2 = cssBox2.ParentBox;
				}
				if (cssBox2.HtmlTag != null)
					cssBox = cssBox2;
			}
			return cssBox;
		}

		private static bool ContainsNamedBox(CssBox box)
		{
			foreach (CssBox box2 in box.Boxes)
			{
				if (box2.HtmlTag != null || ContainsNamedBox(box2))
					return true;
			}
			return false;
		}

		private static void WriteHtml(CssParser cssParser, StringBuilder sb, CssBox box, HtmlGenerationStyle styleGen, Dictionary<CssBox, bool> selectedBoxes, CssBox selectionRoot)
		{
			if (box.HtmlTag != null && selectedBoxes != null && !selectedBoxes.ContainsKey(box))
				return;
			if (box.HtmlTag != null)
			{
				if (box.HtmlTag.Name != "link" || !box.HtmlTag.Attributes.ContainsKey("href") || (!box.HtmlTag.Attributes["href"].StartsWith("property") && !box.HtmlTag.Attributes["href"].StartsWith("method")))
				{
					WriteHtmlTag(cssParser, sb, box, styleGen);
					if (box == selectionRoot)
						sb.Append("<!--StartFragment-->");
				}
				if (styleGen == HtmlGenerationStyle.InHeader && box.HtmlTag.Name == "html" && box.HtmlContainer.CssData != null)
				{
					sb.AppendLine("<head>");
					WriteStylesheet(sb, box.HtmlContainer.CssData);
					sb.AppendLine("</head>");
				}
			}
			if (box.Words.Count > 0)
			{
				foreach (CssRect word in box.Words)
				{
					if (selectedBoxes == null || word.Selected)
					{
						string selectedWord = GetSelectedWord(word, selectedBoxes != null);
						sb.Append(HtmlUtils.EncodeHtml(selectedWord));
					}
				}
			}
			foreach (CssBox box2 in box.Boxes)
			{
				WriteHtml(cssParser, sb, box2, styleGen, selectedBoxes, selectionRoot);
			}
			if (box.HtmlTag != null && !box.HtmlTag.IsSingle)
			{
				if (box == selectionRoot)
					sb.Append("<!--EndFragment-->");
				sb.AppendFormat("</{0}>", box.HtmlTag.Name);
			}
		}

		private static void WriteHtmlTag(CssParser cssParser, StringBuilder sb, CssBox box, HtmlGenerationStyle styleGen)
		{
			sb.AppendFormat("<{0}", box.HtmlTag.Name);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerable<CssBlock> cssBlock = box.HtmlContainer.CssData.GetCssBlock(box.HtmlTag.Name);
			if (cssBlock != null)
			{
				foreach (CssBlock item in cssBlock)
				{
					foreach (KeyValuePair<string, string> property in item.Properties)
					{
						dictionary[property.Key] = property.Value;
					}
				}
			}
			if (box.HtmlTag.HasAttributes())
			{
				sb.Append(" ");
				foreach (KeyValuePair<string, string> attribute in box.HtmlTag.Attributes)
				{
					if (styleGen == HtmlGenerationStyle.Inline && attribute.Key == "style")
					{
						CssBlock cssBlock2 = cssParser.ParseCssBlock(box.HtmlTag.Name, box.HtmlTag.TryGetAttribute("style"));
						foreach (KeyValuePair<string, string> property2 in cssBlock2.Properties)
						{
							dictionary[property2.Key] = property2.Value;
						}
					}
					else if (styleGen == HtmlGenerationStyle.Inline && attribute.Key == "class")
					{
						IEnumerable<CssBlock> cssBlock3 = box.HtmlContainer.CssData.GetCssBlock("." + attribute.Value);
						if (cssBlock3 == null)
							continue;
						foreach (CssBlock item2 in cssBlock3)
						{
							foreach (KeyValuePair<string, string> property3 in item2.Properties)
							{
								dictionary[property3.Key] = property3.Value;
							}
						}
					}
					else
					{
						sb.AppendFormat("{0}=\"{1}\" ", attribute.Key, attribute.Value);
					}
				}
				sb.Remove(sb.Length - 1, 1);
			}
			if (styleGen == HtmlGenerationStyle.Inline && dictionary.Count > 0)
			{
				Dictionary<string, string> dictionary2 = StripDefaultStyles(box, dictionary);
				if (dictionary2.Count > 0)
				{
					sb.Append(" style=\"");
					foreach (KeyValuePair<string, string> item3 in dictionary2)
					{
						sb.AppendFormat("{0}: {1}; ", item3.Key, item3.Value);
					}
					sb.Remove(sb.Length - 1, 1);
					sb.Append("\"");
				}
			}
			sb.AppendFormat("{0}>", box.HtmlTag.IsSingle ? "/" : "");
		}

		private static Dictionary<string, string> StripDefaultStyles(CssBox box, Dictionary<string, string> tagStyles)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerable<CssBlock> cssBlock = box.HtmlContainer.Adapter.DefaultCssData.GetCssBlock(box.HtmlTag.Name);
			foreach (KeyValuePair<string, string> tagStyle in tagStyles)
			{
				bool flag = false;
				foreach (CssBlock item in cssBlock)
				{
					string value;
					if (item.Properties.TryGetValue(tagStyle.Key, out value) && value.Equals(tagStyle.Value, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
					dictionary[tagStyle.Key] = tagStyle.Value;
			}
			return dictionary;
		}

		private static void WriteStylesheet(StringBuilder sb, CssData cssData)
		{
			sb.AppendLine("<style type=\"text/css\">");
			foreach (KeyValuePair<string, List<CssBlock>> item in cssData.MediaBlocks["all"])
			{
				sb.Append(item.Key);
				sb.Append(" { ");
				foreach (CssBlock item2 in item.Value)
				{
					foreach (KeyValuePair<string, string> property in item2.Properties)
					{
						sb.AppendFormat("{0}: {1};", property.Key, property.Value);
					}
				}
				sb.Append(" }");
				sb.AppendLine();
			}
			sb.AppendLine("</style>");
		}

		private static string GetSelectedWord(CssRect rect, bool selectedText)
		{
			if (selectedText && rect.SelectedStartIndex > -1 && rect.SelectedEndIndexOffset > -1)
				return rect.Text.Substring(rect.SelectedStartIndex, rect.SelectedEndIndexOffset - rect.SelectedStartIndex);
			if (selectedText && rect.SelectedStartIndex > -1)
				return rect.Text.Substring(rect.SelectedStartIndex) + (rect.HasSpaceAfter ? " " : "");
			if (selectedText && rect.SelectedEndIndexOffset > -1)
				return rect.Text.Substring(0, rect.SelectedEndIndexOffset);
			return (((rect.OwnerBox.Words[0] == rect) ? IsBoxHasWhitespace(rect.OwnerBox) : rect.HasSpaceBefore) ? " " : "") + rect.Text + (rect.HasSpaceAfter ? " " : "");
		}

		private static void GenerateBoxTree(CssBox box, StringBuilder builder, int indent)
		{
			builder.AppendFormat("{0}<{1}", new string(' ', 2 * indent), box.Display);
			if (box.HtmlTag != null)
				builder.AppendFormat(" element=\"{0}\"", (box.HtmlTag != null) ? box.HtmlTag.Name : string.Empty);
			if (box.Words.Count > 0)
				builder.AppendFormat(" words=\"{0}\"", box.Words.Count);
			builder.AppendFormat("{0}>\r\n", (box.Boxes.Count > 0) ? "" : "/");
			if (box.Boxes.Count <= 0)
				return;
			foreach (CssBox box2 in box.Boxes)
			{
				GenerateBoxTree(box2, builder, indent + 1);
			}
			builder.AppendFormat("{0}</{1}>\r\n", new string(' ', 2 * indent), box.Display);
		}
	}
}
