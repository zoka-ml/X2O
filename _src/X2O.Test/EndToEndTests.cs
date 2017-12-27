using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zoka.X2O.Test
{
	public class SimpleClass
	{
		public int X;
	}

	public class SimpleClassWithMemberClass
	{
		
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

		[TestMethod]
		public void GivenSimplestXmlWithoutTypeDefinition_ItDeserializeCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithoutClassDefinition.xml") as SimpleClass;

			Assert.IsNotNull(simple_class, "SimpleClass has not desrialized");
			Assert.AreEqual(5, simple_class.X, "SimpleClass.X should be 5");
		}
	}
}
