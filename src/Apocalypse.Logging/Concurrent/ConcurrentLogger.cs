using System;
using System.Collections.Concurrent;
using System.Threading;
using Apocalypse.Logging.Concurrent.LoggingOperations;

namespace Apocalypse.Logging.Concurrent
{
    public sealed class ConcurrentLogger : ILogger
    {
        readonly ILogger backed;
        readonly Thread operationsExecutor;
        readonly ReaderWriterLockSlim disposeLocking;
        readonly BlockingCollection<LoggingOperation> operations;
        bool disposing;

        public ConcurrentLogger(ILogger backed)
        {
            this.backed = backed;

            // Setup Operation Executor.
            operationsExecutor = new Thread(OperationsExecutor);
            disposeLocking = new ReaderWriterLockSlim();

            try
            {
                operations = new BlockingCollection<LoggingOperation>();
            }
            catch
            {
                disposeLocking.Dispose();
                throw;
            }

            // Start Operation Executor.
            try
            {
                operationsExecutor.Start();
            }
            catch
            {
                operations.Dispose();
                disposeLocking.Dispose();
                throw;
            }
        }
        
        public void Dispose()
        {
            // Enter Shutdown State.
            disposeLocking.EnterWriteLock();

            try
            {
                disposing = true;
            }
            finally
            {
                disposeLocking.ExitWriteLock();
            }

            // Shutdown Operations Executor.
            var shutdownOperation = new ShutdownOperation();
            operations.Add(shutdownOperation);

            operationsExecutor.Join();

            // Clean up.
            operations.Dispose();
            disposeLocking.Dispose();
            backed.Dispose();
        }

        public void Error(string message, LogCategory category)
        {
            var operation = new LogWritingOperation(LogType.Error, message, category);
            QueueOperation(operation);
        }

        public void Info(string message, LogCategory category)
        {
            var operation = new LogWritingOperation(LogType.Info, message, category);
            QueueOperation(operation);
        }

        void OperationsExecutor()
        {
            for (;;)
            {
                var operation = operations.Take();
                if (!operation.Execute(backed))
                {
                    break;
                }
            }
        }

        void QueueOperation(LoggingOperation operation)
        {
            disposeLocking.EnterReadLock();

            try
            {
                if (disposing)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                operations.Add(operation);
            }
            finally
            {
                disposeLocking.ExitReadLock();
            }
        }
    }
}
