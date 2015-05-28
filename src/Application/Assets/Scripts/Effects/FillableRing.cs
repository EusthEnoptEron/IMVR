using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RingMesh))]
public class FillableRing : MonoBehaviour {
    public Material material;
    public Color color;
    public bool reuseColor = true;

    [SerializeField]
    private float _fill = 0;

    public float Fill
    {
        get
        {
            return _fill;
        }
        set
        {
            _fill = Mathf.Clamp01(value);

            subRing.OuterRadius = Mathf.Lerp(mainRing.InnerRadius, mainRing.OuterRadius, _fill);
        }
    }

    private RingMesh mainRing;
    private RingMesh subRing;

	// Use this for initialization
	void Start () {
        mainRing = GetComponent<RingMesh>();

        mainRing.Updated += mainRing_Updated;
        subRing = new GameObject().AddComponent<RingMesh>();
        subRing.transform.SetParent(transform, false);
        subRing.transform.localPosition += new Vector3(0, 0, -0.001f);

        subRing.GetComponent<MeshRenderer>().material = Material.Instantiate<Material>(material);

        Sync();
	}

    void mainRing_Updated(object sender, System.EventArgs e)
    {
        Sync();
    }

    private void Sync()
    {
        // Copy values
        subRing.InnerRadius = mainRing.InnerRadius;
        subRing.OuterRadius = mainRing.OuterRadius;
        subRing.AngleRange = mainRing.AngleRange;
        subRing.StartAngle = mainRing.StartAngle;
        subRing.Segments = mainRing.Segments;
        subRing.Color = reuseColor ? mainRing.Color : color;

        // Invoke setter
        Fill = Fill;
    }

}
