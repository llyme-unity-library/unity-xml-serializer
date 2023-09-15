using System;
using System.Collections;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		readonly struct Deferment
		{
			public Func<bool> Condition { get; init; }
			
			public IEnumerator Action { get; init; }
		}
		
		IEnumerator Defer
		(Func<bool> condition,
		IEnumerator action)
		{
			if (condition())
			{
				// The condition is already met.
				// No need to defer.
				yield return action;
				yield break;
			}

			deferred.Add(new()
			{
				Condition = condition,
				Action = action
			});
		}
		
		IEnumerator Do_Deferment()
		{
			int index = 0;
			
			while (index < deferred.Count)
			{
				yield return null;

				Deferment deferment = deferred[index];

				if (deferment.Condition())
				{
					yield return deferment.Action;

					deferred.RemoveAt(index);

					continue;
				}

				index++;
			}
		}
	}
}
