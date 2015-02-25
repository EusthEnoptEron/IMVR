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
    public class FileWalker
    {
        private string root;
        private BlockingCollection<string> collection;
        public Predicate<FileInfo> Filter = f => { return true; };

        private CancellationTokenSource cts;

        public FileWalker(string root, BlockingCollection<string> collection)
        {
            this.root = root;
            this.collection = collection;
            cts = new CancellationTokenSource();
        }

        public Task Start()
        {
            return Task.Factory.StartNew(DoWork, cts.Token);
        }

        private void DoWork(object obj)
        {
            var token = (CancellationToken)obj;

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
                            collection.Add(file.FullName);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
