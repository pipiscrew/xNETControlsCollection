using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal abstract class CssRect
	{
		private readonly CssBox _ownerBox;

		private RRect _rect;

		private SelectionHandler _selection;

		public CssBox OwnerBox
		{
			get
			{
				return _ownerBox;
			}
		}

		public RRect Rectangle
		{
			get
			{
				return _rect;
			}
			set
			{
				_rect = value;
			}
		}

		public double Left
		{
			get
			{
				return _rect.X;
			}
			set
			{
				_rect.X = value;
			}
		}

		public double Top
		{
			get
			{
				return _rect.Y;
			}
			set
			{
				_rect.Y = value;
			}
		}

		public double Width
		{
			get
			{
				return _rect.Width;
			}
			set
			{
				_rect.Width = value;
			}
		}

		public double FullWidth
		{
			get
			{
				return _rect.Width + ActualWordSpacing;
			}
		}

		public double ActualWordSpacing
		{
			get
			{
				return (OwnerBox != null) ? ((HasSpaceAfter ? OwnerBox.ActualWordSpacing : 0.0) + (IsImage ? OwnerBox.ActualWordSpacing : 0.0)) : 0.0;
			}
		}

		public double Height
		{
			get
			{
				return _rect.Height;
			}
			set
			{
				_rect.Height = value;
			}
		}

		public double Right
		{
			get
			{
				return Rectangle.Right;
			}
			set
			{
				Width = value - Left;
			}
		}

		public double Bottom
		{
			get
			{
				return Rectangle.Bottom;
			}
			set
			{
				Height = value - Top;
			}
		}

		public SelectionHandler Selection
		{
			get
			{
				return _selection;
			}
			set
			{
				_selection = value;
			}
		}

		public virtual bool HasSpaceBefore
		{
			get
			{
				return false;
			}
		}

		public virtual bool HasSpaceAfter
		{
			get
			{
				return false;
			}
		}

		public virtual RImage Image
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public virtual bool IsImage
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSpaces
		{
			get
			{
				return true;
			}
		}

		public virtual bool IsLineBreak
		{
			get
			{
				return false;
			}
		}

		public virtual string Text
		{
			get
			{
				return null;
			}
		}

		public bool Selected
		{
			get
			{
				return _selection != null;
			}
		}

		public int SelectedStartIndex
		{
			get
			{
				return (_selection != null) ? _selection.GetSelectingStartIndex(this) : (-1);
			}
		}

		public int SelectedEndIndexOffset
		{
			get
			{
				return (_selection != null) ? _selection.GetSelectedEndIndexOffset(this) : (-1);
			}
		}

		public double SelectedStartOffset
		{
			get
			{
				return (_selection != null) ? _selection.GetSelectedStartOffset(this) : (-1.0);
			}
		}

		public double SelectedEndOffset
		{
			get
			{
				return (_selection != null) ? _selection.GetSelectedEndOffset(this) : (-1.0);
			}
		}

		internal double LeftGlyphPadding
		{
			get
			{
				return (OwnerBox != null) ? OwnerBox.ActualFont.LeftPadding : 0.0;
			}
		}

		protected CssRect(CssBox owner)
		{
			_ownerBox = owner;
		}

		public override string ToString()
		{
			return string.Format("{0} ({1} char{2})", Text.Replace(' ', '-').Replace("\n", "\\n"), Text.Length, (Text.Length != 1) ? "s" : string.Empty);
		}

		public bool BreakPage()
		{
			HtmlContainerInt htmlContainer = OwnerBox.HtmlContainer;
			if (Height >= htmlContainer.PageSize.Height)
				return false;
			double num = (Top - (double)htmlContainer.MarginTop) % htmlContainer.PageSize.Height;
			double num2 = (Bottom - (double)htmlContainer.MarginTop) % htmlContainer.PageSize.Height;
			if (num > num2)
			{
				Top += htmlContainer.PageSize.Height - num + 1.0;
				return true;
			}
			return false;
		}
	}
}
