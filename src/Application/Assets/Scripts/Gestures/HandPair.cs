using UnityEngine;
using System.Collections;

namespace Gestures
{
    public class HandPair
    {
        public GenericHand LeftHand { get; private set; }
        public GenericHand RightHand { get; private set; }

        public HandPair(GenericHand leftHand, GenericHand rightHand)
        {
            LeftHand = leftHand;
            RightHand = rightHand;
        }
    }
}