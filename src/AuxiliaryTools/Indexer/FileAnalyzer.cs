using ImageMagick;
using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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


        public FileAnalyzer(BlockingCollection<string> collection)
        {
            this.collection = collection;
            cts = new CancellationTokenSource();
        }

        public async void Start()
        {
            await Task.Factory.StartNew(DoWork, cts.Token);
        }

        private void DoWork(object obj)
        {
            var token = (CancellationToken)obj;

            using (var connection = Database.Connection)
            {
                while (!collection.IsCompleted && !token.IsCancellationRequested)
                {
                    var item = collection.Take();

                    if (Options.Instance.Verbose)
                        Console.WriteLine("Consume: {0}", item);

                    Analyze(item, connection);
                }
            }
            
        }

        private void Analyze(string path, SqliteConnection connection)
        {
            using(var db = new Main(connection)) {
                try
                {
                    var image = new MagickImage(path);
                    var baseColor = new ColorHSL(0, 0, 1);

                    // Get values
                    var statistics = image.Statistics().Composite();
                    var profile = image.GetExifProfile();
                    var red = image.Statistics().GetChannel(PixelChannel.Red);
                    var blue = image.Statistics().GetChannel(PixelChannel.Blue);
                    var green = image.Statistics().GetChannel(PixelChannel.Green);

                    if (red != null && blue != null && green != null)
                    {
                        var color = new MagickColor((byte)red.Mean, (byte)green.Mean, (byte)blue.Mean);
                        baseColor = ColorHSL.FromMagickColor(color);
                    }
                    else
                    {
                        baseColor = new ColorHSL(0, 0, statistics.Mean / 255);
                    }

                    // DB LAYER

                    // Make sure we have a file entry
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
                    statistic.Hue = baseColor.Hue;
                    statistic.Version = 1;
                    statistic.Saturation = baseColor.Saturation;
                    statistic.Lightness = baseColor.Luminosity;
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
    }
}
