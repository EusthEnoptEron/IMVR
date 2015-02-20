using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;


/// <summary>
/// Helper class for I/O operations
/// </summary>
public static class IO {


    /// <summary>
    /// Returns whether or not the file is an image file.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <remarks>Needs overhaul.</remarks>
    public static bool IsImage(FileInfo file)
    {
        Debug.Log("Judge");
        switch (file.Extension)
        {
            case ".jpg":
            case ".png":
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Performs a simple BFS for files on the system.
    /// </summary>
    /// <param name="location"></param>
    /// <returns>Enumerator for files.</returns>
    public static IEnumerable<FileInfo> GetFiles(DirectoryInfo location)
    {
        Queue<DirectoryInfo> queue = new Queue<DirectoryInfo>();
        queue.Enqueue(location);

        while (queue.Count > 0)
        {
            var cwd = queue.Dequeue();

            if (!cwd.Exists) continue;

            FileInfo[] files;

            try {
                files = cwd.GetFiles();
            } catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
                continue;
            }

            // Return files
            foreach (var file in files) yield return file;

            // Enqueue child dirs
            foreach (var directory in cwd.GetDirectories()) queue.Enqueue(directory);

        }
    }

    public static IEnumerable<FileInfo> GetFiles(string location)
    {
        return GetFiles(new DirectoryInfo(location));
    }

   
}
