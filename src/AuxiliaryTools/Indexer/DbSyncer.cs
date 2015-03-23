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
    class DbSyncer : ConsumerNode<DbAction>
    {
        /// <summary>
        /// Number of db actions to store before commiting.
        /// </summary>
        public int BufferSize = 500;
        
        private Queue<DbAction> actions;

        public DbSyncer()
            : base(1)
        {
            this.actions = new Queue<DbAction>(100);
        }

        public void Commit()
        {
            if (actions.Count == 0) return;

            Log(String.Format("Committing {0} changes...", actions.Count), ConsoleColor.Green);

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
                Log("Finished committing!", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Log(e, ConsoleColor.Red);
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

        protected void Log(object text, ConsoleColor color = ConsoleColor.Gray)
        {
            var oldColor = Console.ForegroundColor;
            {
                Console.ForegroundColor = color;
                Console.WriteLine(text);
                Console.ForegroundColor = oldColor;
            }
        }
    }
}
