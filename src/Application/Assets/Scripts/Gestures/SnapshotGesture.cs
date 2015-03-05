using UnityEngine;
using System.Collections;
using Gestures;
using System.Collections.Generic;

namespace Gestures
{
    public class SnapshotGesture : Gesture
    {
        LeapAnimationVector vector;
        public string gestureName;
        public float threshold = 0.7f;

        protected override void Awake()
        {
            base.Awake();
            vector = new LeapAnimationVector(LoadGesture(gestureName));
        }

        public override int BacktrackLength
        {
            get { return vector.HandCount; }
        }

        public override string Name
        {
            get { return gestureName; }
        }

        public override bool Stateful
        {
            get { return false; }
        }

        protected override bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state)
        {
            var comparisonVector = new LeapAnimationVector(cache);
            float similarity = SimilarityVector.Similarity(comparisonVector, vector);
            if (similarity > threshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool Filter(GenericHand hand)
        {
            return hand.IsLeft;
        }
    }
}
