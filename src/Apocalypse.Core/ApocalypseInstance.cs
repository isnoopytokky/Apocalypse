using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Logging;

namespace Apocalypse.Core
{
    public sealed class ApocalypseInstance : IApocalypseInstance
    {
        readonly HashSet<Task> activeTasks = new HashSet<Task>();
        readonly IApocalypseApplication app;

        public ApocalypseInstance(IApocalypseApplication app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            this.app = app;
        }

        public ApocalypseInstanceState State { get; private set; }

        public event EventHandler Starting;
        public event EventHandler Stopping;

        public void Dispose()
        {
        }

        public void RegisterTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            var finalizationProps = new TaskFinalizationProperties();

            lock (activeTasks)
            {
                if (State == ApocalypseInstanceState.Stopping)
                {
                    throw new InvalidOperationException("Instance is being shutting down.");
                }

                finalizationProps.Finalizer = task.ContinueWith(t => 
                {
                    lock (activeTasks)
                    {
                        activeTasks.Remove(finalizationProps.Finalizer);
                    }

                    if (t.IsFaulted)
                    {
                        app.Logger.Error(t.Exception.ToString(), LogCategory.Apocalypse);
                    }
                }, TaskContinuationOptions.RunContinuationsAsynchronously);

                if (!activeTasks.Add(finalizationProps.Finalizer))
                {
                    throw new ArgumentException("The task is already exists.", nameof(task));
                }
            }
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (State != ApocalypseInstanceState.Stopped)
            {
                throw new InvalidOperationException("The instance is not stopped.");
            }

            activeTasks.Clear();

            // Enter main loop.
            State = ApocalypseInstanceState.Running;

            try
            {
                Starting?.Invoke(this, EventArgs.Empty);
                await Task.Delay(-1, cancellationToken);

                State = ApocalypseInstanceState.Stopping;
                Stopping?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                State = ApocalypseInstanceState.Stopped;
                throw;
            }

            // Wait all tasks to finish.
            try
            {
                Task[] tasksToWait;

                lock (activeTasks)
                {
                    tasksToWait = activeTasks.ToArray();
                }

                await Task.WhenAll(tasksToWait);
            }
            finally
            {
                State = ApocalypseInstanceState.Stopped;
            }
        }

        sealed class TaskFinalizationProperties
        {
            public Task Finalizer { get; set; }
        }
    }
}
