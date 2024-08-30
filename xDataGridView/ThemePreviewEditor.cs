using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Utilities.xDataGridView
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[ToolboxItem(false)]
	internal class ThemePreviewEditor : Form
	{
		internal IWindowsFormsEditorService editorService;

        private xCollection.xDataGridView.PresetThemes _selectedTheme;

		private IContainer components = null;

		private Button close_button;

		private Panel panel2;

		private Panel panel1;

		private Label lblTitle;

		private Label lblDescription;

		private Button button1;

		public ListBox lstThemes;

        public xCollection.xDataGridView bdgvPreview;

		public xCollection.xDataGridView.PresetThemes CurrentTheme { get; internal set; }

        public xCollection.xDataGridView.PresetThemes SelectedTheme { get; internal set; }

		public ThemePreviewEditor()
		{
			InitializeComponent();
			base.TopLevel = false;
			bdgvPreview.IsEditor = true;
		}

		private void OnLoad(object sender, EventArgs e)
		{
            string[] names = Enum.GetNames(typeof(xCollection.xDataGridView.PresetThemes));
			bdgvPreview.PopulateWithSampleData();
			ListBox.ObjectCollection items = lstThemes.Items;
			object[] items2 = names;
			items.AddRange(items2);
			lstThemes.SelectedItem = SelectedTheme.ToString();
		}

		private void OnFormShown(object sender, EventArgs e)
		{
			bdgvPreview.Theme = SelectedTheme;
			bdgvPreview.AllowCustomTheming = true;
			bdgvPreview.CurrentTheme.RowsStyle.Font = new Font("Segoe UI Semibold", 7f, FontStyle.Bold);
			bdgvPreview.CurrentTheme.HeaderStyle.Font = new Font("Segoe UI Semibold", 8f, FontStyle.Bold);
			bdgvPreview.CurrentTheme.AlternatingRowsStyle.Font = new Font("Segoe UI Semibold", 7f, FontStyle.Bold);
			bdgvPreview.AllowCustomTheming = false;
			bdgvPreview.RowTemplate.Height = 30;
			bdgvPreview.ColumnHeadersHeight = 30;
		}

		private void OnChangeTheme(object sender, EventArgs e)
		{
            bdgvPreview.Theme = (xCollection.xDataGridView.PresetThemes)Enum.Parse(typeof(xCollection.xDataGridView.PresetThemes), lstThemes.SelectedItem.ToString());
			bdgvPreview.AllowCustomTheming = true;
			bdgvPreview.HeaderBackColor = bdgvPreview.CurrentTheme.HeaderStyle.BackColor;
			bdgvPreview.CurrentTheme.RowsStyle.Font = new Font("Segoe UI Semibold", 7f, FontStyle.Bold);
			bdgvPreview.CurrentTheme.HeaderStyle.Font = new Font("Segoe UI Semibold", 8f, FontStyle.Bold);
			bdgvPreview.CurrentTheme.AlternatingRowsStyle.Font = new Font("Segoe UI Semibold", 7f, FontStyle.Bold);
			bdgvPreview.AllowCustomTheming = false;
			bdgvPreview.RowTemplate.Height = 30;
			bdgvPreview.ColumnHeadersHeight = 30;
		}

		private void OnClickApply(object sender, EventArgs e)
		{
			editorService.CloseDropDown();
			SelectedTheme = bdgvPreview.Theme;
		}

		private void OnClickCancel(object sender, EventArgs e)
		{
			editorService.CloseDropDown();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.close_button = new System.Windows.Forms.Button();
			this.lstThemes = new System.Windows.Forms.ListBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.button1 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblTitle = new System.Windows.Forms.Label();
			this.lblDescription = new System.Windows.Forms.Label();
            this.bdgvPreview = new xCollection.xDataGridView();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)this.bdgvPreview).BeginInit();
			base.SuspendLayout();
			this.close_button.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.close_button.BackColor = System.Drawing.Color.DodgerBlue;
			this.close_button.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.close_button.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
			this.close_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.close_button.Font = new System.Drawing.Font("Segoe UI", 9f);
			this.close_button.ForeColor = System.Drawing.Color.White;
			this.close_button.Location = new System.Drawing.Point(535, 316);
			this.close_button.Name = "close_button";
			this.close_button.Size = new System.Drawing.Size(122, 26);
			this.close_button.TabIndex = 3;
			this.close_button.Text = "Apply Theme";
			this.close_button.UseVisualStyleBackColor = false;
			this.close_button.Click += new System.EventHandler(OnClickApply);
			this.lstThemes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lstThemes.FormattingEnabled = true;
			this.lstThemes.Location = new System.Drawing.Point(464, 67);
			this.lstThemes.Name = "lstThemes";
			this.lstThemes.Size = new System.Drawing.Size(189, 236);
			this.lstThemes.TabIndex = 5;
			this.lstThemes.SelectedIndexChanged += new System.EventHandler(OnChangeTheme);
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Controls.Add(this.button1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 309);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(665, 40);
			this.panel2.TabIndex = 7;
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			this.button1.BackColor = System.Drawing.Color.LightGray;
			this.button1.Cursor = System.Windows.Forms.Cursors.Arrow;
			this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.button1.Font = new System.Drawing.Font("Segoe UI", 9f);
			this.button1.Location = new System.Drawing.Point(434, 7);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(95, 26);
			this.button1.TabIndex = 8;
			this.button1.Text = "Cancel";
			this.button1.UseVisualStyleBackColor = false;
			this.button1.Click += new System.EventHandler(OnClickCancel);
			this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.panel1.Controls.Add(this.lblTitle);
			this.panel1.Controls.Add(this.lblDescription);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(665, 59);
			this.panel1.TabIndex = 6;
			this.lblTitle.AutoSize = true;
			this.lblTitle.BackColor = System.Drawing.Color.WhiteSmoke;
			this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			this.lblTitle.ForeColor = System.Drawing.Color.Black;
			this.lblTitle.Location = new System.Drawing.Point(12, 9);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Size = new System.Drawing.Size(258, 24);
			this.lblTitle.TabIndex = 6;
			this.lblTitle.Text = "Choose your preferred theme";
			this.lblDescription.AutoSize = true;
			this.lblDescription.BackColor = System.Drawing.Color.WhiteSmoke;
			this.lblDescription.ForeColor = System.Drawing.Color.DimGray;
			this.lblDescription.Location = new System.Drawing.Point(13, 33);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(385, 13);
			this.lblDescription.TabIndex = 5;
			this.lblDescription.Text = "You may select your preferred theme; after selecting, you can customize it later...";
			this.bdgvPreview.AllowCustomTheming = false;
			dataGridViewCellStyle.BackColor = System.Drawing.Color.FromArgb(248, 251, 255);
			dataGridViewCellStyle.ForeColor = System.Drawing.Color.Black;
			this.bdgvPreview.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle;
			this.bdgvPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.bdgvPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.bdgvPreview.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
			this.bdgvPreview.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.DodgerBlue;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI Semibold", 11.75f, System.Drawing.FontStyle.Bold);
			dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.bdgvPreview.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
			this.bdgvPreview.ColumnHeadersHeight = 40;
			this.bdgvPreview.CurrentTheme.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(248, 251, 255);
			this.bdgvPreview.CurrentTheme.AlternatingRowsStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold);
			this.bdgvPreview.CurrentTheme.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Black;
			this.bdgvPreview.CurrentTheme.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(210, 232, 255);
			this.bdgvPreview.CurrentTheme.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Black;
			this.bdgvPreview.CurrentTheme.BackColor = System.Drawing.Color.White;
			this.bdgvPreview.CurrentTheme.GridColor = System.Drawing.Color.FromArgb(221, 238, 255);
			this.bdgvPreview.CurrentTheme.HeaderStyle.BackColor = System.Drawing.Color.DodgerBlue;
			this.bdgvPreview.CurrentTheme.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 11.75f, System.Drawing.FontStyle.Bold);
			this.bdgvPreview.CurrentTheme.HeaderStyle.ForeColor = System.Drawing.Color.White;
			this.bdgvPreview.CurrentTheme.Name = null;
			this.bdgvPreview.CurrentTheme.RowsStyle.BackColor = System.Drawing.Color.White;
			this.bdgvPreview.CurrentTheme.RowsStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold);
			this.bdgvPreview.CurrentTheme.RowsStyle.ForeColor = System.Drawing.Color.Black;
			this.bdgvPreview.CurrentTheme.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(210, 232, 255);
			this.bdgvPreview.CurrentTheme.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75f, System.Drawing.FontStyle.Bold);
			dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(210, 232, 255);
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.bdgvPreview.DefaultCellStyle = dataGridViewCellStyle3;
			this.bdgvPreview.EnableHeadersVisualStyles = false;
			this.bdgvPreview.GridColor = System.Drawing.Color.FromArgb(221, 238, 255);
			this.bdgvPreview.HeaderBackColor = System.Drawing.Color.DodgerBlue;
			this.bdgvPreview.HeaderBgColor = System.Drawing.Color.Empty;
			this.bdgvPreview.HeaderForeColor = System.Drawing.Color.White;
			this.bdgvPreview.Location = new System.Drawing.Point(16, 67);
			this.bdgvPreview.Name = "bdgvPreview";
			this.bdgvPreview.RowHeadersVisible = false;
			this.bdgvPreview.RowTemplate.Height = 30;
			this.bdgvPreview.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.bdgvPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.bdgvPreview.Size = new System.Drawing.Size(442, 236);
			this.bdgvPreview.TabIndex = 8;
            this.bdgvPreview.Theme = xCollection.xDataGridView.PresetThemes.Light;
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new System.Drawing.Size(665, 349);
			base.ControlBox = false;
			base.Controls.Add(this.bdgvPreview);
			base.Controls.Add(this.lstThemes);
			base.Controls.Add(this.close_button);
			base.Controls.Add(this.panel1);
			base.Controls.Add(this.panel2);
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "ThemePreviewEditor";
			base.ShowIcon = false;
			base.ShowInTaskbar = false;
			this.Text = "RotationEditor";
			base.Load += new System.EventHandler(OnLoad);
			base.Shown += new System.EventHandler(OnFormShown);
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)this.bdgvPreview).EndInit();
			base.ResumeLayout(false);
		}
	}
}
