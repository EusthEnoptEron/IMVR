using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualHands.Data;

namespace IMVR.Indexer
{
    public delegate void DbAction(SqliteConnection connection, SqliteTransaction transaction);
    class DBSyncNode : ConsumerNode<DbAction>
    {
        /// <summary>
        /// Number of db actions to store before commiting.
        /// </summary>
        public int BufferSize = 500;
        
        private Queue<DbAction> actions;

        public DBSyncNode()
            : base(1)
        {
            this.actions = new Queue<DbAction>(100);
        }

        public void Commit()
        {
            if (actions.Count == 0) return;

            Konsole.Log(String.Format("Committing {0} changes...", actions.Count), ConsoleColor.Green);

            try
            {
                using (var connection = Database.Connection)
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        while (actions.Count > 0)
                        {
                            actions.Dequeue()(connection, transaction);
                        }

                        transaction.Commit();
                    }
                }
                Konsole.Log("Finished committing!", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Konsole.Log(e, ConsoleColor.Red);
            }


        }

        protected override void ProcessItem(DbAction item)
        {
            actions.Enqueue(item);
            if (actions.Count >= BufferSize) Commit();
        }

        protected override void CleanUp()
        {
            Commit();
        }
    }
}
