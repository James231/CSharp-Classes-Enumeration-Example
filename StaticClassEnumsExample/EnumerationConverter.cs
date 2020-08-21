using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StaticClassEnumsExample
{
    public class EnumerationConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return IsSubclassOfRawGeneric(typeof(Enumeration<>), typeToConvert);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            Type baseInterfaceType = typeToConvert.BaseType.GetGenericArguments()[0];

            JsonConverter converter = (JsonConverter)Activator.CreateInstance(
                typeof(EnumerationConverterInner<,>).MakeGenericType(
                    new Type[] { typeToConvert, baseInterfaceType }),
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: new object[] { options },
                culture: null);

            return converter;
        }

        private class EnumerationConverterInner<T, U> :
            JsonConverter<T> where T : Enumeration<U>
        {
            private Type _keyType;

            public EnumerationConverterInner(JsonSerializerOptions options)
            {
                // Cache the key and value types.
                _keyType = typeof(T);
            }

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                string serializedEnum = JsonSerializer.Deserialize<string>(ref reader, options);
                Type selectedType = Type.GetType(serializedEnum);

                if (selectedType == null)
                    throw new JsonException("Deserialzation failed, type string not valid.");

                // Go through static fields of T
                FieldInfo[] fields = _keyType.GetFields(BindingFlags.Static | BindingFlags.Public);
                foreach (FieldInfo info in fields)
                {
                    // Find field (which is of type T) where Value (property on base class is of type selectedType)
                    if (info.FieldType == _keyType)
                    {
                        T fieldValue = (T)info.GetValue(null);
                        if (fieldValue.Value.GetType() == selectedType)
                        {
                            // Then return the value of that field
                            return fieldValue;
                        }
                    }
                }

                throw new JsonException("Deserialization failed to find Type.");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                Enumeration<U> enumeration = (Enumeration<U>)value;
                JsonSerializer.Serialize(writer, enumeration.Value.GetType().ToString(), options);
            }
        }

        private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
