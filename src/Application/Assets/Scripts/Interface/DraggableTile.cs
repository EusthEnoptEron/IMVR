using UnityEngine;
using System.Collections;

public class DraggableTile : MonoBehaviour, IVerticalScroll {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Scroll(float speed, Vector3 delta)
    {
        transform.position += new Vector3(0, delta.y, 0);
    }
}
