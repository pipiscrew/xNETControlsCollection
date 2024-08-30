using System;
using System.IO;

namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	public static class ArgChecker
	{
		public static void AssertIsTrue<TException>(bool condition, string message) where TException : Exception, new()
		{
			if (!condition)
				throw (TException)Activator.CreateInstance(typeof(TException), message);
		}

		public static void AssertArgNotNull(object arg, string argName)
		{
			if (arg == null)
				throw new ArgumentNullException(argName);
		}

		public static void AssertArgNotNull(IntPtr arg, string argName)
		{
			if (arg == IntPtr.Zero)
				throw new ArgumentException("IntPtr argument cannot be Zero", argName);
		}

		public static void AssertArgNotNullOrEmpty(string arg, string argName)
		{
			if (string.IsNullOrEmpty(arg))
				throw new ArgumentNullException(argName);
		}

		public static T AssertArgOfType<T>(object arg, string argName)
		{
			AssertArgNotNull(arg, argName);
			if (!(arg is T))
				throw new ArgumentException(string.Format("Given argument isn't of type '{0}'.", typeof(T).Name), argName);
			return (T)arg;
		}

		public static void AssertFileExist(string arg, string argName)
		{
			AssertArgNotNullOrEmpty(arg, argName);
			if (!File.Exists(arg))
				throw new FileNotFoundException(string.Format("Given file in argument '{0}' not exist.", argName), arg);
		}
	}
}
