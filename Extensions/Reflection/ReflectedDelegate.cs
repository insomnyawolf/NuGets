using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Extensions.Reflection
{
    public static class DelegateFactory
    {
        private static readonly Type TypeOfVoid = typeof(void);
        private static readonly Type TypeOfValueType = typeof(ValueType);
        private static readonly Type TypeOfObject = typeof(object);
        private static readonly Type TypeOfObjectArray = typeof(object[]);

        //public delegate void ReflectedVoidDelegate(object target, params object[] arguments);

        /// <summary>
        /// Creates a LateBoundMethod delegate from a MethodInfo structure
        /// Basically creates a dynamic delegate on the fly.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ReflectedDelegate CreateReflectedDelegate(this MethodInfo method)
        {
            var instanceParameter = Expression.Parameter(TypeOfObject, "target");
            var argumentsParameter = Expression.Parameter(TypeOfObjectArray, "arguments");

            Expression instance;

            if (method.IsStatic)
            {
                instance = null;
            }
            else
            {
                instance = Expression.Convert(instanceParameter, method.DeclaringType);
            }

            //var instance = Expression.Condition(
            //    Expression.Constant(method.IsStatic),
            //    Expression.Default(method.DeclaringType),
            //    Expression.Convert(instanceParameter, method.DeclaringType)
            //    );

            var call = Expression.Call(
                instance,
                method,
                CreateParameterExpressions(method, argumentsParameter)
            );

            var reflectedDelegate = new ReflectedDelegate()
            {
                IsStatic = method.IsStatic,
                DeclaringType = method.DeclaringType,
                IsGeneric = method.IsGenericMethod,
                ParameterInfos = method.GetParameters(),
            };

            if (method.ReturnType == TypeOfVoid)
            {
                reflectedDelegate.ReturnType = ReturnType.Void;
                reflectedDelegate.Lambda = Expression.Lambda<Lambda>(
                            Expression.Block(call, Expression.Constant(null)),
                            instanceParameter,
                            argumentsParameter
                        ).Compile();
            }
            else if (method.ReturnType.IsValueType)
            {
                reflectedDelegate.ReturnType = ReturnType.VaueType;
                reflectedDelegate.Lambda = Expression.Lambda<Lambda>(
                            Expression.Convert(call, TypeOfValueType),
                            instanceParameter,
                            argumentsParameter
                        ).Compile();
            }
            else
            {
                reflectedDelegate.ReturnType = ReturnType.Object;
                reflectedDelegate.Lambda = Expression.Lambda<Lambda>(
                            Expression.Convert(call, TypeOfObject),
                            instanceParameter,
                            argumentsParameter
                        ).Compile();
            }

            return reflectedDelegate;
        }

        /// <summary>
        /// Creates a LateBoundMethod from type methodname and parameter signature that
        /// is turned into a MethodInfo structure and then parsed into a dynamic delegate
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        //public static object Create(Type type, string methodName, params Type[] parameterTypes)
        //{
        //    return type.GetMethod(methodName, parameterTypes).CreateReflectedDelegate();
        //}

        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            return method.GetParameters().Select((parameter, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(argumentsParameter, Expression.Constant(index)),
                    parameter.ParameterType)
                ).ToArray();
        }
    }

    public delegate dynamic Lambda(object target, params object[] arguments);

    public class ReflectedDelegate
    {
        public Lambda Lambda { get; internal set; }
        public ReturnType ReturnType { get; internal set; }
        public bool IsStatic { get; internal set; }
        public bool IsGeneric { get; internal set; }
        public Type DeclaringType { get; set; }
        public ParameterInfo[] ParameterInfos { get; set; }
    }

    public enum ReturnType
    {
        Void,
        VaueType,
        Object
    }
}
