using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.Data;
using VirtualHands.Data;
using System.IO;
using System.Data.SQLite;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbpath = Path.Combine("D:/Dev/VirtualHands/src/Application/Assets", "Database.s3db");
            var connection = new System.Data.SQLite.SQLiteConnection("URI=file:" + dbpath);
            connection.Open();
            using (var db = new Main(connection))
            {
                //var reader = new SQLiteCommand("SELECT Id FROM Files", connection).ExecuteReader();
                //while (reader.Read())
                //{
                //    Console.WriteLine(reader.GetInt32(0));
                //}

                foreach (var path in new string[] { 
                    @"C:\Users\meers1\Pictures\CPVR2-CP\exercises\images\barb.png",
                    @"C:\Users\meers1\Pictures\CPVR2-CP\exercises\images\cpvr_faces_320\0000\04.JPG",
                    @"C:\Users\meers1\Pictures\CPVR2-CP\exercises\images\Sat_PC123RGB.tif"})
                {
                    var image = new MagickImage(path);
                    var baseColor = new ColorHSL(0, 0, 1);
                    //var myFiles = db.Files.ToList();
                    

                    var file = db.Files.FirstOrDefault(f => f.Path == path);
                    file.Path = path;

                    var statistics = image.Statistics().Composite();
                    var profile = image.GetExifProfile();
                    Console.WriteLine("-----------------------------------------\n   {0}\n----------------------------------------", image.FileName);
                    Console.WriteLine("Entropy: {0}\nKurtosis: {1}\nMaximum: {2}\nMean: {3}\nSkewness: {4}\nVariance: {5}",
                        statistics.Entropy,
                        statistics.Kurtosis,
                        statistics.Maximum,
                        statistics.Mean,
                        statistics.Skewness,
                        statistics.Variance
                    );

                    if (profile != null)
                    {
                        Console.WriteLine("Profile: " + profile.Name);
                        foreach (var value in profile.Values)
                        {
                            Console.WriteLine("[{0}] {1}", value.Tag, value.Value);
                        }
                    }

                    var red  = image.Statistics().GetChannel(PixelChannel.Red);
                    var blue = image.Statistics().GetChannel(PixelChannel.Blue);
                    var green= image.Statistics().GetChannel(PixelChannel.Green);

                    if (red != null && blue != null && green != null)
                    {
                        var color = new MagickColor((byte)red.Mean, (byte)green.Mean, (byte)blue.Mean);
                        baseColor = ColorHSL.FromMagickColor(color);
                    }
                    else
                    {
                        baseColor = new ColorHSL(0, 0, statistics.Mean / 255);
                    }

                }
                db.SubmitChanges();
            }


            Console.ReadLine();
        }
    }
}
