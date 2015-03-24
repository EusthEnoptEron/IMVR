using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    [ProtoContract(AsReferenceDefault=true)]
    public class Atlas
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public int TileSize { get; set; }
    }

    [ProtoContract]
    public class AtlasTicket
    {
        [ProtoMember(1)]
        public Atlas Atlas { get; set; }

        [ProtoMember(2)]
        public int Position { get; set; }


        public AtlasTicket() { }
        public AtlasTicket(Atlas atlas, int position)
        {
            Atlas = atlas;
            Position = position;
        }
    }


}
