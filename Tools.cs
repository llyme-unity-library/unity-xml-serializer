using System;
using UnityEngine;

namespace UnityXmlSerializer
{
	public static partial class Tools
	{
		public readonly static Type NON_SERIALIZED =
			typeof(NonSerializedAttribute);
		public readonly static Type SERIALIZE_FIELD =
			typeof(SerializeField);
		public readonly static Type TYPE =
			typeof(Type);
		public readonly static Type COMPONENT =
			typeof(Component);
		public readonly static Type GAME_OBJECT =
			typeof(GameObject);
	}
}
