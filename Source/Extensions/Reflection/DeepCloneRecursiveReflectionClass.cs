using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System;
using System.Numerics;

namespace Extensions
{
    public static class DeepCloneRecursiveReflectionClass
    {
        private static readonly HashSet<Type> InmutableValueTypes = new HashSet<Type>()
        {
            typeof(decimal),
            typeof(Complex),
            typeof(BigInteger),
            typeof(Guid),
            typeof(DateTime),
            typeof(TimeSpan),
            typeof(DateTimeOffset),
        };

        private static readonly Func<object, object> MemberwiseClone;
        static DeepCloneRecursiveReflectionClass()
        {
            MethodInfo cloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var p1 = Expression.Parameter(typeof(object));
            var body = Expression.Call(p1, cloneMethod);
            MemberwiseClone = Expression.Lambda<Func<object, object>>(body, p1).Compile();
        }

        public static T DeepCloneRecursiveReflection<T>(this T original)
        {
            var Visited = new Dictionary<object, object>(/*ReferenceEqualityComparer.Instance*/);
            var nonShallowFields = new Dictionary<Type, FieldInfo[]>();

            return (T)InternalCopy(original, true);

            object InternalCopy(object originalObject, bool includeInObjectGraph)
            {
                if (originalObject is null)
                {
                    return default;
                }

                var typeToReflect = originalObject.GetType();
                if (IsPrimitiveOrImmutable(typeToReflect))
                {
                    return originalObject;
                }

                if (typeof(XElement).IsAssignableFrom(typeToReflect)) return new XElement((XElement)originalObject);

                if (typeof(Delegate).IsAssignableFrom(typeToReflect))
                {
                    // Avoid cloning event subscriptin if any
                    return default;
                }

                if (includeInObjectGraph)
                {
                    if (Visited.TryGetValue(originalObject, out var result)) return result;
                }

                var cloneObject = MemberwiseClone(originalObject);

                if (includeInObjectGraph)
                {
                    Visited.Add(originalObject, cloneObject);
                }

                if (typeToReflect.IsArray)
                {
                    var arrayElementType = typeToReflect.GetElementType()!;

                    if (IsPrimitiveOrImmutable(arrayElementType))
                    {
                        // for an array of primitives, do nothing. The shallow clone is enough.
                    }
                    else if (arrayElementType.IsValueType)
                    {
                        // if its an array of structs, there's no need to check and add the individual elements to 'visited', because in .NET it's impossible to create
                        // references to individual array elements.
                        ReplaceArrayElements((Array)cloneObject, x => InternalCopy(x, false));
                    }
                    else
                    {
                        // it's an array of ref types
                        ReplaceArrayElements((Array)cloneObject, x => InternalCopy(x, true));
                    }
                }
                else
                {
                    var cachedFields = CachedNonShallowFields(typeToReflect);
                    for (int index = 0; index < cachedFields.Length; index++)
                    {
                        var fieldInfo = cachedFields[index];

                        var originalFieldValue = fieldInfo.GetValue(originalObject);
                        // a valuetype field can never have a reference pointing to it, so don't check the object graph in that case
                        var clonedFieldValue = InternalCopy(originalFieldValue, !fieldInfo.FieldType.IsValueType);
                        fieldInfo.SetValue(cloneObject, clonedFieldValue);
                    }
                }

                return cloneObject;
            }

            FieldInfo[] CachedNonShallowFields(Type typeToReflect)
            {
                if (!nonShallowFields.TryGetValue(typeToReflect, out var result))
                {
                    result = NonShallowFields(typeToReflect).ToArray();
                    nonShallowFields[typeToReflect] = result;
                }
                return result;
            }
        }

        private static bool IsPrimitiveOrImmutable(Type type)
        {
            if (type.IsPrimitive)
            {
                return true;
            }
            else if (type.IsValueType)
            {
                if (type.IsEnum) return true;
                return InmutableValueTypes.Contains(type);
            }
            else
            {
                // ref types.. only return true here if the object is DEEPLY immutable, so collections like
                // ImmutableList<T> don't necessarily qualify because the list items themselves could be mutable.
                if (type == typeof(string)) return true;
            }
            return false;
        }

        private static void ReplaceArrayElements(Array array, Func<object?, object?> func, int dimension, int[] counts, int[] indices)
        {
            int len = counts[dimension];

            if (dimension < (counts.Length - 1))
            {
                // not the final dimension, loop the range, and recursively handle one dimension higher
                for (int t = 0; t < len; t++)
                {
                    indices[dimension] = t;
                    ReplaceArrayElements(array, func, dimension + 1, counts, indices);
                }
            }
            else
            {
                // we've reached the final dimension where the elements are closest together in memory. Do a final loop.
                for (int t = 0; t < len; t++)
                {
                    indices[dimension] = t;
                    array.SetValue(func(array.GetValue(indices)), indices);
                }
            }
        }

        private static void ReplaceArrayElements(Array array, Func<object?, object?> func)
        {
            if (array.Rank == 1)
            {
                // do a fast loop for the common case, a one dimensional array
                int len = array.GetLength(0);
                for (int t = 0; t < len; t++)
                {
                    array.SetValue(func(array.GetValue(t)), t);
                }
            }
            else
            {
                // multidimensional array: recursively loop through all dimensions, starting with dimension zero.
                var counts = Enumerable.Range(0, array.Rank).Select(array.GetLength).ToArray();
                var indices = new int[array.Rank];
                ReplaceArrayElements(array, func, 0, counts, indices);
            }
        }

        private static IEnumerable<FieldInfo> NonShallowFields(Type typeToReflect)
        {
            while (typeToReflect.BaseType != null)
            {
                foreach (var fieldInfo in typeToReflect.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (IsPrimitiveOrImmutable(fieldInfo.FieldType)) continue; // this is 5% faster than a where clause..
                    yield return fieldInfo;
                }
                typeToReflect = typeToReflect.BaseType;
            }
        }
    }
}