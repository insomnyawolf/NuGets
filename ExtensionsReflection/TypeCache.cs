using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsReflection
{
    public static partial class ExtensionsReflection
    {
        internal static readonly Type TypeInt = typeof(int);
        internal static readonly Type TypeFloat = typeof(float);
        internal static readonly Type TypeDouble = typeof(double);
        internal static readonly Type TypeDecimal = typeof(decimal);
        internal static readonly Type TypeBool = typeof(bool);
        internal static readonly Type TypeString = typeof(string);
        internal static readonly Type TypeDateTime = typeof(DateTime);
        internal static readonly Type TypeTimeSpan = typeof(TimeSpan);
        internal static readonly Type TypeNullable = typeof(Nullable<>);
    }
}
