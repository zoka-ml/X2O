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

	public interface SimpleInterface
	{
		string D { get; set; }
	}

	public class SimpleInterfaceImpl1 : SimpleInterface
	{
		public string D { get; set; }
		public double E { get; set; }
	}

	public class SimpleInterfaceImpl2 : SimpleInterface
	{
		public string D { get; set; }
		public char F { get; set; }
	}

	public class SimpleClassWithMemberInterface
	{
		public int C;
		public SimpleInterface I { get; set; }
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

		[TestMethod]
		public void GivenSimpleXmlWithInterfaceAsMember_ItDeserializeCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithInterfaceMember.xml") as SimpleClassWithMemberInterface;

			Assert.IsNotNull(simple_class, "SimpleClassWithMemberInterface has not desrialized");
			Assert.AreEqual(7, simple_class.C, "SimpleClassWithMemberInterface.C should be 7");
			Assert.IsNotNull(simple_class.I, "SimpleClassWithMemberInterface.I must not be null");
			Assert.IsInstanceOfType(simple_class.I, typeof(SimpleInterfaceImpl2), "Suppossed the deserialized value to be of SimpleInterfaceImpl2 type");
			Assert.AreEqual("Test text!", simple_class.I.D, "SimpleClassWithMemberInterface.I.D should be \"Test text!\"");
			Assert.AreEqual('x', (simple_class.I as SimpleInterfaceImpl2).F, "SimpleClassWithMemberInterface.I.F should be 'x'");
		}
	}
}
