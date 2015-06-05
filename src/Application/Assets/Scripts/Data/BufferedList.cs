using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;

public class BufferedList : MonoBehaviour {

    public class LazyGameObject {
        public delegate GameObject InitializeAction();

        private InitializeAction _initializor;
        private GameObject _go;
        public LazyGameObject(InitializeAction initializor)
        {
            _initializor = initializor;
        }

        public GameObject GameObject
        {
            get
            {
                if (!_go)
                {
                    _go = _initializor();
                }
                return _go;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _go;
            }
        }
    }

    public VerticalLayoutGroup layoutGroup;
    public ScrollRect scrollRect;
    public float lineHeight = 20;

    private FieldInfo contentBoundsInfo = typeof(ScrollRect).GetField("m_ContentBounds", BindingFlags.NonPublic | BindingFlags.Instance);
    private FieldInfo viewBoundsInfo = typeof(ScrollRect).GetField("m_ViewBounds", BindingFlags.NonPublic | BindingFlags.Instance);

    private List<LazyGameObject> gameObjects = new List<LazyGameObject>();

    private bool _rebuilding = false;
    private GameObject _trash;
    private void Awake()
    {
        scrollRect.onValueChanged.AddListener(OnScroll);
        _trash = new GameObject("Trash bin");
        _trash.SetActive(false);
    }

    private void OnScroll(Vector2 pos)
    {
        Rebuild();
    }

    private void Update()
    {
        _rebuilding = false;
    }

    private void Rebuild()
    {
        if (_rebuilding) return;

        _rebuilding = true;

        //var offset = CalculateOffset(Vector2.zero);
        var offset = Vector2.zero;
        var m_ContentBounds = (Bounds)contentBoundsInfo.GetValue(scrollRect);
        var m_ViewBounds = (Bounds)viewBoundsInfo.GetValue(scrollRect);

        //Debug.LogFormat("Bounds Content: {0}", m_ContentBounds);
        //Debug.LogFormat("Bounds View: {0}", m_ViewBounds);

        //Debug.Log();

        offset.y = (m_ContentBounds.max.y) - m_ViewBounds.max.y;
        //Debug.Log(offset.y);
        //offset.y += (m_ContentBounds.size.y - m_ViewBounds.size.y) * (1-scrollRect.normalizedPosition.y);

        //Debug.Log("OFFSET " + offset);
        Vector2 bufferedOffset = new Vector2(0, 0);
        float validPos = 0;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            float pos = i * lineHeight;

            if (pos < (offset.y - lineHeight) )
            {
                SetActiveSub(gameObjects[i], false);
                bufferedOffset.x = pos + lineHeight;
            }
            else if ((pos + lineHeight) > offset.y + m_ViewBounds.size.y + lineHeight)
            {
                SetActiveSub(gameObjects[i], false);
                bufferedOffset.y = pos - validPos + lineHeight;
            }
            else
            {
                //Debug.Log(gameObjects[i].name + " ["+ pos +"]");
                SetActiveSub(gameObjects[i], true);

                validPos = pos;
            }
        }

        var padding = layoutGroup.padding;
        padding.top = Mathf.RoundToInt(bufferedOffset.x);
        padding.bottom = Mathf.RoundToInt(bufferedOffset.y);
        layoutGroup.padding = padding;
    }

    private void SetActiveSub(LazyGameObject lgo, bool state)
    {
        if (!lgo.IsInitialized && !state) return;

        var go = lgo.GameObject;
        var parent = state ? layoutGroup.transform : _trash.transform;
        if (go.transform.parent != parent)
        {
            if (state)
            {
                go.transform.SetParent(layoutGroup.transform, false);
            }
            else
            {
                go.transform.SetParent(_trash.transform, false);
            }
        }
               
        if(state)
            go.transform.SetAsLastSibling();
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
        var m_ContentBounds = (Bounds)contentBoundsInfo.GetValue(scrollRect);
        var m_ViewBounds = (Bounds)viewBoundsInfo.GetValue(scrollRect);

        //Debug.LogFormat("Bounds Content: {0}", m_ContentBounds);
        //Debug.LogFormat("Bounds View: {0}", m_ViewBounds);


        Vector2 offset = Vector2.zero;
        if (scrollRect.movementType == ScrollRect.MovementType.Unrestricted)
            return offset;

        Vector2 min = m_ContentBounds.min;
        Vector2 max = m_ContentBounds.max;

        if (scrollRect.horizontal)
        {
            min.x += delta.x;
            max.x += delta.x;
            if (min.x > m_ViewBounds.min.x)
                offset.x = m_ViewBounds.min.x - min.x;
            else if (max.x < m_ViewBounds.max.x)
                offset.x = m_ViewBounds.max.x - max.x;
        }

        if (scrollRect.vertical)
        {
            min.y += delta.y;
            max.y += delta.y;
            //Debug.LogFormat("Max Content: {0}, Max View: {1}", max.y, m_ViewBounds.max.y);
            //Debug.LogFormat("Min Content: {0}, Min View: {1}", min.y, m_ViewBounds.min.y);
            if (max.y < m_ViewBounds.max.y)
                offset.y = m_ViewBounds.max.y - max.y;
            else if (min.y > m_ViewBounds.min.y)
                offset.y = m_ViewBounds.min.y - min.y;
        }

        return offset;
    }

    public void AddItems(IEnumerable<LazyGameObject> objects)
    {
        // Make new children with the power of love
        gameObjects.AddRange(objects);
    }

    public void Clear()
    {
        // Kill all children in a socially sustainable way
        gameObjects.Where(go => go.IsInitialized).Select(go => go.GameObject).ToList().ForEach(Destroy);
        gameObjects.Clear();
    }
}
