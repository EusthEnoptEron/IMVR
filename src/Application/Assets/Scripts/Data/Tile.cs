using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using VirtualHands.Data;
using IMVR.Commons;

[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(RectTransform))]
public abstract class Tile : UIBehaviour {
    public int width = 1;
    public int height = 1;

    public File File { get; set; }

    public Vector3 targetPosition;
    public Quaternion targetRotation;
    public Vector3 targetScale;

    public float animationSpeed = 5;
    public const int PIXELS_PER_UNIT = 100;

    protected abstract void OnDestroy();

    //protected virtual void Update()
    //{
    //    transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, animationSpeed * Time.deltaTime);
    //    transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
    //    transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, animationSpeed * Time.deltaTime);
    //}
}
