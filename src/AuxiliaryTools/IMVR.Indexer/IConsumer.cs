using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public interface IConsumer<T>
    {
        void Handshake(IProducer<T> producer);
        BlockingCollection<T> Input { get; }
    }

}
