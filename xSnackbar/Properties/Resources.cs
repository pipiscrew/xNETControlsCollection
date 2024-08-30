using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace xCollection.Properties
{
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
	[DebuggerNonUserCode]
	internal class Resources
	{
		private static ResourceManager resourceMan;

		private static CultureInfo resourceCulture;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (resourceMan == null)
				{
					ResourceManager resourceManager = (resourceMan = new ResourceManager("xSnackbar.Properties.Resources", typeof(Resources).Assembly));
				}
				return resourceMan;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return resourceCulture;
			}
			set
			{
				resourceCulture = value;
			}
		}

		internal static Bitmap error
		{
			get
			{
				object @object = ResourceManager.GetObject("error", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap info
		{
			get
			{
				object @object = ResourceManager.GetObject("info", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap success
		{
			get
			{
				object @object = ResourceManager.GetObject("success", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal static Bitmap warning
		{
			get
			{
				object @object = ResourceManager.GetObject("warning", resourceCulture);
				return (Bitmap)@object;
			}
		}

		internal Resources()
		{
		}
	}
}
