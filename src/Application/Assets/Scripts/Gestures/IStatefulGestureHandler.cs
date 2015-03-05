using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Gestures
{
    interface IStatefulGestureHandler : IEventSystemHandler
    {
        void OnGestureEnter(GestureEventData eventData);
        void OnGestureLeave(GestureEventData eventData);
        void OnGestureMaintain(GestureEventData eventData);
    }
}
