using UnityEngine;
using System.Collections;

public interface IVerticalScroll {
    void Scroll(float speed, Vector3 delta);

    void BeginScroll();

    void EndScroll();
}
