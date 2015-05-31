﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

namespace Gestures
{
    [AddComponentMenu("Event/Clean Hand Input Module")]
    public class CleanHandInputModule : PointerInputModule
    {
        enum HoverZone
        {
            Touch = 0,
            Near = 1,
            Far = 2
        }

        public HandProvider handProvider;
        public Camera eventCamera;
        public FingerType submitFinger = FingerType.Index;

        /// <summary>
        /// Maximum distance for UI objects. Everything farther away than that will
        /// be projected on a sphere at the specified distance.
        /// </summary>
        public float maxDistance = 0.5f;

        public float pressZone = 0.01f;
        public float nearZone = 0.1f;

        private HandState leftHandState;
        private HandState rightHandState;
        

        // Use this for initialization
        void Start()
        {
            if (eventCamera == null) eventCamera = Camera.main;
            if (handProvider == null) handProvider =  HandProvider.Instance;

            if (handProvider == null)
            {
                Debug.LogError("No hand provider found!");
            }
            if (eventCamera == null)
            {
                Debug.LogError("No camera found!");
            }

            leftHandState = new HandState();
            rightHandState = new HandState();

        }

        // Update is called once per frame
        public override void Process()
        {
            ProcessHand(HandType.Left);
            ProcessHand(HandType.Right);
        }



        /// <summary>
        /// Processes the events of a single hand.
        /// </summary>
        /// <param name="hand"></param>
        protected void ProcessHand(HandType type)
        {
            var hand = handProvider.GetHand(type);
            if (hand == null)
            {
                DisableHand(type);
                return;
            }

            var state = hand.IsLeft ? leftHandState : rightHandState;

            foreach (var finger in hand.Fingers)
            {
                ProcessFinger(finger, state);
            }

            var mainFinger = state.GetFingerState(submitFinger);

            // Process the first finger fully
            foreach (var finger in hand.Fingers)
            {
                var fState = state.GetFingerState(finger.Type);

                ProcessPress(fState.eventData, finger);
                ProcessMove(fState.eventData.buttonData);

                if(finger.Type == submitFinger)
                    ProcessDrag(fState.eventData.buttonData);
            }
        }

        private void DisableHand(HandType type)
        {
            var state = type == HandType.Left ? leftHandState : rightHandState;

            foreach (var fingerType in (FingerType[])System.Enum.GetValues(typeof(HandType)))
            {
                state.GetFingerState(fingerType).crosshair.Visible = false;
            }
        }

        private bool GetFingerData(int id, out PointerEventData data, bool create)
        {
            if (!m_PointerData.TryGetValue(id, out data) && create)
            {
                data = new FingerEventData(eventSystem)
                {
                    pointerId = id,
                    finger = null,
                    useDragThreshold = true,
                };
                m_PointerData.Add(id, data);
                eventSystem.pixelDragThreshold = 100;

                return true;
            }
            return false;
        }

        private bool GetFingerData(GenericFinger finger, out PointerEventData data, bool create)
        {
            return GetFingerData(GenericFinger.GetId(finger), out data, create);
        }


