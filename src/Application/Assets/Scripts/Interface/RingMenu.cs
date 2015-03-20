using UnityEngine;
using System.Collections;
using Gestures;
using System.Linq;
using DG.Tweening;

public class RingMenu : MonoBehaviour {
    private bool activated = false;
    private CanvasGroup canvasGroup;
	// Use this for initialization
	void Start () {
        this.canvasGroup = GetComponent<CanvasGroup>();

	}
	
	// Update is called once per frame
	void Update () {
        var hand = HandProvider.Instance.GetHand(HandType.Left, NoHandStrategy.SetNull);
        if (hand != null)
        {
            if (Vector3.Dot(hand.LocalPalmNormal, new Vector3(1, 1, -1).normalized) > 0)
            {
                UpdateOrders();
                SetState(true);
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
        return false;
    }



    private void SetState(bool enabled)
    {
        if (activated != enabled)
        {
            activated = enabled;

            canvasGroup.DOKill();
            var animation = canvasGroup.DOFade(activated ? 1 : 0, 0.5f);

            if (activated) SetChildren();
            else animation.OnComplete(SetChildren);
        }
    }

    private void SetChildren()
    {
        foreach (var child in transform.Children()) child.gameObject.SetActive(activated);
    }


    private void UpdateOrders()
    {
        var orderedChild = transform.Children().OrderByDescending(child => Vector3.Distance(Camera.main.transform.position, child.position));

        foreach (var child in orderedChild)
            child.SetAsFirstSibling();
    }
}
