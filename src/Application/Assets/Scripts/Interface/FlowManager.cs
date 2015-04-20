using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlowManager : Singleton<FlowManager>
{
    private Stack<View> viewStack;

    public View ActiveView
    {
        get
        {
            return viewStack.Count > 0
                ? viewStack.Peek()
                : null;
        }

    }

    void Awake()
    {
        viewStack.Push(null);
    }

    // Use this for initialization
    void Start()
    {
        ChangeView<ArtistOverView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeView(View view, bool pop = true)
    {

        var activeView = pop
            ? viewStack.Pop()
            : ActiveView;

        if (activeView != null)
        {
            activeView.enabled = false;
        }

        activeView = view;
        viewStack.Push(activeView);

        activeView.enabled = true;
    }

    public T ChangeView<T>() where T : View
    {
        var view = new GameObject().AddComponent<T>();
        ChangeView(view);

        return view;
    }


    public T PushView<T>() where T : View
    {
        var view = new GameObject().AddComponent<T>();
        ChangeView(view, false);

        return view;
    }
}