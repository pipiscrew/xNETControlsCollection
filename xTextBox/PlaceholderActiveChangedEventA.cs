namespace System.Windows.Forms
{
	internal class PlaceholderActiveChangedEventArgs : EventArgs
	{
		public bool IsActive { get; private set; }

		public PlaceholderActiveChangedEventArgs(bool isActive)
		{
			IsActive = isActive;
		}
	}
}
