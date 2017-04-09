using System;
using System.Reflection;

namespace Apocalypse.Utility.Attribute
{
    public static class EnumExtensions
    {
        public static T GetCustomAttribute<T>(this Enum value) where T : System.Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);

            return type.GetField(name).GetCustomAttribute<T>();
        }
    }
}
