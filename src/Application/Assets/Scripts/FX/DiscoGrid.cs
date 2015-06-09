using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DiscoGrid : MonoBehaviour {
    private float m_time = 0;

    private float interval = float.MaxValue;
    public float changeTime = 0.1f;
    public Material baseMaterial;
    public float alpha = 0.5f;

    public Color[] colors = new Color[] { 
        Color.red,
        Color.green,
        Color.blue,
        Color.cyan,
        Color.magenta,
        Color.yellow
    };

    public int horizontalTiles = 10;
    public int verticalTiles = 10;

    private Material[] m_materials;
    private Shader m_shader;

	// Use this for initialization
	void Start () {
        m_shader = Shader.Find("Standard");
        m_materials = new Material[verticalTiles * horizontalTiles];
        for (int y = 0; y < verticalTiles; y++)
        {
            for (int x = 0; x < horizontalTiles; x++)
            {
                CreateTile(x, y);
            }
        }

        Jukebox.Instance.Playlist.IndexChange += (sender, evt) =>
        {
            interval = 60 / Jukebox.Instance.Playlist.Current.Tempo;

            if (float.IsInfinity(interval))
            {
                interval = 1;
            }
            Debug.LogFormat("Interval: {0}", interval);
        };
	}


    private void CreateTile(int x, int y)
    {
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        GameObject.Destroy(quad.GetComponent<Collider>());
        quad.transform.SetParent(transform, false);

        quad.transform.localPosition = new Vector3(x - horizontalTiles / 2, 0,  y - verticalTiles / 2);
        quad.transform.rotation = Quaternion.LookRotation(Vector3.down);

        var renderer = quad.GetComponent<MeshRenderer>();
        renderer.sharedMaterial = Material.Instantiate<Material>(baseMaterial);
        m_materials[y * horizontalTiles + x] = renderer.sharedMaterial;
    }


	
	// Update is called once per frame
	void Update () {
        if (Time.time - m_time > interval)
        {
            m_time = Time.time;

            foreach (var material in m_materials)
            {
                var color = colors[Random.Range(0, colors.Length)];
                material.DOColor(new Color(color.r, color.g, color.b, alpha), changeTime);
            }
        }
	}
}