        /// <summary>
        /// Processes the events of a finger
        /// </summary>
        /// <param name="finger"></param>
        protected void ProcessFinger(GenericFinger finger, HandState state)
        {
            // Get event data
            PointerEventData data;
            bool created = GetFingerData(finger, out data, true);
    
            var prevFingerState = state.GetFingerState(finger.Type);
            var selection = prevFingerState.selection;
            float currentDistance = float.PositiveInfinity;


            // Get positions
            Vector3 worldPosition = finger.TipPosition;
            Vector3 screenPosition = eventCamera.WorldToScreenPoint(worldPosition);

            if (created)
                data.position = screenPosition;

            // Clear the used flag
            data.Reset();
            data.delta =  (Vector2)screenPosition - data.position;
            data.position = screenPosition;
            data.button = finger.Type == submitFinger
                          ? PointerEventData.InputButton.Left
                          : PointerEventData.InputButton.Middle;

            data.pointerCurrentRaycast = new RaycastResult();

            // Do the raycast
            eventSystem.RaycastAll(data, m_RaycastResultCache);

            // Process until one is OK

            foreach (var raycast in m_RaycastResultCache)
            {
                if (raycast.gameObject != null)
                {
                   

                    //if (finger.Type == submitFinger)
                    //    Debug.Log(raycast.gameObject.GetPath());

                    float z = raycast.distance;

                    //var offset = raycast.gameObject.GetComponentInChildren<CollisionOffset>();
                    //if (offset != null && offset.source == CollisionOffset.OffsetSource.Element)
                    //    z -= offset.offset;
                    //if (offset != null && offset.source == CollisionOffset.OffsetSource.Camera)
                    //    z = offset.offset;

                    var hitPoint = CalculateHitPoint(screenPosition, z);
                    var distance = GetPerpendicularDistance(worldPosition, hitPoint, raycast.gameObject.transform.forward);

                    bool isChild = data.pointerCurrentRaycast.gameObject
                                    ? raycast.gameObject.transform.IsChildOf(data.pointerCurrentRaycast.gameObject.transform)
                                    : false;
                    bool isParent = data.pointerCurrentRaycast.gameObject 
                                ? data.pointerCurrentRaycast.gameObject.transform.IsChildOf(raycast.gameObject.transform)
                                : false;

                    if (data.eligibleForClick && selection != null)
                    {
                        if (selection.IsSameGameObject(raycast.gameObject))
                        {
                            data.pointerCurrentRaycast = raycast;
                            data.worldPosition = hitPoint;

                            currentDistance = distance;
                        }
                        else continue;
                    }

                    if (isChild ||
                        (!isParent && Mathf.Abs(distance) < Mathf.Abs(currentDistance))
                    )
                    //if (selection != null && selection.valid 
                    //    && selection.IsSameGameObject(raycast.gameObject)
                    //    && selection.target.GetComponent<LooseGroup>() == null )
                    //{
                    //    data.pointerCurrentRaycast = raycast;
                    //    data.worldPosition = hitPoint;

                    //    currentDistance = distance;
                    //    if(finger.Type == submitFinger)
                    //        Debug.Log("BUBUBUBUBU");
                    //    break;
                    //} else if (Mathf.Abs(distance) < Mathf.Abs(currentDistance)
                    //  //  || raycast.gameObject.transform.IsChildOf(data.pointerCurrentRaycast.gameObject.transform)
                    //    )
                    {
                        data.pointerCurrentRaycast = raycast;
                        data.worldPosition = hitPoint;
                        
                        currentDistance = distance;
                    }
                }
            }


            if (data.pointerCurrentRaycast.isValid)
            {
                //if (finger.Type == submitFinger) Debug.Log(data.pointerCurrentRaycast.gameObject.name);

                if (selection != null)
                {

                    if (selection.IsSameGameObject(data.pointerCurrentRaycast.gameObject))
                    {
                        selection.Update(currentDistance);
                    }
                    else
                    {
                        //if (finger.Type == submitFinger)
                        //    Debug.Log("X: " + data.pointerCurrentRaycast.gameObject.GetPath());
                        // Deselect
                        if (!data.eligibleForClick)
                            selection.Discard();
                    }

                    prevFingerState.crosshair.Visible = finger.Type == submitFinger;
                    prevFingerState.crosshair.Value = selection.Proximity;
                }
                else
                {
                    selection = new Selection(data.pointerCurrentRaycast.gameObject, data.worldPosition);
                    selection.Update(currentDistance);
                }

                prevFingerState.crosshair.transform.position = CalculateHitPoint(screenPosition, data.pointerCurrentRaycast.distance, false);
            }
            else
            {
                //if (finger.Type == submitFinger) Debug.Log("Found nopthing");

                if (selection != null)
                {
                    selection.Discard();
                }
                prevFingerState.crosshair.Visible = false;

            }

            m_RaycastResultCache.Clear();
            
            state.SetFingerState(finger.Type, selection, data);
        }

        private Vector3 CalculateHitPoint(Vector2 screenPosition, float distance, bool cap = true)
        {
            distance += eventCamera.nearClipPlane;
            if (cap) distance = Mathf.Min(maxDistance, distance);

            return eventCamera.ScreenToWorldPoint(new Vector3(screenPosition.x,
                screenPosition.y,
                distance));
        }

        private float GetPerpendicularDistance(Vector3 dynamicPoint, Vector3 staticPoint, Vector3 normal)
        {
            var distance = dynamicPoint - staticPoint;
            return -Vector3.Dot(distance, normal);
        }

