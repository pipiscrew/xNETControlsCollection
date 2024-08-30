using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal sealed class ImageDownloader : IDisposable
	{
		private sealed class DownloadData
		{
			public readonly Uri _uri;

			public readonly string _tempPath;

			public readonly string _filePath;

			public DownloadData(Uri uri, string tempPath, string filePath)
			{
				_uri = uri;
				_tempPath = tempPath;
				_filePath = filePath;
			}
		}

		private readonly List<WebClient> _clients = new List<WebClient>();

		private readonly Dictionary<string, List<DownloadFileAsyncCallback>> _imageDownloadCallbacks = new Dictionary<string, List<DownloadFileAsyncCallback>>();

		public void DownloadImage(Uri imageUri, string filePath, bool async, DownloadFileAsyncCallback cachedFileCallback)
		{
			ArgChecker.AssertArgNotNull(imageUri, "imageUri");
			ArgChecker.AssertArgNotNull(cachedFileCallback, "cachedFileCallback");
			bool flag = true;
			lock (_imageDownloadCallbacks)
			{
				if (_imageDownloadCallbacks.ContainsKey(filePath))
				{
					flag = false;
					_imageDownloadCallbacks[filePath].Add(cachedFileCallback);
				}
				else
					_imageDownloadCallbacks[filePath] = new List<DownloadFileAsyncCallback> { cachedFileCallback };
			}
			if (flag)
			{
				string tempFileName = Path.GetTempFileName();
				if (async)
					ThreadPool.QueueUserWorkItem(DownloadImageFromUrlAsync, new DownloadData(imageUri, tempFileName, filePath));
				else
					DownloadImageFromUrl(imageUri, tempFileName, filePath);
			}
		}

		public void Dispose()
		{
			ReleaseObjects();
		}

		private void DownloadImageFromUrl(Uri source, string tempPath, string filePath)
		{
			try
			{
				using (WebClient webClient = new WebClient())
				{
					_clients.Add(webClient);
					webClient.DownloadFile(source, tempPath);
					OnDownloadImageCompleted(webClient, source, tempPath, filePath, null, false);
				}
			}
			catch (Exception error)
			{
				OnDownloadImageCompleted(null, source, tempPath, filePath, error, false);
			}
		}

		private void DownloadImageFromUrlAsync(object data)
		{
			DownloadData downloadData = (DownloadData)data;
			try
			{
				WebClient webClient = new WebClient();
				_clients.Add(webClient);
				webClient.DownloadFileCompleted += OnDownloadImageAsyncCompleted;
				webClient.DownloadFileAsync(downloadData._uri, downloadData._tempPath, downloadData);
			}
			catch (Exception error)
			{
				OnDownloadImageCompleted(null, downloadData._uri, downloadData._tempPath, downloadData._filePath, error, false);
			}
		}

		private void OnDownloadImageAsyncCompleted(object sender, AsyncCompletedEventArgs e)
		{
			DownloadData downloadData = (DownloadData)e.UserState;
			try
			{
				using (WebClient webClient = (WebClient)sender)
				{
					webClient.DownloadFileCompleted -= OnDownloadImageAsyncCompleted;
					OnDownloadImageCompleted(webClient, downloadData._uri, downloadData._tempPath, downloadData._filePath, e.Error, e.Cancelled);
				}
			}
			catch (Exception error)
			{
				OnDownloadImageCompleted(null, downloadData._uri, downloadData._tempPath, downloadData._filePath, error, false);
			}
		}

		private void OnDownloadImageCompleted(WebClient client, Uri source, string tempPath, string filePath, Exception error, bool cancelled)
		{
			if (!cancelled)
			{
				if (error == null)
				{
					string responseContentType = CommonUtils.GetResponseContentType(client);
					if (responseContentType == null || !responseContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
						error = new Exception("Failed to load image, not image content type: " + responseContentType);
				}
				if (error == null)
				{
					if (File.Exists(tempPath))
						try
						{
							File.Move(tempPath, filePath);
						}
						catch (Exception innerException)
						{
							error = new Exception("Failed to move downloaded image from temp to cache location", innerException);
						}
					error = (File.Exists(filePath) ? null : (error ?? new Exception("Failed to download image, unknown error")));
				}
			}
			List<DownloadFileAsyncCallback> value;
			lock (_imageDownloadCallbacks)
			{
				if (_imageDownloadCallbacks.TryGetValue(filePath, out value))
					_imageDownloadCallbacks.Remove(filePath);
			}
			if (value == null)
				return;
			foreach (DownloadFileAsyncCallback item in value)
			{
				try
				{
					item(source, filePath, error, cancelled);
				}
				catch
				{
				}
			}
		}

		private void ReleaseObjects()
		{
			_imageDownloadCallbacks.Clear();
			while (_clients.Count > 0)
			{
				try
				{
					WebClient webClient = _clients[0];
					webClient.CancelAsync();
					webClient.Dispose();
					_clients.RemoveAt(0);
				}
				catch
				{
				}
			}
		}
	}
}
