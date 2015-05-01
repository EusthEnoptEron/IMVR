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
            var startTime = DateTime.Now;
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
                System.IO.File.Delete(Options.Instance.DBPath);
                IMDB db = Options.Instance.DB;

                // -----DEBUG--------
                db.Folders.Clear();
                //db.Folders.Add(@"C:\Users\Simon\Pictures");
                //db.Folders.Add(@"C:\Users\Simon\Music");
                //db.Folders.Add(@"C:\Users\meers1\Music\Music\August Burns Red");
                db.Folders.Add(@"C:\Users\meers1\Music\Music");
                //db.Folders.Add(@"C:\Users\meers1\Music\Music\Comeback Kid");
                //db.Folders.Add(@"C:\Users\meers1\Music\Music\Cyua");
                //db.Folders.Add(@"C:\Users\meers1\Music\Music\Dantalian");
                //db.Folders.Add(@"C:\Users\meers1\Music\Music\anime");
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
                    //musicAnalyzer.Pipe(new EchoNestNode());
                }
              
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

                AbstractWorker.Wait();
                db.Save(Options.Instance.DBPath);


                var albums = db.Artists.SelectMany(artist => artist.Albums);
                Konsole.WriteLine("");
                Konsole.WriteLine("-----------------------------------------");
                Konsole.WriteLine("            DONE [{0:c}]            ", DateTime.Now - startTime);
                Konsole.WriteLine("-----------------------------------------");
                Konsole.WriteLine(" Indexed Artists       : {0}", db.Artists.Count);
                Konsole.WriteLine(" Indexed Songs         : {0}", db.Songs.Count);
                Konsole.WriteLine(" Album Cover Coverage  : {0:P}", albums.Count(album => album.Atlas != null) / (double)albums.Count());
                Konsole.WriteLine(" Echo Nest success rate: {0:P}", db.Songs.Where(song => song.Danceability != null).Count() / (double)db.Songs.Count);
                Konsole.WriteLine(" Indexed Images        : {0}", db.Images.Count);
                Konsole.WriteLine(" DB size               : {0:0.00}MB", new FileInfo(Options.Instance.DBPath).Length / 1024d / 1024d);
                Konsole.WriteLine("-----------------------------------------");


                Console.ReadLine();
            }
        }
    }
}
