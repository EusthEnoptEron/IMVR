using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Gestures;


public class FingerEventData : PointerEventData
{
    public GenericFinger finger;
    public GameObject fingerDown;
    public bool occupied = false;
    public FingerEventData(EventSystem eventSystem) : base(eventSystem)
    {
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
