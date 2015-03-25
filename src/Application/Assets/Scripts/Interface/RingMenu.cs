using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RingMenu : MonoBehaviour, IRingMenu {
    /// <summary>
    /// The hand to which this ring menu applies to.
    /// </summary>
    public HandType handType = HandType.Left;
    public GameObject particleEffect;
    public float particleScale = 1f;

    private bool activated = true;
    private CanvasGroup canvasGroup;
    public IDictionary<FingerType, RingMenuItem> Items { get; private set; }

    private IRingMenu _activeMenu;
    public IRingMenu ActiveMenu
    {
        get { return _activeMenu ?? this; }
        set
        {
            foreach (var child in ActiveMenu.Items.Values)
                child.SetVisibility(false);

            var self = ActiveMenu.Node.GetComponent<RingMenuItem>();
            if (self != null) self.SetVisibility(false);

            _activeMenu = value;
            ActiveMenu.Node.SetActiveInHierarchy(true);

            foreach (var ancestor in ActiveMenu.Node.GetComponentsInParent<RingMenuItem>())
                ancestor.SetVisibility(true);

            foreach (var child in ActiveMenu.Items.Values)
            {
                child.SetVisibility(true);
            }
        }
    }

    public int Level
    {
        get;
        private set;
    }

    private FingerType? submitCandidate;
    private float submitDelta = 0;

	// Use this for initialization
	void Start () {
        this.canvasGroup = GetComponent<CanvasGroup>();

        Items = new Dictionary<FingerType, RingMenuItem>();
        // Fill list of items
        foreach (var child in transform.Children())
        {
            var item = child.GetComponent<RingMenuItem>();
            if (item != null)
                Items.Add(item.fingerType, item);
        }

        ActiveMenu = null;
	}

    public void ChangeMenu(Transform activeMenu)
    {

    }
	
	// Update is called once per frame
	void Update () {
        var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
        if (hand != null)
        {
            if ((activated && ShouldMaintainMenu(hand)) 
                || ShouldShowMenu(hand))
            {
                UpdateOrders();
                UpdateHand(hand);
                SetState(true);

                UpdatePosition(hand);
            }
            else
            {
                SetState(false);
            }
        }
        else
        {
            SetState(false);
        }
	}

    bool ShouldShowMenu(GenericHand hand)
    {
        return ShouldMaintainMenu(hand) && hand.Fingers.All(f => f.Extended);
    }

    bool ShouldMaintainMenu(GenericHand hand)
    {
        return !HandProvider.Instance.GetGesture("Pull")
            && Vector3.Dot(hand.LocalPalmNormal, new Vector3(1, 1, -1).normalized) > 0;        
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

            UpdateCandidate(hand);

            foreach (var finger in hand.Fingers)
            {
                UpdateFinger(finger.Type, finger.Type == submitCandidate ? submitDelta : 0);
            }
        }
    }

    void UpdatePosition(GenericHand hand)
    {
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
            activated = enabled;

            if (!activated) ActiveMenu = null;

            canvasGroup.DOKill();
            var animation = canvasGroup.DOFade(activated ? 1 : 0, 0.5f);

            foreach (var child in ActiveMenu.Items.Values)
            {
                child.SetVisibility(enabled);
            }
            //if (activated) SetChildren();
            //else animation.OnComplete(SetChildren);
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



    public GameObject Node
    {
        get { return gameObject; }
    }


}

