using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zoka.X2O.X2OElementsProcessors
{
	/// <summary>
	/// Interface, which can be implemented by the user as custom processor, especially in cases like:
	///		- type cannot be created by the default processors (no parameterless constructor, ...)
	///		- target type cannot be determined by the default processors
	/// </summary>
	public interface IX2OElementsProcessor
	{
		/// <summary>
		/// When implemented in the descendant, it can process all child nodes of the _parent_element and returns the deserialized object.
		/// Each implementation of this interface is meant to be used as processor for single type, or similar types, so in the case, 
		/// the implementation doesn't know how to process the concrete type, it returns null. Then, the next processor in chain is used.
		/// Return type must be of type _target_type. Implementor may use the _parent_element and its attributes (and other information) to
		/// consider how to create the _target_type.
		/// Implementation should not throw any exception. Exception would disrupt the process of searchig the suitable processor.
		/// </summary>
		/// <param name="_parent_element">
		/// The Xml element, whose child nodes, should be used as values for the returned object.
		/// _parent_element itself may be used to help implementor to create the type (e.g. implementor may read the node attributes to use them as constructor parameters, ...)
		/// </param>
		/// <param name="_target_type">
		/// The type of the return type as is declared by parent object into which this node is to be deserialized. It may be any type, 
		/// even the types, which cannot be instantiated - therefore the implementation must know how to consider the real type and how to create it.
		/// In general, the returned value must be convertible into the _target_type.
		/// </param>
		/// <param name="_config">Configuration of the X2OReader which performs this reading.</param>
		/// <returns>The object, which has been read from the Xml elements, or null, if the implementation do not know how to do it.</returns>
		object ProcessElements(XmlElement _parent_element, Type _target_type, X2OConfig _config);

	}
}
