using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Represents an indexed audio file.
    /// </summary>
    [ProtoContract(AsReferenceDefault = true)]
    public class Song : File
    {
        [ProtoMember(13)]
        public string Title { get; set; }

        [ProtoMember(1)]
        public Artist Artist { get; set; }

        [ProtoMember(2)]
        public Album Album { get; set; }

        [ProtoMember(3)]
        public uint TrackNo { get; set; }

        /// <summary>
        /// Gets or sets the duration in seconds.
        /// </summary>
        [ProtoMember(4)]
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the beats-per-minute.
        /// </summary>
        [ProtoMember(5)]
        public float Tempo { get; set; }

        /// <summary>
        /// Gets or sets the liveness of this song.
        /// </summary>
        [ProtoMember(6)]
        public float? Liveness { get; set; }

        /// <summary>
        /// Gets or sets the speechiness.
        /// </summary>
        [ProtoMember(7)]
        public float? Speechiness { get; set; }

        /// <summary>
        /// Gets or sets the acousticness.
        /// </summary>
        [ProtoMember(8)]
        public float? Acousticness { get; set; }

        /// <summary>
        /// Gets or sets the instrumentalness.
        /// </summary>
        [ProtoMember(9)]
        public float? Instrumentalness { get; set; }

        /// <summary>
        /// Gets or sets the danceability.
        /// </summary>
        [ProtoMember(10)]
        public float? Danceability { get; set; }

        /// <summary>
        /// Gets or sets the energy of this song.
        /// </summary>
        [ProtoMember(11)]
        public float? Energy { get; set; }

        /// <summary>
        /// Gets or sets the variance of this song.
        /// </summary>
        [ProtoMember(12)]
        public float? Variance { get; set; }



    }


}
