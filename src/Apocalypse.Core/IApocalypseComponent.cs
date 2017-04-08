using System;

namespace Apocalypse.Core
{
    public interface IApocalypseComponent : IDisposable
    {
        void Initialize(IApocalypseInstance apocalypse);
    }
}
