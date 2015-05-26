using UnityEngine;
using System.Collections;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RingMesh : MonoBehaviour {

    public event System.EventHandler<System.EventArgs> Updated = delegate { };

    private MeshFilter meshFilter;
    private bool _dirty = true;

    [SerializeField]
    private float _outerRadius = 2;
    [SerializeField]
    private float _innerRadius = 1;

    [SerializeField]
    private float _startAngle = 0;

    [SerializeField]
    private float _angle = 360;

    [SerializeField]
    private int _segments = 20;

    /// <summary>
    /// Vertex color.
    /// </summary>
    public Color color;

	// Use this for initialization
	private void Start () {
        meshFilter = GetComponent<MeshFilter>();
	}

    private void Update()
    {
        if (_dirty)
        {
            UpdateMesh();
        }
    }
	
	// Update is called once per frame
	private void UpdateMesh () {

        var mesh = new Mesh();

        var vertices = new Vector3[Segments * 2 + 2];
        var quads    = new int[(Segments + 1) * 4];

        // Fill list of vertices
        float cos, sin;
        for (int i = 0; i < Segments+1; i++)
        {
            float progress = i / (float)Segments;
            cos = Mathf.Cos(Mathf.Deg2Rad * (StartAngle + AngleRange * progress));
            sin = Mathf.Sin(Mathf.Deg2Rad * (StartAngle + AngleRange * progress));

            vertices[i] = new Vector3(
                cos * InnerRadius,
                0,
                sin * InnerRadius
            );

            vertices[i + Segments + 1] = new Vector3(
                cos * OuterRadius,
                0,
                sin * OuterRadius
            );
        }

        // Make quads
        for (int i = 0; i < Segments; i++)
        {
            int c = i * 4;
            quads[c] = i;
            quads[c + 1] = i + 1;
            quads[c + 2] = Segments + i + 2;
            quads[c + 3] = Segments + i + 1;
        }


        mesh.vertices = vertices;
        mesh.SetIndices(quads, MeshTopology.Quads, 0);
        mesh.SetTriangles(mesh.GetTriangles(0), 0);
        mesh.colors = Enumerable.Repeat(color, vertices.Length).ToArray();
        
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        DestroyImmediate(meshFilter.sharedMesh);
        meshFilter.sharedMesh = mesh;
        _dirty = false;

        Updated(this, new System.EventArgs());
    }



    #region Getters & Setters

    public int Segments
    {
        get
        {
            return _segments;
        }
        set
        {
            _segments = Mathf.Clamp(value, 5, 2000);
            _dirty = true;
        }
    }


    public float AngleRange
    {
        get
        {
            return _angle;
        }
        set
        {
            _angle = value;
            _dirty = true;
        }
    }

    public float StartAngle
    {
        get
        {
            return _startAngle;
        }
        set
        {
            _startAngle = value;
            _dirty = true;
        }
    }

    public float InnerRadius
    {
        get
        {
            return _innerRadius;
        }
        set
        {
            _innerRadius = Mathf.Clamp(value, 0, float.MaxValue);
            _dirty = true;
        }
    }

    public float OuterRadius
    {
        get
        {
            return _outerRadius;
        }
        set
        {
            _outerRadius = Mathf.Clamp(value, 0, float.MaxValue);
            _dirty = true;
        }
    }

    #endregion

}
