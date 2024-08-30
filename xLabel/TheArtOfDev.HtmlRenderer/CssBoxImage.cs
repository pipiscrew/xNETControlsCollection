using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Handlers;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssBoxImage : CssBox
	{
		private readonly CssRectImage _imageWord;

		private ImageLoadHandler _imageLoadHandler;

		private bool _imageLoadingComplete;

		public RImage Image
		{
			get
			{
				return _imageWord.Image;
			}
		}

		public CssBoxImage(CssBox parent, HtmlTag tag)
			: base(parent, tag)
		{
			_imageWord = new CssRectImage(this);
			base.Words.Add(_imageWord);
		}

		protected override void PaintImp(RGraphics g)
		{
			if (_imageLoadHandler == null)
			{
				_imageLoadHandler = new ImageLoadHandler(base.HtmlContainer, OnLoadImageComplete);
				_imageLoadHandler.LoadImage(GetAttribute("src"), (base.HtmlTag != null) ? base.HtmlTag.Attributes : null);
			}
			RRect firstValueOrDefault = CommonUtils.GetFirstValueOrDefault(base.Rectangles);
			RPoint pos = RPoint.Empty;
			if (!IsFixed)
				pos = base.HtmlContainer.ScrollOffset;
			firstValueOrDefault.Offset(pos);
			bool flag = RenderUtils.ClipGraphicsByOverflow(g, this);
			PaintBackground(g, firstValueOrDefault, true, true);
			BordersDrawHandler.DrawBoxBorders(g, this, firstValueOrDefault, true, true);
			RRect rectangle = _imageWord.Rectangle;
			rectangle.Offset(pos);
			rectangle.Height -= base.ActualBorderTopWidth + base.ActualBorderBottomWidth + base.ActualPaddingTop + base.ActualPaddingBottom;
			rectangle.Y += base.ActualBorderTopWidth + base.ActualPaddingTop;
			rectangle.X = Math.Floor(rectangle.X);
			rectangle.Y = Math.Floor(rectangle.Y);
			if (_imageWord.Image != null)
			{
				if (rectangle.Width > 0.0 && rectangle.Height > 0.0)
				{
					if (_imageWord.ImageRectangle == RRect.Empty)
						g.DrawImage(_imageWord.Image, rectangle);
					else
						g.DrawImage(_imageWord.Image, rectangle, _imageWord.ImageRectangle);
					if (_imageWord.Selected)
						g.DrawRectangle(GetSelectionBackBrush(g, true), _imageWord.Left + pos.X, _imageWord.Top + pos.Y, _imageWord.Width + 2.0, DomUtils.GetCssLineBoxByWord(_imageWord).LineHeight);
				}
			}
			else if (_imageLoadingComplete)
			{
				if (_imageLoadingComplete && rectangle.Width > 19.0 && rectangle.Height > 19.0)
					RenderUtils.DrawImageErrorIcon(g, base.HtmlContainer, rectangle);
			}
			else
			{
				RenderUtils.DrawImageLoadingIcon(g, base.HtmlContainer, rectangle);
				if (rectangle.Width > 19.0 && rectangle.Height > 19.0)
					g.DrawRectangle(g.GetPen(RColor.LightGray), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
			}
			if (flag)
				g.PopClip();
		}

		internal override void MeasureWordsSize(RGraphics g)
		{
			if (!_wordsSizeMeasured)
			{
				if (_imageLoadHandler == null && (base.HtmlContainer.AvoidAsyncImagesLoading || base.HtmlContainer.AvoidImagesLateLoading))
				{
					_imageLoadHandler = new ImageLoadHandler(base.HtmlContainer, OnLoadImageComplete);
					if (base.Content == null || !(base.Content != "normal"))
						_imageLoadHandler.LoadImage(GetAttribute("src"), (base.HtmlTag != null) ? base.HtmlTag.Attributes : null);
					else
						_imageLoadHandler.LoadImage(base.Content, (base.HtmlTag != null) ? base.HtmlTag.Attributes : null);
				}
				MeasureWordSpacing(g);
				_wordsSizeMeasured = true;
			}
			CssLayoutEngine.MeasureImageSize(_imageWord);
		}

		public override void Dispose()
		{
			if (_imageLoadHandler != null)
				_imageLoadHandler.Dispose();
			base.Dispose();
		}

		private void SetErrorBorder()
		{
			SetAllBorders("solid", "2px", "#A0A0A0");
			string text3 = (base.BorderRightColor = (base.BorderBottomColor = "#E3E3E3"));
		}

		private void OnLoadImageComplete(RImage image, RRect rectangle, bool async)
		{
			_imageWord.Image = image;
			_imageWord.ImageRectangle = rectangle;
			_imageLoadingComplete = true;
			_wordsSizeMeasured = false;
			if (_imageLoadingComplete && image == null)
				SetErrorBorder();
			if (!base.HtmlContainer.AvoidImagesLateLoading || async)
			{
				CssLength cssLength = new CssLength(base.Width);
				CssLength cssLength2 = new CssLength(base.Height);
				bool layout = cssLength.Number <= 0.0 || cssLength.Unit != CssUnit.Pixels || cssLength2.Number <= 0.0 || cssLength2.Unit != CssUnit.Pixels;
				base.HtmlContainer.RequestRefresh(layout);
			}
		}
	}
}
