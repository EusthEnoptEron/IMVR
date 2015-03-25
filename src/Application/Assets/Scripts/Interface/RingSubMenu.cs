using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using Gestures;
using System.Collections.Generic;

public class RingSubMenu : RingMenuItem, IPointerClickHandler, IRingMenu {

    private Transform submenuNode;
    private RingMenu menu;
   

    protected override void Awake()
    {
        base.Awake();
        Items = new Dictionary<FingerType, RingMenuItem>();

        submenuNode = transform.Children().FirstOrDefault(child => child.name.ToLower() == "menu");
        if (submenuNode != null)
        {

            // Fill list of items
            foreach (var child in submenuNode.Children())
            {
                child.gameObject.SetActive(false);
                var item = child.GetComponent<RingMenuItem>();
                if (item != null)
                    Items.Add(item.fingerType, item);
            }
        }


        menu = transform.GetComponentInParent<RingMenu>();
        Level = transform.Ancestors().Count(parent => parent.GetComponent<RingSubMenu>() != null) + 1;
    }


    protected override void Update()
    {
        base.Update();

        if (menu.ActiveMenu == this || menu.ActiveMenu.Level > Level)
        {
            var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
            if (hand != null)
            {
                transform.position = hand.PalmPosition + hand.PalmNormal * 0.05f * Level;//hand.PalmPosition + Vector3.ProjectOnPlane(finger.GetBone(BoneType.Intermediate).Position - hand.PalmPosition, Camera.main.transform.forward) * 2f;// + Camera.main.transform.TransformDirection(distance);
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

    public GameObject Node
    {
        get { return gameObject; }
    }
}
