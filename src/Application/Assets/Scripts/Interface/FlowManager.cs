using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using System.Linq;

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
        viewStack = new Stack<View>();
        viewStack.Push(null);

        StartCoroutine(PlayDebug());
    }

    IEnumerator PlayDebug()
    {
        var db = IMDB.FromFile(Prefs.Instance.DBPath);
        Jukebox.Instance.Playlist.Add(db.Songs);
        Jukebox.Instance.Playlist.Cyclic = true;
        Jukebox.Instance.Play();
        yield return null;
        //yield return new WaitForSeconds(2);
        //Jukebox.Instance.Pause();
        //yield return new WaitForSeconds(2);
        //Jukebox.Instance.Play();
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
            if (pop)
                activeView.Disable();
            else
                PushStack();
        }

        activeView = view;
        viewStack.Push(activeView);

        activeView.Enable();
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

    public View PopView()
    {
        var view = viewStack.Pop();
        view.Disable();

        PullStack();

        return view;
    }

    private void PushStack()
    {
        foreach (var view in viewStack.ToArray())
            view.Push();
    }

    private void PullStack()
    {
        foreach (var view in viewStack.ToArray())
            view.Pull();
    }
}