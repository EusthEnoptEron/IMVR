using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gestures
{
    abstract class TwoHandGesture : Gesture
    {
        protected override bool Filter(GenericHand hand)
        {
            return false;
        }

        protected override bool Filter(GenericHand leftHand, GenericHand rightHand)
        {
            return true;
        }
    }
}
