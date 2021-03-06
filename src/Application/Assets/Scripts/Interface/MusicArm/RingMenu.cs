﻿using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RingMenu : Singleton<RingMenu>, IRingMenu {
    /// <summary>
    /// The hand to which this ring menu applies to.
    /// </summary>
    public HandType handType = HandType.Left;
    public GameObject particleEffect;
    public float particleScale = 1f;

    private bool activated = true;
    private CanvasGroup canvasGroup;
    public IDictionary<FingerType, RingMenuItem> Items { get; private set; }

    private GameObject _stack;

    private IRingMenu _activeMenu;
    private bool _submitted = false;

    [SerializeField]
    private Sprite _sprite;

    /// <summary>
    /// Gets whether or not the menu is currently opened.
    /// </summary>
    public bool IsOpened
    {
        get
        {
            return activated;
        }
    }


    public IRingMenu ActiveMenu
    {
        get {
            return _activeMenu ?? this;
        
        }
        set
        {
            if (!IsOpened) return;
            // Active menu changed!
            //----------------------

            // Hide all items of the *currently active* menu
            foreach (var child in ActiveMenu.Items.Values)
                child.SetVisibility(false);

            // Hide the currently active menu itself
            var self = ActiveMenu.Node.GetComponent<RingMenuItem>();
            if (self != null) self.SetVisibility(false);

            // Swap
            _activeMenu = value;
            ActiveMenu.Node.gameObject.SetActiveInHierarchy(true);

            //// Make all ancestors that must be visible visible (they will appear stacked on the palm)
            //foreach (var ancestor in ActiveMenu.Node.GetComponentsInParent<RingMenuItem>())
            //    ancestor.SetVisibility(true);

            // Make direct children visible
            foreach (var child in ActiveMenu.Items.Values)
            {
                child.SetVisibility(true);
            }

            OnChangeMenu();
        }
    }


    /// <summary>
    /// Gets or sets the entry point when opening the ring menu.
    /// </summary>
    public IRingMenu EntryPoint;

    public int Level
    {
        get;
        private set;
    }

    private FingerType? submitCandidate;
    private float submitDelta = 0;


    public ToggleGroup SelectionGroup;

    void Awake()
    {
        SelectionGroup = new GameObject().AddComponent<ToggleGroup>();
        SelectionGroup.transform.SetParent(transform, false);
        SelectionGroup.allowSwitchOff = true;
        EntryPoint = this;
    }

	// Use this for initialization
	void Start () {
        _stack = new GameObject("Stack");
        _stack.transform.SetParent(transform.parent);

        this.canvasGroup = GetComponent<CanvasGroup>();

        var playMenu = RingMenuBuilder.CreateItem(FingerType.Index, "Play", this);

        RingMenuBuilder.CreateItem(FingerType.Middle, "Sound Settings", this);
        var shuffleMenu = RingMenuBuilder.CreateItem(FingerType.Ring, "Shuffle", this);

        playMenu.OnClick.AddListener(OnPlay);
        shuffleMenu.OnClick.AddListener(OnShuffle);

        UpdateItems();

        ActiveMenu = null;

	}

    private void OnShuffle()
    {
        Jukebox.Instance.Playlist.ShuffleSongs();
    }

    private void OnPlay()
    {
        if (Jukebox.Instance.IsPlaying)
        {
            Jukebox.Instance.Pause();
        }
        else
        {
            Jukebox.Instance.Play();
        }
    }

	
	// Update is called once per frame
	void Update () {
 

        var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
        if (hand != null)
        {
            if ((activated && ShouldMaintainMenu(hand)) 
                || ShouldShowMenu(hand))
            {
            //    UpdateOrders();
                UpdateHand(hand);
                SetState(true);
            }
            else
            {
                SetState(false);
            }
            UpdatePosition(hand);

        }
        else
        {
            SetState(false);
        }
	}

    bool ShouldShowMenu(GenericHand hand)
    {
        return ShouldMaintainMenu(hand) 
            && hand.Fingers.All(f => f.Extended);
    }

    bool ShouldMaintainMenu(GenericHand hand)
    {
        return !HandProvider.Instance.GetGesture("Pull")
            //&& Vector3.Dot(hand.LocalPalmNormal, new Vector3(1, 1, -1).normalized) > 0;
            && Vector3.Dot(hand.PalmNormal, Camera.main.transform.forward) < -0.5f;
    }

    void UpdateHand(GenericHand hand)
    {
        if (activated)
        {
            List<GenericFinger> fingers = new List<GenericFinger>();

            foreach (var finger in hand.Fingers)
            {
                float dotProduct;
                if (finger.Type == FingerType.Thumb)
                    dotProduct = Vector3.Dot(Vector3.Cross(hand.LocalPalmDirection, hand.LocalPalmNormal).normalized, finger.LocalDirection);
                else
                    dotProduct = Vector3.Dot(hand.LocalPalmDirection, finger.LocalDirection);

                if (dotProduct < 0.3f)
                {
                    fingers.Add(finger);
                }
            }

            _submitted = _submitted && fingers.Count > 0;

            // Do nothing while the user hasn't returned his submitted finger!
            if (!_submitted)
            {

                if (fingers.Count == 1)
                {
                    if (submitCandidate == null || submitCandidate != fingers[0].Type)
                    {
                        if (submitCandidate != null) ExecuteEvent(submitCandidate.Value, ExecuteEvents.pointerUpHandler);

                        submitCandidate = fingers[0].Type;
                        submitDelta = 0;

                        ExecuteEvent(submitCandidate.Value, ExecuteEvents.pointerDownHandler);
                    }
                }
                else
                {
                    if (submitCandidate != null) ExecuteEvent(submitCandidate.Value, ExecuteEvents.pointerUpHandler);
                    submitCandidate = null;
                }
            }

            UpdateCandidate(hand);

            foreach (var finger in hand.Fingers)
            {
                UpdateFinger(finger.Type, finger.Type == submitCandidate ? submitDelta : 0);
            }
        }
    }

    void UpdatePosition(GenericHand hand)
    {
        _stack.transform.position = hand.PalmPosition;
        _stack.transform.rotation = Quaternion.LookRotation(-hand.PalmNormal,
            Vector3.Cross(-hand.PalmNormal, hand.PalmDirection).normalized);
        //transform.position = hand.PalmPosition + Camera.main.transform.TransformDirection(Vector3.right * 0.2f);
    }

    void ExecuteEvent<T>(FingerType type, ExecuteEvents.EventFunction<T> handler) where T : IEventSystemHandler
    {
        if(ActiveMenu.Items.ContainsKey(type)) {
            ExecuteEvents.ExecuteHierarchy(
                ActiveMenu.Items[type].gameObject, 
                new PointerEventData(EventSystem.current),
                handler  
            );
        }
    }


    void UpdateFinger(FingerType type, float progress)
    {
        if (ActiveMenu.Items.ContainsKey(type))
        {
            ActiveMenu.Items[type].Progress = progress;
        }
    }


    void UpdateCandidate(GenericHand hand)
    {
        if (submitCandidate != null)
        {
            submitDelta += Time.deltaTime;
            if (submitDelta > 1f)
            {
                _submitted = true;
                // Submit!
                ExecuteEvent(submitCandidate.Value, ExecuteEvents.pointerClickHandler);
                submitCandidate = null;

                if (particleEffect != null)
                {
                    var effect = GameObject.Instantiate(particleEffect, hand.PalmPosition, Quaternion.identity) as GameObject;
                    
                    foreach (var particle in effect.GetComponentsInChildren<ParticleSystem>())
                    {
                        particle.startSize *= particleScale;
                        particle.startSpeed *= particleScale;
                        particle.startRotation *= particleScale;
                        particle.transform.localScale *= particleScale;
                    }

                    effect.SetActive(true);
                    GameObject.Destroy(effect, 10);
                }

            }
        }
    }


    private void SetState(bool enabled)
    {
        if (activated != enabled)
        {
            if (!enabled) ActiveMenu = null;

            activated = enabled;

            if(enabled) ActiveMenu = EntryPoint;


            Fade(activated ? 1 : 0, 0.5f);

            // Make all ancestors that must be visible visible (they will appear stacked on the palm)
            //foreach (var ancestor in ActiveMenu.Node.GetComponentsInParent<RingMenuItem>())
            //    ancestor.SetVisibility(enabled);

            foreach (var child in ActiveMenu.Items.Values)
            {
                child.SetVisibility(enabled);
            }
            //if (activated) SetChildren();
            //else animation.OnComplete(SetChildren);
        }
    }

    private void Fade(float target, float time)
    {
        foreach (var group in transform.GetComponentsInChildren<CanvasGroup>())
        {
            group.DOFade(target, time);
        }

        foreach (var group in _stack.GetComponentsInChildren<CanvasGroup>())
        {
            group.DOFade(target, time);
        }
    }

    private void SetChildren()
    {
        foreach (var child in transform.Children()) child.gameObject.SetActive(activated);
    }


    private void UpdateOrders()
    {
        var orderedChild = ActiveMenu.Items.Values
            .OrderBy(child => Vector3.Distance(Camera.main.transform.position, child.transform.position));


        foreach (var child in orderedChild)
            child.transform.SetAsFirstSibling();
    }



    public Transform Node
    {
        get { return transform; }
    }




    public void Clear()
    {
        foreach(var item in Items.Values) {
            DestroyImmediate(item.gameObject);
        }
        Items.Clear();
    }


    public void UpdateItems()
    {
        Items = new Dictionary<FingerType, RingMenuItem>();
        // Fill list of items
        foreach (var child in transform.Children())
        {
            //child.gameObject.SetActive(ActiveMenu == this && activated);
            var item = child.GetComponent<RingMenuItem>();
            if (item != null)
            {
                Items.Add(item.fingerType, item);
                item.SetVisibility(ActiveMenu == this && activated);
            }
        }


    }
    
    public Transform ItemNode
    {
        get { return Node; }
    }

    internal void GoBack(bool asEntryPoint = false)
    {
        if (ActiveMenu != null)
        {
            // Skip one because this will also get the current menu itself
            Navigate(ActiveMenu.Node.parent.GetComponentInParent<IRingMenu>(), asEntryPoint);
        }
    }



    internal void RemoveItem(FingerType fingerType)
    {
        RingMenuItem item;

        if (Items.TryGetValue(fingerType, out item))
        {
            Remove(item);
        }
    }

    public void Remove(RingMenuItem item)
    {
        if (item)
        {
            var menu = item.transform.parent.GetComponentInParent<IRingMenu>();

            // Make sure the menu isn't active
            if (ActiveMenu.Node.IsChildOf(item.transform))
                ActiveMenu = menu;

            if (EntryPoint != null && EntryPoint.Node.IsChildOf(item.transform))
            {
                EntryPoint = menu;

            }

            GameObject.DestroyImmediate(item.gameObject);

            menu.UpdateItems();
        }
        else
        {
            ActiveMenu = null;
            EntryPoint = this;
        }
      
    }

    public void Navigate(IRingMenu menu, bool asEntryPoint = false)
    {
        StartCoroutine(DeferredNavigate(menu, asEntryPoint));
    }

    private IEnumerator DeferredNavigate(IRingMenu menu, bool asEntryPoint = false)
    {
        yield return null;

        if(IsOpened)
            ActiveMenu = menu;
        if (asEntryPoint)
            EntryPoint = menu;

    }


    private void OnChangeMenu()
    {
        BuildStack();
    }

    private void BuildStack()
    {
        // Clean up
        foreach (var child in _stack.transform.Children())
            Destroy(child.gameObject);

        var menus = ActiveMenu.Node.GetComponentsInParent<IRingMenu>().Reverse().ToArray();
        for (int i = 0; i < menus.Length; i++)
        {
            var imgContainer = Instantiate(Resources.Load<GameObject>("Prefabs/UI/pref_PalmThumbnail")).GetComponent<RectTransform>();
            var imgText = imgContainer.GetComponentInChildren<Text>();
            var img = imgContainer.GetComponentInChildren<Image>();

            //var img = new GameObject().AddComponent<SpriteRenderer>();
            imgContainer.transform.SetParent(_stack.transform, false);
            img.sprite = menus[i].Thumbnail;
            imgText.text = menus[i].InfoText;

            imgContainer.transform.localScale =
                Vector3.one / (imgContainer.rect.width) * 0.1f;
            imgContainer.transform.localPosition += Vector3.back * ((i + 1) * 0.02f);
            imgContainer.transform.localRotation = Quaternion.Euler(0, 0, -30);
        }
    }


    public bool Exists
    {
        get { return this != null; }
    }


    public Sprite Thumbnail
    {
        get { return _sprite; }
        set { _sprite = value; }
    }

    public string InfoText
    {
        get;
        set;
    }
}

