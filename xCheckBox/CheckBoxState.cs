using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace xCollection
{
	[DebuggerStepThrough]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Description("An abstract class used to define various states within x CheckBox.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class CheckBoxState : INotifyPropertyChanged, ICustomTypeDescriptor
	{
		private string _name;

		private Color _fillColor;

		private Color _borderColor;

		private Color _checkmarkColor;

		private int _checkmarkThickness;

		private int _borderRadius;

		private int _borderThickness;

		private bool _useBorderThicknessForCheckmark;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int CheckBoxSize = 21;

		private string[] _excludeBrowsableProperties = new string[3] { "CheckmarkColor", "CheckmarkThickness", "UseBorderThicknessForCheckmark" };

		private string[] _allBrowsableProperties = new string[0];

		private string[] _excludeBrowsableEvents = new string[1] { "PropertyChanged" };

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Always)]
		public virtual string Name
		{
			get
			{
				return _name;
			}
		}

		[Category("x Properties")]
		[Browsable(true)]
		[NotifyParentProperty(true)]
		[Description("Sets the checkbox inner fill color.")]
		public virtual Color CheckBoxColor
		{
			get
			{
				return _fillColor;
			}
			set
			{
				_fillColor = value;
				NotifyPropertyChanged("CheckBoxColor");
			}
		}

		[Description("Indicates the checkbox's border radius. The higher the value, the more curved the edges appear. Maximum value is 20.")]
		[Category("x Properties")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		public virtual int BorderRadius
		{
			get
			{
				return _borderRadius;
			}
			set
			{
				if (value > 0 && value <= 20)
					_borderRadius = value;
				NotifyPropertyChanged("BorderRadius");
			}
		}

		[Category("x Properties")]
		[Description("Applies the checkbox border thickness. The higher the value, the more the thickness. Maximum value is 21.")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		public virtual int BorderThickness
		{
			get
			{
				return _borderThickness;
			}
			set
			{
				if (value == 0)
					value = 1;
				if (CheckBoxSize <= 21)
				{
					if (value <= 2)
					{
						_borderThickness = value;
						if (UseBorderThicknessForCheckmark)
							_checkmarkThickness = value;
					}
				}
				else if (value <= 10)
				{
					_borderThickness = value;
					if (UseBorderThicknessForCheckmark)
						_checkmarkThickness = value;
				}
				_borderThickness = value;
				NotifyPropertyChanged("BorderThickness");
			}
		}

		[Description("Sets the checkbox border color.")]
		[Category("x Properties")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		public virtual Color BorderColor
		{
			get
			{
				return _borderColor;
			}
			set
			{
				_borderColor = value;
				NotifyPropertyChanged("BorderColor");
			}
		}

		[NotifyParentProperty(true)]
		[Category("x Properties")]
		[Description("Sets the CheckBox checkmark color.")]
		[Browsable(true)]
		public virtual Color CheckmarkColor
		{
			get
			{
				return _checkmarkColor;
			}
			set
			{
				_checkmarkColor = value;
				NotifyPropertyChanged("CheckmarkColor");
			}
		}

		[Category("x Properties")]
		[Description("Gets or sets the checkmark thickness. Maximum value is 20.")]
		[NotifyParentProperty(true)]
		[Browsable(true)]
		public virtual int CheckmarkThickness
		{
			get
			{
				return _checkmarkThickness;
			}
			set
			{
				if (value <= 20)
				{
					if (UseBorderThicknessForCheckmark)
					{
						if (value != BorderThickness)
							throw new ArgumentException("To set the \"CheckmarkThickness\" with a different value than \"BorderThickness\", please set the \"UseBorderThicknessForCheckmark\" property to False.");
					}
					else
						_checkmarkThickness = value;
				}
				_checkmarkThickness = value;
				NotifyPropertyChanged("CheckmarkThickness");
			}
		}

		[Category("x Properties")]
		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[RefreshProperties(RefreshProperties.All)]
		[Description("Gets or sets a value indicating whether to use the border thickness of the checkbox as the border thickness of the checkmark.")]
		[Browsable(true)]
		public virtual bool UseBorderThicknessForCheckmark
		{
			get
			{
				return _useBorderThicknessForCheckmark;
			}
			set
			{
				if (value)
					_checkmarkThickness = _borderThickness;
				_useBorderThicknessForCheckmark = value;
				NotifyPropertyChanged("UseBorderThicknessForCheckmark");
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event PropertyChangedEventHandler PropertyChanged;

		public CheckBoxState(string name = "(Undefined)")
		{
			_name = name;
		}

		private void NotifyPropertyChanged(string propertyName = "")
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return BorderColor.ToString() + "; " + CheckBoxColor.ToString() + "; " + CheckmarkColor.ToString() + "; " + BorderRadius + "; " + BorderThickness + "; " + CheckmarkThickness + "; " + UseBorderThicknessForCheckmark;
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public string GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			EventDescriptorCollection events = TypeDescriptor.GetEvents(this, attributes, true);
			return FilterEvents(events);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EventDescriptorCollection GetEvents()
		{
			EventDescriptorCollection events = TypeDescriptor.GetEvents(this, true);
			return FilterEvents(events);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, attributes, true);
			return FilterProperties(properties);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public PropertyDescriptorCollection GetProperties()
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this, true);
			return FilterProperties(properties);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		private PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection originalCollection)
		{
			IEnumerable<PropertyDescriptor> enumerable = from p in originalCollection.OfType<PropertyDescriptor>()
				where !_allBrowsableProperties.Contains(p.Name)
				select p;
			enumerable = ((!(_name == "OnUncheck") && !(_name == "OnHoverUnchecked")) ? (from p in originalCollection.OfType<PropertyDescriptor>()
				where !_allBrowsableProperties.Contains(p.Name)
				select p) : (from p in originalCollection.OfType<PropertyDescriptor>()
				where !_excludeBrowsableProperties.Contains(p.Name)
				select p));
			PropertyDescriptor[] properties = enumerable.ToArray();
			return new PropertyDescriptorCollection(properties);
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		private EventDescriptorCollection FilterEvents(EventDescriptorCollection origEvents)
		{
			IEnumerable<EventDescriptor> enumerable = (enumerable = from e in origEvents.OfType<EventDescriptor>()
				where !_excludeBrowsableEvents.Contains(e.Name)
				select e);
			EventDescriptor[] events = enumerable.ToArray();
			return new EventDescriptorCollection(events);
		}
	}
}
