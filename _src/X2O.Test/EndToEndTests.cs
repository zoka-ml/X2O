using System;
using System.Collections.Generic;
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

	public class ClassWithEnumerationOfInts
	{
		public string G;
		public IEnumerable<int> H { get; set; }
	}

	[TestClass]
	public class EndToEndTests
	{
		[TestMethod]
		public void GivenXmlWithIntOnly_ItDeserializesCorrectly()
		{
			var i = (int)X2OReader.ReadFromFile("ContentData\\EndToEndTests\\OnlyInt.xml");

			Assert.AreEqual(2, i);
		}

		[TestMethod]
		public void GivenXmlWithSimpleClass_ItDeserializesCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClass.xml") as SimpleClass;

			Assert.IsNotNull(simple_class, "SimpleClass has not desrialized");
			Assert.AreEqual(5, simple_class.X, "SimpleClass.X should be 5");
		}

		[TestMethod]
		public void GivenXmlWithSimpleClassWithoutTypeSpecification_ItDeserializesCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithoutClassDefinition.xml") as SimpleClass;

			Assert.IsNotNull(simple_class, "SimpleClass has not desrialized");
			Assert.AreEqual(5, simple_class.X, "SimpleClass.X should be 5");
		}

		[TestMethod]
		public void GivenXmlWithClassWithAnotherClassAsMember_ItDeserializesCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithMemberClass.xml") as SimpleClassWithMemberClass;

			Assert.IsNotNull(simple_class, "SimpleClassWithMemberClass has not desrialized");
			Assert.AreEqual(10, simple_class.A, "SimpleClassWithMemberClass.A should be 10");
			Assert.IsNotNull(simple_class.B, "SimpleClassWithMemberClass.B must not be null");
			Assert.AreEqual(5, simple_class.B.X, "SimpleClassWithMemberClass.B.X should be 5");
		}

		[TestMethod]
		public void GivenXmlWithClassWithInterfaceAsMember_ItDeserializesCorrectly()
		{
			var simple_class = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\SimpleClassWithInterfaceMember.xml") as SimpleClassWithMemberInterface;

			Assert.IsNotNull(simple_class, "SimpleClassWithMemberInterface has not desrialized");
			Assert.AreEqual(7, simple_class.C, "SimpleClassWithMemberInterface.C should be 7");
			Assert.IsNotNull(simple_class.I, "SimpleClassWithMemberInterface.I must not be null");
			Assert.IsInstanceOfType(simple_class.I, typeof(SimpleInterfaceImpl2), "Suppossed the deserialized value to be of SimpleInterfaceImpl2 type");
			Assert.AreEqual("Test text!", simple_class.I.D, "SimpleClassWithMemberInterface.I.D should be \"Test text!\"");
			Assert.AreEqual('x', (simple_class.I as SimpleInterfaceImpl2).F, "SimpleClassWithMemberInterface.I.F should be 'x'");
		}

		[TestMethod]
		public void GivenXmlOfClassWithEnumerationOfInts_ItDesrializesCorrectly()
		{
			var result = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\ClassWithEnumerationOfInts.xml") as ClassWithEnumerationOfInts;

			Assert.IsNotNull(result, "ClassWithEnumerationOfInts has not deserialized");
			Assert.AreEqual("Test text!", result.G);
			Assert.IsNotNull(result.H, "ClassWithEnumerationOfInts.H should not be null");
			var enumerator = result.H.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(10, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(11, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
		}

		[TestMethod]
		public void GivenXmlOfEnumerableOfInterface_ItDeserializesCorrectly()
		{
			var result = X2OReader.ReadFromFile<IEnumerable<SimpleInterface>>("ContentData\\EndToEndTests\\EnumerableOfInterfaces.xml");

			Assert.IsNotNull(result);
			Assert.IsInstanceOfType(result, typeof(IEnumerable<SimpleInterface>));
			var enumerator = result.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsInstanceOfType(enumerator.Current, typeof(SimpleInterfaceImpl1));
			Assert.AreEqual("Test text 1", (enumerator.Current as SimpleInterfaceImpl1).D);
			Assert.AreEqual(15, (enumerator.Current as SimpleInterfaceImpl1).E);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsInstanceOfType(enumerator.Current, typeof(SimpleInterfaceImpl2));
			Assert.AreEqual("Test text 2", (enumerator.Current as SimpleInterfaceImpl2).D);
			Assert.AreEqual('x', (enumerator.Current as SimpleInterfaceImpl2).F);
			Assert.IsFalse(enumerator.MoveNext());
		}

		[TestMethod]
		public void GivenXmlWithPartiallyExternalConfig_ItDeserializesCorrectly()
		{
			var result = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\ClassWithPartiallyExternalConfig.xml") as SimpleClassWithMemberInterface;

			Assert.IsNotNull(result);
			Assert.AreEqual(7, result.C);
			Assert.IsNotNull(result.I);
			Assert.IsInstanceOfType(result.I, typeof(SimpleInterfaceImpl2));
			Assert.AreEqual("Test text!", (result.I as SimpleInterfaceImpl2).D);
			Assert.AreEqual('x', (result.I as SimpleInterfaceImpl2).F);
		}

		[TestMethod]
		public void GivenXmlWithDictionary_String_SimpleClass_ItDeserializesCorrectly()
		{
			var result = X2OReader.ReadFromFile<Dictionary<string, SimpleClass>>("ContentData\\EndToEndTests\\Dictionary_String_SimpleClass.xml");

			Assert.IsNotNull(result);
			Assert.IsNotNull(result["ItemA"]);
			Assert.AreEqual(10, result["ItemA"].X);
			Assert.IsNotNull(result["ItemB"]);
			Assert.AreEqual(20, result["ItemB"].X);
		}

		[TestMethod]
		public void GivenXmlWithDictionary_String_Object_ItDeserializesCorrectly()
		{
			var result = X2OReader.ReadFromFile<Dictionary<string, object>>("ContentData\\EndToEndTests\\Dictionary_String_Object.xml");

			Assert.IsNotNull(result);
			Assert.IsNotNull(result["Item1"]);
			Assert.IsInstanceOfType(result["Item1"], typeof(SimpleClass));
			Assert.AreEqual(10, (result["Item1"] as SimpleClass).X);
			Assert.IsNotNull(result["Item2"]);
			Assert.IsInstanceOfType(result["Item2"], typeof(ClassWithEnumerationOfInts));
			var item2 = result["Item2"] as ClassWithEnumerationOfInts;
			Assert.AreEqual("Test text", item2.G);
			Assert.IsNotNull(item2.H);
			var enumerator = item2.H.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(20, enumerator.Current);
			Assert.IsTrue(enumerator.MoveNext());
			Assert.AreEqual(21, enumerator.Current);
			Assert.IsFalse(enumerator.MoveNext());
			Assert.IsNotNull(result["Item3"]);
			Assert.IsInstanceOfType(result["Item3"], typeof(int));
			Assert.AreEqual(30, result["Item3"]);
		}

		/// <remarks>
		/// Aim of this test is to test, that type name can be specified simply, like "int" instead of full C# name "Int32". 
		/// It also checks, that generic names may be specified simply. Normally, the name of the List&lt;int&gt; cannot be specified in Xml because:
		///   - characters &lt; &gt; need to be escaped (too complicated)
		///   - and this type cannot be find easily, because, the type name is List`1[Int32] (also too complicated to write in xml)
		/// We assume, that it would be much more easier to write type like List[int] and it could be found.
		/// </remarks>
		[TestMethod]
		public void GivenXmlWithEnumerableOfObjects_UsingSimpleTypeNames_ItDeserializesCorrectly()
		{
			var result = X2OReader.ReadFromFile("ContentData\\EndToEndTests\\EnumerableOfObjects_UsingSimpleTypeNames.xml") as IEnumerable<object>;

			Assert.IsNotNull(result);
			var enumerator = result.GetEnumerator();
			Assert.IsTrue(enumerator.MoveNext());
			Assert.IsInstanceOfType(enumerator.Current, typeof(int));
			var val1 = (int)enumerator.Current;
			Assert.AreEqual(15, val1);
			Assert.IsFalse(enumerator.MoveNext());
		}

	}
}
