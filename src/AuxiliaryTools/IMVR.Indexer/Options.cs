using CommandLine;
using IMVR.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public class Options
    {
        private static Options _instance;
        private Options() { }

        [Option("d", Required=true, HelpText="Path to the SQLite DB")]
        public string DbPath { get; set; }

        [Option("c", Required=false, DefaultValue=null, HelpText="Path to the cache directory")]
        public string CachePath { get; set; }

        public static Options Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Options();
                }
                return _instance;
            }
        }


        private IMDB _db = null;
        public IMDB DB
        {
            get
            {
                if (_db == null)
                {
                    _db = IMDB.FromFile(DbPath);
                }
                return _db;
            }
        }

        [Option('v', "verbose")]
        public bool Verbose { get; set; }
    }
}
