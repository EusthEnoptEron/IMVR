using UnityEngine;
using System.Collections;
using Foundation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading;

public class ImageGrabber
{
    private bool stopped = false;
    private FileSystemWatcher watcher;
    private Queue<FileInfo> images = new Queue<FileInfo>();
    private object lockObj = new object();


    public ImageGrabber()
    {
        //watcher = new FileSystemWatcher("E:\\");
        //watcher.Created += ImageGrabber_Created;
        //watcher.EnableRaisingEvents = true;
        //watcher.IncludeSubdirectories = true;
    }

    public void GrabImages(string location)
    {
        Task.Run(delegate
        {
            Debug.Log("start");

            var images = IO.GetFiles(new DirectoryInfo(location)).Where(IO.IsImage);

            foreach (var img in images)
            {
                if (stopped) return;

                lock (lockObj)
                {
                    this.images.Enqueue(img);
                }
            }
        });
    }

    public void Stop()
    {
        stopped = true;
    }

    public IEnumerable<FileInfo> Take(int number)
    {
        var infos = new List<FileInfo>();
        lock (lockObj)
        {
            while(infos.Count < number && images.Count > 0)
            {
                infos.Add(images.Dequeue());
            }
        }
        return infos;
    }
}
