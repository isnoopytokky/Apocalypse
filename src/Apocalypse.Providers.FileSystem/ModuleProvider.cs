using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Apocalypse.Core;
using Apocalypse.Core.Moduling;
using Newtonsoft.Json;

namespace Apocalypse.Providers.FileSystem
{
    public sealed class ModuleProvider : IModuleProvider
    {
        public const string DefaultModulesDirectory = "modules";

        readonly AssemblyLoadContext apocalypseLoadContext;
        readonly string path;

        public ModuleProvider(AssemblyLoadContext apocalypseLoadContext, string path = DefaultModulesDirectory)
        {
            if (apocalypseLoadContext == null)
            {
                throw new ArgumentNullException(nameof(apocalypseLoadContext));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException("The value is not a valid directory.", nameof(path));
            }

            this.apocalypseLoadContext = apocalypseLoadContext;
            this.path = path;
        }

        public IEnumerable<IApocalypseModule> LoadAllModules()
        {
            return Directory.EnumerateDirectories(path).Select(modulePath => LoadModule(modulePath)).ToArray();
        }

        IApocalypseModule LoadModule(string modulePath)
        {
            var modulePropertiesFile = Path.Combine(modulePath, "module.json");
            var moduleProperties = JsonConvert.DeserializeObject<ModuleProperties>(File.ReadAllText(modulePropertiesFile));

            var moduleAssemblyName = new AssemblyName(moduleProperties.AssemblyName);
            var assemblyLoadContext = new ModuleLoadContext(apocalypseLoadContext, modulePath, moduleAssemblyName);
            var moduleAssembly = assemblyLoadContext.LoadFromAssemblyName(moduleAssemblyName);

            var moduleType = moduleAssembly.GetTypes().Single(typeof(IApocalypseModule).IsAssignableFrom);
            return (IApocalypseModule)Activator.CreateInstance(moduleType);
        }
    }
}
