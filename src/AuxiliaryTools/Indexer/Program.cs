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
using DbLinq.Sqlite;
using EchoNest.Artist;
using EchoNest.Song;

namespace Indexer
{
    class Program
    {
        private const int COLLECTION_BOUND = 100;
        private const int IMAGE_ANALYZERS = 5;
        private const int MUSIC_ANALYZERS = 5;


        private static ArtistBucket AllBuckets
        {

            get
            {
                return ArtistBucket.Biographies | ArtistBucket.Familiarity | ArtistBucket.Terms | ArtistBucket.Hotttnesss | ArtistBucket.YearsActive | ArtistBucket.ArtistLocation;
            }
        }

        static void Main(string[] args)
        {

            //var session = new EchoNest.EchoNestSession("IIYVSIK0ZCRCMU3VS");
            //var profileResponse = session.Query<Profile>().Execute("ストロベリーソングオーケストラ", AllBuckets);
            //session.Query<
            

            args = new string[] { "-v", "-d", Path.Combine("D:/Dev/VirtualHands/src/Application/Assets", "Database.s3db") };
            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                var imageCollection = new BlockingCollection<string>(COLLECTION_BOUND);
                //var musicCollection = new BlockingCollection<string>(COLLECTION_BOUND);
                var dbCollection = new BlockingCollection<DbAction>(1000);

                var producers = new List<Task>();
                var consumers = new List<Task>();
                var dbWorker = new DbSyncer(dbCollection);

                using (var db = Database.Context)
                {
                    new SqliteCommand("VACUUM", (SqliteConnection)db.Connection).ExecuteNonQuery();
                    
                    // Create producers
                    foreach (var library in db.MediaLibraries)
                    {
                        producers.Add(new FileWalker(library.Path, imageCollection)
                        {
                            Filter = IO.IsImage
                        }.Start());
                    }

                    // Create consumers
                    for (int i = 0; i < IMAGE_ANALYZERS; i++)
                        consumers.Add(new ImageAnalyzer(imageCollection, dbCollection).Start());

                   // CleanDb();

                    // Start work
                    //foreach (var producer in producers) producer.Start();
                    //foreach (var consumer in consumers) consumer.Start();

                    db.Connection.Close();
                }

                dbWorker.Start();

                Task.WaitAll(producers.ToArray());
                imageCollection.CompleteAdding();
                Task.WaitAll(consumers.ToArray());
                dbCollection.CompleteAdding();
                dbWorker.Task.Wait();


                //// Wait for break signal
                //while (true)
                //{
             
                //    Thread.Sleep(100);
                //}
               
            }
            
          
        }

        private static async void CleanDb()
        {
            await Task.Run(() =>
            {
                using (var connection = Database.Connection)
                {
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
