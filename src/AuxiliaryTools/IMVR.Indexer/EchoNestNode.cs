using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EchoNest.Artist;
using EchoNest;
using IMVR.Commons;
using System.Threading;
using EchoNest.Song;

namespace IMVR.Indexer
{
    public class EchoNestNode : ConsumerNode<Artist>
    {
        private const string API_KEY = "IIYVSIK0ZCRCMU3VS";
        private EchoNestSession _session;
        private AtlasManager _manager = new AtlasManager();

        private const int RESULT_COUNT = 100;
        // ONLY ONE THREAD
        public EchoNestNode() : base(1)
        {
            _session = new EchoNestSession(API_KEY, true);
            _manager.TileSize = 256;
        }

        protected override void ProcessItem(Artist artist)
        {

            if(Options.Instance.Verbose)
                Out.WriteLine("Artist: {0}", artist.Name);

            var profile = _session.Query<Profile>().Execute(artist.Name, AllBuckets);
            if (profile.Status.Code == ResponseCode.Success)
            {
                // All right, we found something!

                // ---- Collect artist meta information ----
                artist.Biography = profile.Artist.Biographies.Count > 0
                                    ? profile.Artist.Biographies.First().Text
                                    : "";
                artist.Familiarity = (float)(profile.Artist.Familiarity ?? float.NaN);
                artist.Hotttness = (float)(profile.Artist.Hotttnesss ?? float.NaN);
                artist.Terms.AddRange(profile.Artist.Terms.Select(item => new TermItem()
                {
                    Frequency = (float)item.Frequency,
                    Name = item.Name,
                    Weight = (float)item.Weight
                }));

                var yearsActive = profile.Artist.YearsActive.LastOrDefault();
                if (yearsActive != null)
                {
                    artist.StartYear = yearsActive.Start;
                    artist.EndYear = yearsActive.End;
                }

                foreach (var image in profile.Artist.Images.Take(5))
                {
                    artist.Pictures.Add(_manager.GetTicket(image.Url));
                }

                //profile.Artist.Images.FirstOrDefault().Url
                // /----------------------------------------
                
                // ----- Collect song meta data ----
                int offset = 0;
                while (true)
                {
                    var songs = _session.Query<EchoNest.Song.Search>().Execute(GetSearchArgument(artist.Name, offset));
                    if (songs.Status.Code == ResponseCode.Success)
                    {
                        Out.WriteLine("Found {0} songs", songs.Songs.Count);
                        for (int i = 0; i < songs.Songs.Count; i++)
                        {
                            var song = songs.Songs[i];

                            if (i == 0)
                            {
                                artist.Location = song.ArtistLocation.Location;
                                artist.Coordinate = new GeoCoordinate((float)song.ArtistLocation.Latitude, (float)song.ArtistLocation.Longitude);
                            }
                            //Out.WriteLine("SONG: {0} {1}", song.Title, song.AudioSummary.Danceability);

                            foreach (var album in artist.Albums)
                            {
                                foreach (var track in album.Tracks.Where(track => track.Title == song.Title))
                                {
                                    Out.WriteLine("TRACK: {0} {1}", song.Title, song.AudioSummary.Danceability);

                                    track.Danceability = (float)song.AudioSummary.Danceability;
                                    track.Energy = (float)song.AudioSummary.Energy;
                                    track.Tempo = (float)song.AudioSummary.Tempo;
                                }
                            }
                        }

                        if (songs.Songs.Count < RESULT_COUNT)
                        {
                            break;
                        }
                        else
                        {
                            Out.WriteLine("There's more...");
                            offset += RESULT_COUNT;
                        }
                    }
                    else
                    {
                        Konsole.WriteLine(songs.Status.Message, ConsoleColor.Red);
                        break;
                    }
                }
               
                
            }
            //else if (profile.Status.Code == ResponseCode.RateLimitExceeded)
            //{
            //    // Wait half a minute
            //    Thread.Sleep(30 * 1000);

            //    // Try again
            //    ProcessItem(artist);
            //}
        }
        protected override void CleanUp()
        {
            base.CleanUp();
            _manager.Save();
        }

        private EchoNest.Song.SearchArgument GetSearchArgument(string artistName, int? offset = null)
        {
            var argument = new EchoNest.Song.SearchArgument();
            argument.Bucket = AllSongBuckets;
            argument.Artist = artistName;
            argument.Results = RESULT_COUNT;
            argument.Start = offset;
            return argument;
        }

        private static SongBucket AllSongBuckets
        {
            get
            {
                return SongBucket.ArtistLocation |
                       SongBucket.SongHotttness |
                       SongBucket.AudioSummary;
            }
        }

        /// <summary>
        /// Gets an enum that combines all buckets
        /// </summary>
        private static ArtistBucket AllBuckets
        {

            get
            {
                return ArtistBucket.Biographies | 
                       ArtistBucket.Familiarity | 
                       ArtistBucket.Terms | 
                       ArtistBucket.Images |
                       ArtistBucket.Hotttnesss | 
                       ArtistBucket.YearsActive | 
                       ArtistBucket.ArtistLocation;
            }
        }

    }
}
