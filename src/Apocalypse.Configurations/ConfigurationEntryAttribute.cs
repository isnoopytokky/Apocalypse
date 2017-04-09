using System;
using System.Collections;

namespace Apocalypse.Configurations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ConfigurationEntryAttribute : Attribute
    {
        public IEnumerable Choices { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
    }
}
