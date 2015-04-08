using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace IMVR.Commons
{
    /// <summary>
    /// The entry point to access all cached data.
    /// </summary>
    [ProtoContract]
    public class IMDB
    {
        public IEnumerable<File> Files
        {
            get
            {
                foreach (var image in Images) yield return image;
                foreach (var audio in Songs) yield return audio;
            }
        }

        public IMDB()
        {
            Images = new List<Image>();
            Songs = new List<Song>();
            Artists = new List<Artist>();
            
            Folders = new List<string>();
            Atlases = new List<Atlas>();
        }

        [ProtoMember(2)]
        public List<Image> Images { get; private set; }

        [ProtoMember(3)]
        public List<Song> Songs { get; private set; }

        [ProtoMember(1)]
        public List<string> Folders { get; private set; }

        [ProtoMember(4)]
        public List<Atlas> Atlases { get; private set; }

        [ProtoMember(5)]
        public List<Artist> Artists { get; private set; }


        #region Persistence Implementation

        public static IMDB FromFile(string file)
        {
            if (!System.IO.File.Exists(file))
                return new IMDB();

            using (var stream = System.IO.File.OpenRead(file))
            {
                return Serializer.Deserialize<IMDB>(stream);
            }
        }

        public void Save(string file)
        {
            using(var stream = System.IO.File.OpenWrite(file)) {
                Serializer.Serialize<IMDB>(stream, this);
                stream.SetLength(stream.Position);
            }
        }

        #endregion
    }
}
