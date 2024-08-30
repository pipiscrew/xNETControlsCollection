using System.ComponentModel;
using System.Diagnostics;

namespace Utilities.xSnackbar
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Description("Provides a list of supported docking options in Windows Forms.")]
	public class DockOptions
	{
		private bool _dockAll;

		private bool _dockLeft;

		private bool _dockRight;

		private bool _dockTopLeft;

		private bool _dockTopRight;

		private bool _dockFullScreen;

		private bool _dockBottomLeft;

		private bool _dockBottomRight;

		[ParenthesizePropertyName(true)]
		[Description("Sets a value indicating whether the parent form can be docked to all supported screen areas.")]
		[Category("x Properties")]
		public bool DockAll
		{
			get
			{
				return _dockAll;
			}
			set
			{
				_dockAll = value;
				DockLeft = value;
				DockRight = value;
				DockTopLeft = value;
				DockTopRight = value;
				DockBottomLeft = value;
				DockBottomRight = value;
				DockFullScreen = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form can be docked to the left screen area.")]
		public bool DockLeft
		{
			get
			{
				return _dockLeft;
			}
			set
			{
				_dockLeft = value;
			}
		}

		[Description("Sets a value indicating whether the parent form can be docked to the right screen area.")]
		[Category("x Properties")]
		public bool DockRight
		{
			get
			{
				return _dockRight;
			}
			set
			{
				_dockRight = value;
			}
		}

		[Description("Sets a value indicating whether the parent form can be docked to fill the screen's dimensions.")]
		[Category("x Properties")]
		public bool DockFullScreen
		{
			get
			{
				return _dockFullScreen;
			}
			set
			{
				_dockFullScreen = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form can be docked to the top-left screen area.")]
		public bool DockTopLeft
		{
			get
			{
				return _dockTopLeft;
			}
			set
			{
				_dockTopLeft = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the parent form can be docked to the top-right screen area.")]
		public bool DockTopRight
		{
			get
			{
				return _dockTopRight;
			}
			set
			{
				_dockTopRight = value;
			}
		}

		[Description("Sets a value indicating whether the parent form can be docked to the bottom-left screen area.")]
		[Category("x Properties")]
		public bool DockBottomLeft
		{
			get
			{
				return _dockBottomLeft;
			}
			set
			{
				_dockBottomLeft = value;
			}
		}

		[Description("Sets a value indicating whether the parent form can be docked to the bottom-right screen area.")]
		[Category("x Properties")]
		public bool DockBottomRight
		{
			get
			{
				return _dockBottomRight;
			}
			set
			{
				_dockBottomRight = value;
			}
		}

		public DockOptions()
		{
			_dockAll = true;
		}

		public override string ToString()
		{
			return "Full: " + DockFullScreen + "; Left: " + DockLeft + "; Right: " + DockRight + "; Top-left: " + DockTopLeft + "; Top-right: " + DockTopRight + "; Bottom-left: " + DockBottomLeft + "; Bottom-right: " + DockBottomRight;
		}
	}
}
