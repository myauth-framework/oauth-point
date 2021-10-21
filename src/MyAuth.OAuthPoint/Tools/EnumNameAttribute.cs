using System;

#if MYAUTH_CLIENT
namespace MyAuth.OAuthPoint.Client.Tools
#else
namespace MyAuth.OAuthPoint.Tools
#endif
{
    class EnumNameAttribute : Attribute
    {
        public string Name { get; }

        public EnumNameAttribute(string name)
        {
            Name = name;
        }
    }
}
