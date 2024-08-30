using System.Collections.Generic;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class CssSpacingBox : CssBox
	{
		private readonly CssBox _extendedBox;

		private readonly int _startRow;

		private readonly int _endRow;

		public CssBox ExtendedBox
		{
			get
			{
				return _extendedBox;
			}
		}

		public int StartRow
		{
			get
			{
				return _startRow;
			}
		}

		public int EndRow
		{
			get
			{
				return _endRow;
			}
		}

		public CssSpacingBox(CssBox tableBox, ref CssBox extendedBox, int startRow)
			: base(tableBox, new HtmlTag("none", false, new Dictionary<string, string> { { "colspan", "1" } }))
		{
			_extendedBox = extendedBox;
			base.Display = "none";
			_startRow = startRow;
			_endRow = startRow + int.Parse(extendedBox.GetAttribute("rowspan", "1")) - 1;
		}
	}
}
