using System;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		/// <summary>
		/// Writes the type and assembly full names.
		/// </summary>
		protected void Write_Type
			(Type type)
		{
			Writer.WriteAttributeString("type", type.FullName);
			Writer.WriteAttributeString("assembly", type.Assembly.FullName);
		}

		/// <summary>
		/// Writes a null indicator if applicable.
		/// </summary>
		protected bool Write_Null
			(Type type,
			object @object)
		{
			if (@object == null ||
				// For some reason,
				// UnityEngine uses a 'ghost' object to fill up anything `null`.
				@object is UnityEngine.Object object0 && object0 == null)
			{
				Write_Type(type);
				Writer.WriteAttributeString("null", "true");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Writes for primitive/common/base types
		/// if applicable.
		/// </summary>
		protected bool Write_Common
			(Type type,
			object @object)
		{
			if (Stringify(@object, out string text))
			{
				Write_Type(type);
				Writer.WriteString(text);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Writes special case objects when applicable.
		/// </summary>
		protected bool Write_Special
		(Type type,
		object @object)
		{
			switch (@object)
			{
				case Type type0:
					Write_Type(Tools.TYPE);
					Writer.WriteElementString("Type", type0.FullName);
					Writer.WriteElementString("Assembly", type0.Assembly.FullName);
					return true;
			}
			
			if (WriteSpecialResolver(type, @object))
				return true;
			
			return false;
		}
	}
}
