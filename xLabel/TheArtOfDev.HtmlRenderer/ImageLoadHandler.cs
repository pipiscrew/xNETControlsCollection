using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal sealed class ImageLoadHandler : IDisposable
	{
		private readonly HtmlContainerInt _htmlContainer;

		private readonly ActionInt<RImage, RRect, bool> _loadCompleteCallback;

		private FileStream _imageFileStream;

		private RImage _image;

		private RRect _imageRectangle;

		private bool _asyncCallback;

		private bool _releaseImageObject;

		private bool _disposed;

		public RImage Image
		{
			get
			{
				return _image;
			}
		}

		public RRect Rectangle
		{
			get
			{
				return _imageRectangle;
			}
		}

		public ImageLoadHandler(HtmlContainerInt htmlContainer, ActionInt<RImage, RRect, bool> loadCompleteCallback)
		{
			ArgChecker.AssertArgNotNull(htmlContainer, "htmlContainer");
			ArgChecker.AssertArgNotNull(loadCompleteCallback, "loadCompleteCallback");
			_htmlContainer = htmlContainer;
			_loadCompleteCallback = loadCompleteCallback;
		}

		public void LoadImage(string src, Dictionary<string, string> attributes)
		{
			try
			{
				HtmlImageLoadEventArgs htmlImageLoadEventArgs = new HtmlImageLoadEventArgs(src, attributes, OnHtmlImageLoadEventCallback);
				_htmlContainer.RaiseHtmlImageLoadEvent(htmlImageLoadEventArgs);
				_asyncCallback = !_htmlContainer.AvoidAsyncImagesLoading;
				if (htmlImageLoadEventArgs.Handled)
					return;
				if (!string.IsNullOrEmpty(src))
				{
					if (src.StartsWith("data:image", StringComparison.CurrentCultureIgnoreCase))
						SetFromInlineData(src);
					else
						SetImageFromPath(src);
				}
				else
					ImageLoadComplete(false);
			}
			catch (Exception exception)
			{
				_htmlContainer.ReportError(HtmlRenderErrorType.Image, "Exception in handling image source", exception);
				ImageLoadComplete(false);
			}
		}

		public void Dispose()
		{
			_disposed = true;
			ReleaseObjects();
		}

		private void OnHtmlImageLoadEventCallback(string path, object image, RRect imageRectangle)
		{
			if (!_disposed)
			{
				_imageRectangle = imageRectangle;
				if (image != null)
				{
					_image = _htmlContainer.Adapter.ConvertImage(image);
					ImageLoadComplete(_asyncCallback);
				}
				else if (!string.IsNullOrEmpty(path))
				{
					SetImageFromPath(path);
				}
				else
				{
					ImageLoadComplete(_asyncCallback);
				}
			}
		}

		private void SetFromInlineData(string src)
		{
			_image = GetImageFromData(src);
			if (_image == null)
				_htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed extract image from inline data");
			_releaseImageObject = true;
			ImageLoadComplete(false);
		}

		private RImage GetImageFromData(string src)
		{
			string[] array = src.Substring(src.IndexOf(':') + 1).Split(new char[1] { ',' }, 2);
			if (array.Length == 2)
			{
				int num = 0;
				int num2 = 0;
				string[] array2 = array[0].Split(';');
				foreach (string text in array2)
				{
					string text2 = text.Trim();
					if (text2.StartsWith("image/", StringComparison.InvariantCultureIgnoreCase))
						num++;
					if (text2.Equals("base64", StringComparison.InvariantCultureIgnoreCase))
						num2++;
				}
				if (num > 0)
				{
					byte[] buffer = ((num2 > 0) ? Convert.FromBase64String(array[1].Trim()) : new UTF8Encoding().GetBytes(Uri.UnescapeDataString(array[1].Trim())));
					return _htmlContainer.Adapter.ImageFromStream(new MemoryStream(buffer));
				}
			}
			return null;
		}

		private void SetImageFromPath(string path)
		{
			Uri uri = CommonUtils.TryGetUri(path);
			if (uri != null && uri.Scheme != "file")
			{
				SetImageFromUrl(uri);
				return;
			}
			FileInfo fileInfo = CommonUtils.TryGetFileInfo((uri != null) ? uri.AbsolutePath : path);
			if (fileInfo == null)
			{
				_htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed load image, invalid source: " + path);
				ImageLoadComplete(false);
			}
			else
				SetImageFromFile(fileInfo);
		}

		private void SetImageFromFile(FileInfo source)
		{
			if (source.Exists)
			{
				if (_htmlContainer.AvoidAsyncImagesLoading)
				{
					LoadImageFromFile(source.FullName);
					return;
				}
				ThreadPool.QueueUserWorkItem(delegate
				{
					LoadImageFromFile(source.FullName);
				});
			}
			else
				ImageLoadComplete();
		}

		private void LoadImageFromFile(string source)
		{
			try
			{
				FileStream imageFileStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				lock (_loadCompleteCallback)
				{
					_imageFileStream = imageFileStream;
					if (!_disposed)
						_image = _htmlContainer.Adapter.ImageFromStream(_imageFileStream);
					_releaseImageObject = true;
				}
				ImageLoadComplete();
			}
			catch (Exception exception)
			{
				_htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed to load image from disk: " + source, exception);
				ImageLoadComplete();
			}
		}

		private void SetImageFromUrl(Uri source)
		{
			FileInfo localfileName = CommonUtils.GetLocalfileName(source);
			if (localfileName.Exists && localfileName.Length > 0L)
				SetImageFromFile(localfileName);
			else
				_htmlContainer.GetImageDownloader().DownloadImage(source, localfileName.FullName, !_htmlContainer.AvoidAsyncImagesLoading, OnDownloadImageCompleted);
		}

		private void OnDownloadImageCompleted(Uri imageUri, string filePath, Exception error, bool canceled)
		{
			if (!canceled && !_disposed)
			{
				if (error == null)
				{
					LoadImageFromFile(filePath);
					return;
				}
				_htmlContainer.ReportError(HtmlRenderErrorType.Image, "Failed to load image from URL: " + (((object)imageUri != null) ? imageUri.ToString() : null), error);
				ImageLoadComplete();
			}
		}

		private void ImageLoadComplete(bool async = true)
		{
			if (_disposed)
				ReleaseObjects();
			else
				_loadCompleteCallback(_image, _imageRectangle, async);
		}

		private void ReleaseObjects()
		{
			lock (_loadCompleteCallback)
			{
				if (_releaseImageObject && _image != null)
				{
					_image.Dispose();
					_image = null;
				}
				if (_imageFileStream != null)
				{
					_imageFileStream.Dispose();
					_imageFileStream = null;
				}
			}
		}
	}
}
