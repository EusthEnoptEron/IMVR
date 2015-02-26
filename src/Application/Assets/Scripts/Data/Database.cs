using DbLinq.Sqlite;
using DbLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Mono.Data.Sqlite;

namespace VirtualHands.Data
{
    public partial class Main
    {
        public Main() : base("Data Source=" + Path.Combine(Database.DATA_PATH, "Database.s3db") +";DbLinqProvider=sqlite;DbLinqConnectionType=" + typeof(Mono.Data.Sqlite.SqliteConnection).AssemblyQualifiedName)
        {
        }
    }

    public static class Database
    {
        public static string DATA_PATH = Application.dataPath;

        public static string ConnectionString
        {
            get
            {
                return "URI=file:" + Path.Combine(DATA_PATH, "Database.s3db");
            }
        }

        public static SqliteConnection Connection
        {
            get
            {
                
                var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                int affected = (new SqliteCommand("PRAGMA foreign_keys = ON", connection).ExecuteNonQuery());

                return connection;
            }
        }

        public static Main Context
        {
            get
            {
                return new Main(Connection, new SqliteVendor());
            }
        }
    }
}
