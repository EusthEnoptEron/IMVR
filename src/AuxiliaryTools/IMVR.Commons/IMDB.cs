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
                foreach (var audio in Music) yield return audio;
            }
        }

        public IMDB()
        {
            Images = new List<Image>();
            Music = new List<Music>();

            Folders = new List<string>();
        }

        [ProtoMember(2)]
        public List<Image> Images { get; private set; }

        [ProtoMember(3)]
        public List<Music> Music { get; private set; }

        [ProtoMember(1)]
        public List<string> Folders { get; private set; }

        public static IMDB FromFile(string file)
        {
            if (!System.IO.File.Exists(file))
                return new IMDB();

            using(var stream = System.IO.File.OpenRead(file)) {
                return Serializer.Deserialize<IMDB>(stream);
            }
        }

        public void Save(string file)
        {
            using(var stream = System.IO.File.OpenWrite(file)) {
                Serializer.Serialize<IMDB>(stream, this);
            }
        }
    }
}
