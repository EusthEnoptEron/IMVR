using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using Gestures;
using System.Collections.Generic;
using UnityEngine.UI;

public class RingSubMenu : RingMenuItem, IPointerClickHandler, IRingMenu {
    [SerializeField]
    private Sprite _thumbnail;
    private RingMenu menu;
    private bool _destroying = false;

    /// <summary>
    /// Gets called immediately after initialization and makes sure ItemNode is set.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        Items = new Dictionary<FingerType, RingMenuItem>();

        ItemNode = transform.Children().FirstOrDefault(child => child.name.ToLower() == "menu");
    }

    protected void Start()
    {
        menu = transform.GetComponentInParent<RingMenu>();
        Level = transform.Ancestors().Count(parent => parent.GetComponent<RingSubMenu>() != null) + 1;

        if (ItemNode != null)
        {
            // Fill list of items
            UpdateItems();
        }
    }


    protected override void Update()
    {
        base.Update();

        if (menu.ActiveMenu == this || menu.ActiveMenu.Level > Level)
        {
            var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
            if (hand != null)
            {
                transform.position = hand.PalmPosition + hand.PalmNormal * 0.025f * Level;//hand.PalmPosition + Vector3.ProjectOnPlane(finger.GetBone(BoneType.Intermediate).Position - hand.PalmPosition, Camera.main.transform.forward) * 2f;// + Camera.main.transform.TransformDirection(distance);
                transform.rotation = Quaternion.LookRotation(-hand.PalmNormal, hand.PalmDirection);
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(menu != null) {
            menu.ActiveMenu = this;
        }
        Progress = 0;
    }

    public int Level
    {
        get;
        private set;
    }


    public IDictionary<FingerType, RingMenuItem> Items
    {
        get;
        private set;
    }

    public Transform Node
    {
        get { return transform; }
    }


    public void Clear()
    {
        foreach (var item in Items.Values)
        {
            DestroyImmediate(item.gameObject);
        }
        Items.Clear();
    }


    public void UpdateItems()
    {
        Items = new Dictionary<FingerType, RingMenuItem>();
        // Fill list of items
        foreach (var child in ItemNode.Children())
        {
            //child.gameObject.SetActive(menu.ActiveMenu == this);
            var item = child.GetComponent<RingMenuItem>();
            if (item != null)
            {
                Items.Add(item.fingerType, item);

                item.SetVisibility(menu.ActiveMenu == this);
            }
        }
    }


    public Transform ItemNode
    {
        get;
        private set;
    }
    public bool Exists
    {
        get { return this != null; }
    }

    public Sprite Thumbnail
    {
        get;
        set;
    }
}
