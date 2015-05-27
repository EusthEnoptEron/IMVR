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
            Out.Color = ConsoleColor.Blue;
            _session = new EchoNestSession(API_KEY, true);
            _manager.TileSize = 256;
            //_manager.XPowerOf2 = 9; // 512
            //_manager.YPowerOf2 = 9; // 512

            _manager.XPowerOf2 = 12; // 4096
            _manager.YPowerOf2 = 12; // 4096

        }

        protected override void ProcessItem(Artist artist)
        {

            Out.Log("Artist: {0}", artist.Name);
            string artistID = null;

            // ----- Collect song meta data ----
            int offset = 0;
            while (true)
            {
                try
                {
                    var songs = _session.Query<EchoNest.Song.Search>().Execute(GetSearchArgument(artist.Name, offset));
                    if (songs.Status.Code == ResponseCode.Success)
                    {
                        for (int i = 0; i < songs.Songs.Count; i++)
                        {
                            var song = songs.Songs[i];

                            if (i == 0)
                            {
                                artist.Location = song.ArtistLocation.Location;
                                artist.Coordinate = new GeoCoordinate((float)song.ArtistLocation.Latitude, (float)song.ArtistLocation.Longitude);
                            }
                            foreach (var album in artist.Albums)
                            {
                                foreach (var track in album.Tracks.Where(track => track.Title == song.Title))
                                {
                                    Out.Log("Found a matching song!");
                                    artistID = song.ArtistID;
                                    track.Danceability = (float?)song.AudioSummary.Danceability;
                                    track.Energy = (float)song.AudioSummary.Energy;
                                    track.Tempo = (float)song.AudioSummary.Tempo;
                                    track.Valence = (float?)song.AudioSummary.Valence;
                                    track.Speechiness = (float?)song.AudioSummary.Speechiness;
                                    track.Liveness = (float?)song.AudioSummary.Liveness;
                                    track.Instrumentalness = (float?)song.AudioSummary.Instrumentalness;
                                }
                            }
                        }

                        if (songs.Songs.Count < RESULT_COUNT)
                        {
                            break;
                        }
                        else
                        {
                            // There's more to fetch
                            Out.WriteLine("...");
                            offset += RESULT_COUNT;
                        }
                    }
                    else
                    {
                        Konsole.WriteLine(songs.Status.Message, ConsoleColor.Red);
                        break;
                    }
                }
                catch (Exception e)
                {
                    Konsole.WriteLine("Error occurred: {0}", e);
                }

            }

            // Search for profile. Use the ID if we have one.
            ProfileResponse profile;
            if(artistID == null)
                profile = _session.Query<Profile>().Execute(artist.Name, AllBuckets);
            else
                profile = _session.Query<Profile>().Execute(new IdSpace(artistID), AllBuckets);

            if (profile.Status.Code == ResponseCode.Success)
            {
                Out.Log("Found record for artist!");
                // All right, we found something!

                // ---- Collect artist meta information ----
                artist.EchoNestID = profile.Artist.ID;
                artist.Biography = profile.Artist.Biographies.Count > 0
                                    ? profile.Artist.Biographies.First().Text
                                    : "";

                ForeignIdItem twitterCatalog = null;
                ForeignIdItem mbCatalog = null;
                if (profile.Artist.ForeignIds != null)
                {
                    twitterCatalog = profile.Artist.ForeignIds.FirstOrDefault(id => id.Catalog == "twitter");
                    mbCatalog = profile.Artist.ForeignIds.FirstOrDefault(id => id.Catalog == "musicbrainz");
                }

                artist.TwitterHandle = twitterCatalog != null ? twitterCatalog.ID.Replace("twitter:artist:", "") : null;
                artist.MusicBrainzId = mbCatalog != null ? mbCatalog.ID.Replace("musicbrainz:artist:", "") : null;

                if (artist.TwitterHandle != null)
                    Out.WriteLine("Found twitter handle: {0}", artist.TwitterHandle);

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

                foreach (var image in profile.Artist.Images.Take(4))
                {
                    artist.Pictures.Add(_manager.GetTicket(image.Url));
                }

                //profile.Artist.Images.FirstOrDefault().Url
                // /----------------------------------------
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
                       ArtistBucket.IdMusicBrainz |
                       ArtistBucket.IdTwitter |
                       ArtistBucket.ArtistLocation;
            }
        }

    }
}
