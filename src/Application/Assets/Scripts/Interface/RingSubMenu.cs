using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;
using Gestures;

public class RingSubMenu : RingMenuItem, IPointerClickHandler {

    private Transform submenuNode;
    private RingMenu menu;


    protected override void Awake()
    {
        base.Awake();

        submenuNode = transform.Children().FirstOrDefault(child => child.name.ToLower() == "menu");
        if (submenuNode != null) submenuNode.gameObject.SetActive(false);

        menu = transform.GetComponentInParent<RingMenu>();
    }

    protected override void Update()
    {
        base.Update();

        if (menu.ActiveMenu == submenuNode)
        {
            var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
            if (hand != null)
            {
                transform.position = hand.PalmPosition + hand.PalmNormal * 0.05f;//hand.PalmPosition + Vector3.ProjectOnPlane(finger.GetBone(BoneType.Intermediate).Position - hand.PalmPosition, Camera.main.transform.forward) * 2f;// + Camera.main.transform.TransformDirection(distance);
                transform.rotation = Quaternion.LookRotation(-hand.PalmNormal, hand.PalmDirection);
            }
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(menu != null) {
            menu.ActiveMenu = submenuNode;
        }
        Progress = 0;
    }
}
