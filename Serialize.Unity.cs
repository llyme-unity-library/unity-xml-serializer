using System.Collections;
using TypeHelper;
using UnityEngine;
using UnityEngine.Events;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		private void Do_UnityEventBase_Call(object call)
		{
			Writer.WriteStartElement("Listener");
			{
				Object target =
					call.ValueOfField("m_Target") as Object;
				string methodName =
					call.ValueOfField("m_MethodName") as string;
				PersistentListenerMode mode =
					(PersistentListenerMode)call.ValueOfField("m_Mode");
				// int callState =
				// 	(int)call.ValueOfField("m_CallState");
				object args =
					call.ValueOfField("m_Arguments");

				// Writer.WriteElementString(
				// 	"State",
				// 	callState.ToString()
				// );
				Writer.WriteElementString(
					"MethodName",
					methodName
				);
				Writer.WriteElementString(
					"Mode",
					((int)mode).ToString()
				);

				switch (mode)
				{
					case PersistentListenerMode.EventDefined:
						// Dynamic.
						// The event itself will pass the argument.
					case PersistentListenerMode.Void:
						break;
					case PersistentListenerMode.Object:
						object objectArgument =
							args.ValueOfField("m_ObjectArgument");
						Do_Internal(
							objectArgument.GetType(),
							objectArgument,
							false,
							"Argument"
						);
						break;
					case PersistentListenerMode.Int:
						Writer.WriteElementString(
							"Argument",
							args.ValueOfField("m_IntArgument")
							.ToString()
						);
						break;
					case PersistentListenerMode.Float:
						Writer.WriteElementString(
							"Argument",
							args.ValueOfField("m_FloatArgument")
							.ToString()
						);
						break;
					case PersistentListenerMode.String:
						Writer.WriteElementString(
							"Argument",
							args.ValueOfField("m_StringArgument")
							.ToString()
						);
						break;
					case PersistentListenerMode.Bool:
						Writer.WriteElementString(
							"Argument",
							args.ValueOfField("m_BoolArgument")
							.ToString()
						);
						break;
				}

				Do_Internal(
					target.GetType(),
					target,
					false,
					"Target"
				);
			}
			Writer.WriteEndElement();
		}

		private void Do_UnityEventBase(UnityEventBase @event)
		{
			Writer.WriteStartElement("Listeners");
			{
				IEnumerable calls =
					@event
					.ValueOfField<UnityEventBase>("m_PersistentCalls")
					.ValueOfField("m_Calls") as IEnumerable;
				
				foreach (object call in calls)
					Do_UnityEventBase_Call(call);
			}
			Writer.WriteEndElement();
		}

		private void Do_GameObject(GameObject @object)
		{
			Writer.WriteStartElement("Components");
			{
				Component[] components =
					@object.GetComponents<Component>();

				foreach (Component component in components)
					Do_Component(component);
			}
			Writer.WriteEndElement();

			Writer.WriteStartElement("Children");
			{
				foreach (Transform transform in @object.transform)
				{
					Writer.WriteStartElement("Child");
					{
						Do_Object(
							typeof(GameObject),
							transform.gameObject,
							true
						);
					}
					Writer.WriteEndElement();
				}
			}
			Writer.WriteEndElement();
		}

		private void Do_Component
			(Component component)
		{
			Writer.WriteStartElement("Component");
			{
				Do_Object(
					component.GetType(),
					component,
					true
				);
			}
			Writer.WriteEndElement();
		}
	}
}
