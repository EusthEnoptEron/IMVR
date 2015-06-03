using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Represents a sprite atlas.
    /// </summary>
    [ProtoContract(AsReferenceDefault=true)]
    public class Atlas
    {
        /// <summary>
        /// The file path to the atlas.
        /// </summary>
        [ProtoMember(1)]
        public string Path { get; set; }

        /// <summary>
        /// The size of the tiles inside the atlas.
        /// </summary>
        [ProtoMember(2)]
        public int TileSize { get; set; }
    }

    /// <summary>
    /// Represents an image in a sprite atlas.
    /// </summary>
    [ProtoContract]
    public class AtlasTicket
    {
        /// <summary>
        /// Gets or sets the atlas this ticket belongs to.
        /// </summary>
        [ProtoMember(1)]
        public Atlas Atlas { get; set; }

        /// <summary>
        /// Gets or sets the position of this image inside the atlas (from left to right, from top to down)
        /// </summary>
        [ProtoMember(2)]
        public int Position { get; set; }


        /// <summary>
        /// Creates an empty AtlasTicket.
        /// </summary>
        public AtlasTicket() { }

        /// <summary>
        /// Creates an AtlasTicket.
        /// </summary>
        /// <param name="atlas">Atlas this ticket belongs to.</param>
        /// <param name="position">Position this ticket occupies.</param>
        public AtlasTicket(Atlas atlas, int position)
        {
            Atlas = atlas;
            Position = position;
        }
    }


}
