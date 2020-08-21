using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace StaticClassEnumsExample
{
    public abstract class Enumeration<U> : IComparable
    {
        [JsonPropertyName("id")]
        public string Id { get; protected set; }

        [JsonIgnore]
        public U Value { get; private set; }

        protected Enumeration(U value)
        {
            Id = value.GetType().ToString();
            Value = value;
        }

        public override string ToString() => Value.ToString();

        public static IEnumerable<T> GetAll<T>() where T : Enumeration<U>
        {
            var fields = typeof(T).GetFields(BindingFlags.Public |
                                             BindingFlags.Static |
                                             BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration<U>;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }

        public int CompareTo(object other) => Id.CompareTo(((Enumeration<U>)other).Id);
    }
}
