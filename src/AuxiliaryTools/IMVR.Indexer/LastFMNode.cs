using IF.Lastfm.Core.Api;
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
        private LastAuth _auth;
        private AlbumApi _albumApi;
        private AtlasManager _manager = new AtlasManager();
        private AtlasTicket _noCoverTicket;

        private const string API_KEY = "30be2ddd98283c1a658d365519eddb97";
        private const string SECRET = "3ebaaad507e78efd28755ee0c8e91729";
        

        public LastFmNode()
        {
            _auth = new LastAuth(API_KEY, SECRET);
            _albumApi = new AlbumApi(_auth);

            _manager.TileSize = 256;

            _noCoverTicket = _manager.GetTicket( new FileInfo("Resources/No_Cover.jpg").FullName );
        }

        protected override void ProcessItem(Artist artist)
        {
            Konsole.Log("[Last.FM] Artist: {0}", ConsoleColor.Magenta, artist.Name);
            //return;

            DateTime startTime = DateTime.Now;

            foreach (var album in artist.Albums)
            {
                if (album.Atlas == null)
                {
                    Konsole.Log("[Last.FM] Cover: {0}", ConsoleColor.Magenta, album.Name);

                    // Find cover
                    var response = _albumApi.GetAlbumInfoAsync(artist.Name, album.Name, true);
                    response.Wait();
                    var albumInfo = response.Result.Content;
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

            // Wait for a second
            TimeSpan duration = DateTime.Now - startTime;
            Thread.Sleep(1000 - duration.Milliseconds); 
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _manager.Save();
        }
    }
}
