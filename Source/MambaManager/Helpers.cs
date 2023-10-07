using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MambaManager;

public static class Helpers
{
    public static void AddArg(this ProcessStartInfo info, string arg)
    {
        info.ArgumentList.Add(arg);
    }
}
