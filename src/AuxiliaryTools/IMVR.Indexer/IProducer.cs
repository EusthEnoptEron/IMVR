using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public interface IProducer<T>
    {
        void Pipe(IConsumer<T> target);
        bool Done { get; }
    }
}
