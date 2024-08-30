using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace xCollection
{
	internal class StatesColorEditor : UITypeEditor
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
			CheckBoxState checkBoxState = ((e.Context != null) ? ((CheckBoxState)e.Value) : new CheckBoxState("Painter"));
			int num = e.Bounds.Width / 3;
			using (SolidBrush brush = new SolidBrush(checkBoxState.BorderColor))
				e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, num, e.Bounds.Height - 1));
			using (SolidBrush brush2 = new SolidBrush(checkBoxState.CheckBoxColor))
				e.Graphics.FillRectangle(brush2, new Rectangle(e.Bounds.X + num, e.Bounds.Y, num, e.Bounds.Height - 1));
			using (SolidBrush brush3 = new SolidBrush(checkBoxState.CheckmarkColor))
				e.Graphics.FillRectangle(brush3, new Rectangle(e.Bounds.X + num * 2, e.Bounds.Y, e.Bounds.Width - num * 2, e.Bounds.Height - 1));
			base.PaintValue(e);
		}
	}
}
