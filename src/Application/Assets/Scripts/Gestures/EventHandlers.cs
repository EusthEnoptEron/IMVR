using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Gestures;


public class FingerEventData : PointerEventData
{
    public GenericFinger finger;
    public GameObject fingerDown;
    public bool occupied = false;

    private Vector3 _pressPoint;
    public Vector3 pressPoint { get { return _pressPoint; } set { _pressPoint = value; /* indicator.position = value; */ } }

    //private Transform indicator;
    public FingerEventData(EventSystem eventSystem) : base(eventSystem)
    {
        //indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
        //indicator.localScale = Vector3.one * 0.01f;
    }
}

public interface IFingerDownHandler : IEventSystemHandler
{
    void OnFingerDown(FingerEventData eventData, FingerEventData submitFinger);
}

public interface IFingerUpHandler : IEventSystemHandler
{
    void OnFingerUp(FingerEventData eventData, FingerEventData submitFinger);
}
