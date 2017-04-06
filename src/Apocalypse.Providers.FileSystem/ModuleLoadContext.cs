using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Apocalypse.Providers.FileSystem
{
    sealed class ModuleLoadContext : AssemblyLoadContext
    {
        readonly AssemblyLoadContext apocalypseLoadContext;
        readonly string assemblyPath;

        public ModuleLoadContext(AssemblyLoadContext apocalypseLoadContext, string modulePath, AssemblyName moduleAssembly)
        {
            this.apocalypseLoadContext = apocalypseLoadContext;

            // Choose the path to load module's assemblies.
            var srcPath = Path.Combine(modulePath, "src");
            if (Directory.Exists(srcPath))
            {
                var binPath = Path.Combine(srcPath, moduleAssembly.Name, "bin");
                var latestAssembly = (
                    from filePath in Directory.GetFiles(binPath, moduleAssembly.Name + ".dll", SearchOption.AllDirectories)
                    orderby File.GetLastWriteTime(filePath) descending
                    select filePath
                ).First();

                assemblyPath = Path.GetDirectoryName(latestAssembly);
            }
            else
            {
                assemblyPath = modulePath;
            }

            Resolving += ResolveAssembly;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (ReferenceEquals(apocalypseLoadContext, Default))
            {
                // The default load context will be called automatically when returning null from this method.
                return null;
            }
            else
            {
                return apocalypseLoadContext.LoadFromAssemblyName(assemblyName);
            }
        }

        Assembly ResolveAssembly(AssemblyLoadContext context, AssemblyName name)
        {
            var path = Path.Combine(assemblyPath, name.Name + ".dll");
            return LoadFromAssemblyPath(path);
        }
    }
}
