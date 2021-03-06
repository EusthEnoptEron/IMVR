﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;
using System.IO;
using System.Net;

namespace IMVR.Indexer
{
    public class TextureAtlas : IDisposable
    {

        public readonly int TileSize;
        
        public bool IsFull { get; private set; }
        private List<string> entries = new List<string>();

        private IEnumerator<Rectangle> tiles;
        private int tilesNo = 0;
        private List<string> tempFiles = new List<string>();
        
        public int Width { get; private set; }
        public int Height { get; private set; }

        private string path;

        public TextureAtlas(string path, int width, int height, int tileSize = 128)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            Atlas = new Commons.Atlas()
            {
                Path = path,
                TileSize = TileSize
            };

            this.path = path;

            Reset();
            //Texture.filterMode = FilterMode.Trilinear;
            //Texture.anisoLevel = 0;
        }


        public void Reset()
        {
            entries.Clear();
            tiles = Tiles.GetEnumerator();
            tilesNo = 0;
            IsFull = !tiles.MoveNext();
        }

        public bool Contains(string file)
        {
            return entries.Contains(file);
        }

        public void Generate()
        {
            using (var bitmap = new Bitmap(Width, Height))
            using (var g = Graphics.FromImage(bitmap))
            {
                var enumerator = Tiles.GetEnumerator();
                foreach (var entry in entries)
                {
                    enumerator.MoveNext();

                    try
                    {
                        using (Stream stream = entry.StartsWith("http")
                                ? WebRequest.Create(new Uri(entry).AbsoluteUri).GetResponse().GetResponseStream()
                                : File.OpenRead(entry))
                        using (var img = Image.FromStream(stream))
                        {
                            int size;
                            if (img.Width > img.Height)
                                size = img.Height;
                            else
                                size = img.Width;

                            var srcRect = new Rectangle(
                                (img.Width - size) / 2,
                                (img.Height - size) / 2,
                                size, size
                            );

                            g.DrawImage(img, enumerator.Current, srcRect, GraphicsUnit.Pixel);
                        }
                        //using (var img = new MagickImage(entry))
                        //using (var cropped = GetCroppedImage(img)) {
                        //    g.DrawImageUnscaled(cropped, enumerator.Current);
                        //}

                    }
                    catch (ArgumentException e)
                    {
                        Konsole.Log("Invalid format! {0}", ConsoleColor.Red, entry);
                    }
                }
                bitmap.Save(path);
                Konsole.Log("Wrote atlas to {0}", ConsoleColor.Gray, path);
            }
        }


        public IMVR.Commons.AtlasTicket Add(string file)
        {
            if (IsFull) throw new Exception("Atlas is full!");

            var pos = tiles.Current.Location;
            IsFull = !tiles.MoveNext();

            entries.Add(file);

            return new IMVR.Commons.AtlasTicket {
                Atlas = Atlas,
                Position = tilesNo++
            };
        }

        public IMVR.Commons.AtlasTicket Add(byte[] contents)
        {
            // Create and write to temp file...
            var filename = Path.GetTempFileName();
            tempFiles.Add(filename);

            File.WriteAllBytes(filename, contents);


            return Add(filename);
        }


        /// <summary>
        /// Precondition: larger than TileSize
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private Bitmap GetCroppedImage(MagickImage img)
        {
            if (img.Width == TileSize && img.Height == TileSize) return img.ToBitmap();

            float factor;
            if (img.Width > img.Height)
                factor = (float)TileSize / img.Height;
            else
                factor = (float)TileSize / img.Width;

            img.Resize(img.Width * factor, img.Height * factor);
            var centroid = new PointD(img.Width / 2.0, img.Height / 2.0);//img.Moments().Composite().Centroid;

            int x, y, width, height;
            width = height = TileSize;
            if(img.Width > TileSize) {
                // Y is fixed
                y = 0;
                x = Math.Min(Math.Max((int)centroid.X - width / 2, 0), img.Width - width);
            }
            else
            {
                // X is fixed
                x = 0;
                y = Math.Min(Math.Max((int)centroid.Y - height / 2, 0), img.Height - height);
            }
            
            img.Crop(new MagickGeometry(x, y, width, height));
            return img.ToBitmap();
        }

        private IEnumerable<Rectangle> Tiles
        {
            get
            {
                
                for (int y = 0; y < Height / TileSize; y++)
                {
                    for (int x = 0; x < Width / TileSize; x++)
                    {
                        yield return new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                    }
                }
            }
        }


        public Commons.Atlas Atlas { get; set; }

        public void Dispose()
        {
            foreach (var path in tempFiles)
            {
                try
                {
                    File.Delete(path);
                }
                catch (UnauthorizedAccessException) { }
            }
        }
    }
}
