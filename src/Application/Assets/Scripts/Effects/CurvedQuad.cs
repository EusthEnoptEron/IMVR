using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CurvedQuad : BaseVertexEffect, ICanvasRaycastFilter {

    //public float amplitude = 35f;
    public float phase = 0f;
    public float radius = 100;
    public float offset = 0;
    public float angleRange = 180;
    public bool buildVertices = false;

    private RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public float Aspect
    {
        get
        {
            return rectTransform.rect.width / rectTransform.rect.height;
        }
    }
    public float RadRange
    {
        get
        {
            if (rectTransform == null) return 0;
            return rectTransform.rect.width / radius;
        }
    }
    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive() || rectTransform == null)
            return;

        if (buildVertices)
        {
            BuildVertices(verts);
        }

        //float radRange = angleRange * Mathf.PI / 180f;
        float radRange = RadRange;
        float scale = (1 / rectTransform.rect.width);

        
        for (int index = 0; index < verts.Count; index++)
        {
            var uiVertex = verts[index];

            uiVertex.position.z = Mathf.Cos(uiVertex.position.x * scale * radRange + phase) * radius;
            uiVertex.position.x = Mathf.Sin(uiVertex.position.x * scale * radRange + phase) * radius;
            verts[index] = uiVertex;
        }
    }

    private void BuildVertices(List<UIVertex> verts)
    {
        if (verts.Count >= 4)
        {
            var corner1 = new Vector2(verts[0].position.x, verts[0].position.y);
            var corner2 = new Vector2(verts[2].position.x, verts[2].position.y);

            var uv0 = verts[0].uv0;
            var uv1 = verts[2].uv0;


            float xSegments = 50;
            float ySegments = 1;
            Color color = verts[0].color;


            verts.Clear();
            UIVertex vert = UIVertex.simpleVert;

            var distance = corner2 - corner1;

            for (int xStep = 0; xStep < Mathf.Max(1, xSegments); xStep++)
            {
                for (int yStep = 0; yStep < Mathf.Max(1, ySegments); yStep++)
                {
                    var leftRatio = new Vector2((float)xStep / xSegments, (float)yStep / ySegments);
                    var rightRatio = new Vector2((float)(xStep+1) / xSegments, (float)(yStep+1) / ySegments);
                    var left = corner1 + Vector2.Scale(distance, leftRatio);
                    var right = corner1 + Vector2.Scale(distance, rightRatio);

                    vert.position = new Vector2(left.x, left.y);
                    vert.color = color;
                    vert.uv0 = leftRatio;
                    
                    verts.Add(vert);

                    vert.position = new Vector2(left.x, right.y);
                    vert.color = color;
                    vert.uv0 = new Vector2(leftRatio.x, rightRatio.y);
                    verts.Add(vert);

                    vert.position = new Vector2(right.x, right.y);
                    vert.color = color;
                    vert.uv0 = rightRatio;

                    verts.Add(vert);


                    vert.position = new Vector2(right.x, left.y);
                    vert.color = color;
                    vert.uv0 = new Vector2(rightRatio.x, leftRatio.y);

                    verts.Add(vert);
                }
            }

        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        return true;
    }
}
