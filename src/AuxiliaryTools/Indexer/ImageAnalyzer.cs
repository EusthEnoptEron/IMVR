using ImageMagick;
using Mono.Data.Sqlite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualHands.Data;

namespace IMVR.Indexer
{


    public class ImageAnalyzer : DualNode<FileInfo, DbAction>
    {
        private class ImageMetrics
        {
            public ChannelStatistics Statistics;
            public int Width;
            public int Height;
            public MagickColor AverageColor;
            public string Path;
            public ExifProfile Profile;
        }

        public ImageAnalyzer(int threadCount) : base(threadCount)
        {
        }

        //private void CheckAtlas()
        //{
        //    if (atlas == null || atlas.IsFull)
        //    {
        //        if(atlas == null) {
        //            atlas = new TextureAtlas(1 << 11, 1 << 11);
        //        } else {
        //            atlas.Generate(atlasPath.FullName);
        //            atlas.Reset();
        //        }

        //        Console.WriteLine(atlasPath);

        //        // Create new atlas
        //        atlas.Reset();
        //        atlasPath = new FileInfo(Cache.GetPath());
        //    }
        //}


        protected override void ProcessItem(FileInfo path)
        {
            try
            {
                Stopwatch watch = new Stopwatch();

                using (var image = new MagickImage(path.FullName))
                {
                    Console.WriteLine("Consume " + path);

                    //if (image.Width > TextureAtlas.TileSize && image.Height > TextureAtlas.TileSize)
                    //    atlas.Add(path);

                    // Get values
                    var statistics = image.Statistics().Composite();
                    var profile = image.GetExifProfile();

                    image.Resize(1, 1);
                    var firstPixel = image.GetReadOnlyPixels().First();
                    var baseColor = firstPixel.ToColor() ??
                                new MagickColor(firstPixel.GetChannel(0), firstPixel.GetChannel(0), firstPixel.GetChannel(0));


                    var dataBall = new ImageMetrics
                    {
                        Statistics = statistics,
                        Width = image.Width,
                        Height = image.Height,
                        AverageColor = baseColor,
                        Path = path.FullName,
                        Profile = profile
                    };

                    Console.WriteLine("Analyzed, publishing");
                    Publish(GetQuery(dataBall));
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        private DbAction GetQuery(ImageMetrics data)
        {
            return (connection, transaction) =>
            {
                using (var db = new Main(connection))
                {
                    db.Transaction = transaction;

                    var file = db.Files.FirstOrDefault(f => f.Path == data.Path);

                    if (file == null)
                    {
                        file = new VirtualHands.Data.File();
                        db.Files.InsertOnSubmit(file);
                    }

                    file.Path = data.Path;
                    file.Indexed = DateTime.Now;


                    var statistic = file.ImageStatistics.FirstOrDefault();
                    if (statistic == null)
                    {
                        statistic = new ImageStatistic();
                        file.ImageStatistics.Add(statistic);
                    }

                    var systemColor = System.Drawing.Color.FromArgb(data.AverageColor.R, data.AverageColor.G, data.AverageColor.B);

                    statistic.Entropy = data.Statistics.Entropy;
                    statistic.Mean = data.Statistics.Mean;
                    statistic.Kurtosis = data.Statistics.Kurtosis;
                    statistic.Skewness = data.Statistics.Skewness;
                    statistic.Variance = data.Statistics.Variance;
                    statistic.HasExif = data.Profile != null;
                    statistic.Version = 1;
                    statistic.Hue = systemColor.GetHue();
                    statistic.Saturation = systemColor.GetSaturation();
                    statistic.Lightness = systemColor.GetBrightness();
                    statistic.Width = data.Width;
                    statistic.Height = data.Height;

                    statistic.LastModified = new System.IO.FileInfo(data.Path).LastWriteTime;
                    db.SubmitChanges();

                    if (data.Profile != null)
                    {
                        var command = new SqliteCommand("DELETE FROM ExifValues WHERE FileId = @id", connection, transaction);
                        command.Parameters.AddWithValue("@id", file.ID);
                        command.ExecuteNonQuery();


                        command = new SqliteCommand("INSERT INTO ExifValues(`FileId`, `Key`, `Value`) VALUES (@id, @key, @value)", connection, transaction);
                        command.Parameters.AddWithValue("@id", file.ID);
                        var keyVal = command.Parameters.AddWithValue("@key", "");
                        var valVal = command.Parameters.AddWithValue("@value", "");

                        foreach (var value in data.Profile.Values)
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
            };
        }

    }
}
