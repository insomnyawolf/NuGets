using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ExtensionsReflection
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
    public static partial class ExtensionsReflection
    {
        /// <summary>
        /// Reflection Invoke Parameter Helper
        /// </summary>
        /// <param name="method"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static object[] MapParameters(this MethodInfo method, Dictionary<string, object> request)
        {
            ParameterInfo[] paramInfos = method.GetParameters();

            string[] paramNames = paramInfos.Select(p => p.Name).ToArray();
            object[] parameters = new object[paramNames.Length];

            for (int i = 0; i < parameters.Length; ++i)
            {
                ParameterInfo currentParam = paramInfos[i];

                if (request.TryGetValue(currentParam.Name, out var value))
                {
                    parameters[i] = paramInfos[i].ParameterType.ConvertToCompatibleType(value);
                }
            }

            return parameters;
        }

        public static object ConvertToCompatibleType(this Type type, object input, bool acceptLossyConversion = false, string dateFormat = "dd/MM/yyyy", string timespanFormat = "c")
        {
            Type UnderlyingType = Nullable.GetUnderlyingType(type);
            var inputType = input?.GetType();
            var convertType = UnderlyingType ?? type;

            if (inputType == type || inputType == UnderlyingType)
            {
                return input;
            }

            string inputStr;

            if (input is null)
            {
                inputStr = "null";
            }
            else
            {
                inputStr = input.ToString();
            }

            if (string.Equals(inputStr, "null", StringComparison.OrdinalIgnoreCase))
            {
                if (type.IsClass || type.GetGenericTypeDefinition() == TypeNullable)
                {
                    return null;
                }
            }
            else if (convertType == TypeString)
            {
                return inputStr;
            }
            else if (convertType == TypeInt)
            {
                bool Sucess = int.TryParse(s: inputStr, style: NumberStyles.Any, provider: Thread.CurrentThread.CurrentCulture, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeFloat)
            {
                bool Sucess = float.TryParse(s: inputStr, style: NumberStyles.Any, provider: Thread.CurrentThread.CurrentCulture, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeDouble)
            {
                bool Sucess = double.TryParse(s: inputStr, style: NumberStyles.Any, provider: Thread.CurrentThread.CurrentCulture, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeDecimal)
            {
                bool Sucess = decimal.TryParse(s: inputStr, style: NumberStyles.Any, provider: Thread.CurrentThread.CurrentCulture, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeBool)
            {
                bool Sucess = bool.TryParse(value: inputStr, result: out bool Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeDateTime)
            {
                bool Sucess = DateTime.TryParseExact(s: inputStr, format: dateFormat, provider: Thread.CurrentThread.CurrentCulture, style: DateTimeStyles.AllowWhiteSpaces, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeTimeSpan)
            {
                bool Sucess = TimeSpan.TryParseExact(input: inputStr, format: timespanFormat, formatProvider: Thread.CurrentThread.CurrentCulture, styles: TimeSpanStyles.None, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else
            {
                // Es de verdad necesario?
                return Convert.ChangeType(input, convertType);
            }

            if (acceptLossyConversion)
            {
                if (type.IsClass || type.GetGenericTypeDefinition() == TypeNullable)
                {
                    return null;
                }
                return Convert.ChangeType(0, convertType);
            }

            throw new FormatException($"Invalid value '{inputStr}' for type '{type.Name}'");
        }

        public static void UpdateSingleProp(this PropertyInfo propInfo, object obj, object data)
        {
            Type propType = propInfo.PropertyType;

            //try
            //{
            var value = propType.ConvertToCompatibleType(data);
            propInfo.SetValue(obj, value);
            //}
            //catch (Exception ex) when (
            //       ex is NullReferenceException
            //    || ex is InvalidCastException
            //    || ex is FormatException
            //    )
            //{
            //    ProcessException(propType, propInfo, obj);
            //}
        }

        //private static void ProcessException(Type propType, PropertyInfo propInfo, object obj)
        //{
        //    if (propType.IsClass || propType.GetGenericTypeDefinition() == typeof(Nullable<>))
        //    {
        //        propInfo.SetValue(obj, null);
        //    }
        //    else
        //    {
        //        throw new InvalidDataException($"Invalid conversion for property '{propInfo.Name}' of type '{propInfo.DeclaringType.Name}'.");

        //        //propInfo.SetValue(obj, Convert.ChangeType(0, propType));
        //    }
        //}
    }
}
