using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace Gestures {

    public enum HandType
    {
        Left,
        Right
    }


    public enum NoHandStrategy
    {
        SetNull,
        Keep,
        DelayedSetNull
    }

    public abstract class HandProvider : MonoBehaviour, IGestureHandler, IStatefulGestureHandler
    {
        private static HandProvider _instance;
        //private HashSet<GestureEventData> activeGestures = new HashSet<GestureEventData>();
        private Dictionary<string, GestureEventData> backBuffer = new Dictionary<string, GestureEventData>();
        private Dictionary<string, GestureEventData> activeGestures = new Dictionary<string, GestureEventData>();

        public abstract GenericHand GetHand(HandType type,
            NoHandStrategy strategy = NoHandStrategy.SetNull);

        public GestureEventData[] GetGestures()
        {
            return activeGestures.Values.ToArray();
        }

        public GestureEventData GetGestureEvent(string name)
        {
            return activeGestures.ContainsKey(name)
                ? activeGestures[name] 
                : null;
        }

        public bool GetGesture(string name)
        {
            return activeGestures.ContainsKey(name);
        }

        public bool GetGestureEnter(string name)
        {
            var gesture = GetGestureEvent(name);
            if (gesture != null && gesture.State != GestureState.Enter)
                gesture = null;

            return gesture != null;
        }

        public bool GetGestureExit(string name)
        {
            var gesture = GetGestureEvent(name);
            if (gesture != null && gesture.State != GestureState.Leave)
                gesture = null;

            return gesture != null;
        }

        protected virtual void LateUpdate()
        {
            var temp = activeGestures;
            activeGestures = backBuffer;

            backBuffer = temp;
            backBuffer.Clear();
        }


        public static HandProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    var holder = GameObject.FindGameObjectWithTag("HandController");
                    if (holder != null)
                    {
                        _instance = holder.GetComponent<HandProvider>();
                    }

                    if (_instance == null)
                    {
                        Debug.LogError("No Hand Provider in scene! (did you set the tag?)");
                    }

                }
                return _instance;
            }
        }

        protected virtual void OnLevelWasLoaded(int level)
        {
            _instance = null;
        }

#region legacy

        public abstract GenericHand LeftHand
        {
            get;
        }

        public abstract GenericHand RightHand
        {
            get;
        }


        public virtual GenericHand[] Hands
        {
            get
            {
                return new GenericHand[] { LeftHand, RightHand }.Where(h => h != null).ToArray();
            }
        }
#endregion


        public void OnGesture(GestureEventData eventData)
        {
            backBuffer[eventData.Gesture.Name] = eventData;
        }

        public void OnGestureEnter(GestureEventData eventData)
        {
            backBuffer[eventData.Gesture.Name] = eventData;
        }

        public void OnGestureLeave(GestureEventData eventData)
        {
            backBuffer[eventData.Gesture.Name] = eventData;
        }

        public void OnGestureMaintain(GestureEventData eventData)
        {
            if(!backBuffer.ContainsKey(eventData.Gesture.Name))
                backBuffer[eventData.Gesture.Name] = eventData;
        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnEnable()
        {
            
        }

    }
}
