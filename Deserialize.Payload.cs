namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		class Payload
		{
			/// <summary>
			/// Object value.
			/// Usually for the object being deserialized.
			/// </summary>
			public object @object = null;
			/// <summary>
			/// Boolean value.
			/// Usually for deferment.
			/// </summary>
			public bool boolean = false;
		}
	}
}
