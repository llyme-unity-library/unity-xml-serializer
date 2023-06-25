using System;

namespace UnityXmlSerializer
{
	/// <summary>
	/// When deserializing,
	/// the deserializer will always create
	/// a new object and replace any existing value in the field.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class IgnoreParentValueAttribute : Attribute
	{
		public static readonly Type TYPE =
			typeof(IgnoreParentValueAttribute);
	}
}
