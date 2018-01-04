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
	/// Default implementation of the IX2OElementsProcessor which is used in case, the target object is derived from the IEnumerable.
	/// As this processor creates only List&lt;object&gt; or List&lt;T&gt;, it may read only types, which can be converted into these types.
	/// For reading other types of enumerables, you may use this implementation as you own base, overriding some the functions.
	/// </summary>
	public class X2OEnumerableProcessor : IX2OElementsProcessor
	{
		/// <summary>Holder for information about IEnumerable type used by this processor.</summary>
		public struct EnumerableInfo
		{
			/// <summary>Instance of the IEnumerable</summary>
			public IEnumerable	Enumerable;

			/// <summary>What is the declared type of the items inside the enumerable (object, or some interface or concrete type?)</summary>
			public Type DeclaredItemType;

			/// <summary>Action, which takes the instance of the IEnumerable and item and adds it to the IEnumerable.</summary>
			public Action<IEnumerable, object> AddAction;

		}

		/// <inheritdoc />
		/// <remarks>
		/// This function firstly check, if the type will be IEnumerable.
		/// Then it tries to create the instance and get information about the enumerable by calling function CreateEnumerable.
		/// If successful, it goes through all the child nodes of _parent_element and deserializes the objects, which are than add
		/// using the AddAction.
		/// </remarks>
		public virtual object								ProcessElements(XmlElement _parent_element, Type _declared_type, X2OConfig _config)
		{
			// if the declared type is not the IEnumerable, we cannot help here
			if (!typeof(IEnumerable).IsAssignableFrom(_declared_type))
				return null;

			var enumerable_info = CreateEnumerable(_parent_element, _declared_type, _config);
			if (!enumerable_info.HasValue)
				return null;

			foreach(XmlElement element in _parent_element.ChildNodes)
			{
				var item = _config.Processor.ProcessElements(element, enumerable_info.Value.DeclaredItemType, _config);
				enumerable_info.Value.AddAction(enumerable_info.Value.Enumerable, item);
			}

			return enumerable_info.Value.Enumerable;
		}


		/// <summary>
		/// This function tries to create the instance of the target type according to the information in _declared_type and _parent_element.
		/// In this implementation, it is able to create only the List&lt;object&gt; or List&lt;T&gt; types, according to the _declared_type and
		/// only in cases, that the _declared_type is convertible into these Lists.
		/// </summary>
		public virtual EnumerableInfo?						CreateEnumerable(XmlElement _parent_element, Type _declared_type, X2OConfig _config)
		{
			Action<IEnumerable, object> add_action = (l, i) => l.GetType().GetMethod("Add").Invoke(l, new[] { i });

			// find if it is generic, find the generic type, create list of that type, and try if it can convert into declared type
			if (_declared_type.IsGenericType && _declared_type.GenericTypeArguments.Length == 1)
			{
				// it is generic type, like List<T> or IEnumerable<T>
				var item_type = _declared_type.GenericTypeArguments[0];
				var tgt_type = typeof(List<>).MakeGenericType(item_type);
				if (_declared_type.IsAssignableFrom(tgt_type))
				{
					try
					{
						return new EnumerableInfo() {
							Enumerable = Activator.CreateInstance(tgt_type) as IEnumerable,
							DeclaredItemType = item_type,
							AddAction = add_action
						};
					}
					catch { }
				}
			}
			else
			{
				if (_declared_type.IsAssignableFrom(typeof(List<object>)))
				{
					try
					{
						return new EnumerableInfo() {
							Enumerable = new List<object>(),
							DeclaredItemType = typeof(object),
							AddAction = add_action
						};
					}
					catch { }
				}
			}

			return null;
		}
	}
}