        private HoverZone JudgePoint(Vector3 dynamicPoint, Vector3 staticPoint, PointerEventData data)
        {
            var distance = dynamicPoint - staticPoint;
            var normal   = data.pointerCurrentRaycast.gameObject.transform.forward;
            var perpDistance = Mathf.Abs(Vector3.Dot(distance, normal));

            if (perpDistance <= pressZone)
            {
                return HoverZone.Touch;
            }
            else if (perpDistance <= nearZone)
            {
                return HoverZone.Near;
            }
            else
            {
                return HoverZone.Far;
            }
        }


        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessPress(MouseButtonEventData data, GenericFinger finger)
        {
            var pointerEvent = (FingerEventData)data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
            bool submittable = finger.Type == submitFinger;

            pointerEvent.finger = finger;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                //Debug.LogFormat("PRESS {0}", currentOverGo.GetPath());
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                float time = Time.unscaledTime;

                // FingerDown
                pointerEvent.fingerDown = ExecuteEvents.ExecuteHierarchy<IFingerDownHandler>(currentOverGo, pointerEvent, FingerDownHandler);

                if (submittable)
                {
                    DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                    // search for the control that will receive the press
                    // if we can't find a press handler set the press
                    // handler to be what would receive a click.
                    var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                    // didnt find a press handler... search for a click handler
                    if (newPressed == null)
                        newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                    // Debug.Log("Pressed: " + newPressed);


                    if (newPressed == pointerEvent.lastPress)
                    {
                        var diffTime = time - pointerEvent.clickTime;
                        if (diffTime < 0.3f)
                            ++pointerEvent.clickCount;
                        else
                            pointerEvent.clickCount = 1;

                        pointerEvent.clickTime = time;
                    }
                    else
                    {
                        pointerEvent.clickCount = 1;
                    }

                    pointerEvent.pointerPress = newPressed;
                    pointerEvent.rawPointerPress = currentOverGo;

                    //Debug.LogFormat("CLICK {0}", currentOverGo.GetPath());

                    pointerEvent.clickTime = time;

                    // Save the drag handler as well
                    pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                    if (pointerEvent.pointerDrag != null)
                        ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                }
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
            {

                ExecuteEvents.Execute<IFingerUpHandler>(pointerEvent.fingerDown, pointerEvent, FingerUpHandler);

                if (submittable)
                {
                    //Debug.LogFormat("RELEASE {0}", pointerEvent.pointerPress.gameObject.GetPath());

                    // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                    // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                    // see if we mouse up on the same element that we clicked on...
                    var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                    // PointerClick and Drop events
                    if (/*pointerEvent.pointerPress == pointerUpHandler && */pointerEvent.eligibleForClick)
                    {
                        ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                    }
                    else if (pointerEvent.pointerDrag != null)
                    {
                        ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                    }
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                // redo pointer enter / exit to refresh state
                // so that if we moused over somethign that ignored it before
                // due to having pressed on something else
                // it now gets it.
                if (currentOverGo != pointerEvent.pointerEnter)
                {
                    HandlePointerExitAndEnter(pointerEvent, null);
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                }
            }
        }

        private void FingerDownHandler(IFingerDownHandler handler, BaseEventData eventData)
        {
            var evt = ExecuteEvents.ValidateEventData<FingerEventData>(eventData);

            PointerEventData submitEventData;
            GetFingerData(GenericFinger.GetId(submitFinger, evt.finger.Hand.IsLeft ? HandType.Left : HandType.Right), out submitEventData, true);

            var submitEvt = ExecuteEvents.ValidateEventData<FingerEventData>(submitEventData);

            handler.OnFingerDown(evt, submitEvt);
        }

        private void FingerUpHandler(IFingerUpHandler handler, BaseEventData eventData)
        {
            var evt = ExecuteEvents.ValidateEventData<FingerEventData>(eventData);

            PointerEventData submitEventData;
            GetFingerData(GenericFinger.GetId(submitFinger, evt.finger.Hand.IsLeft ? HandType.Left : HandType.Right), out submitEventData, true);

            var submitEvt = ExecuteEvents.ValidateEventData<FingerEventData>(submitEventData);

            handler.OnFingerUp(evt, submitEvt);
        }




        #region Nested Classes

        /// <summary>
        /// Adapted from: https://bitbucket.org/Unity-Technologies/ui/src/fd5d3578da8c33883b7c56dd2b2f4d4bbc87095b/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs?at=4.6
        /// </summary>
        protected class HandState
        {
            private List<FingerState> m_TrackedFingers = new List<FingerState>();

            public HandState()
            {
            }
            public bool AnyPressesThisFrame()
            {
                for (int i = 0; i < m_TrackedFingers.Count; i++)
                {
                    if (m_TrackedFingers[i].eventData.PressedThisFrame())
                        return true;
                }
                return false;
            }

            public bool AnyReleasesThisFrame()
            {
                for (int i = 0; i < m_TrackedFingers.Count; i++)
                {
                    if (m_TrackedFingers[i].eventData.ReleasedThisFrame())
                        return true;
                }
                return false;
            }

            public FingerState GetFingerState(FingerType finger)
            {
                FingerState tracked = null;
                for (int i = 0; i < m_TrackedFingers.Count; i++)
                {
                    if (m_TrackedFingers[i].finger == finger)
                    {
                        tracked = m_TrackedFingers[i];
                        break;
                    }
                }

                if (tracked == null)
                {
                    // TODO: set finger
                    tracked = new FingerState { finger = finger, eventData = new MouseButtonEventData() };
                    tracked.crosshair = (GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Crosshair")) as GameObject).GetComponent<CrosshairControl>();
                    tracked.crosshair.Visible = false;
                    m_TrackedFingers.Add(tracked);
                }
                return tracked;
            }

            public void SetFingerState(FingerType finger, Selection selection, PointerEventData data)
            {
                var toModify = GetFingerState(finger);
                toModify.eventData.buttonState = selection == null 
                    ? PointerEventData.FramePressState.NotChanged
                    : selection.GetFrameState();

                //if (finger == FingerType.Index && selection != null)
                //    Debug.LogFormat("{0} ({1})", toModify.eventData.buttonState, selection != null ? selection.distance : 1000);

                toModify.eventData.buttonData = data;
                toModify.selection = selection;

                if (selection != null && !selection.valid) {
                    toModify.selection = null;
                    data.pointerCurrentRaycast = new RaycastResult();
                }

                //toModify.crosshair.Value = distance;
            }

        }

        protected class FingerState
        {
            public UnityEngine.EventSystems.PointerInputModule.MouseButtonEventData eventData
            {
                get;
                set;
            }

            public FingerType finger
            {
                get;
                set;
            }

            public CrosshairControl crosshair { get; set; }

            public Selection selection = null;

            public GameObject pressedObject { get; set; }
            public GameObject selectedObject { get; set; }

        }

        protected class Selection
        {
            public GameObject target;
            private GameObject handler;
            private Vector3 hitPoint;
            public float selectionTime;
            public float distance;
            public bool pressed = false;

            public float selectionDistance = 0.2f;
            public float pressDistance = 0.01f;
            public float selectionDuration = 5;
            private float timeThreshold = 0;


            public bool valid = true;

            private PointerEventData.FramePressState pressState;

            public Selection(GameObject target, Vector3 hitPoint)
            {
                this.target = target;
                this.handler = GetEventHandler(target);
                this.hitPoint = target.transform.InverseTransformPoint(hitPoint);
                selectionTime = Time.time;
            }

            public Vector3 GetHitPoint()
            {
                return target.transform.TransformPoint(hitPoint);
            }

            public float Proximity
            {
                get
                {
                    return 1 - Mathf.Clamp01((distance - pressDistance) / (selectionDistance - pressDistance));
                }
            }

            public bool IsSameGameObject(GameObject go)
            {
                if (handler == null) return false;
                return GetEventHandler(go) == handler;
            }

            private GameObject GetEventHandler(GameObject go)
            {
                var candidate = ExecuteEvents.GetEventHandler<IEventSystemHandler>(go);
                return candidate;
            }

            public void Discard()
            {
                valid = false;
            }
            public void Update(float distance)
            {
                pressState = PointerEventData.FramePressState.NotChanged;
                this.distance = distance;
                if (!pressed) distance = Mathf.Abs(distance);


                if (pressed && distance > pressDistance)
                {
                    timeThreshold += Time.deltaTime;

                    if (timeThreshold > 0.1f)
                    {
                        valid = false;
                       
                        //Debug.Log(distance);
                        pressState = PointerEventData.FramePressState.Released;
                        pressed = false;
                        
                    }
                }
                else if(distance > selectionDistance) {
                    valid = false;
                   
                    
                } else if(!pressed && 
                    (distance < pressDistance /*|| Time.time - selectionTime > selectionDuration*/)) {
                        timeThreshold = 0;
                        pressed = true;
                        pressState = PointerEventData.FramePressState.Pressed;
                    }
                else
                {
                    timeThreshold = 0;
                }
                
            }

            internal PointerEventData.FramePressState GetFrameState()
            {
                return pressState;
            }
        }


        #endregion
    }

}