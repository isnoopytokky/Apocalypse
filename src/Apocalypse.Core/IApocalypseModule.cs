using System;

namespace Apocalypse.Core
{
    public interface IApocalypseModule : IDisposable
    {
        void Initialize(IApocalypseApplication app);
    }
}
