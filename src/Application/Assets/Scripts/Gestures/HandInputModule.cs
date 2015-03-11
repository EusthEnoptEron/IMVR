using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using Gestures;
using Debug = UnityEngine.Debug;

namespace Gestures
{
    [AddComponentMenu("Event/Hand Input Module")]
    public class HandInputModule : PointerInputModule
    {
        private class HandEventData
        {
            public PointerEventData Data;
            public GameObject Selection;
            public TouchPhase Phase;

            public HandEventData(EventSystem eventSystem)
            {
                Data = new PointerEventData(eventSystem);
                Selection = null;
                Phase = TouchPhase.Stationary;
            }
        }
        public GameObject selectedCanvas;

        public HandProvider handProvider;

        public Camera camera;
        public bool submitOnFingerDown = true;

        private GameObject selection;
        public Transform pointer;

        private Dictionary<bool, HandEventData> handEvents = new Dictionary<bool, HandEventData>();
        private Dictionary<bool, CanvasDragEventData> dragEvents = new Dictionary<bool, CanvasDragEventData>();

        private bool canvasDeselectable = true;
        private float selectedTime;

        private float timeoutSelection = float.PositiveInfinity;

        private GameObject leftMaru;
        private GameObject rightMaru;
        public float sensitiveRegion = 0.02f;
        void Start()
        {
            if (camera == null) camera = Camera.main;

            handEvents[false] = new HandEventData(eventSystem);
            handEvents[true] = new HandEventData(eventSystem);
            dragEvents[false] = new CanvasDragEventData(eventSystem);
            dragEvents[true] = new CanvasDragEventData(eventSystem);

            leftMaru = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightMaru = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            leftMaru.transform.localScale *= 0.03f;
            rightMaru.transform.localScale *= 0.03f;

            //leftMaru.SetActive(false);
            //rightMaru.SetActive(false);


        }

        public override void Process()
        {
            if (handProvider == null) return;
            var leftHand = handProvider.GetHand(HandType.Left, NoHandStrategy.DelayedSetNull);
            var rightHand = handProvider.GetHand(HandType.Right, NoHandStrategy.DelayedSetNull);

            if (!ProcessDragging(leftHand, rightHand))
            {
                ProcessHand(leftHand);
                ProcessHand(rightHand);
            }
        }

        private bool ProcessDragging(GenericHand leftHand, GenericHand rightHand)
        {

            if (selectedCanvas != null && leftHand != null && rightHand != null)
            {
                var index1 = leftHand.GetFinger(FingerType.Index).Extended
                                ? leftHand.GetFinger(FingerType.Index)
                                : leftHand.GetFinger(FingerType.Middle);

                var thumb1 = leftHand.GetFinger(FingerType.Thumb);

                var index2 = rightHand.GetFinger(FingerType.Index).Extended
                                ? rightHand.GetFinger(FingerType.Index)
                                : rightHand.GetFinger(FingerType.Middle);

                var thumb2 = rightHand.GetFinger(FingerType.Thumb);

                var p1 = index1.Bones[1].Position;
                var p2 = index2.Bones[1].Position;

                // Dragging phase
                if (Math.Abs(Vector3.Dot(thumb1.Direction, index1.Direction)) < 0.7 &&
                    Math.Abs(Vector3.Dot(thumb2.Direction, index2.Direction)) < 0.7 &&
                    (Vector3.Dot(thumb1.LocalDirection, thumb2.LocalDirection)) < -0.1 &&
                    thumb1.Extended && thumb2.Extended && index1.Extended && index2.Extended
                    )
                //HandPosition.DuckHandRight.IsActive(rightHand))
                {
                    // Center
                    var p = (p1 + p2) / 2f;
                    // Normal
                    var n = ((Vector3.Cross(index1.Direction, thumb1.Direction).normalized
                        - Vector3.Cross(index2.Direction, thumb2.Direction).normalized) / 2f).normalized;
                    //n = Vector3.Cross(index1.Direction, thumb1.Direction).normalized;


                    var up = ((index2.Direction + index1.Direction) / 2).normalized;

                    // Should always point toward the camera!
                    if (Vector3.Dot(-n, camera.transform.forward) < 0)
                    {
                        n = -n;
                    }
                    var q = Quaternion.LookRotation(-n);

                    var size = new Vector2();

                    ExecuteEvents.Execute<IMovableCanvas>(selectedCanvas, new BaseEventData(eventSystem), (obj, e) =>
                    {
                        obj.OnMove(p, size, q);
                    });


                    selectedTime = Time.time;
                    timeoutSelection = 2;

                    return true;
                }
            }

            if (Time.time - selectedTime > 1)
            {
                if (selectedCanvas == null || canvasDeselectable)
                {
                    // Selection phase
                    var pinchedCanvas = GetPinchedCanvas(rightHand);
                    if (pinchedCanvas == null) pinchedCanvas = GetPinchedCanvas(leftHand);

                    if (pinchedCanvas != null)
                    {
                        if (ExecuteEvents.CanHandleEvent<ISelectableCanvas>(pinchedCanvas))
                        {

                            SelectCanvas(pinchedCanvas);
                            return true;
                        }
                    }
                }
            }

            if (!float.IsInfinity(timeoutSelection))
            {
                timeoutSelection -= Time.deltaTime;

                if (timeoutSelection < 0)
                {
                    SelectCanvas(selectedCanvas);
                }

            }
            return false;

        }

