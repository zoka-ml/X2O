using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
			var cfg = new X2OConfig(GetDefaultElementsProcessors());

			return cfg;
		}

		/// <summary>Creates new instance of the X2O config, with adding the default type resolvers</summary>
		public static X2OConfig								WithDefaultTypeResolvers()
		{
			var cfg = new X2OConfig(null);
			cfg.m_TypeResolvers.AddRange(cfg.GetDefaultTypeResolvers());
			return cfg;
		}

		/// <summary>Creates new instance of the X2O config, adding the default elements processors and type resolvers</summary>
		public static X2OConfig								DefaultConfig()
		{
			var cfg = new X2OConfig(GetDefaultElementsProcessors());
			cfg.m_TypeResolvers.AddRange(cfg.GetDefaultTypeResolvers());
			return cfg;
		}

		/// <summary>Empty constructor</summary>
		protected X2OConfig()
		{
			Processor = new X2OProcessor();
		}

		/// <summary>Constuctor taking the list of elements processors</summary>
		protected X2OConfig(IEnumerable<X2OElementsProcessors.IX2OElementsProcessor> _elements_processors)
			: this()
		{
			if (_elements_processors != null)
			{
				foreach (var elements_processor in _elements_processors)
				{
					Processor.AddElementsProcessors(elements_processor);
				}
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

		private static IEnumerable<X2OElementsProcessors.IX2OElementsProcessor> GetDefaultElementsProcessors()
		{
			return new X2OElementsProcessors.IX2OElementsProcessor[] {
				new X2OElementsProcessors.X2OComplexObjectProcessor(),
				new X2OElementsProcessors.X2ODictionaryProcessor(),
				new X2OElementsProcessors.X2OEnumerableProcessor(),
				new X2OElementsProcessors.X2OValueProcessor(),
				new X2OElementsProcessors.X2OExternalFileProcessor()
			};
		}

		#endregion // Processor

		#region Type resolvers

		private List<Func<string, Type>>					m_TypeResolvers = new List<Func<string, Type>>();

		/// <summary>The list of type resolvers used to convert type name string into the type</summary>
		public IEnumerable<Func<string, Type>>				TypeResolvers => m_TypeResolvers;

		/// <summary>Will add the type resolver into the list of resolvers</summary>
		public X2OConfig									UsingTypeResolver(Func<string, Type> _type_resolver)
		{
			m_TypeResolvers.Add(_type_resolver);
			return this;
		}

		// ToDo: generally, having this instance method is not ideal. Probably it should be static, but at the moment, 
		// I have no idea, how to correctly resolve need of Processor instance to call GetTypeByName
		private IEnumerable<Func<string, Type>>				GetDefaultTypeResolvers()
		{
			return new Func<string, Type>[] {
				(s => s == "string" ? typeof(string) : null),
				(s => s == "int" ? typeof(int) : null),
				(s => s == "uint" ? typeof(uint) : null),
				(s => s == "long" ? typeof(long) : null),
				(s => s == "ulong" ? typeof(ulong) : null),
				(s => s == "short" ? typeof(short) : null),
				(s => s == "ushort" ? typeof(ushort) : null),
				(s => s == "char" ? typeof(char) : null),
				(s => s == "bool" ? typeof(bool) : null),
				(s => s == "byte" ? typeof(byte) : null),
				(s => s == "sbyte" ? typeof(sbyte) : null),
				(s => s == "decimal" ? typeof(decimal) : null),
				(s => s == "double" ? typeof(double) : null),
				(s => s == "float" ? typeof(float) : null),
				(s => s == "object" ? typeof(object) : null),
				(s => {
					var lt = typeof(List<object>);
					var regex = new Regex(@"(?<gen_type>[A-Za-z_][\w_]*)\s*\["); // (?:\s*,\s*(?<gen_parx>[a-zA-Z_][\w_]*))*\]
					var match = regex.Match(s);
					if (!match.Success)
						return null;
					var gen_type = match.Groups["gen_type"].Value;
					var gen_pars = new List<Type>();
					do
					{
						regex = new Regex(@"\s*,*\s*(?<gen_par>[a-zA-Z_][\w_]*)");
						match = regex.Match(s, match.Index + match.Length);
						if (match.Success)
						{
							var gen_par = Processor.GetTypeByName(match.Groups["gen_par"].Value, this);
							if (gen_par == null)
								return null;
							gen_pars.Add(gen_par);
						}

					} while (match.Success);

					regex = new Regex(@"\]");
					match = regex.Match(s, match.Index + match.Length);
					if (!match.Success || gen_pars.Count == 0)
						return null;

					var generic_type_without_parameters = Processor.GetTypeByName($"{gen_type}`{gen_pars.Count}", this);
					if (generic_type_without_parameters != null)
					{
						var tgt_type = generic_type_without_parameters.MakeGenericType(gen_pars.ToArray());
						return tgt_type;
					}
					return null;
				})

			};
		}

		#endregion // Type resolvers

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
