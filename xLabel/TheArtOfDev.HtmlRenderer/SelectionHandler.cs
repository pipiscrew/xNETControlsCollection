using System;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Dom;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal sealed class SelectionHandler : IDisposable
	{
		private readonly CssBox _root;

		private readonly ContextMenuHandler _contextMenuHandler;

		private RPoint _selectionStartPoint;

		private CssRect _selectionStart;

		private CssRect _selectionEnd;

		private int _selectionStartIndex = -1;

		private int _selectionEndIndex = -1;

		private double _selectionStartOffset = -1.0;

		private double _selectionEndOffset = -1.0;

		private bool _backwardSelection;

		private bool _inSelection;

		private bool _isDoubleClickSelect;

		private bool _mouseDownInControl;

		private bool _mouseDownOnSelectedWord;

		private bool _cursorChanged;

		private DateTime _lastMouseDown;

		private object _dragDropData;

		public SelectionHandler(CssBox root)
		{
			ArgChecker.AssertArgNotNull(root, "root");
			_root = root;
			_contextMenuHandler = new ContextMenuHandler(this, root.HtmlContainer);
		}

		public void SelectAll(RControl control)
		{
			if (_root.HtmlContainer.IsSelectionEnabled)
			{
				ClearSelection();
				SelectAllWords(_root);
				control.Invalidate();
			}
		}

		public void SelectWord(RControl control, RPoint loc)
		{
			if (_root.HtmlContainer.IsSelectionEnabled)
			{
				CssRect cssBoxWord = DomUtils.GetCssBoxWord(_root, loc);
				if (cssBoxWord != null)
				{
					cssBoxWord.Selection = this;
					_selectionStartPoint = loc;
					_selectionStart = (_selectionEnd = cssBoxWord);
					control.Invalidate();
				}
			}
		}

		public void HandleMouseDown(RControl parent, RPoint loc, bool isMouseInContainer)
		{
			bool flag = !isMouseInContainer;
			if (isMouseInContainer)
			{
				_mouseDownInControl = true;
				_isDoubleClickSelect = (DateTime.Now - _lastMouseDown).TotalMilliseconds < 400.0;
				_lastMouseDown = DateTime.Now;
				_mouseDownOnSelectedWord = false;
				if (_root.HtmlContainer.IsSelectionEnabled && parent.LeftMouseButton)
				{
					CssRect cssBoxWord = DomUtils.GetCssBoxWord(_root, loc);
					if (cssBoxWord == null || !cssBoxWord.Selected)
						flag = true;
					else
						_mouseDownOnSelectedWord = true;
				}
				else if (parent.RightMouseButton)
				{
					CssRect cssBoxWord2 = DomUtils.GetCssBoxWord(_root, loc);
					CssBox linkBox = DomUtils.GetLinkBox(_root, loc);
					if (_root.HtmlContainer.IsContextMenuEnabled)
						_contextMenuHandler.ShowContextMenu(parent, cssBoxWord2, linkBox);
					flag = cssBoxWord2 == null || !cssBoxWord2.Selected;
				}
			}
			if (flag)
			{
				ClearSelection();
				parent.Invalidate();
			}
		}

		public bool HandleMouseUp(RControl parent, bool leftMouseButton)
		{
			bool flag = false;
			_mouseDownInControl = false;
			if (_root.HtmlContainer.IsSelectionEnabled)
			{
				flag = _inSelection;
				if (!_inSelection && leftMouseButton && _mouseDownOnSelectedWord)
				{
					ClearSelection();
					parent.Invalidate();
				}
				_mouseDownOnSelectedWord = false;
				_inSelection = false;
			}
			return flag = flag || DateTime.Now - _lastMouseDown > TimeSpan.FromSeconds(1.0);
		}

		public void HandleMouseMove(RControl parent, RPoint loc)
		{
			if (_root.HtmlContainer.IsSelectionEnabled && _mouseDownInControl && parent.LeftMouseButton)
			{
				if (_mouseDownOnSelectedWord)
				{
					if ((DateTime.Now - _lastMouseDown).TotalMilliseconds > 200.0)
						StartDragDrop(parent);
				}
				else
				{
					HandleSelection(parent, loc, !_isDoubleClickSelect);
					_inSelection = _selectionStart != null && _selectionEnd != null && (_selectionStart != _selectionEnd || _selectionStartIndex != _selectionEndIndex);
				}
				return;
			}
			CssBox linkBox = DomUtils.GetLinkBox(_root, loc);
			if (linkBox == null)
			{
				if (_root.HtmlContainer.IsSelectionEnabled)
				{
					CssRect cssBoxWord = DomUtils.GetCssBoxWord(_root, loc);
					_cursorChanged = cssBoxWord != null && !cssBoxWord.IsImage && (!cssBoxWord.Selected || (cssBoxWord.SelectedStartIndex >= 0 && !(cssBoxWord.Left + cssBoxWord.SelectedStartOffset <= loc.X)) || (!(cssBoxWord.SelectedEndOffset < 0.0) && !(cssBoxWord.Left + cssBoxWord.SelectedEndOffset >= loc.X)));
					if (_cursorChanged)
						parent.SetCursorIBeam();
					else
						parent.SetCursorDefault();
				}
				else if (_cursorChanged)
				{
					parent.SetCursorDefault();
				}
			}
			else
			{
				_cursorChanged = true;
				parent.SetCursorHand();
			}
		}

		public void HandleMouseLeave(RControl parent)
		{
			if (_cursorChanged)
			{
				_cursorChanged = false;
				parent.SetCursorDefault();
			}
		}

		public void CopySelectedHtml()
		{
			if (_root.HtmlContainer.IsSelectionEnabled)
			{
				string html = DomUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true);
				string selectedPlainText = DomUtils.GetSelectedPlainText(_root);
				if (!string.IsNullOrEmpty(selectedPlainText))
					_root.HtmlContainer.Adapter.SetToClipboard(html, selectedPlainText);
			}
		}

		public string GetSelectedText()
		{
			return _root.HtmlContainer.IsSelectionEnabled ? DomUtils.GetSelectedPlainText(_root) : null;
		}

		public string GetSelectedHtml()
		{
			return _root.HtmlContainer.IsSelectionEnabled ? DomUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true) : null;
		}

		public int GetSelectingStartIndex(CssRect word)
		{
			return (word != (_backwardSelection ? _selectionEnd : _selectionStart)) ? (-1) : (_backwardSelection ? _selectionEndIndex : _selectionStartIndex);
		}

		public int GetSelectedEndIndexOffset(CssRect word)
		{
			return (word != (_backwardSelection ? _selectionStart : _selectionEnd)) ? (-1) : (_backwardSelection ? _selectionStartIndex : _selectionEndIndex);
		}

		public double GetSelectedStartOffset(CssRect word)
		{
			return (word != (_backwardSelection ? _selectionEnd : _selectionStart)) ? (-1.0) : (_backwardSelection ? _selectionEndOffset : _selectionStartOffset);
		}

		public double GetSelectedEndOffset(CssRect word)
		{
			return (word != (_backwardSelection ? _selectionStart : _selectionEnd)) ? (-1.0) : (_backwardSelection ? _selectionStartOffset : _selectionEndOffset);
		}

		public void ClearSelection()
		{
			_dragDropData = null;
			ClearSelection(_root);
			_selectionStartOffset = -1.0;
			_selectionStartIndex = -1;
			_selectionEndOffset = -1.0;
			_selectionEndIndex = -1;
			_selectionStartPoint = RPoint.Empty;
			_selectionStart = null;
			_selectionEnd = null;
		}

		public void Dispose()
		{
			_contextMenuHandler.Dispose();
		}

		private void HandleSelection(RControl control, RPoint loc, bool allowPartialSelect)
		{
			CssLineBox cssLineBox = DomUtils.GetCssLineBox(_root, loc);
			if (cssLineBox == null)
				return;
			CssRect cssRect = DomUtils.GetCssBoxWord(cssLineBox, loc);
			if (cssRect == null && cssLineBox.Words.Count > 0)
			{
				if (loc.Y > cssLineBox.LineBottom)
					cssRect = cssLineBox.Words[cssLineBox.Words.Count - 1];
				else if (loc.X < cssLineBox.Words[0].Left)
				{
					cssRect = cssLineBox.Words[0];
				}
				else if (loc.X > cssLineBox.Words[cssLineBox.Words.Count - 1].Right)
				{
					cssRect = cssLineBox.Words[cssLineBox.Words.Count - 1];
				}
			}
			if (cssRect == null)
				return;
			if (_selectionStart == null)
			{
				_selectionStartPoint = loc;
				_selectionStart = cssRect;
				if (allowPartialSelect)
					CalculateWordCharIndexAndOffset(control, cssRect, loc, true);
			}
			_selectionEnd = cssRect;
			if (allowPartialSelect)
				CalculateWordCharIndexAndOffset(control, cssRect, loc, false);
			ClearSelection(_root);
			if (CheckNonEmptySelection(loc, allowPartialSelect))
			{
				CheckSelectionDirection();
				SelectWordsInRange(_root, _backwardSelection ? _selectionEnd : _selectionStart, _backwardSelection ? _selectionStart : _selectionEnd);
			}
			else
				_selectionEnd = null;
			_cursorChanged = true;
			control.SetCursorIBeam();
			control.Invalidate();
		}

		private static void ClearSelection(CssBox box)
		{
			foreach (CssRect word in box.Words)
			{
				word.Selection = null;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				ClearSelection(box2);
			}
		}

		private void StartDragDrop(RControl control)
		{
			if (_dragDropData == null)
			{
				string html = DomUtils.GenerateHtml(_root, HtmlGenerationStyle.Inline, true);
				string selectedPlainText = DomUtils.GetSelectedPlainText(_root);
				_dragDropData = control.Adapter.GetClipboardDataObject(html, selectedPlainText);
			}
			control.DoDragDropCopy(_dragDropData);
		}

		public void SelectAllWords(CssBox box)
		{
			foreach (CssRect word in box.Words)
			{
				word.Selection = this;
			}
			foreach (CssBox box2 in box.Boxes)
			{
				SelectAllWords(box2);
			}
		}

		private bool CheckNonEmptySelection(RPoint loc, bool allowPartialSelect)
		{
			if (!allowPartialSelect)
				return true;
			if (Math.Abs(_selectionStartPoint.X - loc.X) <= 1.0 && Math.Abs(_selectionStartPoint.Y - loc.Y) < 5.0)
				return false;
			return _selectionStart != _selectionEnd || _selectionStartIndex != _selectionEndIndex;
		}

		private void SelectWordsInRange(CssBox root, CssRect selectionStart, CssRect selectionEnd)
		{
			bool inSelection = false;
			SelectWordsInRange(root, selectionStart, selectionEnd, ref inSelection);
		}

		private bool SelectWordsInRange(CssBox box, CssRect selectionStart, CssRect selectionEnd, ref bool inSelection)
		{
			foreach (CssRect word in box.Words)
			{
				if (!inSelection && word == selectionStart)
					inSelection = true;
				if (inSelection)
				{
					word.Selection = this;
					if (selectionStart == selectionEnd || word == selectionEnd)
						return true;
				}
			}
			foreach (CssBox box2 in box.Boxes)
			{
				if (SelectWordsInRange(box2, selectionStart, selectionEnd, ref inSelection))
					return true;
			}
			return false;
		}

		private void CalculateWordCharIndexAndOffset(RControl control, CssRect word, RPoint loc, bool selectionStart)
		{
			int selectionIndex;
			double selectionOffset;
			CalculateWordCharIndexAndOffset(control, word, loc, selectionStart, out selectionIndex, out selectionOffset);
			if (selectionStart)
			{
				_selectionStartIndex = selectionIndex;
				_selectionStartOffset = selectionOffset;
			}
			else
			{
				_selectionEndIndex = selectionIndex;
				_selectionEndOffset = selectionOffset;
			}
		}

		private static void CalculateWordCharIndexAndOffset(RControl control, CssRect word, RPoint loc, bool inclusive, out int selectionIndex, out double selectionOffset)
		{
			selectionIndex = 0;
			selectionOffset = 0.0;
			double num = loc.X - word.Left;
			if (word.Text == null)
			{
				selectionIndex = -1;
				selectionOffset = -1.0;
			}
			else if (num > word.Width - word.OwnerBox.ActualWordSpacing || loc.Y > DomUtils.GetCssLineBoxByWord(word).LineBottom)
			{
				selectionIndex = word.Text.Length;
				selectionOffset = word.Width;
			}
			else if (num > 0.0)
			{
				double maxWidth = num + (inclusive ? 0.0 : (1.5 * word.LeftGlyphPadding));
				int charFit;
				double charFitWidth;
				control.MeasureString(word.Text, word.OwnerBox.ActualFont, maxWidth, out charFit, out charFitWidth);
				selectionIndex = charFit;
				selectionOffset = charFitWidth;
			}
		}

		private void CheckSelectionDirection()
		{
			if (_selectionStart == _selectionEnd)
				_backwardSelection = _selectionStartIndex > _selectionEndIndex;
			else if (DomUtils.GetCssLineBoxByWord(_selectionStart) == DomUtils.GetCssLineBoxByWord(_selectionEnd))
			{
				_backwardSelection = _selectionStart.Left > _selectionEnd.Left;
			}
			else
			{
				_backwardSelection = _selectionStart.Top >= _selectionEnd.Bottom;
			}
		}
	}
}
