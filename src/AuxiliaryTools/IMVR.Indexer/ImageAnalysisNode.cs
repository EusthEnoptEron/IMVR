using ImageMagick;
using IMVR.Commons;
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


    public class ImageAnalysisNode : DualNode<FileInfo, DBTask>
    {
        //private const ExifTag[] RELEVANT_TAGS = { 
        //    ExifTag.DateTimeOriginal, ExifTag.ExifVersion, ExifTag.ExposureTime, ExifTag.FocalLength, ExifTag.GPSAltitude, ExifTag.GPSLongitude, ExifTag.GPSLatitude };

        public ImageAnalysisNode(int threadCount) : base(threadCount, 100)
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
                    if (Options.Instance.Verbose)
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



                    var dbImage = new Image()
                    {
                        Path = path.FullName,
                        Entropy = (float)statistics.Entropy,
                        Kurtosis = (float)statistics.Kurtosis,
                        Skewness = (float)statistics.Skewness,
                        Variance = (float)statistics.Variance,
                        Hue = systemColor.GetHue(),
                        Saturation = systemColor.GetSaturation(),
                        Lightness = systemColor.GetBrightness(),
                        Mean = (float)statistics.Mean,
                        Width = image.Width,
                        Height = image.Height,
                        Indexed = DateTime.Now,
                        LastModified = path.LastWriteTime,
                    };

                    SaveImage(dbImage);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }


        private void SaveImage(Image image)
        {
            Publish((db) =>
            {
                db.Images.Add(image);
            });
        }
    }
}
