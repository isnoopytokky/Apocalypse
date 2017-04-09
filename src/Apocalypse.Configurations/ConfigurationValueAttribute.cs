using System;

namespace Apocalypse.Configurations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ConfigurationValueAttribute : Attribute
    {
    }
}
