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
			var root_node = GetXmlRootElementFromFile(_filename);
			var cfg = new X2OConfig();
			var root_type = cfg.Processor.GetTypeOfRoot(root_node);
			var result = ReadFromXmlElement(root_node, root_type, cfg);

			return result; 
		}

		/// <summary>
		/// Will read the XML file and returns instance of the read object completely initialized with values from XML file.
		/// The Type generic argument is used as the declared type of the root node.
		/// </summary>
		/// <param name="_filename">The filename of the XML file to read into the object</param>
		/// <returns>The instance of the object according to the type read from XML file</returns>
		public static T										ReadFromFile<T>(string _filename)
		{
			var root_node = GetXmlRootElementFromFile(_filename);
			var cfg = new X2OConfig();
			var root_type = typeof(T);
			var result = ReadFromXmlElement(root_node, root_type, cfg);

			return (T)result;
		}


		/// <summary>
		/// Will read the XML file and returns instance of the read object completely initialized with values from XML file.
		/// </summary>
		/// <param name="_element">Usually the root node of the XML document to be deserialized and returned as object instance.</param>
		/// <param name="_declared_type">The type which is the suppossed the be the result (or convertible into this type)</param>
		/// <param name="_config">Configurtion object of X2OReader</param>
		/// <returns>The instance of the object according to the type read from XML file</returns>
		public static object								ReadFromXmlElement(XmlElement _element, Type _declared_type, X2OConfig _config)
		{
			var result = _config.Processor.ProcessElements(_element, _declared_type, _config);
			return result;
		}

		private static XmlElement							GetXmlRootElementFromFile(string _filename)
		{
			var xml_doc = new XmlDocument();
			xml_doc.Load(_filename);
			var root_node = xml_doc.ChildNodes.OfType<XmlNode>().Single(n => n.NodeType == XmlNodeType.Element) as XmlElement;
			return root_node;
		}
	}
}
