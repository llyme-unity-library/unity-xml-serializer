using System;
using System.Collections;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		private readonly struct Deferment
		{
			public Func<bool> Condition { get; init; }
			
			public Func<IEnumerator> Action { get; init; }
		}
		
		private IEnumerator Defer
			(Func<bool> condition,
			Func<IEnumerator> action)
		{
			if (condition())
			{
				yield return action();
				yield break;
			}

			deferred.Add(new()
			{
				Condition = condition,
				Action = action
			});
		}
		
		private IEnumerator Do_Deferment()
		{
			int index = 0;

			while (index < deferred.Count)
			{
				yield return null;

				Deferment deferment = deferred[index];

				if (deferment.Condition())
				{
					yield return deferment.Action();

					deferred.RemoveAt(index);

					continue;
				}

				index++;
			}
		}
	}
}
