using Indexer;
using Mono.Data.Sqlite;
using DbLinq.Data.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualHands.Data
{
    public partial class Main
    {
        public Main(Indexer.Options options) : base("Data Source=" + options.DbPath +";DbLinqProvider=sqlite;DbLinqConnectionType=" + typeof(Mono.Data.Sqlite.SqliteConnection).AssemblyQualifiedName)
        {
        }
    }

    public static class Database
    {
        public static string ConnectionString
        {
            get
            {
                return "URI=file:" + Options.Instance.DbPath;
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
                return new Main(Connection);
            }
        }
    }
}
