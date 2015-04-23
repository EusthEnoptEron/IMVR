using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CircleLayout))]
public class CylinderInteractor : MonoBehaviour, IDragHandler {

    private const float LOW_SPEED_THRESHOLD = 0.05f;

    private HandProvider handInput;
    private VelocityMeasurer measurer = new VelocityMeasurer();
    private CircleLayout layout;

    private bool rotating = false;
    /// <summary>
    /// The maximum speed the head may rotate before ignoring all input.
    /// </summary>
    public float ovrSpeedThreshold = 0.2f;

    private bool interacting = false;

	// Use this for initialization
	void Start () {
        handInput = HandProvider.Instance;
        layout = GetComponent<CircleLayout>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (handInput.GetGesture("hand_grab_l"))
        //{
        //    Debug.Log("GRAB");
        //}
        //if (handInput.GetGestureExit("hand_grab_l"))
        //{
        //    Debug.Log("LEAVE");
        //}
        
        var hand = handInput.GetHand(HandType.Right);
        if (hand != null 
        //    && OVRManager.display.angularVelocity.sqrMagnitude < ovrSpeedThreshold
        )
        {
            Handle(hand);

            //if (handInput.GetGestureEnter("Push"))
            //{
            //    StartCoroutine(PushPull(true));
            //}
            //if (handInput.GetGestureEnter("Pull"))
            //{
            //    StartCoroutine(PushPull(false));
            //}
        }
        else if(interacting)
        {
            interacting = false;
            GetComponentInParent<View>().SetInteraction(true);
        }
    }


    IEnumerator PushPull(bool forward)
    {
        string eventName = forward ? "Push" : "Pull";

        var prevPosition = handInput.GetHand(HandType.Left).PalmPosition;
        var camera = Camera.main.transform;
        var startScale = layout.transform.localScale;

        while (!handInput.GetGestureExit(eventName))
        {
            var position = handInput.GetHand(HandType.Left).PalmPosition;

            float pushDistance = Vector3.Dot(
                position - prevPosition,
                camera.forward
            );

            if ( (forward && pushDistance > 0) || (!forward && pushDistance < 0) )
            {
                layout.radius += pushDistance;
                //layout.transform.localScale += Vector3.one * ((1 / layout.radius) * pushDistance);
            }

            prevPosition = position;
            yield return null;
        }
    }


    void Handle(GenericHand hand)
    {
        measurer.AddPosition(hand.GetFinger(FingerType.Pinky).TipPosition);
        var velocity = measurer.GetVelocity();

        if(hand.Fingers
            .Where(f => f.Type != FingerType.Thumb)
            .All(f => ( Vector3.ProjectOnPlane(f.TipPosition - transform.position, transform.up).magnitude > layout.radius)))
        {
            //Debug.LogFormat("{0}: {1} ({2})", name,, layout.radius );
            var horizontalVelocity = Vector3.Dot(velocity, Camera.main.transform.right);

            if (horizontalVelocity > LOW_SPEED_THRESHOLD || interacting)
            {
                if (!interacting)
                    GetComponentInParent<View>().SetInteraction(false);
                interacting = true;

                float factor = horizontalVelocity > 0.5 ? 20 : 10;
                layout.AddTorque(horizontalVelocity * factor);
            }
        }
        else
        {
            if(interacting)
                GetComponentInParent<View>().SetInteraction(true);
            interacting = false;
        }
            return;

        //if (Vector3.Dot(velocity.normalized, hand.PalmNormal) < 0.2f) return;
        //if (!hand.Fingers.All(f => f.Extended)) return;
    }

    IEnumerator Swipe(HandType handType)
    {
        while (true)
        {

        }
    }


    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("DRAG");
    }
}
