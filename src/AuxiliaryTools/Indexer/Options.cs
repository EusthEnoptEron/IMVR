﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public class Options
    {
        private static Options _instance;
        private Options() { }

        [Option("d", Required=true, HelpText="Path to the SQLite DB")]
        public string DbPath { get; set; }

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

        [Option('v', "verbose")]
        public bool Verbose { get; set; }
    }
}