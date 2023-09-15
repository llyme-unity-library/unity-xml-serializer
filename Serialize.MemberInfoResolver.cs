using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		public static readonly string[] GRID = new string[]
		{
			"cellSize",
			"cellGap",
			"cellLayout",
			"cellSwizzle"
		};
		
		public static readonly string[] GAME_OBJECT = new string[]
		{
			"name",
			"tag",
			"layer",
			"isStatic",
			"activeSelf"
		};
		
		public static readonly string[] BEHAVIOUR = new string[]
		{
			"enabled"
		};

		public static readonly string[] RENDERER = new string[]
		{
			"enabled"
		};

		public static readonly string[] TRANSFORM = new string[]
		{
			"localPosition",
			"localRotation",
			"localScale"
		};

		public static readonly string[] COLLIDER_2D = new string[]
		{
			"sharedMaterial",
			"isTrigger",
			"usedByEffector",
			"usedByComposite",
			"offset",
			"layerOverridePriority",
			"includeLayers",
			"excludeLayers",
			"forceSendLayers",
			"forceReceiveLayers",
			"contactCaptureLayers",
			"callbackLayers"
		};

		public static readonly string[] BOX_COLLIDER_2D = new string[]
		{
			"autoTiling",
			"edgeRadius",
			"size"
		};

		public static readonly string[] CIRCLE_COLLIDER_2D = new string[]
		{
			"radius"
		};
		
		public static readonly string[] EDGE_COLLIDER_2D = new string[]
		{
			"edgeRadius",
			"useAdjacentStartPoint",
			"adjacentStartPoint",
			"useAdjacentEndPoint",
			"adjacentEndPoint"
		};

		public static readonly string[] EDGE_COLLIDER_2D_EXCLUDE = new string[]
		{
			"usedByComposite"
		};

		public static readonly string[] CIRCLE_COLLIDER_2D_EXCLUDE = new string[]
		{
			"usedByComposite"
		};

		public static readonly string[] COMPOSITE_COLLIDER_2D = new string[]
		{
			"geometryType",
			"generationType",
			"vertexDistance",
			"offsetDistance",
			"edgeRadius"
		};

		public static readonly string[] COMPOSITE_COLLIDER_2D_EXCLUDE = new string[]
		{
			"usedByComposite"
		};

		public static readonly string[] RIGIDBODY_2D_DYNAMIC = new string[]
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

		public static readonly string[] RIGIDBODY_2D_KINEMATIC = new string[]
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

		public static readonly string[] RIGIDBODY_2D_STATIC = new string[]
		{
			"bodyType",
			"sharedMaterial",
			"simulated"
		};

		public static readonly string[] SPRITE_RENDERER = new string[]
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

		public static readonly string[] LIGHT_2D_FREEFORM = new string[]
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

		public static readonly string[] LIGHT_2D_SPRITE = new string[]
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

		public static readonly string[] LIGHT_2D_POINT = new string[]
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

		public static readonly string[] LIGHT_2D_GLOBAL = new string[]
		{
			"lightType",
			"color",
			"intensity",
			"m_ApplyToSortingLayers",
			"blendStyleIndex",
			"lightOrder",
			"m_OverlapOperation"
		};

		public static readonly string[] SHADOW_CASTER_2D = new string[]
		{
			"m_HasRenderer",
			"m_ApplyToSortingLayers",
			"m_ShapePath",
			"m_ShadowGroup",
			"useRendererSilhouette",
			"selfShadows",
			"castsShadows",
		};

		public static readonly string[] AUDIO_REVERB_ZONE_PRESET = new string[]
		{
			"minDistance",
			"maxDistance",
			"reverbPreset",
		};

		public static readonly string[] AUDIO_REVERB_ZONE_USER = new string[]
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

		public static readonly string[] PLATFORM_EFFECTOR_2D = new string[]
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

		public static readonly string[] SURFACE_EFFECTOR_2D = new string[]
		{
			"useColliderMask",
			"colliderMask",
			"speed",
			"speedVariation",
			"forceScale",
			"useContactForce",
			"useFriction",
			"useBounce"
		};
		
		public static readonly string[] AREA_EFFECTOR_2D = new string[]
		{
			"useColliderMask",
			"colliderMask",
			"useGlobalAngle",
			"forceAngle",
			"forceMagnitude",
			"forceVariation",
			"forceTarget",
			"drag",
			"angularDrag"
		};
		
		public static readonly string[] SORTING_GROUP = new string[]
		{
			"sortingLayerID",
			"sortingOrder",
			"sortAtRoot"
		};
		
		static bool MIR_Default
		(object @object,
		string key,
		ref bool shouldWrite)
		{
			if (!shouldWrite)
				return false;
				
			bool @override = false;
			shouldWrite = false;

			if (@object is GameObject)
			{
				shouldWrite =
					GAME_OBJECT.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				return true;
			}

			if (@object is Behaviour)
			{
				shouldWrite |=
					BEHAVIOUR.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = shouldWrite;
			}
			
			if (@object is Renderer)
			{
				shouldWrite |=
					RENDERER.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = shouldWrite;
			}

			if (@object is Transform)
			{
				shouldWrite |=
					TRANSFORM.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
			}

			if (@object is Collider2D)
			{
				shouldWrite |=
					COLLIDER_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
			}

			if (@object is BoxCollider2D)
			{
				shouldWrite |=
					BOX_COLLIDER_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
			}

			if (@object is CircleCollider2D)
			{
				shouldWrite |=
					CIRCLE_COLLIDER_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				shouldWrite &=
					!CIRCLE_COLLIDER_2D_EXCLUDE.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
			}
			
			if (@object is CompositeCollider2D)
			{
				shouldWrite |=
					COMPOSITE_COLLIDER_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				shouldWrite &=
					!COMPOSITE_COLLIDER_2D_EXCLUDE.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
			}
			
			if (@object is EdgeCollider2D)
			{
				shouldWrite |=
					EDGE_COLLIDER_2D.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				shouldWrite &=
					!EDGE_COLLIDER_2D_EXCLUDE.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);
				@override = true;
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
					shouldWrite |= array.Contains(
						key,
						StringComparer.OrdinalIgnoreCase
					);

				return true;
			}

			if (@object is SpriteRenderer)
			{
				shouldWrite |=
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

				shouldWrite |= array.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);

				return true;
			}

			if (@object is PlatformEffector2D)
			{
				shouldWrite |= PLATFORM_EFFECTOR_2D.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);

				return true;
			}
			
			if (@object is SurfaceEffector2D)
			{
				shouldWrite |= SURFACE_EFFECTOR_2D.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);
				
				return true;
			}
			
			if (@object is AreaEffector2D)
			{
				shouldWrite |= AREA_EFFECTOR_2D.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);
				
				return true;
			}
			
			if (@object is SortingGroup)
			{
				shouldWrite |= SORTING_GROUP.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);
				
				return true;
			}
			
			if (@object is Grid)
			{
				shouldWrite |= GRID.Contains(
					key,
					StringComparer.OrdinalIgnoreCase
				);
				
				return true;
			}
			
			return @override;
		}
		
		protected virtual bool MemberInfoResolver
		(object @object,
		string key,
		ref bool shouldWrite) => false;
	}
}
