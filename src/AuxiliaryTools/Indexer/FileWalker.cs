using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualHands.Data;

namespace Indexer
{
    public class FileWalker : AbstractWorker, IProducer<FileInfo>
    {
        private string root;
        public Predicate<FileInfo> Filter = f => { return true; };

        private IConsumer<FileInfo> pathConsumer;


        public FileWalker(string root) : base(1)
        {
            this.root = root;
        }

        public IConsumer<FileInfo> Target
        {
            get
            {
                return pathConsumer;
            }
            set
            {
                Pipe(value);
            }
        }

        protected override void Process(CancellationToken token)
        {
            using (var db = Database.Context)
            {
                foreach (var file in IO.GetFiles(root))
                {
                    if (Task.Factory.CancellationToken.IsCancellationRequested)
                        break;

                    if (Filter(file))
                    {
                        if (db.Files.Where(f => f.Path == file.FullName).Count() == 0)
                        {
                            if (Options.Instance.Verbose)
                                Console.WriteLine("Add: {0}", file.FullName);

                            if (pathConsumer != null)
                                pathConsumer.Input.Add(file);
                        }
                    }
                }
            }
        }


        protected override void StartUp()
        {
            base.StartUp();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            Done = true;
        }

        public void Pipe(IConsumer<FileInfo> target)
        {
            if (pathConsumer != null) throw new Exception("Target already set!");

            target.Handshake(this);
            pathConsumer = target;
        }

        public bool Done
        {
            get;
            private set;
        }


        public T2 Pipe<T2>(T2 target) where T2 : IConsumer<FileInfo>
        {
            if (pathConsumer != null) throw new Exception("Target already set!");

            target.Handshake(this);
            pathConsumer = target;

            return target;
        }
    }
}
