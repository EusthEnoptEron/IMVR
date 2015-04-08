using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Makes sure that mesh opacity correspondences to that of parenting canvas.
/// </summary>
public class CanvasOpacityHelper : MonoBehaviour {
    private CanvasGroup[] groups;
    private MeshRenderer renderer;
    private float m_opacity = 1;

    public bool useSharedMaterial = true;

	// Use this for initialization
	void Start () {
        groups = GetComponentsInParent<CanvasGroup>();
        renderer = GetComponent<MeshRenderer>();

        if (renderer == null) enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        var material  = useSharedMaterial ? renderer.sharedMaterial : renderer.material;
        float opacity = groups.Aggregate(1f, (factor, group) => group.alpha * factor);

        if (opacity != m_opacity)
        {
            m_opacity = opacity;
            material.color = new Color(material.color.r, material.color.g, material.color.b, m_opacity);
        }
	}
}
