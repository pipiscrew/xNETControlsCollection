using System;
using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Adapters;
using TheArtOfDev.HtmlRenderer.Core.Entities;
using TheArtOfDev.HtmlRenderer.Core.Parse;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core
{
	public sealed class CssData
	{
		private static readonly List<CssBlock> _emptyArray = new List<CssBlock>();

		private readonly Dictionary<string, Dictionary<string, List<CssBlock>>> _mediaBlocks = new Dictionary<string, Dictionary<string, List<CssBlock>>>(StringComparer.InvariantCultureIgnoreCase);

		internal IDictionary<string, Dictionary<string, List<CssBlock>>> MediaBlocks
		{
			get
			{
				return _mediaBlocks;
			}
		}

		internal CssData()
		{
			_mediaBlocks.Add("all", new Dictionary<string, List<CssBlock>>(StringComparer.InvariantCultureIgnoreCase));
		}

		public static CssData Parse(RAdapter adapter, string stylesheet, bool combineWithDefault = true)
		{
			CssParser cssParser = new CssParser(adapter);
			return cssParser.ParseStyleSheet(stylesheet, combineWithDefault);
		}

		public bool ContainsCssBlock(string className, string media = "all")
		{
			Dictionary<string, List<CssBlock>> value;
			return _mediaBlocks.TryGetValue(media, out value) && value.ContainsKey(className);
		}

		public IEnumerable<CssBlock> GetCssBlock(string className, string media = "all")
		{
			List<CssBlock> value = null;
			Dictionary<string, List<CssBlock>> value2;
			if (_mediaBlocks.TryGetValue(media, out value2))
				value2.TryGetValue(className, out value);
			return value ?? _emptyArray;
		}

		public void AddCssBlock(string media, CssBlock cssBlock)
		{
			Dictionary<string, List<CssBlock>> value;
			if (!_mediaBlocks.TryGetValue(media, out value))
			{
				value = new Dictionary<string, List<CssBlock>>(StringComparer.InvariantCultureIgnoreCase);
				_mediaBlocks.Add(media, value);
			}
			if (!value.ContainsKey(cssBlock.Class))
			{
				List<CssBlock> list = new List<CssBlock>();
				list.Add(cssBlock);
				value[cssBlock.Class] = list;
				return;
			}
			bool flag = false;
			List<CssBlock> list2 = value[cssBlock.Class];
			foreach (CssBlock item in list2)
			{
				if (item.EqualsSelector(cssBlock))
				{
					flag = true;
					item.Merge(cssBlock);
					break;
				}
			}
			if (!flag)
			{
				if (cssBlock.Selectors == null)
					list2.Insert(0, cssBlock);
				else
					list2.Add(cssBlock);
			}
		}

		public void Combine(CssData other)
		{
			ArgChecker.AssertArgNotNull(other, "other");
			foreach (KeyValuePair<string, Dictionary<string, List<CssBlock>>> mediaBlock in other.MediaBlocks)
			{
				foreach (KeyValuePair<string, List<CssBlock>> item in mediaBlock.Value)
				{
					foreach (CssBlock item2 in item.Value)
					{
						AddCssBlock(mediaBlock.Key, item2);
					}
				}
			}
		}

		public CssData Clone()
		{
			CssData cssData = new CssData();
			foreach (KeyValuePair<string, Dictionary<string, List<CssBlock>>> mediaBlock in _mediaBlocks)
			{
				Dictionary<string, List<CssBlock>> dictionary = new Dictionary<string, List<CssBlock>>(StringComparer.InvariantCultureIgnoreCase);
				foreach (KeyValuePair<string, List<CssBlock>> item in mediaBlock.Value)
				{
					List<CssBlock> list = new List<CssBlock>();
					foreach (CssBlock item2 in item.Value)
					{
						list.Add(item2.Clone());
					}
					dictionary[item.Key] = list;
				}
				cssData._mediaBlocks[mediaBlock.Key] = dictionary;
			}
			return cssData;
		}
	}
}
