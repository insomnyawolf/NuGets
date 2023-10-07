using System;
using System.Collections.Generic;
using System.Text;

namespace BasicIrcClient
{
    public static class LogHelpers
    {
        public static string ToJsonString(this object obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }
    }
}
