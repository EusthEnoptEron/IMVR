using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{

    [ProtoContract(AsReferenceDefault = true)]
    public class Album
    {
        public Album()
        {
            Tracks = new List<Song>();
        }

        /// <summary>
        /// Gets or sets the album name,
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public int? Year { get; set; }

        /// <summary>
        /// Gets the tracks in this album, ordered by their number.
        /// </summary>
        [ProtoMember(3)]
        public List<Song> Tracks { get; private set; }

        /// <summary>
        /// Gets or sets the Atlas Ticket for the cover art.
        /// </summary>
        [ProtoMember(4)]
        public AtlasTicket Atlas { get; set; }


        /// <summary>
        /// Gets the artists participating in this album.
        /// </summary>
        public Artist[] Artists
        {
            get
            {
                return Tracks.Select(t => t.Artist).Distinct().ToArray();
            }
        }
    }
}
