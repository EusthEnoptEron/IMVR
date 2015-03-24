using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
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
}
