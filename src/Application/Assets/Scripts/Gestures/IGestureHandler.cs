using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Gestures
{
    interface IGestureHandler : IEventSystemHandler
    {
        void OnGesture(GestureEventData eventData);
    }
}
