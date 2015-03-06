using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public delegate void DbAction(SqliteConnection connection);
    class DbSyncer : AbstractConsumer<DbAction>
    {
        public DbSyncer(BlockingCollection<DbAction> actions)
            : base(actions)
        {
        }

        public void Commit()
        {

        }

        protected override void ProcessItem(DbAction item)
        {
            Console.WriteLine("add to db...");
        }
    }
}
