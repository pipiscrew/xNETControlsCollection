using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace Utilities.xSnackbar
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Description("Provides a list of options for customizing a borderless Form's border regions.")]
	public class FormBorderOptions
	{
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DebuggerStepThrough]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Description("Provides a list of options for customizing a single borderless Form's border region.")]
        public class BorderRegion
        {
            private bool showBorder = true;
            public bool ShowBorder
            {
                get { return showBorder; }
                set { showBorder = value; }
            }

            private int borderThickness = 1;
            public int BorderThickness
            {
                get { return borderThickness; }
                set { borderThickness = value; }
            }

            private Color borderColor = Color.Silver;
            public Color BorderColor
            {
                get { return borderColor; }
                set { borderColor = value; }
            }

            public override string ToString()
            {
                return string.Format("Visible: {0}", ShowBorder);
            }
        }

		private BorderRegion _topBorder = new BorderRegion();

		private BorderRegion _leftBorder = new BorderRegion();

		private BorderRegion _rightBorder = new BorderRegion();

		private BorderRegion _bottomBorder = new BorderRegion();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderRegion TopBorder
		{
			get
			{
				return _topBorder;
			}
			set
			{
				_topBorder = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderRegion LeftBorder
		{
			get
			{
				return _leftBorder;
			}
			set
			{
				_leftBorder = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderRegion RightBorder
		{
			get
			{
				return _rightBorder;
			}
			set
			{
				_rightBorder = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BorderRegion BottomBorder
		{
			get
			{
				return _bottomBorder;
			}
			set
			{
				_bottomBorder = value;
			}
		}

		public override string ToString()
		{
			return string.Format("(Active: {0})", (TopBorder.ShowBorder || BottomBorder.ShowBorder || LeftBorder.ShowBorder || RightBorder.ShowBorder) ? true : false);
		}
	}
}
