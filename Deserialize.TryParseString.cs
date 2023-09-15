using System;
using TextHelper;
using UnityEngine;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{		
		/// <summary>
		/// Tries to parse a stringified object
		/// to the given type.
		/// </summary>
		public static bool TryParseString
		(Type type,
		string raw,
		out object value)
		{
			if (type.IsEnum)
			{
				if (long.TryParse(raw, out long value0))
				{
					value = Enum.ToObject(type, value0);
					return true;
				}

				value = null;
				return false;
			}
			
			if (type == typeof(byte))
			{
				value =
					byte.TryParse(raw, out byte result)
					? result
					: (byte)0;
				return true;
			}

			if (type == typeof(string))
			{
				value = raw;
				return true;
			}

			if (type == typeof(bool))
			{
				value =
					bool.TryParse(raw, out bool result) &&
					result;
				return true;
			}

			if (type == typeof(int) ||
				type.IsSubclassOf(typeof(Enum)))
			{
				value = int.TryParse(raw, out int result)
					? result
					: 0;
				return true;
			}

			if (type == typeof(uint))
			{
				value = uint.TryParse(raw, out uint result)
					? result
					: 0;
				return true;
			}

			if (type == typeof(float))
			{
				value = float.TryParse(raw, out float result)
					? result
					: 0f;
				return true;
			}
			
			if (type == typeof(LayerMask))
			{
				value = (LayerMask)raw.Int();
				return true;
			}

			if (type == typeof(Vector2))
			{
				value = raw.Vector2();
				return true;
			}

			if (type == typeof(Vector2Int))
			{
				value = raw.Vector2Int();
				return true;
			}

			if (type == typeof(Vector3))
			{
				value = raw.Vector3();
				return true;
			}

			if (type == typeof(Vector3Int))
			{
				value = raw.Vector3Int();
				return true;
			}

			if (type == typeof(Vector4))
			{
				value = raw.Vector4();
				return true;
			}

			if (type == typeof(Quaternion))
			{
				value = raw.Quaternion();
				return true;
			}

			if (type == typeof(Color))
			{
				value = raw.Color();
				return true;
			}
			
			if (type == typeof(Color32))
			{
				value = raw.Color32();
				return true;
			}

			if (typeof(ScriptableObject).IsAssignableFrom(type))
			{
				value = Resources.Load(raw);
				return true;
			}

			if (type == typeof(GameObject))
			{
				value = Resources.Load<GameObject>(raw);
				return true;
			}

			if (type == typeof(Bounds))
			{
				value = raw.Bounds();
				return true;
			}

			value = null;
			return false;
		}
	}
}
