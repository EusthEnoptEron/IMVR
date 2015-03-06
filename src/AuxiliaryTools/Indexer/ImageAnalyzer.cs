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

namespace Indexer
{
    public class ImageAnalyzer : AbstractConsumer<string>
    {
        private BlockingCollection<DbAction> actions;

        public ImageAnalyzer(BlockingCollection<string> inCollection,
                             BlockingCollection<DbAction> outCollection) : base(inCollection)
        {
            this.actions = outCollection;
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


        protected override void ProcessItem(string path)
        {
            try
            {
                Stopwatch watch = new Stopwatch();

                using (var image = new MagickImage(path))
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

                    var systemColor = System.Drawing.Color.FromArgb(baseColor.R, baseColor.G, baseColor.B);


                    actions.Add((SqliteConnection connection) =>
                    {
                        using (var db = new Main(connection))
                        {
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
                    });
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }
    }
}
