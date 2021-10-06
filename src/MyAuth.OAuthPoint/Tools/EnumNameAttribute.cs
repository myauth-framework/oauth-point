using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace MyAuth.OAuthPoint.Tools
{
    public class EnumNameAttribute : Attribute
    {
        public string Name { get; }

        public EnumNameAttribute(string name)
        {
            Name = name;
        }

        public static string GetName(Enum item)
        {
            var type = item.GetType();
            var memberInfo = type.GetMember(item.ToString());
            var enumNameAttr = memberInfo.First().GetCustomAttribute<EnumNameAttribute>();

            return enumNameAttr != null
                ? enumNameAttr.Name
                : item.ToString();
        }
    }
}
