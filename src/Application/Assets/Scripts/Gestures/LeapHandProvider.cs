using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Linq;

namespace Gestures
{
    public class LeapHandProvider : HandProvider
    {
        private class HandSnapshot
        {
            private bool dirty;
            public readonly GenericHand Hand = null;
            public readonly float Time;

            public HandSnapshot(GenericHand hand, float time)
            {
                this.Hand = hand;
                this.dirty = false;
                this.Time = time;
            }

            public HandSnapshot(HandSnapshot snapshot)
            {
                this.Hand = snapshot.Hand;
                this.dirty = true;
                this.Time = snapshot.Time;
            }

            public bool IsOutdated
            {
                get { return dirty; }
            }
        }

        public bool isHeadMounted = true;
        /// <summary>
        /// Threshold for hand confidence.
        /// </summary>
        public float threshold = 0f;

        public float delay = 1f;
        /// <summary>
        /// The transform all hands should be rooted to. Defaults to world coordinates.
        /// </summary>
        public Transform root;

        Leap.Controller controller;


        private Dictionary<HandType, HandSnapshot> hands;

        void Awake()
        {
            hands = new Dictionary<HandType, HandSnapshot>();
            hands.Add(HandType.Left, null);
            hands.Add(HandType.Right, null);


            controller = new Leap.Controller();
            if(isHeadMounted)
                controller.SetPolicyFlags(Leap.Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
        }

        protected void Update()
        {
            var frame = controller.Frame();
            Vector3 origin = root == null ? Vector3.zero : root.position;

            foreach (var type in (HandType[])Enum.GetValues(typeof(HandType)))
            {
                var hand = getHand(frame, type);
                if (hand == null || hand.Confidence < threshold)
                {
                    if (hands[type] != null)
                    {
                        hands[type] = new HandSnapshot(hands[type]);
                    }
                }
                else
                {
                    var genericHand = new GenericHand(hand,
                        transform.position,
                        transform.lossyScale,
                        transform.parent == null ? Quaternion.identity : transform.parent.rotation,
                        transform.localRotation);
                    hands[type] = new HandSnapshot(genericHand, Time.time);
                }
               
              
            }
            //hands.Add(HandType.Left, new NormalizedHand(

        }


        /// <summary>
        /// Tries to get the specified hand from the frame.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private Leap.Hand getHand(Leap.Frame frame, HandType type)
        { 
            // We have to find a new hand.
            bool isLeft = type == HandType.Left ? true : false;
            int id = (hands[type] != null) ? hands[type].Hand.Id : -1;

            var handSelection = frame.Hands.Where(h => h.IsLeft == isLeft).OrderByDescending(h => h.Confidence);

            Leap.Hand rightmost = null;
            Leap.Hand idHand = null;
            foreach (var hand in handSelection)
            {
                if (hand.Confidence > 0.5 || hand.Id == id)
                {
                    if (rightmost == null
                        || (type == HandType.Left && hand.PalmPosition.x < rightmost.PalmPosition.x)
                        || (type == HandType.Right && hand.PalmPosition.x > rightmost.PalmPosition.x))
                    {
                        rightmost = hand;
                    }
                }
                if (hand.Id == id)
                {
                    idHand = hand;
                }
            }
            if (idHand == null) return rightmost;
            if (rightmost == idHand || rightmost == null) return idHand;

            if (rightmost.Confidence > idHand.Confidence)
                return rightmost;
            else
                return idHand;

        }

        public Leap.Frame GetFrame()
        {
            return controller.Frame();
        }

        public override GenericHand GetHand(HandType type, NoHandStrategy strategy = NoHandStrategy.SetNull)
        {
            
            var snapshot = hands[type];

            if (snapshot != null)
            {
                if (strategy == NoHandStrategy.SetNull && snapshot.IsOutdated)
                    return null;
                else if (strategy == NoHandStrategy.DelayedSetNull && (Time.time - snapshot.Time) > delay)
                {
                    return null;
                }
                else
                {
                    //if (type == HandType.Right && snapshot.Hand != null)
                    //{
                    //    var hand = new GenericHand(HandPosition.PinchRight.GetHand());

                    //    var diff = snapshot.Hand.PalmPosition - hand.PalmPosition;
                    //    hand.Transform.Translation += diff;

                    //    var qDiff = Quaternion.Inverse(hand.LocalPalmRotation) * snapshot.Hand.LocalPalmRotation;

                    //    hand.Transform.RotateAround(hand.LocalPalmPosition, qDiff);
                    //    //hand.WristPosition = snapshot.Hand.WristPosition;

                    //    return hand;
                    //}
                    return snapshot.Hand;
                }
            }
            else
            {
                return null;
            }
        }

        public override GenericHand LeftHand
        {
            get
            {
                return GetHand(HandType.Left, NoHandStrategy.DelayedSetNull);
            }
        }

        public override GenericHand RightHand
        {
            get
            {
                return GetHand(HandType.Right, NoHandStrategy.DelayedSetNull);

            }
        }
    }
}
