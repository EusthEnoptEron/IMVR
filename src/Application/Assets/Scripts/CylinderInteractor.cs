using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;


[RequireComponent(typeof(CylinderLayout))]
public class CylinderInteractor : MonoBehaviour {
    internal enum Direction
    {
        Unknown,
        Vertical,
        Horizontal
    }


    private const float LOW_SPEED_THRESHOLD = 0.2f;

    private HandProvider handInput;
    private VelocityMeasurer measurer = new VelocityMeasurer();
    private CylinderLayout layout;
    private Direction _direction = Direction.Unknown;
    

    private bool rotating = false;
    /// <summary>
    /// The maximum speed the head may rotate before ignoring all input.
    /// </summary>
    public float ovrSpeedThreshold = 0.1f;

    private bool interacting = false;

    private View _view;

	// Use this for initialization
	void Start () {
        handInput = HandProvider.Instance;
        layout = GetComponent<CylinderLayout>();
        _view = GetComponentInParent<View>();
	}
	
	// Update is called once per frame
	void Update () {

        if (_view.Interaction != InteractionMode.Disabled)
        {
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
            else if (interacting)
            {
                _direction = Direction.Unknown;
                interacting = false;
                GetComponentInParent<View>().SetInteraction(InteractionMode.Enabled);
            }
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

        int validFingers = hand.Fingers
            .Where(f => f.Type != FingerType.Thumb)
            .Count(f => (Vector3.ProjectOnPlane(f.TipPosition - transform.position, transform.up).magnitude > layout.radius));

        if ((!interacting && validFingers >= 4) || (interacting && validFingers > 0))
        {

            //Debug.LogFormat("{0}: {1} ({2})", name,, layout.radius );
            var horizontalVelocity = Vector3.Dot(velocity, Camera.main.transform.right);
            var verticalVelocity = Vector3.Dot(velocity, Camera.main.transform.up);
            float absHorizontalVelocity = Mathf.Abs(horizontalVelocity);
            float absVerticalVelocity = Mathf.Abs(verticalVelocity);

            if (!interacting)
                GetComponentInParent<View>().SetInteraction(InteractionMode.Partly);
            interacting = true;
            if ((_direction == Direction.Unknown && absHorizontalVelocity > LOW_SPEED_THRESHOLD && absVerticalVelocity < absHorizontalVelocity) 
                || _direction == Direction.Horizontal)
            {
                if (!interacting)
                    GetComponentInParent<View>().SetInteraction(InteractionMode.Partly);
                interacting = true;
                _direction = Direction.Horizontal;

                float factor = horizontalVelocity > 0.5 ? 20 : 10;
                layout.AddTorque(horizontalVelocity * factor);
            }
            else if ((_direction == Direction.Unknown && absVerticalVelocity > LOW_SPEED_THRESHOLD && absHorizontalVelocity < absVerticalVelocity) 
                || _direction == Direction.Vertical)
            {
               
                _direction = Direction.Vertical;


                float factor = 20;
                var avgPos = hand.Fingers.Where(f => f.Type != FingerType.Thumb)
                                         .Select(f => f.TipPosition)
                                         .Aggregate((v1, v2) => v1 + v2)
                                         + hand.GetFinger(FingerType.Index).TipPosition
                                         / 5f;


                var groupGO = layout.GetTileAtPosition(avgPos);
                if (groupGO)
                {
                    Debug.Log(groupGO.name);
                    var group = groupGO.GetComponent<LayoutGroup>();
                    if (group)
                    {
                        group.scrollSpeed += verticalVelocity * factor;
                    }
                }
                else
                {
                    Debug.Log("none found");
                }
                //layout.AddTorque(horizontalVelocity * factor);
            }
        }
        else
        {
            if(interacting)
                GetComponentInParent<View>().SetInteraction(InteractionMode.Enabled);
            interacting = false;
            _direction = Direction.Unknown;
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

}
