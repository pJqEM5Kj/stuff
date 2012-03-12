using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace ConsoleApplication
{
	class Program
	{
		public static void Main(string[] args)
		{
			//
			XmlDocument m = new XmlDocument();
			m.Load("D:\\1.xtpl");

			process(m, m);

			m.Save("D:\\777.xtpl");
		}

		public static void process(XmlNode node, XmlDocument originalDocument)
		{
			if (node == null)
			{
				throw new ArgumentNullException();
			}

			switch (node.NodeType)
			{
				case XmlNodeType.Attribute:
					if (node.Name == "trim-from")
					{
						XmlAttribute xmlAttribute = (XmlAttribute)node;

						XmlElement newXmlElement = originalDocument.CreateElement("trim-from");
						newXmlElement.AppendChild(originalDocument.CreateTextNode(node.Value));

						if (xmlAttribute.OwnerElement.HasChildNodes)
						{
							xmlAttribute.OwnerElement.InsertBefore(
								newXmlElement, xmlAttribute.OwnerElement.FirstChild);
						}
						else
						{
							xmlAttribute.OwnerElement.AppendChild(newXmlElement);
						}
					}

					break;
			}

			if (node.Attributes != null)
			{
				XmlAttribute attribToDelete = null;
				foreach (XmlAttribute attrib in node.Attributes)
				{
					if (attrib.Name == "trim-from")
					{
						attribToDelete = attrib;
					}
					
					process(attrib, originalDocument);
				}

				if (attribToDelete != null)
				{
					attribToDelete.OwnerElement.RemoveAttributeNode(attribToDelete);
				}
			}

			if (node.HasChildNodes)
			{
				foreach (XmlNode childNode in node.ChildNodes)
				{
					process(childNode, originalDocument);
				}
			}
		}
	}
}