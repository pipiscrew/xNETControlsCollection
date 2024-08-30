using System;
using System.Collections.Generic;
using System.Globalization;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal class CssBox : CssBoxProperties, IDisposable
	{
		private CssBox _parentBox;

		protected HtmlContainerInt _htmlContainer;

		private readonly HtmlTag _htmltag;

		private readonly List<CssRect> _boxWords = new List<CssRect>();

		private readonly List<CssBox> _boxes = new List<CssBox>();

		private readonly List<CssLineBox> _lineBoxes = new List<CssLineBox>();

		private readonly List<CssLineBox> _parentLineBoxes = new List<CssLineBox>();

		private readonly Dictionary<CssLineBox, RRect> _rectangles = new Dictionary<CssLineBox, RRect>();

		private SubString _text;

		internal bool _tableFixed;

		protected bool _wordsSizeMeasured;

		private CssBox _listItemBox;

		private CssLineBox _firstHostingLineBox;

		private CssLineBox _lastHostingLineBox;

		private ImageLoadHandler _imageLoadHandler;

		public HtmlContainerInt HtmlContainer
		{
			get
			{
				return _htmlContainer ?? (_htmlContainer = ((_parentBox != null) ? _parentBox.HtmlContainer : null));
			}
			set
			{
				_htmlContainer = value;
			}
		}

		public CssBox ParentBox
		{
			get
			{
				return _parentBox;
			}
			set
			{
				if (_parentBox != null)
					_parentBox.Boxes.Remove(this);
				_parentBox = value;
				if (value != null)
					_parentBox.Boxes.Add(this);
			}
		}

		public List<CssBox> Boxes
		{
			get
			{
				return _boxes;
			}
		}

		public bool IsBrElement
		{
			get
			{
				return _htmltag != null && _htmltag.Name.Equals("br", StringComparison.InvariantCultureIgnoreCase);
			}
		}

		public bool IsInline
		{
			get
			{
				return (base.Display == "inline" || base.Display == "inline-block") && !IsBrElement;
			}
		}

		public bool IsBlock
		{
			get
			{
				return base.Display == "block";
			}
		}

		public virtual bool IsClickable
		{
			get
			{
				return HtmlTag != null && HtmlTag.Name == "a" && !HtmlTag.HasAttribute("id");
			}
		}

		public virtual bool IsFixed
		{
			get
			{
				if (!(base.Position == "fixed"))
				{
					if (ParentBox != null)
					{
						CssBox cssBox = this;
						while (cssBox.ParentBox != null && cssBox != cssBox.ParentBox)
						{
							cssBox = cssBox.ParentBox;
							if (!(cssBox.Position == "fixed"))
								continue;
							return true;
						}
						return false;
					}
					return false;
				}
				return true;
			}
		}

		public virtual string HrefLink
		{
			get
			{
				return GetAttribute("href");
			}
		}

		public CssBox ContainingBlock
		{
			get
			{
				if (ParentBox == null)
					return this;
				CssBox parentBox = ParentBox;
				while (!parentBox.IsBlock && parentBox.Display != "list-item" && parentBox.Display != "table" && parentBox.Display != "table-cell" && parentBox.ParentBox != null)
				{
					parentBox = parentBox.ParentBox;
				}
				if (parentBox == null)
					throw new Exception("There's no containing block on the chain");
				return parentBox;
			}
		}

		public HtmlTag HtmlTag
		{
			get
			{
				return _htmltag;
			}
		}

		public bool IsImage
		{
			get
			{
				return Words.Count == 1 && Words[0].IsImage;
			}
		}

		public bool IsSpaceOrEmpty
		{
			get
			{
				if ((Words.Count != 0 || Boxes.Count != 0) && (Words.Count != 1 || !Words[0].IsSpaces))
				{
					foreach (CssRect word in Words)
					{
						if (!word.IsSpaces)
							return false;
					}
				}
				return true;
			}
		}

		public SubString Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
				_boxWords.Clear();
			}
		}

		internal List<CssLineBox> LineBoxes
		{
			get
			{
				return _lineBoxes;
			}
		}

		internal List<CssLineBox> ParentLineBoxes
		{
			get
			{
				return _parentLineBoxes;
			}
		}

		internal Dictionary<CssLineBox, RRect> Rectangles
		{
			get
			{
				return _rectangles;
			}
		}

		internal List<CssRect> Words
		{
			get
			{
				return _boxWords;
			}
		}

		internal CssRect FirstWord
		{
			get
			{
				return Words[0];
			}
		}

		internal CssLineBox FirstHostingLineBox
		{
			get
			{
				return _firstHostingLineBox;
			}
			set
			{
				_firstHostingLineBox = value;
			}
		}

		internal CssLineBox LastHostingLineBox
		{
			get
			{
				return _lastHostingLineBox;
			}
			set
			{
				_lastHostingLineBox = value;
			}
		}

		public CssBox(CssBox parentBox, HtmlTag tag)
		{
			if (parentBox != null)
			{
				_parentBox = parentBox;
				_parentBox.Boxes.Add(this);
			}
			_htmltag = tag;
		}

		public static CssBox CreateBox(HtmlTag tag, CssBox parent = null)
		{
			ArgChecker.AssertArgNotNull(tag, "tag");
			if (tag.Name == "img")
				return new CssBoxImage(parent, tag);
			if (tag.Name == "iframe")
				return new CssBoxFrame(parent, tag);
			if (tag.Name == "hr")
				return new CssBoxHr(parent, tag);
			return new CssBox(parent, tag);
		}

		public static CssBox CreateBox(CssBox parent, HtmlTag tag = null, CssBox before = null)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			CssBox cssBox = new CssBox(parent, tag);
			cssBox.InheritStyle();
			if (before != null)
				cssBox.SetBeforeBox(before);
			return cssBox;
		}

		public static CssBox CreateBlock()
		{
			CssBox cssBox = new CssBox(null, null);
			cssBox.Display = "block";
			return cssBox;
		}

		public static CssBox CreateBlock(CssBox parent, HtmlTag tag = null, CssBox before = null)
		{
			ArgChecker.AssertArgNotNull(parent, "parent");
			CssBox cssBox = CreateBox(parent, tag, before);
			cssBox.Display = "block";
			return cssBox;
		}

		public void PerformLayout(RGraphics g)
		{
			try
			{
				PerformLayoutImp(g);
			}
			catch (Exception exception)
			{
				HtmlContainer.ReportError(HtmlRenderErrorType.Layout, "Exception in box layout", exception);
			}
		}

		public void Paint(RGraphics g)
		{
			try
			{
				if (!(base.Display != "none") || !(base.Visibility == "visible"))
					return;
				if (base.Position == "fixed")
					g.SuspendClipping();
				bool flag;
				if (!(flag = Rectangles.Count == 0))
				{
					RRect clip = g.GetClip();
					RRect clientRectangle = ContainingBlock.ClientRectangle;
					clientRectangle.X -= 2.0;
					clientRectangle.Width += 2.0;
					if (!IsFixed)
						clientRectangle.Offset(HtmlContainer.ScrollOffset);
					clip.Intersect(clientRectangle);
					if (clip != RRect.Empty)
						flag = true;
				}
				if (flag)
					PaintImp(g);
				if (base.Position == "fixed")
					g.ResumeClipping();
			}
			catch (Exception exception)
			{
				HtmlContainer.ReportError(HtmlRenderErrorType.Paint, "Exception in box paint", exception);
			}
		}

		public void SetBeforeBox(CssBox before)
		{
			int num = _parentBox.Boxes.IndexOf(before);
			if (num < 0)
				throw new Exception("before box doesn't exist on parent");
			_parentBox.Boxes.Remove(this);
			_parentBox.Boxes.Insert(num, this);
		}

		public void SetAllBoxes(CssBox fromBox)
		{
			foreach (CssBox box in fromBox._boxes)
			{
				box._parentBox = this;
			}
			_boxes.AddRange(fromBox._boxes);
			fromBox._boxes.Clear();
		}

		public void ParseToWords()
		{
			_boxWords.Clear();
			int i = 0;
			bool flag;
			bool flag2 = (flag = base.WhiteSpace == "pre" || base.WhiteSpace == "pre-wrap") || base.WhiteSpace == "pre-line";
			while (i < _text.Length)
			{
				for (; i < _text.Length && _text[i] == '\r'; i++)
				{
				}
				if (i >= _text.Length)
					continue;
				int j;
				for (j = i; j < _text.Length && char.IsWhiteSpace(_text[j]) && _text[j] != '\n'; j++)
				{
				}
				if (j > i)
				{
					if (flag)
						_boxWords.Add(new CssRectWord(this, HtmlUtils.DecodeHtml(_text.Substring(i, j - i)), false, false));
				}
				else
				{
					for (j = i; j < _text.Length && !char.IsWhiteSpace(_text[j]) && _text[j] != '-'; j++)
					{
						if (!(base.WordBreak != "break-all"))
							break;
						if (CommonUtils.IsAsianCharecter(_text[j]))
							break;
					}
					if (j < _text.Length && (_text[j] == '-' || base.WordBreak == "break-all" || CommonUtils.IsAsianCharecter(_text[j])))
						j++;
					if (j > i)
					{
						bool hasSpaceBefore = !flag && i > 0 && _boxWords.Count == 0 && char.IsWhiteSpace(_text[i - 1]);
						bool hasSpaceAfter = !flag && j < _text.Length && char.IsWhiteSpace(_text[j]);
						_boxWords.Add(new CssRectWord(this, HtmlUtils.DecodeHtml(_text.Substring(i, j - i)), hasSpaceBefore, hasSpaceAfter));
					}
				}
				if (j < _text.Length && _text[j] == '\n')
				{
					j++;
					if (flag2)
						_boxWords.Add(new CssRectWord(this, "\n", false, false));
				}
				i = j;
			}
		}

		public virtual void Dispose()
		{
			if (_imageLoadHandler != null)
				_imageLoadHandler.Dispose();
			foreach (CssBox box in Boxes)
			{
				box.Dispose();
			}
		}

		protected virtual void PerformLayoutImp(RGraphics g)
		{
			if (base.Display != "none")
			{
				RectanglesReset();
				MeasureWordsSize(g);
			}
			if (IsBlock || base.Display == "list-item" || base.Display == "table" || base.Display == "inline-table" || base.Display == "table-cell")
			{
				if (base.Display != "table-cell" && base.Display != "table")
				{
					double num = ContainingBlock.Size.Width - ContainingBlock.ActualPaddingLeft - ContainingBlock.ActualPaddingRight - ContainingBlock.ActualBorderLeftWidth - ContainingBlock.ActualBorderRightWidth;
					if (base.Width != "auto" && !string.IsNullOrEmpty(base.Width))
						num = CssValueParser.ParseLength(base.Width, num, this);
					base.Size = new RSize(num, base.Size.Height);
					base.Size = new RSize(num - base.ActualMarginLeft - base.ActualMarginRight, base.Size.Height);
				}
				if (base.Display != "table-cell")
				{
					CssBox previousSibling = DomUtils.GetPreviousSibling(this);
					if (base.Position == "fixed")
					{
						double num2 = 0.0;
						double num3 = 0.0;
					}
					else
					{
						double num2 = ContainingBlock.Location.X + ContainingBlock.ActualPaddingLeft + base.ActualMarginLeft + ContainingBlock.ActualBorderLeftWidth;
						double num3 = ((previousSibling == null && ParentBox != null) ? ParentBox.ClientTop : ((ParentBox == null) ? base.Location.Y : 0.0)) + MarginTopCollapse(previousSibling) + ((previousSibling != null) ? (previousSibling.ActualBottom + previousSibling.ActualBorderBottomWidth) : 0.0);
						base.Location = new RPoint(num2, num3);
						base.ActualBottom = num3;
					}
				}
				if (base.Display == "table" || base.Display == "inline-table")
					CssLayoutEngineTable.PerformLayout(g, this);
				else if (DomUtils.ContainsInlinesOnly(this))
				{
					base.ActualBottom = base.Location.Y;
					CssLayoutEngine.CreateLineBoxes(g, this);
				}
				else if (_boxes.Count > 0)
				{
					foreach (CssBox box in Boxes)
					{
						box.PerformLayout(g);
					}
					base.ActualRight = CalculateActualRight();
					base.ActualBottom = MarginBottomCollapse();
				}
			}
			else
			{
				CssBox previousSibling2 = DomUtils.GetPreviousSibling(this);
				if (previousSibling2 != null)
				{
					if (base.Location == RPoint.Empty)
						base.Location = previousSibling2.Location;
					base.ActualBottom = previousSibling2.ActualBottom;
				}
			}
			base.ActualBottom = Math.Max(base.ActualBottom, base.Location.Y + base.ActualHeight);
			CreateListItemBox(g);
			if (!IsFixed)
			{
				double width = Math.Max(GetMinimumWidth() + GetWidthMarginDeep(this), (base.Size.Width < 90999.0) ? (base.ActualRight - HtmlContainer.Root.Location.X) : 0.0);
				HtmlContainer.ActualSize = CommonUtils.Max(HtmlContainer.ActualSize, new RSize(width, base.ActualBottom - HtmlContainer.Root.Location.Y));
			}
		}

		internal virtual void MeasureWordsSize(RGraphics g)
		{
			if (_wordsSizeMeasured)
				return;
			if (base.BackgroundImage != "none" && _imageLoadHandler == null)
			{
				_imageLoadHandler = new ImageLoadHandler(HtmlContainer, OnImageLoadComplete);
				_imageLoadHandler.LoadImage(base.BackgroundImage, (HtmlTag != null) ? HtmlTag.Attributes : null);
			}
			MeasureWordSpacing(g);
			if (Words.Count > 0)
			{
				foreach (CssRect word in Words)
				{
					word.Width = ((word.Text != "\n") ? g.MeasureString(word.Text, base.ActualFont).Width : 0.0);
					word.Height = base.ActualFont.Height;
				}
			}
			_wordsSizeMeasured = true;
		}

		protected sealed override CssBoxProperties GetParent()
		{
			return _parentBox;
		}

		private int GetIndexForList()
		{
			bool flag = !string.IsNullOrEmpty(ParentBox.GetAttribute("reversed"));
			int result;
			if (!int.TryParse(ParentBox.GetAttribute("start"), out result))
			{
				if (flag)
				{
					result = 0;
					foreach (CssBox box in ParentBox.Boxes)
					{
						if (box.Display == "list-item")
							result++;
					}
				}
				else
					result = 1;
			}
			foreach (CssBox box2 in ParentBox.Boxes)
			{
				if (!box2.Equals(this))
				{
					if (box2.Display == "list-item")
						result += ((!flag) ? 1 : (-1));
					continue;
				}
				return result;
			}
			return result;
		}

		private void CreateListItemBox(RGraphics g)
		{
			if (!(base.Display == "list-item") || !(base.ListStyleType != "none"))
				return;
			if (_listItemBox == null)
			{
				_listItemBox = new CssBox(null, null);
				_listItemBox.InheritStyle(this);
				_listItemBox.Display = "inline";
				_listItemBox.HtmlContainer = HtmlContainer;
				if (!base.ListStyleType.Equals("disc", StringComparison.InvariantCultureIgnoreCase))
				{
					if (!base.ListStyleType.Equals("circle", StringComparison.InvariantCultureIgnoreCase))
					{
						if (base.ListStyleType.Equals("square", StringComparison.InvariantCultureIgnoreCase))
							_listItemBox.Text = new SubString("♠");
						else if (!base.ListStyleType.Equals("decimal", StringComparison.InvariantCultureIgnoreCase))
						{
							if (base.ListStyleType.Equals("decimal-leading-zero", StringComparison.InvariantCultureIgnoreCase))
								_listItemBox.Text = new SubString(GetIndexForList().ToString("00", CultureInfo.InvariantCulture) + ".");
							else
								_listItemBox.Text = new SubString(CommonUtils.ConvertToAlphaNumber(GetIndexForList(), base.ListStyleType) + ".");
						}
						else
						{
							_listItemBox.Text = new SubString(GetIndexForList().ToString(CultureInfo.InvariantCulture) + ".");
						}
					}
					else
						_listItemBox.Text = new SubString("o");
				}
				else
					_listItemBox.Text = new SubString("•");
				_listItemBox.ParseToWords();
				_listItemBox.PerformLayoutImp(g);
				_listItemBox.Size = new RSize(_listItemBox.Words[0].Width, _listItemBox.Words[0].Height);
			}
			_listItemBox.Words[0].Left = base.Location.X - _listItemBox.Size.Width - 5.0;
			_listItemBox.Words[0].Top = base.Location.Y + base.ActualPaddingTop;
		}

		internal CssRect FirstWordOccourence(CssBox b, CssLineBox line)
		{
			if (b.Words.Count == 0 && b.Boxes.Count == 0)
				return null;
			if (b.Words.Count > 0)
			{
				foreach (CssRect word in b.Words)
				{
					if (line.Words.Contains(word))
						return word;
				}
				return null;
			}
			foreach (CssBox box in b.Boxes)
			{
				CssRect cssRect = FirstWordOccourence(box, line);
				if (cssRect != null)
					return cssRect;
			}
			return null;
		}

		internal string GetAttribute(string attribute)
		{
			return GetAttribute(attribute, string.Empty);
		}

		internal string GetAttribute(string attribute, string defaultValue)
		{
			return (HtmlTag != null) ? HtmlTag.TryGetAttribute(attribute, defaultValue) : defaultValue;
		}

		internal double GetMinimumWidth()
		{
			double maxWidth = 0.0;
			CssRect maxWidthWord = null;
			GetMinimumWidth_LongestWord(this, ref maxWidth, ref maxWidthWord);
			double num = 0.0;
			if (maxWidthWord != null)
			{
				for (CssBox cssBox = maxWidthWord.OwnerBox; cssBox != null; cssBox = ((cssBox != this) ? cssBox.ParentBox : null))
				{
					num += cssBox.ActualBorderRightWidth + cssBox.ActualPaddingRight + cssBox.ActualBorderLeftWidth + cssBox.ActualPaddingLeft;
				}
			}
			return maxWidth + num;
		}

		private static void GetMinimumWidth_LongestWord(CssBox box, ref double maxWidth, ref CssRect maxWidthWord)
		{
			if (box.Words.Count > 0)
			{
				foreach (CssRect word in box.Words)
				{
					if (word.Width > maxWidth)
					{
						maxWidth = word.Width;
						maxWidthWord = word;
					}
				}
				return;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				GetMinimumWidth_LongestWord(box2, ref maxWidth, ref maxWidthWord);
			}
		}

		private static double GetWidthMarginDeep(CssBox box)
		{
			double num = 0.0;
			if (box.Size.Width > 90999.0 || (box.ParentBox != null && box.ParentBox.Size.Width > 90999.0))
			{
				while (box != null)
				{
					num += box.ActualMarginLeft + box.ActualMarginRight;
					box = box.ParentBox;
				}
			}
			return num;
		}

		internal double GetMaximumBottom(CssBox startBox, double currentMaxBottom)
		{
			foreach (CssLineBox key in startBox.Rectangles.Keys)
			{
				currentMaxBottom = Math.Max(currentMaxBottom, startBox.Rectangles[key].Bottom);
			}
			foreach (CssBox box in startBox.Boxes)
			{
				currentMaxBottom = Math.Max(currentMaxBottom, GetMaximumBottom(box, currentMaxBottom));
			}
			return currentMaxBottom;
		}

		internal void GetMinMaxWidth(out double minWidth, out double maxWidth)
		{
			double min = 0.0;
			double maxSum = 0.0;
			double paddingSum = 0.0;
			double marginSum = 0.0;
			GetMinMaxSumWords(this, ref min, ref maxSum, ref paddingSum, ref marginSum);
			maxWidth = paddingSum + maxSum;
			minWidth = paddingSum + ((min < 90999.0) ? min : 0.0);
		}

		private static void GetMinMaxSumWords(CssBox box, ref double min, ref double maxSum, ref double paddingSum, ref double marginSum)
		{
			double? num = null;
			if (box.Display != "inline" && box.Display != "table-cell" && box.WhiteSpace != "nowrap")
			{
				num = maxSum;
				maxSum = marginSum;
			}
			paddingSum += box.ActualBorderLeftWidth + box.ActualBorderRightWidth + box.ActualPaddingRight + box.ActualPaddingLeft;
			if (box.Display == "table")
				paddingSum += CssLayoutEngineTable.GetTableSpacing(box);
			if (box.Words.Count > 0)
			{
				foreach (CssRect word in box.Words)
				{
					maxSum += word.FullWidth + (word.HasSpaceBefore ? word.OwnerBox.ActualWordSpacing : 0.0);
					min = Math.Max(min, word.Width);
				}
				if (box.Words.Count > 0 && !box.Words[box.Words.Count - 1].HasSpaceAfter)
					maxSum -= box.Words[box.Words.Count - 1].ActualWordSpacing;
			}
			else
			{
				for (int i = 0; i < box.Boxes.Count; i++)
				{
					CssBox cssBox = box.Boxes[i];
					marginSum += cssBox.ActualMarginLeft + cssBox.ActualMarginRight;
					GetMinMaxSumWords(cssBox, ref min, ref maxSum, ref paddingSum, ref marginSum);
					marginSum -= cssBox.ActualMarginLeft + cssBox.ActualMarginRight;
				}
			}
			if (num.HasValue)
				maxSum = Math.Max(maxSum, num.Value);
		}

		internal bool HasJustInlineSiblings()
		{
			return ParentBox != null && DomUtils.ContainsInlinesOnly(ParentBox);
		}

		internal new void InheritStyle(CssBox box = null, bool everything = false)
		{
			base.InheritStyle(box ?? ParentBox, everything);
		}

		protected double MarginTopCollapse(CssBoxProperties prevSibling)
		{
			double num2 = ((prevSibling != null) ? (base.CollapsedMarginTop = Math.Max(prevSibling.ActualMarginBottom, base.ActualMarginTop)) : ((_parentBox == null || !(base.ActualPaddingTop < 0.1) || !(base.ActualPaddingBottom < 0.1) || !(_parentBox.ActualPaddingTop < 0.1) || !(_parentBox.ActualPaddingBottom < 0.1)) ? base.ActualMarginTop : Math.Max(0.0, base.ActualMarginTop - Math.Max(_parentBox.ActualMarginTop, _parentBox.CollapsedMarginTop))));
			if (num2 < 0.1 && HtmlTag != null && HtmlTag.Name == "hr")
				num2 = GetEmHeight() * 1.1000000238418579;
			return num2;
		}

		public bool BreakPage()
		{
			HtmlContainerInt htmlContainer = HtmlContainer;
			if (base.Size.Height >= htmlContainer.PageSize.Height)
				return false;
			double num = (base.Location.Y - (double)htmlContainer.MarginTop) % htmlContainer.PageSize.Height;
			double num2 = (base.ActualBottom - (double)htmlContainer.MarginTop) % htmlContainer.PageSize.Height;
			if (num > num2)
			{
				double num3 = htmlContainer.PageSize.Height - num;
				base.Location = new RPoint(base.Location.X, base.Location.Y + num3 + 1.0);
				return true;
			}
			return false;
		}

		private double CalculateActualRight()
		{
			if (base.ActualRight > 90999.0)
			{
				double num = 0.0;
				foreach (CssBox box in Boxes)
				{
					num = Math.Max(num, box.ActualRight + box.ActualMarginRight);
				}
				return num + base.ActualPaddingRight + base.ActualMarginRight + base.ActualBorderRightWidth;
			}
			return base.ActualRight;
		}

		private double MarginBottomCollapse()
		{
			double num = 0.0;
			if (ParentBox != null && ParentBox.Boxes.IndexOf(this) == ParentBox.Boxes.Count - 1 && _parentBox.ActualMarginBottom < 0.1)
			{
				double actualMarginBottom = _boxes[_boxes.Count - 1].ActualMarginBottom;
				num = ((base.Height == "auto") ? Math.Max(base.ActualMarginBottom, actualMarginBottom) : actualMarginBottom);
			}
			return Math.Max(base.ActualBottom, _boxes[_boxes.Count - 1].ActualBottom + num + base.ActualPaddingBottom + base.ActualBorderBottomWidth);
		}

		internal void OffsetTop(double amount)
		{
			List<CssLineBox> list = new List<CssLineBox>();
			foreach (CssLineBox key in Rectangles.Keys)
			{
				list.Add(key);
			}
			foreach (CssLineBox item in list)
			{
				RRect rRect = Rectangles[item];
				Rectangles[item] = new RRect(rRect.X, rRect.Y + amount, rRect.Width, rRect.Height);
			}
			foreach (CssRect word in Words)
			{
				word.Top += amount;
			}
			foreach (CssBox box in Boxes)
			{
				box.OffsetTop(amount);
			}
			if (_listItemBox != null)
				_listItemBox.OffsetTop(amount);
			base.Location = new RPoint(base.Location.X, base.Location.Y + amount);
		}

		protected virtual void PaintImp(RGraphics g)
		{
			if (!(base.Display != "none") || (!(base.Display != "table-cell") && !(base.EmptyCells != "hide") && IsSpaceOrEmpty))
				return;
			bool flag = RenderUtils.ClipGraphicsByOverflow(g, this);
			List<RRect> list = ((Rectangles.Count == 0) ? new List<RRect>(new RRect[1] { base.Bounds }) : new List<RRect>(Rectangles.Values));
			RRect clip = g.GetClip();
			RRect[] array = list.ToArray();
			RPoint rPoint = RPoint.Empty;
			if (!IsFixed)
				rPoint = HtmlContainer.ScrollOffset;
			for (int i = 0; i < array.Length; i++)
			{
				RRect rect = array[i];
				rect.Offset(rPoint);
				if (IsRectVisible(rect, clip))
				{
					PaintBackground(g, rect, i == 0, i == array.Length - 1);
					BordersDrawHandler.DrawBoxBorders(g, this, rect, i == 0, i == array.Length - 1);
				}
			}
			PaintWords(g, rPoint);
			for (int j = 0; j < array.Length; j++)
			{
				RRect rRect = array[j];
				rRect.Offset(rPoint);
				if (IsRectVisible(rRect, clip))
					PaintDecoration(g, rRect, j == 0, j == array.Length - 1);
			}
			foreach (CssBox box in Boxes)
			{
				if (box.Position != "absolute" && !box.IsFixed)
					box.Paint(g);
			}
			foreach (CssBox box2 in Boxes)
			{
				if (box2.Position == "absolute")
					box2.Paint(g);
			}
			foreach (CssBox box3 in Boxes)
			{
				if (box3.IsFixed)
					box3.Paint(g);
			}
			if (flag)
				g.PopClip();
			if (_listItemBox != null)
				_listItemBox.Paint(g);
		}

		private bool IsRectVisible(RRect rect, RRect clip)
		{
			rect.X -= 2.0;
			rect.Width += 2.0;
			clip.Intersect(rect);
			if (clip != RRect.Empty)
				return true;
			return false;
		}

		protected void PaintBackground(RGraphics g, RRect rect, bool isFirst, bool isLast)
		{
			if (!(rect.Width > 0.0) || !(rect.Height > 0.0))
				return;
			RBrush rBrush = null;
			if (base.BackgroundGradient != "none")
				rBrush = g.GetLinearGradientBrush(rect, base.ActualBackgroundColor, base.ActualBackgroundGradient, base.ActualBackgroundGradientAngle);
			else if (RenderUtils.IsColorVisible(base.ActualBackgroundColor))
			{
				rBrush = g.GetSolidBrush(base.ActualBackgroundColor);
			}
			if (rBrush != null)
			{
				RGraphicsPath rGraphicsPath = null;
				if (base.IsRounded)
					rGraphicsPath = RenderUtils.GetRoundRect(g, rect, base.ActualCornerNw, base.ActualCornerNe, base.ActualCornerSe, base.ActualCornerSw);
				object prevMode = null;
				if (HtmlContainer != null && !HtmlContainer.AvoidGeometryAntialias && base.IsRounded)
					prevMode = g.SetAntiAliasSmoothingMode();
				if (rGraphicsPath == null)
					g.DrawRectangle(rBrush, Math.Ceiling(rect.X), Math.Ceiling(rect.Y), rect.Width, rect.Height);
				else
					g.DrawPath(rBrush, rGraphicsPath);
				g.ReturnPreviousSmoothingMode(prevMode);
				if (rGraphicsPath != null)
					rGraphicsPath.Dispose();
				rBrush.Dispose();
			}
			if (_imageLoadHandler != null && _imageLoadHandler.Image != null && isFirst)
				BackgroundImageDrawHandler.DrawBackgroundImage(g, this, _imageLoadHandler, rect);
		}

		private void PaintWords(RGraphics g, RPoint offset)
		{
			if (base.Width.Length <= 0)
				return;
			bool rtl = base.Direction == "rtl";
			foreach (CssRect word in Words)
			{
				if (word.IsLineBreak)
					continue;
				RRect clip = g.GetClip();
				RRect rectangle = word.Rectangle;
				rectangle.Offset(offset);
				clip.Intersect(rectangle);
				if (!(clip != RRect.Empty))
					continue;
				RPoint point = new RPoint(word.Left + offset.X, word.Top + offset.Y);
				if (word.Selected)
				{
					CssLineBox cssLineBoxByWord = DomUtils.GetCssLineBoxByWord(word);
					double num = ((word.SelectedStartOffset > -1.0) ? word.SelectedStartOffset : ((cssLineBoxByWord.Words[0] == word || !word.HasSpaceBefore) ? 0.0 : (0.0 - base.ActualWordSpacing)));
					bool flag = word.HasSpaceAfter && !cssLineBoxByWord.IsLastSelectedWord(word);
					double num2 = ((word.SelectedEndOffset > -1.0) ? word.SelectedEndOffset : (word.Width + (flag ? base.ActualWordSpacing : 0.0)));
					RRect rect = new RRect(word.Left + offset.X + num, word.Top + offset.Y, num2 - num, cssLineBoxByWord.LineHeight);
					g.DrawRectangle(GetSelectionBackBrush(g, false), rect.X, rect.Y, rect.Width, rect.Height);
					if (HtmlContainer.SelectionForeColor != RColor.Empty && (word.SelectedStartOffset > 0.0 || word.SelectedEndIndexOffset > -1))
					{
						g.PushClipExclude(rect);
						g.DrawString(word.Text, base.ActualFont, base.ActualColor, point, new RSize(word.Width, word.Height), rtl);
						g.PopClip();
						g.PushClip(rect);
						g.DrawString(word.Text, base.ActualFont, GetSelectionForeBrush(), point, new RSize(word.Width, word.Height), rtl);
						g.PopClip();
					}
					else
						g.DrawString(word.Text, base.ActualFont, GetSelectionForeBrush(), point, new RSize(word.Width, word.Height), rtl);
				}
				else
					g.DrawString(word.Text, base.ActualFont, base.ActualColor, point, new RSize(word.Width, word.Height), rtl);
			}
		}

		protected void PaintDecoration(RGraphics g, RRect rectangle, bool isFirst, bool isLast)
		{
			if (!string.IsNullOrEmpty(base.TextDecoration) && !(base.TextDecoration == "none"))
			{
				double num = 0.0;
				if (base.TextDecoration == "underline")
					num = Math.Round(rectangle.Top + base.ActualFont.UnderlineOffset);
				else if (base.TextDecoration == "line-through")
				{
					num = rectangle.Top + rectangle.Height / 2.0;
				}
				else if (base.TextDecoration == "overline")
				{
					num = rectangle.Top;
				}
				num -= base.ActualPaddingBottom - base.ActualBorderBottomWidth;
				double num2 = rectangle.X;
				if (isFirst)
					num2 += base.ActualPaddingLeft + base.ActualBorderLeftWidth;
				double num3 = rectangle.Right;
				if (isLast)
					num3 -= base.ActualPaddingRight + base.ActualBorderRightWidth;
				RPen pen = g.GetPen(base.ActualColor);
				pen.Width = 1.0;
				pen.DashStyle = RDashStyle.Solid;
				g.DrawLine(pen, num2, num, num3, num);
			}
		}

		internal void OffsetRectangle(CssLineBox lineBox, double gap)
		{
			if (Rectangles.ContainsKey(lineBox))
			{
				RRect rRect = Rectangles[lineBox];
				Rectangles[lineBox] = new RRect(rRect.X, rRect.Y + gap, rRect.Width, rRect.Height);
			}
		}

		internal void RectanglesReset()
		{
			_rectangles.Clear();
		}

		private void OnImageLoadComplete(RImage image, RRect rectangle, bool async)
		{
			if (image != null && async)
				HtmlContainer.RequestRefresh(false);
		}

		protected RColor GetSelectionForeBrush()
		{
			return (HtmlContainer.SelectionForeColor != RColor.Empty) ? HtmlContainer.SelectionForeColor : base.ActualColor;
		}

		protected RBrush GetSelectionBackBrush(RGraphics g, bool forceAlpha)
		{
			RColor selectionBackColor = HtmlContainer.SelectionBackColor;
			if (selectionBackColor != RColor.Empty)
			{
				if (forceAlpha && selectionBackColor.A > 180)
					return g.GetSolidBrush(RColor.FromArgb(180, selectionBackColor.R, selectionBackColor.G, selectionBackColor.B));
				return g.GetSolidBrush(selectionBackColor);
			}
			return g.GetSolidBrush(CssUtils.DefaultSelectionBackcolor);
		}

		protected override RFont GetCachedFont(string fontFamily, double fsize, RFontStyle st)
		{
			return HtmlContainer.Adapter.GetFont(fontFamily, fsize, st);
		}

		protected override RColor GetActualColor(string colorStr)
		{
			return HtmlContainer.CssParser.ParseColor(colorStr);
		}

		protected override RPoint GetActualLocation(string X, string Y)
		{
			double x = CssValueParser.ParseLength(X, HtmlContainer.PageSize.Width, this, null);
			double y = CssValueParser.ParseLength(Y, HtmlContainer.PageSize.Height, this, null);
			return new RPoint(x, y);
		}

		public override string ToString()
		{
			string text = ((HtmlTag != null) ? string.Format("<{0}>", HtmlTag.Name) : "anon");
			if (IsBlock)
				return string.Format("{0}{1} Block {2}, Children:{3}", (ParentBox == null) ? "Root: " : string.Empty, text, base.FontSize, Boxes.Count);
			if (base.Display == "none")
				return string.Format("{0}{1} None", (ParentBox == null) ? "Root: " : string.Empty, text);
			return string.Format("{0}{1} {2}: {3}", (ParentBox == null) ? "Root: " : string.Empty, text, base.Display, Text);
		}
	}
}
