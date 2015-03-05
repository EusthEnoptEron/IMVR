using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Gestures
{
    public enum GestureState
    {
        None,
        Enter,
        Maintain,
        Leave,
        Execute
    }
    public class GestureEventData : BaseEventData
    {
        public GestureEventData(EventSystem system, GestureState state, Gesture gesture) : base(system)
        {
            State = state;
            Gesture = gesture;
        }

        public GestureState State { get; private set; }
        public Gesture Gesture { get; private set; }
        public GenericHand Hand { get; set; }
        public HandPair HandPair { get; set; }
        public bool UseBothHands { get; set; }
    }
}
