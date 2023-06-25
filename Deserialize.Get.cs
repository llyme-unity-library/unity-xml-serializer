using System;
using System.Reflection;
using System.Xml;
using TypeHelper;
using UnityEngine;
using XmlHelper;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		private Type Get_Type(XmlNode node)
		{
			string assemblyName = node.Attributes.String("assembly");
			string typeName = node.Attributes.String("type");
			Type type = TypeHelper.Generic.FromFullName(
				assemblyName,
				typeName
			);

			if (type == null)
				Debug.LogWarning($"Unknown data type `{assemblyName}.{typeName}`!");

			return type;
		}
		
		private Action<object> Get_Members_Field
			(XmlNode node,
			Type type,
			object value,
			out object memberValue)
		{
			FieldInfo info =
				type.GetFieldInHierarchy(
					node.Name,
					TypeHelper.Generic.GENERIC_MEMBER_FLAG
				);

			if (info == null)
			{
				memberValue = null;
				return null;
			}

			void Setter(object newValue) =>
				info.SetValue(value, newValue);

			memberValue =
				value.HasAttribute(IgnoreParentValueAttribute.TYPE)
				? null
				: info.GetValue(value);
			return Setter;
		}

		private Action<object> Get_Members_Property
			(XmlNode node,
			Type type,
			object value,
			out object memberValue)
		{
			PropertyInfo info =
				type.GetPropertyInHierarchy(
					node.Name,
					TypeHelper.Generic.GENERIC_MEMBER_FLAG
				);

			if (info == null)
			{
				memberValue = null;
				return null;
			}

			void Setter(object newValue) =>
				info.SetValue(value, newValue);

			memberValue =
				value.HasAttribute(IgnoreParentValueAttribute.TYPE)
				? null
				: info.GetValue(value);
			return Setter;
		}
		
		private bool Get_BasicType
			(XmlNode node,
			Type type,
			out object value)
		{
			string raw = node.InnerText;
			
			if (TryStringifyResolver(type, raw, out object resolved_value)) 
			{
				value = resolved_value;
				return true;
			}
			
			if (node.EmptyOrText())
				return TryParseString(
					type,
					raw,
					out value
				);

			value = default;
			return false;
		}
	}
}
