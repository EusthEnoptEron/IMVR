using UnityEngine;
using System.Collections;
using System.Linq;

public class CrosshairControl : MonoBehaviour {

    protected float m_value = 0;
    public float Value
    {
        get { return m_value; }
        set { m_value = value; }
    }

    protected bool m_visible = true;
    public bool Visible
    {
        get
        {
            return m_visible;
        }
        set
        {
            m_visible = value;
            if (!m_visible) GetComponent<CanvasGroup>().alpha = 0;
            else GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    public float scale = 1f;
    
    private RectTransform environment;
    public float startPos = 0.1f;
    public float endPos = 0.12f;

	// Use this for initialization
	void Start () {
        environment = transform.GetComponentsInChildren<RectTransform>().First(rect => rect.name == "Environment");

	}
	
	// Update is called once per frame
	void Update () {
        float pos = Mathf.Lerp(startPos, endPos, Value);
        //environment.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos, 05f - pos);
        //environment.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos, 05f - pos);
        environment.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pos);
        environment.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pos);

        // Resize uniformly
        transform.localScale = Vector3.Distance(transform.position, Camera.main.transform.position) * Vector3.one * scale;
        
        // Billboard effect
        transform.rotation = Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized);
	}

}
