using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public abstract class ConsumerNode<T> : AbstractWorker, IConsumer<T>
    {
        public BlockingCollection<T> Input { get; private set; }
        private List<IProducer<T>> producerList = new List<IProducer<T>>();

        protected ConsumerNode(int threadCount = 1, int capacity = 10000)
            : base(threadCount)
        {
            Input = new BlockingCollection<T>(capacity);
        }

        public void Handshake(IProducer<T> producer)
        {
            producerList.Add(producer);

            if (!IsStarted) Start();
        }

        private bool ProducersDone
        {
            get
            {
                return producerList.All(p => p.Done);
            }
        }

        protected override void Process(CancellationToken token)
        {
            while (!Input.IsCompleted && !token.IsCancellationRequested && !ProducersDone)
            {
                //Console.WriteLine(collection.IsCompleted + " " + collection.IsAddingCompleted + " "  + collection.Count);
                T item;
                if (Input.TryTake(out item, 100))
                {
                    ProcessItem(item);
                }
            }
        }

        protected override void CleanUp()
        {
        }

        protected abstract void ProcessItem(T item);
    }
}
