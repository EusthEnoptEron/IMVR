using IMVR.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace IMVR.Indexer
{

    /// <summary>
    /// Node that analyzes audio files. Its task is to pass the signal to:
    /// 1) the analyzer that parses the audio files 
    /// 2) the analyzer that connects to the Echo Nest API to acquire artist information
    /// </summary>
    public class MusicNode : DualNode<FileInfo, Song>, IProducer<Artist>
    {
        private IConsumer<Artist> artistAnalyzer;

        private Dictionary<string, Artist> artistList = new Dictionary<string,Artist>();

        // Only make one of these
        public MusicNode() : base(1) { }


        protected override void ProcessItem(FileInfo item)
        {
            var file = TagLib.File.Create(item.FullName);
            string artistName = file.Tag.FirstPerformer;
            string albumName = file.Tag.Album ?? "Unknown Album";

            // Ignore empty artists
            if (artistName != null)
            {
                // Determine artist
                Artist artist;
                if (artistList.ContainsKey(artistName))
                {
                    artist = artistList[artistName];
                }
                else
                {
                    artist = new Artist()
                    {
                        Name = artistName
                    };

                    artistList.Add(artistName, artist);
                }

                // Determine album
                var album = artist.Albums.FirstOrDefault(a => a.Name == albumName);
                if (album == null)
                {
                    album = new Album()
                    {
                        Name = albumName
                    };

                    artist.Albums.Add(album);
                }
                
                // Determine song
                Song song = new Song()
                {
                    Title = file.Tag.Title,
                    Album = album,
                    Artist = artist,
                    TrackNo = file.Tag.Track,

                    // File properties
                    Path = item.FullName,
                    LastModified = item.LastWriteTime,
                    Indexed = DateTime.Now
                };

                album.Tracks.Add(song);
                
                // Foward music file
                Publish(song);

                if (artistAnalyzer != null)
                    artistAnalyzer.Input.Add(artist);
            }

           
        }

        public void Pipe(IConsumer<Artist> target)
        {
            target.Handshake(this);
            artistAnalyzer = target;
        }
    }
}
