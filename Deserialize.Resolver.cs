using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TextHelper;
using UnityEngine;
using XmlHelper;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		bool BDGOR_Default(XmlNode node, object value)
		{
			XmlNode members = node.Element("Members");
			
			switch (value)
			{
				case Animator animator:
					void AnimatorOnFinish()
					{
						RuntimeAnimatorController rac =
							animator.runtimeAnimatorController;
						
						if (rac == null)
							return;
						
						animator.runtimeAnimatorController = null;
						animator.runtimeAnimatorController = rac;
					}
				
					OnFinish += AnimatorOnFinish;
					return false;
				
				case GameObject @object:
					@object.name = members.StringOf("name");
					@object.layer = members.IntOf("layer");
					@object.isStatic = members.BoolOf("isStatic", false);
					@object.tag = members.StringOf("tag", @default: "Untagged");
					@object.SetActive(members.BoolOf("activeSelf", false));

					return true;

				case Transform transform:
					transform.localPosition = members.Vector3Of("localPosition");
					transform.localRotation = members.QuaternionOf("localRotation");
					transform.localScale = members.Vector3Of("localScale");
					return true;
				
				case EdgeCollider2D edge:
					List<Vector2> points =
						node
						.Element("Points")
						.Elements("Point")
						.Select(v => v.InnerText.Vector2())
						.ToList();

					void Listener() =>
						edge.SetPoints(points);

					OnFinish += Listener;
					return false;
			}
			
			return false;
		}
		
		/// <summary>
		/// Allows resolving how Unity GameObjects are parsed.
		/// </summary>
		/// <returns>
		/// `true` to prevent defaults.
		/// </returns>
		protected virtual bool BeforeDeserializeGameObjectResolver
		(XmlNode node,
		object value)
		{
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
		(XmlNode node, Type type, GameObject gameObject)
		{
		}
	}
}
