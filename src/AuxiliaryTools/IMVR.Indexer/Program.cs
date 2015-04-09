using System;
using System.Linq;
using System.IO;
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

        private const string accessKeyId = "AKIAI6KMD7GWHAFA46ZA";
        private const string secretKey = "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";

        
        static void Main(string[] args)
        {
            //var client = new GracenoteClient("13232384-4CA925FFEF026C96B030F81372DB39CA");
            //var albumSearcher = new AlbumSearcher(client);
            //var searchResult  = albumSearcher.Search(new SearchCriteria
            //{
            //    AlbumTitle = "TVアニメ「ダンタリアンの書架」オリジナル・サウンド トラック組曲「ダンタリアンの書架」 Disc 1",
            //    Artist = "辻陽",
            //    SearchMode = SearchMode.BestMatchWithCoverArt  
            //});

            //Console.WriteLine(searchResult.Count);
            args = new string[] { "-v", "-d", Path.Combine("D:/Dev/IMVR/src/Application/Assets", "Database.bin") };

            if (CommandLine.Parser.Default.ParseArguments(args, Options.Instance))
            {
                IMDB db = Options.Instance.DB;

                // -----DEBUG--------
                db.Folders.Clear();
                //db.Folders.Add(@"C:\Users\Simon\Pictures");
                //db.Folders.Add(@"C:\Users\Simon\Music");
                db.Folders.Add(@"C:\Users\meers1\Music\NoisyCell");
                db.Folders.Add(@"C:\Users\meers1\Music\Music\anime");
                //db.Folders.Add(@"C:\Users\meers1\Pictures");
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

                var musicAnalyzer = new MusicIndexer();
                {
                    musicAnalyzer.Pipe(new LastFmNode());
                    musicAnalyzer.Pipe(new EchoNestNode());
                }
              

                // Create producers
                foreach (var library in db.Folders.Distinct())
                {
                    new FileWalker(library)
                    {
                        Filter = IO.IsImage,
                        Target = imageAnalyzer
                    }.Start();

                    new FileWalker(library)
                    {
                        Filter = IO.IsMusic,
                        Target = musicAnalyzer
                    }.Start();
                }

                // Needed when there is no file walker to initialize the image analyzer
                imageAnalyzer.Start();
                musicAnalyzer.Start();

                AbstractWorker.Wait();

                Console.ReadLine();
            }
        }
    }
}
