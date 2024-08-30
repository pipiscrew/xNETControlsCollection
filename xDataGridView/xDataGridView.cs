using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace xCollection
{
	[Description("Provides more enhanced data display experiences with extended visualization features and options.")]
	[DebuggerStepThrough]
	[Category("x UI For Windows Forms")]
	[ToolboxBitmap(typeof(DataGrid))]
	public class xDataGridView : DataGridView
	{
		public enum PresetThemes
		{
			Dark,
			Light,
			Crimson,
			LimeGreen,
			Yellow,
			Orange,
			Maroon,
			Chocolate,
			DodgerBlue,
			Navy,
			MediumSeaGreen,
			Teal,
			DarkSlateGray,
			ForestGreen,
			DarkViolet,
			Purple,
			MediumVioletRed
		}

		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class xDataGridViewTheme
		{
			private RowThemingStyles _defaultRowStyle = new RowThemingStyles();

			private HeaderThemingStyles _headerStyle = new HeaderThemingStyles();

			private RowThemingStyles _alternatingRowsStyle = new RowThemingStyles();

			[Browsable(false)]
			public string Name { get; set; }

			[ParenthesizePropertyName(true)]
			[Description("Gets or sets the theme's BackColor.")]
			public Color BackColor { get; set; }

			[ParenthesizePropertyName(true)]
			[Description("Gets or sets the theme's GridColor.")]
			public Color GridColor { get; set; }

			[Description("Gets or sets the theme's default rows style options.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
			public RowThemingStyles RowsStyle
			{
				get
				{
					return _defaultRowStyle;
				}
				set
				{
					_defaultRowStyle = value;
				}
			}

			[Description("Gets or sets the theme's alterating rows style options.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
			public RowThemingStyles AlternatingRowsStyle
			{
				get
				{
					return _alternatingRowsStyle;
				}
				set
				{
					_alternatingRowsStyle = value;
				}
			}

			[Description("Gets or sets the theme's header style options.")]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
			public HeaderThemingStyles HeaderStyle
			{
				get
				{
					return _headerStyle;
				}
				set
				{
					_headerStyle = value;
				}
			}

			public override string ToString()
			{
				return string.Format("BackColor: {0}; GridColor: {1};", BackColor, GridColor);
			}
		}

		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class HeaderThemingStyles
		{
			[Description("Gets or sets the theme's header font.")]
			public Font Font { get; set; }

			[Description("Gets or sets the theme's header ForeColor.")]
			public Color ForeColor { get; set; }

			[Description("Gets or sets the theme's header BackColor.")]
			public Color BackColor { get; set; }

			[Description("Gets or sets the theme's header selection ForeColor.")]
			public Color SelectionForeColor { get; set; }

			[Description("Gets or sets the theme's header selection BackColor.")]
			public Color SelectionBackColor { get; set; }

			public override string ToString()
			{
				return string.Format("ForeColor: {0}; BackColor: {1}; ", ForeColor, BackColor) + string.Format("SelectionForeColor: {0}; ", SelectionForeColor) + string.Format("SelectionBackColor: {0}; Font: {1}", SelectionBackColor, Font);
			}
		}

		[DebuggerStepThrough]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public class RowThemingStyles
		{
			[Description("Gets or sets the theme's row Font.")]
			public Font Font { get; set; }

			[Description("Gets or sets the theme's row ForeColor.")]
			public Color ForeColor { get; set; }

			[Description("Gets or sets the theme's row BackColor.")]
			public Color BackColor { get; set; }

			[Description("Gets or sets the theme's row selection ForeColor.")]
			public Color SelectionForeColor { get; set; }

			[Description("Gets or sets the theme's row selection BackColor.")]
			public Color SelectionBackColor { get; set; }

			public override string ToString()
			{
				return string.Format("ForeColor: {0}; BackColor: {1}; SelectionForeColor: {2}; ", ForeColor, BackColor, SelectionForeColor) + string.Format("SelectionBackColor: {0}; Font: {1}; ", SelectionBackColor, Font);
			}
		}

		private Color _headerForeColor;

		private Color _headerBackColor;

		private PresetThemes _presetTheme = PresetThemes.Light;

		private xDataGridViewTheme _currentTheme = new xDataGridViewTheme();

		private IContainer components = null;

		[Category("x Properties")]
		[Description("When enabled, the currently applied theme will be customizable using the 'CurrentTheme' property; otherwise otherwise, the currently applied theme won't be customizable.")]
		public bool AllowCustomTheming { get; set; }

		[DefaultValue(true)]
		[DisplayName("Theme")]
		[Editor(typeof(ThemePreviewUIEditor), typeof(UITypeEditor))]
		[Description("Lets you choose and apply a preset theme from the current list of preset themes.")]
		[Category("x Properties")]
		public PresetThemes Theme
		{
			get
			{
				return _presetTheme;
			}
			set
			{
				_presetTheme = value;
				if (!AllowCustomTheming)
					ApplyPresetTheme(_presetTheme);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Category("x Properties")]
		[Description("Previews the current theme and its applied style properties. You can change the applied theme's options right from here.")]
		[Editor(typeof(ThemesColorEditor), typeof(UITypeEditor))]
		public xDataGridViewTheme CurrentTheme
		{
			get
			{
				if (AllowCustomTheming)
				{
					ApplyTheme(_currentTheme);
					_headerBackColor = _currentTheme.HeaderStyle.BackColor;
					_headerForeColor = _currentTheme.HeaderStyle.ForeColor;
				}
				return _currentTheme;
			}
		}

		[DefaultValue(true)]
		[Category("x Properties")]
		[Description("Sets the header's background color.")]
		[RefreshProperties(RefreshProperties.Repaint)]
		public Color HeaderBackColor
		{
			get
			{
				return _headerBackColor;
			}
			set
			{
				_headerBackColor = value;
				_currentTheme.HeaderStyle.BackColor = value;
			}
		}

		[DefaultValue(true)]
		[Category("x Properties")]
		[RefreshProperties(RefreshProperties.Repaint)]
		[Description("Sets the header's fore color.")]
		public Color HeaderForeColor
		{
			get
			{
				return _headerForeColor;
			}
			set
			{
				_headerForeColor = value;
				_currentTheme.HeaderStyle.ForeColor = value;
			}
		}

		[Browsable(false)]
		[Obsolete("This property has been deprecated. Use HeaderBackColor instead.")]
		public Color HeaderBgColor { get; set; }

		internal bool Loaded { get; set; }

		internal bool IsEditor { get; set; }

		[Description("Displays data in a customizable grid with additional theme styles and enhancements.")]
		public xDataGridView()
		{
			InitializeComponent();
			SetDefaults();
		}

		public void PopulateWithSampleData()
		{
			try
			{
				base.Columns.Add("FullName", "       Full Name");
				base.Columns.Add("Age", "       Age");
				base.Columns.Add("JobTitle", "       Job Title");
				base.Columns.Add("Location", "       Location");
				base.Rows.Add("       Vincent Williamson", "       31", "       iOS Developer", "       Washington");
				base.Rows.Add("       Tyler Reyes", "       22", "       UI/UX Designer", "       New York");
				base.Rows.Add("       Justin Black", "       26", "       Front End Developer", "       Los Angeles");
				base.Rows.Add("       Sean Guzman", "       25", "       Web Designer", "       San Francisco");
				base.Rows.Add("       James Snuggles", "       42", "       AI Engineer", "       Seattle");
				base.Rows.Add("       Don Parveyer", "       31", "       Animations Designer", "       Oklahoma");
				base.Rows.Add("       Lisa Grandeur", "       27", "       UI/UX Creative", "       Tennessee");
				base.Rows.Add("       Josh Alonso", "       22", "       Motion Graphics", "       Connecticut");
				base.Rows.Add("       Evangeline Myers", "       25", "       Programmer/Analyst", "       New Jersey");
			}
			catch (Exception)
			{
			}
		}

		public void ApplyTheme(xDataGridViewTheme theme)
		{
			_currentTheme = theme;
			base.GridColor = theme.GridColor;
			_headerBackColor = theme.HeaderStyle.BackColor;
			_headerForeColor = theme.HeaderStyle.ForeColor;
			base.ColumnHeadersDefaultCellStyle.BackColor = theme.HeaderStyle.BackColor;
			base.ColumnHeadersDefaultCellStyle.ForeColor = theme.HeaderStyle.ForeColor;
			base.ColumnHeadersDefaultCellStyle.SelectionBackColor = theme.HeaderStyle.SelectionBackColor;
			base.ColumnHeadersDefaultCellStyle.SelectionForeColor = theme.HeaderStyle.SelectionForeColor;
			base.ColumnHeadersDefaultCellStyle.Font = theme.HeaderStyle.Font;
			base.DefaultCellStyle.Font = theme.RowsStyle.Font;
			base.DefaultCellStyle.ForeColor = theme.RowsStyle.ForeColor;
			base.DefaultCellStyle.BackColor = theme.RowsStyle.BackColor;
			base.DefaultCellStyle.SelectionBackColor = theme.RowsStyle.SelectionBackColor;
			base.DefaultCellStyle.SelectionForeColor = theme.RowsStyle.SelectionForeColor;
			base.AlternatingRowsDefaultCellStyle.BackColor = theme.AlternatingRowsStyle.BackColor;
			base.AlternatingRowsDefaultCellStyle.ForeColor = theme.AlternatingRowsStyle.ForeColor;
		}

		private void SetDefaults()
		{
			try
			{
				DoubleBuffered = true;
				base.RowHeadersVisible = false;
				base.BorderStyle = BorderStyle.None;
				base.EnableHeadersVisualStyles = false;
				base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
				base.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
				base.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
				base.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
				if (!AllowCustomTheming)
					Theme = PresetThemes.Light;
			}
			catch (Exception)
			{
			}
		}

		internal void PopulateWithPreviewData()
		{
			try
			{
				base.Columns.Add("FullName", "       Full Name");
				base.Columns.Add("Age", "       Age");
				base.Columns.Add("JobTitle", "       Job Title");
				base.Columns.Add("Location", "       Location");
				base.Rows.Add("       Vincent Williamson", "       31", "       iOS Developer", "       Washington");
				base.Rows.Add("       Tyler Reyes", "       22", "       UI/UX Designer", "       New York");
				base.Rows.Add("       Justin Black", "       26", "       Front End Developer", "       Los Angeles");
				base.Rows.Add("       Sean Guzman", "       25", "       Web Designer", "       San Francisco");
				base.Rows.Add("       James Snuggles", "       42", "       AI Engineer", "       Seattle");
			}
			catch (Exception)
			{
			}
		}

		internal xDataGridViewTheme ApplyPresetTheme(PresetThemes presetTheme, bool applyHeightPreferences = true)
		{
			try
			{
				if (applyHeightPreferences)
				{
					base.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
					base.ColumnHeadersHeight = 40;
					base.RowTemplate.Height = 40;
				}
				xDataGridViewTheme xDataGridViewTheme = new xDataGridViewTheme
				{
					BackColor = Color.FromArgb(15, 16, 18),
					GridColor = Color.FromArgb(50, 56, 62),
					HeaderStyle = new HeaderThemingStyles
					{
						ForeColor = Color.White,
						BackColor = Color.FromArgb(15, 16, 18),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.FromArgb(15, 16, 18).LightenBy(20),
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold)
					},
					RowsStyle = new RowThemingStyles
					{
						ForeColor = Color.White,
						SelectionBackColor = Color.FromArgb(114, 117, 119),
						SelectionForeColor = Color.WhiteSmoke,
						BackColor = Color.FromArgb(33, 37, 41),
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						ForeColor = Color.White,
						SelectionBackColor = Color.FromArgb(114, 117, 119),
						SelectionForeColor = Color.WhiteSmoke,
						BackColor = Color.FromArgb(44, 48, 52),
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold)
					}
				};
				xDataGridViewTheme xDataGridViewTheme2 = new xDataGridViewTheme
				{
					BackColor = Color.White,
					GridColor = Color.FromArgb(221, 238, 255),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.DodgerBlue,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DodgerBlue.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.White,
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.FromArgb(210, 232, 255)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						SelectionForeColor = Color.Black,
						BackColor = Color.DodgerBlue.LightenBy(97),
						SelectionBackColor = Color.FromArgb(210, 232, 255)
					}
				};
				xDataGridViewTheme xDataGridViewTheme3 = new xDataGridViewTheme
				{
					BackColor = Color.Crimson,
					GridColor = Color.Crimson.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Crimson,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Crimson.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Crimson.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Crimson.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Crimson.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Crimson.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme4 = new xDataGridViewTheme
				{
					BackColor = Color.LimeGreen,
					GridColor = Color.LimeGreen.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.LimeGreen,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.LimeGreen.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.LimeGreen.LightenBy(80),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.LimeGreen.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.LimeGreen.LightenBy(75),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.LimeGreen.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme5 = new xDataGridViewTheme
				{
					BackColor = Color.Yellow,
					GridColor = Color.Yellow.LightenBy(60),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Yellow,
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Yellow.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Yellow.LightenBy(80),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Yellow.DarkenBy(20)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Yellow.LightenBy(75),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Yellow.DarkenBy(20)
					}
				};
				xDataGridViewTheme xDataGridViewTheme6 = new xDataGridViewTheme
				{
					BackColor = Color.Orange,
					GridColor = Color.Orange.LightenBy(68),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Orange,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Orange.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Orange.LightenBy(80),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Orange.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Orange.LightenBy(75),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Orange.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme7 = new xDataGridViewTheme
				{
					BackColor = Color.Maroon,
					GridColor = Color.Maroon.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Maroon,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Maroon.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Maroon.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Maroon.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Maroon.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Maroon.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme8 = new xDataGridViewTheme
				{
					BackColor = Color.Chocolate,
					GridColor = Color.Chocolate.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Chocolate,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Chocolate.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Chocolate.LightenBy(80),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Chocolate.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Chocolate.LightenBy(75),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.Chocolate.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme9 = new xDataGridViewTheme
				{
					BackColor = Color.DodgerBlue,
					GridColor = Color.DodgerBlue.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.DodgerBlue,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DodgerBlue.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DodgerBlue.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DodgerBlue.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DodgerBlue.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DodgerBlue.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme10 = new xDataGridViewTheme
				{
					BackColor = Color.Navy,
					GridColor = Color.Navy.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Navy,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Navy.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Navy.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Navy.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Navy.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Navy.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme11 = new xDataGridViewTheme
				{
					BackColor = Color.MediumSeaGreen,
					GridColor = Color.MediumSeaGreen.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.MediumSeaGreen,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.MediumSeaGreen.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.MediumSeaGreen.LightenBy(80),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.MediumSeaGreen.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.MediumSeaGreen.LightenBy(75),
						SelectionForeColor = Color.Black,
						SelectionBackColor = Color.MediumSeaGreen.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme12 = new xDataGridViewTheme
				{
					BackColor = Color.Teal,
					GridColor = Color.Teal.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Teal,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Teal.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Teal.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Teal.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Teal.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Teal.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme13 = new xDataGridViewTheme
				{
					BackColor = Color.DarkSlateGray,
					GridColor = Color.DarkSlateGray.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.DarkSlateGray,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkSlateGray.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DarkSlateGray.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkSlateGray.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DarkSlateGray.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkSlateGray.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme14 = new xDataGridViewTheme
				{
					BackColor = Color.ForestGreen,
					GridColor = Color.ForestGreen.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.ForestGreen,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.ForestGreen.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.ForestGreen.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.ForestGreen.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.ForestGreen.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.ForestGreen.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme15 = new xDataGridViewTheme
				{
					BackColor = Color.DarkViolet,
					GridColor = Color.DarkViolet.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.DarkViolet,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkViolet.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DarkViolet.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkViolet.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.DarkViolet.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.DarkViolet.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme16 = new xDataGridViewTheme
				{
					BackColor = Color.Purple,
					GridColor = Color.Purple.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.Purple,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Purple.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Purple.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Purple.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.Purple.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.Purple.LightenBy(40)
					}
				};
				xDataGridViewTheme xDataGridViewTheme17 = new xDataGridViewTheme
				{
					BackColor = Color.MediumVioletRed,
					GridColor = Color.MediumVioletRed.LightenBy(70),
					HeaderStyle = new HeaderThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 11.75f, FontStyle.Bold),
						ForeColor = Color.White,
						BackColor = Color.MediumVioletRed,
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.MediumVioletRed.DarkenBy(20)
					},
					RowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.MediumVioletRed.LightenBy(80),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.MediumVioletRed.LightenBy(40)
					},
					AlternatingRowsStyle = new RowThemingStyles
					{
						Font = new Font("Segoe UI Semibold", 9.75f, FontStyle.Bold),
						ForeColor = Color.Black,
						BackColor = Color.MediumVioletRed.LightenBy(75),
						SelectionForeColor = Color.White,
						SelectionBackColor = Color.MediumVioletRed.LightenBy(40)
					}
				};
				switch (presetTheme)
				{
				case PresetThemes.Dark:
					ApplyTheme(xDataGridViewTheme);
					return xDataGridViewTheme;
				case PresetThemes.Light:
					ApplyTheme(xDataGridViewTheme2);
					return xDataGridViewTheme2;
				case PresetThemes.Crimson:
					ApplyTheme(xDataGridViewTheme3);
					return xDataGridViewTheme3;
				case PresetThemes.LimeGreen:
					ApplyTheme(xDataGridViewTheme4);
					return xDataGridViewTheme4;
				case PresetThemes.Yellow:
					ApplyTheme(xDataGridViewTheme5);
					return xDataGridViewTheme5;
				case PresetThemes.Orange:
					ApplyTheme(xDataGridViewTheme6);
					return xDataGridViewTheme6;
				case PresetThemes.Maroon:
					ApplyTheme(xDataGridViewTheme7);
					return xDataGridViewTheme7;
				case PresetThemes.Chocolate:
					ApplyTheme(xDataGridViewTheme8);
					return xDataGridViewTheme8;
				case PresetThemes.DodgerBlue:
					ApplyTheme(xDataGridViewTheme9);
					return xDataGridViewTheme9;
				case PresetThemes.Navy:
					ApplyTheme(xDataGridViewTheme10);
					return xDataGridViewTheme10;
				case PresetThemes.MediumSeaGreen:
					ApplyTheme(xDataGridViewTheme11);
					return xDataGridViewTheme11;
				case PresetThemes.Teal:
					ApplyTheme(xDataGridViewTheme12);
					return xDataGridViewTheme12;
				case PresetThemes.DarkSlateGray:
					ApplyTheme(xDataGridViewTheme13);
					return xDataGridViewTheme13;
				case PresetThemes.ForestGreen:
					ApplyTheme(xDataGridViewTheme14);
					return xDataGridViewTheme14;
				case PresetThemes.DarkViolet:
					ApplyTheme(xDataGridViewTheme15);
					return xDataGridViewTheme15;
				case PresetThemes.Purple:
					ApplyTheme(xDataGridViewTheme16);
					return xDataGridViewTheme16;
				case PresetThemes.MediumVioletRed:
					ApplyTheme(xDataGridViewTheme17);
					return xDataGridViewTheme17;
				default:
					return xDataGridViewTheme2;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
		{
			base.OnRowPostPaint(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			Invalidate();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			Invalidate();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Invalidate();
		}

		protected override void OnScroll(ScrollEventArgs e)
		{
			base.OnScroll(e);
			Invalidate();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
		}
	}
}
