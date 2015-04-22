using UnityEngine;
using System.Collections;

public class CustomPlane : MonoBehaviour {

    public float xSize = 1;
    public float zSize = 1;
    public int xSegments = 2;
    public int zSegments = 2;

    public void UpdateMesh()
    {
        var meshFilter = GetComponent<MeshFilter>();
        var mesh = CreateMesh(meshFilter.sharedMesh);
        meshFilter.sharedMesh = mesh;
    }

    private Mesh CreateMesh(Mesh mesh = null)
    {
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }

        float dx = xSize / xSegments;
        float dy = zSize / zSegments;
        float x0 = -xSize / 2;
        float z0 = -zSize / 2;
        int vertexCount = (xSegments + 1) * (zSegments + 1);

        var vertices = new Vector3[vertexCount];
        var uvs = new Vector2[vertexCount];
        var normals = new Vector3[vertexCount];
        var triangles = new int[xSegments * zSegments * 2 * 3];

        int index = 0;
        for (int iz = 0; iz < zSegments + 1; iz++)
        {
            for (int ix = 0; ix < xSegments + 1; ix++)
            {
                vertices[index] = new Vector3(x0 + ix * dx, 0, z0 + iz * dy);
                uvs[index] = new Vector2(ix / (float)xSegments, iz / (float)zSegments);
                normals[index] = Vector3.up;
                index++;
            }
        }

        var quadIndex = 0;
        for (int iz = 0; iz < zSegments; iz++)
        {
            for (int ix = 0; ix < xSegments; ix++)
            {
                var firstVertexIndex = iz * (xSegments + 1) + ix;
                triangles[quadIndex + 0] = firstVertexIndex + 1;
                triangles[quadIndex + 1] = firstVertexIndex;
                triangles[quadIndex + 2] = firstVertexIndex + xSegments + 1;
                triangles[quadIndex + 3] = firstVertexIndex + xSegments + 2;
                triangles[quadIndex + 4] = firstVertexIndex + 1;
                triangles[quadIndex + 5] = firstVertexIndex + xSegments + 1;
                quadIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        return mesh;
    }
}
