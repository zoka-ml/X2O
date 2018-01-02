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
		public int A { get; set; }
		public SimpleClass B { get; set; }
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

		[TestMethod]
		public void GivenSimpleXmlWithAnotherClassAsMember_ItDeserializeCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithMemberClass.xml") as SimpleClassWithMemberClass;

			Assert.IsNotNull(simple_class, "SimpleClassWithMemberClass has not desrialized");
			Assert.AreEqual(10, simple_class.A, "SimpleClassWithMemberClass.A should be 10");
			Assert.IsNotNull(simple_class.B, "SimpleClassWithMemberClass.B must not be null");
			Assert.AreEqual(5, simple_class.B.X, "SimpleClassWithMemberClass.B.X should be 5");
		}
	}
}
