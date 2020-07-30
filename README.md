# C# Classes Enumeration Example

If you have used the `enum` type before you will be familiar with this:

```
public enum MyEnum {
    Option1,
    Option2,
}
```
where you can create an instance of type `MyEnum` and it either has value `MyEnum.Option1` or `MyEnum.Option2`.

But what if you want to associate more data (fields / properties / methods) with each option? This repo contains a very short code example showing how.

The result is being able to store a list of classes (instances) in your `enum` like this:
```
// Value of enum can be classes
MyEnum myEnum = MyEnum.MyClass1;

// Set another class value
myEnum = MyEnum.MyClass2;

// Can invoke method from the  class
myEnum.Value.MyCommonMethod();
```

**Big Warning:** Using the approach demonstrated in this repo is likely bad practice for most use cases. It's just a fun example of what is possible with C#.

## How to create your enum:

Firstly all your classes need to use a common interface or base class, in this case both our classes implement `MyInterface`:
```
public interface MyInterface
{
    void MyCommonMethod();
}
class MyClass1 : MyInterface
{
    public void MyCommonMethod()
    {
        Console.WriteLine("Method on MyClass1 called");
    }
}
class MyClass2 : MyInterface
{
    public void MyCommonMethod()
    {
        Console.WriteLine("Method on MyClass2 called");
    }
}
```

Then you need to creaate your enum type. This cannot be done using `public enum MyEnum { ... }`, instead you create a class which inherits from `Enumeration<T>` where `T` is the type of your interface (or base class). You create your 'options' as fields, and set them to be instances of your `MyEnum` type where you pass instances of your classes `MyClass1` and `MyClass2` into the constructor.

```
public class MyEnum : Enumeration<MyInterface>
{
    public static readonly MyEnum MyClass1 = new MyEnum(1, new MyClass1());
    public static readonly MyEnum MyClass2 = new MyEnum(1, new MyClass2());

    public MyEnum(int id, MyInterface obj)
        : base(id, obj)
    {
    }
}
```

Then we are done! You can create an instance of your enum in the usual way:
```
MyEnum myEnum = MyEnum.MyClass1;
```
and your variable `myEnum` can either have values `MyEnum.MyClass1` or `MyEnum.MyClass2`. Then you can access methods/fields through `myEnum.Value` which returns the instance of MyInterface.


## Why this isn't practical

You could just use an instance of `MyInterface` instead. If you prefer this approach because it avoids your variable being nullable, then be careful because the interface references within the enum could also be null. E.g. `MyInterface.MyClass1.Value` could be null. This makes it almost pointless, but could be fixed if you did some more work with reflection, using static classes and only letting the enum values store class types.

Also if you want an enum, but you want to store more data in each option than just a name ... then reconsider whether you should really be hardcoding all that information at all. It might be better off dynamically loaded.




