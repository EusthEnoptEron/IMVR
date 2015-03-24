using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{

    /// <summary>
    /// Node that analyzes audio files. Its task is to pass the signal to:
    /// 1) the analyzer that parses the audio files for meta data
    /// 2) the analyzer that connects to the Echo Nest API to acquire artist information
    /// </summary>
    public class MusicNode : DualNode<FileInfo, FileInfo>, IProducer<string>
    {
        private IConsumer<string> artistAnalyzer;

        public MusicNode(int threadCount) : base(threadCount) { }

        protected override void ProcessItem(FileInfo item)
        {
            // Foward music file
            Publish(item);

            // Determine artist
            // TODO: do it
            if (artistAnalyzer != null)
                artistAnalyzer.Input.Add("");
        }

        public void Pipe(IConsumer<string> target)
        {
            target.Handshake(this);
            artistAnalyzer = target;
        }
    }
}
