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
using IMVR.Commons;

namespace IMVR.Indexer
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
            

            args = new string[] { "-v", "-d", Path.Combine("E:/Dev/VirtualHands/src/Application/Assets", "Database.bin") };

            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                var db = IMDB.FromFile(Options.Instance.DbPath);


                // -----DEBUG--------
                db.Folders.Clear();
                db.Folders.Add(@"C:\Users\Simon\Pictures");
                // -----/DEBUG--------

                // Clean db
                db.Music.Clear();
                db.Images.Clear();


                // Prepare workers
                var dbWorker = new PersistenceWorker(db);
                var imageAnalyzer =
                    new ImageAnalyzer(IMAGE_ANALYZERS)
                    {
                        Target = dbWorker
                    };

                // Create producers
                foreach (var library in db.Folders.Distinct())
                {
                    var walker = new FileWalker(library)
                    {
                        Filter = IO.IsImage,
                        Target = imageAnalyzer
                    };

                    walker.Start();
                }

                imageAnalyzer.Start();
                dbWorker.Task.Wait();
                
                Console.ReadLine();
            }
        }
    }
}
