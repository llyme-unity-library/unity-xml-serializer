using UnityEngine;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		private bool Defer_Object_Component
			(object @object,
			bool inGameObject)
		{
			if (inGameObject)
				return false;

			// Components should only be serialized
			// within a GameObject.

			bool flag = @object is Component;

			if (flag)
			{
				// Defer the component until it is
				// being serialized from a GameObject.

				if (!deferment.TryGetValue(@object, out int index))
				{
					deferment[@object] = index = reference.Count;
					// Mark as `null` to make space.
					// This is where the component will be placed
					// when it is about to be serialized.
					reference.Add(null);
				}

				Writer.WriteAttributeString(
					"ref-id",
					index.ToString()
				);
			}

			return flag;
		}
	}
}
