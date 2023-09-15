using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using UnityEngine;
using XmlHelper;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		public IEnumerator Do()
		{
			if (Done)
				throw new Exception("This should only be called once!");
				
			yield return Do_Internal(Node, null);

			Payload payload = payloads.Pop();
			OnAfterDeserialize?.Invoke(payload.@object);
			Value = payload.@object;

			foreach (IAfterFullDeserialize @object in afterFullDeserialize)
			{
				@object.AfterFullDeserialize();
				yield return null;
			}
			
			OnFinish?.Invoke();

			Done = true;
		}

		/// <summary>
		/// Has a payload.
		/// <br></br>
		/// object = the deserialized object.
		/// <br></br>
		/// boolean = if the payload is deferred.
		/// </summary>
		IEnumerator Do_Internal
		(XmlNode node,
		object value)
		{
			// Try any deferred actions.
			
			yield return Do_Deferment();
			
			
			// Setup payload for this object.

			Payload resultPayload = new()
			{
				// Use current value as default.
				// Useful with arrays and readonly structures.
				@object = value
			};
			payloads.Push(resultPayload);


			// Null

			if (node.Attributes.Bool("null"))
			{
				resultPayload.@object = null;
				yield break;
			}


			// Reference

			if (node.Attributes.TryInt("ref-id", out int refid))
			{
				if (reference.ContainsKey(refid))
				{
					// Already deserialized.
					resultPayload.@object =
						reference[refid];
					yield break;
				}

				// Referenced object is yet to be deserialized.
				// Defer it.

				bool Condition() =>
					reference.ContainsKey(refid);

				IEnumerator Action()
				{
					resultPayload.@object = reference[refid];
					resultPayload.boolean = false;

					yield break;
				}

				yield return Defer(Condition, Action());

				// Mark as deferred.
				resultPayload.boolean = true;
				yield break;
			}


			// Get Data Type

			Type type = Get_Type(node);

			if (type == null)
				// Unknown data type.
				// Return as is.
				yield break;
				
				
			// Resolver
			
			if (ObjectResolver(type, node, out object resolved_value))
			{
				resultPayload.@object = resolved_value;
				yield break;
			}


			// Unity Events

			yield return Do_UnityEventBase(
				node,
				type,
				resultPayload
			);

			Payload eventPayload = payloads.Pop();

			if (eventPayload.boolean)
				// Deserialized a UnityEventBase.
				// Return as is.
				yield break;
				
				
			// Type Objects

			if (Tools.TYPE.IsAssignableFrom(type))
			{
				resultPayload.@object =
					TypeHelper.Generic.FromFullName(
						node.StringOf("Assembly"),
						node.StringOf("Type")
					);
				yield break;
			}
			
			
			// GameObjects

			yield return Do_GameObject(
				node,
				type,
				resultPayload
			);

			Payload gameObjectPayload = payloads.Pop();

			if (gameObjectPayload.boolean)
				yield break;

			if (Tools.COMPONENT.IsAssignableFrom(type))
			{
				// A component is being deserialized
				// but is not in a GameObject.
				// Defer it.
				components[node.Attributes.Int("id")] = node;
				yield break;
			}


			// Basic Types

			if (Get_BasicType(node, type, out object value0))
			{
				resultPayload.@object = value0;
				yield break;
			}


			// Other Types

			if (resultPayload.@object != null)
				reference[node.Attributes.Int("id")] =
					resultPayload.@object;

			yield return Do_Dictionary(node, type, resultPayload);
			yield return Do_Enumerable(node, type, resultPayload);

			if (resultPayload.@object == null)
			{
				reference[node.Attributes.Int("id")] =
					resultPayload.@object =
					Activator.CreateInstance(type);
			}


			// Parameters

			yield return Do_Parameters(
				node,
				type,
				resultPayload.@object
			);


			// Processing

			if (resultPayload.@object is IAfterFullDeserialize value1)
				afterFullDeserialize.Add(value1);
		}

		IEnumerator Do_Parameters
		(XmlNode node,
		Type type,
		object value)
		{
			if (value == null)
				yield break;
				
			if (BDGOR_Default(node, value))
				yield break;
				
			if (BeforeDeserializeGameObjectResolver(node, value))
				yield break;
			
			yield return Do_Members(node, type, value);
		}
		
		IEnumerator Do_Members_Internal
		(Action<object> setter,
		Payload payload)
		{
			OnAfterDeserialize?.Invoke(payload.@object);
			setter(payload.@object);
			yield break;
		}
		
		IEnumerator Do_Members
		(XmlNode node,
		Type type,
		object value)
		{
			IEnumerable<XmlNode> members =
				node
				.Element("Members")
				.Elements();

			foreach (XmlNode item in members)
			{
				yield return null;

				Action<object> setter =
					Get_Members_Field(
						item,
						type,
						value,
						out object rawValue
					) ??
					Get_Members_Property(
						item,
						type,
						value,
						out rawValue
					);

				if (setter == null)
				{
					Debug.LogWarning(
						$"`{type.FullName}` has no member `{item.Name}`!"
					);
					continue;
				}

				yield return Do_Internal(
					item,
					rawValue
				);

				Payload memberPayload = payloads.Pop();

				if (memberPayload.boolean)
				{
					// Deferred.

					bool Condition() => !memberPayload.boolean;

					yield return Defer(
						Condition,
						Do_Members_Internal(
							setter,
							memberPayload
						)
					);
					
					continue;
				}

				yield return
					Do_Members_Internal(
						setter,
						memberPayload
					);
			}
		}

		IEnumerator Do_Dictionary
		(XmlNode node,
		Type type,
		Payload payload)
		{
			if (!node.TryElement("Dictionary", out XmlNode dictionaryNode))
				yield break;

			if (payload.@object == null)
				reference[node.Attributes.Int("id")] =
					payload.@object =
					Activator.CreateInstance(type);

			if (payload.@object is not IDictionary dictionary)
				yield break;

			foreach (XmlNode child in dictionaryNode.Elements("Item"))
			{
				XmlNode keyElement = child.Element("Key");
				XmlNode valueElement = child.Element("Value");

				yield return Do_Internal(
					keyElement,
					null
				);

				Payload keyPayload = payloads.Pop();

				yield return Do_Internal(
					valueElement,
					null
				);

				Payload valuePayload = payloads.Pop();

				bool Condition() =>
					!keyPayload.boolean &&
					!valuePayload.boolean;

				IEnumerator Action()
				{
					OnAfterDeserialize?.Invoke(keyPayload.@object);
					OnAfterDeserialize?.Invoke(valuePayload.@object);
					dictionary[keyPayload.@object] = valuePayload.@object;

					yield break;
				}

				yield return Defer(Condition, Action());
			}
		}

		IEnumerator Do_Enumerable_Array
			(XmlNode node,
			Type type,
			Payload payload)
		{
			Payload resultPayload = new();
			payloads.Push(resultPayload);

			if (payload.@object is not Array array)
				yield break;

			if (array.IsFixedSize &&
				array.Length != node.ChildNodes.Count)
			{
				// Size of the array is insufficient.
				// Forcibly resize it.
				payload.@object =
					Activator.CreateInstance(
						type,
						node.ChildNodes.Count
					);
				array = payload.@object as Array;
			}

			reference[node.Attributes.Int("id")] =
				payload.@object;

			int index = 0;

			foreach (XmlNode child in node.Elements("Item"))
			{
				int childIndex = index;

				yield return Do_Internal(
					child,
					null
				);

				Payload childPayload = payloads.Pop();

				bool Condition() =>
					!childPayload.boolean;

				IEnumerator Action()
				{
					OnAfterDeserialize?.Invoke(childPayload.@object);
					array.SetValue(childPayload.@object, childIndex);

					return null;
				}

				deferred.Add(new()
				{
					Condition = Condition,
					Action = Action()
				});

				index++;
			}

			resultPayload.boolean = true;
		}

		IEnumerator Do_Enumerable
			(XmlNode node,
			Type type,
			Payload payload)
		{
			if (!node.TryElement("Enumerable", out XmlNode enumerableNode))
				yield break;

			payload.@object ??=
				Activator.CreateInstance(
					type,
					enumerableNode.ChildNodes.Count
				);

			if (payload.@object == null)
				yield break;

			yield return Do_Enumerable_Array(
				enumerableNode,
				type,
				payload
			);

			Payload arrayPayload = payloads.Pop();

			if (arrayPayload.boolean)
				yield break;

			yield return Do_Enumerable_Method(
				enumerableNode,
				type,
				payload.@object
			);

			Payload methodPayload = payloads.Pop();

			if (methodPayload.boolean)
				yield break;
		}

		IEnumerator Do_Enumerable_Method
			(XmlNode node,
			Type type,
			object value)
		{
			Payload resultPayload = new();
			payloads.Push(resultPayload);

			MethodInfo method =
				type.GetMethod("Add") ??
				type.GetMethod("Enqueue") ??
				type.GetMethod("Push");

			reference[node.Attributes.Int("id")] = value;

			if (method == null)
				yield break;

			foreach (XmlNode child in node.Elements("Item"))
			{
				yield return Do_Internal(
					child,
					null
				);

				Payload childPayload =
					payloads.Pop();

				bool Condition() =>
					!childPayload.boolean;

				IEnumerator Action()
				{
					OnAfterDeserialize?.Invoke(childPayload.@object);
					method.Invoke(
						value,
						new object[] { childPayload.@object }
					);

					yield break;
				}

				yield return Defer(Condition, Action());
			}

			resultPayload.boolean = true;
		}
	}
}
