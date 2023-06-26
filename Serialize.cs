using System;
using System.Collections.Generic;
using System.Xml;

namespace UnityXmlSerializer
{
	/// <summary>
	/// Serializes an object.
	/// <br></br>
	/// Preserves references.
	/// <br></br>
	/// Serializes public fields by default.
	/// <br></br>
	/// `SerializeField` attribute allows
	/// non-literal fields and properties to be serialized.
	/// </summary>
	public abstract partial class Serialize<T> where T : Serialize<T>
	{
		private readonly List<object> reference = new();
		private readonly Dictionary<object, int> deferment = new();

		public XmlWriter Writer { get; private set; }

		public object Object { get; private set; }

		public Type ObjectType { get; private set; }

		public string RootNodeName { get; private set; }
	}
}
