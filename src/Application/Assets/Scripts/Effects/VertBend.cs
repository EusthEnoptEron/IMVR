using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Modify the given vertices so they follow a sin curve
 */
public class VertBend : BaseVertexEffect
{
    public float scale = 10f;
    public float amplitude = 35f;
    public float phase = 0f;
    public bool normalized = false;


    public override void ModifyVertices(List<UIVertex> verts)
    {
        if (!IsActive())
            return;

        float scale = this.scale;
        if (normalized)
        {
            scale *= (1 / GetComponent<RectTransform>().rect.width) * Mathf.PI * 2;
        }

        for (int index = 0; index < verts.Count; index++)
        {
            var uiVertex = verts[index];

            uiVertex.position.z = Mathf.Sin(uiVertex.position.x * scale + phase) * amplitude;
            verts[index] = uiVertex;
        }
    }
}