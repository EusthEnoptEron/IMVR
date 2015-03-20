using UnityEngine;
using System.Collections;
using System.Linq;

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

    public abstract class HandProvider : MonoBehaviour
    {
        private static HandProvider _instance;

        public abstract GenericHand GetHand(HandType type,
            NoHandStrategy strategy = NoHandStrategy.SetNull);

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

    }
}
