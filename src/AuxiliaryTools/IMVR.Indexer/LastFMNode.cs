using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Objects;
using IMVR.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    /// <summary>
    /// Uses last.fm to fetch covers of artists.
    /// </summary>
    public class LastFmNode : ConsumerNode<Artist>
    {
        public int RequestInterval = 1000;

        private const string API_KEY = "30be2ddd98283c1a658d365519eddb97";
        private const string SECRET = "3ebaaad507e78efd28755ee0c8e91729";
        
        private LastAuth _auth;
        private AlbumApi _albumApi;
        private ArtistApi _artistApi;

        private AtlasManager _manager = new AtlasManager();
        private AtlasTicket _noCoverTicket;
        private readonly string[] COMMON_ARTWORK_NAMES = { 
            "front.jpg",
            "cover.jpg",
            "font.png",
            "cover.png"         
        };

        public LastFmNode()
        {
            _auth = new LastAuth(API_KEY, SECRET);
            _albumApi = new AlbumApi(_auth);
            _artistApi = new ArtistApi(_auth);

            _manager.TileSize = 256;

            _noCoverTicket = _manager.GetTicket( new FileInfo("Resources/No_Cover.jpg").FullName );

            Out.Color = ConsoleColor.Magenta;
        }


        private DateTime lastRequest = DateTime.MinValue;

        private void MakeRequest(Action action)
        {
            var timeSinceLastRequest = DateTime.Now - lastRequest;
            int sleepTime = RequestInterval - timeSinceLastRequest.Milliseconds;
            if(sleepTime > 0)
                Thread.Sleep(sleepTime);

            action();
        }

        protected override void ProcessItem(Artist artist)
        {
            Out.Log("[Last.FM] Artist: {0}", artist.Name);


            //MakeRequest(delegate
            //{
            //    var artistInfo = _artistApi.GetArtistInfoAsync(artist.Name, "en", true).Result.Content;
            //    if (artistInfo != null)
            //    {
            //        artist.Biography = artistInfo.Bio.Summary;

            //        //if (artist.Image == null && artistInfo.MainImage.Largest != null)
            //        //{
            //        //    var image = artistInfo.MainImage.Largest;
            //        //    if (image != null)
            //        //    {
            //        //        artist.Image = _manager.GetTicket(image.AbsoluteUri);
            //        //    }
            //        //}
            //    }
            //});

            foreach (var album in artist.Albums)
            {
                if (album.Atlas != null) continue;


                var firstTrack = album.Tracks.First();
                var path = Path.GetDirectoryName(firstTrack.Path);
                var artwork = Directory.GetFiles(path).Select(file => Path.GetFileName(file)).Intersect(COMMON_ARTWORK_NAMES).FirstOrDefault();
                if (artwork != null)
                {
                    // Found some artwork, use that!
                    Out.Log("Found artwork in folder!");

                    album.Atlas = _manager.GetTicket(Path.Combine(path, artwork));
                }
                else
                {
                    MakeRequest(delegate
                    {
                        Out.Log("Cover: {0}", album.Name);

                        // Find cover
                        var albumInfo = _albumApi.GetAlbumInfoAsync(artist.Name, album.Name, true).Result.Content;
                        if (albumInfo != null && albumInfo.Images.Largest != null)
                        {
                            album.Atlas = _manager.GetTicket(albumInfo.Images.Largest.AbsoluteUri);
                            Out.Log("Cover found!");
                        }
                        else
                        {
                            album.Atlas = _noCoverTicket;
                            Out.Log("No cover found...");
                        }
                    });
                }
            }
           
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _manager.Save();
        }
    }
}
