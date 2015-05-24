using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gestures;

public interface IRingMenu {
    int Level { get; }
    IDictionary<FingerType, RingMenuItem> Items { get; }
    GameObject Node { get; }
}
