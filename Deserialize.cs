using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace UnityXmlSerializer
{
	public abstract partial class Deserialize<T> where T : Deserialize<T>
	{
		private readonly Dictionary<int, object> reference = new();
		/// <summary>
		/// List of objects that are waiting
		/// for another object to be deserialized.
		/// <br></br>
		/// The key is the ID of the required object.
		/// </summary>
		private readonly List<Deferment> deferred = new();
		private readonly Dictionary<int, XmlNode> components = new();
		private readonly List<IAfterFullDeserialize> afterFullDeserialize = new();
		/// <summary>
		/// These are called after a component is added
		/// to a GameObject,
		/// before the members are serialized.
		/// </summary>
		protected readonly List<Action<Component>> afterComponentAdded = new();
		private readonly Stack<Payload> payloads = new();

		public delegate void OnAfterDeserializeDelegate(object @object);
		
		public delegate void OnFinishDelegate();

		/// <summary>
		/// Called whenever something is fully deserialized,
		/// including its members.
		/// <br></br>
		/// This also includes a GameObject's components.
		/// </summary>
		public event OnAfterDeserializeDelegate OnAfterDeserialize;
		
		public event OnFinishDelegate OnFinish;

		/// <summary>
		/// Base parent for GameObjects.
		/// </summary>
		public Transform Parent { get; private set; } = null;

		public XmlNode Node { get; private set; } = null;

		public object Value { get; private set; } = null;

		public bool Done { get; private set; } = false;
	}
}
