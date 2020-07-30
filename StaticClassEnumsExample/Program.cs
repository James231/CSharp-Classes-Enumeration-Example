using System;

namespace StaticClassEnumsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Value of enum can be classes
            MyEnum myEnum = MyEnum.MyClass1;

            // Set another class value
            myEnum = MyEnum.MyClass2;

            // Can invoke method from the  class
            myEnum.Value.MyCommonMethod();
        }
    }
}
