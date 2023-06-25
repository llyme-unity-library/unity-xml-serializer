using System;
using TextHelper;
using UnityEngine;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		/// <summary>
		/// Stringifies the given object.
		/// </summary>
		public bool Stringify
			(object value,
			out string text)
		{
			// Try the resolver.
			
			if (StringifyResolver(value, out text))
				return true;
					
					
			if (value == null)
			{
				text = null;
				return false;
			}

			switch (value)
			{
				case decimal _:
				case uint _:
				case short _:
				case ushort _:
				case int _:
				case string _:
				case long _:
				case double _:
				case float _:
				case bool _:
					text = value?.ToString();
					return true;

				case Vector4 vector4:
					text = vector4.ToString("F9");
					return true;

				case Vector3 vector3:
					text = vector3.ToString("F9");
					return true;

				case Vector2 vector2:
					text = vector2.ToString("F9");
					return true;

				case Vector2Int vector2int:
					text = vector2int.ToString();
					return true;

				case Quaternion quaternion:
					text = quaternion.ToString("F9");
					return true;

				case Color color:
					text = color.ToString("F9");
					return true;

				case Bounds bounds:
					text = bounds.Stringify();
					return true;

				case Enum @enum:
					text = Convert.ToInt64(@enum).ToString();
					return true;
			}
			
			text = null;
			return false;
		}
	}
}
