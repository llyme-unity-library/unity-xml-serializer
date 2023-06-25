using System.IO;
using System.Text;
using System.Xml;

namespace UnityXmlSerializer
{
	public partial class Serialize<T>
	{
		public Serialize<T> Set(object @object, XmlWriter writer)
		{
			Writer = writer;
			Object = @object;
			ObjectType = @object.GetType();
			RootNodeName = "Root";
			
			return this;
		}
		
		public Serialize<T> Set(object @object, bool pretty, Encoding encoding)
		{
			XmlWriterSettings settings =
				pretty
				? new()
				{
					Indent = true,
					IndentChars = "\t",
					Encoding = encoding
				}
				: new()
				{
					Encoding = encoding
				};

			using MemoryStream ms = new();
			using XmlWriter writer =
				XmlWriter.Create(ms, settings);

			Set(@object, writer);
			
			return this;
		}
	}
}
