using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorksheetToObjects
{
    public abstract class WorksheetReadError
    {
        public abstract string ToString();
    }

    public class WorksheetReadErrorGeneric : WorksheetReadError
    {
        public string Message { get; set; }

        public override string ToString() => Message;
    }

    public class WorksheetReadErrorMissingColumns : WorksheetReadError
    {
        public List<string> MissingColumns { get; set; }

        public WorksheetReadErrorMissingColumns()
        {
            MissingColumns = new List<string>();
        }

        public void AddMissingColumn(string name)
        {
            MissingColumns.Add(name);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("The following columns could not be found:\n");
            foreach (var column in MissingColumns)
            {
                sb.Append('\t').Append(column).Append('\n');
            }

            return sb.ToString();
        }
    }
}
