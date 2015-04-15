using DoctaJonez.Drawing.Imaging;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VirtualHands.Data.Image
{
    public class TextureAtlas
    {
        public Texture2D Texture { get; private set; }
        public const int TileSize = 128;
        public bool IsFull { get; private set; }
        private Dictionary<string, Rect> entries = new Dictionary<string,Rect>();
        private IEnumerator<Rect> tiles;

        private int jobs = 0;

        public TextureAtlas(int width, int height)
        {
            Texture = new Texture2D(width, height);
            tiles = Tiles.GetEnumerator();
            IsFull = !tiles.MoveNext();

            //Texture.filterMode = FilterMode.Trilinear;
            //Texture.anisoLevel = 0;
        }

        public bool Contains(string file)
        {
            return entries.ContainsKey(file);
        }

        private IEnumerable<Rect> Tiles
        {
            get
            {
                for (int x = 0; x < Texture.width / TileSize; x++)
                {
                    for (int y = 0; y < Texture.height / TileSize; y++)
                    {
                        yield return new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a sprite definition for a given file, adding it to the atlas if not existing.
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public Sprite GetSprite(string file)
        {
            if (!entries.ContainsKey(file))
            {
                // Gotta add this file
                var rect = entries[file] = tiles.Current;
                IsFull = !tiles.MoveNext();

                var job = new DeferredLoader.Job(file, Texture, rect.position, rect.size);
                jobs++;
                job.Done += JobDone;
                DeferredLoader.Instance.AddJob(job);
            }

            return Sprite.Create(Texture, entries[file], new Vector2(0.5f, 0.5f), TileSize, 0, SpriteMeshType.FullRect);
        }

        private void JobDone()
        {
            jobs--;

            if (jobs == 0)
            {
                Update();
            }
        }

        public void Update()
        {
            Texture.Apply();
        }
    }

    public class DeferredLoader : Singleton<DeferredLoader>
    {
        private const int WIDTH = 128;
        private const int HEIGHT = 128;
        private const float DELAY = 0.005f;
        private const int ATLAS_SIZE = 2048;

        private Queue<Job> jobs = new Queue<Job>();
        private List<TextureAtlas> atlases = new List<TextureAtlas>(
            new TextureAtlas[] {
                new TextureAtlas(ATLAS_SIZE, ATLAS_SIZE)
            }
        );
        void Start()
        {
            StartCoroutine(DoWork());
        }

        private IEnumerator DoWork()
        {
            while (true)
            {
                Job job = null;
                while ((job == null || job.Cancelled) && jobs.Count > 0)
                {
                    job = jobs.Dequeue();
                }
                if (job != null)
                {
                    var www = new WWW(new Uri(job.File).AbsoluteUri);
                    yield return www;
                    www.LoadImageIntoTexture(job.Texture);
                    job.Done();
                    

                    //// Found a job to take care of
                    //if (!System.IO.File.Exists(job.File)) continue;

                    //var task = Load(job.File);

                    //// Wait for completion
                    //yield return StartCoroutine(task.WaitRoutine());

                    //int done = 0;
                    //for (int x = 0; x < job.Texture.width; x++)
                    //    for (int y = 0; y < job.Texture.height; y++)
                    //    {
                    //        //var pxl = result.GetPixel(x, y);

                    //        job.Texture.SetPixel((int)job.Offset.x + x, (int)job.Offset.y + y, task.Result[x, y]);

                    //        if (done++ > 50000)
                    //        {
                    //            done = 0;
                    //            yield return 0;
                    //        }
                    //    }

                    //Debug.Log("DONE");
                    //job.Done();
                    //job.Texture.Apply(true);
                }
                //yield return new WaitForSeconds(DELAY);
                yield return 0;
            }
        }

        private Task<Color32[,]> Load(string file)
        {
            return Load(file, Vector2.zero);
        }
        private Task<Color32[,]> Load(string file, Vector2 targetSize)
        {
            return Task<Color32[,]>.Run(delegate
            {

                Color32[,] pixels;
                
                using (var img = System.Drawing.Image.FromFile(file))
                using (var result = targetSize == Vector2.zero
                            ? img as Bitmap
                            : ImageUtilities.ResizeImage(img, (int)targetSize.x, (int)targetSize.y))
                {
                    pixels = new Color32[result.Width, result.Height];


                    for (int x = 0; x < result.Width; x++)
                        for (int y = 0; y < result.Height; y++)
                        {
                            var pxl = result.GetPixel(x, y);

                            pixels[x, result.Height - y - 1] = new Color32(pxl.R, pxl.G, pxl.B, pxl.A);
                        }
                }
                
                return pixels;

            });
        }

        public void AddJob(Job job)
        {
            jobs.Enqueue(job);
        }

        public Sprite LoadSprite(string file)
        {
            var myAtlas = atlases.FirstOrDefault(a => a.Contains(file)) ?? atlases.Last();
            var sprite = myAtlas.GetSprite(file);

            if (atlases.Last().IsFull)
            {
                //atlases.Last().Update();
                atlases.Add(new TextureAtlas(ATLAS_SIZE, ATLAS_SIZE));

            }


            return sprite;
        }


        public Texture2D LoadTexture(string file, Action callback = null)
        {
            var existingJob = jobs.FirstOrDefault(j => j.File == file);

            if (existingJob != null)
            {
                existingJob.Cancelled = false;
                return existingJob.Texture;
            }

            int width, height;

            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (System.Drawing.Image tif = System.Drawing.Image.FromStream(stream: stream,
                                                    useEmbeddedColorManagement: false,
                                                    validateImageData: false))
                {
                     width = (int)tif.PhysicalDimension.Width;
                     height = (int)tif.PhysicalDimension.Height;
                }
            }


            var texture = new Texture2D(width, height);
            var job = new Job(file, texture);
            if(callback != null)
                job.Done += callback;

            jobs.Enqueue(job);

            return texture;
        }

        public void UnloadTexture(string file)
        {
            foreach( var job in jobs.Where(job => job.File == file) ) {
                job.Cancelled = true;
            }
        }


        public class Job
        {
            public string File { get; private set; }
            public Texture2D Texture { get; private set; }
            public Vector2 Offset { get; private set; }
            public Vector2 Size { get; set; }

            public bool Cancelled = false;

            public Action Done = delegate { };

            public Job(string file, Texture2D texture) : this(file, texture, new Vector2(0,0), new Vector2(WIDTH, HEIGHT))
            {
            }



            public Job(string file, Texture2D texture, Vector2 offset, Vector2 size)
            {
                File = file;
                Texture = texture;
                Offset = offset;
                Size = size;
            }

        }
    }

    enum LoaderPriority
    {
        Immediate,
        Preparation,
        Unimportant
    }
}
