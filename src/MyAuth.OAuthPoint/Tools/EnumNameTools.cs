using System;
using System.Linq;
using System.Reflection;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Tools
#else
namespace MyAuth.OAuthPoint.Tools
#endif
{
    class EnumNameTools
    {
        public static string GetName(Enum item)
        {
            var type = item.GetType();
            var memberInfo = type.GetMember(item.ToString());
            var enumNameAttr = memberInfo.First().GetCustomAttribute<EnumNameAttribute>();

            return enumNameAttr != null
                ? enumNameAttr.Name
                : item.ToString();
        }

        public static Enum GetValue(Type enumType, string strValue)
        {
            var found = enumType.GetFields()
                .Select(m => new {Member = m, Attribute = m.GetCustomAttribute<EnumNameAttribute>()})
                .FirstOrDefault(m => m.Attribute != null && m.Attribute.Name == strValue);

            if (found != null)
                return (Enum)found.Member.GetValue(null);

            if(strValue != null)
                return (Enum) Enum.Parse(enumType, strValue);

            return (Enum)Enum.ToObject(enumType, 0);
        }
    }
}