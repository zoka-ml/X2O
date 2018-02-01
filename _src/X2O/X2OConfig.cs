using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoka.X2O
{
	/// <summary>Configuration object for X2OReader.</summary>
	public class X2OConfig
	{

		#region Construction

		/// <summary>Create new instance of the X2O config without any custom settings made</summary>
		public static X2OConfig								EmptyConfig()
		{
			var cfg = new X2OConfig();
			return cfg;
		}

		/// <summary>Creates new instance of the X2O config with adding the default elements processors</summary>
		public static X2OConfig								WithDefaultElementsProcessors()
		{
			var cfg = new X2OConfig(new X2OElementsProcessors.X2OComplexObjectProcessor(),
									new X2OElementsProcessors.X2ODictionaryProcessor(),
									new X2OElementsProcessors.X2OEnumerableProcessor(),
									new X2OElementsProcessors.X2OValueProcessor(),
									new X2OElementsProcessors.X2OExternalFileProcessor()
									);

			return cfg;
		}

		/// <summary>Empty constructor</summary>
		protected X2OConfig()
		{
			Processor = new X2OProcessor();
		}

		/// <summary>Constuctor taking the list of elements processors</summary>
		/// <remarks>
		/// It adds the default elements processors into the default X2OProcessor:
		///		- X2OComplexObjectProcessor
		///		- X2OEnumerableProcessor
		///		- X2OValueProcessor
		/// </remarks>
		protected X2OConfig(params X2OElementsProcessors.IX2OElementsProcessor [] _elements_processors)
			: this()
		{
			foreach (var elements_processor in _elements_processors)
			{
				Processor.AddElementsProcessors(elements_processor);
			}
		}

		#endregion // Construction

		#region Processor

		/// <summary>The processor of the Xml nodes which is used to read whole document and return deserialized objects.</summary>
		public X2OProcessor									Processor { get; }
		/// <summary>Will change the configuration to (also) use the passed elements processor</summary>
		public X2OConfig									UsingElementsProcessor(X2OElementsProcessors.IX2OElementsProcessor _elements_processor)
		{
			Processor.AddElementsProcessors(_elements_processor);
			return this;
		}

		#endregion // Processor

		#region Other settings

		/// <summary>List of search path for external config files</summary>
		protected List<string>								m_ExternalConfigSearchPaths = new List<string>();
		/// <summary>List of search path for external config files</summary>
		public IEnumerable<string>							ExternalConfigSearchPaths => m_ExternalConfigSearchPaths;
		/// <summary>Will add the search path into the list of external config search paths</summary>
		public X2OConfig									UsingExternalConfigSearchPath(string _search_path)
		{
			m_ExternalConfigSearchPaths.Add(_search_path);
			return this;
		}

		#endregion // Other settings
	}
}
