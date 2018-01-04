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
		/// <summary>Empty constructor</summary>
		/// <remarks>
		/// It adds the default elements processors into the default X2OProcessor:
		///		- X2OComplexObjectProcessor
		///		- X2OEnumerableProcessor
		///		- X2OValueProcessor
		/// </remarks>
		public X2OConfig()
		{
			Processor = new X2OProcessor(
				new X2OElementsProcessors.X2OComplexObjectProcessor(),
				new X2OElementsProcessors.X2OEnumerableProcessor(),
				new X2OElementsProcessors.X2OValueProcessor()
				);
		}

		/// <summary>The processor of the Xml nodes which is used to read whole document and return deserialized objects.</summary>
		public X2OProcessor									Processor { get; }

	}
}
