using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using xCollection;

namespace Utilities.xSnackbar
{
	[Description("Provides button-like capabilities and awesome image manipulation features when working with images.")]
	[DefaultProperty("Image")]
	[ToolboxItem(false)]
	[Browsable(false)]
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[DefaultEvent("Click")]
	internal class xImageButtonExtended : UserControl
	{
		public enum FlipOrientation
		{
			Normal,
			Horizontal,
			Vertical
		}

		private enum ImageStates
		{
			Normal = 0,
			TotallyFaded = 3,
			MediallyFaded = 2,
			PartiallyFaded = 1
		}

		private const int BYTES_PER_PIXEL = 4;

		private bool _enter;

		private bool _isZoomedIn;

		private bool _allowAnimations;

		private bool _fadeWhenInactive;

		private bool _showActiveImage;

		private bool _showSizeMarkers;

		private bool _showImageBorders;

		private bool _showCursorChanges;

		private bool _allowZooming = true;

		private int _flag;

		private int _rotation;

		private int _zoomSpeed;

		private int _imageWidth;

		private int _imageHeight;

		private int _imageMargin;

		private string _imageLocation;

		private double _addx;

		private double _addy;

		private double _currentWidth;

		private double _currentHeight;

		private Image _image;

		private Image _savedImage;

		private Image _errorImage;

		private Image _activeImage;

		private Image _initialImage;

		private Image _previousImage;

		private Image _bufferedImage;

		private Graphics _graphics;

		private FlipOrientation _flip;

		private Timer _drawingTimer;

		private Cursor _currentCursor;

		private ToolTip _tooltip = new ToolTip();

		private PictureBox _picture = new PictureBox
		{
			WaitOnLoad = false,
			SizeMode = PictureBoxSizeMode.Zoom
		};

		private ImageAttributes[] _imageAttributes;

