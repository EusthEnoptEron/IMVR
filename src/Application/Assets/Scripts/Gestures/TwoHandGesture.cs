using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gestures
{
    public abstract class TwoHandGesture : Gesture
    {
        protected override bool Filter(GenericHand hand)
        {
            return false;
        }

        protected override bool Filter(GenericHand leftHand, GenericHand rightHand)
        {
            return true;
        }
        protected override bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state)
        {
            return false;
        }
    }
}
