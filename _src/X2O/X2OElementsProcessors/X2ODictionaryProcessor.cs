using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Zoka.X2O.Helpers;

namespace Zoka.X2O.X2OElementsProcessors
{
	/// <summary>
	/// Default implementation of the IX2OElementsProcessor which is used in case, the target object is derived from the IDictionary.
	/// </summary>
	public class X2ODictionaryProcessor : IX2OElementsProcessor
	{
		/// <summary>Information about dictionary</summary>
		public struct DictionaryInfo
		{
			/// <summary>Instance of the dictionary</summary>
			public IDictionary Dictionary;
			/// <summary>The type of the Keys in dictionary</summary>
			public Type KeyType;
			/// <summary>The type of the Values in dictionary</summary>
			public Type ValueType;
		}

		/// <inheritdoc />
		/// <remarks>
		/// </remarks>
		public virtual object								ProcessElements(XmlElement _parent_element, Type _target_type, X2OConfig _config)
		{
			// if the declared type is not the IDictionary, we cannot help here
			if (!typeof(IDictionary).IsAssignableFrom(_target_type))
				return null;

			var dictionary_info = CreateDictionary(_parent_element, _target_type, _config);
			if (!dictionary_info.HasValue)
				return null;

			foreach (XmlElement element in _parent_element.ChildNodes)
			{
				if (element.Name == "Item")
				{
					XmlElement key_node = null, value_node = null;
					foreach(XmlElement item_element in element.ChildNodes)
					{
						if (item_element.Name == "Key")
							key_node = item_element;
						if (item_element.Name == "Value")
							value_node = item_element;
					}
					if (key_node != null)
					{
						var key = _config.Processor.ProcessElements(key_node, dictionary_info.Value.KeyType, _config);
						object value = null;
						if (value_node != null)
							value = _config.Processor.ProcessElements(value_node, dictionary_info.Value.ValueType, _config);

						if (key != null)
							dictionary_info.Value.Dictionary.Add(key, value);
					}
				}
			}
			return dictionary_info.Value.Dictionary;
		}

		/// <summary></summary>
		public virtual DictionaryInfo?							CreateDictionary(XmlElement _parent_element, Type _target_type, X2OConfig _config)
		{
			if (_target_type.IsGenericType && _target_type.GenericTypeArguments.Length == 2)
			{
				// generic dictionary like Dictionary<T1, T2>
				var key_type = _target_type.GenericTypeArguments[0];
				var value_type = _target_type.GenericTypeArguments[1];
				var dict_type = typeof(Dictionary<,>).MakeGenericType(key_type, value_type);
				if (dict_type.IsAssignableFrom(_target_type))
				{
					try
					{
						var dict = Activator.CreateInstance(_target_type) as IDictionary;
						// declared type is derived from Dictionary<T1,T2> and we guessed the type parameters correctly
						var dict_info = new DictionaryInfo() {
							Dictionary = dict,
							KeyType = key_type,
							ValueType = value_type
						};
						return dict_info;
					}
					catch { }
				}
			}

			// in any other cases, we cannot guess the key and value types without traversing the object hierarchy
			// for these cases, it is better to return without guessing
			// user can override this function to get the correct types

			return null;
		}

	}
}
