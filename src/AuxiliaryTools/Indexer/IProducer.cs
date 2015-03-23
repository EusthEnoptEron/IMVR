using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Indexer
{
    public interface IProducer<T>
    {
        void Pipe(IConsumer<T> target);
        bool Done { get; }
    }

    public interface IConsumer<T>
    {
        void Handshake(IProducer<T> producer);
        BlockingCollection<T> Input { get; }
    }

    public abstract class DualNode<I, O> : ConsumerNode<I>, IProducer<O>
    {
        private List<IConsumer<O>> targets = new List<IConsumer<O>>();


        public IConsumer<O> Target
        {
            set
            {
                Pipe(value);
            }
        }

        protected DualNode(int threadCount = 1, int capacity = 10000)
            : base(threadCount, capacity)
        {
        }

        public void Pipe(IConsumer<O> target)
        {
            target.Handshake(this);
            targets.Add(target);
        }

        protected void Publish(O item)
        {
            foreach (var target in targets)
            {
                target.Input.Add(item);
            }
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            Done = true;
        }

        public bool Done
        {
            get;
            private set;
        }
    }

    public abstract class ConsumerNode<T> : AbstractWorker, IConsumer<T>
    {
        public BlockingCollection<T> Input { get; private set; }
        private List<IProducer<T>> producerList = new List<IProducer<T>>();

        protected ConsumerNode(int threadCount = 1, int capacity = 10000) : base(threadCount)
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


    public class MusicConsumerNode : DualNode<FileInfo, FileInfo>, IProducer<string>
    {
        private IConsumer<string> artistAnalyzer;

        public MusicConsumerNode(int threadCount) : base(threadCount) { }

        protected override void ProcessItem(FileInfo item)
        {
            // Foward music file
            Publish(item);

            // Determine artist
            // TODO: do it
            if(artistAnalyzer != null)
                artistAnalyzer.Input.Add("");
        }

        public void Pipe(IConsumer<string> target)
        {
            target.Handshake(this);
            artistAnalyzer = target;
        }
    }

    public class ArtistAnalyzerNode : DualNode<string, DbAction>
    {
        protected override void ProcessItem(string item)
        {
            // Do something

            // Publish(myAction);
        }

    }
}
