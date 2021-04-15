using ExcelDataReader;
using ExtensionsReflection;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace WorksheetToObjects
{
    public class WorkSheetReader
    {
        private IExcelDataReader ExcelDataReader;
        private DataSet ExcelDataSet;
        public WorkSheetReader(string path)
        {
            string extension = Path.GetExtension(path).Replace(".", string.Empty);

            WorkSheetType type;

            if (!Enum.TryParse(extension, true, out type))
            {
                throw new InvalidDataException("Invalid Excel File");
            }

            // ExcelDataReader works with the binary Excel file, so it needs a FileStream
            // This is how we avoid dependencies on ACE or Interop:
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);

            Initialize(stream, type);
        }

        public WorkSheetReader(Stream stream, WorkSheetType type)
        {
            Initialize(stream, type);
        }

        private void Initialize(Stream stream, WorkSheetType type)
        {
            switch (type)
            {
                case WorkSheetType.Csv:
                    ExcelDataReader = ExcelReaderFactory.CreateCsvReader(stream);
                    break;
                case WorkSheetType.Xls:
                    ExcelDataReader = ExcelReaderFactory.CreateBinaryReader(stream);
                    break;
                case WorkSheetType.Xlsx:
                case WorkSheetType.xlsb:
                    ExcelDataReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    break;
            }

            ExcelDataSet = ExcelDataReader.AsDataSet();
        }

        public IEnumerable<string> GetWorksheetNames()
        {
            var sheets = from DataTable sheet in ExcelDataReader.AsDataSet().Tables select sheet.TableName;
            return sheets;
        }

        public IList<DataRow> GetSheet(string sheet)
        {
            var workSheet = ExcelDataSet.Tables[sheet];
            var rows = from DataRow a in workSheet.Rows select a;
            return rows.ToList();
        }

        // rowReader.Depth
        public WorksheetReadError ParseSheet<T>(out List<T> data, int headerRowPosition = 0)
        {
            data = new List<T>();

            Type currentType = typeof(T);
            PropertyInfo[] properties = currentType.GetProperties();

            string sheetName = currentType.GetCustomAttribute<WorkSheetNameAttribute>()?.SheetName ?? currentType.Name;

            IList<DataRow> sheet;
            try
            {
                sheet = GetSheet(sheetName).ToList();
            }
            catch (NullReferenceException)
            {
                return new WorksheetReadErrorGeneric()
                {
                    Message = $"No se encontro la hoja '{sheetName}' en el documento seleccinado."
                };
            }

            var headers = sheet[headerRowPosition].ItemArray;


            WorksheetReadErrorMissingColumns error = new WorksheetReadErrorMissingColumns();
            foreach (var pinfo in properties)
            {
                if (pinfo.GetCustomAttribute<WorkSheetNonRequieredAttribute>() == null && pinfo.GetCustomAttribute<WorkSheetNonMappedFieldsAttribute>() == null)
                {
                    var name = pinfo.GetCustomAttribute<WorkSheetColumnNameAttribute>()?.ColumnName ?? pinfo.Name;
                    if (!headers.Contains(name))
                    {
                        error.AddMissingColumn(name);
                    }
                }
            }

            if (error.MissingColumns.Count > 0)
            {
                return error;
            }

            for (int index = headerRowPosition + 1; index < sheet.Count; index++)
            {
                var row = sheet[index].ItemArray;
                data.Add(DeserializeObj<T>(currentType, properties, headers, row));
            }

            return null;
        }

        private static T DeserializeObj<T>(Type currentType, PropertyInfo[] Properties, object[] header, object[] row)
        {
            var obj = FormatterServices.GetSafeUninitializedObject(currentType);
            MemberInfo[] members = FormatterServices.GetSerializableMembers(currentType);
            object[] data = new object[members.Length];

            List<int> mappedColumns = new List<int>();
            PropertyInfo NonMappedFields = null;

            for (int index = 0; index < Properties.Length; index++)
            {
                if (Properties[index].GetCustomAttribute<WorkSheetNonMappedFieldsAttribute>() != null)
                {
                    NonMappedFields = Properties[index];
                    continue;
                }
                string columnName = Properties[index].GetCustomAttribute<WorkSheetColumnNameAttribute>()?.ColumnName ?? Properties[index].Name;

                for (int i = 0; i < header.Length; i++)
                {
                    if (columnName == header[i].ToString())
                    {
                        Properties[index].UpdateSingleProp(obj, row[i].ToString());
                        mappedColumns.Add(i);
                        break;
                    }
                }

            }

            if (NonMappedFields != null)
            {
                List<string> values = new List<string>();
                for (int field = 0; field < row.Length; field++)
                {
                    if (!mappedColumns.Contains(field))
                    {
                        values.Add(row[field].ToString());
                    }
                }
                NonMappedFields.UpdateSingleProp(obj, values);
            }

            return (T)FormatterServices.PopulateObjectMembers(obj, members, data);
        }
    }

    public enum WorkSheetType
    {
        Csv,
        Xls,
        Xlsx,
        xlsb
    }
}
