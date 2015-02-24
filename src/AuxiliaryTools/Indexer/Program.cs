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
using System.Threading.Tasks;

namespace Indexer
{
    class Program
    {
        private const int COLLECTION_BOUND = 100;

        static void Main(string[] args)
        {
            args = new string[] { "-v", "-d", Path.Combine("D:/Dev/VirtualHands/src/Application/Assets", "Database.s3db") };
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

                    CleanDb();

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

        private static async void CleanDb()
        {
            await Task.Run(() =>
            {
                using (var connection = Database.Connection)
                {
                    new SqliteCommand("VACUUM", connection).ExecuteNonQuery();

                    while (true)
                    {

                        using (var ctx = new Main(connection))
                        {
                            foreach (var file in ctx.Files)
                            {
                                if (!System.IO.File.Exists(file.Path))
                                {
                                    ctx.Files.DeleteOnSubmit(file);
                                }
                            }

                            ctx.SubmitChanges();
                        }

                        break;
                        //Thread.Sleep(10000);
                    }

                }
            });
        }
    }
}
