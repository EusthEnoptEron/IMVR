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


        public LastFmNode()
        {
            _auth = new LastAuth(API_KEY, SECRET);
            _albumApi = new AlbumApi(_auth);
            _artistApi = new ArtistApi(_auth);

            _manager.TileSize = 256;

            _noCoverTicket = _manager.GetTicket( new FileInfo("Resources/No_Cover.jpg").FullName );
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
            Konsole.Log("[Last.FM] Artist: {0}", ConsoleColor.Magenta, artist.Name);

            MakeRequest(delegate
            {
                var artistInfo = _artistApi.GetArtistInfoAsync(artist.Name, "en", true).Result.Content;
                if (artistInfo != null)
                {
                    artist.Biography = artistInfo.Bio.Summary;

                    if (artist.Image == null && artistInfo.MainImage.Largest != null)
                    {
                        var image = artistInfo.MainImage.Largest;
                        if (image != null)
                        {
                            artist.Image = _manager.GetTicket(image.AbsoluteUri);
                        }
                    }
                }
            });

            MakeRequest(delegate
            {
                foreach (var album in artist.Albums)
                {
                    if (album.Atlas == null)
                    {
                        Konsole.Log("[Last.FM] Cover: {0}", ConsoleColor.Magenta, album.Name);

                        // Find cover
                        var albumInfo = _albumApi.GetAlbumInfoAsync(artist.Name, album.Name, true).Result.Content;
                        if (albumInfo != null && albumInfo.Images.Largest != null)
                        {
                            album.Atlas = _manager.GetTicket(albumInfo.Images.Largest.AbsoluteUri);
                            Konsole.Log("[Last.FM] Cover found!", ConsoleColor.Magenta);
                        }
                        else
                        {
                            album.Atlas = _noCoverTicket;
                            Konsole.Log("[Last.FM] No cover found...", ConsoleColor.Magenta);
                        }
                    }
                }
            });
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _manager.Save();
        }
    }
}
