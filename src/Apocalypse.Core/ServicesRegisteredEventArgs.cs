using System;
using Microsoft.Extensions.DependencyInjection;

namespace Apocalypse.Core
{
    public sealed class ServicesRegisteredEventArgs : EventArgs
    {
        public ServicesRegisteredEventArgs(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
