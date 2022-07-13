using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    }

    public class TypeConversionConfig : TypeConverterSettings
    {
        public bool AcceptLossyConversion { get; set; } = false;
    }
}
