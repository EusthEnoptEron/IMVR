using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IMVR.Indexer
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
         
            var buffer = new byte[128];
            random.NextBytes(buffer);
            var path = MakePath(CalculateMD5Hash(buffer) + extension);

            while (File.Exists(path))
            {
                random.NextBytes(buffer);
                path = MakePath(CalculateMD5Hash(buffer) + extension);
            }

            return path;
            
        }

        private static string CalculateMD5Hash(byte[] bytes)
        {
            // step 1, calculate MD5 hash from input
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] hash = md5.ComputeHash(bytes);

                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }
                return sb.ToString();
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
