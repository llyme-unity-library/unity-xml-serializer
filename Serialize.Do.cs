using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		public void Do() =>
			Do_Internal(
				ObjectType,
				Object,
				false,
				RootNodeName
			);

		private void Do_Internal
			(Type type,
			object @object,
			bool inGameObject,
			string name)
		{
			if (Attribute.IsDefined(type, Tools.NON_SERIALIZED))
				return;

			Writer.WriteStartElement(name);
			{
				Do_Object(type, @object, inGameObject);
			}
			Writer.WriteEndElement();
		}

		private void Do_Object_Type
			(Type type)
		{
			Writer.WriteAttributeString("type", type.FullName);
			Writer.WriteAttributeString("assembly", type.Assembly.FullName);
		}

		private void Do_Object
			(Type type,
			object @object,
			bool inGameObject)
		{
			// Null

			if (Write_Null(type, @object))
				return;


			// Primitive/Common/Base Types

			if (Write_Common(type, @object))
				return;


			// Special Types

			if (Write_Special(type, @object))
				return;


			// Referencing

			int index = reference.IndexOf(@object);

			if (index != -1)
			{
				Writer.WriteAttributeString("ref-id", index.ToString());
				return;
			}


			// Deferment

			if (Defer_Object_Component(@object, inGameObject))
				return;


			if (deferment.TryGetValue(@object, out int id))
			{
				// The object was deferred.
				// Use the assigned index to
				// align with referencing.
				deferment.Remove(@object);
				reference[id] = @object;
			}
			else
			{
				id = reference.Count;
				reference.Add(@object);
			}

			Writer.WriteAttributeString("id", id.ToString());
			Do_Object_Type(type);


			// Custom Types

			switch (@object)
			{
				case UnityEventBase @event:
					Do_UnityEventBase(@event);
					return;

				case GameObject gameObject:
					Do_GameObject(gameObject);
					break;

				case IDictionary dictionary:
					Do_Dictionary(type, dictionary);
					break;

				case IEnumerable enumerable:
					Do_Enumerable(type, enumerable);
					break;
			}

			Do_Members(@object);
		}

		private void Do_Members
			(object @object)
		{
			bool PropertyFilter(string key, PropertyInfo info)
			{
				if (Filter.GameObjectFilter(@object, key, out bool flag))
					return flag;

				return Attribute.IsDefined(info, Tools.SERIALIZE_FIELD);
			}

			bool FieldFilter(string key, FieldInfo info)
			{
				if (Filter.GameObjectFilter(@object, key, out bool flag))
					return flag;

				if (info.IsLiteral)
					return false;

				if (Attribute.IsDefined(info, Tools.NON_SERIALIZED))
					return false;

				if (Attribute.IsDefined(info, Tools.SERIALIZE_FIELD))
					return true;

				if (info.IsPrivate)
					return false;

				if (info.IsInitOnly)
					return false;

				return true;
			}

			IEnumerable<KeyValuePair<string, PropertyInfo>> properties =
				TypeHelper.Objects.AllProperties(@object, predicate: PropertyFilter);
			IEnumerable<KeyValuePair<string, FieldInfo>> fields =
				TypeHelper.Objects.AllFields(@object, predicate: FieldFilter);

			if (!fields.Any() &&
				!properties.Any())
				return;

			Writer.WriteStartElement("Members");
			{
				foreach (KeyValuePair<string, PropertyInfo> pair in properties)
					Do_Internal(
						pair.Value.PropertyType,
						pair.Value.GetValue(@object),
						false,
						pair.Key
					);

				foreach (KeyValuePair<string, FieldInfo> pair in fields)
					Do_Internal(
						pair.Value.FieldType,
						pair.Value.GetValue(@object),
						false,
						pair.Key
					);
			}
			Writer.WriteEndElement();
		}

		private void Do_Dictionary
			(Type type,
			IDictionary dictionary)
		{
			Writer.WriteStartElement("Dictionary");
			{
				Type[] genericTypes =
					type.GetGenericArguments();

				foreach (object key in dictionary.Keys)
				{
					object value = dictionary[key];

					Writer.WriteStartElement("Item");
					{
						Do_Internal(
							key != null
							? key.GetType()
							: genericTypes[0],
							key,
							false,
							"Key"
						);
						Do_Internal(
							value != null
							? value.GetType()
							: genericTypes[1],
							value,
							false,
							"Value"
						);
					}
					Writer.WriteEndElement();
				}
			}
			Writer.WriteEndElement();
		}

		private void Do_Enumerable
			(Type type,
			IEnumerable enumerable)
		{
			Type genericType;

			if (type.HasElementType)
				genericType = type.GetElementType();

			else if (type.ContainsGenericParameters)
				genericType = type.GetGenericParameterConstraints()[0];

			else if (type.GenericTypeArguments.Length > 0)
				genericType = type.GenericTypeArguments[0];

			else
				return;

			Writer.WriteStartElement("Enumerable");
			{
				foreach (object key in enumerable)
					Do_Internal(
						key != null
						? key.GetType()
						: genericType,
						key,
						false,
						"Item"
					);
			}
			Writer.WriteEndElement();
		}
	}
}
