using System.Collections.Generic;
using TheArtOfDev.HtmlRenderer.Core.Utils;

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
	public sealed class CssBlock
	{
		private readonly string _class;

		private readonly Dictionary<string, string> _properties;

		private readonly List<CssBlockSelectorItem> _selectors;

		private readonly bool _hover;

		public string Class
		{
			get
			{
				return _class;
			}
		}

		public List<CssBlockSelectorItem> Selectors
		{
			get
			{
				return _selectors;
			}
		}

		public IDictionary<string, string> Properties
		{
			get
			{
				return _properties;
			}
		}

		public bool Hover
		{
			get
			{
				return _hover;
			}
		}

		public CssBlock(string @class, Dictionary<string, string> properties, List<CssBlockSelectorItem> selectors = null, bool hover = false)
		{
			ArgChecker.AssertArgNotNullOrEmpty(@class, "@class");
			ArgChecker.AssertArgNotNull(properties, "properties");
			_class = @class;
			_selectors = selectors;
			_properties = properties;
			_hover = hover;
		}

		public void Merge(CssBlock other)
		{
			ArgChecker.AssertArgNotNull(other, "other");
			foreach (string key in other._properties.Keys)
			{
				_properties[key] = other._properties[key];
			}
		}

		public CssBlock Clone()
		{
			return new CssBlock(_class, new Dictionary<string, string>(_properties), (_selectors != null) ? new List<CssBlockSelectorItem>(_selectors) : null);
		}

		public bool Equals(CssBlock other)
		{
			if (other == null)
				return false;
			if (this == other)
				return true;
			if (!object.Equals(other._class, _class))
				return false;
			if (!object.Equals(other._properties.Count, _properties.Count))
				return false;
			foreach (KeyValuePair<string, string> property in _properties)
			{
				if (other._properties.ContainsKey(property.Key))
				{
					if (!object.Equals(other._properties[property.Key], property.Value))
						return false;
					continue;
				}
				return false;
			}
			if (!EqualsSelector(other))
				return false;
			return true;
		}

		public bool EqualsSelector(CssBlock other)
		{
			if (other != null)
			{
				if (this != other)
				{
					if (other.Hover == Hover)
					{
						if (other._selectors != null || _selectors == null)
						{
							if (other._selectors == null || _selectors != null)
							{
								if (other._selectors != null && _selectors != null)
								{
									if (!object.Equals(other._selectors.Count, _selectors.Count))
										return false;
									for (int i = 0; i < _selectors.Count; i++)
									{
										if (object.Equals(other._selectors[i].Class, _selectors[i].Class))
										{
											if (!object.Equals(other._selectors[i].DirectParent, _selectors[i].DirectParent))
												return false;
											continue;
										}
										return false;
									}
								}
								return true;
							}
							return false;
						}
						return false;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (this == obj)
				return true;
			if (obj.GetType() != typeof(CssBlock))
				return false;
			return Equals((CssBlock)obj);
		}

		public override int GetHashCode()
		{
			return (((_class != null) ? _class.GetHashCode() : 0) * 397) ^ ((_properties != null) ? _properties.GetHashCode() : 0);
		}

		public override string ToString()
		{
			string text = _class + " { ";
			foreach (KeyValuePair<string, string> property in _properties)
			{
				text += string.Format("{0}={1}; ", property.Key, property.Value);
			}
			return text + " }";
		}
	}
}