        private void SelectCanvas(GameObject canvas)
        {
            if (selectedCanvas != null)
            {
                ExecuteEvents.Execute<ISelectableCanvas>(selectedCanvas, new BaseEventData(eventSystem), (obj, e) =>
                {
                    obj.OnUnselect(e);
                });
            }


            if (selectedCanvas == canvas)
            {
                selectedCanvas = null;
            }
            else
            {
                selectedCanvas = canvas;
                canvasDeselectable = false;


                // Invoke
                ExecuteEvents.Execute<ISelectableCanvas>(selectedCanvas, new BaseEventData(eventSystem), (obj, e) =>
                {
                    obj.OnSelect(e);
                });
            }

            selectedTime = Time.time;
            timeoutSelection = float.PositiveInfinity;
        }

        private void ProcessHand(GenericHand hand)
        {
            if (hand == null) return;

            GenericFinger index = hand.GetFinger(FingerType.Index);
            Vector3 worldPosition = index.TipPosition;
            Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

            // Debug
            if (hand.IsLeft) leftMaru.transform.position = worldPosition;
            else rightMaru.transform.position = worldPosition;
            // ------

            var evt = handEvents[hand.IsLeft];
            var eventData = evt.Data;

            eventData.Reset();

            eventData.delta = (Vector2)screenPosition - eventData.position;
            eventData.position = screenPosition;
            eventData.button = PointerEventData.InputButton.Left;

            eventSystem.RaycastAll(eventData, m_RaycastResultCache);

            var raycast = FindFirstRaycast(m_RaycastResultCache);
            eventData.pointerCurrentRaycast = raycast;
            eventData.worldPosition = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, raycast.distance + camera.nearClipPlane));

            var state = GetState(hand.GetFinger(FingerType.Index), hand.IsLeft);
            evt.Phase = state;
            if (state == TouchPhase.Began || state == TouchPhase.Moved)
                evt.Selection = raycast.gameObject;
            else
                evt.Selection = null;


            ProcessTouchPress(eventData, state == TouchPhase.Began, state == TouchPhase.Ended);

            if (state == TouchPhase.Moved && !ExecuteEvents.CanHandleEvent<IPointerClickHandler>(eventData.selectedObject))
            {
                // ---T-O-D-O--: Fix Drag 'n Drop
                ProcessMove(eventData);
                ProcessDrag(eventData);
            }

