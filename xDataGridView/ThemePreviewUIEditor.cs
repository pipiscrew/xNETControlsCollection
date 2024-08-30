using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Utilities.xDataGridView;

namespace xCollection
{
	[DebuggerStepThrough]
	internal class ThemePreviewUIEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService windowsFormsEditorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
			if (windowsFormsEditorService != null)
			{
				ThemePreviewEditor themePreviewEditor = new ThemePreviewEditor();
				themePreviewEditor.SelectedTheme = (xDataGridView.PresetThemes)value;
				themePreviewEditor.editorService = windowsFormsEditorService;
				windowsFormsEditorService.DropDownControl(themePreviewEditor);
				value = themePreviewEditor.SelectedTheme;
			}
			return value;
		}
	}
}
