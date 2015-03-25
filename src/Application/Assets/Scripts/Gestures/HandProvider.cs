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
        private HashSet<GestureEventData> activeGestures = new HashSet<GestureEventData>();

        public abstract GenericHand GetHand(HandType type,
            NoHandStrategy strategy = NoHandStrategy.SetNull);

        public GestureEventData[] GetGestures()
        {
            return activeGestures.ToArray();
        }

        protected virtual void Update()
        {
            activeGestures.Clear();
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
            activeGestures.Add(eventData);
        }

        public void OnGestureEnter(GestureEventData eventData)
        {
            activeGestures.Add(eventData);
        }

        public void OnGestureLeave(GestureEventData eventData)
        {
            activeGestures.Add(eventData);
        }

        public void OnGestureMaintain(GestureEventData eventData)
        {
            activeGestures.Add(eventData);
        }
    }
}
