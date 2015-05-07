using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using System.Linq;
using Gestures;

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
        Jukebox.Instance.Playlist.Shuffle = true;
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


    private Vector3 m_pullStartPosition;
    private bool m_pulling = false;
    // Update is called once per frame
    void Update()
    {
        var hand = HandProvider.Instance.GetHand(HandType.Left);
        m_pulling = m_pulling && hand != null;

        if(HandProvider.Instance.GetGestureEnter("Pull")) {
            m_pullStartPosition = hand.PalmPosition;
            m_pulling = true;
        }

        if (m_pulling)
        {
            var pullEndPosition = hand.PalmPosition;
            var axis = (Camera.main.transform.position - m_pullStartPosition).normalized;
            var distance = Vector3.Dot(pullEndPosition - m_pullStartPosition, axis);

            if (distance > 0.05f)
            {
                if (viewStack.Count > 1) {
                    GoBack();
                }
                m_pulling = false;
            }
        }

        if (HandProvider.Instance.GetGestureExit("Pull"))
        {
            m_pulling = false;
        }
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


    /// <summary>
    /// Steps into a view.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Navigate<T>() where T : View
    {
        var view = new GameObject().AddComponent<T>();
        ChangeView(view, false);

        return view;
    }

    /// <summary>
    /// Goes back a view.
    /// </summary>
    /// <returns></returns>
    public View GoBack()
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

    public void NavigateToOverview()
    {
        Navigate<ArtistOverView>();
    }
}