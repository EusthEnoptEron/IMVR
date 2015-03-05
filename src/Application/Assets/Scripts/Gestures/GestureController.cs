using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace Gestures
{

    public class GestureController : MonoBehaviour
    { 
        public HandProvider provider;
        private Dictionary<Gesture, float> tracker = new Dictionary<Gesture, float>();
        public float minInterval = 0.5f;

        // Use this for initialization
        void Awake()
        {
            if(provider == null)
                provider = GetComponent<HandProvider>() ?? GetComponentInParent<HandProvider>();
        }

        // Update is called once per frame
        void Update()
        {
            GenericHand leftHand = provider.LeftHand;
            GenericHand rightHand = provider.RightHand;
            HandPair bothHands = null;
            if (leftHand != null && rightHand != null)
            {
                bothHands = new HandPair(leftHand, rightHand);
            }


            ExecuteEvents.Execute<IGesture>(gameObject, null, (gesture, data) =>
            {
                TestForGesture(gesture, leftHand);
                TestForGesture(gesture, rightHand);
                TestForGesture(gesture, bothHands);
            });
        }


        private void TestForGesture(IGesture gesture, GenericHand hand)
        {
            if (hand == null) return;

            var state = gesture.Handle(hand);
            var eventData = makeEvent((Gesture)gesture, state);
            eventData.UseBothHands = false;
            eventData.Hand = hand;

            triggerEvent(eventData);
        }

        private void TestForGesture(IGesture gesture, HandPair pair)
        {
            if (pair == null) return;

            var state = gesture.Handle(pair.LeftHand, pair.RightHand);
            var eventData = makeEvent((Gesture)gesture, state);
            eventData.UseBothHands = true;
            eventData.HandPair = pair;
          
            triggerEvent(eventData);
        }

        private GestureEventData makeEvent(Gesture gesture, GestureState state)
        {
            var eventData = new GestureEventData(EventSystem.current, state, gesture);
            return eventData;
        }

        private void triggerEvent(GestureEventData data)
        {
            // TODO: fix ExecuteHierarchy
            switch (data.State)
            {
                case GestureState.Enter:
                    ExecuteEvents.ExecuteHierarchy<IStatefulGestureHandler>(gameObject, data, (h, d) => { h.OnGestureEnter((GestureEventData)d); });
                    break;
                case GestureState.Leave:
                    ExecuteEvents.ExecuteHierarchy<IStatefulGestureHandler>(gameObject, data, (h, d) => { h.OnGestureLeave((GestureEventData)d); });
                    break;
                case GestureState.Maintain:
                    ExecuteEvents.ExecuteHierarchy<IStatefulGestureHandler>(gameObject, data, (h, d) => { h.OnGestureMaintain((GestureEventData)d); });
                    break;
                case GestureState.Execute:
                    if (!tracker.ContainsKey(data.Gesture) || (Time.time - tracker[data.Gesture]) > minInterval)
                    {
                        tracker[data.Gesture] = Time.time;
                        ExecuteEvents.ExecuteHierarchy<IGestureHandler>(gameObject, data, (h, d) => { h.OnGesture((GestureEventData)d); });
                    }
                    break;
            }
        }


        public void OnGestureEnter(GestureEventData eventData)
        {
            Debug.Log("GESTURE ENTER");
        }

        public void OnGestureLeave(GestureEventData eventData)
        {
            Debug.Log("GESTURE LEAVE");
        }

        public void OnGesture(GestureEventData eventData)
        {
            Debug.Log("GESTURE");
        }


        public void OnGestureMaintain(GestureEventData eventData)
        {
        }
    }
}