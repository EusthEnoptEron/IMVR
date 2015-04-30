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
                    //Console.Out.Flush();
                }
            }
        }
        public static void WriteLine(string format, ConsoleColor color = ConsoleColor.Gray, params object[] args) {
            WriteLine(String.Format(format, args), color);
        }


        public static void Log(object text, ConsoleColor color = ConsoleColor.Gray)
        {
            if(Options.Instance.Verbose)
                WriteLine(text.ToString(), color);
        }

        public static void Log(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            if (Options.Instance.Verbose)
                WriteLine(text, color);
        }
        public static void Log(string format, ConsoleColor color = ConsoleColor.Gray, params object[] args)
        {
            if (Options.Instance.Verbose)
                WriteLine(String.Format(format, args), color);
        }

        public class NamedConsole {
            public string Name { get; private set; }
            public ConsoleColor Color { get; set; }

            public NamedConsole(string name, ConsoleColor color)
            {
                Name = name;
                Color = color;
            }

            public void WriteLine(object text)
            {
                Konsole.WriteLine(Prepare(text.ToString()), Color);
            }

            public void WriteLine(string format, params object[] args)
            {
                Konsole.WriteLine(Prepare(String.Format(format, args)), Color);
            }

            public void Log(object text)
            {
                Konsole.Log(Prepare(text.ToString()), Color);
            }


            public void Log(string format, params object[] args)
            {
                Konsole.Log(Prepare(String.Format(format, args)), Color);
            }

            private string Prepare(string text)
            {
                return String.Format("[{0}] {1}", Name, text);
            }

        }
    }
}
