using IMVR.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{

    public delegate void DBTask(IMDB db);

    public class PersistenceWorker : ConsumerNode<DBTask>
    {
        /// <summary>
        /// Number of db actions to store before commiting.
        /// </summary>
        public int BufferSize = 100;
        
        private int changes = 0;

        private IMDB db;
        public PersistenceWorker(IMDB db)
            : base(1)
        {
            this.db = db;
        }

        protected override void ProcessItem(DBTask task)
        {
            task(db);
            changes++;

            if (changes > BufferSize) Commit();
        }

        protected override void CleanUp()
        {
            Commit();
        }

        public void Commit()
        {
            Log(String.Format("Committing {0} changes...", changes), ConsoleColor.Green);

            try
            {
                db.Save(Options.Instance.DbPath);
                Log("Finished committing!", ConsoleColor.Green);
            }
            catch (Exception e)
            {
                Log(e, ConsoleColor.Red);
            }

            changes = 0;
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
