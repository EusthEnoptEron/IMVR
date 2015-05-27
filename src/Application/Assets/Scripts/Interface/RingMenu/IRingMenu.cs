using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Gestures;

public interface IRingMenu {
    int Level { get; }
    IDictionary<FingerType, RingMenuItem> Items { get; }

    /// <summary>
    /// Updates the list of items inside this menu.
    /// </summary>
    void Clear();
    void UpdateItems();

    Transform Node { get; }
    Transform ItemNode { get; }

    Sprite Thumbnail { get; }
}
