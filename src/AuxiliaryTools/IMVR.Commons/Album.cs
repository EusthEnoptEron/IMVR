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
        /// <summary>
        /// Gets or sets the album name,
        /// </summary>
        public string Name { get; set; }

        public int? Year { get; set; }

        /// <summary>
        /// Gets or sets the tracks in this album, ordered by their number.
        /// </summary>
        public Song[] Tracks { get; set; }

        /// <summary>
        /// Gets or sets the Atlas Ticket for the cover art.
        /// </summary>
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
