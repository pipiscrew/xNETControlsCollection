using System;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class ContextMenuAdapter : RContextMenu
	{
		private readonly ContextMenuStrip _contextMenu;

		public override int ItemsCount
		{
			get
			{
				return _contextMenu.Items.Count;
			}
		}

		public ContextMenuAdapter()
		{
			_contextMenu = new ContextMenuStrip();
			_contextMenu.ShowImageMargin = false;
		}

		public override void AddDivider()
		{
			_contextMenu.Items.Add("-");
		}

		public override void AddItem(string text, bool enabled, EventHandler onClick)
		{
			ArgChecker.AssertArgNotNullOrEmpty(text, "text");
			ArgChecker.AssertArgNotNull(onClick, "onClick");
			ToolStripItem toolStripItem = _contextMenu.Items.Add(text, null, onClick);
			toolStripItem.Enabled = enabled;
		}

		public override void RemoveLastDivider()
		{
			if (_contextMenu.Items[_contextMenu.Items.Count - 1].Text == string.Empty)
				_contextMenu.Items.RemoveAt(_contextMenu.Items.Count - 1);
		}

		public override void Show(RControl parent, RPoint location)
		{
			_contextMenu.Show(((ControlAdapter)parent).Control, Utils.ConvertRound(location));
		}

		public override void Dispose()
		{
			_contextMenu.Dispose();
		}
	}
}
