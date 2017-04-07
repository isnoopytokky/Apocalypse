using System;
using System.Collections.Generic;
using Apocalypse.Logging;

namespace Apocalypse.Core
{
    public interface IApocalypseApplication
    {
        ILogger Logger { get; }
        IEnumerable<IApocalypseModule> Modules { get; }

        event EventHandler ModulesLoaded;
        event EventHandler<ServicesRegisteredEventArgs> ServicesRegistered;

        void Run();
    }
}
