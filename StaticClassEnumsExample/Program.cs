#nullable enable

using System;
using System.Text.Json;

namespace StaticClassEnumsExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Value of enum can be classes
            MyEnum myEnum = MyEnum.MyClass1;

            // Set another class value
            myEnum = MyEnum.MyClass1;

            // For serialization with System.Text.Json we need to register a custom converter
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new EnumerationConverter());

            // Serialize to JSON
            string json = JsonSerializer.Serialize(myEnum, serializeOptions);

            // Deserialize back to MyEnum
            MyEnum deserializedEnum = JsonSerializer.Deserialize<MyEnum>(json, serializeOptions);

            // Can invoke method from the class
            deserializedEnum.Value.MyCommonMethod();
        }
    }
}
