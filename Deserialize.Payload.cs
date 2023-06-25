using System.Collections;
using System.Collections.Generic;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		private class Payload
		{
			public readonly Queue<IEnumerator> queue = new();

			public object @object = null;
			public bool boolean = false;
		}
	}
}
