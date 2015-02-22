using UnityEngine;
using System.Collections;
using System.Linq;
using Foundation;
using System.IO;
using System;
using System.Collections.Generic;
using DoctaJonez.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Imaging;

public class ImageSource : IDataSource {

    ImageGrabber grabber = new ImageGrabber();
    public ImageSource(string root)
    {
        grabber.GrabImages(root);
    }

    public TileBuffer ReadForward()
    {
        Task previousTask = null;
        var buffer =  new TileBuffer(grabber.Take(10).Select(file =>
        {
            var tile = new GameObject().AddComponent<ImageTile>();
            //TaskManager.StartRoutine(LoadTexture(file, tile));
            var task = new Task(LoadTexture(file, tile));
            
            if(previousTask != null) {
                previousTask.ContinueWith(delegate {
                    task.Start();
                });
            } else {
                task.Start();
            }
            previousTask = task;

            return (Tile)tile;
        }));

        return buffer;
    }

    public TileBuffer ReadBackward()
    {
        throw new System.NotImplementedException();
    }

    public void Reset()
    {
    }

    private IEnumerator LoadTexture(FileInfo file, ImageTile tile)
    {
        yield return 0;

        byte[] data = new byte[0];
        int width, height;
        width = height = 256;
        Color32[,] pixels = new Color32[width, height];
        var texture = new Texture2D(width, height);


        Bitmap result = null;
        var loadTask = Task.Run(delegate
        {
            //using (var imgFile = new ImageMagick.MagickImage(file.FullName))
            //{
            //    var statistics = imgFile.Statistics();
            //}

            using(var img = System.Drawing.Image.FromFile(file.FullName)) {
                result = ImageUtilities.ResizeImage(img, width, height);
                 for (int x = 0; x < result.Width; x++)
                     for (int y = 0; y < result.Height; y++)
                     {
                         var pxl = result.GetPixel(x, y);

                         pixels[x, y] = new UnityEngine.Color(pxl.R / 255f, pxl.G / 255f, pxl.B / 255f);
                     }
                 result.Dispose();
            //using(var result = ImageUtilities.ResizeImage(img, 256, 256))
            //using(var stream = new MemoryStream()) {
            //    result.Save(stream, ImageFormat.Jpeg);
            //    data = stream.ToArray();
                //result.Save("E:\\tmp\\video\\" + file.Name, ImageFormat.Png);
                //File.WriteAllBytes("E:\\tmp\\video\\" + file.Name, data);
            }
        });

        yield return TaskManager.StartRoutine(loadTask.WaitRoutine());

        //File.WriteAllBytes("E:\\tmp\\video\\" + file.Name, data);

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
       



        int done = 0;
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
        {
            //var pxl = result.GetPixel(x, y);

            texture.SetPixel(x, y, pixels[x,y] );

            if (done++ > 50000)
            {
                done = 0;
                yield return 0;
            }
        }
        //Debug.Log("OK");
        watch.Start();
        result.Dispose();
        //texture.LoadImage(data);
        //yield return 0;
        texture.Apply(true);
        watch.Stop();

        float size;
        if (texture.width > texture.height)
        {
            size = texture.height;
        } else {
            size = texture.width;
        }
        //UnityEngine.Debug.Log(String.Format("{0} ({3}) ({1}/{2})", watch.ElapsedMilliseconds, texture.width, texture.height, file.Extension));


        var sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        tile.sprite = sprite;
    }


    public void Dispose()
    {
        grabber.Stop();
    }
}
