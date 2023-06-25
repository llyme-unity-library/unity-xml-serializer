using System.IO;
using System.Xml;
using UnityEngine;

namespace UnityXmlSerializer
{
	public partial class Deserialize<T>
	{
		public Deserialize<T> Set
		(XmlNode node,
		Transform parent = null)
		{
			Node = node;
			Parent = parent;
			
			return this;
		}
		
		public Deserialize<T> Set
		(XmlDocument document,
		Transform parent = null)
		{
			return Set(document.DocumentElement, parent);
		}
		
		public Deserialize<T> Set(string filepath, Transform parent = null)
		{
			using FileStream fs = new(
				filepath,
				FileMode.Open,
				FileAccess.Read
			);
			using BufferedStream bs = new(fs);
			using XmlReader xr = XmlReader.Create(bs);

			XmlDocument doc = new();
			doc.Load(xr);

			return Set(doc, parent);
		}
	}
}
