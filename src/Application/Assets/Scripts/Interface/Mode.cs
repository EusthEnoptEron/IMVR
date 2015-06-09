using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IMVR.Commons;
using System.Linq;
using Gestures;

public abstract class Mode : MonoBehaviour
{
    private Stack<View> viewStack;
    public Theme Theme { get; protected set; }

    public View ActiveView
    {
        get
        {
            return viewStack.Count > 0
                ? viewStack.Peek()
                : null;
        }
    }

    protected virtual void Awake()
    {
        viewStack = new Stack<View>();
        viewStack.Push(null);

    }


    // Use this for initialization
    protected abstract void Start();

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
            StartCoroutine(HandlePushDown(0.05f));
        }

        if (HandProvider.Instance.GetGesture("Pull Up"))
        {
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
                    foreach (var view in viewStack)
                        view.Disable();
                }
            }
            else
            {
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

        OnViewChanged();

    }

    public T ChangeView<T>() where T : View
    {
        var view = new GameObject().AddComponent<T>();
        ChangeView(view);

        return view;
    }


    private void OnDisable()
    {
        foreach (var view in viewStack.Where(v => v != null))
            view.Disable();
    }

    private void OnEnable()
    {
        foreach (var view in viewStack.Where(v => v != null))
            view.Enable();

        if(ActiveView != null)
            OnViewChanged();
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
    /// <returns>The view that was hidden.</returns>
    public View GoBack()
    {
        var view = viewStack.Pop();
        view.Disable();

        PullStack();
        OnViewChanged();

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


    private void OnViewChanged()
    {
        StartCoroutine(BuildMenu());
    }

    private IEnumerator BuildMenu()
    {

        yield return null;

        var menu = RingMenu.Instance;
        menu.RemoveItem(FingerType.Pinky);

        ActiveView.BuildMenu(menu);

        menu.UpdateItems();

    }
}