using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Gestures
{
    interface IGesture : IEventSystemHandler
    {
        GestureState Handle(GenericHand baseHand);
        GestureState Handle(GenericHand baseLeftHand, GenericHand baseRightHand);
    }
}
