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
            StartCoroutine(HandlePull(0.05f));
        }

        if (HandProvider.Instance.GetGestureEnter("Push Down"))
        {
            Debug.Log("PUSH DOWN");
            StartCoroutine(HandlePushDown(0.05f));
        }

        if (HandProvider.Instance.GetGesture("Pull Up"))
        {
            Debug.Log("SHOW");
            foreach (var view in viewStack)
            {
                view.Enable();
            }
        }
    }


    private IEnumerator HandlePull(float requiredDistance)
    {
        m_pulling = true;

        var hand = HandProvider.Instance.GetHand(HandType.Left);
        var startPosition = hand.PalmPosition;

        while (HandProvider.Instance.GetGesture("Pull"))
        {
            hand = HandProvider.Instance.GetHand(HandType.Left);
            if (hand != null)
            {
                var pullEndPosition = hand.PalmPosition;
                var axis = (Camera.main.transform.position - startPosition).normalized;
                var distance = Vector3.Dot(pullEndPosition - startPosition, axis);

                if (distance > requiredDistance)
                {
                    if (viewStack.Count > 1)
                    {
                        GoBack();
                        break;
                    }
                }
            }
            else break;

            yield return null;
        }

        m_pulling = false;
    }

    private IEnumerator HandlePushDown(float requiredDistance)
    {
        var hand = HandProvider.Instance.GetHand(HandType.Left);
        var startPosition = hand.PalmPosition;

        while (HandProvider.Instance.GetGesture("Push Down"))
        {
            hand = HandProvider.Instance.GetHand(HandType.Left);
            if (hand != null)
            {
                var pullEndPosition = hand.PalmPosition;
                var axis = -Camera.main.transform.up;
                var distance = Vector3.Dot(pullEndPosition - startPosition, axis);

                if (distance > requiredDistance)
                {
                    Debug.Log("YAY");
                    foreach (var view in viewStack)
                        view.Disable();
                }
            }
            else
            {
                Debug.Log("LEAVE");
                break;
            }

            yield return null;
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

    public void NavigateToMetaGroup(MetaGroup group)
    {
        var view = Navigate<MetaGroupView>();
        view.group = group;
    }
    public void NavigateToMetaGroup(string group) {
        NavigateToMetaGroup((MetaGroup)System.Enum.Parse(typeof(MetaGroup), group, false));
    }

}