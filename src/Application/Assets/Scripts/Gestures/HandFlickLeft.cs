using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Gestures
{
    public class HandFlickLeft : Gesture
    {
        LeapAnimationVector vector;

        protected override void Awake()
        {
            base.Awake();
            vector = new LeapAnimationVector(LoadGesture("HandFlickLeft"));
        }

        public override int BacktrackLength
        {
            get { return vector.HandCount; }
        }

        public override string Name
        {
            get { return "HandFlickLeft"; }
        }

        public override bool Stateful
        {
            get { return false; }
        }

        protected override bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state)
        {
            var comparisonVector = new LeapAnimationVector(cache);
            float similarity = SimilarityVector.Similarity(comparisonVector, vector);
            if (similarity > 0.7f)
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