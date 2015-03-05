using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[ExecuteInEditMode]
public class RichQuad : Image {
    public int xSegments = 5;
    public int ySegments = 5;
    public float segmentWidth = 1;
    public int iterations = 4;

    protected override void OnFillVBO(List<UIVertex> baseVbo)
    {
        // Make base
        base.OnFillVBO(baseVbo);

        // Make more segments
        List<UIVertex> vbo = new List<UIVertex>();
        for (int i = 0; i < baseVbo.Count; i += 4)
        {
            SplitQuad(baseVbo[i], baseVbo[i + 1], baseVbo[i + 2], baseVbo[i + 3], iterations, vbo);
        }

        baseVbo.Clear();
        baseVbo.AddRange(vbo);
    }

    protected void SplitQuad(UIVertex topLeft, UIVertex topRight, UIVertex bottomRight, UIVertex bottomLeft, int iterations, List<UIVertex> vbo)
    {
        if (iterations > 0)
        {
            UIVertex topCenter = UIVertex.simpleVert;
            UIVertex bottomCenter = UIVertex.simpleVert;
            UIVertex leftCenter = UIVertex.simpleVert;
            UIVertex rightCenter = UIVertex.simpleVert;
            UIVertex center = UIVertex.simpleVert;

            FillUIVertex(topLeft, bottomRight, 0.5f, ref center);
            FillUIVertex(topLeft, topRight, 0.5f, ref topCenter);
            FillUIVertex(topLeft, bottomLeft, 0.5f, ref leftCenter);
            FillUIVertex(topRight, bottomRight, 0.5f, ref rightCenter);
            FillUIVertex(bottomLeft, bottomRight, 0.5f, ref bottomCenter);

            // Top left quad
            SplitQuad(topLeft, topCenter, center, leftCenter, --iterations, vbo);
            SplitQuad(topCenter, topRight, rightCenter, center, iterations, vbo);
            SplitQuad(leftCenter, center, bottomCenter, bottomLeft, iterations, vbo);
            SplitQuad(center, rightCenter, bottomRight, bottomCenter, iterations, vbo);
        }
        else
        {
            vbo.AddRange(new UIVertex[] {
                topLeft, topRight, bottomRight, bottomLeft
            });
        }
    }

    private void FillUIVertex(UIVertex v1, UIVertex v2, float ratio, ref UIVertex v)
    {
        v.color = Color32.Lerp(v1.color, v2.color, ratio);
        v.normal = Vector3.Lerp(v1.normal, v2.normal, ratio);
        v.position = Vector3.Lerp(v1.position, v2.position, ratio);
        v.uv0 = Vector2.Lerp(v1.uv0, v2.uv0, ratio);
        v.uv1 = Vector2.Lerp(v1.uv1, v2.uv1, ratio);
    }

    protected void OnFillVBO2(System.Collections.Generic.List<UIVertex> vbo)
    {

        Vector2 corner1 = Vector2.zero;
        Vector2 corner2 = Vector2.zero;

        corner1.x = 0f;
        corner1.y = 0f;
        corner2.x = 1f;
        corner2.y = 1f;

        corner1.x -= rectTransform.pivot.x;
        corner1.y -= rectTransform.pivot.y;
        corner2.x -= rectTransform.pivot.x;
        corner2.y -= rectTransform.pivot.y;

        corner1.x *= rectTransform.rect.width;
        corner1.y *= rectTransform.rect.height;
        corner2.x *= rectTransform.rect.width;
        corner2.y *= rectTransform.rect.height;

        vbo.Clear();

        UIVertex vert = UIVertex.simpleVert;

        var distance = corner2 - corner1;

        for (int xStep = 0; xStep < Mathf.Max(1, xSegments); xStep++)
        {
            for (int yStep = 0; yStep < Mathf.Max(1, ySegments); yStep++)
            {
                var leftRatio = new Vector2((float)xStep / xSegments, (float)yStep / ySegments);
                var rightRatio = new Vector2((float)(xStep + 1) / xSegments, (float)(yStep + 1) / ySegments);
                    
                var left = corner1 + new Vector2((float)xStep / xSegments * distance.x, (float)yStep / ySegments * distance.y);
                var right = corner1 + new Vector2((float)(xStep+1) / xSegments * distance.x, (float)(yStep+1) / ySegments * distance.y);
                
                vert.position = new Vector2(left.x, left.y);
                vert.color = color;
                vert.uv0 = leftRatio; 
                vbo.Add(vert);

                vert.position = new Vector2(left.x, right.y);
                vert.color = color;
                vert.uv0 = new Vector2(leftRatio.x, rightRatio.y); 

                vbo.Add(vert);

                vert.position = new Vector2(right.x, right.y);
                vert.color = color;
                vert.uv0 = rightRatio;
                vbo.Add(vert);

                vert.position = new Vector2(right.x, left.y);
                vert.color = color;
                vert.uv0 = new Vector2(rightRatio.x, leftRatio.y); 

                vbo.Add(vert);
            }
        }
    }
}
