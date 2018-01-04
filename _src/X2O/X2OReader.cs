using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zoka.X2O.X2OElementsProcessors;

namespace Zoka.X2O
{
	/// <summary></summary>
	public static class X2OReader
	{
		/// <summary>Will read the XML file and returns instance of the read object completely initialized with values from XML file</summary>
		/// <param name="_filename">The filename of the XML file to read into the object</param>
		/// <returns>The instance of the object according to the type read from XML file</returns>
		public static object								ReadFromFile(string _filename)
		{
			var xml_doc = new XmlDocument();
			xml_doc.Load(_filename);
			var root_node = xml_doc.ChildNodes.OfType<XmlNode>().Single(n => n.NodeType == XmlNodeType.Element) as XmlElement;
			var cfg = new X2OConfig();
			var root_type = cfg.Processor.GetTypeOfRoot(root_node);
			var result = cfg.Processor.ProcessElements(root_node, root_type, cfg);

			return result; 
		}
	}
}
