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

        public abstract GenericHand GetHand(HandType type,
            NoHandStrategy strategy = NoHandStrategy.SetNull);


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
