using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class ControlAdapter : RControl
	{
		private readonly Control _control;

		private readonly bool _useGdiPlusTextRendering;

		public Control Control
		{
			get
			{
				return _control;
			}
		}

		public override RPoint MouseLocation
		{
			get
			{
				return Utils.Convert(_control.PointToClient(Control.MousePosition));
			}
		}

		public override bool LeftMouseButton
		{
			get
			{
				return (Control.MouseButtons & MouseButtons.Left) != 0;
			}
		}

		public override bool RightMouseButton
		{
			get
			{
				return (Control.MouseButtons & MouseButtons.Right) != 0;
			}
		}

		public ControlAdapter(Control control, bool useGdiPlusTextRendering)
			: base(WinFormsAdapter.Instance)
		{
			ArgChecker.AssertArgNotNull(control, "control");
			_control = control;
			_useGdiPlusTextRendering = useGdiPlusTextRendering;
		}

		public override void SetCursorDefault()
		{
			_control.Cursor = Cursors.Default;
		}

		public override void SetCursorHand()
		{
			_control.Cursor = Cursors.Hand;
		}

		public override void SetCursorIBeam()
		{
			_control.Cursor = Cursors.IBeam;
		}

		public override void DoDragDropCopy(object dragDropData)
		{
			_control.DoDragDrop(dragDropData, DragDropEffects.Copy);
		}

		public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
		{
			using (GraphicsAdapter graphicsAdapter = new GraphicsAdapter(_control.CreateGraphics(), _useGdiPlusTextRendering, true))
				graphicsAdapter.MeasureString(str, font, maxWidth, out charFit, out charFitWidth);
		}

		public override void Invalidate()
		{
			_control.Invalidate();
		}
	}
}
