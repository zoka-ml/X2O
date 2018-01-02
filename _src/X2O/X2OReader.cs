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

		/// <summary>Processes all child nodes</summary>
		private static void									ProcessNodes(XmlNodeList _node_list, ref object _target_object)
		{
			foreach(XmlNode node in _node_list)
			{
				if (node.NodeType == XmlNodeType.Element)
					ProcessElement(node, ref _target_object);
			}
		}

		/// <summary>Processes the single element</summary>
		private static void									ProcessElement(XmlNode _node, ref object _parent_of_object)
		{
			// find the target into which it will be deserialized
			if (_parent_of_object == null)
			{
				// in this case, we must create it first

				IEnumerable<Type> tgt_types = null;
				var type_attr = _node.Attributes["type"];
				if (type_attr != null)
				{
					// first try by type attribute
					tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
								from t in a.GetTypes()
								where t.FullName == type_attr.Value || t.Name == type_attr.Value || t.AssemblyQualifiedName == type_attr.Value
								select t;


				}
				if (tgt_types == null || !tgt_types.Any())
				{
					// i not found before, try by the node name
					tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
								from t in a.GetTypes()
								where t.FullName == _node.Name || t.Name == _node.Name
								select t;
				}

				if (tgt_types == null || !tgt_types.Any())
					throw new Exception("Expected the type, either by the \"type\" attribute, or by node name.");

				_parent_of_object = Activator.CreateInstance(tgt_types.First());

				ProcessNodes(_node.ChildNodes, ref _parent_of_object);
			}
			else
			{
				// we have parent, so we will deserialize into some member

				// first find such member by the name
				var member_info = _parent_of_object.GetType().GetMembers().Single(m => m.Name.CompareTo(_node.Name) == 0);
				if (member_info == null)
					throw new Exception($"Member of name \"{_node.Name}\" was expected in the object of type \"{_parent_of_object.GetType().FullName}\". Cannot continue in deserialization.");

				Type member_type = GetTypeOfMember(member_info);

				// now find out, whether it is value or complex type
				if (member_type.IsValueType || member_type.Equals(typeof(string)))
				{
					var xval = _node.ChildNodes[0].Value;
					var tgt_type_val = Convert.ChangeType(xval, member_type);
					SetMemberValue(member_info, _parent_of_object, tgt_type_val);
				}
				else
				{
					// in this case, the value is complex, so we need to create new object

					// first we must induce the object type
					IEnumerable<Type> tgt_types = null;
					var type_attr = _node.Attributes["type"];
					if (type_attr != null)
					{
						// first try by type attribute
						tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
									from t in a.GetTypes()
									where t.FullName == type_attr.Value || t.Name == type_attr.Value || t.AssemblyQualifiedName == type_attr.Value
									select t;


					}
					if (tgt_types == null || !tgt_types.Any())
					{
						// if not found before, try by the member type
						tgt_types = new List<Type>() { member_type };
					}

					if (tgt_types == null || !tgt_types.Any())
						throw new Exception("Expected the type specification, using the \"type\" attribute.");

					var new_obj = Activator.CreateInstance(tgt_types.First());
					SetMemberValue(member_info, _parent_of_object, new_obj);

					ProcessNodes(_node.ChildNodes, ref new_obj);
				}

			}
		}

		/// <summary>Will return the type of the member, in cases it is Field or Property</summary>
		/// <exception cref="NotSupportedException">In case the member is not Field nor Property</exception>
		private static Type									GetTypeOfMember(MemberInfo _member_info)
		{
			if (_member_info.MemberType == MemberTypes.Field)
				return (_member_info as FieldInfo).FieldType;
			if (_member_info.MemberType == MemberTypes.Property)
				return (_member_info as PropertyInfo).PropertyType;

			throw new NotSupportedException("Getting type of member which is not Field nor Property is not allowed");
		}

		/// <summary>Will set the value of the member in case it is Field or Property</summary>
		/// <exception cref="NotSupportedException">In case the member is not Field nor Property</exception>
		private static void									SetMemberValue(MemberInfo _member_info, object _parent_object, object _value)
		{
			if (_member_info.MemberType == MemberTypes.Field)
				(_member_info as FieldInfo).SetValue(_parent_object, _value);
			else if (_member_info.MemberType == MemberTypes.Property)
				(_member_info as PropertyInfo).SetValue(_parent_object, _value);
			else throw new NotSupportedException("Setting value of member which is not Field nor Property is not allowed");
		}
	}
}
