using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

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

    public static IEnumerable<Transform> AncestorsAndThis(this Transform node)
    {
        var parent = node;
        while (parent != null)
        {
            yield return parent;
            parent = parent.parent;
        }
    }

    public static IEnumerable<Transform> Ancestors(this Transform node)
    {
        return node.AncestorsAndThis().Skip(1);
    }


    public static string GetPath(this GameObject go)
    {
        return string.Join("/", go.transform.AncestorsAndThis().Select(a => a.name).Reverse().ToArray());
    }

    public static void SetActiveInHierarchy(this GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (var ancestor in obj.transform.Ancestors())
            ancestor.gameObject.SetActive(state);
    }


    public static Transform FindRecursively(this Transform node, string name)
    {
        return node.Descendants().FirstOrDefault(transform => transform.name == name);
    }

    public static Tweener Fade(this CanvasGroup group, float alpha, float duration)
    {
        group.DOKill(false);
        return group.DOFade(alpha, duration);
    }

    public static Vector3 To(this GameObject from, GameObject to)
    {
        return (to.transform.position - from.transform.position).normalized;
    }
}
