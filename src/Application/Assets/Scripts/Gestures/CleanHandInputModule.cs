using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;


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

        public float pressZone = 0.01f;
        public float nearZone = 0.1f;

        private HandState leftHandState;
        private HandState rightHandState;


        // Use this for initialization
        void Start()
        {
            handProvider = handProvider ?? GameObject.FindObjectOfType<HandProvider>();
            eventCamera = eventCamera ?? Camera.main;

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
            ProcessHand(handProvider.GetHand(HandType.Left));
            ProcessHand(handProvider.GetHand(HandType.Right));
        }



        /// <summary>
        /// Processes the events of a single hand.
        /// </summary>
        /// <param name="hand"></param>
        protected void ProcessHand(GenericHand hand)
        {
            if (hand == null) return;

            var state = hand.IsLeft ? leftHandState : rightHandState;

            foreach (var finger in hand.Fingers)
            {
                ProcessFinger(finger, state);
            }

            var mainFinger = state.GetFingerState(submitFinger);

            // Process the first finger fully
            ProcessPress(mainFinger.eventData);
            ProcessMove(mainFinger.eventData.buttonData);
            ProcessDrag(mainFinger.eventData.buttonData);
        }


        /// <summary>
        /// Processes the events of a finger
        /// </summary>
        /// <param name="finger"></param>
        protected void ProcessFinger(GenericFinger finger, HandState state)
        {
            // Get event data
            PointerEventData data;
            bool created = GetPointerData(GetButtonId(finger), out data, true);
            var prevFingerState = state.GetFingerState(finger.Type);
            GameObject pressedObject = null;
            GameObject selectedObject = null;


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

            // Do the raycast
            eventSystem.RaycastAll(data, m_RaycastResultCache);


            // Process until one is OK
            foreach (var raycast in m_RaycastResultCache)
            {
                if (raycast.gameObject != null)
                {
                    var hitPoint = eventCamera.ScreenToWorldPoint(
                        new Vector3(screenPosition.x,
                                    screenPosition.y, 
                                    raycast.distance + eventCamera.nearClipPlane)
                    );

                    data.pointerCurrentRaycast = raycast;

                    var hoverZone = JudgePoint(worldPosition, hitPoint, data);

                    if (hoverZone < HoverZone.Far)
                    {
                        data.worldPosition = hitPoint;

                        if (hoverZone == HoverZone.Touch)
                        {
                            // We're touching and no other candidate
                            pressedObject = raycast.gameObject;
                            break;
                        }
                        else if (prevFingerState.pressedObject == raycast.gameObject) {
                            // Keep pressed
                            pressedObject = raycast.gameObject;
                            break;
                        }
                        else
                        {
                            // Select!
                            selectedObject = raycast.gameObject;
                        }
                    }

                    break;
                }
            }

            m_RaycastResultCache.Clear();

            PointerEventData.FramePressState buttonState;
            if (pressedObject != null && prevFingerState.pressedObject == null)
            {
                buttonState = PointerEventData.FramePressState.Pressed;
            }
            else if (pressedObject != null && pressedObject != prevFingerState.pressedObject)
            {
                buttonState = PointerEventData.FramePressState.Released;
            }
            else if(pressedObject == null && prevFingerState.pressedObject != null)
            {
                buttonState = PointerEventData.FramePressState.Released;
            }
            else if (selectedObject == null && prevFingerState.selectedObject != null)
            {
                DeselectIfSelectionChanged(null, data);
                buttonState = PointerEventData.FramePressState.NotChanged;
            }
            else
            {
                buttonState = PointerEventData.FramePressState.NotChanged;
            }

            if (pressedObject != null)
            {
                prevFingerState.pressedObject = pressedObject;
                prevFingerState.selectedObject = null;
            }
            else
            {
                prevFingerState.pressedObject = null;
                prevFingerState.selectedObject = selectedObject;
            }

            state.SetFingerState(finger.Type, buttonState, data);
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


        private int GetButtonId(GenericFinger finger)
        {
            return (int)finger.Type + (finger.Hand.IsLeft ? 5 : 0);
        }


        /// <summary>
        /// Process the current mouse press.
        /// </summary>
        private void ProcessPress(MouseButtonEventData data)
        {
            var pointerEvent = data.buttonData;
            var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

            // PointerDown notification
            if (data.PressedThisFrame())
            {
                pointerEvent.eligibleForClick = true;
                pointerEvent.delta = Vector2.zero;
                pointerEvent.dragging = false;
                pointerEvent.useDragThreshold = true;
                pointerEvent.pressPosition = pointerEvent.position;
                pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

                DeselectIfSelectionChanged(currentOverGo, pointerEvent);

                // search for the control that will receive the press
                // if we can't find a press handler set the press
                // handler to be what would receive a click.
                var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

                // didnt find a press handler... search for a click handler
                if (newPressed == null)
                    newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

                // Debug.Log("Pressed: " + newPressed);

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
            }

            // PointerUp notification
            if (data.ReleasedThisFrame())
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



        #region Nested Classes

        /// <summary>
        /// Adapted from: https://bitbucket.org/Unity-Technologies/ui/src/fd5d3578da8c33883b7c56dd2b2f4d4bbc87095b/UnityEngine.UI/EventSystem/InputModules/PointerInputModule.cs?at=4.6
        /// </summary>
        protected class HandState
        {
            private List<FingerState> m_TrackedFingers = new List<FingerState>();

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
                    m_TrackedFingers.Add(tracked);
                }
                return tracked;
            }

            public void SetFingerState(FingerType finger, PointerEventData.FramePressState stateForMouseButton, PointerEventData data)
            {
                var toModify = GetFingerState(finger);
                toModify.eventData.buttonState = stateForMouseButton;
                toModify.eventData.buttonData = data;
            }
        }

        protected class FingerState
        {
            public MouseButtonEventData eventData
            {
                get;
                set;
            }

            public FingerType finger
            {
                get;
                set;
            }

            public GameObject pressedObject { get; set; }
            public GameObject selectedObject { get; set; }

        }


        #endregion
    }

}