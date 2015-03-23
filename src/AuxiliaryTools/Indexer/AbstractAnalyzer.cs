using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public abstract class AbstractConsumer<T> : AbstractWorker
    {
        private BlockingCollection<T> collection;

        protected AbstractConsumer(BlockingCollection<T> collection) : base()
        {
            this.collection = collection;
        }

        protected override void Process(CancellationToken token)
        {
            while (!collection.IsCompleted && !token.IsCancellationRequested)
            {
                //Console.WriteLine(collection.IsCompleted + " " + collection.IsAddingCompleted + " "  + collection.Count);
                T item;
                if (collection.TryTake(out item, 100))
                {
                    ProcessItem(item);
                }
            }

            CleanUp();
        }

        protected override void CleanUp()
        {
        }

        protected abstract void ProcessItem(T item);
    }
}
