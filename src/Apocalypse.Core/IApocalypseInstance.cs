using System;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Logging;

namespace Apocalypse.Core
{
    public interface IApocalypseInstance : IDisposable
    {
        Task RunAsync(ILogger logger, CancellationToken cancellationToken);
    }
}
