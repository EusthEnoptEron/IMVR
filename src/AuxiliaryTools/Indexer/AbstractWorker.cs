using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Indexer
{
    public abstract class AbstractWorker
    {
        private CancellationTokenSource cts;
        private int threadCount;
        private int remainingThreads;

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
                StartUp();

                for (int i = 0; i < threadCount; i++ )
                {
                    remainingThreads++;
                    Task = Task.Factory.StartNew(DoWork, cts.Token);
                }
            }
            return Task;
        }

        public bool IsStarted
        {
            get
            {
                return Task != null;
            }
        }

        private void DoWork(object obj)
        {
            try
            {
                var token = (CancellationToken)obj;

                Process(token);
            }
            finally
            {
                if(--remainingThreads == 0)
                    CleanUp();
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


    }
}
