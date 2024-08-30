using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Utilities.xSnackbar
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	[Browsable(false)]
	[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
	public class WindowHandler : NativeWindow
	{
		private const int CS_DBLCLKS = 8;

		private const int WS_MINIMIZEBOX = 131072;

		private bool _allowResizing;

		private Form _parentForm;

		private xFormDock _xFormDock;

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		internal xFormDock xFormDock
		{
			get
			{
				return _xFormDock;
			}
			set
			{
				_xFormDock = value;
			}
		}

		public WindowHandler(Form parent, bool allowResizing)
		{
			try
			{
				_parentForm = parent;
				_allowResizing = allowResizing;
				if (allowResizing)
					AssignHandle(parent.Handle);
			}
			catch (Exception)
			{
			}
		}

		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message m)
		{
			try
			{
				if (_allowResizing && m.Msg == 132)
				{
					int x = m.LParam.ToInt32() & 0xFFFF;
					int y = m.LParam.ToInt32() >> 16;
					int num = 0;
					int num2 = 0;
					Point point = _parentForm.PointToClient(new Point(x, y));
					num2 = ((point.Y < 5) ? 2 : ((point.Y > _parentForm.ClientSize.Height - 5) ? 1 : 0));
					num = ((point.X < 5) ? 2 : ((point.X > _parentForm.ClientSize.Width - 5) ? 1 : 0));
					switch (num + (num2 << 2))
					{
					case 1:
						m.Result = new IntPtr(11);
						return;
					case 2:
						m.Result = new IntPtr(10);
						return;
					case 4:
						m.Result = new IntPtr(15);
						return;
					case 5:
						m.Result = new IntPtr(17);
						return;
					case 6:
						m.Result = new IntPtr(16);
						return;
					case 8:
						m.Result = new IntPtr(12);
						return;
					case 9:
						m.Result = new IntPtr(14);
						return;
					case 10:
						m.Result = new IntPtr(13);
						return;
					}
				}
				base.WndProc(ref m);
			}
			catch (Exception)
			{
			}
		}
	}
}
