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

namespace Apocalypse.Console
{
    static class Program
    {
        static IContainer CreateServiceFactory()
        {
            var container = new Container(rules => rules.WithTrackingDisposableTransients());

            try
            {
                RegisterHostServices(container);
                RegisterApocalypseCoreServices(container);
            }
            catch
            {
                container.Dispose();
                throw;
            }

            return container;
        }

        static IEnumerable<IApocalypseModule> LoadModules(IContainer services)
        {
            var provider = new ModuleProvider(AssemblyLoadContext.Default, "modules");
            return provider.LoadAllModules();
        }

        static void Main(string[] args)
        {
            using (var services = CreateServiceFactory())
            {
                var modules = LoadModules(services);

                try
                {
                    Run(services);
                }
                finally
                {
                    foreach (var module in modules)
                    {
                        module.Dispose();
                    }
                }
            }
        }

        static void RegisterApocalypseCoreServices(IContainer container)
        {
            container.Register<IApocalypseInstance, ApocalypseInstance>();
        }

        static void RegisterHostServices(IContainer container)
        {
            container.RegisterDelegate<ILogger>(resolver =>
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
            }, reuse: Reuse.Singleton);
        }

        static void Run(IContainer services)
        {
            var logger = services.Resolve<ILogger>();

            // Initialize.            
            logger.Info("Starting Apocalypse.");

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

                var instance = services.Resolve<IApocalypseInstance>();
                instance.RunAsync(terminateEvent.Token).Wait();
            }

            // Finalize.
            logger.Info("Finalizing Apocalypse.");
        }
    }
}
