using System;
using System.Xml;
using UnityEngine;
using XmlHelper;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		protected virtual bool BeforeDeserializeGameObjectResolver
		(XmlNode node,
		object value)
		{
			if (value == null)
				return false;

			XmlNode members = node.Element("Members");

			switch (value)
			{
				case GameObject @object:
					@object.name = members.StringOf("name");
					@object.layer = members.IntOf("layer");
					@object.isStatic = members.BoolOf("isStatic");
					@object.tag = members.StringOf("tag");
					@object.SetActive(members.BoolOf("activeSelf"));

					return false;

				case Transform transform:
					transform.localPosition = members.Vector3Of("localPosition");
					transform.localRotation = members.QuaternionOf("localRotation");
					transform.localScale = members.Vector3Of("localScale");
					return false;
			}

			return true;
		}
			
		/// <summary>
		/// Allows a user to create user-defined de-stringification.
		/// <br>
		/// If a type is unknown,
		/// use this to resolve them.
		/// </summary>
		protected virtual bool TryStringifyResolver
		(Type type,
		string raw,
		out object value)
		{
			value = null;
			return false;
		}
		
		/// <summary>
		/// Allows a user to create user-defined object instantiation.
		/// <br>
		/// If a type is unknown,
		/// use this to resolve them.
		/// </summary>
		protected virtual bool ObjectResolver
		(Type type,
		XmlNode node,
		out object result)
		{
			result = null;
			return false;
		}
		
		protected virtual void BeforeDeserializeComponentResolver
		(Type type, GameObject gameObject)
		{
		}
	}
}
