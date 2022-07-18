using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using TypeConverterHelper;

namespace Extensions
{
    // Since the code is not as beautiful as i would like, enjoy this ascii art
    //
    //                     .
    //                    / V\
    //                  / `  /
    //                 <<   |
    //                 /    |
    //               /      |
    //             /        |
    //           /    \  \ /
    //          (      ) | |
    //  ________|   _/_  | |
    //<__________\______)\__)
    public static class ExtensionsReflection
    {

        public static TypeConversionConfig TypeConversionConfig = new TypeConversionConfig();
        /// <summary>
        /// Reflection Invoke Parameter Helper
        /// </summary>
        /// <param name="method"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static object[] MapParameters(this MethodInfo method, Dictionary<string, object> request, TypeConversionConfig typeConversionConfig = null)
        {
            ParameterInfo[] paramInfos = method.GetParameters();

            string[] paramNames = paramInfos.Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                ParameterInfo currentParam = paramInfos[i];

                if (request.TryGetValue(currentParam.Name, out var value))
                {
                    parameters[i] = paramInfos[i].ParameterType.ConvertToCompatibleType(value, typeConversionConfig);
                }
            }

            return parameters;
        }

        public static object ConvertToCompatibleType(this Type type, object input, TypeConversionConfig typeConversionConfig = null)
        {
            if (typeConversionConfig is null)
            {
                typeConversionConfig = TypeConversionConfig;
            }

            Type UnderlyingType = Nullable.GetUnderlyingType(type);
            var inputType = input?.GetType();
            var convertType = UnderlyingType ?? type;

            if (inputType == type || inputType == UnderlyingType)
            {
                return input;
            }

            string inputStr = input switch
            {
                null => "null",
                _ => input.ToString(),
            };

            if (TypeConverter.ConvertTo(inputStr, convertType, out var result, typeConversionConfig))
            {
                return result;
            }

            if (typeConversionConfig.AcceptLossyConversion)
            {
                if (type.IsClass)
                {
                    return null;
                }
                return Convert.ChangeType(0, convertType);
            }

            throw new FormatException($"Invalid value '{inputStr}' for type '{type.Name}'");
        }

        public static void UpdateSingleProp(this PropertyInfo propInfo, object obj, object data, TypeConversionConfig typeConversionConfig = null)
        {
            Type propType = propInfo.PropertyType;

            var value = propType.ConvertToCompatibleType(data, typeConversionConfig);
            propInfo.SetValue(obj, value);
        }

        public static bool IsAsyncMethod(this MethodInfo methodInfo)
        {
            // Obtain the custom attribute for the method.
            if (methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() is null)
            {
                // Null is returned if the attribute isn't present for the method.
                return false;
            }
            return true;
        }

        public static bool HasProperty(this object objectToCheck, string propertyName)
        {
            var type = objectToCheck.GetType();
            return type.GetProperty(propertyName) != null;
        }

    }

    public class TypeConversionConfig : TypeConverterSettings
    {
        public bool AcceptLossyConversion { get; set; } = false;
    }

    public static class DelegateFactory
    {
        private static readonly Type TypeOfVoid = typeof(void);
        private static readonly Type TypeOfValueType = typeof(ValueType);
        private static readonly Type TypeOfObject = typeof(object);
        private static readonly Type TypeOfObjectArray = typeof(object[]);

        public delegate dynamic Lambda(object target, params object[] arguments);
        //public delegate void ReflectedVoidDelegate(object target, params object[] arguments);

        /// <summary>
        /// Creates a LateBoundMethod delegate from a MethodInfo structure
        /// Basically creates a dynamic delegate on the fly.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static ReflectedDelegate CreateReflectedDelegate(this MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(TypeOfObject, "target");
            ParameterExpression argumentsParameter = Expression.Parameter(TypeOfObjectArray, "arguments");

            var instance = Expression.Condition(
                Expression.Constant(method.IsStatic),
                Expression.Default(method.DeclaringType),
                Expression.Convert(instanceParameter, method.DeclaringType)
                );

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

        public class ReflectedDelegate
        {
            public Lambda Lambda { get; internal set; }
            public ReturnType ReturnType { get; internal set; }
            public bool IsStatic { get; internal set; }
            public bool IsGeneric { get; internal set; }
            public Type DeclaringType { get; set; }
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

    public enum ReturnType
    {
        Void,
        VaueType,
        Object
    }
}
