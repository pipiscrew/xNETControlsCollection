namespace TheArtOfDev.HtmlRenderer.Core.Utils
{
	internal delegate void ActionInt<in T>(T obj);
	internal delegate void ActionInt<in T1, in T2>(T1 arg1, T2 arg2);
	internal delegate void ActionInt<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
}
