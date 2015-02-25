using DoctaJonez.Drawing.Imaging;
using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VirtualHands.Data.Image
{
    public class DeferredLoader : Singleton<DeferredLoader>
    {
        private const int WIDTH = 128;
        private const int HEIGHT = 128;
        private const float DELAY = 0.01f;

        private Queue<Job> jobs = new Queue<Job>();

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
                    // Found a job to take care of

                    var task = Load(job.File);

                    // Wait for completion
                    yield return StartCoroutine(task.WaitRoutine());

                    int done = 0;
                    for (int x = 0; x < WIDTH; x++)
                        for (int y = 0; y < HEIGHT; y++)
                        {
                            //var pxl = result.GetPixel(x, y);

                            job.Texture.SetPixel(x, y, task.Result[x, y]);

                            if (done++ > 50000)
                            {
                                done = 0;
                                yield return 0;
                            }
                        }

                    job.Texture.Apply(true);
                }
                yield return new WaitForSeconds(DELAY);
            }
        }

        private Task<Color32[,]> Load(string file)
        {
            return Task<Color32[,]>.Run(delegate
            {
                Color32[,] pixels = new Color32[WIDTH, HEIGHT];

                using (var img = System.Drawing.Image.FromFile(file))
                using (var result = ImageUtilities.ResizeImage(img, WIDTH, HEIGHT))
                {
                    for (int x = 0; x < result.Width; x++)
                        for (int y = 0; y < result.Height; y++)
                        {
                            var pxl = result.GetPixel(x, y);

                            pixels[x, HEIGHT - y - 1] = new Color32(pxl.R, pxl.G, pxl.B, pxl.A);
                        }
                }
                
                return pixels;

            });
        }


        public Texture2D LoadTexture(string file)
        {
            var existingJob = jobs.FirstOrDefault(job => job.File == file);

            if (existingJob != null)
            {
                existingJob.Cancelled = false;
                return existingJob.Texture;
            }

            var texture = new Texture2D(WIDTH, HEIGHT);

            jobs.Enqueue(new Job(file, texture));
            
            return texture;
        }

        public void UnloadTexture(string file)
        {
            foreach( var job in jobs.Where(job => job.File == file) ) {
                job.Cancelled = true;
            }
        }


        private class Job
        {
            public string File { get; private set; }
            public Texture2D Texture { get; private set; }
            public bool Cancelled = false;

            public Job(string file, Texture2D texture)
            {
                File = file;
                Texture = texture;
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
