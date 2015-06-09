using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SongMetaChart))]
public class SelectionVisualizer : MonoBehaviour {
    public SongSelection selection;
    public Material material;

    private SongMetaChart _chart;
    private GameObject _mesh; 

	// Use this for initialization
    void Start()
    {
        if (selection == null) enabled = false;
        else
        {
            _chart = GetComponent<SongMetaChart>();

            // Make selection mesh
            _mesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            {
                _mesh.name = "Selection";
                _mesh.transform.SetParent(transform, false);
                _mesh.GetComponent<Renderer>().material = material;

                Destroy(_mesh.GetComponent<Collider>());
            }

            selection.SelectionChanged += selection_SelectionChanged;
            _chart.AxisChanged += chart_AxisChanged;
        }
    }

    void chart_AxisChanged(object sender, System.EventArgs e)
    {
        UpdateMesh();
    }

    void selection_SelectionChanged(object sender, System.EventArgs e)
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        // Place mesh correctly! 
        // Pivot is in the center, so we need an offset.
        var offset = new Vector3(-0.5f, -0.5f, -0.5f);

        var x = selection.GetCriterion(_chart.xAxis);
        var y = selection.GetCriterion(_chart.yAxis);
        var z = selection.GetCriterion(_chart.zAxis);

        _mesh.transform.localScale = new Vector3(
            x.Max - x.Min,
            y.Max - y.Min,
            z.Max - z.Min
        );

        _mesh.transform.localPosition = new Vector3(
            x.Min,
            y.Min,
            z.Min
        ) + Vector3.Scale(offset, Vector3.one - _mesh.transform.localScale);

    }
}
