using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Adapters
{
	public abstract class RControl
	{
		private readonly RAdapter _adapter;

		public RAdapter Adapter
		{
			get
			{
				return _adapter;
			}
		}

		public abstract bool LeftMouseButton { get; }

		public abstract bool RightMouseButton { get; }

		public abstract RPoint MouseLocation { get; }

		protected RControl(RAdapter adapter)
		{
			ArgChecker.AssertArgNotNull(adapter, "adapter");
			_adapter = adapter;
		}

		public abstract void SetCursorDefault();

		public abstract void SetCursorHand();

		public abstract void SetCursorIBeam();

		public abstract void DoDragDropCopy(object dragDropData);

		public abstract void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth);

		public abstract void Invalidate();
	}
}
