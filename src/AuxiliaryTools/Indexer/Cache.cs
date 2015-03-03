using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public static class Cache
    {
        private static Random random = new Random();
        private static string Root
        {
            get
            {
                return Options.Instance.CachePath ?? Path.Combine(Path.GetTempPath(), "VirtualHands");
            }
        }

        public static string GetPath(string extension = ".jpg")
        {
            using (var md5 = MD5.Create())
            {
                var buffer = new byte[128];
                random.NextBytes(buffer);
                var path = MakePath(md5.ComputeHash(buffer) + extension);

                while (File.Exists(path))
                {
                    random.NextBytes(buffer);
                    path = MakePath(md5.ComputeHash(buffer) + extension);
                }

                return path;
            }
        }

        private static string MakePath(string fileName)
        {
            string first = fileName.Substring(0, 2);
            string second = fileName.Substring(0, 4);

            var path = Path.Combine(new string[] { Root, first, second });

            // Make sure structure exists
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return Path.Combine(path, fileName);
        }
    }
}
