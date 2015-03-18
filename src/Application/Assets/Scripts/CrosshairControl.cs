using UnityEngine;
using System.Collections;
using System.Linq;


[RequireComponent(typeof(CanvasGroup))]
public class CrosshairControl : MonoBehaviour {

    private float targetValue = 0;
    private float targetAlpha = 1;

    protected float m_value = 0;
    protected float m_alpha = 0;

    public float Value
    {
        get { return targetValue; }
        set { targetValue = value; }
    }


    public float animationSpeed = 5;

    public bool Visible
    {
        get
        {
            return targetAlpha == 1;
        }
        set
        {
            targetAlpha = value ? 1 : 0;
        }
    }

    public float scale = 1f;
    
    private RectTransform environment;
    public float startPos = 0.1f;
    public float endPos = 0.12f;
    private CanvasGroup group;

	// Use this for initialization
	void Start () {
        environment = transform.GetComponentsInChildren<RectTransform>().First(rect => rect.name == "Environment");
        group = GetComponent<CanvasGroup>();
	}
	
	// Update is called once per frame
	void Update () {
        m_value = Mathf.Lerp(m_value, targetValue, Time.deltaTime * animationSpeed);
        m_alpha = Mathf.Lerp(m_alpha, targetAlpha, Time.deltaTime * animationSpeed);

        
        float pos = Mathf.Lerp(startPos, endPos, m_value);
        //environment.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, pos, 05f - pos);
        //environment.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos, 05f - pos);
        environment.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pos);
        environment.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, pos);

        // Resize uniformly
        transform.localScale = Vector3.Distance(transform.position, Camera.main.transform.position) * Vector3.one * scale;
        
        // Billboard effect
        transform.rotation = Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized);

        if(group.alpha != m_alpha)
            group.alpha = m_alpha;
	}

}
