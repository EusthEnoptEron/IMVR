using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gestures
{
    public class PanGesture : Gesture
    {
        public float threshold = 0.5f;
        public float threshold2 = 0.7f;

        public override int BacktrackLength
        {
            get { return 1; }
        }

        public override string Name
        {
            get { return "Pan"; }
        }

        public override bool Stateful
        {
            get { return true; }
        }

        protected override bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state)
        {

            float t = state == GestureState.None ? threshold : threshold2;

            foreach (var finger in hand.Fingers)
            {
                if (finger.Type == FingerType.Thumb) continue;

                var dir0 = finger.GetBone(BoneType.Proximal).LocalDirection;
                var dir1 = finger.GetBone(BoneType.Intermediate).LocalDirection;

                if (Vector3.Dot(dir0, dir1) >= t)
                    return false;

            }

            return true;
        }

        protected override bool Filter(GenericHand hand)
        {
            return hand.IsLeft;
        }
    }

}