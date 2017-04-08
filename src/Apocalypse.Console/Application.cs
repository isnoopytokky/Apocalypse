using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading;
using Apocalypse.Console.Logging;
using Apocalypse.Core;
using Apocalypse.Logging;
using Apocalypse.Logging.Concurrent;
using Apocalypse.Providers.FileSystem;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Apocalypse.Console
{
    sealed class Application : IApocalypseApplication, IDisposable
    {
        readonly IContainer serviceContainer;
        bool disposed;

        public Application()
        {
            // Load Modules.
            Modules = LoadAllModules();

            try
            {
                ModulesLoaded?.Invoke(this, EventArgs.Empty);

                // Register Services.
                var services = RegisterServices();
                ServicesRegistered?.Invoke(this, new ServicesRegisteredEventArgs(services));
                serviceContainer = new Container().WithDependencyInjectionAdapter(services);
            }
            catch
            {
                foreach (var module in Modules)
                {
                    module.Dispose();
                }
                throw;
            }
        }

        ~Application()
        {
            Dispose(false);
        }

        public ILogger Logger { get; } = CreateLogger();
        public IEnumerable<IApocalypseModule> Modules { get; }

        public event EventHandler ModulesLoaded;
        public event EventHandler<ServicesRegisteredEventArgs> ServicesRegistered;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            // Initialize.            
            Logger.Info("Starting Apocalypse.", LogCategory.Apocalypse);

            // Run.
            using (var terminateEvent = new CancellationTokenSource())
            {
                System.Console.CancelKeyPress += (sender, e) =>
                {
                    if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                    {
                        if (!terminateEvent.IsCancellationRequested)
                        {
                            terminateEvent.Cancel();
                        }
                        e.Cancel = true;
                    }
                };

                RunInstance(terminateEvent.Token);
            }

            // Finalize.
            Logger.Info("Finalizing Apocalypse.", LogCategory.Apocalypse);
        }

        static ILogger CreateLogger()
        {
            var consoleLogger = new ConsoleLogger();

            try
            {
                return new SerializableLogger(consoleLogger);
            }
            catch
            {
                consoleLogger.Dispose();
                throw;
            }
        }

        void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var module in Modules)
                {
                    module.Dispose();
                }

                serviceContainer.Dispose();
                Logger.Dispose();
            }

            disposed = true;
        }

        IEnumerable<IApocalypseModule> LoadAllModules()
        {
            var provider = new ModuleProvider(AssemblyLoadContext.Default);
            var modules = provider.LoadAllModules();

            try
            {
                foreach (var module in modules)
                {
                    module.Initialize(this);
                }
            }
            catch
            {
                foreach (var module in modules)
                {
                    module.Dispose();
                }
                throw;
            }

            return modules;
        }

        IServiceCollection RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IApocalypseApplication>(this);
            services.AddTransient<IApocalypseInstance, ApocalypseInstance>();

            return services;
        }

        void RunInstance(CancellationToken cancellationToken)
        {
            var serviceProvider = serviceContainer.Resolve<IServiceProvider>();

            using (var providerScope = serviceProvider.CreateScope())
            using (var instance = providerScope.ServiceProvider.GetRequiredService<IApocalypseInstance>())
            {
                var components = providerScope.ServiceProvider.GetServices<IApocalypseComponent>();

                try
                {
                    foreach (var component in components)
                    {
                        component.Initialize(instance);
                    }

                    instance.RunAsync(cancellationToken).Wait();
                }
                finally
                {
                    foreach (var component in components)
                    {
                        component.Dispose();
                    }
                }
            }
        }
    }
}
