using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Zoka.X2O.X2OElementsProcessors
{
	/// <summary>
	/// Default implementation of the IX2OElementsProcessor which is used in case, the _parent_element has attribute which
	/// indicates, that the content of the node should be read from another file.
	/// </summary>
	public class X2OExternalFileProcessor : IX2OElementsProcessor
	{
		/// <inheritdoc />
		/// <remarks>
		/// The function looks for the fromFile attribute, and if found, and there are no children it uses the X2OReader to 
		/// read this file and desrialize its content, which is then returned as the value of this element.
		/// </remarks>
		public object ProcessElements(XmlElement _parent_element, Type _target_type, X2OConfig _config)
		{
			var external_config_attr = _parent_element.Attributes["fromFile"];
			if (external_config_attr != null && !_parent_element.HasChildNodes)
			{
				foreach(var possible_file in FindExistingFiles(external_config_attr.Value, _config))
				{
					try
					{
						var result = X2OReader.ReadFromFile(possible_file, _config);
						if (result != null)
							return result;
					}
					catch { }
				}
			}

			return null;
		}

		/// <summary>
		/// Based on the filename in the attribute for loading the external file, it finds the files, which exists
		/// and could be loaded to be read.
		/// It uses the ExternalConfigSearchPaths property of the X2OConfig, after it is not found in the application current directory.
		/// </summary>
		private IEnumerable<string>							FindExistingFiles(string _filename, X2OConfig _config)
		{
			var fi = new System.IO.FileInfo(_filename);
			if (fi.Exists)
				yield return fi.FullName;

			if (_config?.ExternalConfigSearchPaths != null)
			{
				foreach (var search_path in _config.ExternalConfigSearchPaths)
				{
					var pathname = System.IO.Path.Combine(search_path, _filename);
					fi = new System.IO.FileInfo(pathname);
					if (fi.Exists)
						yield return fi.FullName;
				}
			}

			yield break;
		}

	}
}