            if (state == TouchPhase.Ended || state == TouchPhase.Canceled)
            {
                if (canvasDeselectable == false)
                {
                    canvasDeselectable = true;
                    selectedTime = Time.time;
                }
            }


            //var dragEvt = dragEvents[hand.IsLeft];
            //if (CheckCanvasDragging(hand))
            //{
            //    if (dragEvt.dragging)
            //    {
            //        if(dragEvt.worldDelta != Vector3.zero)
            //            ExecuteEvents.Execute<ICanvasDragHandler>(dragEvt.canvas,
            //           dragEvt, (o, e) => { o.OnDragMove((CanvasDragEventData)e); });
            //    }
            //    else
            //    {
            //        dragEvt.dragging = true;

            //        ExecuteEvents.Execute<ICanvasDragHandler>(dragEvt.canvas,
            //           dragEvt, (o, e) => { o.OnDragStart((CanvasDragEventData)e); });
            //    }
            //}
            //else
            //{
            //    if (dragEvt.dragging)
            //    {
            //        dragEvt.dragging = false;
            //        ExecuteEvents.Execute<ICanvasDragHandler>(dragEvt.canvas, 
            //            dragEvt, (o, e) => { o.OnDragEnd((CanvasDragEventData)e); });
            //    }
            //    dragEvt.canvas = null;
            //}

        }

        private GameObject GetPinchedCanvas(GenericHand hand)
        {
            if (hand == null) return null;
            // if(hand.IsLeft) return null;

            //    if (HandPosition.PinchRight.IsActive(hand, 0.4f))
            {
                //OculusDebug.Log("Pinch");
                // We're pinching. A canvas?
                var evt = new PointerEventData(eventSystem);
                GameObject selectedCanvas = null;


                foreach (var finger in hand.Fingers)
                {
                    if (finger.Type == FingerType.Index || finger.Type == FingerType.Middle)
                    {
                        if (!finger.Extended) return null;
                        evt.Reset();
                        evt.position = camera.WorldToScreenPoint(finger.TipPosition);
                        float distance = Vector3.Distance(finger.TipPosition, camera.transform.position) + 0.01f;

                        eventSystem.RaycastAll(evt, m_RaycastResultCache);
                        var raycast = FindFirstRaycast(m_RaycastResultCache);

                        if (raycast.isValid && raycast.distance < distance && (distance - raycast.distance) < sensitiveRegion)
                        {
                            //if (ExecuteEvents.CanHandleEvent<IPointerClickHandler>(raycast.gameObject)) return null;

                            var canvas = raycast.gameObject.GetComponentInParent<Canvas>();
                            if (canvas == null) return null;
                            if (selectedCanvas != null && canvas.gameObject != selectedCanvas) return null;

                            selectedCanvas = canvas.gameObject;
                        }
                        else return null;
                    }
                    else if (finger.Type != FingerType.Thumb && finger.Extended) return null;
                }

                return selectedCanvas;
            }

            return null;
        }

        private bool CheckCanvasDragging(GenericHand hand)
        {
            var evt = dragEvents[hand.IsLeft];
            evt.Reset();
            evt.worldDelta = Vector3.zero;
            if (evt.Hand != null)
            {
                evt.worldDelta = hand.PalmPosition - evt.Hand.PalmPosition;
                evt.deltaRotation = Quaternion.FromToRotation(evt.Hand.PalmNormal, hand.PalmNormal);
            }
            evt.Hand = hand;

            float dotProduct = Vector3.Dot(hand.PalmNormal, (hand.PalmPosition - camera.transform.position).normalized);
            if (dotProduct < 0)
            {
                return false;
            }
            else if (evt.dragging)
            {
                return true;
            }

            GameObject selectedCanvas = evt.canvas;
            foreach (var finger in hand.Fingers)
            {
                evt.Reset();
                evt.position = camera.WorldToScreenPoint(finger.TipPosition);
                float distance = Vector3.Distance(finger.TipPosition, camera.transform.position) + 0.01f;

                eventSystem.RaycastAll(evt, m_RaycastResultCache);
                var raycast = FindFirstRaycast(m_RaycastResultCache);

                if (raycast.isValid && raycast.distance < distance)
                {
                    var canvas = raycast.gameObject.GetComponentInParent<Canvas>();
                    if (canvas == null) return false;
                    if (selectedCanvas != null && canvas.gameObject != selectedCanvas) return false;

                    selectedCanvas = canvas.gameObject;
                }
                else return false;
            }
            evt.canvas = selectedCanvas;
            return true;
        }

        private TouchPhase GetState(GenericFinger finger, bool isLeft)
        {
            var eventData = handEvents[isLeft];
            var selection = eventData.Selection;
            var raycast = eventData.Data.pointerCurrentRaycast;
            //var collisionPoin
            float distance = Vector3.Distance(finger.TipPosition, camera.transform.position);

            if (raycast.isValid)
            {
                float collisionDistance = Vector3.Distance(eventData.Data.worldPosition, camera.transform.position);
                Debug.Log(String.Format("{0} - {1}", distance, collisionDistance));

                //leftMaru.transform.position = camera.transform.position + raycast.distance * (hand.GetFinger(FingerType.Index).TipPosition - camera.transform.position).normalized;
                //leftMaru.transform.rotation =
                //    Quaternion.LookRotation(Vector3.up, camera.transform.position - leftMaru.transform.position);
                // We hit something
                if (distance < collisionDistance || (distance - collisionDistance) > sensitiveRegion)
                {
                    // But we ain't clicking it
                    if (eventData.Selection == null)
                    {
                        return TouchPhase.Canceled;
                    }
                    else
                    {
                        return TouchPhase.Ended;
                    }
                }
                else
                {
                    // We ARE clicking it
                    if (selection == null)
                    {
                        return TouchPhase.Began;
                    }
                    else
                    {
                        return TouchPhase.Moved;
                    }
                }
            }
            else
            {
                if (selection != null)
                {
                    return TouchPhase.Ended;
                }
                else
                {
                    return TouchPhase.Canceled;
                }

            }
        }


        private void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
        {
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;
            if (pressed) OculusDebug.Log("Pressed");
            if (released) OculusDebug.Log("Released");
            // PointerDown notification
            if (pressed)
            {
                pointerEvent.eligibleForClick = !submitOnFingerDown;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                if (pointerEvent.pointerEnter != currentOverGo)
                {
                    // send a pointer enter to the touched element if it isn't the one to select...
                    HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                    pointerEvent.pointerEnter = currentOverGo;
                }

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                Debug.Log("Pressed: " + newPressed);

                float time = Time.unscaledTime;

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

                pointerEvent.clickTime = time;

                // Save the drag handler as well
                pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);

                if (submitOnFingerDown && ExecuteEvents.CanHandleEvent<IPointerClickHandler>(pointerEvent.pointerPress))
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);

            }

            // PointerUp notification
            if (released)
            {
                // Debug.Log("Executing pressup on: " + pointer.pointerPress);
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

                // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

                // see if we mouse up on the same element that we clicked on...
                var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // PointerClick and Drop events
                if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
                {
                    ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
                }
                else if (pointerEvent.pointerDrag != null)
                {
                    ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
                }

                pointerEvent.eligibleForClick = false;
                pointerEvent.pointerPress = null;
                pointerEvent.rawPointerPress = null;

                if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.dragging = false;
                pointerEvent.pointerDrag = null;

                if (pointerEvent.pointerDrag != null)
                    ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

                pointerEvent.pointerDrag = null;

                // send exit events as we need to simulate this on touch up on touch device
                ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
                pointerEvent.pointerEnter = null;
            }
        }

        public override bool IsPointerOverGameObject(int pointerId)
        {
            return true;
        }

    }

}