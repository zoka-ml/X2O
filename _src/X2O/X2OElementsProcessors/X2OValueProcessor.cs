using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zoka.X2O.X2OElementsProcessors
{
	/// <summary>
	/// Default implementation of the IX2OElementsProcessor which is used in case, the target object is value type or string.
	/// </summary>
	public class X2OValueProcessor : IX2OElementsProcessor
	{
		/// <inheritdoc />
		/// <remarks>The function uses Convert class functionality to convert from XmlValue of child nodes into the target type.</remarks>
		public object ProcessElements(XmlElement _parent_element, Type _target_type, X2OConfig _config)
		{
			if (!_target_type.IsValueType && !_target_type.Equals(typeof(string)))
				return null;

			var xval = _parent_element.ChildNodes[0].Value;
			var tgt_type_val = Convert.ChangeType(xval, _target_type);
			return tgt_type_val;
		}
	}
}
