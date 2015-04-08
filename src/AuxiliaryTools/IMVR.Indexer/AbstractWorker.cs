using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public abstract class AbstractWorker
    {
        private CancellationTokenSource cts;
        private int threadCount;
        private int remainingThreads;
        private bool _isStarted = false;
        private bool _isInitialized = false;
        private object _initializationLock = new object();
        
        private static List<Task> Tasks = new List<Task>();

        public Task Task
        {
            get;
            private set;
        }

        protected AbstractWorker()
        {
        }

        public AbstractWorker(int threadCount)
        {
            cts = new CancellationTokenSource();
            this.threadCount = threadCount;
        }

        public Task Start()
        {
            if (!IsStarted)
            {
                _isStarted = true;

                for (int i = 0; i < threadCount; i++ )
                {
                    remainingThreads++;
                    Task = Task.Factory.StartNew(DoWork, cts.Token);
                    Tasks.Add(Task);
                }
            }
            return Task;
        }

        public bool IsStarted
        {
            get
            {
                return _isStarted;
            }
        }

        private void DoWork(object obj)
        {
            try
            {
                var token = (CancellationToken)obj;

                lock (_initializationLock)
                {
                    if (!_isInitialized)
                    {
                        _isInitialized = true;
                        StartUp();
                    }
                }

                Process(token);
            }
            finally
            {
                if (--remainingThreads == 0)
                {
                    var type = this.GetType();
                    lock (type)
                    {
                        Monitor.PulseAll(type);
                    }
                    CleanUp();
                }
            }
        }


        protected virtual void StartUp()
        {
        }

        protected virtual void CleanUp()
        {
        }

        protected abstract void Process(CancellationToken token);


        public void Cancel()
        {
            cts.Cancel();
        }


        /// <summary>
        /// Waits until *all* AbstractWorkers are done with their work.
        /// </summary>
        internal static void Wait()
        {
            while (Tasks.Count > 0)
            {
                var tasks = Tasks.ToArray();
                Tasks.Clear();

                Task.WaitAll(tasks);
            }
        }


        public static Task StartNew(Action action)
        {
            var task = Task.Factory.StartNew(action);
            Tasks.Add(task);
            return task;
        }
    }
}
