using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialLayout : UnityEngine.UI.LayoutGroup {
    public float minRadius = 1000;

    public float Radius {get; private set;}
    public float UnitRadius { get { return Radius / Tile.PIXELS_PER_UNIT; } }

    public float elementHeight = 50;

    public float minAngle;
    public float maxAngle;

    public override void CalculateLayoutInputVertical()
    {
        //SetLayoutInputForAxis(Radius * 2, Radius * 2, Radius * 2, 1);
    }

    public override void SetLayoutHorizontal()
    {
        int axis = 0;
        float size = rectTransform.rect.size[0];
        float innerSize = size - padding.horizontal;
        foreach (var child in  rectChildren)
        {
            float min = LayoutUtility.GetMinSize(child, axis);
            float preferred = LayoutUtility.GetPreferredSize(child, axis);
            float flexible = LayoutUtility.GetFlexibleSize(child, axis);
          //  if ((axis == 0 ? childForceExpandWidth : childForceExpandHeight))
                flexible = Mathf.Max(flexible, 1);

            float requiredSpace = Mathf.Clamp(innerSize, min, flexible > 0 ? size : preferred);
            float startOffset = GetStartOffset(axis, requiredSpace);
            SetChildAlongAxis(child, axis, startOffset, requiredSpace);
        }
    }

    public override void SetLayoutVertical()
    {
        if (transform.childCount == 0)
        {
            Radius = 0;
            return;
        }
        Radius = Mathf.Max(minRadius, elementHeight / (2 * Mathf.Tan(Mathf.PI / transform.childCount)));
        int sideCount = Mathf.CeilToInt(Mathf.PI / Mathf.Atan(elementHeight / Radius));

        float ratio = Mathf.PI * 2 / sideCount;
        float offset = rectTransform.rect.height / 2;
        minAngle = 0;
        maxAngle = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            var rect = rectChildren[i];
            
            float angle = (i - rectChildren.Count / 2) * ratio;
            float angleDeg = angle * 180 / Mathf.PI;

            //LayoutUtility.Get
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, Mathf.Sin(angle) * Radius + offset, elementHeight);
            rect.localPosition = new Vector3(rect.localPosition.x, rect.localPosition.y, -Mathf.Cos(angle) * Radius);
            rect.localRotation = Quaternion.Euler(-angleDeg, 0, 0);

            minAngle = Mathf.Min(minAngle, angleDeg);
            maxAngle = Mathf.Max(maxAngle, angleDeg);
            Debug.Log(angleDeg);
        }
        Debug.Log(offset);
    }
}
