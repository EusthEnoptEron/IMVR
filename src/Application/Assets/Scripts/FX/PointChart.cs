﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(ParticleSystem))]
public class PointChart : MonoBehaviour {
    private MeshFilter meshFilter;
    private ParticleSystem particleSystem;

    public string xAxisLabel = "";
    public string yAxisLabel = "";
    public string zAxisLabel = "";


    public Vector3 pivotOffset = new Vector3(-0.5f, -0.5f, -0.5f);
    //public Transform xLabel;
    //public Transform yLabel;
    //public Transform zLabel;


    public Vector3[] points = new Vector3[0];

    //void Awake()
    //{
    //    //xLabel = new GameObject().transform;
    //    //yLabel = new GameObject().transform;
    //    //zLabel = new GameObject().transform;

    //    //xLabel.SetParent(transform, false);
    //    //yLabel.SetParent(transform, false);
    //    //zLabel.SetParent(transform, false);

    //    //xLabel.localPosition = Vector3.right;
    //    //yLabel.localPosition = Vector3.up;
    //    //zLabel.localPosition = Vector3.forward;
    //}

	// Use this for initialization
	void Awake () {

        //Randomize();

        meshFilter = GetComponent<MeshFilter>();
        particleSystem = GetComponent<ParticleSystem>();

        RebuildMesh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 200, 100), "Rebuild"))
    //    {
    //        Randomize();
    //        BuildMesh();
    //    }
    //}

    void Randomize()
    {
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new Vector3(Random.value, Random.value, Random.value);
        }
    }


    public void RebuildMesh()
    {
        var axisMesh = new Mesh();
        var axisVertices = new List<Vector3>();
        var axisTris = new List<int>();

        // Build axes
        var zAxis = Vector3.forward;
        var yAxis = Vector3.up;
        var xAxis = Vector3.right;

        BuildLine(pivotOffset, zAxis + pivotOffset, 0.02f, axisVertices, axisTris);
        BuildLine(pivotOffset, yAxis + pivotOffset, 0.02f, axisVertices, axisTris);
        BuildLine(pivotOffset, xAxis + pivotOffset, 0.02f, axisVertices, axisTris);

        // Build grid
        var helperMesh = new Mesh();
        var helperVertices = new List<Vector3>();
        var helperTris = new List<int>();

        BuildHelperLines(xAxis, yAxis, zAxis, helperVertices, helperTris);
        BuildHelperLines(yAxis, zAxis, xAxis, helperVertices, helperTris);
        BuildHelperLines(zAxis, xAxis, yAxis, helperVertices, helperTris);


        axisMesh.vertices = axisVertices.ToArray();
        axisMesh.triangles = axisTris.ToArray();

        helperMesh.vertices = helperVertices.ToArray();


        // Clean up
        Destroy(meshFilter.sharedMesh);
        meshFilter.sharedMesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(new CombineInstance[] {
            new CombineInstance() {
                 mesh = axisMesh
            },
            new CombineInstance() {
                mesh = helperMesh
            }
        }, false, false);

        meshFilter.sharedMesh.SetIndices(helperTris.ToArray(), MeshTopology.Lines, 1);



        UpdatePoints();

    }

    public void UpdatePoints()
    {
        float scale = transform.lossyScale.x;

        var particles = new ParticleSystem.Particle[points.Length];
        particleSystem.Emit(points.Length);
        particleSystem.GetParticles(particles);
        for (int i = 0; i < points.Length; i++)
        {
            particles[i].size = Mathf.Lerp(0.3f, 0.05f, points.Length / 5000) * scale;
            particles[i].lifetime = particles[i].startLifetime = 100000f;
            particles[i].velocity = Vector3.zero;
            particles[i].position = (points[i] + pivotOffset) * scale;
            particles[i].color = new Color(points[i].x, points[i].y, points[i].z);
            particles[i].angularVelocity = Random.RandomRange(-45f, 45f);
        }
        particleSystem.SetParticles(particles, particles.Length);
    }
    

    void BuildHelperLines(Vector3 primaryAxis, Vector3 secondaryAxis, Vector3 thirdAxis, List<Vector3> vertices, List<int> tris)
    {
        for (int x = 0; x <= 10; x++)
        {
            float pX = x / 10f;
            for (int y = 0; y <= 10; y++)
            {
                float pY = y / 10f;

                int offset = vertices.Count;
                vertices.AddRange(new Vector3[]{
                    pX * primaryAxis + pY * secondaryAxis + pivotOffset,
                    pX * primaryAxis + pY * secondaryAxis + thirdAxis + pivotOffset
                });
                
                tris.AddRange(new int[] {
                       offset, offset + 1
                });
                //BuildLine(pX * primaryAxis + pY * secondaryAxis,
                //          pX * primaryAxis + pY * secondaryAxis + thirdAxis, 0.001f, vertices, tris);
            }
        }

    }

    void BuildLine(Vector3 from, Vector3 to, float width,
        List<Vector3> vertices, List<int> tris)
    {
        float halfWidth = width / 2;
        var fwd = (to - from).normalized;
        var up = Vector3.ProjectOnPlane(Vector3.up, fwd).normalized;
     
        if (up == Vector3.zero) up = Vector3.up;
        if (up == fwd) up = Vector3.forward;

        var right = -Vector3.Cross(up, fwd).normalized;
        
        from -= fwd * halfWidth;
        to += fwd * halfWidth;

        int offset = vertices.Count;

        // FROM vertices (top left, top right, bottom right, bottom left)
        vertices.AddRange(new Vector3[] {
            from + (up - right) * halfWidth,
            from + (up + right) * halfWidth,
            from + (-up + right) * halfWidth,
            from + (-up - right) * halfWidth
        });

        int offset2 = vertices.Count;

        // TO vertices
        vertices.AddRange(new Vector3[] {
            to + (up - right) * halfWidth,
            to + (up + right) * halfWidth,
            to + (-up + right)  * halfWidth,
            to + (-up - right) * halfWidth
        });

        // tris
        MakeQuad(offset, offset + 1, offset + 2, offset + 3, tris);
        MakeQuad(offset2 + 1, offset2, offset2 + 3, offset2 + 2, tris);


        // TODO: make connection tris.
        // left
        MakeQuad(offset2, offset, offset + 3, offset2 + 3, tris);
        // up
        MakeQuad(offset2, offset2+1, offset+1, offset, tris);
        // bottom
        MakeQuad(offset + 2, offset2 + 2, offset2 + 3, offset + 3, tris);
        // right
        MakeQuad(offset + 1, offset2 + 1, offset2 + 2, offset + 2, tris);


    }

    private void MakeQuad(int iTL, int iTR, int iBR, int iBL, List<int> tris)
    {
        tris.AddRange(
            new int[] {
                // TL    BL         TR
                iTL, iBL, iTR,
                // TL   BR          TR
                iTR, iBL, iBR,
            }
        );
    }
}
