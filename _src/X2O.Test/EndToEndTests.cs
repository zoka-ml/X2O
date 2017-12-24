using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zoka.X2O.Test
{
	public class SimpleClass
	{
		public int X;
	}

	[TestClass]
	public class EndToEndTests
	{
		[TestMethod]
		public void GivenSimplestXml_ItDeserializeCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClass.xml") as SimpleClass;

			Assert.IsNotNull(simple_class, "SimpleClass has not desrialized");
			Assert.AreEqual(5, simple_class.X, "SimpleClass.X should be 5");
		}
	}
}
