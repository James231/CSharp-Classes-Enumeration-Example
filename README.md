# C# Classes Enumeration Example

If you have used the `enum` keyword before you will be familiar with this:

```cs
public enum MyEnum {
    Option1,
    Option2,
}
```
where you can create an instance of type `MyEnum` and it either has value `MyEnum.Option1` or `MyEnum.Option2`.

But what if you want to associate more data (fields / properties / methods) with each option? This repo demonstrates how.

The result is being able to store a list of classes (instances) in your `enum` like this:
```cs
// Value of enum can be classes
MyEnum myEnum = MyEnum.MyClass1;

// Set another class value
myEnum = MyEnum.MyClass2;

// Can invoke method from the class
myEnum.Value.MyCommonMethod();
```

**Big Warning:** Using the approach demonstrated in this repo is likely bad practice for most use cases. It's just a fun example of what is possible with C#.

## Why not an instance of an Interface?

^ This is probably your first thought. Why not just do the following:

```cs
public static class SingletonReferences {
    public static MyClass1 MyClass1Instance; // value set in constructor or elsewhere
    public static MyClass2 MyClass1Instance;
}

IMyInterface myClass = SingletonReferences.MyClass1Instance;

myClass = SingletonReferences.MyClass2Instance;

myClass.MyCommonMethod();
```
Here are a few reasons:

1. Serialization. If you serialize an enum it just results in an integer and the class enum is similar. But, using interfaces it will serialize the contents of the whole class rather than an integer identifying the singleton instance. Yes, it can be fixed but it is not default behaviour.

2. Nullablility. Enums are not nullable. Instances of interfaces are. These class enums are somewhere in between. While they are nullable, it is less likely you will accidently set them to null. If you have a better approach to achieve non-nullable class enums (without relying on C# 8 non-nullable warnings), please let me know!

3. Fun.

## How to create your enum:

Firstly all your classes need to use a common interface or base class, in this case both our classes implement `IMyInterface`:
```cs
public interface IMyInterface
{
    void MyCommonMethod();
}
class MyClass1 : IMyInterface
{
    public void MyCommonMethod()
    {
        Console.WriteLine("Method on MyClass1 called");
    }
}
class MyClass2 : IMyInterface
{
    public void MyCommonMethod()
    {
        Console.WriteLine("Method on MyClass2 called");
    }
}
```

Then you need to create your enum type. This cannot be done using `public enum MyEnum { ... }`, instead you create a class which inherits from `Enumeration<T>` where `T` is the type of your interface (or base class). You create your 'options' as `static readonly` fields, and set them to be instances of your `MyEnum` type where you pass instances of your classes `MyClass1` and `MyClass2` into the constructor.

```cs
public class MyEnum : Enumeration<IMyInterface>
{
    public static readonly MyEnum MyClass1 = new MyEnum(new MyClass1());
    public static readonly MyEnum MyClass2 = new MyEnum(new MyClass2());

    public MyEnum(IMyInterface obj)
        : base(obj)
    {
    }
}
```

Then we are done! You can create an instance of your enum in the usual way:
```
MyEnum myEnum = MyEnum.MyClass1;
```
and your variable `myEnum` can either have values `MyEnum.MyClass1` or `MyEnum.MyClass2` (or `null`). Then you can access methods/fields through `myEnum.Value` which returns the instance of `IMyInterface`.

## Serialization

In my opion this is where the main advantage comes over simply using an instance of the interface.

I've included working JSON serialization using `System.Text.Json`. If you serialize the following variable:
```cs
MyEnum myEnum = MyEnum.MyClass1;
```
In JSON it is just serialized to a string:
```json
"myEnum": "StaticClassEnumsExample.MyClass2"
```
where `StaticClassEnumsExample` is the namespace which `MyClass2` lives within. Essentially it just stores the type name.

Serialization can be performed like this using a Custom Converter I've written:
```cs
// Register the custom converter
var serializeOptions = new JsonSerializerOptions();
serializeOptions.Converters.Add(new EnumerationConverter());

// Initialize value of enum
MyEnum myEnum = MyEnum.MyClass1;

// Serialize to JSON
string json = JsonSerializer.Serialize(myEnum, serializeOptions);

// Derserialize from JSON back to MyEnum
MyEnum deserializedEnum = JsonSerializer.Deserialize<MyEnum>(json, serializeOptions);
```

## Why this isn't practical

1. Switch statements don't work. Currently I don't know a way of convincing the compiler that the reference `MyEnum myEnum` must be constant (switch statements require constant values).

2. These kind of enums are still nullable. I can still set `myEnum = null;`. To provent this I highly recommend you use C# 8 and enable warnings when setting non-nullable reference types to null. You can do this by project wide or by adding `#nullable enable` to the top of your file. Then typing `myEnum = null;` gives you a warning (which you can turn into an error by changing you code analysis ruleset). See [this post](https://devblogs.microsoft.com/dotnet/embracing-nullable-reference-types/) for more info.

3. You need to write your own custom JSON converter if you want to use Json.Net instead of System.Text.Json. You can use the included 
convereter as a reference.

4. Other C# devs might see this in your code and question it. It is not standard so might be hard for some to understand.

5. The standard approach isn't that bad. You can just make your classes singletons (without `static`) and pass them around with instances of `IMyInterface` instead. If you are using Dependency Injection, then it should be obvious the classes are singletons. If not, then forcing singletons might make you code a bit harder to read, but C# Devs are likely to be familiar with this approach already.

Also if you want an enum, but you want to store more data in each option than just a name ... then reconsider whether you should really be hardcoding all that information at all. It might be better off dynamically loaded.