using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using XmlHelper;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		private IEnumerator Do_GameObject_Children
			(XmlNode node,
			GameObject gameObject)
		{
			IEnumerable<XmlNode> childNodes =
				node
				.Element("Children")
				.Elements("Child");

			foreach (XmlNode childNode in childNodes)
			{
				if (childNode.Attributes.TryInt("ref-id", out int refid))
				{
					// Referencing an existing object.

					bool Condition() =>
						reference.ContainsKey(refid);

					IEnumerator Action()
					{
						GameObject child = reference[refid] as GameObject;
						child.transform.SetParent(gameObject.transform);

						yield break;
					}

					yield return Defer(Condition, Action());
					continue;
				}

				GameObject child = new();

				child.transform.SetParent(gameObject.transform);

				reference[childNode.Attributes.Int("id")] = child;

				yield return Do_Parameters(childNode, Tools.GAME_OBJECT, child);
				yield return Do_Components(childNode, child);
				yield return Do_GameObject_Children(childNode, child);

				OnAfterDeserialize?.Invoke(child);
			}
		}

		private IEnumerator Do_Component
			(XmlNode node,
			GameObject gameObject)
		{
			int deferredID = -1;

			if (node.Attributes.TryInt("ref-id", out int id))
			{
				bool flag =
					components.TryGetValue(
						id,
						out node
					);

				if (!flag)
					// Asking for a reference,
					// but doesn't exist.
					yield break;

				deferredID = id;
			}

			Type componentType =
				TypeHelper.Generic.FromFullName(
					node.Attributes.String("assembly"),
					node.Attributes.String("type")
				);

			if (componentType == null)
				yield break;

			BeforeDeserializeComponentResolver(node, componentType, gameObject);

			bool hasComponent =
				gameObject.TryGetComponent(
					componentType,
					out Component component
				);

			if (!hasComponent)
				component = gameObject.AddComponent(componentType);

			foreach (Action<Component> action in afterComponentAdded)
			{
				action(component);
				yield return null;
			}

			afterComponentAdded.Clear();

			if (component == null)
			{
				Debug.LogWarning(
					$"Unable to instantiate component `{componentType.Name}`!"
				);
				yield break;
			}

			reference[node.Attributes.Int("id")] = component;

			yield return Do_Parameters(
				node,
				componentType,
				component
			);

			OnAfterDeserialize?.Invoke(component);
		}

		private IEnumerator Do_Components
		(XmlNode node,
		GameObject gameObject)
		{
			IEnumerable<XmlNode> componentNodes =
				node
				.Element("Components")
				.Elements("Component");

			foreach (XmlNode componentNode in componentNodes)
				yield return Do_Component(componentNode, gameObject);
		}

		private IEnumerator Do_UnityEventBase_Listener_Argument
		(XmlNode node,
		PersistentListenerMode mode)
		{
			Payload resultPayload = new();
			payloads.Push(resultPayload);

			switch (mode)
			{
				case PersistentListenerMode.Void:
					yield break;

				case PersistentListenerMode.Object:
					yield return Do_Internal(
						node.Element("Argument"),
						null
					);

					Payload argumentPayload = payloads.Pop();
					resultPayload.boolean = true;
					resultPayload.@object = argumentPayload;
					yield break;

				case PersistentListenerMode.Int:
					resultPayload.boolean = true;
					resultPayload.@object = node.IntOf("Argument");
					yield break;

				case PersistentListenerMode.Float:
					resultPayload.boolean = true;
					resultPayload.@object = node.FloatOf("Argument", 0f);
					yield break;

				case PersistentListenerMode.String:
					resultPayload.boolean = true;
					resultPayload.@object = node.StringOf("Argument");
					yield break;

				case PersistentListenerMode.Bool:
					resultPayload.boolean = true;
					resultPayload.@object = node.BoolOf("Argument", false);
					yield break;
			}
		}
		
		private bool Do_UnityEventBase_Listener_Method_Mode
		(MethodInfo info,
		Type parameterType)
		{
			ParameterInfo[] @params = info.GetParameters();
		
			if (parameterType == null)
				// Void
				return @params.Length == 0;
			
			return
				@params.Length == 1 &&
				@params[0].ParameterType == parameterType;
		}
		
		private bool Do_UnityEventBase_Listener_Method_Bool(MethodInfo info) =>
			Do_UnityEventBase_Listener_Method_Mode(info, typeof(bool));
		
		private bool Do_UnityEventBase_Listener_Method_Float(MethodInfo info) =>
			Do_UnityEventBase_Listener_Method_Mode(info, typeof(float));
		
		private bool Do_UnityEventBase_Listener_Method_Int(MethodInfo info) =>
			Do_UnityEventBase_Listener_Method_Mode(info, typeof(int));
		
		private bool Do_UnityEventBase_Listener_Method_String(MethodInfo info) =>
			Do_UnityEventBase_Listener_Method_Mode(info, typeof(string));

		private bool Do_UnityEventBase_Listener_Method_Object(MethodInfo info) =>
			info.GetParameters().Length == 1;

		private bool Do_UnityEventBase_Listener_Method_Void(MethodInfo info) =>
			Do_UnityEventBase_Listener_Method_Mode(info, null);

		private IEnumerator Do_UnityEventBase_Listener_Method
		(XmlNode node,
		object @event,
		Payload targetPayload,
		MethodInfo[] listenerInfos)
		{
			// Grab the event's method that will
			// add the target's method as a listener.

			MethodInfo addListenerInfo =
				typeof(UnityEventBase)
				.GetMethod(
					"AddListener",
					TypeHelper.Generic.GENERIC_MEMBER_FLAG
				);


			// Check the listener mode.

			PersistentListenerMode mode =
				(PersistentListenerMode)node.IntOf("Mode", 1);
			
			
			// Get the correct listener info.
			
			MethodInfo listenerInfo = null;
			
			switch (mode)
			{
				case PersistentListenerMode.EventDefined:
					yield return
						Do_UnityEventBase_Listener_Method_Dynamic(
							@event,
							targetPayload,
							listenerInfos
							.FirstOrDefault(Do_UnityEventBase_Listener_Method_Object),
							addListenerInfo
						);
					yield break;
					
				case PersistentListenerMode.Bool:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_Bool);
					break;
					
				case PersistentListenerMode.Float:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_Float);
					break;
					
				case PersistentListenerMode.Int:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_Int);
					break;
					
				case PersistentListenerMode.Object:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_Object);
					break;
					
				case PersistentListenerMode.String:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_String);
					break;
					
				case PersistentListenerMode.Void:
					listenerInfo =
						listenerInfos
						.FirstOrDefault(Do_UnityEventBase_Listener_Method_Void);
					break;
			}
			
			if (listenerInfo == null)
				yield break;
			
			// Check the listener mode.

			yield return
				Do_UnityEventBase_Listener_Method_Constant(
					node,
					@event,
					targetPayload,
					listenerInfo,
					addListenerInfo,
					mode
				);
		}

		private IEnumerator Do_UnityEventBase_Listener_Method_Dynamic
		(object @event,
		Payload targetPayload,
		MethodInfo listenerInfo,
		MethodInfo addListenerInfo)
		{
			if (listenerInfo == null)
				yield break;
				
			// Add the the method of the target as is.
			// The event will pass the arguments.

			addListenerInfo.Invoke(
				@event,
				new object[]
				{
					targetPayload.@object,
					listenerInfo
				}
			);

			yield break;
		}

		private IEnumerator Do_UnityEventBase_Listener_Method_Constant_Internal
		(object @event,
		Payload targetPayload,
		MethodInfo listenerInfo,
		MethodInfo addListenerInfo,
		Payload argumentPayload)
		{
			object methodTarget = null;
			MethodInfo methodInfo = null;

			// Find the `UnityEvent` base type
			// and take its generic arguments.

			Type eventType = @event.GetType();

			while (eventType.BaseType != typeof(UnityEventBase))
			{
				eventType = eventType.BaseType;
				yield return null;
			}


			// Create the corresponding delegate
			// for the number of arguments needed.
			// This will wrap the target's method
			// inside a delegate.
			// The arguments will be given
			// as `object` data type for simplicity,
			// either way they are unused.

			Type[] arguments = eventType.GetGenericArguments();

			switch (arguments.Length)
			{
				case 0:
					Action delegate0 =
						argumentPayload.boolean
						? () => listenerInfo.Invoke(
							targetPayload.@object,
							new object[] { argumentPayload.@object }
						)
						: () => listenerInfo.Invoke(
							targetPayload.@object,
							new object[] { }
						);

					methodInfo = delegate0.Method;
					methodTarget = delegate0.Target;
					break;

				case 1:
					Action<object> delegate1 =
						argumentPayload.boolean
						? _ => listenerInfo.Invoke(
							targetPayload.@object,
							new object[] { argumentPayload.@object }
						)
						: _ => listenerInfo.Invoke(
							targetPayload.@object,
							new object[] { }
						);

					methodInfo = delegate1.Method;
					methodTarget = delegate1.Target;
					break;
			}

			if (methodTarget == null ||
				methodInfo == null)
				// No corresponding delegate
				// for the number of arguments needed.
				yield break;


			// Add the target's wrapped method
			// to the event as a listener.

			addListenerInfo.Invoke(
				@event,
				new object[]
				{
					methodTarget,
					methodInfo
				}
			);
		}

		private IEnumerator Do_UnityEventBase_Listener_Method_Constant
		(XmlNode node,
		object @event,
		Payload targetPayload,
		MethodInfo listenerInfo,
		MethodInfo addListenerInfo,
		PersistentListenerMode mode)
		{
			yield return
				Do_UnityEventBase_Listener_Argument(
					node,
					mode
				);
			
			Payload argumentPayload = payloads.Pop();

			if (mode != PersistentListenerMode.Object)
			{
				yield return
					Do_UnityEventBase_Listener_Method_Constant_Internal(
						@event,
						targetPayload,
						listenerInfo,
						addListenerInfo,
						argumentPayload
					);
				yield break;
			}

			Payload objectPayload =
				argumentPayload.@object as Payload;

			bool Condition() =>
				!objectPayload.boolean;

			IEnumerator Action()
			{
				argumentPayload.@object =
					objectPayload.@object;

				return
					Do_UnityEventBase_Listener_Method_Constant_Internal(
						@event,
						targetPayload,
						listenerInfo,
						addListenerInfo,
						argumentPayload
					);
			}

			yield return Defer(Condition, Action());
		}

		private IEnumerator Do_UnityEventBase_Listener_Internal
		(XmlNode node,
		object @event,
		string methodName,
		Payload targetPayload)
		{
			if (targetPayload.@object == null)
				// Target does not exist.
				yield break;


			// Check if the method exists in the target.
			
			MethodInfo[] listenerInfos =
				targetPayload.@object
				.GetType()
				.GetMethods(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.Static |
					BindingFlags.FlattenHierarchy
				)
				.Where(v => v.Name == methodName)
				.ToArray();
			
			if (listenerInfos.Length == 0)
				// The method does not exist in the target.
				yield break;


			// Deserialize the method's credentials and
			// attempt to add as a listener.

			yield return
				Do_UnityEventBase_Listener_Method(
					node,
					@event,
					targetPayload,
					listenerInfos
				);
		}

		private IEnumerator Do_UnityEventBase_Listener
			(XmlNode node,
			object @event)
		{
			// Check if the method is provided.

			string methodName = node.StringOf("MethodName");

			if (string.IsNullOrEmpty(methodName))
				// Empty function.
				yield break;


			// Find the target in the current context.

			yield return Do_Internal(
				node.Element("Target"),
				null
			);

			Payload targetPayload = payloads.Pop();

			bool Condition() =>
				!targetPayload.boolean;

			IEnumerator Action() =>
				Do_UnityEventBase_Listener_Internal(
					node,
					@event,
					methodName,
					targetPayload
				);

			yield return Defer(Condition, Action());
		}

		private IEnumerator Do_UnityEventBase
			(XmlNode node,
			Type type,
			Payload payload)
		{
			Payload resultPayload = new();
			payloads.Push(resultPayload);

			if (!typeof(UnityEventBase).IsAssignableFrom(type))
				yield break;

			if (payload.@object == null)
				payload.@object = Activator.CreateInstance(type);

			IEnumerable<XmlNode> listeners =
				node
				.Element("Listeners")
				.Elements("Listener");

			foreach (XmlNode listenerNode in listeners)
				yield return Do_UnityEventBase_Listener(
					listenerNode,
					payload.@object
				);

			resultPayload.boolean = true;
		}

		private IEnumerator Do_GameObject
			(XmlNode node,
			Type type,
			Payload payload)
		{
			Payload resultPayload = new();
			payloads.Push(resultPayload);

			if (type != Tools.GAME_OBJECT)
				yield break;
				
			GameObject gameObject = payload.@object != null
				? payload.@object as GameObject
				: new();
			gameObject.transform.SetParent(Parent);
			payload.@object = gameObject;

			reference[node.Attributes.Int("id")] = gameObject;

			yield return Do_Parameters(node, type, gameObject);
			yield return Do_Components(node, gameObject);
			yield return Do_GameObject_Children(node, gameObject);

			resultPayload.boolean = true;
		}
	}
}
