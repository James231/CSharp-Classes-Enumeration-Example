using System;
using System.Collections.Generic;
using System.Text;

namespace StaticClassEnumsExample
{
    public class MyEnum : Enumeration<MyInterface>
    {
        public static readonly MyEnum MyClass1 = new MyEnum(1, new MyClass1());
        public static readonly MyEnum MyClass2 = new MyEnum(1, new MyClass2());

        public MyEnum(int id, MyInterface obj)
        : base(id, obj)
        {
        }
    }
}
