using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zoka.X2O
{
	/// <summary>
	/// The X2O reader is the basic class, and main public interface to be used to deserialize the objects of any type from XML
	/// using the simple conventions, which can be configured.
	/// </summary>
	public static class X2OReader
	{
		/// <summary>Will read the XML file and returns instance of the read object completely initialized with values from XML file</summary>
		/// <param name="_filename">The filename of the XML file to read into the object</param>
		/// <returns>The instance of the object according to the type read from XML file</returns>
		public static object								ReadFromFile(string _filename)
		{
			var xml_doc = new XmlDocument();
			xml_doc.Load(_filename);
			var root_node = xml_doc.ChildNodes.OfType<XmlNode>().Single(n => n.NodeType == XmlNodeType.Element);
			object result = null;
			ProcessElement(root_node, ref result);

			return result;
		}

		private static void									ProcessNodes(XmlNodeList _node_list, ref object _target_object)
		{
			foreach(XmlNode node in _node_list)
			{
				if (node.NodeType == XmlNodeType.Element)
					ProcessElement(node, ref _target_object);
			}
		}

		private static void									ProcessElement(XmlNode _node, ref object _target_object)
		{
			if (_target_object == null)
			{
				_target_object = CreateObjectByNode(_node);
				ProcessNodes(_node.ChildNodes, ref _target_object);
			}
			else
			{
				if (_node.HasChildNodes && _node.ChildNodes.Count == 1 && _node.ChildNodes[0].NodeType == XmlNodeType.Text)
				{
					ProcessValue(_node, ref _target_object);
				}
			}

		}

		private static void									ProcessValue(XmlNode _node, ref object _target_object)
		{
			var members = _target_object.GetType().GetMembers();
			var desired_members = members.Where(m => m.Name == _node.Name);

			if (!desired_members.Any())
				return;

			var xval = _node.ChildNodes[0].Value;

			foreach(var mi in desired_members)
			{
				if (mi.MemberType == MemberTypes.Field)
				{
					var fi = mi as FieldInfo;
					var tgt_type_val = Convert.ChangeType(xval, fi.FieldType);
					fi.SetValue(_target_object, tgt_type_val);
				}
				if (mi.MemberType == MemberTypes.Property)
				{
					var pi = mi as PropertyInfo;
					var tgt_type_val = Convert.ChangeType(xval, pi.PropertyType);
					pi.SetValue(_target_object, tgt_type_val);
				}
			}
		}


		private static object								CreateObjectByNode(XmlNode _node)
		{
			var type_attr = _node.Attributes["type"];
			IEnumerable<Type> tgt_types = null;

			if (type_attr != null)
			{
				tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
							from t in a.GetTypes()
							where t.FullName == type_attr.Value || t.Name == type_attr.Value || t.AssemblyQualifiedName == type_attr.Value
							select t;
			}

			if (tgt_types == null || !tgt_types.Any())
			{
				tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
							from t in a.GetTypes()
							where t.FullName == _node.Name || t.Name == _node.Name
							select t;
			}

			if (tgt_types == null || !tgt_types.Any())
				return null;

			var inst = Activator.CreateInstance(tgt_types.First());
			return inst;
		}

	}
}
