using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    [ProtoContract(AsReferenceDefault=true)]
    public class Artist
    {
        public Artist()
        {
            Terms = new List<TermItem>();
            Albums = new List<Album>();
        }

        /// <summary>
        /// Gets or sets the name of the artist.
        /// </summary>
        [ProtoMember(1)] public string Name { get; set; }

        /// <summary>
        /// Gets or sets the biography of this artist.
        /// </summary>
        [ProtoMember(2)] public string Biography { get; set; }


        /// <summary>
        /// Gets a list of albums by this artist.
        /// </summary>
        [ProtoMember(8)]
        public List<Album> Albums { get; private set; }

        /// <summary>
        /// Gets or sets the global familiarity of this artist.
        /// </summary>
        [ProtoMember(3)]
        public float Familiarity { get; set; }

        /// <summary>
        /// Gets or sets the "hotttness" of this artist.
        /// </summary>
        [ProtoMember(4)]
        public float Hotttness { get; set; }

        /// <summary>
        /// Gets or sets the year this artist started their musical activities.
        /// </summary>
        [ProtoMember(5)]
        public int StartYear { get; set; }

        /// <summary>
        /// Gets or sets the year this artist stopped their musical activities.
        /// </summary>
        [ProtoMember(6)]
        public int? EndYear { get; set; }

        /// <summary>
        /// Gets a list of terms describing this artist (i.e. genres)
        /// TODO: split into Style & Mood?
        /// </summary>
        [ProtoMember(7)]
        public List<TermItem> Terms { get; private set; }

        /// <summary>
        /// Gets or sets the location the artist hails from.
        /// </summary>
        [ProtoMember(10)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the coordinate of the artist's origin.
        /// </summary>
        [ProtoMember(11)]
        public GeoCoordinate Coordinate { get; set; }
    }

    [ProtoContract]
    public class TermItem
    {
        public string Name { get; set;}
        public float Weight { get; set; }
        public float Frequency { get; set; }
    }

    [ProtoContract]
    public struct GeoCoordinate
    {
        [ProtoMember(1)]
        private readonly float latitude;
        [ProtoMember(2)]
        private readonly float longitude;

        public float Latitude { get { return latitude; } }
        public float Longitude { get { return longitude; } }

        public GeoCoordinate(float latitude, float longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", Latitude, Longitude);
        }
    }
}
