using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Utilities.xSnackbar
{
	internal static class Shadower
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		public struct MARGINS
		{
			public int leftWidth;

			public int rightWidth;

			public int topHeight;

			public int bottomHeight;
		}

		private static bool _isAeroEnabled;

		private static bool _isDraggingEnabled;

		private const int WM_NCHITTEST = 132;

		private const int WS_MINIMIZEBOX = 131072;

		private const int HTCLIENT = 1;

		private const int HTCAPTION = 2;

		private const int CS_DBLCLKS = 8;

		private const int CS_DROPSHADOW = 131072;

		private const int WM_NCPAINT = 133;

		private const int WM_ACTIVATEAPP = 28;

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static extern int DwmIsCompositionEnabled(ref int pfEnabled);

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static bool IsCompositionEnabled()
		{
			if (Environment.OSVersion.Version.Major < 6)
				return false;
			bool enabled;
			DwmIsCompositionEnabled(out enabled);
			return enabled;
		}

		public static void ApplyShadows(Form form)
		{
			int attrValue = 2;
			DwmSetWindowAttribute(form.Handle, 2, ref attrValue, 4);
			MARGINS mARGINS = default(MARGINS);
			mARGINS.bottomHeight = 1;
			mARGINS.leftWidth = 0;
			mARGINS.rightWidth = 0;
			mARGINS.topHeight = 0;
			MARGINS pMarInset = mARGINS;
			DwmExtendFrameIntoClientArea(form.Handle, ref pMarInset);
		}

		[DllImport("dwmapi.dll")]
		private static extern int DwmIsCompositionEnabled(out bool enabled);

		[DllImport("Gdi32.dll")]
		private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

		private static bool CheckIfAeroIsEnabled()
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				int pfEnabled = 0;
				DwmIsCompositionEnabled(ref pfEnabled);
				return pfEnabled == 1;
			}
			return false;
		}
	}
}
