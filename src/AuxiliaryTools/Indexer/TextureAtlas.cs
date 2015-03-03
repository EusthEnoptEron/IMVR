using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMagick;

namespace Indexer
{
    public class TextureAtlas
    {

        public const int TileSize = 128;
        
        public bool IsFull { get; private set; }
        private List<string> entries = new List<string>();

        private IEnumerator<Rectangle> tiles;
        
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TextureAtlas(int width, int height)
        {
            Width = width;
            Height = height;
            tiles = Tiles.GetEnumerator();
            IsFull = !tiles.MoveNext();
            //Texture.filterMode = FilterMode.Trilinear;
            //Texture.anisoLevel = 0;
        }

        public bool Contains(string file)
        {
            return entries.Contains(file);
        }

        public void Generate(string path)
        {
            using (var bitmap = new Bitmap(Width, Height))
            using (var g = Graphics.FromImage(bitmap))
            {
                var enumerator = Tiles.GetEnumerator();
                foreach (var entry in entries)
                {
                    enumerator.MoveNext();

                    using (var img = new MagickImage(entry))
                    using (var cropped = GetCroppedImage(img)) {
                        g.DrawImageUnscaled(cropped, enumerator.Current);
                    }
                }
                bitmap.Save(path);
            }
        }

        public void Add(string file)
        {
            if (IsFull) throw new Exception("Atlas is full!");
            IsFull = !tiles.MoveNext();

            entries.Add(file);
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
            var centroid = img.Moments().Composite().Centroid;

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
                for (int x = 0; x < Width / TileSize; x++)
                {
                    for (int y = 0; y < Height / TileSize; y++)
                    {
                        yield return new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize);
                    }
                }
            }
        }

    }
}
