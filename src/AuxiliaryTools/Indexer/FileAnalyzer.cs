using ImageMagick;
using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualHands.Data;

namespace Indexer
{
    public class FileAnalyzer
    {
        private BlockingCollection<string> collection;
        private CancellationTokenSource cts;
        private const int COMMIT_INTERVAL = 100;


        public FileAnalyzer(BlockingCollection<string> collection)
        {
            this.collection = collection;
            cts = new CancellationTokenSource();
        }

        public Task Start()
        {
            return Task.Factory.StartNew(DoWork, cts.Token);
        }

        private void DoWork(object obj)
        {
            var token = (CancellationToken)obj;
            int counter = 0;

            using (var connection = Database.Connection)
            {
                SqliteTransaction transaction = connection.BeginTransaction();

                while (!collection.IsCompleted && !token.IsCancellationRequested)
                {
                    Console.WriteLine(collection.IsCompleted + " " + collection.IsAddingCompleted + " "  + collection.Count);
                    string item;
                    if (collection.TryTake(out item, 100))
                    {

                        if (Options.Instance.Verbose)
                            Console.WriteLine("Consume: {0}", item);

                        Analyze(item, connection);

                        if (counter++ >= COMMIT_INTERVAL)
                        {

                            if (Options.Instance.Verbose)
                                Console.WriteLine("Commit");

                            transaction.Commit();
                            transaction.Dispose();

                            transaction = connection.BeginTransaction();
                            counter = 0;
                        }
                    }
                }
                Console.WriteLine("DONE");


                transaction.Commit();
                transaction.Dispose();
            }
            
        }

        private void Analyze(string path, SqliteConnection connection)
        {
            using(var db = new Main(connection)) {
                try
                {
                    Stopwatch watch = new Stopwatch();

                    var image = new MagickImage(path);

                    // Get values
                    var statistics = image.Statistics().Composite();
                    var profile = image.GetExifProfile();

                    image.Resize(1, 1);
                    var firstPixel = image.GetReadOnlyPixels().First();
                    var baseColor = firstPixel.ToColor() ??
                                new MagickColor(firstPixel.GetChannel(0), firstPixel.GetChannel(0), firstPixel.GetChannel(0));

                    var systemColor = System.Drawing.Color.FromArgb(baseColor.R, baseColor.G, baseColor.B);

                    //RGBtoHSV(baseColor.R / 255f, baseColor.G / 255f, baseColor.B / 255f, out c_hue, out c_saturation, out c_value);

                    //Console.WriteLine(baseColor.Saturation);
                    //Console.WriteLine(image.GetReadOnlyPixels().First().Channels);

                    //var red = image.Statistics().GetChannel(PixelChannel.Red);
                    //var blue = image.Statistics().GetChannel(PixelChannel.Blue);
                    //var green = image.Statistics().GetChannel(PixelChannel.Green);

                    //if (red != null && blue != null && green != null)
                    //{
                    //    var color = new MagickColor((byte)red.Mean, (byte)green.Mean, (byte)blue.Mean);
                    //    baseColor = ColorHSL.FromMagickColor(color);
                    //}
                    //else
                    //{
                    //    baseColor = new ColorHSL(0, 0, statistics.Mean / 255);
                    //}

                    // DB LAYER

                    // Make sure we have a file entry
                    watch.Start();

                    var file = db.Files.FirstOrDefault(f => f.Path == path);

                    if (file == null)
                    {
                        file = new VirtualHands.Data.File();
                        db.Files.InsertOnSubmit(file);
                    }

                    file.Path = path;
                    file.Indexed = DateTime.Now;

                    var statistic = file.ImageStatistics.FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new ImageStatistic();
                        file.ImageStatistics.Add(statistic);
                    }

                    statistic.Entropy = statistics.Entropy;
                    statistic.Mean = statistics.Mean;
                    statistic.Kurtosis = statistics.Kurtosis;
                    statistic.Skewness = statistics.Skewness;
                    statistic.Variance = statistics.Variance;
                    statistic.HasExif = profile != null;
                    statistic.Version = 1;
                    statistic.Hue = systemColor.GetHue();
                    statistic.Saturation = systemColor.GetSaturation();
                    statistic.Lightness = systemColor.GetBrightness();
                    statistic.Width = image.Width;
                    statistic.Height = image.Height;

                    statistic.LastModified = new System.IO.FileInfo(path).LastWriteTime;
                    db.SubmitChanges();

                    if (profile != null)
                    {
                        var command = new SqliteCommand("DELETE FROM ExifValues WHERE FileId = @id", connection);
                        command.Parameters.AddWithValue("@id", file.ID);
                        command.ExecuteNonQuery();


                        command = new SqliteCommand("INSERT INTO ExifValues(`FileId`, `Key`, `Value`) VALUES (@id, @key, @value)", connection);
                        command.Parameters.AddWithValue("@id", file.ID);
                        var keyVal = command.Parameters.AddWithValue("@key", "");
                        var valVal = command.Parameters.AddWithValue("@value", "");

                        foreach (var value in profile.Values)
                        {
                            keyVal.Value = value.Tag.ToString();
                            valVal.Value = value.Value.ToString();
                            try
                            {
                                command.ExecuteNonQuery();
                            }
                            catch (SqliteException e)
                            {
                                Console.Error.WriteLine("{0}: {1}", value.Tag, e.Message);
                            }
                        }
                    }
                     

                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
            }
        }

        public void Stop()
        {
            cts.Cancel();
        }

        // r,g,b values are from 0 to 1
        // h = [0,360], s = [0,1], v = [0,1]
        //		if s == 0, then h = -1 (undefined)
        private void RGBtoHSV(double r, double g, double b, out double h, out double s, out double v)
        {
            double min, max, delta;
            min = Math.Min(Math.Min(r, g), b);
            max = Math.Max(Math.Max(r, g), b);
            
            v = max;				// v
            delta = max - min;
            if (max != 0)
                s = delta / max;		// s
            else
            {
                // r = g = b = 0		// s = 0, v is undefined
                s = 0;
                h = -1;
                return;
            }
            if (r == max)
                h = (g - b) / delta;		// between yellow & magenta
            else if (g == max)
                h = 2 + (b - r) / delta;	// between cyan & yellow
            else
                h = 4 + (r - g) / delta;	// between magenta & cyan
            h *= 60;				// degrees
            if (h < 0)
                h += 360;
        }
    }
}
