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
        private EchoNestSession session;

        // ONLY ONE THREAD
        public EchoNestNode() : base(1)
        {
            session = new EchoNestSession(API_KEY);
        }

        protected override void ProcessItem(Artist artist)
        {

            if(Options.Instance.Verbose)
                Konsole.WriteLine("Artist: {0}",  ConsoleColor.Yellow, artist.Name);
            return;

            var profile = session.Query<Profile>().Execute(artist.Name, AllBuckets);
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
                //profile.Artist.Images.FirstOrDefault().Url
                // /----------------------------------------


                // ----- Collect song meta data ----
                var songs = session.Query<EchoNest.Song.Search>().Execute(GetSearchArgument(artist.Name));

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
                                track.Danceability = (float)song.AudioSummary.Danceability;
                                track.Energy = (float)song.AudioSummary.Energy;
                                track.Tempo = (float)song.AudioSummary.Tempo;
                            }
                        }
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

        private EchoNest.Song.SearchArgument GetSearchArgument(string artistName, int? offset = null)
        {
            var argument = new EchoNest.Song.SearchArgument();
            argument.Bucket = AllSongBuckets;
            argument.Artist = artistName;
            argument.Results = 100;
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
