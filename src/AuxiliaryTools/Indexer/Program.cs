using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using ImageMagick;
using System.Data;
using VirtualHands.Data;
using System.IO;
using DbLinq;
using Mono.Data.Sqlite;
using System.Collections.Concurrent;
using System.Threading;

namespace Indexer
{
    class Program
    {
        private const int COLLECTION_BOUND = 100;

        static void Main(string[] args)
        {
            args = new string[] { "-v", "-d", Path.Combine("E:/Dev/VirtualHands/src/Application/Assets", "Database.s3db") };
            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                var collection = new BlockingCollection<string>(COLLECTION_BOUND);

                using (var db = Database.Context)
                {
                    var producers = new List<FileWalker>();
                    var consumers = new List<FileAnalyzer>();

                    // Create producers
                    foreach (var library in db.MediaLibraries)
                    {
                        producers.Add(new FileWalker(library.Path, collection)
                        {
                            Filter = IO.IsImage
                        });
                    }

                    // Create consumers
                    consumers.Add(new FileAnalyzer(collection));
                    consumers.Add(new FileAnalyzer(collection));
                    consumers.Add(new FileAnalyzer(collection));
                    consumers.Add(new FileAnalyzer(collection));
                    consumers.Add(new FileAnalyzer(collection));


                    // Start work
                    foreach (var producer in producers) producer.Start();
                    foreach (var consumer in consumers) consumer.Start();

                    db.Connection.Close();
                }
            }

            // Wait for break signal
            while (true)
            {
                Thread.Sleep(100);
            }
               
        }
    }
}
