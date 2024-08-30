using System.Drawing;

namespace xCollection
{
	internal interface ISnackbarOptions
	{
		ISnackbarOptions MessageOptions(string message);

		ISnackbarOptions MessageOptions(string message, xSnackbar.MessageTypes type);

		ISnackbarOptions MessageOptions(string message, xSnackbar.MessageTypes type, Color foreColor);

		ISnackbarOptions MessageOptions(string message, xSnackbar.MessageTypes type, Color foreColor, Font font);

		ISnackbarOptions PositionOptions(xSnackbar.Positions position);

		ISnackbarOptions PositionOptions(xSnackbar.Positions position, xSnackbar.Hosts host);

		ISnackbarOptions ActionOptions(string actionText);

		ISnackbarOptions ActionOptions(string actionText, Color foreColor);

		ISnackbarOptions ActionOptions(string actionText, Color foreColor, Font font);

		ISnackbarOptions IconOptions(bool showIcon);

		ISnackbarOptions IconOptions(bool showIcon, Image image);

		ISnackbarOptions CloseIconOptions(bool showIcon);

		ISnackbarOptions CloseIconOptions(bool showIcon, Image image);

		ISnackbarOptions CloseIconOptions(bool showIcon, Image image, bool fade);

		ISnackbarOptions CloseIconOptions(bool showIcon, Image image, bool fade, bool zoom);

		ISnackbarOptions BackgroundOptions(Color backColor);

		ISnackbarOptions BackgroundOptions(Color backColor, Color borderColor);

		ISnackbarOptions BackgroundOptions(Color backColor, Color borderColor, bool showShadows);

		ISnackbarOptions ExtraOptions(bool clickToClose);

		ISnackbarOptions ExtraOptions(bool clickToClose, bool doubleClickToClose);

		ISnackbarOptions ExtraOptions(bool clickToClose, bool doubleClickToClose, bool topMost);
	}
}
