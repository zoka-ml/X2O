using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zoka.X2O.Helpers;

namespace Zoka.X2O.X2OElementsProcessors
{
	/// <summary>
	/// Default implementation of the IX2OElementsProcessor which is used in case, the target object is complex object (classes, ...)
	/// </summary>
	public class X2OComplexObjectProcessor : IX2OElementsProcessor
	{
		/// <inheritdoc />
		public virtual object								ProcessElements(XmlElement _parent_element, Type _declared_type, X2OConfig _config)
		{
			var inst = CreateInstance(_parent_element, _declared_type, _config);
			if (inst == null)
				return null;

			foreach(XmlElement element in _parent_element.ChildNodes)
			{
				var member_info = inst.GetType().GetMembers().Single(m => m.Name == element.Name);
				var member_type = member_info.GetTypeOfMember();
				var val = _config.Processor.ProcessElements(element, member_type, _config);
				member_info.SetMemberValue(inst, val);
			}

			return inst;
		}

		/// <summary>
		/// Based on the _declared_type and XmlElement settings of _parent_element it creates the target object, where the values from the
		/// child nodes are to be read.
		/// </summary>
		/// <returns>The instance of the target object or null, if the processor doesn't know how to read such type of object.</returns>
		public virtual object								CreateInstance(XmlElement _parent_element, Type _declared_type, X2OConfig _config)
		{
			// first use the type in the type attribute of element, if there is one
			var type_attr = _parent_element.Attributes["type"];
			if (type_attr != null)
			{
				var type = _config.Processor.GetTypeByName(type_attr.Value);
				try
				{
					return Activator.CreateInstance(type);
				}
				catch { }
			}

			// otherwise try the declared type
			try
			{
				return Activator.CreateInstance(_declared_type);
			}
			catch { }

			// or in some cases it may happen, that the node has the name of the type
			{
				var type = _config.Processor.GetTypeByName(_parent_element.Name);
				try
				{
					return Activator.CreateInstance(type);
				}
				catch { }
			}
			return null;
		}
	}
}
