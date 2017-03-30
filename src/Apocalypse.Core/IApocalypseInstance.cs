using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Core
{
    public interface IApocalypseInstance : IDisposable
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
