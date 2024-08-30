using System;
using Utilities.xSnackbar.Views;

namespace xCollection
{
	public static class SnackbarExtensions
	{
		public static void Then(this xSnackbar.SnackbarResult result, Action<xSnackbar.SnackbarResult> action)
		{
			try
			{
				SnackbarView._result = result;
				if (action != null)
					SnackbarView._action = action;
				else
					SnackbarView._action = null;
				SnackbarView._result = xSnackbar.SnackbarResult.AutoClosed;
			}
			catch (Exception)
			{
			}
		}
	}
}
