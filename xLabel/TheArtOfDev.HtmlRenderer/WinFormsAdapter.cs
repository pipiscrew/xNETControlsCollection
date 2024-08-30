using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.WinForms.Utilities;

namespace TheArtOfDev.HtmlRenderer.WinForms.Adapters
{
	internal sealed class WinFormsAdapter : RAdapter
	{
		private static readonly WinFormsAdapter _instance = new WinFormsAdapter();

		public static WinFormsAdapter Instance
		{
			get
			{
				return _instance;
			}
		}

		private WinFormsAdapter()
		{
			AddFontFamilyMapping("monospace", "Courier New");
			AddFontFamilyMapping("Helvetica", "Arial");
			FontFamily[] families = FontFamily.Families;
			foreach (FontFamily fontFamily in families)
			{
				AddFontFamily(new FontFamilyAdapter(fontFamily));
			}
		}

		protected override RColor GetColorInt(string colorName)
		{
			Color c = Color.FromName(colorName);
			return Utils.Convert(c);
		}

		protected override RPen CreatePen(RColor color)
		{
			return new PenAdapter(new Pen(Utils.Convert(color)));
		}

		protected override RBrush CreateSolidBrush(RColor color)
		{
			Brush brush = ((color == RColor.White) ? Brushes.White : ((color == RColor.Black) ? Brushes.Black : ((color.A >= 1) ? new SolidBrush(Utils.Convert(color)) : Brushes.Transparent)));
			return new BrushAdapter(brush, false);
		}

		protected override RBrush CreateLinearGradientBrush(RRect rect, RColor color1, RColor color2, double angle)
		{
			return new BrushAdapter(new LinearGradientBrush(Utils.Convert(rect), Utils.Convert(color1), Utils.Convert(color2), (float)angle), true);
		}

		protected override RImage ConvertImageInt(object image)
		{
			return (image != null) ? new ImageAdapter((Image)image) : null;
		}

		protected override RImage ImageFromStreamInt(Stream memoryStream)
		{
			return new ImageAdapter(Image.FromStream(memoryStream));
		}

		protected override RFont CreateFontInt(string family, double size, RFontStyle style)
		{
			return new FontAdapter(new Font(family, (float)size, (FontStyle)style));
		}

		protected override RFont CreateFontInt(RFontFamily family, double size, RFontStyle style)
		{
			return new FontAdapter(new Font(((FontFamilyAdapter)family).FontFamily, (float)size, (FontStyle)style));
		}

		protected override object GetClipboardDataObjectInt(string html, string plainText)
		{
			return ClipboardHelper.CreateDataObject(html, plainText);
		}

		protected override void SetToClipboardInt(string text)
		{
			ClipboardHelper.CopyToClipboard(text);
		}

		protected override void SetToClipboardInt(string html, string plainText)
		{
			ClipboardHelper.CopyToClipboard(html, plainText);
		}

		protected override void SetToClipboardInt(RImage image)
		{
			Clipboard.SetImage(((ImageAdapter)image).Image);
		}

		protected override RContextMenu CreateContextMenuInt()
		{
			return new ContextMenuAdapter();
		}

		protected override void SaveToFileInt(RImage image, string name, string extension, RControl control = null)
		{
			using (SaveFileDialog saveFileDialog = new SaveFileDialog())
			{
				saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg";
				saveFileDialog.FileName = name;
				saveFileDialog.DefaultExt = extension;
				DialogResult dialogResult = ((control == null) ? saveFileDialog.ShowDialog() : saveFileDialog.ShowDialog(((ControlAdapter)control).Control));
				if (dialogResult == DialogResult.OK)
					((ImageAdapter)image).Image.Save(saveFileDialog.FileName);
			}
		}
	}
}
