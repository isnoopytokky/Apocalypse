using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Core
{
    public sealed class ApocalypseInstance : IApocalypseInstance
    {
        public void Dispose()
        {
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(0);
        }
    }
}
