using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Core
{
    public sealed class ApocalypseInstance : IApocalypseInstance
    {
        readonly HashSet<Task> activeTasks = new HashSet<Task>();

        public ApocalypseInstanceState State
        {
            get;
            private set;
        }

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

            lock (activeTasks)
            {
                if (State == ApocalypseInstanceState.Stopping)
                {
                    throw new InvalidOperationException("Instance is being shutting down.");
                }

                if (!activeTasks.Add(task))
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

            // Create a task that listen for termination.
            var terminationWaitingTask = Task.Delay(-1, cancellationToken);
            RegisterTask(terminationWaitingTask);

            // Enter main loop.
            State = ApocalypseInstanceState.Running;

            try
            {
                Starting?.Invoke(this, EventArgs.Empty);
                await MainLoop(terminationWaitingTask);
                Stopping?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                State = ApocalypseInstanceState.Stopped;
                throw;
            }

            State = ApocalypseInstanceState.Stopping;

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

        async Task MainLoop(Task terminationWaitingTask)
        {
            for (;;)
            {
                Task[] tasksToWait;

                // Wait one of active tasks to finish.
                lock (activeTasks)
                {
                    tasksToWait = activeTasks.ToArray();
                }

                var completedTask = await Task.WhenAny(tasksToWait);

                lock (activeTasks)
                {
                    activeTasks.Remove(completedTask);
                }

                // Check termination.
                if (completedTask == terminationWaitingTask)
                {
                    break;
                }
            }
        }
    }
}
