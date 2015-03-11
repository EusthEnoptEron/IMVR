using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace Gestures
{

    /// <summary>
    /// Handles gestures and hand positions
    /// </summary>
    public abstract class Gesture : MonoBehaviour, IGesture
    {
        public abstract int BacktrackLength { get; }
        public abstract string Name { get; }
        public abstract bool Stateful { get; }

        protected abstract bool Process(GenericHand hand, LinkedList<GenericHand> cache, GestureState state);
        protected virtual bool Process(HandPair pair, LinkedList<HandPair> cache, GestureState state)
        {
            return false;
        }

        LinkedList<GenericHand>[] handCache;
        GestureState[] states;
        LinkedList<HandPair> handPairCache;
        GestureState pairState;

        protected virtual void Awake()
        {
            handCache = new LinkedList<GenericHand>[] {
                new LinkedList<GenericHand>(),
                new LinkedList<GenericHand>()
            };
            states = new GestureState[2];

            handPairCache = new LinkedList<HandPair>();
        }

        public GestureState Handle(GenericHand baseHand)
        {
            if (Filter(baseHand))
            {
                var hand = Transform(baseHand);
                var cache = handCache[GetCachePosition(baseHand)];
                var state = states[GetCachePosition(baseHand)];
                Push(cache, hand);

                if (cache.Count == BacktrackLength && Process(hand, cache, state))
                {
                    if (state == GestureState.Enter) state = GestureState.Maintain;
                    else if(state != GestureState.Maintain)
                    {
                        if (Stateful) state = GestureState.Enter;
                        else state = GestureState.Execute;
                    }
                }
                else
                {
                    Leave(ref state);
                }

                // save back
                states[GetCachePosition(baseHand)] = state;
                return state;
            }
            return GestureState.None;
        }

        public GestureState Handle(GenericHand baseLeftHand, GenericHand baseRightHand)
        {
            if (Filter(baseLeftHand, baseRightHand))
            {
                var leftHand = Transform(baseLeftHand);
                var rightHand = Transform(baseRightHand);
                var handPair = new HandPair(leftHand, rightHand);
                var state = pairState;
                Push(handPairCache, handPair);

                if (handPairCache.Count == BacktrackLength && Process(handPair, handPairCache, state))
                {
                    if (state == GestureState.Enter) state = GestureState.Maintain;
                    else {
                        if (Stateful) state = GestureState.Enter;
                        else state = GestureState.Execute;
                    }
                }
                else
                {
                    Leave(ref state);
                }
                return state;
            }
            return GestureState.None;
        }

        private void Leave(ref GestureState state)
        {
            if (state == GestureState.Maintain || state == GestureState.Enter)
            {
                state = GestureState.Leave;
            }
            else
            {
                state = GestureState.None;
            }
        }

        protected virtual bool Filter(GenericHand hand)
        {
            return true;
        }

        protected virtual bool Filter(GenericHand leftHand, GenericHand rightHand)
        {
            return false;
        }

        protected virtual GenericHand Transform(GenericHand hand)
        {
            return hand;
        }

        private void Push<T>(LinkedList<T> cache, T hand)
        {
            cache.AddFirst(hand);

            while (cache.Count > BacktrackLength) cache.RemoveLast();
        }



        /// <summary>
        /// This is called before the transform (so that left and right hands can still be told apart even if they are made equal later)
        /// </summary>
        /// <param name="hand"></param>
        /// <returns></returns>
        protected virtual int GetCachePosition(GenericHand hand)
        {
            int pos = hand.IsLeft ? 0 : 1;
            return pos;
        }

        //public void OnLeftHand(ICollection<GenericHand> history)
        //{
        //    Debug.Log(String.Format("{0} -> {1}", history.Count, BacktrackCount));
        //    if (history.Count >= BacktrackCount)
        //    {
        //        // Get first n entries
        //        var myHistory = history.Where((h, i) => i < BacktrackCount);
        //        HandleLeftHand(myHistory.First(), myHistory);
        //        HandleHand(myHistory.First(), myHistory);
        //    }
        //}

        //public void OnRightHand(ICollection<GenericHand> history)
        //{
        //    if (history.Count >= BacktrackCount)
        //    {
        //        // Get first n entries
        //        var myHistory = history.Where((h, i) => i < BacktrackCount);
        //        HandleRightHand(myHistory.First(), myHistory);
        //        HandleHand(myHistory.First(), myHistory);
        //    }
        //}

        //public void OnBothHands(ICollection<HandPair> history)
        //{
        //    if (history.Count >= BacktrackCount)
        //    {
        //        // Get first n entries
        //        var myHistory = history.Where((h, i) => i < BacktrackCount);
        //        HandleBothHands(myHistory.First().LeftHand, myHistory.First().RightHand, myHistory);
        //    }
        //}

        protected GenericHand[] LoadGesture(string name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string path = Application.dataPath + "/Snapshots/"+name+".bin";
            GenericHand[] hands;

            using (var stream = File.OpenRead(path))
            {
                hands = bf.Deserialize(stream) as GenericHand[];
            }
            return hands;
        }
    }
}
