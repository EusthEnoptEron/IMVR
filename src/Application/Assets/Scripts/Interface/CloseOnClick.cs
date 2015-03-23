using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CloseOnClick : MonoBehaviour, IPointerClickHandler {
    public bool pause = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerClick(PointerEventData eventData)
    {
#if UNITY_EDITOR
        if (pause)
            UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused;
        else
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.isPlaying = false;       
#endif
    }
}