		private static float[][] _matrixItemsActive = new float[5][]
		{
			new float[5] { 1f, 0f, 0f, 0f, 0f },
			new float[5] { 0f, 1f, 0f, 0f, 0f },
			new float[5] { 0f, 0f, 1f, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 1f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		};

		private static float[][] _matrixItemsDim = new float[5][]
		{
			new float[5] { 1f, 0f, 0f, 0f, 0f },
			new float[5] { 0f, 1f, 0f, 0f, 0f },
			new float[5] { 0f, 0f, 1f, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 0.75f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		};

		private static float[][] _matrixItemsDimmer = new float[5][]
		{
			new float[5] { 1f, 0f, 0f, 0f, 0f },
			new float[5] { 0f, 1f, 0f, 0f, 0f },
			new float[5] { 0f, 0f, 1f, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 0.5f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		};

		private static float[][] _matrixItemsDimmest = new float[5][]
		{
			new float[5] { 1f, 0f, 0f, 0f, 0f },
			new float[5] { 0f, 1f, 0f, 0f, 0f },
			new float[5] { 0f, 0f, 1f, 0f, 0f },
			new float[5] { 0f, 0f, 0f, 0.25f, 0f },
			new float[5] { 0f, 0f, 0f, 0f, 1f }
		};

		private ColorMatrix[] _colorMatrix = new ColorMatrix[4]
		{
			new ColorMatrix(_matrixItemsActive),
			new ColorMatrix(_matrixItemsDim),
			new ColorMatrix(_matrixItemsDimmer),
			new ColorMatrix(_matrixItemsDimmest)
		};

		private IContainer components = null;

		[Category("x Properties")]
		[Description("Sets the distance between the container and the underlying image.")]
		public virtual int ImageMargin
		{
			get
			{
				return _imageMargin;
			}
			set
			{
				_imageMargin = value;
				try
				{
					_imageWidth = base.Width - _imageMargin;
					_imageHeight = base.Height - _imageMargin;
					Calculate();
					Refresh();
					EventHandler imageMarginChanged = this.ImageMarginChanged;
					if (imageMarginChanged != null)
						imageMarginChanged(this, EventArgs.Empty);
				}
				catch (Exception)
				{
				}
			}
		}

		[Description("Sets the distance between the container and the underlying image.")]
		[Obsolete("This property has been deprecated as of version 1.6.0.0. Please use the property 'ImageMargin' instead.")]
		[Browsable(false)]
		[Category("x Properties")]
		public int Zoom
		{
			get
			{
				return ImageMargin;
			}
			set
			{
				ImageMargin = value;
			}
		}

		[Category("x Properties")]
		[Description("Sets the angle of rotation for the image. Please provide a value between 0 and 360.")]
		public virtual int Rotation
		{
			get
			{
				return _rotation;
			}
			set
			{
				if (value >= 0 && value <= 360)
				{
					_rotation = value;
					Calculate();
					Refresh();
					EventHandler rotationChanged = this.RotationChanged;
					if (rotationChanged != null)
						rotationChanged(this, EventArgs.Empty);
					return;
				}
				throw new ArgumentOutOfRangeException("Please provide a rotation value that is between 0 and 360.");
			}
		}

		[Description("Sets the time in milliseconds the zooming animation takes when active.")]
		[Browsable(false)]
		[Category("x Properties")]
		public virtual int ZoomSpeed
		{
			get
			{
				return _zoomSpeed;
			}
			set
			{
				_zoomSpeed = value;
				EventHandler zoomSpeedChanged = this.ZoomSpeedChanged;
				if (zoomSpeedChanged != null)
					zoomSpeedChanged(this, EventArgs.Empty);
			}
		}

		[Description("Flips the image to any given orientation. Supports both Vertical and Horizontal orientation-flipping options.")]
		[Category("x Properties")]
		public virtual FlipOrientation Flip
		{
			get
			{
				return _flip;
			}
			set
			{
				_flip = value;
				Calculate();
				Refresh();
				EventHandler flipChanged = this.FlipChanged;
				if (flipChanged != null)
					flipChanged(this, EventArgs.Empty);
			}
		}

		[Category("x Properties")]
		[Description("Sets the image to be displayed in the Image Button.")]
		public Image Image
		{
			get
			{
				return _image;
			}
			set
			{
				try
				{
					_previousImage = _image;
					_image = (Bitmap)value;
					if (_previousImage != value)
					{
						EventHandler imageChanged = this.ImageChanged;
						if (imageChanged != null)
							imageChanged(this, EventArgs.Empty);
					}
					Calculate();
					Refresh();
				}
				catch (Exception)
				{
				}
			}
		}

		[Browsable(false)]
		[Category("x Properties")]
		[Description("Sets the image to be applied whenever the image button is active or hovered onto.")]
		[Obsolete("This property has been deprecated as of version 1.6.0.0. Please use the property 'ActiveImage' instead.")]
		public virtual Image ImageActive
		{
			get
			{
				return _activeImage;
			}
			set
			{
				_activeImage = value;
				EventHandler activeImageChanged = this.ActiveImageChanged;
				if (activeImageChanged != null)
					activeImageChanged(this, EventArgs.Empty);
			}
		}

		[Description("Sets the image to be applied whenever the image button is active or hovered onto.")]
		[Category("x Properties")]
		public virtual Image ActiveImage
		{
			get
			{
				return _activeImage;
			}
			set
			{
				_activeImage = value;
				EventHandler activeImageChanged = this.ActiveImageChanged;
				if (activeImageChanged != null)
					activeImageChanged(this, EventArgs.Empty);
			}
		}

		[Browsable(false)]
		[Description("Sets the default image size.")]
		[Category("x Properties")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Size ImageSize
		{
			get
			{
				return new Size(_imageWidth, _imageHeight);
			}
			set
			{
				Size size = value;
				_imageWidth = size.Width;
				size = value;
				_imageHeight = size.Height;
				if (_imageWidth > base.Width)
					_imageWidth = base.Width;
				if (_imageHeight > base.Height)
					_imageHeight = base.Height;
				Calculate();
				Refresh();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("x Properties")]
		[Description("Sets the maximum size of the image when hovered onto or active.This defaults to the standard set size of the control.")]
		public Size ImageZoomSize
		{
			get
			{
				return new Size(base.Width, base.Height);
			}
			set
			{
				Size size = value;
				base.Width = size.Width;
				size = value;
				base.Height = size.Height;
				Calculate();
				Refresh();
			}
		}

		[Category("x Properties")]
		[Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		[Description("Allows you to easily set some ToolTip information to be displayed to the user on mouse-hovering over the control.")]
		public virtual string ToolTipText
		{
			get
			{
				return _tooltip.GetToolTip(this);
			}
			set
			{
				if (value != string.Empty)
				{
					_tooltip.UseFading = true;
					_tooltip.UseAnimation = true;
					_tooltip.SetToolTip(this, value);
				}
			}
		}

		[Description("Sets a value indicating whether the control will show cursor changes during mouse-hover events.")]
		[Category("x Properties")]
		public virtual bool ShowCursorChanges
		{
			get
			{
				return _showCursorChanges;
			}
			set
			{
				_showCursorChanges = value;
				EventHandler showCursorChangesChanged = this.ShowCursorChangesChanged;
				if (showCursorChangesChanged != null)
					showCursorChangesChanged(this, EventArgs.Empty);
			}
		}

		[Category("x Properties")]
		[Description("Sets a value indicating whether the 'ActiveImage' will be applied on mouse-hover.")]
		public virtual bool ShowActiveImage
		{
			get
			{
				return _showActiveImage;
			}
			set
			{
				_showActiveImage = value;
			}
		}

		[Category("x Properties")]
		[Browsable(true)]
		[Description("Sets a value indicating whether zooming is enabled.")]
		public virtual bool AllowZooming
		{
			get
			{
				return _allowZooming;
			}
			set
			{
				_allowZooming = value;
				EventHandler allowZoomingChanged = this.AllowZoomingChanged;
				if (allowZoomingChanged != null)
					allowZoomingChanged(this, EventArgs.Empty);
			}
		}

		[Browsable(false)]
		[Category("x Properties")]
		[Description("Sets a value indicating whether animations are enabled.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowAnimations
		{
			get
			{
				return _allowAnimations;
			}
			set
			{
				_allowAnimations = value;
				EventHandler allowAnimationsChanged = this.AllowAnimationsChanged;
				if (allowAnimationsChanged != null)
					allowAnimationsChanged(this, EventArgs.Empty);
			}
		}

		[Description("Automatically fades the image when inactive.")]
		[Category("x Properties")]
		public virtual bool FadeWhenInactive
		{
			get
			{
				return _fadeWhenInactive;
			}
			set
			{
				_fadeWhenInactive = value;
				EventHandler fadeWhenInactiveChanged = this.FadeWhenInactiveChanged;
				if (fadeWhenInactiveChanged != null)
					fadeWhenInactiveChanged(this, EventArgs.Empty);
				Calculate();
				Refresh();
			}
		}

		[Browsable(false)]
		public virtual bool IsZoomedIn
		{
			get
			{
				return _isZoomedIn;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("x Properties")]
		[Browsable(false)]
		[Description("Sets a value indicating whether the size markers displayed at the control's top-area will be visible during design-time.")]
		public virtual bool ShowSizeMarkers
		{
			get
			{
				return _showSizeMarkers;
			}
			set
			{
				_showSizeMarkers = value;
				Calculate();
				Refresh();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Category("x Properties")]
		[Description("Sets a value indicating whether borders will be viewed for the image applied during design-time.")]
		[Browsable(false)]
		public virtual bool ShowImageBorders
		{
			get
			{
				return _showImageBorders;
			}
			set
			{
				_showImageBorders = value;
				Calculate();
				Refresh();
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				createParams.ExStyle |= 33554432;
				return createParams;
			}
		}

		[Category("x Properties: Asynchronous")]
		[Description("Sets the path or URL for the image to display in the Image Button.")]
		public string ImageLocation
		{
			get
			{
				return _imageLocation;
			}
			set
			{
				_imageLocation = value;
				if (!(value != string.Empty) && value == null)
				{
					_picture.ImageLocation = value;
					Image = InitialImage;
					if (WaitOnLoad)
						Load();
					else
						LoadAsync();
				}
			}
		}

		[Description("Sets a value indicating whether an image is loaded synchronously.")]
		[Category("x Properties: Asynchronous")]
		public bool WaitOnLoad
		{
			get
			{
				return _picture.WaitOnLoad;
			}
			set
			{
				_picture.WaitOnLoad = value;
			}
		}

		[Category("x Properties: Asynchronous")]
		[Description("Sets the image displayed in the Image Button when the main image is loading.")]
		public Image InitialImage
		{
			get
			{
				return _initialImage;
			}
			set
			{
				_initialImage = (Bitmap)value;
			}
		}

		[Description("Sets the image displayed in the Image Button when an error occurs during the image-loading process or if the image load is canceled.")]
		[Category("x Properties: Asynchronous")]
		public Image ErrorImage
		{
			get
			{
				return _errorImage;
			}
			set
			{
				_errorImage = (Bitmap)value;
			}
		}

		[Category("x Events")]
		[Description("Occurs whenever the image has been changed.")]
		public event EventHandler ImageChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the property 'Zoom' has been changed.")]
		public event EventHandler ImageMarginChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the image button has been zoomed-in.")]
		public event EventHandler ZoomedIn = null;

		[Description("Occurs whenever the image button has been zoomed-out.")]
		[Category("x Events")]
		public event EventHandler ZoomedOut = null;

		[Browsable(false)]
		[Description("Occurs whenever the property 'AllowAnimations' has been changed.")]
		[Category("x Events")]
		public event EventHandler AllowAnimationsChanged = null;

		[Description("Occurs whenever the property 'AllowZooming' has been changed.")]
		[Category("x Events")]
		public event EventHandler AllowZoomingChanged = null;

		[Description("Occurs whenever the active image has been changed.")]
		[Category("x Events")]
		public event EventHandler ActiveImageChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the image's rotation has been changed.")]
		public event EventHandler RotationChanged = null;

		[Description("Occurs whenever the image's flip-orientation has been changed.")]
		[Category("x Events")]
		public event EventHandler FlipChanged = null;

		[Category("x Events")]
		[Description("Occurs whenever the property 'FadeWhenInactive' has been changed.")]
		public event EventHandler FadeWhenInactiveChanged = null;

		[Description("Occurs whenever the property 'ShowCursorChanges' has been changed.")]
		[Category("x Events")]
		public event EventHandler ShowCursorChangesChanged = null;

		[Browsable(false)]
		[Category("x Events")]
		[Description("Occurs whenever the property 'RotationSpeed' has been changed.")]
		public event EventHandler ZoomSpeedChanged = null;

		[Description("Occurs when the asynchronous image-load operation is completed, been canceled, or raised an exception.")]
		[Category("x Events: Asynchronous")]
		public event AsyncCompletedEventHandler LoadCompleted;

		[Category("x Events: Asynchronous")]
		[Description("Occurs when the progress of an asynchronous image-loading operation has changed.")]
		public event ProgressChangedEventHandler LoadProgressChanged;

		public xImageButtonExtended()
		{
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			_imageAttributes = new ImageAttributes[4];
			for (int i = 0; i < 4; i++)
			{
				_imageAttributes[i] = new ImageAttributes();
				_imageAttributes[i].SetColorMatrix(_colorMatrix[i], ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			}
			_zoomSpeed = 10;
			_imageMargin = 40;
			_allowZooming = true;
			_allowAnimations = true;
			_showActiveImage = true;
			_showSizeMarkers = true;
			_showImageBorders = true;
			_showCursorChanges = true;
			_flip = FlipOrientation.Normal;
			_imageWidth = base.Width / 2;
			_imageHeight = base.Height / 2;
			_bufferedImage = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
			_graphics = Graphics.FromImage(_bufferedImage);
			_picture.LoadCompleted += OnLoadCompleted;
			_picture.LoadProgressChanged += OnLoadProgressChanged;
			InitializeComponent();
			_drawingTimer = new Timer();
			_drawingTimer.Interval = _zoomSpeed;
			_drawingTimer.Tick += OnDrawingTimerTick;
			AllowZooming = true;
			BackColor = Color.Transparent;
			ImageZoomSize = new Size(90, 90);
			AllowZooming = true;
		}

		private void Calculate()
		{
			try
			{
				_imageWidth = base.Width - _imageMargin;
				_imageHeight = base.Height - _imageMargin;
				_currentWidth = _imageWidth;
				_currentHeight = _imageHeight;
				_addx = (double)(base.Width - _imageWidth) / 10.0;
				_addy = (double)(base.Height - _imageHeight) / 10.0;
			}
			catch (Exception)
			{
			}
		}

		private void Render(ImageStates state)
		{
			int num = 0;
			if (_fadeWhenInactive)
			{
				switch (state)
				{
				case ImageStates.TotallyFaded:
					num = 3;
					break;
				case ImageStates.PartiallyFaded:
					num = 1;
					break;
				case ImageStates.Normal:
					num = 0;
					break;
				default:
					num = 0;
					break;
				case ImageStates.MediallyFaded:
					num = 2;
					break;
				}
			}
			else
				num = 0;
			Render(num);
		}

		private void Render(int state)
		{
			try
			{
				if (_image == null)
					return;
				_graphics.Clear((base.Parent == null || BackColor != Color.Transparent) ? BackColor : base.Parent.BackColor);
				Rectangle destRect = new Rectangle((int)((double)base.Width - _currentWidth) / 2, (int)((double)base.Height - _currentHeight) / 2, (int)_currentWidth, (int)_currentHeight);
				_graphics.DrawImage(_image, destRect, 0f, 0f, _image.Width, _image.Height, GraphicsUnit.Pixel, _imageAttributes[state]);
				if (IsInDesignMode())
				{
					Rectangle rectangle = new Rectangle(destRect.X - 5, destRect.Y - 5, destRect.Width + 10, destRect.Height + 10);
					if (ShowSizeMarkers)
						ControlPaint.DrawStringDisabled(_graphics, string.Format("{0} x {1} : {2} x {3}", base.Width, base.Height, ImageSize.Width, ImageSize.Height), new Font(Font.Name, 7f, FontStyle.Regular), ControlPaint.Light(FindForm().ForeColor, 0.2f), base.ClientRectangle, StringFormat.GenericDefault);
					if (ShowImageBorders)
						ControlPaint.DrawFocusRectangle(_graphics, rectangle, ControlPaint.Dark(FindForm().BackColor, 0f), BackColor);
				}
				ImageExtensions.FlipOrientation flip = ImageExtensions.FlipOrientation.Normal;
				if (_flip == FlipOrientation.Horizontal)
					flip = ImageExtensions.FlipOrientation.Horizontal;
				else if (_flip == FlipOrientation.Vertical)
				{
					flip = ImageExtensions.FlipOrientation.Vertical;
				}
				else if (_flip == FlipOrientation.Normal)
				{
					flip = ImageExtensions.FlipOrientation.Normal;
				}
				using (Graphics graphics = CreateGraphics())
				{
					graphics.Rotate(_rotation, base.Width, base.Height);
					graphics.Flip(flip, base.Width, base.Height);
					graphics.DrawImageUnscaled(_bufferedImage, 0, 0);
				}
			}
			catch (Exception)
			{
			}
		}

		public override void Refresh()
		{
			Render(ImageStates.TotallyFaded);
		}

		public void ZoomIn()
		{
			_enter = true;
			if (ShowActiveImage && ActiveImage != null)
			{
				_savedImage = _image;
				_image = _activeImage;
			}
			if (AllowAnimations)
				_drawingTimer.Interval = _zoomSpeed;
			else
				_drawingTimer.Interval = 1;
			_drawingTimer.Enabled = true;
			if (ShowCursorChanges)
			{
				_currentCursor = Cursor;
				Cursor = Cursors.Hand;
			}
			_isZoomedIn = true;
			EventHandler zoomedIn = this.ZoomedIn;
			if (zoomedIn != null)
				zoomedIn(this, EventArgs.Empty);
		}

		public void ZoomOut()
		{
			_enter = false;
			if (ShowActiveImage && ActiveImage != null)
				_image = _savedImage;
			_drawingTimer.Enabled = true;
			if (ShowCursorChanges)
				Cursor = _currentCursor;
			_isZoomedIn = false;
			EventHandler zoomedOut = this.ZoomedOut;
			if (zoomedOut != null)
				zoomedOut(this, EventArgs.Empty);
		}

		public void Rotate(int angle, bool allowAnimation = true, int animationSpeed = 500)
		{
			if (allowAnimation)
				Transition.run(this, "Rotation", angle, new TransitionType_EaseInEaseOut(animationSpeed));
			else
				Rotation = angle;
		}

		public new void Load(string url)
		{
			_picture.Load(url);
		}

		public new void Load()
		{
			if (ImageLocation != string.Empty || ImageLocation != null)
				_picture.Load();
		}

		public void LoadAsync()
		{
			_picture.LoadAsync();
		}

		public void LoadAsync(string url)
		{
			_picture.LoadAsync(url);
		}

		public void CancelAsync()
		{
			_picture.CancelAsync();
		}

		private void OnLoadProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			ProgressChangedEventHandler loadProgressChanged = this.LoadProgressChanged;
			if (loadProgressChanged != null)
				loadProgressChanged(_picture, e);
		}

		private void OnLoadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			AsyncCompletedEventHandler loadCompleted = this.LoadCompleted;
			if (loadCompleted != null)
				loadCompleted(_picture, e);
			if (e.Cancelled || e.Error != null)
				Image = ErrorImage;
			else
				Image = _picture.Image;
		}

		private new static Image Resize(Image imgToResize, Size size)
		{
			int num = imgToResize.Width;
			int num2 = imgToResize.Height;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			num4 = (float)size.Width / (float)num;
			num5 = (float)size.Height / (float)num2;
			num3 = ((!(num5 < num4)) ? num4 : num5);
			int num6 = (int)((float)num * num3);
			int num7 = (int)((float)num2 * num3);
			Bitmap bitmap = new Bitmap(num6, num7);
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.DrawImage(imgToResize, 0, 0, num6, num7);
			graphics.Dispose();
			return bitmap;
		}

		private static Image Crop(Image img, Rectangle cropArea)
		{
			Bitmap bitmap = new Bitmap(img);
			return bitmap.Clone(cropArea, bitmap.PixelFormat);
		}

		private static Bitmap GrayscaleConvert(Bitmap original)
		{
			Bitmap bitmap = new Bitmap(original.Width, original.Height);
			Graphics graphics = Graphics.FromImage(bitmap);
			ColorMatrix colorMatrix = new ColorMatrix(new float[5][]
			{
				new float[5] { 0.3f, 0.3f, 0.3f, 0f, 0f },
				new float[5] { 0.59f, 0.59f, 0.59f, 0f, 0f },
				new float[5] { 0.11f, 0.11f, 0.11f, 0f, 0f },
				new float[5] { 0f, 0f, 0f, 1f, 0f },
				new float[5] { 0f, 0f, 0f, 0f, 1f }
			});
			ImageAttributes imageAttributes = new ImageAttributes();
			imageAttributes.SetColorMatrix(colorMatrix);
			graphics.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, imageAttributes);
			graphics.Dispose();
			return bitmap;
		}

		private Image SetImageOpacity(Image image, float opacity)
		{
			try
			{
				Bitmap bitmap = new Bitmap(image.Width, image.Height);
				Graphics graphics = Graphics.FromImage(bitmap);
				ColorMatrix colorMatrix = new ColorMatrix();
				colorMatrix.Matrix33 = opacity;
				ImageAttributes imageAttributes = new ImageAttributes();
				imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				graphics.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				graphics.Dispose();
				return bitmap;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private Image SetImageOpacity(Image originalImage, double opacity)
		{
			if ((originalImage.PixelFormat & PixelFormat.Indexed) != PixelFormat.Indexed)
			{
				Bitmap bitmap = (Bitmap)originalImage.Clone();
				Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
				BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
				IntPtr scan = bitmapData.Scan0;
				int num = bitmap.Width * bitmap.Height * 4;
				byte[] array = new byte[num];
				Marshal.Copy(scan, array, 0, num);
				for (int i = 0; i < array.Length; i += 4)
				{
					if (array[i + 4 - 1] != 0)
						array[i + 3] = (byte)((double)(int)array[i + 3] * opacity);
				}
				Marshal.Copy(array, 0, scan, num);
				bitmap.UnlockBits(bitmapData);
				return bitmap;
			}
			return originalImage;
		}

		private bool IsInDesignMode()
		{
			if (Application.ExecutablePath.IndexOf("devenv.exe", StringComparison.OrdinalIgnoreCase) > -1)
				return true;
			return false;
		}

		private void OnDrawingTimerTick(object sender, EventArgs e)
		{
			if (_enter)
			{
				if (AllowZooming)
				{
					if (_currentWidth < (double)base.Width)
						_currentWidth += _addx;
					if (_currentHeight < (double)base.Height)
						_currentHeight += _addy;
					if (_currentWidth >= (double)base.Width && _currentHeight >= (double)base.Height)
						_drawingTimer.Enabled = false;
				}
				_flag++;
			}
			else
			{
				if (AllowZooming)
				{
					if (_currentWidth > (double)_imageWidth)
						_currentWidth -= _addx;
					if (_currentHeight > (double)_imageHeight)
						_currentHeight -= _addy;
					if (_currentWidth <= (double)_imageWidth && _currentHeight <= (double)_imageHeight)
						_drawingTimer.Enabled = false;
				}
				_flag--;
			}
			try
			{
				if (_flag > 9)
				{
					Render(ImageStates.Normal);
					if (!AllowZooming)
						_drawingTimer.Enabled = false;
				}
				else if (_flag > 6)
				{
					Render(ImageStates.PartiallyFaded);
				}
				else if (_flag > 3)
				{
					Render(ImageStates.MediallyFaded);
				}
				else
				{
					Render(ImageStates.TotallyFaded);
					if (!AllowZooming && !IsZoomedIn)
						_drawingTimer.Enabled = false;
				}
			}
			catch (Exception)
			{
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Refresh();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			ZoomIn();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (IsZoomedIn)
				ZoomOut();
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			OnMouseLeave(EventArgs.Empty);
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			if (base.Visible)
				Refresh();
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!IsZoomedIn && ShowCursorChanges)
			{
				_currentCursor = Cursor;
				Cursor = Cursors.Hand;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (!IsZoomedIn && ShowCursorChanges)
				Cursor = _currentCursor;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			try
			{
				_bufferedImage = new Bitmap(base.Width, base.Height, PixelFormat.Format32bppArgb);
				_graphics = Graphics.FromImage(_bufferedImage);
				Calculate();
				Refresh();
			}
			catch (Exception)
			{
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			base.SuspendLayout();
			base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			base.ResumeLayout(false);
		}
	}
}
