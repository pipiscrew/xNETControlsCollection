using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Adapters.Entities;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Handlers
{
	internal sealed class FontsHandler
	{
		private readonly RAdapter _adapter;

		private readonly Dictionary<string, string> _fontsMapping = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

		private readonly Dictionary<string, RFontFamily> _existingFontFamilies = new Dictionary<string, RFontFamily>(StringComparer.InvariantCultureIgnoreCase);

		private readonly Dictionary<string, Dictionary<double, Dictionary<RFontStyle, RFont>>> _fontsCache = new Dictionary<string, Dictionary<double, Dictionary<RFontStyle, RFont>>>(StringComparer.InvariantCultureIgnoreCase);

		public FontsHandler(RAdapter adapter)
		{
			ArgChecker.AssertArgNotNull(adapter, "global");
			_adapter = adapter;
		}

		public bool IsFontExists(string family)
		{
			bool result;
			string value;
			if (!(result = _existingFontFamilies.ContainsKey(family)) && _fontsMapping.TryGetValue(family, out value))
				result = _existingFontFamilies.ContainsKey(value);
			return result;
		}

		public void AddFontFamily(RFontFamily fontFamily)
		{
			ArgChecker.AssertArgNotNull(fontFamily, "family");
			_existingFontFamilies[fontFamily.Name] = fontFamily;
		}

		public void AddFontFamilyMapping(string fromFamily, string toFamily)
		{
			ArgChecker.AssertArgNotNullOrEmpty(fromFamily, "fromFamily");
			ArgChecker.AssertArgNotNullOrEmpty(toFamily, "toFamily");
			_fontsMapping[fromFamily] = toFamily;
		}

		public RFont GetCachedFont(string family, double size, RFontStyle style)
		{
			RFont rFont = TryGetFont(family, size, style);
			if (rFont == null)
			{
				string value;
				if (!_existingFontFamilies.ContainsKey(family) && _fontsMapping.TryGetValue(family, out value))
				{
					rFont = TryGetFont(value, size, style);
					if (rFont == null)
					{
						rFont = CreateFont(value, size, style);
						_fontsCache[value][size][style] = rFont;
					}
				}
				if (rFont == null)
					rFont = CreateFont(family, size, style);
				_fontsCache[family][size][style] = rFont;
			}
			return rFont;
		}

		private RFont TryGetFont(string family, double size, RFontStyle style)
		{
			RFont result = null;
			if (_fontsCache.ContainsKey(family))
			{
				Dictionary<double, Dictionary<RFontStyle, RFont>> dictionary = _fontsCache[family];
				if (dictionary.ContainsKey(size))
				{
					Dictionary<RFontStyle, RFont> dictionary2 = dictionary[size];
					if (dictionary2.ContainsKey(style))
						result = dictionary2[style];
				}
				else
					_fontsCache[family][size] = new Dictionary<RFontStyle, RFont>();
			}
			else
			{
				_fontsCache[family] = new Dictionary<double, Dictionary<RFontStyle, RFont>>();
				_fontsCache[family][size] = new Dictionary<RFontStyle, RFont>();
			}
			return result;
		}

		private RFont CreateFont(string family, double size, RFontStyle style)
		{
			RFontFamily value;
			try
			{
				return _existingFontFamilies.TryGetValue(family, out value) ? _adapter.CreateFont(value, size, style) : _adapter.CreateFont(family, size, style);
			}
			catch
			{
				return _existingFontFamilies.TryGetValue(family, out value) ? _adapter.CreateFont(value, size, RFontStyle.Regular) : _adapter.CreateFont(family, size, RFontStyle.Regular);
			}
		}
	}
}
