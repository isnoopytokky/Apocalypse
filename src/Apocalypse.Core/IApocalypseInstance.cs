using System;
using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Core
{
    public interface IApocalypseInstance : IDisposable
    {
        ApocalypseInstanceState State { get; }
        event EventHandler Starting;
        event EventHandler Stopping;
        void RegisterTask(Task task);
        Task RunAsync(CancellationToken cancellationToken);
    }
}
