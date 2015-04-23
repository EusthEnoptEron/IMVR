using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class AtlasQuad : MonoBehaviour {
    public Sprite sprite;

    private bool _dirty = true;
    private MeshRenderer _renderer;
    private MeshFilter _filter;
    
	// Use this for initialization
	void Start () {
        _renderer = GetComponent<MeshRenderer>();
        _filter = GetComponent<MeshFilter>();

        if (!_renderer) _renderer = gameObject.AddComponent<MeshRenderer>();
        if (!_filter) _filter = gameObject.AddComponent<MeshFilter>();

        if (sprite != null)
        {
            // Set texture
            if (_renderer.sharedMaterial == null)
            {
                var standardShader = Shader.Find("Standard");
                _renderer.sharedMaterial = new Material(standardShader);
                _renderer.sharedMaterial.SetFloat("_Glossiness", 0);
                _renderer.sharedMaterial.SetFloat("_Metallic", 0);
            }
            _renderer.material.mainTexture = sprite.texture;

            MakeMesh();
        }
	}

    private void MakeMesh()
    {
        var mesh = new Mesh();
        var vertices = new Vector3[4];
        var uvs = new Vector2[4];
        var normals = new Vector3[4];

        int[] faces;

        // TL, TR, BR, BL
        vertices[0] = new Vector3(-0.5f, 0.5f, 0);
        vertices[1] = new Vector3(0.5f, 0.5f, 0);
        vertices[2] = new Vector3(0.5f, -0.5f, 0);
        vertices[3] = new Vector3(-0.5f, -0.5f, 0);

        var factor = new Vector2(1f / sprite.texture.width, 1f / sprite.texture.height);
        uvs[0] = Vector3.Scale(sprite.rect.position + sprite.rect.size, factor);
        uvs[1] = Vector3.Scale(sprite.rect.position + new Vector2(0, sprite.rect.height), factor);
        uvs[2] = Vector3.Scale(sprite.rect.position, factor);
        uvs[3] = Vector3.Scale(sprite.rect.position + new Vector2(sprite.rect.width, 0), factor);

        normals[0] = new Vector3(0, 0, -1);
        normals[1] = new Vector3(0, 0, -1);
        normals[2] = new Vector3(0, 0, -1);
        normals[3] = new Vector3(0, 0, -1);

        faces = new int[] { 
            0, 1, 3,
            1, 2, 3
        };

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = faces;
        mesh.uv = uvs;

        _filter.mesh = mesh;
    }
}
