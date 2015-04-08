﻿using UnityEngine;
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

    public static void SetActiveInHierarchy(this GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (var ancestor in obj.transform.Ancestors())
            ancestor.gameObject.SetActive(state);
    }

    public static IEnumerable<Transform> Ancestors(this Transform node)
    {
        var parent = node.parent;
        while (parent != null)
        {
            yield return parent;
            parent = parent.parent;
        }
    }
}