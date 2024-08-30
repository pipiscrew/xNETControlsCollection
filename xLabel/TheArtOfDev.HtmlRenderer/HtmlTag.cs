using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Dom
{
	internal sealed class HtmlTag
	{
		private readonly string _name;

		private readonly bool _isSingle;

		private readonly Dictionary<string, string> _attributes;

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public Dictionary<string, string> Attributes
		{
			get
			{
				return _attributes;
			}
		}

		public bool IsSingle
		{
			get
			{
				return _isSingle;
			}
		}

		public HtmlTag(string name, bool isSingle, Dictionary<string, string> attributes = null)
		{
			ArgChecker.AssertArgNotNullOrEmpty(name, "name");
			_name = name;
			_isSingle = isSingle;
			_attributes = attributes;
		}

		public bool HasAttributes()
		{
			return _attributes != null && _attributes.Count > 0;
		}

		public bool HasAttribute(string attribute)
		{
			return _attributes != null && _attributes.ContainsKey(attribute);
		}

		public string TryGetAttribute(string attribute, string defaultValue = null)
		{
			return (_attributes == null || !_attributes.ContainsKey(attribute)) ? defaultValue : _attributes[attribute];
		}

		public override string ToString()
		{
			return string.Format("<{0}>", _name);
		}
	}
}
