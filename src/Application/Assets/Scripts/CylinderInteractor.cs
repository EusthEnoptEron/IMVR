using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(CircleLayout))]
public class CylinderInteractor : MonoBehaviour {

    private const float LOW_SPEED_THRESHOLD = 0.1f;

    private HandProvider handInput;
    private VelocityMeasurer measurer = new VelocityMeasurer();
    private CircleLayout layout;

    private bool rotating = false;
    /// <summary>
    /// The maximum speed the head may rotate before ignoring all input.
    /// </summary>
    public float ovrSpeedThreshold = 0.2f;

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
        if (hand != null && 
            OVRManager.display.angularVelocity.sqrMagnitude < ovrSpeedThreshold)
        {
            Handle(hand);

            if (handInput.GetGestureEnter("Push"))
            {
                StartCoroutine(PushPull(true));
            }
            if (handInput.GetGestureEnter("Pull"))
            {
                StartCoroutine(PushPull(false));
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

        if (Vector3.Dot(velocity.normalized, hand.PalmNormal) < 0.2f) return;
        if (!hand.Fingers.All(f => f.Extended)) return;
        
        float factor = velocity.sqrMagnitude > 0.5 ? 10 : 5;

        if (velocity.magnitude > LOW_SPEED_THRESHOLD)
        {
            layout.GetComponent<Rigidbody>().AddRelativeTorque(
                Vector3.up * Vector3.Dot(velocity, Camera.main.transform.right) * factor
            );
        } else {

        }
    }

    IEnumerator Swipe(HandType handType)
    {
        while (true)
        {

        }
    }




    class VelocityMeasurer
    {
        public float Interval = 0.3f;

        struct Entry
        {
            public float Time;
            public Vector3 Position;
        }

        private List<Entry> entries = new List<Entry>();

        public void AddPosition(Vector3 position)
        {

            entries.Add(new Entry
            {
                Time = Time.time,
                Position = position
            });
        }

        public Vector3 GetVelocity()
        {
            // Calculate
            return GetDifference() / 0.5f;
        }

        public Vector3 GetDifference()
        {
            entries.RemoveAll(e => e.Time < Time.time - Interval);
            return (entries.LastOrDefault().Position - entries.FirstOrDefault().Position);
        }
    }

}
