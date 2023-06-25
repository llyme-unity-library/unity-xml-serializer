using System;

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
		protected virtual bool StringifyResolver(object value, out string result)
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
		protected virtual bool WriteSpecialResolver(Type type, object @object)
		{
			return false;
		}
	}
}
