using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Core.Settings
{
    public interface ISettingsLoader
    {
        Task<T> LoadSettingsAsync<T>(CancellationToken cancellationToken);
    }
}
