using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace TheArtOfDev.HtmlRenderer.WinForms.Utilities
{
	internal static class Win32Utils
	{
		public const int WsBorder = 8388608;

		public const int WsExClientEdge = 512;

		public const int BitBltCopy = 13369376;

		public const int BitBltPaint = 15597702;

		public const int WmSetCursor = 32;

		public const int IdcHand = 32649;

		public const int TextAlignDefault = 0;

		public const int TextAlignRtl = 256;

		public const int TextAlignBaseline = 24;

		public const int TextAlignBaselineRtl = 280;

		[DllImport("user32.dll")]
		public static extern int SetCursor(int hCursor);

		[DllImport("user32.dll")]
		public static extern int LoadCursor(int hInstance, int lpCursorName);

		public static IntPtr CreateMemoryHdc(IntPtr hdc, int width, int height, out IntPtr dib)
		{
			IntPtr intPtr = CreateCompatibleDC(hdc);
			SetBkMode(intPtr, 1);
			BitMapInfo pbmi = default(BitMapInfo);
			pbmi.biSize = Marshal.SizeOf((object)pbmi);
			pbmi.biWidth = width;
			pbmi.biHeight = -height;
			pbmi.biPlanes = 1;
			pbmi.biBitCount = 32;
			pbmi.biCompression = 0;
			IntPtr ppvBits;
			dib = CreateDIBSection(hdc, ref pbmi, 0u, out ppvBits, IntPtr.Zero, 0u);
			SelectObject(intPtr, dib);
			return intPtr;
		}

		public static void ReleaseMemoryHdc(IntPtr memoryHdc, IntPtr dib)
		{
			DeleteObject(dib);
			DeleteDC(memoryHdc);
		}

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromDC(IntPtr hdc);

		[DllImport("User32", SetLastError = true)]
		public static extern int GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

		public static Rectangle GetWindowRectangle(IntPtr handle)
		{
			Rectangle lpRect;
			GetWindowRect(handle, out lpRect);
			return new Rectangle(lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
		}

		[DllImport("User32.dll")]
		public static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

		[DllImport("gdi32.dll")]
		public static extern int SetTextAlign(IntPtr hdc, uint fMode);

		[DllImport("gdi32.dll")]
		public static extern int SetBkMode(IntPtr hdc, int mode);

		[DllImport("gdi32.dll")]
		public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiObj);

		[DllImport("gdi32.dll")]
		public static extern uint SetTextColor(IntPtr hdc, int color);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode)]
		public static extern bool GetTextMetrics(IntPtr hdc, out TextMetric lptm);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTextExtentPoint32W")]
		public static extern int GetTextExtentPoint32(IntPtr hdc, [MarshalAs(UnmanagedType.LPWStr)] string str, int len, ref Size size);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetTextExtentExPointW")]
		public static extern bool GetTextExtentExPoint(IntPtr hDc, [MarshalAs(UnmanagedType.LPWStr)] string str, int nLength, int nMaxExtent, int[] lpnFit, int[] alpDx, ref Size size);

		[DllImport("gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "TextOutW")]
		public static extern bool TextOut(IntPtr hdc, int x, int y, [MarshalAs(UnmanagedType.LPWStr)] string str, int len);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

		[DllImport("gdi32.dll")]
		public static extern int GetClipBox(IntPtr hdc, out Rectangle lprc);

		[DllImport("gdi32.dll")]
		public static extern int SelectClipRgn(IntPtr hdc, IntPtr hrgn);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, uint dwRop);

		[DllImport("gdi32.dll", EntryPoint = "GdiAlphaBlend")]
		public static extern bool AlphaBlend(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, BlendFunction blendFunction);

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern bool DeleteDC(IntPtr hdc);

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDIBSection(IntPtr hdc, [In] ref BitMapInfo pbmi, uint iUsage, out IntPtr ppvBits, IntPtr hSection, uint dwOffset);
	}
}
