using System.Collections.Generic;

namespace Apocalypse.Core.Moduling
{
    public interface IModuleProvider
    {
        IEnumerable<IApocalypseModule> LoadAllModules();
    }
}
