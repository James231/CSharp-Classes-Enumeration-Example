using System;
using System.Collections.Generic;
using System.Text;

namespace StaticClassEnumsExample
{
    public class MyEnum : Enumeration<IMyInterface>
    {
        public static readonly MyEnum MyClass1 = new MyEnum(new MyClass1());
        public static readonly MyEnum MyClass2 = new MyEnum(new MyClass2());

        public MyEnum(IMyInterface obj)
        : base(obj)
        {
        }
    }
}
