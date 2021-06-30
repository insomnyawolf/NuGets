using ReflectionExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace CsvToObjects
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
    public static class CsvTools
    {
        public static CsvToolsConfig CsvToolsConfig = new CsvToolsConfig();

        #region Mensajes

        public const string ColumnaInvalida = "Tamaño de Columna Invalido en la linea {0} --> {1}";

        #endregion Mensajes

        /// <summary>
        /// Converts the data contained in the stream into the desired object array
        /// </summary>
        /// <typeparam name="T">
        /// Desired Output Object
        /// </typeparam>
        /// <param name="stream">
        /// DataStream wich contains the data that will be parsed
        /// </param>
        /// <param name="values">Parsed Values</param>
        /// <returns>If the conversion was sucesfull or not</returns>
        public static bool Deserialize<T>(Stream stream, out List<T> values, CsvToolsConfig csvToolsConfig = null) where T : class, new()
        {
            values = new List<T>();

            if (csvToolsConfig is null)
            {
                csvToolsConfig = CsvToolsConfig;
            }

            using (var streamReader = csvToolsConfig.EncodingOrigin is null ? new StreamReader(stream) : new StreamReader(stream, csvToolsConfig.EncodingOrigin))
            {
                string data = streamReader.ReadToEnd();

                Type currentType = typeof(T);
                PropertyInfo[] properties = currentType.GetProperties();

                string line;
                int lineCount = 0;

                string[] fields = Array.Empty<string>();
                bool IsFirst = true;

                CsvIgnoreTopAttribute customAttribute = currentType.GetCustomAttribute<CsvIgnoreTopAttribute>();

                using (var stringReader = new StringReader(data))
                {
                    while ((line = stringReader.ReadLine()) != null)
                    {
                        lineCount++;
                        string[] lineFields = line.Split(new char[] { csvToolsConfig.SplitPattern }, StringSplitOptions.None);
                        if (customAttribute == null || customAttribute.IgnoreTop < lineCount)
                        {
                            if (IsFirst)
                            {
                                IsFirst = false;
                                fields = lineFields;
                                if (!IsValidateCsv(properties, fields))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                values.Add(DeserializeObj<T>(lineFields, fields, currentType, properties, csvToolsConfig.TypeConversionConfig));
                            }
                        }
                    }
                }
            }
            return true;
        }

        private static bool IsValidateCsv(PropertyInfo[] Properties, string[] CsvColumns)
        {
            var RequieredColumnsProp = Properties.Where(Property => Property.GetCustomAttributes<CsvRequieredColumnAttribute>().Any()).ToArray();

            List<string> RequieredColumns = new List<string>();

            for (int Index = 0; Index < RequieredColumnsProp.Length; Index++)
            {
                RequieredColumns.Add(GetColumnName(RequieredColumnsProp[Index]));
            }

            var MissingColumns = RequieredColumns.Count(RequieredColumn => !CsvColumns.Any(CsvColumn => CsvColumn == RequieredColumn));

            return MissingColumns == 0;
        }

        /// <summary>
        /// Deserialized x1 register into an object
        /// </summary>
        /// <typeparam name="T">Desired Object Output</typeparam>
        /// <returns>deserialized object</returns>
        private static T DeserializeObj<T>(string[] fieldData, string[] fields, Type currentType, PropertyInfo[] Properties, TypeConversionConfig typeConversionConfig)
        {
            var obj = FormatterServices.GetSafeUninitializedObject(currentType);
            MemberInfo[] members = FormatterServices.GetSerializableMembers(currentType);
            object[] data = new object[members.Length];

            for (int index = 0; index < Properties.Length; index++)
            {
                Type propType = Properties[index].PropertyType;

                CsvIgnoreAttribute ignoreAttribute = Properties[index].GetCustomAttribute<CsvIgnoreAttribute>();
                if (ignoreAttribute == null)
                {
                    string name = GetColumnName(Properties[index]);

                    for (int i = 0; i < fields.Length; ++i)
                    {
                        if (i < fieldData.Length && name == fields[i])
                        {
                            Properties[index].SetValue(obj, propType.ConvertToCompatibleType(fieldData[i], typeConversionConfig));
                        }
                    }
                }
            }
            return (T)FormatterServices.PopulateObjectMembers(obj, members, data);
        }

        private const char LineBreak = '\n';

        public static string Serialize<T>(IList<T> data, CsvToolsConfig csvToolsConfig = null) where T : class
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (csvToolsConfig is null)
            {
                csvToolsConfig = CsvToolsConfig;
            }

            Type currentType = typeof(T);
            PropertyInfo[] Properties = currentType.GetProperties();

            StringBuilder CsvBuilder = new StringBuilder();
            // Column Definition
            int IndexToStopSplit = Properties.Length - 1;
            for (int prop = 0; prop < Properties.Length; prop++)
            {
                CsvIgnoreAttribute ignoreAttribute = Properties[prop].GetCustomAttribute<CsvIgnoreAttribute>();
                if (ignoreAttribute == null)
                {
                    CsvNameAttribute nameAttribute = Properties[prop].GetCustomAttribute<CsvNameAttribute>();
                    string fieldName;
                    if (nameAttribute != null)
                    {
                        fieldName = nameAttribute.FieldName;
                    }
                    else
                    {
                        fieldName = Properties[prop].Name;
                    }

                    CsvBuilder.Append(fieldName);

                    if (IndexToStopSplit > prop)
                        CsvBuilder.Append(csvToolsConfig.SplitPattern);
                }
            }

            CsvBuilder.Append(LineBreak);

            // Data
            for (int item = 0; item < data.Count; item++)
            {
                T currentItem = data[item];
                for (int prop = 0; prop < Properties.Length; prop++)
                {
                    CsvIgnoreAttribute ignoreAttribute = Properties[prop].GetCustomAttribute<CsvIgnoreAttribute>();
                    if (ignoreAttribute == null)
                    {
                        var value = Properties[prop].GetValue(currentItem);

                        if (value == null)
                        {
                            // Do not append nulls
                        }
#warning OPTIMIZE THIS
                        else if (value.GetType() == typeof(DateTime) || value.GetType() == typeof(DateTime?))
                        {
                            CsvBuilder.Append(((DateTime)value).ToString(csvToolsConfig.TypeConversionConfig.DateTimeFormat));
                        }
                        else if (value.GetType() == typeof(TimeSpan) || value.GetType() == typeof(TimeSpan?))
                        {
                            CsvBuilder.Append(((TimeSpan)value).ToString(csvToolsConfig.TypeConversionConfig.TimeSpanFormat));
                        }
                        else
                        {
                            CsvBuilder.Append(value);
                        }

                        if (IndexToStopSplit > prop)
                            CsvBuilder.Append(csvToolsConfig.SplitPattern);
                    }
                }
                CsvBuilder.Append(LineBreak);
            }

            return CsvBuilder.ToString();
        }

        private static string GetColumnName(PropertyInfo Property)
        {
            string name = Property.Name;

            CsvNameAttribute namettribute = Property.GetCustomAttribute<CsvNameAttribute>();

            if (namettribute != null)
            {
                name = namettribute.FieldName;
            }

            return name;
        }
    }
}
