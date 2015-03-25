using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Represents an indexed file.
    /// </summary>
    [ProtoContract]
    [ProtoInclude(100, typeof(Song))]
    [ProtoInclude(101, typeof(Image))]
    public abstract class File
    {
        /// <summary>
        /// Path to the file on the file system.
        /// </summary>
        [ProtoMember(1)]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the date when the file was indexed.
        /// </summary>
        [ProtoMember(2)]
        public DateTime Indexed { get; set; }

        /// <summary>
        /// Gets or sets the date the file was last modified.
        /// </summary>
        [ProtoMember(3)]
        public DateTime LastModified { get; set; }
    }
}
