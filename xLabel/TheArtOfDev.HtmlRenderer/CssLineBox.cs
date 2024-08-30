using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssLineBox
	{
		private readonly List<CssRect> _words;

		private readonly CssBox _ownerBox;

		private readonly Dictionary<CssBox, RRect> _rects;

		private readonly List<CssBox> _relatedBoxes;

		public List<CssBox> RelatedBoxes
		{
			get
			{
				return _relatedBoxes;
			}
		}

		public List<CssRect> Words
		{
			get
			{
				return _words;
			}
		}

		public CssBox OwnerBox
		{
			get
			{
				return _ownerBox;
			}
		}

		public Dictionary<CssBox, RRect> Rectangles
		{
			get
			{
				return _rects;
			}
		}

		public double LineHeight
		{
			get
			{
				double num = 0.0;
				foreach (KeyValuePair<CssBox, RRect> rect in _rects)
				{
					num = Math.Max(num, rect.Value.Height);
				}
				return num;
			}
		}

		public double LineBottom
		{
			get
			{
				double num = 0.0;
				foreach (KeyValuePair<CssBox, RRect> rect in _rects)
				{
					num = Math.Max(num, rect.Value.Bottom);
				}
				return num;
			}
		}

		public CssLineBox(CssBox ownerBox)
		{
			_rects = new Dictionary<CssBox, RRect>();
			_relatedBoxes = new List<CssBox>();
			_words = new List<CssRect>();
			_ownerBox = ownerBox;
			_ownerBox.LineBoxes.Add(this);
		}

		internal void ReportExistanceOf(CssRect word)
		{
			if (!Words.Contains(word))
				Words.Add(word);
			if (!RelatedBoxes.Contains(word.OwnerBox))
				RelatedBoxes.Add(word.OwnerBox);
		}

		internal List<CssRect> WordsOf(CssBox box)
		{
			List<CssRect> list = new List<CssRect>();
			foreach (CssRect word in Words)
			{
				if (word.OwnerBox.Equals(box))
					list.Add(word);
			}
			return list;
		}

		internal void UpdateRectangle(CssBox box, double x, double y, double r, double b)
		{
			double num = box.ActualBorderLeftWidth + box.ActualPaddingLeft;
			double num2 = box.ActualBorderRightWidth + box.ActualPaddingRight;
			double num3 = box.ActualBorderTopWidth + box.ActualPaddingTop;
			double num4 = box.ActualBorderBottomWidth + box.ActualPaddingTop;
			if ((box.FirstHostingLineBox != null && box.FirstHostingLineBox.Equals(this)) || box.IsImage)
				x -= num;
			if ((box.LastHostingLineBox != null && box.LastHostingLineBox.Equals(this)) || box.IsImage)
				r += num2;
			if (!box.IsImage)
			{
				y -= num3;
				b += num4;
			}
			if (Rectangles.ContainsKey(box))
			{
				RRect rRect = Rectangles[box];
				Rectangles[box] = RRect.FromLTRB(Math.Min(rRect.X, x), Math.Min(rRect.Y, y), Math.Max(rRect.Right, r), Math.Max(rRect.Bottom, b));
			}
			else
				Rectangles.Add(box, RRect.FromLTRB(x, y, r, b));
			if (box.ParentBox != null && box.ParentBox.IsInline)
				UpdateRectangle(box.ParentBox, x, y, r, b);
		}

		internal void AssignRectanglesToBoxes()
		{
			foreach (CssBox key in Rectangles.Keys)
			{
				key.Rectangles.Add(this, Rectangles[key]);
			}
		}

		internal void SetBaseLine(RGraphics g, CssBox b, double baseline)
		{
			List<CssRect> list = WordsOf(b);
			if (!Rectangles.ContainsKey(b))
				return;
			RRect rRect = Rectangles[b];
			double num = 0.0;
			if (list.Count > 0)
				num = list[0].Top - rRect.Top;
			else
			{
				CssRect cssRect = b.FirstWordOccourence(b, this);
				if (cssRect != null)
					num = cssRect.Top - rRect.Top;
			}
			if (b.ParentBox != null && b.ParentBox.Rectangles.ContainsKey(this) && rRect.Height < b.ParentBox.Rectangles[this].Height)
			{
				double y = baseline - num;
				RRect value = new RRect(rRect.X, y, rRect.Width, rRect.Height);
				Rectangles[b] = value;
				b.OffsetRectangle(this, num);
			}
			foreach (CssRect item in list)
			{
				if (!item.IsImage)
					item.Top = baseline;
			}
		}

		public bool IsLastSelectedWord(CssRect word)
		{
			for (int i = 0; i < _words.Count - 1; i++)
			{
				if (_words[i] == word)
					return !_words[i + 1].Selected;
			}
			return true;
		}

		public override string ToString()
		{
			string[] array = new string[Words.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = Words[i].Text;
			}
			return string.Join(" ", array);
		}
	}
}
