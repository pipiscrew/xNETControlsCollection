using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssLayoutEngineTable
	{
		private readonly CssBox _tableBox;

		private CssBox _caption;

		private CssBox _headerBox;

		private CssBox _footerBox;

		private readonly List<CssBox> _bodyrows = new List<CssBox>();

		private readonly List<CssBox> _columns = new List<CssBox>();

		private readonly List<CssBox> _allRows = new List<CssBox>();

		private int _columnCount;

		private bool _widthSpecified;

		private double[] _columnWidths;

		private double[] _columnMinWidths;

		private CssLayoutEngineTable(CssBox tableBox)
		{
			_tableBox = tableBox;
		}

		public static double GetTableSpacing(CssBox tableBox)
		{
			int num = 0;
			int num2 = 0;
			foreach (CssBox box in tableBox.Boxes)
			{
				if (box.Display == "table-column")
					num2 += GetSpan(box);
				else if (box.Display == "table-row-group")
				{
					foreach (CssBox box2 in tableBox.Boxes)
					{
						num++;
						if (box2.Display == "table-row")
							num2 = Math.Max(num2, box2.Boxes.Count);
					}
				}
				else if (box.Display == "table-row")
				{
					num++;
					num2 = Math.Max(num2, box.Boxes.Count);
				}
				if (num > 30)
					break;
			}
			return (double)(num2 + 1) * GetHorizontalSpacing(tableBox);
		}

		public static void PerformLayout(RGraphics g, CssBox tableBox)
		{
			ArgChecker.AssertArgNotNull(g, "g");
			ArgChecker.AssertArgNotNull(tableBox, "tableBox");
			try
			{
				CssLayoutEngineTable cssLayoutEngineTable = new CssLayoutEngineTable(tableBox);
				cssLayoutEngineTable.Layout(g);
			}
			catch (Exception exception)
			{
				tableBox.HtmlContainer.ReportError(HtmlRenderErrorType.Layout, "Failed table layout", exception);
			}
		}

		private void Layout(RGraphics g)
		{
			MeasureWords(_tableBox, g);
			AssignBoxKinds();
			InsertEmptyBoxes();
			double availCellSpace = CalculateCountAndWidth();
			DetermineMissingColumnWidths(availCellSpace);
			EnforceMinimumSize();
			EnforceMaximumSize();
			CssBox tableBox = _tableBox;
			CssBox tableBox2 = _tableBox;
			CssBox tableBox3 = _tableBox;
			string text2 = (_tableBox.PaddingBottom = "0");
			string text4 = (tableBox3.PaddingRight = text2);
			string text7 = (tableBox.PaddingLeft = (tableBox2.PaddingTop = text4));
			LayoutCells(g);
		}

		private void AssignBoxKinds()
		{
			foreach (CssBox box in _tableBox.Boxes)
			{
				switch (box.Display)
				{
				case "table-row-group":
					foreach (CssBox box2 in box.Boxes)
					{
						if (box2.Display == "table-row")
							_bodyrows.Add(box2);
					}
					break;
				case "table-row":
					_bodyrows.Add(box);
					break;
				case "table-caption":
					_caption = box;
					break;
				case "table-footer-group":
					if (_footerBox != null)
						_bodyrows.Add(box);
					else
						_footerBox = box;
					break;
				case "table-column-group":
					if (box.Boxes.Count == 0)
					{
						int span = GetSpan(box);
						for (int j = 0; j < span; j++)
						{
							_columns.Add(box);
						}
						break;
					}
					foreach (CssBox box3 in box.Boxes)
					{
						int span2 = GetSpan(box3);
						for (int k = 0; k < span2; k++)
						{
							_columns.Add(box3);
						}
					}
					break;
				case "table-column":
				{
					for (int i = 0; i < GetSpan(box); i++)
					{
						_columns.Add(box);
					}
					break;
				}
				case "table-header-group":
					if (_headerBox != null)
						_bodyrows.Add(box);
					else
						_headerBox = box;
					break;
				}
			}
			if (_headerBox != null)
				_allRows.AddRange(_headerBox.Boxes);
			_allRows.AddRange(_bodyrows);
			if (_footerBox != null)
				_allRows.AddRange(_footerBox.Boxes);
		}

		private void InsertEmptyBoxes()
		{
			if (_tableBox._tableFixed)
				return;
			int num = 0;
			List<CssBox> bodyrows = _bodyrows;
			foreach (CssBox item in bodyrows)
			{
				for (int i = 0; i < item.Boxes.Count; i++)
				{
					CssBox extendedBox = item.Boxes[i];
					int rowSpan = GetRowSpan(extendedBox);
					int num2 = GetCellRealColumnIndex(item, extendedBox);
					for (int j = num + 1; j < num + rowSpan; j++)
					{
						if (bodyrows.Count <= j)
							continue;
						int num3 = 0;
						for (int k = 0; k < bodyrows[j].Boxes.Count; k++)
						{
							if (num3 != num2)
							{
								num3++;
								num2 -= GetColSpan(bodyrows[j].Boxes[k]) - 1;
								continue;
							}
							bodyrows[j].Boxes.Insert(num3, new CssSpacingBox(_tableBox, ref extendedBox, num));
							break;
						}
					}
				}
				num++;
			}
			_tableBox._tableFixed = true;
		}

		private double CalculateCountAndWidth()
		{
			if (_columns.Count > 0)
				_columnCount = _columns.Count;
			else
			{
				foreach (CssBox allRow in _allRows)
				{
					_columnCount = Math.Max(_columnCount, allRow.Boxes.Count);
				}
			}
			_columnWidths = new double[_columnCount];
			for (int i = 0; i < _columnWidths.Length; i++)
			{
				_columnWidths[i] = double.NaN;
			}
			double availableCellWidth = GetAvailableCellWidth();
			if (_columns.Count > 0)
			{
				for (int j = 0; j < _columns.Count; j++)
				{
					CssLength cssLength = new CssLength(_columns[j].Width);
					if (cssLength.Number > 0.0)
					{
						if (cssLength.IsPercentage)
							_columnWidths[j] = CssValueParser.ParseNumber(_columns[j].Width, availableCellWidth);
						else if (cssLength.Unit == CssUnit.Pixels || cssLength.Unit == CssUnit.None)
						{
							_columnWidths[j] = cssLength.Number;
						}
					}
				}
			}
			else
			{
				foreach (CssBox allRow2 in _allRows)
				{
					for (int k = 0; k < _columnCount; k++)
					{
						if ((k >= 20 && !double.IsNaN(_columnWidths[k])) || k >= allRow2.Boxes.Count || !(allRow2.Boxes[k].Display == "table-cell"))
							continue;
						double num = CssValueParser.ParseLength(allRow2.Boxes[k].Width, availableCellWidth, allRow2.Boxes[k]);
						if (num > 0.0)
						{
							int colSpan = GetColSpan(allRow2.Boxes[k]);
							num /= (double)Convert.ToSingle(colSpan);
							for (int l = k; l < k + colSpan; l++)
							{
								_columnWidths[l] = (double.IsNaN(_columnWidths[l]) ? num : Math.Max(_columnWidths[l], num));
							}
						}
					}
				}
			}
			return availableCellWidth;
		}

		private void DetermineMissingColumnWidths(double availCellSpace)
		{
			double num = 0.0;
			if (_widthSpecified)
			{
				int num2 = 0;
				double[] columnWidths = _columnWidths;
				foreach (double num3 in columnWidths)
				{
					if (double.IsNaN(num3))
						num2++;
					else
						num += num3;
				}
				int num4 = num2;
				double[] array = null;
				if (num2 < _columnWidths.Length)
				{
					array = new double[_columnWidths.Length];
					for (int j = 0; j < _columnWidths.Length; j++)
					{
						array[j] = _columnWidths[j];
					}
				}
				if (num2 > 0)
				{
					double[] minFullWidths;
					double[] maxFullWidths;
					GetColumnsMinMaxWidthByContent(true, out minFullWidths, out maxFullWidths);
					int num5;
					do
					{
						num5 = num2;
						for (int k = 0; k < _columnWidths.Length; k++)
						{
							double num6 = (availCellSpace - num) / (double)num2;
							if (double.IsNaN(_columnWidths[k]) && num6 > maxFullWidths[k])
							{
								_columnWidths[k] = maxFullWidths[k];
								num2--;
								num += maxFullWidths[k];
							}
						}
					}
					while (num5 != num2);
					if (num2 > 0)
					{
						double num7 = (availCellSpace - num) / (double)num2;
						for (int l = 0; l < _columnWidths.Length; l++)
						{
							if (double.IsNaN(_columnWidths[l]))
								_columnWidths[l] = num7;
						}
					}
				}
				if (num2 != 0 || !(num < availCellSpace))
					return;
				if (num4 <= 0)
				{
					for (int m = 0; m < _columnWidths.Length; m++)
					{
						_columnWidths[m] += (availCellSpace - num) * (_columnWidths[m] / num);
					}
					return;
				}
				double num8 = (availCellSpace - num) / (double)num4;
				for (int n = 0; n < _columnWidths.Length; n++)
				{
					if (array == null || double.IsNaN(array[n]))
						_columnWidths[n] += num8;
				}
				return;
			}
			double[] minFullWidths2;
			double[] maxFullWidths2;
			GetColumnsMinMaxWidthByContent(true, out minFullWidths2, out maxFullWidths2);
			for (int num9 = 0; num9 < _columnWidths.Length; num9++)
			{
				if (double.IsNaN(_columnWidths[num9]))
					_columnWidths[num9] = minFullWidths2[num9];
				num += _columnWidths[num9];
			}
			for (int num10 = 0; num10 < _columnWidths.Length; num10++)
			{
				if (maxFullWidths2[num10] > _columnWidths[num10])
				{
					double num11 = _columnWidths[num10];
					_columnWidths[num10] = Math.Min(_columnWidths[num10] + (availCellSpace - num) / (double)Convert.ToSingle(_columnWidths.Length - num10), maxFullWidths2[num10]);
					num = num + _columnWidths[num10] - num11;
				}
			}
		}

		private void EnforceMaximumSize()
		{
			int i = 0;
			double widthSum = GetWidthSum();
			while (widthSum > GetAvailableTableWidth() && CanReduceWidth())
			{
				for (; !CanReduceWidth(i); i++)
				{
				}
				_columnWidths[i] -= 1.0;
				i++;
				if (i >= _columnWidths.Length)
					i = 0;
			}
			double maxTableWidth = GetMaxTableWidth();
			if (!(maxTableWidth < 90999.0))
				return;
			widthSum = GetWidthSum();
			if (!(maxTableWidth < widthSum))
				return;
			double[] minFullWidths;
			double[] maxFullWidths;
			GetColumnsMinMaxWidthByContent(false, out minFullWidths, out maxFullWidths);
			for (int j = 0; j < _columnWidths.Length; j++)
			{
				_columnWidths[j] = minFullWidths[j];
			}
			widthSum = GetWidthSum();
			if (maxTableWidth < widthSum)
			{
				for (int k = 0; k < 15; k++)
				{
					if (!(maxTableWidth < widthSum - 0.1))
						break;
					int num = 0;
					double num2 = 0.0;
					double num3 = 0.0;
					for (int l = 0; l < _columnWidths.Length; l++)
					{
						if (_columnWidths[l] > num2 + 0.1)
						{
							num3 = num2;
							num2 = _columnWidths[l];
							num = 1;
						}
						else if (_columnWidths[l] > num2 - 0.1)
						{
							num++;
						}
					}
					double num4 = ((num3 > 0.0) ? (num2 - num3) : ((widthSum - maxTableWidth) / (double)_columnWidths.Length));
					if (num4 * (double)num > widthSum - maxTableWidth)
						num4 = (widthSum - maxTableWidth) / (double)num;
					for (int m = 0; m < _columnWidths.Length; m++)
					{
						if (_columnWidths[m] > num2 - 0.1)
							_columnWidths[m] -= num4;
					}
					widthSum = GetWidthSum();
				}
				return;
			}
			for (int n = 0; n < 15; n++)
			{
				if (!(maxTableWidth > widthSum + 0.1))
					break;
				int num5 = 0;
				for (int num6 = 0; num6 < _columnWidths.Length; num6++)
				{
					if (_columnWidths[num6] + 1.0 < maxFullWidths[num6])
						num5++;
				}
				if (num5 == 0)
					num5 = _columnWidths.Length;
				bool flag = false;
				double num7 = (maxTableWidth - widthSum) / (double)num5;
				for (int num8 = 0; num8 < _columnWidths.Length; num8++)
				{
					if (_columnWidths[num8] + 0.1 < maxFullWidths[num8])
					{
						num7 = Math.Min(num7, maxFullWidths[num8] - _columnWidths[num8]);
						flag = true;
					}
				}
				for (int num9 = 0; num9 < _columnWidths.Length; num9++)
				{
					if (!flag || _columnWidths[num9] + 1.0 < maxFullWidths[num9])
						_columnWidths[num9] += num7;
				}
				widthSum = GetWidthSum();
			}
		}

		private void EnforceMinimumSize()
		{
			foreach (CssBox allRow in _allRows)
			{
				foreach (CssBox box in allRow.Boxes)
				{
					int colSpan = GetColSpan(box);
					int cellRealColumnIndex = GetCellRealColumnIndex(allRow, box);
					int num = cellRealColumnIndex + colSpan - 1;
					if (_columnWidths.Length > cellRealColumnIndex && _columnWidths[cellRealColumnIndex] < GetColumnMinWidths()[cellRealColumnIndex])
					{
						double num2 = GetColumnMinWidths()[cellRealColumnIndex] - _columnWidths[cellRealColumnIndex];
						_columnWidths[num] = GetColumnMinWidths()[num];
						if (cellRealColumnIndex < _columnWidths.Length - 1)
							_columnWidths[cellRealColumnIndex + 1] -= num2;
					}
				}
			}
		}

		private void LayoutCells(RGraphics g)
		{
			double num = Math.Max(_tableBox.ClientLeft + GetHorizontalSpacing(), 0.0);
			double num2 = Math.Max(_tableBox.ClientTop + GetVerticalSpacing(), 0.0);
			double y = num2;
			double val = num;
			double num3 = 0.0;
			int num4 = 0;
			if (_tableBox.TextAlign == "center" || _tableBox.TextAlign == "right")
			{
				double widthSum = GetWidthSum();
				num = ((_tableBox.TextAlign == "right") ? (GetAvailableTableWidth() - widthSum) : (num + (GetAvailableTableWidth() - widthSum) / 2.0));
				_tableBox.Location = new RPoint(num - _tableBox.ActualBorderLeftWidth - _tableBox.ActualPaddingLeft - GetHorizontalSpacing(), _tableBox.Location.Y);
			}
			for (int i = 0; i < _allRows.Count; i++)
			{
				CssBox cssBox = _allRows[i];
				double x = num;
				int num5 = 0;
				bool flag = false;
				for (int j = 0; j < cssBox.Boxes.Count; j++)
				{
					CssBox cssBox2 = cssBox.Boxes[j];
					if (num5 >= _columnWidths.Length)
						break;
					int rowSpan = GetRowSpan(cssBox2);
					int cellRealColumnIndex = GetCellRealColumnIndex(cssBox, cssBox2);
					double cellWidth = GetCellWidth(cellRealColumnIndex, cssBox2);
					cssBox2.Location = new RPoint(x, y);
					cssBox2.Size = new RSize(cellWidth, 0.0);
					cssBox2.PerformLayout(g);
					CssSpacingBox cssSpacingBox = cssBox2 as CssSpacingBox;
					if (cssSpacingBox != null)
					{
						if (cssSpacingBox.EndRow == num4)
							num3 = Math.Max(num3, cssSpacingBox.ExtendedBox.ActualBottom);
					}
					else if (rowSpan == 1)
					{
						num3 = Math.Max(num3, cssBox2.ActualBottom);
					}
					val = Math.Max(val, cssBox2.ActualRight);
					num5++;
					x = cssBox2.ActualRight + GetHorizontalSpacing();
				}
				foreach (CssBox box in cssBox.Boxes)
				{
					CssSpacingBox cssSpacingBox2 = box as CssSpacingBox;
					if (cssSpacingBox2 == null && GetRowSpan(box) == 1)
					{
						box.ActualBottom = num3;
						CssLayoutEngine.ApplyCellVerticalAlignment(g, box);
					}
					else if (cssSpacingBox2 != null && cssSpacingBox2.EndRow == num4)
					{
						cssSpacingBox2.ExtendedBox.ActualBottom = num3;
						CssLayoutEngine.ApplyCellVerticalAlignment(g, cssSpacingBox2.ExtendedBox);
					}
					if (_tableBox.PageBreakInside == "avoid" && (flag = box.BreakPage()))
					{
						y = box.Location.Y;
						break;
					}
				}
				if (flag)
				{
					i = ((i != 1) ? (i - 1) : (-1));
					num3 = 0.0;
				}
				else
				{
					y = num3 + GetVerticalSpacing();
					num4++;
				}
			}
			val = Math.Max(val, _tableBox.Location.X + _tableBox.ActualWidth);
			_tableBox.ActualRight = val + GetHorizontalSpacing() + _tableBox.ActualBorderRightWidth;
			_tableBox.ActualBottom = Math.Max(num3, num2) + GetVerticalSpacing() + _tableBox.ActualBorderBottomWidth;
		}

		private double GetSpannedMinWidth(CssBox row, CssBox cell, int realcolindex, int colspan)
		{
			double num = 0.0;
			for (int i = realcolindex; i < row.Boxes.Count || i < realcolindex + colspan - 1; i++)
			{
				if (i < GetColumnMinWidths().Length)
					num += GetColumnMinWidths()[i];
			}
			return num;
		}

		private static int GetCellRealColumnIndex(CssBox row, CssBox cell)
		{
			int num = 0;
			foreach (CssBox box in row.Boxes)
			{
				if (!box.Equals(cell))
				{
					num += GetColSpan(box);
					continue;
				}
				break;
			}
			return num;
		}

		private double GetCellWidth(int column, CssBox b)
		{
			double num = Convert.ToSingle(GetColSpan(b));
			double num2 = 0.0;
			for (int i = column; (double)i < (double)column + num; i++)
			{
				if (column >= _columnWidths.Length)
					break;
				if (_columnWidths.Length <= i)
					break;
				num2 += _columnWidths[i];
			}
			return num2 + (num - 1.0) * GetHorizontalSpacing();
		}

		private static int GetColSpan(CssBox b)
		{
			string attribute = b.GetAttribute("colspan", "1");
			int result;
			if (!int.TryParse(attribute, out result))
				return 1;
			return result;
		}

		private static int GetRowSpan(CssBox b)
		{
			string attribute = b.GetAttribute("rowspan", "1");
			int result;
			if (!int.TryParse(attribute, out result))
				return 1;
			return result;
		}

		private static void MeasureWords(CssBox box, RGraphics g)
		{
			if (box == null)
				return;
			foreach (CssBox box2 in box.Boxes)
			{
				box2.MeasureWordsSize(g);
				MeasureWords(box2, g);
			}
		}

		private bool CanReduceWidth()
		{
			for (int i = 0; i < _columnWidths.Length; i++)
			{
				if (CanReduceWidth(i))
					return true;
			}
			return false;
		}

		private bool CanReduceWidth(int columnIndex)
		{
			if (_columnWidths.Length >= columnIndex || GetColumnMinWidths().Length >= columnIndex)
				return false;
			return _columnWidths[columnIndex] > GetColumnMinWidths()[columnIndex];
		}

		private double GetAvailableTableWidth()
		{
			CssLength cssLength = new CssLength(_tableBox.Width);
			if (cssLength.Number > 0.0)
			{
				_widthSpecified = true;
				return CssValueParser.ParseLength(_tableBox.Width, _tableBox.ParentBox.AvailableWidth, _tableBox);
			}
			return _tableBox.ParentBox.AvailableWidth;
		}

		private double GetMaxTableWidth()
		{
			CssLength cssLength = new CssLength(_tableBox.MaxWidth);
			if (cssLength.Number > 0.0)
			{
				_widthSpecified = true;
				return CssValueParser.ParseLength(_tableBox.MaxWidth, _tableBox.ParentBox.AvailableWidth, _tableBox);
			}
			return 9999.0;
		}

		private void GetColumnsMinMaxWidthByContent(bool onlyNans, out double[] minFullWidths, out double[] maxFullWidths)
		{
			maxFullWidths = new double[_columnWidths.Length];
			minFullWidths = new double[_columnWidths.Length];
			foreach (CssBox allRow in _allRows)
			{
				for (int i = 0; i < allRow.Boxes.Count; i++)
				{
					int cellRealColumnIndex = GetCellRealColumnIndex(allRow, allRow.Boxes[i]);
					cellRealColumnIndex = ((_columnWidths.Length > cellRealColumnIndex) ? cellRealColumnIndex : (_columnWidths.Length - 1));
					if ((!onlyNans || double.IsNaN(_columnWidths[cellRealColumnIndex])) && i < allRow.Boxes.Count)
					{
						double minWidth;
						double maxWidth;
						allRow.Boxes[i].GetMinMaxWidth(out minWidth, out maxWidth);
						int colSpan = GetColSpan(allRow.Boxes[i]);
						minWidth /= (double)colSpan;
						maxWidth /= (double)colSpan;
						for (int j = 0; j < colSpan; j++)
						{
							minFullWidths[cellRealColumnIndex + j] = Math.Max(minFullWidths[cellRealColumnIndex + j], minWidth);
							maxFullWidths[cellRealColumnIndex + j] = Math.Max(maxFullWidths[cellRealColumnIndex + j], maxWidth);
						}
					}
				}
			}
		}

		private double GetAvailableCellWidth()
		{
			return GetAvailableTableWidth() - GetHorizontalSpacing() * (double)(_columnCount + 1) - _tableBox.ActualBorderLeftWidth - _tableBox.ActualBorderRightWidth;
		}

		private double GetWidthSum()
		{
			double num = 0.0;
			double[] columnWidths = _columnWidths;
			foreach (double num2 in columnWidths)
			{
				if (!double.IsNaN(num2))
				{
					num += num2;
					continue;
				}
				throw new Exception("CssTable Algorithm error: There's a NaN in column widths");
			}
			num += GetHorizontalSpacing() * (double)(_columnWidths.Length + 1);
			return num + (_tableBox.ActualBorderLeftWidth + _tableBox.ActualBorderRightWidth);
		}

		private static int GetSpan(CssBox b)
		{
			double value = CssValueParser.ParseNumber(b.GetAttribute("span"), 1.0);
			return Math.Max(1, Convert.ToInt32(value));
		}

		private double[] GetColumnMinWidths()
		{
			if (_columnMinWidths == null)
			{
				_columnMinWidths = new double[_columnWidths.Length];
				foreach (CssBox allRow in _allRows)
				{
					foreach (CssBox box in allRow.Boxes)
					{
						int colSpan = GetColSpan(box);
						int cellRealColumnIndex = GetCellRealColumnIndex(allRow, box);
						int num = Math.Min(cellRealColumnIndex + colSpan, _columnMinWidths.Length) - 1;
						double num2 = GetSpannedMinWidth(allRow, box, cellRealColumnIndex, colSpan) + (double)(colSpan - 1) * GetHorizontalSpacing();
						_columnMinWidths[num] = Math.Max(_columnMinWidths[num], box.GetMinimumWidth() - num2);
					}
				}
			}
			return _columnMinWidths;
		}

		private double GetHorizontalSpacing()
		{
			return (_tableBox.BorderCollapse == "collapse") ? (-1.0) : _tableBox.ActualBorderSpacingHorizontal;
		}

		private static double GetHorizontalSpacing(CssBox box)
		{
			return (box.BorderCollapse == "collapse") ? (-1.0) : box.ActualBorderSpacingHorizontal;
		}

		private double GetVerticalSpacing()
		{
			return (_tableBox.BorderCollapse == "collapse") ? (-1.0) : _tableBox.ActualBorderSpacingVertical;
		}
	}
}
