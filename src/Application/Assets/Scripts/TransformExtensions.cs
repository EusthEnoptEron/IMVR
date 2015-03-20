using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtensions {
    public static IEnumerable<Transform> Children(this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            yield return parent.GetChild(i);
        }
    }

    public static IEnumerable<Transform> Descendants(this Transform parent)
    {
        var queue = new Queue<Transform>();
        queue.Enqueue(parent);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            foreach (var child in current.Children())
            {
                queue.Enqueue(child);
                yield return child;
            }
        }
    }

}
