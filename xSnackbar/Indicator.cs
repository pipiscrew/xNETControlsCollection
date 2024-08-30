using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Utilities.xSnackbar
{
	[Browsable(false)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DebuggerStepThrough]
	internal class Indicator : Form
	{
		private IContainer components = null;

		public Indicator()
		{
			InitializeComponent();
		}

		private void SnapArea_Load(object sender, EventArgs e)
		{
		}

		private void SnapArea_Shown(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Utilities.xSnackbar.Indicator));
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(183, 160);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			base.KeyPreview = true;
			base.Name = "SnapArea";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			base.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Indicator";
			base.Load += new System.EventHandler(SnapArea_Load);
			base.Shown += new System.EventHandler(SnapArea_Shown);
			base.ResumeLayout(false);
		}
	}
}
