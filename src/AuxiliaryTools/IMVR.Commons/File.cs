using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IMVR.Commons
{
    /// <summary>
    /// Represents an indexed file.
    /// </summary>
    public abstract class File
    {
        /// <summary>
        /// Path to the file on the file system.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the date when the file was indexed.
        /// </summary>
        public DateTime Indexed { get; set; }

        /// <summary>
        /// Gets or sets the date the file was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}
