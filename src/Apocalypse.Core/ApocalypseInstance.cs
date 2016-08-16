using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Logging;

namespace Apocalypse.Core
{
    public sealed class ApocalypseInstance : IApocalypseInstance
    {
        public void Dispose()
        {
        }

        public Task RunAsync(ILogger logger, CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
