using System;
using System.Collections;
using System.Linq;
using TypeHelper;
using UnityEngine;
using UnityEngine.Events;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		private static partial class Filter
		{
			private static readonly string[] GAME_OBJECT = new string[]
			{
				"name",
				"tag",
				"layer",
				"isStatic",
				"activeSelf"
			};

			private static readonly string[] BEHAVIOUR = new string[]
			{
				"enabled"
			};

			private static readonly string[] RENDERER = new string[]
			{
				"enabled"
			};

			private static readonly string[] TRANSFORM = new string[]
			{
				"localPosition",
				"localRotation",
				"localScale"
			};

			private static readonly string[] COLLIDER_2D = new string[]
			{
				"isTrigger",
				"offset",
				"sharedMaterial",
				"usedByComposite",
				"usedByEffector"
			};

			private static readonly string[] BOX_COLLIDER_2D = new string[]
			{
				"autoTiling",
				"edgeRadius",
				"size"
			};

			private static readonly string[] CIRCLE_COLLIDER_2D = new string[]
			{
				"radius"
			};

			private static readonly string[] RIGIDBODY_2D_DYNAMIC = new string[]
			{
				"bodyType",
				"sharedMaterial",
				"simulated",
				"useAutoMass",
				"mass",
				"drag",
				"angularDrag",
				"gravityScale",
				"collisionDetectionMode",
				"sleepMode",
				"interpolation",
				"constraints"
			};

			private static readonly string[] RIGIDBODY_2D_KINEMATIC = new string[]
			{
				"bodyType",
				"sharedMaterial",
				"useFullKinematicContacts",
				"simulated",
				"collisionDetectionMode",
				"sleepMode",
				"interpolation",
				"constraints"
			};

			private static readonly string[] RIGIDBODY_2D_STATIC = new string[]
			{
				"bodyType",
				"sharedMaterial",
				"simulated"
			};

			private static readonly string[] SPRITE_RENDERER = new string[]
			{
				"sprite",
				"color",
				"flipX",
				"flipY",
				"drawMode",
				"size",
				"tileMode",
				"maskInteraction",
				"spriteSortPoint",
				"sharedMaterial",
				"sortingLayerID",
				"sortingOrder",
				"renderingLayerMask"
			};

			private static readonly string[] LIGHT_2D_FREEFORM = new string[]
			{
				"lightType",
				"color",
				"intensity",
				"m_FalloffIntensity",
				"m_ApplyToSortingLayers",
				"blendStyleIndex",
				"lightOrder",
				"m_OverlapOperation",
				"shadowsEnabled",
				"shadowIntensity",
				"volumeIntensityEnabled",
				"m_LightVolumeIntensity",
				"volumetricShadowsEnabled",
				"shadowVolumeIntensity",
				"m_NormalMapQuality",
				"m_NormalMapDistance"
			};

			private static readonly string[] LIGHT_2D_SPRITE = new string[]
			{
				"lightType",
				"color",
				"intensity",
				"m_LightCookieSprite",
				"m_ApplyToSortingLayers",
				"blendStyleIndex",
				"lightOrder",
				"m_OverlapOperation",
				"shadowsEnabled",
				"shadowIntensity",
				"volumeIntensityEnabled",
				"m_LightVolumeIntensity",
				"volumetricShadowsEnabled",
				"shadowVolumeIntensity",
				"m_NormalMapQuality",
				"m_NormalMapDistance"
			};

			private static readonly string[] LIGHT_2D_POINT = new string[]
			{
				"lightType",
				"color",
				"intensity",
				"pointLightInnerRadius",
				"pointLightOuterRadius",
				"pointLightInnerAngle",
				"pointLightOuterAngle",
				"m_FalloffIntensity",
				"m_ApplyToSortingLayers",
				"blendStyleIndex",
				"lightOrder",
				"m_OverlapOperation",
				"shadowsEnabled",
				"shadowIntensity",
				"volumeIntensityEnabled",
				"m_LightVolumeIntensity",
				"volumetricShadowsEnabled",
				"shadowVolumeIntensity",
				"m_NormalMapQuality",
				"m_NormalMapDistance"
			};

			private static readonly string[] LIGHT_2D_GLOBAL = new string[]
			{
				"lightType",
				"color",
				"intensity",
				"m_ApplyToSortingLayers",
				"blendStyleIndex",
				"lightOrder",
				"m_OverlapOperation"
			};

			private static readonly string[] SHADOW_CASTER_2D = new string[]
			{
				"m_HasRenderer",
				"m_ApplyToSortingLayers",
				"m_ShapePath",
				"m_ShadowGroup",
				"useRendererSilhouette",
				"selfShadows",
				"castsShadows",
			};

			private static readonly string[] AUDIO_REVERB_ZONE_PRESET = new string[]
			{
				"minDistance",
				"maxDistance",
				"reverbPreset",
			};

			private static readonly string[] AUDIO_REVERB_ZONE_USER = new string[]
			{
				"minDistance",
				"maxDistance",
				"reverbPreset",
				"room",
				"roomHF",
				"roomLF",
				"decayTime",
				"decayHFRatio",
				"reflections",
				"reflectionsDelay",
				"reverb",
				"reverbDelay",
				"HFReference",
				"LFReference",
				"diffusion",
				"density"
			};

			private static readonly string[] PLATFORM_EFFECTOR_2D = new string[]
			{
				"useColliderMask",
				"colliderMask",
				"rotationalOffset",
				"useOneWay",
				"useOneWayGrouping",
				"surfaceArc",
				"useSideFriction",
				"useSideBounce",
				"sideArc"
			};

			public static bool GameObjectFilter
				(object @object,
				string key,
				out bool flag)
			{
				bool result = false;
				flag = false;

				if (@object is GameObject)
				{
					flag =
						GAME_OBJECT.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					return true;
				}

				if (@object is Behaviour)
				{
					flag |=
						BEHAVIOUR.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = flag;
				}

				if (@object is Renderer)
				{
					flag |=
						RENDERER.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = flag;
				}

				if (@object is Transform)
				{
					flag |=
						TRANSFORM.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = true;
				}

				if (@object is Collider2D)
				{
					flag |=
						COLLIDER_2D.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = true;
				}

				if (@object is BoxCollider2D)
				{
					flag |=
						BOX_COLLIDER_2D.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = true;
				}

				if (@object is CircleCollider2D)
				{
					flag |=
						CIRCLE_COLLIDER_2D.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					result = true;
				}

				if (@object is Rigidbody2D rigidbody)
				{
					string[] array =
						rigidbody.bodyType switch
						{
							RigidbodyType2D.Dynamic =>
							RIGIDBODY_2D_DYNAMIC,

							RigidbodyType2D.Kinematic =>
							RIGIDBODY_2D_KINEMATIC,

							RigidbodyType2D.Static =>
							RIGIDBODY_2D_STATIC,

							_ => null
						};

					if (array != null)
						flag |= array.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);

					return true;
				}

				if (@object is SpriteRenderer)
				{
					flag |=
						SPRITE_RENDERER.Contains(
							key,
							StringComparer.OrdinalIgnoreCase
						);
					return true;
				}

				if (@object is AudioReverbZone zone)
				{
					string[] array =
						zone.reverbPreset switch
						{
							AudioReverbPreset.User =>
							AUDIO_REVERB_ZONE_USER,

							_ =>
							AUDIO_REVERB_ZONE_PRESET
						};

					flag |= array.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);

					return true;
				}

				if (@object is PlatformEffector2D)
				{
					flag |= PLATFORM_EFFECTOR_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);

					return true;
				}

				return result;
			}
		}

		private void Do_UnityEventBase_Call
			(object call)
		{
			Writer.WriteStartElement("Listener");
			{
				UnityEngine.Object target =
					call.ValueOfField("m_Target") as UnityEngine.Object;
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

		private void Do_UnityEventBase
			(UnityEventBase @event)
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

		private void Do_GameObject
			(GameObject @object)
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
