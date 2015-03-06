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
        public Task Task
        {
            get;
            private set;
        }

        protected AbstractWorker()
        {
            cts = new CancellationTokenSource();
        }

        public Task Start()
        {
            if(Task == null)
                Task = Task.Factory.StartNew(DoWork, cts.Token);
            return Task;
        }

        private void DoWork(object obj)
        {
            var token = (CancellationToken)obj;

            Process(token);
        }

        protected abstract void Process(CancellationToken token);


        public void Cancel()
        {
            cts.Cancel();
        }


    }
}
