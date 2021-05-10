using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public static TypeConversionConfig TypeConversionConfig = new();
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
                bool Sucess = int.TryParse(s: inputStr, style: typeConversionConfig.NumberStyles, provider: typeConversionConfig.CultureInfo, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeFloat)
            {
                bool Sucess = float.TryParse(s: inputStr, style: typeConversionConfig.NumberStyles, provider: typeConversionConfig.CultureInfo, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeDouble)
            {
                bool Sucess = double.TryParse(s: inputStr, style: typeConversionConfig.NumberStyles, provider: typeConversionConfig.CultureInfo, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeDecimal)
            {
                bool Sucess = decimal.TryParse(s: inputStr, style: typeConversionConfig.NumberStyles, provider: typeConversionConfig.CultureInfo, result: out var Parsed);
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
                bool Sucess = DateTime.TryParseExact(s: inputStr, format: typeConversionConfig.DateTimeFormat, provider: typeConversionConfig.CultureInfo, style: typeConversionConfig.DateTimeStyles, result: out var Parsed);
                if (Sucess)
                {
                    return Parsed;
                }
            }
            else if (convertType == TypeTimeSpan)
            {
                bool Sucess = TimeSpan.TryParseExact(input: inputStr, format: typeConversionConfig.TimeSpanFormat, formatProvider: typeConversionConfig.CultureInfo, styles: typeConversionConfig.TimeSpanStyles, result: out var Parsed);
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

            if (typeConversionConfig.AcceptLossyConversion)
            {
                if (type.IsClass || type.GetGenericTypeDefinition() == TypeNullable)
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
    }
}
