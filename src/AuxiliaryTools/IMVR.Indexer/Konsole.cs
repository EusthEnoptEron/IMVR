using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
{
    public static class Konsole
    {
        private static object lockObj = new object();

        private static ConsoleColor oldColor;
        public static void WriteLine(object text, ConsoleColor color = ConsoleColor.Gray)
        {
            WriteLine(text.ToString(), color);
        }

        public static void WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            lock (lockObj)
            {
                var oldColor = Console.ForegroundColor;
                {
                    Console.ForegroundColor = color;
                    Console.WriteLine(text);
                    Console.ForegroundColor = oldColor;
                }
            }
        }
        public static void WriteLine(string format, ConsoleColor color = ConsoleColor.Gray, params object[] args) {
            WriteLine(String.Format(format, args), color);
        }

    }
}
