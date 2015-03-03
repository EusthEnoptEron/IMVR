﻿using System;
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
                var collection = new BlockingCollection<string>(COLLECTION_BOUND);
                var producers = new List<Task>();
                var consumers = new List<Task>();
                using (var db = Database.Context)
                {
                    new SqliteCommand("VACUUM", (SqliteConnection)db.Connection).ExecuteNonQuery();
                    
                    // Create producers
                    foreach (var library in db.MediaLibraries)
                    {
                        producers.Add(new FileWalker(library.Path, collection)
                        {
                            Filter = IO.IsImage
                        }.Start());
                    }

                    // Create consumers
                    consumers.Add(new FileAnalyzer(collection).Start());
                    //consumers.Add(new FileAnalyzer(collection).Start());
                    //consumers.Add(new FileAnalyzer(collection).Start());
                    //consumers.Add(new FileAnalyzer(collection).Start());
                    //consumers.Add(new FileAnalyzer(collection).Start());

                   // CleanDb();

                    // Start work
                    //foreach (var producer in producers) producer.Start();
                    //foreach (var consumer in consumers) consumer.Start();

                    db.Connection.Close();
                }


                Task.WaitAll(producers.ToArray());
                collection.CompleteAdding();
                Task.WaitAll(consumers.ToArray());

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
