using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zoka.X2O.X2OElementsProcessors;

namespace Zoka.X2O
{
	/// <summary>
	/// The default implementation of X2O processor which is used to walkthrough the XML structure and tries to
	/// process all nodes using the elements processors.
	/// </summary>
	public class X2OProcessor
	{
		#region Construction

		/// <summary>Empty contructor which adds no default elements processor</summary>
		public X2OProcessor()
		{
			m_ElementsProcessors = new List<IX2OElementsProcessor>();
		}

		/// <summary>
		/// Constructor taking the list of element processors to be used to process XML elements during 
		/// reading the XML file.
		/// </summary>
		public X2OProcessor(params IX2OElementsProcessor[] _elements_processors)
		{
			m_ElementsProcessors = new List<IX2OElementsProcessor>(_elements_processors);
		}

		#endregion // Construction

		#region Elements processors

		/// <summary>List of the element processors, protected, so it is accessible to possible overriders.</summary>
		protected List<IX2OElementsProcessor>				m_ElementsProcessors;

		/// <summary>Publicly visible list of elements processors</summary>
		public virtual IEnumerable<IX2OElementsProcessor>	ElementsProcessors => m_ElementsProcessors;

		/// <summary>Will add the elements processor into the list, so in the next opportunity, this processor may be used.</summary>
		public virtual void									AddElementsProcessors(IX2OElementsProcessor _elements_processor)
		{
			m_ElementsProcessors.Add(_elements_processor);
		}

		#endregion // Elements processors

		/// <summary>
		/// Will process elements under the _parent element by finding the best suitable 
		/// elements processor (the one which returns non-null value)
		/// </summary>
		/// <returns>
		/// The object which has evolved as the result of elements processor run, 
		/// or null, if the current element with its child nodes could not be read by any elements processor.
		/// </returns>
		public virtual object								ProcessElements(XmlElement _parent_element, Type _declared_type, X2OConfig _config)
		{
			foreach(var processor in ElementsProcessors.Reverse())
			{
				var result = processor.ProcessElements(_parent_element, _declared_type, _config);
				if (result != null)
					return result;
			}

			return null;
		}

		/// <summary>
		/// Will analyse the passed element (which is root) and returns the type which can be used as _declared_type
		/// when calling the ProcessElements function for the root node of Xml.
		/// </summary>
		public virtual Type									GetTypeOfRoot(XmlElement _element)
		{
			var type_attr = _element.Attributes["type"];
			Type tgt_type = null;
			if (type_attr != null)
			{
				tgt_type = GetTypeByName(type_attr.Value);
			}

			if (tgt_type == null)
			{
				tgt_type = GetTypeByName(_element.Name);
			}

			return tgt_type;
		}

		/// <summary>
		/// This function based on the name of type returns the type.
		/// This helps to consider the type even with type shortcuts (e.g. not using namespaces and so on).
		/// It goes through the full list of types in all loaded assemblies and tries to find the best match - only single is allowed.
		/// </summary>
		/// <exception cref="InvalidOperationException">In case, there is more than one suitable type.</exception>
		public virtual Type									GetTypeByName(string _type_name)
		{
			// first try by type attribute
			var tgt_types = from a in AppDomain.CurrentDomain.GetAssemblies()
							from t in a.GetTypes()
							where t.FullName == _type_name || t.Name == _type_name || t.AssemblyQualifiedName == _type_name
							select t;

			if (tgt_types == null || !tgt_types.Any())
				return null;
			if (tgt_types.Count() > 1)
				throw new  InvalidOperationException("Multiple types found");
			return tgt_types.First();
		}

	}
}
