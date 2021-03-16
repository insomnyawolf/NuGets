using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace MoeBooruApi
{
    public enum MoeBooruServer
    {
        Konachan,
        Yandere
    }

    public enum MoeBooruRating
    {
        [EnumMember(Value = "null")]
        Any,
        [EnumMember(Value = "e")]
        Explicit,
        [EnumMember(Value = "q")]
        Questionabe,
        [EnumMember(Value = "s")]
        Safe
    }

    public enum MoeBooruStatus
    {
        [EnumMember(Value = "active")]
        Active
    }

    public static class EnumExtensions
    {
        public static string ToStringCustom(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            FieldInfo info = type.GetField(enumValue.ToString());

            if (info.GetCustomAttribute<EnumMemberAttribute>() is EnumMemberAttribute value)
            {
                return value.Value;
            }

            return enumValue.ToString();
        }
    }
}
