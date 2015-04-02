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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DbLinq.Sqlite;
using EchoNest.Artist;
using EchoNest.Song;
using IMVR.Commons;

namespace IMVR.Indexer
{
    /// <summary>
    /// Entry point of this application.
    /// </summary>
    class Program
    {
        private const int COLLECTION_BOUND = 100;
        private const int IMAGE_ANALYZERS = 5;
        private const int MUSIC_ANALYZERS = 5;


        static void Main(string[] args)
        {

            //var session = new EchoNest.EchoNestSession("IIYVSIK0ZCRCMU3VS");
            //var profileResponse = session.Query<Profile>().Execute("ストロベリーソングオーケストラ", AllBuckets);
            //session.Query<
            

            args = new string[] { "-v", "-d", Path.Combine("E:/Dev/VirtualHands/src/Application/Assets", "Database.bin") };

            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                IMDB db = IMDB.FromFile(Options.Instance.DbPath);

                // -----DEBUG--------
                db.Folders.Clear();
                db.Folders.Add(@"C:\Users\Simon\Pictures");
                db.Folders.Add(@"C:\Users\Simon\Music");
                // -----/DEBUG--------

                // Clean db
                db.Songs.Clear();
                db.Images.Clear();
                db.Artists.Clear();
                db.Atlases.Clear();


                // Prepare workers
                var dbWorker = new PersistenceNode(db);
                var imageAnalyzer =
                    new ImageAnalysisNode(IMAGE_ANALYZERS)
                    {
                        Target = dbWorker
                    };

                var musicAnalyzer = new MusicNode();
              

                // Create producers
                foreach (var library in db.Folders.Distinct())
                {
                    //new FileWalker(library)
                    //{
                    //    Filter = IO.IsImage,
                    //    Target = imageAnalyzer
                    //}.Start();

                    new FileWalker(library)
                    {
                        Filter = IO.IsMusic,
                        Target = musicAnalyzer
                    }.Start();

                }

                // Needed when there is no file walker to initialize the image analyzer
                imageAnalyzer.Start();
                musicAnalyzer.Start();

                //dbWorker.Task.Wait();
                Task.WaitAll(AbstractWorker.Tasks.ToArray());
                
                Console.ReadLine();
            }
        }
    }
}
