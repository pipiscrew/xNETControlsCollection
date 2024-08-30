using System;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	public delegate void DownloadFileAsyncCallback(Uri imageUri, string filePath, Exception error, bool canceled);
}
