using UnityEngine;
using System.Collections;

public class DebugControls : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var imgManager = FindObjectOfType<ImageManager>();
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (Input.GetKey(KeyCode.LeftControl))
                imgManager.transform.localScale += Vector3.one * Time.deltaTime;
            else if(Input.GetKey(KeyCode.LeftShift))
                imgManager.transform.localPosition += Camera.main.transform.forward * Time.deltaTime;
            else
                imgManager.transform.localPosition += Camera.main.transform.up * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.LeftControl))
                imgManager.transform.localScale -= Vector3.one * Time.deltaTime;
            else if (Input.GetKey(KeyCode.LeftShift))
                imgManager.transform.localPosition -= Camera.main.transform.forward * Time.deltaTime;
            else
                imgManager.transform.localPosition -= Camera.main.transform.up * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            imgManager.transform.localPosition -= Camera.main.transform.right * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            imgManager.transform.localPosition += Camera.main.transform.right * Time.deltaTime;
        }

	}
}
