using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;

namespace xCollection
{
	[DebuggerStepThrough]
	internal class ThemesColorEditor : UITypeEditor
	{
		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			return base.EditValue(context, provider, value);
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			xDataGridView.xDataGridViewTheme xDataGridViewTheme = ((e.Context != null) ? ((xDataGridView.xDataGridViewTheme)e.Value) : new xDataGridView.xDataGridViewTheme());
			int num = e.Bounds.Width / 2;
			using (SolidBrush brush = new SolidBrush(xDataGridViewTheme.HeaderStyle.BackColor))
				e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, num, e.Bounds.Height - 1));
			using (SolidBrush brush2 = new SolidBrush(xDataGridViewTheme.RowsStyle.BackColor))
				e.Graphics.FillRectangle(brush2, new Rectangle(e.Bounds.X + num, e.Bounds.Y, num, e.Bounds.Height - 1));
			base.PaintValue(e);
		}
	}
}
