using System.Drawing;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class BrushAdapter : RBrush
	{
		private readonly Brush _brush;

		private readonly bool _dispose;

		public Brush Brush
		{
			get
			{
				return _brush;
			}
		}

		public BrushAdapter(Brush brush, bool dispose)
		{
			_brush = brush;
			_dispose = dispose;
		}

		public override void Dispose()
		{
			if (_dispose)
				_brush.Dispose();
		}
	}
}
