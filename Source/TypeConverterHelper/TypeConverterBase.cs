using System;
using System.Reflection;

namespace TypeConverterHelper
{
    public abstract class TypeConverterBase 
    {
        public abstract MethodInfo ConvertMethodReflectionCache { get; set; }
    }

    public abstract class TypeConverterBase<T> : TypeConverterBase
    {
        public override MethodInfo ConvertMethodReflectionCache { get; set; }

        public TypeConverterBase()
        {
            var type = GetType();

            ConvertMethodReflectionCache = type.GetMethod("Convert");
        }

        public abstract bool Convert(string value, out T result, TypeConverterSettings typeConverterSettings = null);
    }
}
