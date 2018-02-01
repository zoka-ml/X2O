# Zoka.X2O - What is it for?
**Zoka.X2O** is small C# library used to deserialize your XML (mainly configuration) files into the strongly typed objects, without need to write any code for handling intefaces, enumerables and so. It uses simple typing conventions to get targets object with single line of code. Example:
```C#
var instance = X2OReader.ReadFromFile("YourFile.xml") as ISomeInterface;
```
Oh, you miss the definition for `ISomeInterface`, right?
```C#
public interface ISomeInterface {
	int X { get; set; }
}
public class SomeClassOfISomeInterfaceImplementation {
	public int X { get; set; }
	public string S { get; set; }
}
```
And finally the XML:
```XML
<?xml version="1.0" encoding="utf-8" ?>
<SomeClassOfISomeInterfaceImplementation>
	<X>5</X>
	<S>text info</S>
</SomeClassOfISomeInterfaceImplementation>
```
And that is all. With the single line from the beginning you receive the correct instance of the interface, with correct data read, with no other configuration required.
