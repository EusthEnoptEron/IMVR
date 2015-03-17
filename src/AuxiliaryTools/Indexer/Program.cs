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
            

            args = new string[] { "-v", "-d", Path.Combine("E:/Dev/VirtualHands/src/Application/Assets", "Database.s3db") };
            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                var dbWorker = new DbSyncer();
                var imageAnalyzer = new ImageAnalyzer(IMAGE_ANALYZERS);
                imageAnalyzer.Pipe(dbWorker);

                using (var db = Database.Context)
                {
                    new SqliteCommand("VACUUM", (SqliteConnection)db.Connection).ExecuteNonQuery();
                    

                    // Create producers
                    foreach (var library in db.MediaLibraries)
                    {
                        var walker = new FileWalker(library.Path)
                        {
                            Filter = IO.IsImage
                        };

                        walker.Pipe(imageAnalyzer);
                        walker.Start();
                    }

                   // CleanDb();

                    // Start work
                    
                    db.Connection.Close();
                }

                imageAnalyzer.Start();

                dbWorker.Start();
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
