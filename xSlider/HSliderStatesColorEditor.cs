using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using xCollection;

namespace Utilities.xSlider
{
	[DebuggerStepThrough]
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class HSliderStatesColorEditor : UITypeEditor
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
            xHSlider.SliderState sliderState = ((e.Context != null) ? ((xHSlider.SliderState)e.Value) : new xHSlider.SliderState("Painter"));
			int num = e.Bounds.Width / 3;
			using (SolidBrush brush = new SolidBrush(sliderState.SliderColor))
				e.Graphics.FillRectangle(brush, new Rectangle(e.Bounds.X, e.Bounds.Y, num, e.Bounds.Height - 1));
			using (SolidBrush brush2 = new SolidBrush(sliderState.ElapsedColor))
				e.Graphics.FillRectangle(brush2, new Rectangle(e.Bounds.X + num, e.Bounds.Y, num, e.Bounds.Height - 1));
			using (SolidBrush brush3 = new SolidBrush(sliderState.ThumbColor))
				e.Graphics.FillRectangle(brush3, new Rectangle(e.Bounds.X + num * 2, e.Bounds.Y, e.Bounds.Width - num * 2, e.Bounds.Height - 1));
			base.PaintValue(e);
		}
	}
}
