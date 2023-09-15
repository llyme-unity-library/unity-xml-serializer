using System;
using UnityEngine;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		/// <summary>
		/// Allows a user to create user-defined stringification.
		/// <br>
		/// If an instance is unknown,
		/// use this to resolve them.
		/// </summary>
		protected virtual bool StringifyResolver
		(object value,
		out string result)
		{
			result = null;
			return false;
		}
		
		/// <summary>
		/// Allows a user to create user-defined stringification.
		/// <br>
		/// If an instance is unknown,
		/// use this to resolve them.
		/// </summary>
		protected virtual bool WriteSpecialResolver
		(Type type,
		object @object)
		{
			return false;
		}
		
		protected virtual void BeforeObjectMembers
		(Type type,
		object @object)
		{
			if (@object is EdgeCollider2D edge)
			{
				Writer.WriteStartElement("Points");
				{
					foreach (Vector2 point in edge.points)
						Writer.WriteElementString(
							"Point",
							point.ToString()
						);
				}
				Writer.WriteEndElement();
			}
		}
	}
}
